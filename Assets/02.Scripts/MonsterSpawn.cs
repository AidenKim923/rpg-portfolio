using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Minsung.UTIL;

namespace Minsung.MonsterSpawn
{
    public class MonsterSpawn : MonoBehaviour
    {
        /**************************************************************
         * 
         *                  Field
         * 
         **************************************************************/

        #region Field
        [Header("Monster")]
        [SerializeField] private GameObject m_goMonsterPrefab;
        [SerializeField] private Transform m_trSpawnPoint;
        [SerializeField] private float m_fSpawnTime = 5.0f;
        [SerializeField] private float m_fSpawnDistance = 5.0f;
        [SerializeField] private int m_nMaxMonsterCount = 10;

        private List<GameObject> m_goMonsterList = new List<GameObject>();
        private List<Vector3> m_vSpawnPositions  = new List<Vector3>();

        #endregion

        /**************************************************************
         * 
         *                  Unity Event
         * 
         **************************************************************/
        #region Unity Event

        private void Start()
        {
            CreateSpawn();
        }

        private void Update()
        {
            if (m_goMonsterList.Count < (m_nMaxMonsterCount % 2))
            {
                SpawnMonster();
            }
        }


        #endregion

        /**************************************************************
         * 
         *                  Private Method
         * 
         **************************************************************/

        #region Private Method
        [ContextMenu("Spawn")]
        private void CreateSpawn()
        {
            for (int i = 0; i < m_nMaxMonsterCount; i++)
            {
                Quaternion randRot = Quaternion.Euler(Vector3.up * Random.Range(0, 360));
                GameObject goMonster = Instantiate(m_goMonsterPrefab, m_trSpawnPoint.position, m_trSpawnPoint.rotation * randRot);
                goMonster.transform.position = Util.GetRandomPos(m_trSpawnPoint, m_fSpawnDistance);

                m_goMonsterList.Add(goMonster);
                m_vSpawnPositions.Add(goMonster.transform.position);
            }
        }

        private void SpawnMonster()
        {
            Quaternion randRot = Quaternion.Euler(Vector3.up * Random.Range(0, 360));
            GameObject goMonster = Instantiate(m_goMonsterPrefab, m_trSpawnPoint.position, m_trSpawnPoint.rotation * randRot);
            goMonster.transform.position = Util.GetRandomPos(m_trSpawnPoint, m_fSpawnDistance);

            m_goMonsterList.Add(goMonster);
        }

        #endregion
    }

}
