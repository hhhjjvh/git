using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskPanelUI : MonoBehaviour
{
    [SerializeField] private Transform taskListContainer;
    [SerializeField] private GameObject taskEntryPrefab;

    private void OnEnable()
    {
        UpdateTaskList();
        TaskManager.Instance.OnTaskUpdated += UpdateTaskList;
    }

    private void OnDisable()
    {
        TaskManager.Instance.OnTaskUpdated -= UpdateTaskList;
    }

    public void UpdateTaskList()
    {
        // ��վ���Ŀ
        foreach (Transform child in taskListContainer)
        {
            // Destroy(child.gameObject);
            PoolMgr.Instance.Release(child.gameObject);
        }
       //ȷ��ɾ��
        taskListContainer.DetachChildren();


        // ��̬��������Ŀ
        foreach (var task in TaskManager.Instance.activeTasks)
        {
            GameObject entry = PoolMgr.Instance.GetObj("TaskEntryUI");//Instantiate(taskEntryPrefab, taskListContainer);
            entry.transform.SetParent(taskListContainer);
            entry.GetComponent<TaskEntryUI>().BindData(task);
        }
    }
}
