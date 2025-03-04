using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
public abstract class TaskCondition : ScriptableObject
{
    [Header("��������")]
    public string guid; // ����Ψһ��ʶ��
    public string conditionDescription; // �����������硰�ռ�5��ƻ������
    public int requiredAmount = 1;      // ��Ҫ��ɵ�����
    [NonSerialized] public int currentAmount; // ��ǰ���ȣ�����ʱ��
    public System.Action OnProgressUpdated;// �����¼�

   

    // ��ʼ��ʱ����GUID����CreateAssetMenu����ʱ�Զ����ɣ�

#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        // ���ڱ༭������ʱ���� GUID
        if (string.IsNullOrEmpty(guid))
        {
            guid = System.Guid.NewGuid().ToString();
            EditorUtility.SetDirty(this);
        }
    }
#endif
    public virtual void ResetProgress()
    {
        OnProgressUpdated?.Invoke();
        currentAmount = 0;
    }
    public abstract bool IsConditionMet();    // ��������Ƿ�����
    public abstract void RegisterListeners(); // ע���¼�����
    public abstract void UnregisterListeners(); // ע���¼�����
}
