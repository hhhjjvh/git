using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class TaskCompletedUI : MonoBehaviour
{
    public TMP_Text taskNameText;
   // public TMP_Text rewardsText;
    public CanvasGroup canvasGroup;
    private float showDuration = 10f;

    public void Show(TaskSO task)
    {
        gameObject.SetActive(true);
        canvasGroup.alpha = 1;
       
        string rewards = GetRewardsDescription(task);
        taskNameText.text = 
         $"�������{task.taskName}\n<size=20><b>��ý���: {rewards}</b></size>";
        StartCoroutine(FadeOut());
    }

    private string GetRewardsDescription(TaskSO task)
    {
        List<string> rewardDescriptions = new List<string>();
        foreach (var effect in task.onCompletedEffects)
        {
            if (effect is EventEffect eventEffect)
            {
                rewardDescriptions.Add(eventEffect.GetEffectDescription());
            }
        }
        return rewardDescriptions.Count > 0 ? string.Join("     ", rewardDescriptions) : "�޽���";
    }

    private IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(showDuration);

        // ��ѡ����ӵ�������
        float fadeDuration = 1f;
        float elapsed = 0;
        while (elapsed < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(1, 0, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        gameObject.SetActive(false);
        
        //Destroy(gameObject);
    }
}