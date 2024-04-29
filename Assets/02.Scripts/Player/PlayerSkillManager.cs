using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Minsung.SKILLDATA;
using UnityEditor.Rendering.Universal;
using Minsung.PLAYER;

namespace Minsung.PLAYERSKILLMANAGER
{
    public class PlayerSkillManager : MonoBehaviour
    {
        /**************************************************************
        * 
        *                  Field
        * 
        **************************************************************/
        #region Field

        public List<SkillData> m_lstPlayerSkillData = new List<SkillData>();

        public GameObject m_goEffectPrefab;


        #endregion

        /**************************************************************
        * 
        *                  Unity Event
        * 
        **************************************************************/
        #region Unity Event

        private void Awake()
        {
            for (var i = 0; i < m_lstPlayerSkillData.Count; i++)
            {
                m_lstPlayerSkillData[i].Init();
            }
            GameObject.FindObjectOfType<PlayerCtrl>().SetPlayerSkillManager(this);
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        #endregion

        /**************************************************************
        * 
        *                  Public Methods
        * 
        **************************************************************/
        #region Public Methods
        public void AddSkill(SkillData _skillData)
        {
            m_lstPlayerSkillData.Add(_skillData);
            _skillData.IsAcquired = true;
        }

        public void UseSkill(SkillData _skillData)
        {
            if (m_lstPlayerSkillData.Contains(_skillData) && !IsSkillOnCooltime(_skillData))
            {
                Debug.Log("Use Skill : " + _skillData.SkillName);
                PlaySkillEffect(_skillData);
                StartCoroutine(StartSkillCooltime(_skillData));
            }
            else
            {
                Debug.Log("Skill is on cooltime");
            }
        }

        public bool IsSkillOnCooltime(SkillData _skillData)
        {
            return _skillData.IsOnCooltime;
        }



        #endregion

        /**************************************************************
        * 
        *                  Private Methods
        * 
        **************************************************************/

        #region Private Methods

        private IEnumerator StartSkillCooltime(SkillData _skillData)
        {
            _skillData.StartCooltime();
            yield return new WaitForSeconds(_skillData.SkillCoolTime);
            _skillData.ResetCooltime();
        }

        private void PlaySkillEffect(SkillData _skillData)
        {
            GameObject effect = Instantiate(m_goEffectPrefab, transform.position, Quaternion.identity);

            Destroy(effect, _skillData.SkillRunningTime);
        }

        #endregion
    }
}
