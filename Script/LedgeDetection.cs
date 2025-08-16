using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeDetection : MonoBehaviour
{
    [SerializeField] private float radius;
    [SerializeField] private Player player;
    [SerializeField] private Enemy enemy;
    [SerializeField] private LayerMask whatIsGround;
    private bool canDetected;

    private BoxCollider2D boxCd => GetComponent<BoxCollider2D>();

    private void Update()
    {
        // nếu là leo hết tường rồi thì kiểm tra 
        if(player!=null && canDetected)
            player.ledgeDetected = Physics2D.OverlapCircle(transform.position, radius,whatIsGround);

        if(enemy!= null && canDetected)
            enemy.ledgeDetected = Physics2D.OverlapCircle(transform.position, radius, whatIsGround);

    }

    // kiểm tra xem đã leo hết tường chưa 
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            canDetected = false;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(boxCd.bounds.center, boxCd.size, 0);
        
        foreach (var hit in colliders)
        {
            if (hit.gameObject.GetComponent<FlatformController>() != null)
            {
                return;
            }
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            canDetected = true;
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
