using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraControl : MonoBehaviour
{
    public static CameraControl Instance;
    public CinemachineConfiner2D confiner=> GetComponent<CinemachineConfiner2D>();
    public CinemachineVirtualCamera virtualCamera=> GetComponent<CinemachineVirtualCamera>();
    public bool isSwitchingLayer = false;
    

    private void Awake()
    {
        Instance = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    

    public void SwitchLayer(Vector3 newPlayerPosition)
    {
        // 更新玩家位置
        //transform.position = newPlayerPosition;

        // 强制同步摄像机位置
        SmoothSwitchLayer(PlayerManager.instance.player.transform, newPlayerPosition, 0.1f);

         var cameraBrain = Camera.main.GetComponent<CinemachineBrain>();
        virtualCamera.ForceCameraPosition(newPlayerPosition, Quaternion.identity);

        // 确保摄像机立即对齐
        cameraBrain.ManualUpdate();
        //virtualCamera.enabled=true;
        GameObject.FindGameObjectWithTag("MainCamera").transform.position = newPlayerPosition;
        //confiner.enabled = true;
       // confiner.InvalidateCache();
        //RefreshConfiner(GetComponent<CinemachineConfiner2D>());
    }
    public void SmoothSwitchLayer(Transform playerTransform, Vector3 newPosition, float delay = 0.2f)
    {
        StartCoroutine(SwitchLayerCoroutine(playerTransform, newPosition, delay));
    }

    private IEnumerator SwitchLayerCoroutine(Transform playerTransform, Vector3 newPosition, float delay)
    {
        isSwitchingLayer = true;
        // 暂时禁用摄像机跟随
        virtualCamera.Follow = null;

        // 移动玩家到新位置
       // playerTransform.position = newPosition;

        // 等待切换完成
        yield return new WaitForSeconds(delay);

        // 恢复摄像机跟随
        virtualCamera.Follow = playerTransform;
        isSwitchingLayer = false;
    }
    
    public void GetNewCameraBounds()
    {
        var bounds = GameObject.FindGameObjectWithTag("Bounds");
       // Debug.Log(bounds);
        if (bounds != null)
        {
            confiner.m_BoundingShape2D = bounds.GetComponent<Collider2D>();
            confiner.InvalidateCache();
        }
    }
}
