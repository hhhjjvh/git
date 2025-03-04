
using UnityEngine;

public class WhiteTreasureBox : ITreasureBox
{
    private GameObject ballPoint;
    private AnimatorStateInfo info;
    private bool isCreateItem;
    protected override void Start()
    {
        base.Start();
        ballPoint = transform.Find("WeaponCreatePoint").gameObject;
    }
    protected override void Update()
    {
        if (!isFinish)
        {
            if (isPlayerEnter && !isCreateItem)
            {
                isCreateItem = true;
                m_Animator.enabled = true;
            }
            info = m_Animator.GetCurrentAnimatorStateInfo(0);
            if (info.normalizedTime > 1 && !isFinish)
            {
                isFinish = true;
                int count1 = Random.Range(3, 7);
                for (int i = 0; i < count1; i++)
                {
                    GameObject Copper = PoolMgr.Instance.GetObj("EnergyBall", new Vector2(Random.Range(-2f, 2f) + transform.position.x,
                        Random.Range(0, 2) + transform.position.y), Quaternion.identity);
                    Copper.transform.SetParent(transform.parent);
                    // ItemFactory.Instance.GetItem(ItemType.EnergyBall, (Vector2)ballPoint.transform.position + Random.insideUnitCircle).AddToController();
                }
                int count2 = Random.Range(5, 10);
                for (int i = 0; i < count2; i++)
                {   //ItemFactory.Instance.GetCoin(CoinType.Coppers, (Vector2)ballPoint.transform.position + Random.insideUnitCircle * 2).AddToController();
                    GameObject Copper = PoolMgr.Instance.GetObj("Coppers", new Vector2(Random.Range(-2f, 2f) + transform.position.x,
                        Random.Range(0, 2) + transform.position.y), Quaternion.identity);
                    Copper.GetComponentInChildren<CoinTrigger>().SetCoin(Random.Range(1, 10));
                    Copper.transform.SetParent(transform.parent);

                }
            }
        }
    }
}
