using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiFadeScreen : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeSpeed = 1f;

    public async UniTask FadeOut(float duration)
    {
        float startAlpha = canvasGroup.alpha;
        float elapsed = 0;
        while (elapsed < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 1, elapsed / duration);
            elapsed += Time.deltaTime;
            await UniTask.Yield();
        }
        canvasGroup.alpha = 1;
    }

    public async UniTask FadeIn(float duration)
    {
        float startAlpha = canvasGroup.alpha;
        float elapsed = 0;
        while (elapsed < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0, elapsed / duration);
            elapsed += Time.deltaTime;
            await UniTask.Yield();
        }
        canvasGroup.alpha = 0;
    }
}
