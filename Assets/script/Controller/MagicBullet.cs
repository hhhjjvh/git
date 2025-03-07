using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class MagicBullet : MonoBehaviour
{
    player1 player;
    Transform closeenemy;
    public float rotationSpeed = 10f;  // ��ת�ٶ�
    public float maxSpeed = 4f;  // �������ٶ�
    public float minSpeed = 3f;   // ��С�����ٶ�
    public float rotationLag = 0.5f;  // ��ת�ӳ�ʱ�䣬Խ��Խ�ͺ�

    private Vector3 velocity = Vector3.zero;  // ����ƽ���ƶ����ٶȱ���
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
      
        // 4. ��������������ľ���
        //float distance = Vector3.Distance(transform.position, closeenemy.position);

        //// 5. ���ݾ�����������ٶ�
        //float currentSpeed = Mathf.Lerp(minSpeed, maxSpeed, 1 / distance); // ����ԽԶ���ٶ�Խ�죬��֮Խ��
        float currentSpeed = speed*Random.Range(0.8f, 1.2f);

        // 6. ʹ�� SmoothDamp ��ƽ���ƶ��������ͺ�Ч��
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
