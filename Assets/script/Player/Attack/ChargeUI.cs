using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class ChargeUI : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Slider chargeSlider;
    [SerializeField] private Image fillImage;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Color Settings")]
    [SerializeField] private Color baseColor = Color.white;
    [SerializeField] private Color stage1Color = Color.yellow;
    [SerializeField] private Color stage2Color = Color.red;
    [SerializeField] private Color stage3Color = Color.magenta;

    [Header("Super Charge Settings")]
  //  [SerializeField] private Image superChargeIndicator;
    [SerializeField] private Color readyColor = Color.red;
    [SerializeField] private Color activeColor = new Color(0.3f, 0, 0, 1);
    [SerializeField] private float flashSpeed = 10f;

    private bool isSuperChargeActive;


    [Header("Animation Settings")]
    [SerializeField] private float fadeSpeed = 5f;

    private void Awake()
    {
        chargeSlider.value = 0;
        canvasGroup.alpha = 0;
    }
    private void Update()
    {
        if (canvasGroup.alpha > 0 && chargeSlider.value == 0)
        {
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 0, fadeSpeed * Time.deltaTime);
        }
    }
    public void PulseSuperChargeWarning()
    {
        if (!isSuperChargeActive)
        {
            StartCoroutine(SuperChargeFlash());
        }
    }

    private IEnumerator SuperChargeFlash()
    {
        isSuperChargeActive = true;
        float timer = 0;
        //bool superChargeAvailable = PlayerManager.instance.player.chargeAttackState.superChargeAvailable;
        while (PlayerManager.instance.player.chargeAttackState.superChargeAvailable)
        {
            float lerpValue = Mathf.PingPong(timer * flashSpeed, 1);
            fillImage.color = Color.Lerp(readyColor, activeColor, lerpValue);
            timer += Time.deltaTime;
            yield return null;
        }

        fillImage.color = Color.clear;
        isSuperChargeActive = false;
    }

public void UpdateChargeUI(float progress, int stage)
    {
        chargeSlider.value = progress;
        UpdateColor(stage);
    }

    public void ShowUI()
    {
        canvasGroup.alpha = 1;
    }

    public void HideUI()
    {
        canvasGroup.alpha = 0;
    }

    private void UpdateColor(int stage)
    {
        fillImage.color = stage switch
        {
            3 => stage3Color,
            2 => stage2Color,
            1 => stage1Color,
            _ => baseColor
        };
    }
}
