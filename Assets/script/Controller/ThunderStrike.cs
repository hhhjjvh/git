using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderStrike : MonoBehaviour
{
  
    //public GameObject thunderStrike;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
      
        
    }
    private void AnimationTrigger()
    {

        // PoolManager.instance.ReturnToControllerPool(gameObject.transform.parent.gameObject);
        PoolMgr.Instance.Release(gameObject.transform.parent.gameObject);
    }
}
