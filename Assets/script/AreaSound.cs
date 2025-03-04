using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AreaSound : MonoBehaviour
{
    [SerializeField] private int soundID;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<player1>() != null)
        {
            //AudioManager.instance.PlaySFX(soundID,null);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<player1>() != null)
        {
            //AudioManager.instance.StopSFXWithTime(soundID);
        }
    }
}
