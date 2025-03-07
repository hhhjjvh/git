using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceAndFireController : ThunderStrikeContorllers
{
    float time = 0;
    void OnEnable()
    {
        time = 1;
    }
    void Update()
    {
        if (time > 0)
        {
            time -= Time.deltaTime;
        }
        else
        {
            PoolMgr.Instance.Release(gameObject);
        }
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
    }
}
