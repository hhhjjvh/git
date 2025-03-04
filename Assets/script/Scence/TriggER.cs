using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;

public class TriggER : MonoBehaviour
{
    player1 player;
    bool canstay;
    float timer ;
    private PolygonCollider2D polygonCollider=> GetComponent<PolygonCollider2D>();
    // Start is called before the first frame update
    void Start()
    {
        player =PlayerManager.instance.player;
       // polygonCollider = GetComponent<PolygonCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (timer >= 0)
        {
            timer -= Time.deltaTime;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player"&&CameraControl.Instance.isSwitchingLayer==false)
        {
            timer = 3f;
            StartCoroutine(fadeScreen());
           
            //Debug.Log("触发器");

        }
    }
  IEnumerator fadeScreen()
    {
       UI.instance.fadeScreen.FadeOut();
        canstay=false;
        yield return new WaitForSeconds(0.2f);
       
        player.stateMachine.ChangeState(player.idleState);
        CameraControl.Instance.confiner.enabled = false;
        CameraControl.Instance.confiner.m_BoundingShape2D = GetComponent<Collider2D>();
        CameraControl.Instance.confiner.InvalidateCache();
        CameraControl.Instance.confiner.enabled = true;
        yield return new WaitForSeconds(0.2f);
        UI.instance.fadeScreen.FadeIn();
        canstay = true;

    }
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player"&&canstay&&timer<=0)
        {
            if (CameraControl.Instance.confiner.m_BoundingShape2D != GetComponent<Collider2D>())
            {
                timer = 3f;
                StartCoroutine(fadeScreen());
                //if (RandomMapGenerator.Instance != null && RandomMapGenerator.Instance.currentRoom != null)
                //{
                //    Vector2Int center = RandomMapGenerator.Instance.currentRoom.Center;
                //    PlayerManager.instance.player.transform.position = new Vector3(center.x, center.y, 0);
                //}
                //CameraControl.Instance.confiner.m_BoundingShape2D = GetComponent<Collider2D>();
            }
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        
        if (other.gameObject.tag == "Player")
        {
            if (player.stateMachine.currentState == player.flyState)
            {
                canstay = false;
               // player.stateMachine.ChangeState(player.idleState);
                //CameraControl.Instance.confiner.m_BoundingShape2D = null;
            }


        }
    }
    public void RefreshConfiner(CinemachineConfiner confiner)
    {
        if (confiner != null)
        {
            confiner.enabled = false;
            confiner.enabled = true;
        }
    }
    // 动态设置碰撞器为房间边界
    public void SetRoomBounds(int width, int height)
    {
        // 计算房间边界顶点
        Vector2[] points = new Vector2[4];
        points[0] = new Vector2(0, 0);               // 左下角
        points[1] = new Vector2(0, height);          // 左上角
        points[2] = new Vector2(width, height);      // 右上角
        points[3] = new Vector2(width, 0);           // 右下角

        // 设置 Polygon Collider 的顶点
        polygonCollider.points = points;
    }

}
