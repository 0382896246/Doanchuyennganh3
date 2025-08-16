using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private Transform[] levelPart;
    [SerializeField] private Vector3 nextPartPosition;
 
    [SerializeField] private float distanceToSpawn;
    [SerializeField] private float distanceToDelete;
    [SerializeField] private Transform player;
  
    void Update()
    {
        DeleteFlatform();
        GenerateFlatform();
    }

    private void GenerateFlatform()
    {
        while(Vector2.Distance(player.transform.position,nextPartPosition) < distanceToSpawn)
        {
            Transform part = levelPart[Random.Range(0, levelPart.Length)];
            Vector2 newPosition = new Vector2(nextPartPosition.x - part.Find("StartPoint").position.x, 0);
            //transform nơi sinh vị trí sẽ là 1 child trong levelgenerator
            Transform newpart = Instantiate(part, newPosition, transform.rotation, transform);
            nextPartPosition = newpart.Find("EndPoint").position;
        }
    }

    private void DeleteFlatform()
    {
        if (transform.childCount > 0)
        {
            Transform partDelete = transform.GetChild(0); 
            if (Vector2.Distance(player.transform.position, partDelete.position) > distanceToDelete)
            {
                Destroy(partDelete.gameObject);
            }
        }
    }
}
