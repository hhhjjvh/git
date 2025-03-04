using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;


[CreateAssetMenu(menuName = "Event/Effects/TaskEvent")]
public class TaskEventEffect : EventEffect
{
    [Header("�����¼�����")]
    public TaskEventType eventType = TaskEventType.OnAccepted;

    [Header("Ч������")]
    public GameObject spawnPrefab;   // ���ɶ����罱�����ߣ�
    public AudioClip soundEffect;    // ������Ч
    public string uiMessage;         // ��ʾ��UI�ı�

    public enum TaskEventType
    {
        OnAccepted,  // ��ȡ����ʱ����
        OnCompleted  // �������ʱ����
    }

    public override void ApplyEffect(GameObject target)
    {
        // ʵ�־���Ч��
        if (soundEffect != null)
        {
            AudioSource.PlayClipAtPoint(soundEffect, target.transform.position);
        }

        if (spawnPrefab != null)
        {
            Instantiate(spawnPrefab, target.transform.position, Quaternion.identity);
        }

        //if (!string.IsNullOrEmpty(uiMessage))
        //{
        //    UIManager.Instance.ShowMessage(uiMessage);
        //}
    }
}
