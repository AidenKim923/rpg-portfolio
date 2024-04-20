using System.Collections;
using System.Collections.Generic;
using Minsung.Weapon;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;


namespace Minsung.Player
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

        [Header("Movement")]
        [SerializeField] private GameObject     m_goClickEffect;
        [SerializeField] private LayerMask      m_clickableLayers;
        [SerializeField] private float          m_fLastClickTime;
        [SerializeField] private float          m_fClickDelay        = 0.2f;
        [SerializeField] private float          m_fClickDestroyTime  = 1.0f;
        [SerializeField] private float          m_fRotSpeed          = 50.0f;

        private bool m_bisClick = false;
        private bool m_bisWater;
        private PlayerState m_ePlayerState = PlayerState.IDLE;
        private readonly int m_hashWater = Animator.StringToHash("IsWater");

        public bool  m_bIsAttacking = false;
        public Minsung.PlayerCombat.PlayerCombat playerCombat;
        public Minsung.Weapon.Weapon m_weapon;

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
            playerCombat = GetComponent<Minsung.PlayerCombat.PlayerCombat>();
        }

        private void Start()
        {
            StartCoroutine(UpdatePlayerState());
            StartCoroutine(CheckPlayterState());

            Minsung.PlayerCombat.PlayerCombat.OnPlayerAttack += AttackStart;
        }

        void OnEnable()
        {
            m_input.Enable();
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

        public void AttackStart()
        {
            m_bIsAttacking = true;
            m_bisClick = false;
            m_agent.enabled = false;
            Debug.Log("Playerctrl AttackStart");

        }

        public void AttackEnd()
        {
            m_bIsAttacking = false;
            m_agent.enabled = true;
            playerCombat.AttackEnd();
            //여기서 콤보 종료 시간
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
        #endregion
    }

}
