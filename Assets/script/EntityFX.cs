using Cinemachine;

using System.Collections;
using TMPro;
using UnityEngine;


public class EntityFX : MonoBehaviour
{
    private player1 player;
    private SpriteRenderer spriteRenderer;
    [Header("Screen shake fx")]
    private CinemachineImpulseSource shakeSource;
    [SerializeField] private float shakeMultiplier;
    public Vector3 shakeSwordPower;
    public Vector3 shakeAttackPower;

    //[Header("Pop UP text")]
    //[SerializeField] private GameObject popUpText;


    [Header("Flash FX")]
    [SerializeField] private Material hitMat;
    [SerializeField] private float flashDuration;
    private Material originalMat;

    [Header("Ailment colors")]
    [SerializeField] private Color[] chillColor;
    [SerializeField] private Color[] igniteColor;
    [SerializeField] private Color[] shockColor;

    [SerializeField] private ParticleSystem igniteParticle;
    [SerializeField] private ParticleSystem chillParticle;
    [SerializeField] private ParticleSystem shockParticle;

    //[SerializeField] private GameObject hitFX;
   // [SerializeField] private GameObject criticalHitFX;


    //[SerializeField] private GameObject AfterImage;
    [SerializeField] private float colorLooseRate;
    [SerializeField] private float AfterImageCooldown;
    private float afterImageTimer;
    #region Charge Particles
    [Header("Charge Particles")]
    [SerializeField] private ParticleSystem chargeBaseParticles;
    [SerializeField] private ParticleSystem chargeStage1Particles;
    [SerializeField] private ParticleSystem chargeStage2Particles;
    [SerializeField] private ParticleSystem chargeStage3Particles;
    [SerializeField] private ParticleSystem chargeCompleteParticles;

    [Header("Attack Particles")]
    [SerializeField] private ParticleSystem attackImpactParticles;
    [SerializeField] private ParticleSystem attackTrailParticles;
    // 蓄力特效控制

    public void StartChargeEffect(int stage)
    {
        StopAllChargeParticles();

        switch (stage)
        {
            case 1:
                chargeBaseParticles.Play();
                chargeStage1Particles.Play();
                break;
            case 2:
                chargeStage1Particles.Stop();
                chargeStage2Particles.Play();
                break;
            case 3:
                chargeStage2Particles.Stop();
                chargeStage3Particles.Play();
                break;
        }
    }

