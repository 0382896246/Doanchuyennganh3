using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCreator : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefabs;
    [SerializeField] private Transform respawnPostion;
    [SerializeField] private float chanceToSpawn;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            if (Random.Range(0, 100) <= chanceToSpawn)
            {
              GameObject newEnemy = Instantiate(enemyPrefabs,respawnPostion.position,Quaternion.identity);
                Destroy(newEnemy, 30);
            }
        }
    }
}
