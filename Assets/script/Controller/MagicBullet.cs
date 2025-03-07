using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class MagicBullet : MonoBehaviour
{
    player1 player;
    Transform closeenemy;
    public float rotationSpeed = 10f;  // 旋转速度
    public float maxSpeed = 4f;  // 最大飞行速度
    public float minSpeed = 3f;   // 最小飞行速度
    public float rotationLag = 0.5f;  // 旋转延迟时间，越大越滞后

    private Vector3 velocity = Vector3.zero;  // 用于平滑移动的速度变量
    //private Rigidbody2D rb;
    float time;
    float speed = 10;
    private List<Transform> tragets = new List<Transform>();
    // Start is called before the first frame update
    void Start()
    {
      //  rb = GetComponent<Rigidbody2D>();
        player = PlayerManager.instance.player;
    }
    void OnDisable()
    {
        closeenemy = null;
        time= 0;
    }
    public void SetUP(List<Transform> tragets)
    {
        this.tragets = tragets;
        speed=Random.Range(8, 12);
        if (tragets.Count > 0)
        {
            closeenemy = tragets[Random.Range(0, tragets.Count)];
        }
    }
    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
       // closeenemy = closeEnemy(transform);
        if (time > 5)
        {
            PoolMgr.Instance.Release(gameObject);
        }
        tragets.RemoveAll(enemy => enemy == null || enemy.GetComponent<CharacterStats>().isDead == true
       || enemy.gameObject.activeSelf == false || enemy.GetComponent<Enemy>().attackLayerName != "Player");
        if (closeenemy == null|| closeenemy.GetComponent<CharacterStats>().isDead || closeenemy.GetComponent<Enemy>().attackLayerName != "Player")
        {
            if (tragets.Count ==0)
            {
                tragets = AllEnemy(transform);
            }

            if (tragets.Count > 0)
            {
                closeenemy = tragets[Random.Range(0, tragets.Count)];
            }
        }
        if (closeenemy == null)
        {
            PoolMgr.Instance.Release(gameObject);
            return;
        }
      
        // 4. 计算距离敌人最近的距离
        //float distance = Vector3.Distance(transform.position, closeenemy.position);

        //// 5. 根据距离调整飞行速度
        //float currentSpeed = Mathf.Lerp(minSpeed, maxSpeed, 1 / distance); // 距离越远，速度越快，反之越慢
        float currentSpeed = speed*Random.Range(0.8f, 1.2f);

        // 6. 使用 SmoothDamp 来平滑移动，加入滞后效果
        transform.position = Vector3.SmoothDamp(transform.position, closeenemy.position, ref velocity, rotationLag, currentSpeed, Time.deltaTime);
    
    //transform.position = Vector2.MoveTowards(transform.position, closeenemy.position, speed * Time.deltaTime);
      
        transform.right = -(closeenemy.position-transform.position).normalized; 
    }
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null && !collision.GetComponent<CharacterStats>().isDead
            && collision.GetComponent<Enemy>().attackLayerName=="Player")
        {
            player.stats.DoMagicDamage(collision.GetComponent<Enemy>().stats,0.5f);
            AudioManager.instance.PlaySFX(36,null);
           // AttackSense.instance.HitPause(1);
            PoolMgr.Instance.Release(gameObject);
        }
    }
    protected virtual List<Transform> AllEnemy(Transform checktransform)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(checktransform.position, 50f);
        //float closestDistance = Mathf.Infinity;
        List<Transform> closestEnemy = new List<Transform>();
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null && !hit.GetComponent<CharacterStats>().isDead &&hit.GetComponent<Enemy>().attackLayerName == "Player")
            {
                closestEnemy.Add(hit.transform);
            }
        }
        return closestEnemy;
    }
}
