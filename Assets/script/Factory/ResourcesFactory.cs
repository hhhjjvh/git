using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections;
using UnityEngine.InputSystem;

public class ResourcesFactory : IResourceFactory
{
   // private string DataPath = "Data/";
    //private string EnemyPath = "Prefabs/Enemy/";
    //private string UIPath = "Prefabs/UI/";
    //private string FXPath = "Prefabs/FX/";
    //private string ControllersPath = "Prefabs/Controllers/";

    public GameObject GetControllers(string name)
    {
        return AAMgr.LoadAsset<GameObject>(name);
            // Resources.Load<GameObject>(ControllersPath + name);
    }

    public GameObject GetEnemy(EnemyName name)
    {
        //GameObject enemy = Addressables.LoadAssetAsync<GameObject>(name).WaitForCompletion();
       /* GameObject enemy = Resources.Load<GameObject>(EnemyPath + name);
        if (enemy == null)
        {
            Debug.Log("没有找到" + name + "敌人");
            return null;
        }
        return enemy;
        */
       string name1 = name.ToString();
        return AAMgr.LoadAsset<GameObject>(name1);

    }

    public  GameObject GetFX(string name)
    {
        /*
          Assets/Resources_moved/
        GameObject fx;
        Addressables.LoadAssetAsync<GameObject>(name).Completed += (handle) =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                fx= handle.Result;
               
            }
        };
        */
       

        return AAMgr.LoadAsset<GameObject>(name);

        //

        // GameObject fx = Resources.Load<GameObject>(FXPath + name);
        // return Resources.Load<GameObject>(FXPath + name);
    }

    public GameObject GetUI(string  name)
    {
        //GameObject ui = Resources.Load<GameObject>(UIPath + name);
        return AAMgr.LoadAsset<GameObject>(name);
    }

}
