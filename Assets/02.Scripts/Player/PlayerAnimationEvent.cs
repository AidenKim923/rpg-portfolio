using System.Collections;
using System.Collections.Generic;
using Minsung.PLAYER;
using Minsung.MONSTERCONTROL;
using Unity.VisualScripting;
using UnityEngine;

/**
*   - 플레이어의 스킬을 발동시키는 클래스
*  - 플레이어의 스킬을 발동시키는 함수를 가지고 있음
*   TODO 스킬이 MonsterArea에 스킬이 발동되는데 Monster에 스킬이 발동되도록 수정
*   ! 스킬이 발동되는 위치가 플레이어의 위치로 고정되어 있음
*/

namespace Minsung.PLAYERANIMATIONEVENT
{
    public class PlayerAnimationEvent : MonoBehaviour
    {
        /**************************************************************
        * 
        *                  Field
        * 
        **************************************************************/

        #region Field

        [SerializeField] private List<GameObject> m_goEffectList;
        private GameObject m_goCloneEffect;
        private PlayerCtrl m_PlayerCtrl;
        private MonsterCtrl m_MonsterCtrl;

        private Coroutine coroutine;

        #endregion


        /**************************************************************
        * 
        *                  Unity Event
        * 
        **************************************************************/

        #region Unity Event

        private void Awake()
        {
            m_PlayerCtrl = GetComponentInParent<PlayerCtrl>();

        }

        private void Start()
        {
            m_goCloneEffect = Instantiate(m_goEffectList[m_PlayerCtrl.m_nSkillIndex], transform.position, Quaternion.identity);
            m_goCloneEffect.transform.SetParent(transform);
            m_goCloneEffect.transform.position = transform.position;
            m_goCloneEffect.SetActive(false);
        }

        private void Update()
        {
            FindClosestMonsterDistance(Input.mousePosition);
        }

        #endregion

        /**************************************************************
        * 
        *                  Public Methods
        * 
        **************************************************************/

        #region Public Methods

        /// <summary>
        /// 플레이어의 스킬을 발동시키는 함수
        /// </summary>
        public void InstantiateEffect()
        {

            // float skillRange = m_PlayerCtrl.curSkill.SkillRange;
            // if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, skillRange, MonsterLayer))
            // {
            //     Debug.Log("몬스터가 범위 안에 있습니다.");
            //     // TODO: Add code to handle when a monster is within range
            //     Vector3 skillPosition = hit.point;
            //     InstantiateSkill(skillPosition);
            // }
            // else
            // {
            //     Debug.Log("몬스터가 범위 밖에 있습니다.");
            //     // TODO: Add code to handle when no monster is within range
            //     if (Physics.Raycast(transform.position, transform.forward, out RaycastHit groundHit, skillRange, m_PlayerCtrl.GetClickableLayer()))
            //     {
            //         Vector3 skillPosition = groundHit.point;
            //         float distanceToMouse = Vector3.Distance(skillPosition, Input.mousePosition);
            //         if (distanceToMouse <= skillRange)
            //         {
            //             InstantiateSkill(skillPosition);
            //         }
            //         else
            //         {
            //             Debug.Log("마우스 커서가 일정 범위보다 더 멀어서 생성하지 않습니다.");
            //         }
            //     }
            // }
            m_PlayerCtrl.m_bIsAttacking = true;
            Debug.Log(m_goEffectList[m_PlayerCtrl.m_nSkillIndex].name);

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, m_PlayerCtrl.GetClickableLayer()))
            {
                Vector3 m_v3MousePosition = hit.point;
                Vector3 m_v3PlayerPosition = transform.position;
                float m_fDistanceToMouse = Vector3.Distance(m_v3MousePosition, m_v3PlayerPosition);

                // 몬스터가 너무 먼 경우
                if (m_fDistanceToMouse > 20 || (FindClosestMonsterDistance(m_v3MousePosition) > 20))
                {
                    // m_goCloneEffect = Instantiate(m_goEffectList[m_PlayerCtrl.m_nSkillIndex], m_v3PlayerPosition, Quaternion.identity);
                    // m_goCloneEffect.SetActive(true);
                    // coroutine = StartCoroutine(OffSkillEffect(m_PlayerCtrl.curSkill.SkillRunningTime));
                    Debug.Log("몬스터가 너무 멀거나 없습니다.");
                }
                else
                {

                    // GameObject closestMonster = FindClosestMonster(m_v3MousePosition);
                    // MonsterCtrl closestMonsterCtrl = closestMonster.GetComponent<MonsterCtrl>();
                    if (m_goMonster != null)
                    {
                        Debug.Log("몬스터가 있습니다.");
                        if (m_MonsterCtrl != null)
                        {
                            Vector3 monsterPosition = m_goMonster.transform.position;
                            m_goCloneEffect = Instantiate(m_goEffectList[m_PlayerCtrl.m_nSkillIndex], monsterPosition, Quaternion.identity);
                            m_goCloneEffect.SetActive(true);
                            coroutine = StartCoroutine(OffSkillEffect(m_PlayerCtrl.curSkill.SkillRunningTime));
                            Debug.Log("몬스터 발견 : " + m_MonsterCtrl.name);
                        }
                    }
                    else
                    {
                        Debug.Log("몬스터가 없습니다.");
                    }
                }
            }

        }

        public void DestroyEffect()
        {
            m_PlayerCtrl.m_bIsAttacking = false;
        }

        #endregion

        /**************************************************************
        * 
        *                  Private Methods
        * 
        **************************************************************/

        #region Private Methods

        /// <summary>
        /// 스킬 이펙트를 끄는 함수
        /// </summary>
        /// <param name="_skillRunningTime"></param>
        /// <returns></returns>
        private IEnumerator OffSkillEffect(float _skillRunningTime)
        {
            yield return new WaitForSeconds(_skillRunningTime);
            m_goCloneEffect.transform.SetParent(transform);
            m_goCloneEffect.transform.position = transform.position;
            m_goCloneEffect.SetActive(false);

            StopCoroutine(coroutine);
            coroutine = null;
        }

        GameObject m_goMonster;
        public LayerMask MonsterLayer;

        /// <summary>
        /// 마우스 위치와 제일 가까운 몬스터와의 거리를 찾는 함수
        /// </summary>
        /// <param name="_m_v3MousePosition"></param>
        /// <returns></returns>
        private float FindClosestMonsterDistance(Vector3 _m_v3MousePosition)
        {
            Ray ray = Camera.main.ScreenPointToRay(_m_v3MousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, MonsterLayer))
            {
                Debug.Log("몬스터를 찾았습니다.");
                m_goMonster = hit.collider.gameObject;
                m_MonsterCtrl = m_goMonster.GetComponent<MonsterCtrl>();
                return Vector3.Distance(transform.position, hit.point);
            }
            // else
            // {
            //     m_goMonster = null;
            //     m_MonsterCtrl = null;
            // }

            return 0;
        }

        /// <summary>
        /// 마우스 위치와 제일 가까운 몬스터를 찾는 함수
        /// </summary>
        /// <param name="_m_v3MousePosition"></param>
        /// <returns></returns>
        private GameObject FindClosestMonster(Vector3 _m_v3MousePosition)
        {
            Ray ray = Camera.main.ScreenPointToRay(_m_v3MousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("Hit : " + hit.collider.gameObject.name);
                return hit.collider.gameObject;
            }

            return null;
        }
        #endregion

    }
}
