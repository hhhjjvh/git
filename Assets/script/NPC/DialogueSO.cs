using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;



[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/DialogueSO")]
public class DialogueSO : ScriptableObject
{
    public enum SystemType { Legacy, NodeBased }

    [Header("System Settings")]
    public SystemType systemType = SystemType.NodeBased;

    [Header("Node Based System")]
    public DialogueNode[] nodes; // �Ի��ڵ�����
    [NonSerialized] public int currentNodeIndex; // ��ǰ�ڵ�����������ʱ��

    [Header("Legacy System")]
    [TextArea(1, 50)] public string dialogueData;  // һ�����ַ������������жԻ�����
    [HideInInspector] public string[] cachedLines;
    public DialogueEvent[] events;

    [Header("Common Settings")]
    public bool hasName; // �Ƿ���ʾ��ɫ��
    public EventEffect[] EndscriptableEffects;

    void OnValidate()
    {
        // �����ϵͳ����
        if (systemType == SystemType.Legacy)
        {
            cachedLines = !string.IsNullOrEmpty(dialogueData)
                ? dialogueData.Split(new[] { "/n" }, StringSplitOptions.RemoveEmptyEntries)
                              .Select(line => line.Trim()).ToArray()
                : Array.Empty<string>();
          
            for (int i = 0; i < events.Length; i++)
            {
                events[i].triggerLine = Array.FindIndex(cachedLines, line => line.Contains(events[i].triggerSymbol));

            }
        }
        
    }
    public void ResetProgress() => currentNodeIndex = 0;
}
[System.Serializable]
public class DialogueEvent
{
    [Tooltip("�ڶԻ������в���˷���ʱ����ѡ��")]
    public string triggerSymbol = "@trigger";
    public int triggerLine = -1; // �����к�
   
    public UnityEvent onReached; // �����¼�
}








