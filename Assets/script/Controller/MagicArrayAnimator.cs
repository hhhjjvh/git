using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicArrayAnimator : MonoBehaviour
{
    
    private void SummonMagicBullet()
    {
        int count =Random.Range(7, 10);
        for (int i = 0; i < count; i++)
        {
            Vector3 pos = new Vector3 (transform.position.x+Random.Range(-1f, 1f), transform.position.y + Random.Range(-1f, 1f), transform.position.z);
            GameObject bullet = PoolMgr.Instance.GetObj("MagicBullet", pos, transform.rotation);
            bullet.GetComponent<MagicBullet>().SetUP(AllEnemy(transform));
        }
    }
    private void DestroyTrigger()
    {
        
        PoolMgr.Instance.Release(gameObject);
    }
    protected virtual List<Transform> AllEnemy(Transform checktransform)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(checktransform.position, 50f);
        //float closestDistance = Mathf.Infinity;
        List<Transform> closestEnemy = new List<Transform>();
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null && !hit.GetComponent<CharacterStats>().isDead && hit.GetComponent<Enemy>().attackLayerName == "Player")
            {
                closestEnemy.Add(hit.transform);
            }
        }
        return closestEnemy;
    }
}
