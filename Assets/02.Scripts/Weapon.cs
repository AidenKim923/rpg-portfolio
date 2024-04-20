using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minsung.Weapon
{
    public class Weapon : MonoBehaviour
    {
        [Header("Weapon Info")]
        public float m_fDamage;
        public float m_fAttackSpeed;
        public float m_fAttackRange;

        BoxCollider m_boxCollider;

        // Start is called before the first frame update
        void Start()
        {
            m_boxCollider = GetComponent<BoxCollider>();
        }

        private void OnTriggerEnter(Collider _coll)
        {
            //var enemy = _coll.gameObject.GetComponent<enemyAIPatrol>();
            //if (enemy != null)
            //{
            //    enemy.health.HP -= m_fDamage;

            //    if (enemy.health.HP <= 0)
            //    {
            //        enemy.Die();
            //    }
            //}
        }

        public void AttackStart()
        {
            m_boxCollider.enabled = true;
        }

        public void AttackEnd()
        {
            m_boxCollider.enabled = false;
        }
    }

}
