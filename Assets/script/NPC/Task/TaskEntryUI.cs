using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    using TMPro;
using NUnit.Framework.Interfaces;
using System.Drawing;

public class TaskEntryUI : MonoBehaviour
{
    public TMP_Text taskNameText;
    public TMP_Text descriptionText;
    public TMP_Text progressText;
   // public TMP_Text rewardsText; // ���������ı�
    private TaskSO boundTask;
    public void BindData(TaskSO task)
    {
        // ����������ʾ
        string rewards = GetRewardsDescription(task);
        taskNameText.text = task.taskName;
        descriptionText.text = $"{task.description}\n<size=30><b>����: {rewards}</b></size>";
      
        // ������������
        string progress = "";
        foreach (var condition in task.conditions)
        {
            progress += $"{condition.conditionDescription}: {condition.currentAmount}/{condition.requiredAmount}\n";
        }
        progressText.text = progress;

        boundTask = task;
       // UpdateUI();
        foreach (var condition in task.conditions)
        {
            condition.OnProgressUpdated += UpdateUI;
        }
        
        
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
        return rewardDescriptions.Count > 0 ? string.Join("     ", rewardDescriptions) : "��";
    }

    private void OnDestroy()
    {
        if (boundTask != null)
        {
            foreach (var condition in boundTask.conditions)
            {
                condition.OnProgressUpdated -= UpdateUI;
            }
        }
    }
    private float lastUpdateTime;
    private void UpdateUI()
    {
        if (Time.time - lastUpdateTime < 0.5f) return; // ÿ 0.5 �����ˢ��һ��
        lastUpdateTime = Time.time;
        // ���°�����
        BindData(boundTask);
    }
}
