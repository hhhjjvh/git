using System.Threading.Tasks;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager instance;
    public Transform FXpoolParent;
    public Transform ControllerspoolParent;

    //private Queue<GameObject> shadowPool[] = new Queue<GameObject>()[];
    //private Queue<GameObject> shadowPool = new Queue<GameObject>();
    private ShadowPool[] FXPools;
    private ShadowPool[] ControllersPools;
    // private List<ShadowPool> shadowPools ;

    private void Awake()
    {
        instance = this;
        //shadowPools = new List<ShadowPool>();

    }

    #region FX
    public void ReturnToFXPool(GameObject obj)
    {
        obj.SetActive(false);
        //shadowPool.Enqueue(obj);
        foreach (ShadowPool pool in FXPools)
        {
            if (pool.gameObject.name == obj.name)
            {
                pool.shadowPool.Enqueue(obj);
            }
        }
    }
    public GameObject GetFXFromPool(string name)
    {
        if (FXPools == null)
        {
            addNewToFXPool(name);
        }

        foreach (ShadowPool pool in FXPools)
        {
            if (pool.gameObject.name == name)
            {
                if (pool.shadowPool.Count == 0)
                {
                    pool.FillPool();
                }
                GameObject obj = pool.shadowPool.Dequeue();
                obj.SetActive(true);
                return obj;
            }

        }
        addNewToFXPool(name);
        return GetFXFromPool(name);


    }

    private void addNewToFXPool(string name)
    {
        var poolObj = new GameObject(name);
        poolObj.transform.SetParent(FXpoolParent);
        //poolObj.transform.position = this.transform.position;
        poolObj.SetActive(false);
        var poolp = poolObj.AddComponent<ShadowPool>();
        GameObject shadow = ProxyResourceFactory.Instance.Factory.GetFX(name);


        shadow.name = name;

        poolp.SetShadow(shadow);
        
       // Destroy(shadow);
        poolp.gameObject.name = name;
        poolp.FillPool();
        poolObj.SetActive(true);

        //shadowPools = new List<ShadowPool>(shadowPools);
        // shadowPools.Add(poolp);
        FXPools = FXpoolParent.GetComponentsInChildren<ShadowPool>();
        shadow.transform.SetParent(poolObj.transform);
        
        ReturnToFXPool(shadow);
    }
    #endregion
    #region Controllers
    public void ReturnToControllerPool(GameObject obj)
    {
        obj.SetActive(false);
        foreach (ShadowPool pool in ControllersPools)
        {
            if (pool.gameObject.name == obj.name)
            {

                pool.ReturnPool(obj);
            }
        }
    }
    public GameObject GetControllerFromPool(string name)
    {
        if (ControllersPools == null)
        {
            addNewToControllerPool(name);
        }
        foreach (ShadowPool pool in ControllersPools)
        {
            if (pool.gameObject.name == name)
            {
                if (pool.shadowPool.Count == 0)
                {
                    pool.FillPool();
                }
                GameObject obj = pool.shadowPool.Dequeue();
                obj.SetActive(true);
                return obj;
            }
        }
        addNewToControllerPool(name);
        return GetControllerFromPool(name);
    }

    private void addNewToControllerPool(string name)
    {
        var poolObj = new GameObject(name);
        poolObj.transform.SetParent(ControllerspoolParent);
        // poolObj.transform.position = this.transform.position;
        poolObj.SetActive(false);
        var poolp = poolObj.AddComponent<ShadowPool>();
        GameObject shadow = ProxyResourceFactory.Instance.Factory.GetControllers(name);
        shadow.name = name;
        poolp.SetShadow(shadow);
       
       // Destroy(shadow);
        poolp.gameObject.name = name;
        poolp.FillPool();
        poolObj.SetActive(true);
        ControllersPools = ControllerspoolParent.GetComponentsInChildren<ShadowPool>();
        shadow.transform.SetParent(poolObj.transform);
        
        ReturnToControllerPool(shadow);
    }
    #endregion
}
