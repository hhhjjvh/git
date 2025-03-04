using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinTrigger : MonoBehaviour
{
    public int coin = 1;
    private Rigidbody2D rb;
    private float time = 0;
    private void Awake()
    {
        rb = GetComponentInParent<Rigidbody2D>();
    }
    // Start is called before the first frame update

    void Start()
    {
        
    }
    void OnEnable()
    {
        time = 0.6f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(transform.parent.transform.position, PlayerManager.instance.player.transform.position) < 3&&time<0)
        {
            transform.parent.transform.position = Vector2.MoveTowards(transform.position,
            PlayerManager.instance.player.transform.position, Time.deltaTime * 6);
        }
        if (time >= 0)
        {
            time -= Time.deltaTime;
        }
    }
    public void SetCoin(int coin)
    {
        this.coin = coin;
        rb.velocity = new Vector2(Random.Range(-5f, 5f), Random.Range(3f,8f));
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player"&&time<0)
        {
            PlayerManager.instance.addCoin(coin);
            PoolMgr.Instance.Release(gameObject.transform.parent.gameObject);
        }
    }

}
