using System.Collections;
using System.Collections.Generic;
using Minsung.Weapon;
using Minsung.UTIL;
using Minsung.PLAYERSKILLMANAGER;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;
using Minsung.SKILLDATA;
using UnityEngine.UIElements.Experimental;


namespace Minsung.PLAYER
{
    public class PlayerCtrl : MonoBehaviour
    {
        /**************************************************************
        * 
        *                  Field
        * 
        **************************************************************/
        #region Field

        private CustomAction m_input;
        private GameObject m_goCurrentClickEffect;
        private Animator m_anim;
        private NavMeshAgent m_agent;
        public PlayerSkillManager m_PlayerSkillMgr;

        [Header("Movement")]
        [SerializeField] private GameObject m_goClickEffect;
        [SerializeField] private LayerMask m_clickableLayers;
        [SerializeField] private float m_fLastClickTime;
        [SerializeField] private float m_fClickDelay = 0.2f;
        [SerializeField] private float m_fClickDestroyTime = 1.0f;
        [SerializeField] private float m_fRotSpeed = 50.0f;

        private bool m_bisClick = false;
        private bool m_bisWater;
        private PlayerState m_ePlayerState = PlayerState.IDLE;
        private readonly int m_hashWater = Animator.StringToHash("IsWater");

        public Minsung.Weapon.Weapon m_weapon;



        private readonly int[] m_arrHashAttack = new int[] { Animator.StringToHash("DoAttack1"), Animator.StringToHash("DoAttack2")
                                                              ,Animator.StringToHash("DoAttack3"), Animator.StringToHash("DoAttack4") };

        [Header("공격 관련")]
        [SerializeField] private float m_fAttackMovementPower = 3.0f;
        public bool m_bIsAttacking = false;
        private int m_nAttackIndex = 0;
        [SerializeField, Tooltip("공격콤보 초기화 시간")]
        private float m_fAttackInitTime = 1.5f;
        [SerializeField, Tooltip("기본공격 딜레이")]
        private float m_fAttackDelay = 0.2f;
        private int m_nMaxAttackIndex = 4;

        private Coroutine m_CoAttackInitTime;
        private Coroutine m_CoAttackComboDelay;

        /// <summary> 공격콤보 초기화 시간과 비교하며 판단할 변수 </summary>
        private float m_fAttackRunTime = 0.0f;
        private WaitForSeconds m_wfsAttackInitSeconds;
        private WaitForSeconds m_wfsAttackDelay;

        public SkillData curSkill;
        public int m_nSkillIndex;
        #endregion

        /**************************************************************
        * 
        *                  Unity Event
        * 
        **************************************************************/
        #region Unity Event
        void Awake()
        {
            Init_GetComponent();
            m_input = new CustomAction();

            AssignInputs();

            m_agent.speed = 5.0f;
            m_weapon = GetComponentInChildren<Minsung.Weapon.Weapon>();

            m_wfsAttackInitSeconds = new WaitForSeconds(m_fAttackInitTime);
            m_wfsAttackDelay = new WaitForSeconds(m_fAttackDelay);

            m_nMaxAttackIndex = m_arrHashAttack.Length;

        }

        void OnEnable()
        {
            m_input.Enable();
        }

        private void Start()
        {
            StartCoroutine(UpdatePlayerState());
            StartCoroutine(CheckPlayterState());
        }

        private void OnTriggerEnter(Collider _coll)
        {
            if (_coll.gameObject.layer == LayerMask.NameToLayer("Water"))
            {
                m_bisWater = true;
            }
        }

        private void OnTriggerExit(Collider _coll)
        {
            if (_coll.gameObject.layer == LayerMask.NameToLayer("Water"))
            {
                m_bisWater = false;
            }
        }

        void Update()
        {
            MoveForMousePos();
            if (Input.GetButtonDown("Fire1"))
            {
                Attack();
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                SkillAttack(1);
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                SkillAttack(2);
            }
        }

        private void LateUpdate()
        {
            if (m_agent.enabled == true)
            {
                SmoothRotation();
            }

        }


        void OnDisable()
        {
            m_input.Disable();
        }

