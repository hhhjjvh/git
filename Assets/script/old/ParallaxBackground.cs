using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    private GameObject cam;
    [SerializeField] private float parallaxEffect;
    private float xPosition;
    private float yPosition;

    
    private float length;

    void Start()
    {
      //  cam = GameObject.Find("Virtual Camera");
       // cam = GameObject.FindGameObjectWithTag("MainCamera");
        cam = GameObject.FindGameObjectWithTag("Player");

        xPosition = transform.position.x;
        yPosition = transform.position.y;
       
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }


    void Update()
    {
        float distanceMove = cam.transform.position.x *(1- parallaxEffect);
        float distanceToMovex = cam.transform.position.x * parallaxEffect;
        float distanceMovey = 0;
            // cam.transform.position.y * parallaxEffect;
        transform.position = new Vector2(xPosition +distanceToMovex, yPosition + distanceMovey);

        

        //transform.position = new Vector2(xPosition +distanceToMovex,yPosition);

        if(distanceMove > xPosition + length)
        {
            xPosition += length;
        }
        else if(distanceMove < xPosition - length)
        {
            xPosition -= length ;
        }
    }
}
