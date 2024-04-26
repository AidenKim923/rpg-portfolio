using System.Collections;
using System.Collections.Generic;
using Minsung.PLAYER;
using Unity.VisualScripting;
using UnityEngine;

namespace Minsung.PLAYERANIMATIONEVENT
{
    public class PlayerAnimationEvent : MonoBehaviour
    {
        [SerializeField] private List<GameObject> m_goEffectList;
        private GameObject m_goCloneEffect;
        private PlayerCtrl m_PlayerCtrl;

        private Coroutine coroutine;

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

        public void InstantiateEffect()
        {
            m_PlayerCtrl.m_bIsAttacking = true;
            Debug.Log(m_goEffectList[m_PlayerCtrl.m_nSkillIndex].name);

            m_goCloneEffect = Instantiate(m_goEffectList[m_PlayerCtrl.m_nSkillIndex], transform.position, Quaternion.identity);

            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                m_goCloneEffect.transform.SetParent(transform);
                m_goCloneEffect.transform.position = transform.position;
                m_goCloneEffect.SetActive(false);

            }
            m_goCloneEffect.transform.SetParent(null);
            m_goCloneEffect.transform.position = transform.position;
            m_goCloneEffect.SetActive(true);
            coroutine = StartCoroutine(OffSkillEffect(m_PlayerCtrl.curSkill.SkillRunningTime));
        }

        public void DestroyEffect()
        {
            m_PlayerCtrl.m_bIsAttacking = false;
        }

        private IEnumerator OffSkillEffect(float _skillRunningTime)
        {
            yield return new WaitForSeconds(_skillRunningTime);
            m_goCloneEffect.transform.SetParent(transform);
            m_goCloneEffect.transform.position = transform.position;
            m_goCloneEffect.SetActive(false);

            StopCoroutine(coroutine);
            coroutine = null;
        }
    }
}
