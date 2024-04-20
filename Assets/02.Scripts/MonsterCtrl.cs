using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

namespace Minsung.Monster
{
    public class MonsterCtrl : MonoBehaviour
    {
        /**************************************************************
        * 
        *                  Field
        * 
        **************************************************************/
        #region Field
        [SerializeField] private MonsterState m_eMonsterState = MonsterState.IDLE;

        [Header("Monster")]
        [SerializeField] private float m_fTraceDistance    = 10.0f;
        [SerializeField] private float m_fAttackDistance   = 2.0f;
        [SerializeField] private float m_fAttackDelayTime  = 5.0f;
        [SerializeField] private bool  m_bIsDie            = false;
        [SerializeField] private bool  m_bIsAttack         = false;

        private Transform monsterTr;
        private Transform playerTr;
        private NavMeshAgent m_agent;

        private Animator m_anim;
        private readonly int hashTrace          = Animator.StringToHash("IsTrace");
        private readonly int hashAttack         = Animator.StringToHash("DoAttack");
        private readonly int hashDie            = Animator.StringToHash("IsDie");
        private readonly int hashCycleOffset    = Animator.StringToHash("CycleOffset");

        private Coroutine coCheckAttack;

        
        #endregion

        /**************************************************************
         * 
         *                  Unity Event
         * 
         **************************************************************/
        #region Unity Event
        void Awake()
        {
            monsterTr = GetComponent<Transform>();
            GameObject goPlayer = GameObject.FindGameObjectWithTag("Player");
            Debug.Assert(goPlayer, "플레이어 오브젝트를 못찾았습니다.");

            playerTr = goPlayer?.GetComponent<Transform>();
            m_agent = GetComponent<NavMeshAgent>();
            m_anim = GetComponent<Animator>();
        }

        private void Start()
        {
            m_anim.SetFloat(hashCycleOffset, Random.Range(0.0f, 1.0f));

            StartCoroutine(CheckMonsterState());
            StartCoroutine(MonsterAction());
        }

        void OnEnable()
        {

        }


        private void OnTriggerEnter(Collider _coll)
        {
            if (_coll.gameObject.CompareTag("PlayerMeleeAttack"))
            {
                Debug.Log("몬스터 피격");
                m_eMonsterState = MonsterState.HITTED;
            }
        }

        private void OnTriggerExit(Collider _coll)
        {

        }

        void Update()
        {
            
        }

        private void LateUpdate()
        {

        }

        void OnDisable()
        {

        }

        #endregion

        /**************************************************************
         * 
         *                  Private Method
         * 
         **************************************************************/


        #region private
        private IEnumerator CheckMonsterState()
        {
            while (!m_bIsDie)
            {
                yield return new WaitForSeconds(0.3f);

                float distance = Vector3.Distance(monsterTr.position, playerTr.position);

                if (distance <= m_fAttackDistance)
                {
                    m_eMonsterState = MonsterState.ATTACK;
                }
                else if (distance <= m_fTraceDistance)
                {
                    m_eMonsterState = MonsterState.TRACE;
                }
                else
                {
                    m_eMonsterState = MonsterState.IDLE;
                }
            }
        }

        private IEnumerator MonsterAction()
        {
            while (!m_bIsDie)
            {
                switch (m_eMonsterState)
                {
                    case MonsterState.IDLE:
                        // 추적 중지
                        m_agent.isStopped = true;
                        m_anim.SetBool(hashTrace, false);
                        break;

                    case MonsterState.TRACE:
                        // 추적 대상의 좌표로 이동 시작
                        m_agent.isStopped = false;
                        m_agent.destination = playerTr.position;

                        m_anim.SetBool(hashTrace, true);
                        break;

                    case MonsterState.ATTACK:
                        m_agent.isStopped = true;
                        if (coCheckAttack == null)
                        {
                            coCheckAttack = StartCoroutine(CheckIsAttack());
                        }
                        break;

                    case MonsterState.DIE:
                        m_bIsDie = true;
                        break;
                }

                yield return new WaitForSeconds(0.3f);
            }

        }

        private IEnumerator CheckIsAttack()
        {
            m_bIsAttack = true;
            m_anim.SetTrigger(hashAttack);

            yield return new WaitForSeconds(m_fAttackDelayTime);
            m_bIsAttack = false;
            StopCoroutine(coCheckAttack);
            coCheckAttack = null;

        }

        private void OnDrawGizmos()
        {
            if (m_eMonsterState == MonsterState.TRACE)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(monsterTr.position, m_fTraceDistance);
            }
            if (m_eMonsterState == MonsterState.ATTACK)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(monsterTr.position, m_fAttackDistance);
            }
        }

        #endregion
    }
}