        #endregion

        /**************************************************************
         * 
         *                  Public Method
         * 
         **************************************************************/

        #region Public Method

        /// <summary>PlayerCombat에서 click layer얻을수 있게 리턴</summary>
        /// <returns>m_clickableLayers</returns>
        public LayerMask GetClickableLayer() => m_clickableLayers;

        /// <summary> PlayerCombat.cs 참조해서 실행 </summary>
        public void AttackColliderOn()
        {
            m_weapon.AttackStart();
        }

        /// <summary> PlayerCombat.cs 참조해서 실행 </summary>
        public void AttackColliderOff()
        {
            m_weapon.AttackEnd();
        }

        public void SetPlayerSkillManager(PlayerSkillManager _playerSkillManager)
        {
            m_PlayerSkillMgr = _playerSkillManager;
            if (m_PlayerSkillMgr == null)
            {
                Debug.LogError("PlayerSkillManager is Null");
            }
        }

        #endregion

        /**************************************************************
         * 
         *                  Private Method
         * 
         **************************************************************/

        #region Init
        private void Init_GetComponent()
        {
            m_agent = GetComponent<NavMeshAgent>();
            m_anim = GetComponent<Animator>();
            // m_PlayerSkillMgr = this.gameObject.GetComponent<PlayerSkillManager>();

            m_agent.updateRotation = false;
        }

        #endregion

        #region private
        private void AssignInputs()
        {
            m_input.Main.Move.started += ctx => ClickStart();
            m_input.Main.Move.canceled += ctx => ClickEnd();
        }

        /// <summary> 클릭시작 (Input System)</summary>
        private void ClickStart()
        {
            // 공격중이 아닐때만 이동을 시작하도록 변경
            if (!m_bIsAttacking)
            {
                m_bisClick = true;
                MoveForMousePos();
            }
            // 이동중에 공격을 시작을 한 경우 이동을 멈추도록 변경
            else
            {
                m_bisClick = false;
                m_agent.isStopped = true;
            }
        }

        /// <summary> 클릭종료 (Input System)</summary>
        private void ClickEnd()
        {
            if (!m_bIsAttacking)
            {
                m_bisClick = false;
                m_agent.isStopped = false; // 클릭 종료시 다시 이동 시작
            }
        }

