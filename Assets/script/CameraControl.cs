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
        // �������λ��
        //transform.position = newPlayerPosition;

        // ǿ��ͬ�������λ��
        SmoothSwitchLayer(PlayerManager.instance.player.transform, newPlayerPosition, 0.1f);

         var cameraBrain = Camera.main.GetComponent<CinemachineBrain>();
        virtualCamera.ForceCameraPosition(newPlayerPosition, Quaternion.identity);

        // ȷ���������������
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
        // ��ʱ�������������
        virtualCamera.Follow = null;

        // �ƶ���ҵ���λ��
       // playerTransform.position = newPosition;

        // �ȴ��л����
        yield return new WaitForSeconds(delay);

        // �ָ����������
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
