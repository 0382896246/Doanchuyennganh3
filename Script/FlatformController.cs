using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlatformController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sr;

    [SerializeField] private SpriteRenderer headerSr;

    private void Start()
    {
        headerSr.transform.parent = transform.parent;
        headerSr.transform.localScale = new Vector2(sr.transform.localScale.x, .1f);
        headerSr.transform.position = new Vector2(transform.position.x,sr.bounds.max.y - .1f);

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            if (GameManager.instance.colorEntirePlatform)
            {
            
                headerSr.color = GameManager.instance.flatformColor;
                sr.color = GameManager.instance.flatformColor;

            }
            else
            {
            
                headerSr.color = GameManager.instance.flatformColor;

            }
        }
    }
}