    public void StopChargeEffect(bool immediate = false)
    {
        if (immediate)
        {
            chargeBaseParticles.Stop();
            chargeStage1Particles.Stop();
            chargeStage2Particles.Stop();
            chargeStage3Particles.Stop();
        }
        else
        {
            chargeBaseParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            chargeStage1Particles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            chargeStage2Particles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            chargeStage3Particles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }

    public void PlayChargeCompleteEffect()
    {
        chargeCompleteParticles.Play();
    }

    // 攻击特效控制
    public void PlayAttackImpact(Vector3 position)
    {
        attackImpactParticles.transform.position = position;
        attackImpactParticles.Play();
    }

    public void StartAttackTrail()
    {
        attackTrailParticles.Play();
    }

    public void StopAttackTrail()
    {
        attackTrailParticles.Stop();
    }

    private void StopAllChargeParticles()
    {
        chargeBaseParticles.Stop();
        chargeStage1Particles.Stop();
        chargeStage2Particles.Stop();
        chargeStage3Particles.Stop();
        chargeCompleteParticles.Stop();
       
    }

#endregion
    // Start is called before the first frame update
    void Start()
    {

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        originalMat = spriteRenderer.material;
        player = PlayerManager.instance.player;
        shakeSource = GetComponent<CinemachineImpulseSource>();

    }
    void Update()
    {
        afterImageTimer -= Time.deltaTime;
    }

    public void CreatePopUpText(string text, Color color)
    {
        float randx = Random.Range(-0.1f, 0.1f);
        float randy = Random.Range(-1.2f, -0.8f);
        Vector3 pos = new Vector3(randx, randy, 0);
        //GameObject popUp = PoolManager.instance.GetFXFromPool("poptext");

        //popUp.transform.position = transform.position + pos;
        //popUp.transform.rotation = Quaternion.identity;
        GameObject popUp = PoolMgr.Instance.GetObj("poptext", transform.position + pos, Quaternion.identity);

        //GameObject popUp = Instantiate(popUpText, transform.position + pos, Quaternion.identity);
        popUp.GetComponent<TextMeshPro>().text = text;
        popUp.GetComponent<TextMeshPro>().color = color;
       

    }
    public void ScreenShake(Vector3 shakePower)
    {
        shakeSource.m_DefaultVelocity = new Vector3(shakePower.x * player.facingDirection, shakePower.y) * shakeMultiplier;
        shakeSource.GenerateImpulse();
    }
    public void ScreenShake(float x, float y)
    {
        shakeSource.m_DefaultVelocity = new Vector3(x, y) * shakeMultiplier;
        shakeSource.GenerateImpulse();
    }
    public void CreatAfterImage()
    {
        if (afterImageTimer <= 0)
        {
            afterImageTimer = AfterImageCooldown;
           // GameObject afterImage = PoolManager.instance.GetFXFromPool("AfterImage");
           //// Instantiate(AfterImage, transform.position, transform.rotation);
           // afterImage.transform.position = transform.position;
           // afterImage.transform.rotation = transform.rotation;
           GameObject afterImage = PoolMgr.Instance.GetObj("AfterImage", transform.GetComponent<entity>().anim.transform.position, transform.rotation);
            afterImage.GetComponent<AfterImageFX>().SetupAfterImage(colorLooseRate, spriteRenderer.sprite);
        }
    }
    public IEnumerator FlashFX()
    {

       // spriteRenderer.material = hitMat;
        //Color originalColor = spriteRenderer.color;
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(flashDuration);
        //spriteRenderer.color = originalColor;
        //spriteRenderer.material = originalMat;

    }
    public void RedColorBlink()
    {
        if (spriteRenderer.color != Color.white)
        {
             spriteRenderer.color = Color.white;
        }
        else
        {
            // spriteRenderer.color = Color.red;
        }
    }
    public void CancelRedBlink()
    {
        CancelInvoke();
        spriteRenderer.color = Color.white;

        igniteParticle.Stop();
        chillParticle.Stop();
        shockParticle.Stop();
        // Debug.Log("CancelRedBlink");
    }
    // Update is called once per frame


    public void IgniteFxFor(float duration)
    {
        if (!igniteParticle.isPlaying)
        {
            igniteParticle.Play();
        }
        // Debug.Log("Ignite34255");
        InvokeRepeating("IgniteColorFx", 0, .3f);
        Invoke("CancelRedBlink", duration);
    }
    public void ChillFxFor(float duration)
    {
        if (!chillParticle.isPlaying)
        {
            chillParticle.Play();
        }
        InvokeRepeating("ChillColorFx", 0, .3f);
        Invoke("CancelRedBlink", duration);
        // Debug.Log("ChillFxFor");
    }
    public void ShockFxFor(float duration)
    {
        if (!shockParticle.isPlaying)
        {
            shockParticle.Play();
        }
        InvokeRepeating("ShockColorFx", 0, .3f);
        Invoke("CancelRedBlink", duration);

    }
    private void IgniteColorFx()
    {
        // Debug.Log("Ignite34255");
        if (spriteRenderer.color != igniteColor[0])
        {
            spriteRenderer.color = igniteColor[0];
            //Debug.Log("Red");
        }
        else
        {
            spriteRenderer.color = igniteColor[1];
            // Debug.Log("Blue");
        }
    }
    private void ChillColorFx()
    {

        //Debug.Log("ChillColorFx");
        if (spriteRenderer.color != chillColor[0])
        {
            spriteRenderer.color = chillColor[0];
        }
        else
        {
            spriteRenderer.color = chillColor[1];
        }

    }
    private void ShockColorFx()
    {
        if (spriteRenderer.color != shockColor[0])
        {
            spriteRenderer.color = shockColor[0];
        }
        else
        {
            spriteRenderer.color = shockColor[1];
        }

    }

    public void CreatHitFX(Transform hitPoint, bool critical)
    {
        float zRotation = Random.Range(-90, 90);
        float xPosition = Random.Range(-.5f, .5f);
        float yPosition = Random.Range(-.5f, .5f);

        Vector3 hitFXRotaion = new Vector3(0, 0, zRotation);

        GameObject hitPrefab;
        if (critical)
        {
           // hitPrefab = PoolManager.instance.GetFXFromPool("FX024_1");
            

            float yRotation = 0;
            zRotation = Random.Range(-45, 45);
            if (GetComponent<entity>().facingDirection == -1)
            {
                yRotation = 180;
            }
            hitFXRotaion = new Vector3(0, yRotation, zRotation);
            hitPrefab = PoolMgr.Instance.GetObj("FX024_1", hitPoint.position + new Vector3(xPosition, yPosition, 0), Quaternion.Euler(hitFXRotaion));
            //StartCoroutine(returnpool(0.5f, hitPrefab));
        }
        else
        {
           // hitPrefab = PoolManager.instance.GetFXFromPool("SP103_01");
          //  hitPrefab = PoolMgr.Instance.GetObj("SP103_01", transform.position, Quaternion.identity);
            hitPrefab = PoolMgr.Instance.GetObj("SP103_01", hitPoint.position + new Vector3(xPosition, yPosition, 0), Quaternion.Euler(hitFXRotaion));
           // StartCoroutine(returnpool(0.25f, hitPrefab));
        }
        //hitPrefab.transform.position = hitPoint.position + new Vector3(xPosition, yPosition, 0);
        //hitPrefab.transform.rotation = Quaternion.Euler(hitFXRotaion);
        

        //GameObject hitFXInstance = Instantiate(hitPrefab, hitPoint.position + new Vector3(xPosition, yPosition, 0), Quaternion.Euler(hitFXRotaion), hitPoint);

        
        
        
    }

    IEnumerator returnpool(float time,GameObject obj)
    {
        yield return new WaitForSeconds(time);
        // PoolManager.instance.ReturnToFXPool(obj);
        PoolMgr.Instance.Release(obj);

    }

}
