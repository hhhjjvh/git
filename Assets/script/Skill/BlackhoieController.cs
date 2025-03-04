using System.Collections.Generic;
using UnityEngine;

public class BlackhoieController : MonoBehaviour
{
    [SerializeField] private GameObject hotKey;
    [SerializeField] private List<KeyCode> hotKeys;

    private float maxSize;
    private float growSpeed;
    private float shrinkSpeed;
    private float blackholeDuration;
    private bool canGrow = true;
    private bool canShrink;
    private bool playerCanDisapear = true;

    

    private bool canCreateHotKey = true;
    private bool canAttack;
    private int amountOfAttacks;
    private float cloneAttackCooldown;
    private float cloneAttackTimer;
    private float canAttackTimer = .6f;

    private List<Transform> tragets = new List<Transform>();
    private List<GameObject> crertedhotKeyList = new List<GameObject>();

    public bool playerCanExitState { get; private set; }

    public void SetupBlackhole(float maxSize, float growSpeed, float shrinkSpeed, int amountOfAttacks, float cloneAttackCooldown, float blackholeDuration)
    {
        this.maxSize = maxSize;
        this.growSpeed = growSpeed;
        this.shrinkSpeed = shrinkSpeed;
        this.amountOfAttacks = amountOfAttacks;
        this.cloneAttackCooldown = cloneAttackCooldown;
        this.blackholeDuration = blackholeDuration;

    }
    // Start is called before the first frame update

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        cloneAttackTimer -= Time.deltaTime;
        blackholeDuration -= Time.deltaTime;
        canAttackTimer -= Time.deltaTime;
        if (blackholeDuration <= 0)
        {
            blackholeDuration = Mathf.Infinity;
            if (tragets.Count > 0)
            {
                ReleaseCloneAttack();
            }
            else
            {
                FinishBlackHoleAbility();
            }
        }
        if (canAttackTimer <= 0)
        {
            ReleaseCloneAttack();
        }

        CloneAttackLogic();

        if (canGrow && !canShrink)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(maxSize, maxSize), Time.deltaTime * growSpeed);
        }
        if (canShrink)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(0, 0), Time.deltaTime * shrinkSpeed);
            if (transform.localScale.x <= 0.1f)
            {
                Destroy(gameObject);
            }
        }
       

    }

    private void ChackTragets()
    {
        for (int i = 0; i < tragets.Count; i++)
        {
            if (tragets[i] == null) tragets.RemoveAt(i);
        }
    }

    private void ReleaseCloneAttack()
    {
        if (tragets.Count <= 0) return;
        canAttack = true;
        DestroyHotKeys();
        canCreateHotKey = false;
        if (playerCanDisapear)
        {
            playerCanDisapear = false;
            PlayerManager.instance.player.MakeTransprent(true);
        }
    }

    private void CloneAttackLogic()
    {
        

        if (cloneAttackTimer <= 0 && canAttack && amountOfAttacks > 0)
        {
            
            cloneAttackTimer = cloneAttackCooldown;
            float offset = 0;
            if (Random.Range(0, 100) > 50)
            {
                offset = 2;
            }
            else
            {
                offset = -2;
            }
            if (tragets.Count != 0)
            {
                ChackTragets();
                if (tragets.Count == 0) return;
                SkillManager.instance.clone.CreateClone(tragets[Random.Range(0, tragets.Count)], new Vector2(offset, 0));
            }
            else
            {
                Invoke(nameof(FinishBlackHoleAbility), 1f);
            }
            amountOfAttacks--;
            //Debug.Log(amountOfAttacks);
            if (amountOfAttacks <= 0)
            {
                //FinishBlackHoleAbility();
                Invoke(nameof(FinishBlackHoleAbility), 1f);
            }
        }
    }

    private void FinishBlackHoleAbility()
    {
        DestroyHotKeys();
        playerCanExitState = true;
        //PlayerManager.instance.player.ExitBlackHoleAbility();
        canShrink = true;
        canAttack = false;
    }

    private void DestroyHotKeys()
    {
        if (crertedhotKeyList.Count <= 0) return;

        foreach (var item in crertedhotKeyList)
        {
            Destroy(item);
        }
    }
   
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //tragets = new List<Transform>();
        if (collision.GetComponent<Enemy>() != null && !collision.GetComponent<CharacterStats>().isDead)
        {
           //Debug.Log(tragets.Count);
            tragets.Add(collision.transform);
            collision.GetComponent<Enemy>().FreezeTimer(true);
            GreateHotKey(collision);
            if (tragets.Count <= 6)
            {
                cloneAttackCooldown *= (float)6 / (tragets.Count + 5);
                amountOfAttacks += 5;
            }

        }

    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<CharacterStats>() != null && collision.GetComponent<CharacterStats>().isDead)
        {
           
            RemoveEnemyFromList(collision.transform);
            
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            collision.GetComponent<Enemy>().FreezeTimer(false);
        }
    }

    private void GreateHotKey(Collider2D collision)
    {
        if (hotKeys.Count <= 0 || !canCreateHotKey) return;
        GameObject go = Instantiate(hotKey, collision.transform.position + new Vector3(0, 2), Quaternion.identity);
        crertedhotKeyList.Add(go);

        KeyCode key = hotKeys[Random.Range(0, hotKeys.Count)];
        hotKeys.Remove(key);
        BlackhoieHotKeyController bhk = go.GetComponent<BlackhoieHotKeyController>();
        bhk.SetHotKey(key, collision.transform, this);
    }

    public void AddEnemyToList(Transform enemy) => tragets.Add(enemy);
    public void RemoveEnemyFromList(Transform enemy) => tragets.Remove(enemy);
}
