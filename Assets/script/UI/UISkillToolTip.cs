using NUnit.Framework.Interfaces;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UISkillToolTip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI skillText;
    [SerializeField] private TextMeshProUGUI skillName;
    [SerializeField] private TextMeshProUGUI skillCost;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ShowToolTip(string Descprtion, string skillName,int cost)
    {
        string combinedText =

    $"<size=50><b>{skillName}</b></size>\n" +
    $"<size=40><color=#AAAAAA>{Descprtion}</color></size>\n" +  // ��ɫ��Ʒ����  
    $"<size=45><u>{"���:" + cost.ToString()}</u></size>";  // �»�����ƷЧ��


        this.skillName.text = combinedText; //skillName;
        skillText.text = Descprtion;
        skillCost.text = "���:" + cost.ToString();


        gameObject.SetActive(true);
    }
    public void HideToolTip()
    {
        gameObject.SetActive(false);
    }
}
