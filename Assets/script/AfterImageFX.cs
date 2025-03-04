using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImageFX : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private float colorLooseRate;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        float alpha = spriteRenderer.color.a-colorLooseRate*Time.deltaTime;
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alpha);

        if (spriteRenderer.color.a <= 0)
        {
            // Destroy(gameObject);
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1);
            //PoolManager.instance.ReturnToFXPool(gameObject);
            PoolMgr.Instance.Release(gameObject);
        }
    }
    public void SetupAfterImage(float colorLooseRate, Sprite sprite)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        this.colorLooseRate = colorLooseRate;

    }
}