        /// <summary> Mouse 위치에 따른 Navgation Destination 설정 </summary>
        private void MoveForMousePos()
        {
            if (m_bisClick && m_agent.enabled == true)
            {
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, m_clickableLayers))
                {
                    // 마지막 클릭시간과 현재시간을 비교하여 클릭이펙트를 딜레이를 적용
                    if (Time.time - m_fLastClickTime >= m_fClickDelay)
                    {
                        m_agent.destination = hit.point;
                        if (m_goClickEffect != null)
                        {
                            if (m_goCurrentClickEffect != null)
                            {
                                Destroy(m_goCurrentClickEffect, m_fClickDestroyTime);
                            }

                            m_goCurrentClickEffect = Instantiate(m_goClickEffect, hit.point + new Vector3(0, 0.1f, 0), m_goClickEffect.transform.rotation);
                            m_fLastClickTime = Time.time;

                            Destroy(m_goCurrentClickEffect, m_fClickDestroyTime);
                        }
                    }

                }
            }
        }

        /// <summary> AI의 회전 처리 </summary>
        private void SmoothRotation()
        {
            if (m_agent.remainingDistance >= 1.0f)
            {
                //에이전트의 이동 방향
                Vector3 direction = m_agent.desiredVelocity;

                //회전 각도(쿼터니언) 산출
                if (direction != Vector3.zero)
                {
                    Quaternion beforeRot = Quaternion.LookRotation(direction);

                    //구면 선형보간 함수로 부드러운 회전 처리
                    transform.rotation = Quaternion.Slerp(transform.rotation, beforeRot, Time.fixedDeltaTime * m_fRotSpeed);
                }
            }
        }

        /// <summary>플레이어 상태 업데이트</summary>
        private IEnumerator UpdatePlayerState()
        {
            while (true)
            {
                if (m_agent.enabled == true)
                {
                    if (m_agent.remainingDistance <= 0.05f)
                    {
                        m_ePlayerState = PlayerState.IDLE;
                    }
                    else
                    {
                        if (m_bisWater)
                        {
                            m_ePlayerState = PlayerState.SWIM;
                        }
                        else
                        {
                            m_ePlayerState = PlayerState.RUN;
                        }
                    }
                }
                yield return null;
            }
        }

        /// <summary>플레이어 상태에 따른 이벤트 실행</summary>
        private IEnumerator CheckPlayterState()
        {
            while (true)
            {
                switch (m_ePlayerState)
                {
                    case PlayerState.IDLE:
                        MovementAnim(false);
                        break;

                    case PlayerState.RUN:
                        MovementAnim(true);
                        m_agent.speed = 5.0f;
                        m_anim.SetBool(m_hashWater, false);
                        break;

                    case PlayerState.SWIM:
                        MovementAnim(true);
                        m_bisWater = true;
                        m_agent.speed = 2.0f;
                        m_anim.SetBool(m_hashWater, true);

                        break;
                }
                yield return null;
            }
        }

        /// <summary>플레이어의 움직임 에니메이션</summary>
        /// <param name="isMovement">움직임이 있다면</param>
        private void MovementAnim(bool isMovement = false)
        {
            if (!isMovement)
            {
                m_anim.SetFloat("Movement", 0);
            }
            else
            {
                m_anim.SetFloat("Movement", m_agent.velocity.magnitude);
            }
        }

        #region Attack

        private void Attack()
        {
            if (!m_bIsAttacking)
            {
                m_bIsAttacking = true;
                Util.RunCoroutine(ref m_CoAttackInitTime, StartCoroutine(AttackInitDelaySeconds()), this);
                Util.RunCoroutine(ref m_CoAttackComboDelay, StartCoroutine(AttackComboDelay()), this);


                m_anim.SetTrigger(m_arrHashAttack[m_nAttackIndex]);

                m_nAttackIndex++;
                if (m_nAttackIndex >= m_nMaxAttackIndex)
                {
                    m_nAttackIndex = 0;
                }

            }
        }

        private IEnumerator AttackInitDelaySeconds()
        {
            float t = 0.0f;
            float time = 0.0f;
            float destTime = m_fAttackInitTime;

            while (time < 1.0f)
            {
                t += Time.deltaTime;
                time = t / destTime;

                yield return null;
            }
            m_nAttackIndex = 0;
        }

        private IEnumerator AttackComboDelay()
        {
            yield return m_wfsAttackDelay;
            m_bIsAttacking = false;
        }

        /// <summary> Player의 스킬 공격 
        /// <para/> SKill Manager의 플레이어 스킬Index 를 지닌
        /// <para/> skill_Datas[] 배열 이용
        /// </summary>
        protected void SkillAttack(int skillNum)
        {

            if (true)
            {
                //현재 사용할 스킬. 0번째 부터 시작함.
                m_nSkillIndex = skillNum - 1;
                Debug.Log($"{m_nSkillIndex} 번째 스킬 사용");
                curSkill = m_PlayerSkillMgr.m_lstPlayerSkillData[m_nSkillIndex];


                //획득하지 않은 상태면
                if (curSkill == null) return;
                //보유 마나보다 스킬 마나가 크다면 공격 중단.
                //    if (m_nCurMP < curSkill.GetSkillManaAmount()) yield break;

                //사용 가능 상태가 아니면
                if (curSkill.IsOnCooltime)
                {
                    Debug.Log("스킬 쿨타임 중");
                    return;
                }

                StartCoroutine(CheckCoolTime(curSkill));

                m_bIsAttacking = true;

                m_anim.SetTrigger(curSkill.GetAnimHash());

                m_bIsAttacking = false;
            }

        }

        public IEnumerator CheckCoolTime(SkillData _skillData)
        {
            _skillData.IsOnCooltime = true;
            yield return new WaitForSeconds(_skillData.SkillCoolTime);
            _skillData.IsOnCooltime = false;
        }

        #endregion

        #endregion
    }

}
