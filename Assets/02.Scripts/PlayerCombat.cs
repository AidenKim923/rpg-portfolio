using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Minsung.AttackSO;
using Minsung.Weapon;
using Minsung.Player;
using UnityEngine.Animations;
using Minsung.UTIL;

namespace Minsung.PlayerCombat
{
    public class PlayerCombat : MonoBehaviour
    {
        /**************************************************************
         * 
         *                  Field
         * 
         **************************************************************/
        #region Field
        [SerializeField] private List<AttackSO.AttackSO> m_asoCombo;
        [SerializeField] private Weapon.Weapon m_weapon;

        private PlayerCtrl m_playerCtrl;

        public delegate void PlayerAttackHandler();
        public static event PlayerAttackHandler OnPlayerAttack;

        private float m_fLastClickedTime;
        private float m_fLastComboEnd;
        private int m_nComboCounter;
        private int m_nCurrentComboIndex;

        private Animator m_anim;
        private Rigidbody m_rigid;

        public enum AttackCount
        {
            ONE = 1,
            TWO,
            THREE,
            FOUR,
            END

        }

        private readonly int[] hashAttack = new int[] { Animator.StringToHash("DoAttack1"), Animator.StringToHash("DoAttack1"), Animator.StringToHash("DoAttack2")
                                                              ,Animator.StringToHash("DoAttack3"), Animator.StringToHash("DoAttack4") };

        [Header("공격 관련")]
        [SerializeField] private float m_fAttackMovementPower = 3.0f;
        private int m_nAttackCount = 0;
        [SerializeField, Tooltip("공격콤보 초기화 시간")]
        private float m_fAttackInitTime = 1.5f;
        [SerializeField, Tooltip("기본공격 딜레이")]
        private float m_fAttackDealy = 0.2f;

        private Coroutine m_CoAttackTimeCheck;
        private Coroutine m_CoAttackMovement;

        private AttackCount m_IATKCNT = AttackCount.ONE;
        /// <summary> 공격콤보 초기화 시간과 비교하며 판단할 변수 </summary>
        private float m_fAttackRunTime = 0.0f;

        



        #endregion

        /**************************************************************
         * 
         *                  Unity Event
         * 
         **************************************************************/
        #region Unity Event
        private void Awake()
        {
            m_anim = GetComponent<Animator>();
            m_playerCtrl = GetComponent<PlayerCtrl>();
            m_rigid = GetComponent<Rigidbody>();

        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

            if (Input.GetButtonDown("Fire1"))
            {
                Attack();

            }
        }

        #endregion

        /**************************************************************
         * 
         *                  Method
         * 
         **************************************************************/
        #region Method

        private void AttackLookAt()
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, m_playerCtrl.GetClickableLayer()))
            {
                Vector3 pos = new Vector3(hit.point.x, transform.position.y, hit.point.z);
                transform.LookAt(pos);
            }
        }

        private void AttackComboAction()
        {
            m_anim.runtimeAnimatorController = m_asoCombo[m_nComboCounter].m_animatorOv;
            m_anim.Play("Attack", 0, 0);
            m_weapon.m_fDamage = m_asoCombo[m_nComboCounter].m_fDamage;
            m_nComboCounter++;
            m_fLastClickedTime = Time.time;

            Debug.Log("Attack");

            if (m_nComboCounter >= m_asoCombo.Count)
            {
                m_nComboCounter = 0;
            }
        }

        Coroutine coAttackEnd = null;

        public void AttackEnd()
        {
            if (coAttackEnd == null)
            {
                coAttackEnd = StartCoroutine(CheckEndCombo());
            }
        }

        private IEnumerator CheckEndCombo()
        {
            float time = 0.0f;
            float endTime = 1.0f;
            while (time < endTime && !m_playerCtrl.m_bIsAttacking)
            {
                time += Time.deltaTime;

                yield return null;
            }

            m_nComboCounter = 0;
        }

        /// <summary> 플레이어 모든 공격관련 함수. </summary>
        private void PlayerAttack()
        {
            //공격 중이 아니거나, 대미지 입는 중이 아닐 때
            if (!(m_playerCtrl.m_bIsAttacking))
            {
                //m_playerStatMgr.SetTotalCombo(++m_nTotalCombo);
                m_playerCtrl.m_bIsAttacking = true;
                m_fAttackRunTime = 0;

                //기본적으로 공격들은 1콤보의 대미지를 가지게 된다.
                //기본공격은 따로 수
                //공격력 수정하는 함수.. 콤보마다 대미지가 다름
                //SetAddSTR((int)AttackCount.ONE * 5);

                if (m_CoAttackTimeCheck == null)
                {
                    m_CoAttackTimeCheck = StartCoroutine(CheckAttackInitTime());
                }

                Attack();
            }
        }

        /// <summary> 공격 초기화 시간을 재는 코루틴 함수 </summary>
        private IEnumerator CheckAttackInitTime()
        {
            float t = 0.0f;

            while (true)
            {
                if (m_fAttackRunTime < m_fAttackInitTime)
                {
                    if (!(m_playerCtrl.m_bIsAttacking || m_IATKCNT == AttackCount.ONE))
                    {
                        m_fAttackRunTime += Time.deltaTime;
                    }
                    t = m_fAttackRunTime / m_fAttackInitTime;
                }
                else
                {
                    Attack_Init();
                }
                yield return null;
            }

            void Attack_Init()
            {
                StopCoroutine(m_CoAttackTimeCheck);
                m_CoAttackTimeCheck = null;
                m_fAttackRunTime = 0;
                m_nAttackCount = 0;
                m_IATKCNT = AttackCount.ONE;
            }
        }

        private void Attack()
        {
            //m_weaponCtr.Use(); //무기 콜라이더 활성화 / 비활성화 루틴
            //기본공격의 시간이 공격초기화 시간보다 길어진다면 콤보 x
            Util.CheckRunCoroutine(ref m_CoAttackMovement, StartCoroutine(AttackMovement(m_fAttackMovementPower)), this);
            //CheckRunCoroutine(m_CoAttackDelayCheck, StartCoroutine(AttackDelay(m_fAttackDealy)));

            // Util.SetAddSTR((int)m_IATKCNT * 5);

            m_anim.SetTrigger(hashAttack[(int)m_IATKCNT]);

            ++m_IATKCNT;
            if (m_IATKCNT >= AttackCount.END) m_IATKCNT = AttackCount.ONE;

            m_nAttackCount++;
        }


        /// <summary>
        /// 어택시 앞으로 나가는 함수... 
        /// </summary>
        /// <param name="_movementPower"></param>
        /// <returns></returns>
        public IEnumerator AttackMovement(float _movementPower)
        {
            m_rigid.velocity = Vector3.zero;
            m_rigid.angularVelocity = Vector3.zero;

            m_rigid.AddForce(m_rigid.transform.forward * _movementPower, ForceMode.Impulse);

            yield return new WaitForSeconds(0.2f);

            m_rigid.velocity = Vector3.zero;
            m_rigid.angularVelocity = Vector3.zero;
        }
        #endregion
    }
}

