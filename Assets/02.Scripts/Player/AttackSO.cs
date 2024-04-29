using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minsung.AttackSO
{
    [CreateAssetMenu(menuName = "Attacks/Normal Attack")]
    public class AttackSO : ScriptableObject
    {
        public AnimatorOverrideController m_animatorOv;
        public float m_fDamage;
    }
}
