using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private float speed;
    [SerializeField] private string targetLayerName = "Player";
    
    private Rigidbody2D rb;
    // [SerializeField] private bool worked;
    [SerializeField] private bool flipped=false;

    [SerializeField] private bool canmove;

    private CharacterStats characterStats;

    private Vector2 facingDirection;
    Collider2D collision;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (canmove)
        {
            if (flipped)
            {
                rb.velocity = -facingDirection;
            }
            else
            {
                rb.velocity = facingDirection;
                transform.right = rb.velocity;
            }
        }
        else if (collision != null)
        {
           // transform.position = collision.transform.position;
        }
           
        
    }
    private void StuckInto(Collider2D collision)
    {
        GetComponentInChildren<ParticleSystem>().Stop();
        GetComponent<CapsuleCollider2D>().enabled = false;
        canmove= false;
        GetComponent<Rigidbody2D>().isKinematic = true;
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        this.collision = collision;
        transform.parent = collision.transform;
        //Destroy(gameObject, 5f);
        StartCoroutine(returnpool(5f, gameObject));
    }
    private void StartInto()
    {
        GetComponentInChildren<ParticleSystem>().Play();
        GetComponent<CapsuleCollider2D>().enabled = true;
        canmove = true;
        flipped = false;
        GetComponent<Rigidbody2D>().isKinematic = false;
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
       
    }

    public void FlipArrow()
    {
       
        if (flipped) return;
        //rb.velocity = -facingDirection;
        //transform.right = rb.velocity;
        transform.Rotate(0, 180, 0);
        flipped = true;
        targetLayerName = "Enemy";
    }
    public void Setup(CharacterStats characterStats,Vector2 facingDirection)
    {
        this.characterStats = characterStats;
       this.facingDirection = facingDirection;
        targetLayerName = characterStats.GetComponent<Enemy>().attackLayerName;
        StartCoroutine(returnpool(7f, gameObject));
        StartInto();

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log(collision.gameObject.name);
        if (collision.gameObject.layer == LayerMask.NameToLayer(targetLayerName)&&collision.GetComponent<CharacterStats>()!=null&& characterStats!=null)
        {
            characterStats.DoDamage(collision.GetComponent<CharacterStats>(), characterStats.GetComponent<Enemy>());
            StuckInto(collision);

        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            StuckInto(collision);
        }

    }
    IEnumerator returnpool(float time, GameObject obj)
    {
        yield return new WaitForSeconds(time);
        //PoolManager.instance.ReturnToControllerPool(obj);
        PoolMgr.Instance.Release(obj);

    }
}
