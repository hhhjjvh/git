using UnityEngine;

public class ItemObjectTrigger : MonoBehaviour, IInteractable
{
    private ItemObject myitemObject => GetComponentInParent<ItemObject>();
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (myitemObject != null)
                myitemObject.Name.SetActive(true);
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (myitemObject != null)
                myitemObject.Name.SetActive(false);
        }
    }


    public void TiggerAction()
    {  
        myitemObject.PickupItem();
    }

    public bool IsInteractable()
    {
        return true;
    }
}
