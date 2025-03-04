using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBallTrigger : MonoBehaviour
{
    public int count = 8;
   // private Rigidbody2D rb;
    private void Awake()
    {
       // rb = GetComponentInParent<Rigidbody2D>();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.parent.transform.position = Vector2.MoveTowards(transform.position, PlayerManager.instance.player.transform.position, Time.deltaTime * 6);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            AudioManager.instance.PlaySFX(24, null);
            collision.gameObject.GetComponent<PlayerStats>().addMana(count);
            PoolMgr.Instance.Release(gameObject.transform.parent.gameObject);
        }
    }
}
