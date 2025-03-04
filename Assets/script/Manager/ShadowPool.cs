using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowPool : MonoBehaviour
{
    
    private GameObject shadow;

    //private Queue<GameObject> shadowPool[] = new Queue<GameObject>()[];
    public Queue<GameObject> shadowPool = new Queue<GameObject>();
   
    /*
    private ObjectPool<GameObject> shadowPool2 = new ObjectPool<GameObject>(CreateShadow, GetShadow, ReturnShadow, DestroyShadow);

    private static void GetShadow(GameObject @object)
    {
        @object.SetActive(true);
    }

    private static GameObject CreateShadow()
    {
       GameObject obj = ProxyResourceFactory.Instance.Factory.GetFX("AfterImage");
       obj.transform.SetParent(instance.transform);
       return obj;
    }

    private static void ReturnShadow(GameObject @object)
    {
        @object.SetActive(false);
    }

    private static void DestroyShadow(GameObject @object)
    {
        Destroy(@object);
    }
    */
   
    public void FillPool()
    {
        for (int i = 0; i < 5; i++)
        {
            GameObject obj = Instantiate(shadow);
            obj.gameObject.name = shadow.name;
            obj.transform.SetParent(this.transform);
            ReturnPool(obj);
        }
    }
    public void ReturnPool(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(this.transform);
        shadowPool.Enqueue(obj);
    }
    public GameObject GetFromPool()
    {

        if (shadowPool.Count == 0)
        {
            FillPool();

        }

        GameObject obj = shadowPool.Dequeue();
        obj.SetActive(true);
       StartCoroutine(Return(obj));
        return obj;

    }
    public void SetShadow(GameObject obj)
    {
        shadow = obj;
    }
    IEnumerator Return(GameObject obj)
    {
        yield return new WaitForSeconds(5f);
        ReturnPool(obj);
    }
}
