using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    private entity entity=> GetComponentInParent<entity>();
    private RectTransform rectTransform => GetComponent<RectTransform>();
    private Slider slider;
    private CharacterStats myStats;
    [SerializeField] private TextMeshProUGUI healtext;
    [SerializeField] private TextMeshProUGUI lvtext;
    
    // Start is called before the first frame update
    void Awake()
    {
        //rectTransform = GetComponent<RectTransform>();
       // entity = GetComponentInParent<entity>();
        slider = GetComponentInChildren<Slider>();
        myStats = GetComponentInParent<CharacterStats>();
        entity.onFlipped+=FlipUI;
        //myStats.OnHealthChanged+=UpdateHealthUI;

        UpdateHealthUI();
    }


    // Update is called once per frame
    void Update()
    {
        healtext.text = myStats.health.ToString() + "/" + myStats.GetMaxHealth().ToString();
        lvtext.text ="Lv:"+ myStats.GetLevel().ToString();
        UpdateHealthUI();
        //if(entity.GetComponent<Transform>().rotation!= rectTransform.rotation)
        {
            //rectTransform.rotation = entity.GetComponent<Transform>().rotation;
            //FlipUI();
        }
    }
    private void UpdateHealthUI()
    {
        slider.maxValue= myStats.GetMaxHealth();
        slider.value = myStats.health;
    }
    public void FlipUI()=> rectTransform.Rotate(0, 180, 0);
    void OnEnable()
    {
       // entity.onFlipped += FlipUI;
        myStats.OnHealthChanged += UpdateHealthUI;
    }
    private void OnDisable()
    {
       // entity.onFlipped -= FlipUI;
        myStats.OnHealthChanged -= UpdateHealthUI;
    }
    

}
