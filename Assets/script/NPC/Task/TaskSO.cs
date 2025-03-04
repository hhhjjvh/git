using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;


[CreateAssetMenu(menuName = "Task/TaskSO")]
public class TaskSO : ScriptableObject
{
    public string taskID;          // ����Ψһ��ʶ
    public string taskName;        // ��������
    [TextArea] public string description; // ��������
    public TaskStatus status = TaskStatus.NotStarted; // ����״̬
    public TaskSO[] prerequisites; // ǰ��������������
    [Header("��������")]
    public TaskCondition[] conditions; // ֧�ֶ������
    [Header("�ű����¼�")]
    public EventEffect[] onAcceptedEffects; // ��ȡ����ʱ�������¼�
    public EventEffect[] onCompletedEffects; // �������ʱ�������¼�

    public enum TaskStatus
    {
        NotStarted,   // δ��ȡ
        InProgress,   // ������
        Completed     // �����
    }
    private void OnValidate()
    {


#if UNITY_EDITOR
        if (!EditorApplication.isPlaying)
        {
            string path = AssetDatabase.GetAssetPath(this);
            taskID = AssetDatabase.AssetPathToGUID(path);
        }
#endif
    }
    public void ResetProgress()
    {
        foreach (var condition in conditions)
        {
            condition.ResetProgress();
        }
        status = TaskStatus.NotStarted;
    }
}