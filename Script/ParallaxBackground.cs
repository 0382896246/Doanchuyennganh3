using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    private GameObject cam;
    
    [SerializeField] private float parralaxEffect;
    
    private float length;
    private float xPosition;

    void Start()
    {
        cam = GameObject.Find("Main Camera");
        length=GetComponent<SpriteRenderer>().bounds.size.x;
        xPosition = transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        // tạo hiệu ứng parallax
        float distanceMove = cam.transform.position.x * (1- parralaxEffect); 
        // ví dụ parralaxeffect = 0.6 thì kc di chuyển là 40 % = 0.4 của camera 
        
        float distanceToMove= cam.transform.position.x * parralaxEffect;
        transform.position= new Vector3(xPosition+ distanceToMove,transform.position.y);
        
        //giúp luôn kiểm tra xem background có vượt qua camera player hay ko 
        if (distanceMove > xPosition + length)
        {
            xPosition = xPosition + length;
        }
    }
}
