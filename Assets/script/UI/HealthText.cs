using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealthText : MonoBehaviour
{
    private TextMeshPro text;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {
        // text.text = PlayerManager.instance.player.stats.health.ToString()+"/"+ PlayerManager.instance.player.stats.GetMaxHealth().ToString();
        text.text = " 223";
    }
}
