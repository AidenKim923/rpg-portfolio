// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
// using Newtonsoft.Json;

// /* ���
//  * 
//  *   -SkILL Tree�� Skill Data
//  *   -Player�� ������ Skill Data
//  *   
//  */


// [System.Serializable]
// public class SkillData
// {
//     /**********************************************
//      *                Field
//      **********************************************/
//     #region option Field
//     [JsonProperty] private int    m_nID;
//     [JsonProperty] private string m_sSkillName;
//     [JsonProperty] private int    m_nSkillLevel;
//     [JsonProperty] private int    m_nUsedLevel; // ���� ���� ����
//     [JsonProperty] private string m_sAnimParameterName; // ������ �ִϸ��̼� �̸�
//     [JsonProperty] private string m_sToolTip; //��ų ����
//     [JsonProperty] private string m_sSpritePath;
//     [JsonProperty] private int    m_nSkillDamagePer; // ��ų % ������
//     [JsonProperty] private float  m_fCooldown;
//     [JsonProperty] private int    m_nManaAmount; // �䱸 ����
//     [JsonProperty] private bool   m_bAcquired;  // ��ų ���� üũ
//     #endregion
//     #region private Field
//     private Sprite m_iSprite;
//     private bool m_bInAvailable;
//     private int m_nSkillDamage;
//     private int _anim_Hash;
//     #endregion

//     /**********************************************
//     *                Get, Set Methods
//     **********************************************/
//     #region Get
//     public int GetID() => m_nID;
//     /// <summary> ��ų �̸� ��ȯ </summary>
//     public string GetSKillName() => m_sSkillName;
//     public int GetSkillLevel() => m_nSkillLevel;
//     /// <summary> ��ų ���� ������ ��ȯ</summary>
//     public int GetSkillUsedLevel() => m_nUsedLevel;
//     /// <summary> ���� ���� ��ȯ </summary>
//     public string GetToolTip() => m_sToolTip;
//     /// <summary> ��ų ȹ�� ���� </summary>
//     public bool GetIsAcquired() => m_bAcquired;
//     /// <summary> ��ų ������ �� ��ȯ </summary>
//     public int GetSkillDamage() { return m_nSkillDamage; }
//     /// <summary> �ִϸ��̼��� �ؽ��ڵ� </summary>
//     public int GetAnimHash() { return _anim_Hash; }
//     /// <summary> ���� �Ҹ� �� ��ȯ </summary>
//     public int GetSkillManaAmount() { return m_nManaAmount; }
//     /// <summary> ��ų�� ������ </summary>
//     public float GetSkillDamagePer() { return m_nSkillDamagePer; }
//     ///<summary> ��ų sprite ��ȯ  </summary>
//     public Sprite GetSkill_Sprite() { return m_iSprite; }
//     /// <summary>  �ִϸ��̼� �̸� </summary>
//     public string GetAnimName() { return m_sAnimParameterName; }
//     /// <summary>  ��ų�� ��Ÿ�� </summary>
//     public float GetCoolDown() { return m_fCooldown; }
//     ///<summary>  ��ų ��� ���� </summary>
//     public bool GetInAvailable() { return m_bInAvailable; }
//     #endregion

//     #region Set Methods
//     public void SetSkillDamagePer(int _skillDamagePer) { m_nSkillDamagePer = _skillDamagePer; }
//     /// <summary> ��ų ȹ�� ���� ���� </summary>
//     public void SetIsAcquired(bool value) => m_bAcquired = value;
//     /// <summary> ��ų�� ���� </summary>
//     public void SetSkillLevel(int value)
//     {
//         m_nSkillLevel = value;
//     }

//     ///<summary>  ��ų ��� ���� �缳�� </summary>
//     public void SetInAvailable(bool value) { this.m_bInAvailable = value; }
//     /// <summary> ��ų ����� ���� </summary>
//     public void SetSkillDamage(float _power)
//     {
//         m_nSkillDamage = (int)(m_nSkillDamagePer / 100.0 * _power);
//     }
//     #endregion

//     /**********************************************
//     *                   Methods
//     **********************************************/
//     #region public Methods
//     public void Init()
//     {
//         SetAnimHash();
//         SetSpriteImage();
//     }
//     /// <summary> �ִϸ��̼��� Parameter�� int�� �ؽ� �Ѵ�. </summary>
//     public void SetAnimHash()
//     {
//         _anim_Hash = Animator.StringToHash(m_sAnimParameterName);
//     }
//     /// <summary> ��������Ʈ �̹����� Path�� ���� �ҷ��´�. </summary>
//     public void SetSpriteImage()
//     {
//         m_iSprite = Resources.Load<Sprite>(m_sSpritePath);
//     }
//     /// <summary> ��ų�� ���� �� �ý��� </summary>
//     public void LevelUP()
//     {
//         m_nSkillLevel += 1;
//         m_nUsedLevel += 3;
//         m_nSkillDamagePer = (int)(m_nSkillDamagePer * 1.25f);
//         m_fCooldown -= 1;
//         m_nManaAmount = (int)(m_nManaAmount * 1.5f);
//     }
//     #endregion
// }
