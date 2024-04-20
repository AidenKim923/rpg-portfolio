using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Minsung.UTIL;

public class inhwan : MonoBehaviour
{
    private bool m_bisAttack;
    private bool m_bisAttackDelay;

    public Animator m_anim;
    public Rigidbody m_rigid;

    private void Awake()
    {
        m_anim = GetComponent<Animator>();
        m_rigid = GetComponent<Rigidbody>();
    }



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
    [SerializeField] private float m_fAttackMovementPower = 30.0f;
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

    /// <summary> 플레이어 모든 공격관련 함수. </summary>
    private void PlayerAttack()
    {
        //공격 중이 아니거나, 대미지 입는 중이 아닐 때
        if (!(m_bisAttack))
        {
            //m_playerStatMgr.SetTotalCombo(++m_nTotalCombo);
            m_bisAttack = true;
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
                if (!(m_bisAttack || m_IATKCNT == AttackCount.ONE))
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

        // SetAddSTR((int)m_IATKCNT * 5);

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
}
