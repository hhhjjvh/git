using UnityEngine;
using System.Collections;

public class ThunderStrikeContorller : MonoBehaviour
{
    [SerializeField] private CharacterStats characterState;
    [SerializeField] private float speed;

    private int damage;


    private Animator animator;
    private Animator oldanimator;
   
    private bool triggered = false;
    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();

        oldanimator = animator;
       

    }
    // Start is called before the first frame update
    void Start()
    {
        //animator = GetComponentInC<Animator>();

    }
    public void Setup(CharacterStats characterState, int damage)
    {
        animator = oldanimator;
        animator.transform.localPosition = new Vector3(0, .3f);
        animator.transform.localRotation = Quaternion.Euler(-8.303f, 9.788f, 90);

        transform.localScale = new Vector3(1, 1);
        this.characterState = characterState;
        this.damage = damage;

        triggered = false;
        StartCoroutine(returnpool(5f, gameObject));
    }


    // Update is called once per frame
    void Update()
    {
        if (triggered|| characterState==null) return;
        transform.position = Vector2.MoveTowards(transform.position, characterState.transform.position, speed * Time.deltaTime);
        transform.right = transform.position - characterState.transform.position;
        if (Vector2.Distance(transform.position, characterState.transform.position) < 1f)
        {
            animator.transform.localPosition = new Vector3(0, .5f);
            animator.transform.localRotation = Quaternion.identity;
            transform.localRotation = Quaternion.identity;
            transform.localScale = new Vector3(3, 3);

            characterState.DoMagicDamage(damage);
            animator.SetTrigger("Hit");
            triggered = true;
        }

    }
    IEnumerator returnpool(float time, GameObject obj)
    {
        yield return new WaitForSeconds(time);
        //PoolManager.instance.ReturnToControllerPool(obj);
        PoolMgr.Instance.Release(obj);

    }
}
