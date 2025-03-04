using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BlackhoieHotKeyController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private KeyCode hotKey;
    private TextMeshProUGUI hotKeyText;

    private Transform myEnemy;
    private BlackhoieController myBlackhole;
    public void SetHotKey(KeyCode key, Transform enemy, BlackhoieController blackhole)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        hotKeyText= GetComponentInChildren<TextMeshProUGUI>();

        myEnemy = enemy;
        myBlackhole = blackhole;

        hotKey = key;
        hotKeyText.text = key.ToString();
    }

    void Start()
    {

    }
    void Update()
    {
        if (Input.GetKeyDown(hotKey))
        {
            myBlackhole.AddEnemyToList(myEnemy);
            hotKeyText.color = Color.clear;
            spriteRenderer.color = Color.clear;
        }
    }

}
