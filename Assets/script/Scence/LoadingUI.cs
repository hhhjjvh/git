using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingUI : MonoBehaviour
{
    [SerializeField] private Slider progressBar;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private Button cancelButton;

    public void UpdateProgress(float value)
    {
        progressBar.value = value;
        progressText.text = $"{value * 100:F0}%";
    }

    public void ShowCancelButton(Action callback)
    {
        cancelButton.gameObject.SetActive(true);
        cancelButton.onClick.AddListener(() => callback?.Invoke());
    }

    public void HideCancelButton()
    {
        cancelButton.gameObject.SetActive(false);
        cancelButton.onClick.RemoveAllListeners();
    }
}

