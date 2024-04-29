using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
* 
*   -SkILL Tree의 Skill Data
*   -Player의 보유한 Skill Data
*   TODO : Json 프로퍼티 추가, 상속받아서 PlayerSkillData 클래스 생성
*   
*/

namespace Minsung.SKILLDATA
{
    [System.Serializable]
    public class SkillData
    {
        /**************************************************************
        * 
        *                  Field
        * 
        **************************************************************/
        #region option Field


        [SerializeField] private int m_nSkillID;
        [SerializeField] private string m_strSkillName;
        [SerializeField] private int m_nSkillLevel;
        [SerializeField] private int m_nSkillUsedLevel;
        [SerializeField] private string m_strAnimParameterName;
        [SerializeField] private string m_strSkillDesc;
        [SerializeField] private string m_strSpritePath;
        [SerializeField] private int m_nSkillDamagePer;
        [SerializeField] private float m_fSkillCoolTime;
        [SerializeField] private float m_fMpAmount;
        [SerializeField] private float m_fSkillRunningTime;
        [SerializeField] private float m_fSkillRange;
        [SerializeField] private bool m_bIsAcquired;

        #endregion

        #region private Field

        private Sprite m_iSprite;
        private bool m_bIsAvailable;
        private int m_nSkillDamage;
        private int _anim_Hash;

        #endregion

        /**************************************************************
        * 
        *                 Properties
        * 
        **************************************************************/

        #region Properties

        public int SkillID
        {
            get { return m_nSkillID; }
            set { m_nSkillID = value; }
        }

        public string SkillName
        {
            get { return m_strSkillName; }
            set { m_strSkillName = value; }
        }

        public int SkillLevel
        {
            get { return m_nSkillLevel; }
            set { m_nSkillLevel = value; }
        }

        public int SkillUsedLevel
        {
            get { return m_nSkillUsedLevel; }
            set { m_nSkillUsedLevel = value; }
        }

        public string AnimParameterName
        {
            get { return m_strAnimParameterName; }
            set { m_strAnimParameterName = value; }
        }

        public string SkillDesc
        {
            get { return m_strSkillDesc; }
            set { m_strSkillDesc = value; }
        }

        public string SpritePath
        {
            get { return m_strSpritePath; }
            set { m_strSpritePath = value; }
        }

        public int SkillDamagePer
        {
            get { return m_nSkillDamagePer; }
        }
        public void SetSkillDamagePer(float _power)
        {
            m_nSkillDamage = (int)(m_nSkillDamagePer / 100.0 * _power);
        }

        public float SkillCoolTime
        {
            get { return m_fSkillCoolTime; }
            set { m_fSkillCoolTime = value; }
        }

        public float MpAmount
        {
            get { return m_fMpAmount; }
        }

        public float SkillRunningTime
        {
            get { return m_fSkillRunningTime; }
            set { m_fSkillRunningTime = value; }
        }

        public float SkillRange
        {
            get { return m_fSkillRange; }
            set { m_fSkillRange = value; }
        }

        public bool IsAcquired
        {
            get { return m_bIsAcquired; }
            set { m_bIsAcquired = value; }
        }

        public bool IsOnCooltime
        {
            get { return m_bIsAvailable; }
            set { m_bIsAvailable = value; }
        }

        #endregion

        /**************************************************************
        * 
        *                 Public Methods
        * 
        **************************************************************/

        #region Public Methods

        public void Init()
        {
            SetAnimHash();
            // SetSpriteImage();
        }

        public int GetAnimHash()
        {
            return _anim_Hash;
        }

        public void SetAnimHash()
        {
            _anim_Hash = Animator.StringToHash(m_strAnimParameterName);
        }

        public void SetSpriteImage()
        {
            m_iSprite = Resources.Load<Sprite>(m_strSpritePath);
        }

        public void LevelUP()
        {
            m_nSkillLevel += 1;
            m_nSkillUsedLevel += 3;
            m_nSkillDamagePer = (int)(m_nSkillDamagePer * 1.25f);
            m_fSkillCoolTime -= 1;
            m_fMpAmount = (int)(m_fMpAmount * 1.5f);
        }

        public void ResetCooltime()
        {
            IsOnCooltime = false;
        }

        public void StartCooltime()
        {
            IsOnCooltime = true;
        }


        #endregion

    }

}
