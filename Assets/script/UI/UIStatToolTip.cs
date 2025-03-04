using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIStatToolTip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI description;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetDescription(string desc)
    {
        Vector2 mosePosition = Input.mousePosition;
        float xOffset = 0;
        float yOffset = 0;
        if (mosePosition.x > Screen.width / 2)
        {
            xOffset = -200;
        }
        else
        {
            xOffset = 200;
        }
        if (mosePosition.y > Screen.height / 2)
        {
            yOffset = -200;
        }
        else
        {
            yOffset = 200;
        }
        transform.position = new Vector3(mosePosition.x + xOffset, mosePosition.y + yOffset, 0);

        description.text = desc;
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        description.text = "";
        gameObject.SetActive(false);
    }
}
