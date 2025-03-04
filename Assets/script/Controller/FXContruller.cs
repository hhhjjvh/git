using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXContruller : MonoBehaviour
{
    
    private void DestroyTrigger()
    {
        
        PoolMgr.Instance.Release(gameObject);
    }
}
