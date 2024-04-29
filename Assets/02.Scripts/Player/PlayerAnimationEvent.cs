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
            // 몬스터 컨트롤러에서 받아와서 몬스터의 위치에 스킬이 발동되도록 수정
            Vector3 MonsterPosition = m_PlayerCtrl.GetMonster().transform.position;
            m_goCloneEffect = Instantiate(m_goEffectList[m_PlayerCtrl.m_nSkillIndex], MonsterPosition, Quaternion.identity);
            m_goCloneEffect.SetActive(true);
            coroutine = StartCoroutine(OffSkillEffect(m_PlayerCtrl.curSkill.SkillRunningTime));


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
        #endregion

    }
}
