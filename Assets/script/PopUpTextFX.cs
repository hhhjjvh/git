using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopUpTextFX : MonoBehaviour
{
    private TextMeshPro textMesh;

    [SerializeField] private float speed;
    [SerializeField] private float desapearceSpeed;
    [SerializeField] private float colorDespearance;

    [SerializeField] private float lifetime;
    private float textTimer;
    // Start is called before the first frame update
    void Start()
    {
        textMesh=GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {
        
        transform.position=Vector3.Lerp(transform.position,new Vector2( transform.position.x ,transform.position.y+1) , Time.deltaTime * speed);
        textTimer-=Time.deltaTime;
        if(textTimer<=0)
        {
            float alpha=textMesh.color.a-colorDespearance*Time.deltaTime;
            textMesh.color=new Color(textMesh.color.r,textMesh.color.g,textMesh.color.b,alpha);
            if(textMesh.color.a<50)
            {
                speed=desapearceSpeed;
            }
            if(textMesh.color.a<=0)
            {
                textMesh.color=new Color(textMesh.color.r,textMesh.color.g,textMesh.color.b,1);
                //PoolManager.instance.ReturnToFXPool(gameObject);
                PoolMgr.Instance.Release(gameObject);
                //Destroy(gameObject);
            }
        }
    }
}
