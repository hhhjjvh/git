using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Reflection;
using System.IO;

public class SaveManager : MonoBehaviour
{
    private GameData data;
    public static SaveManager instance;

    private string saveFileNameTemplate = "save_slot{0}.json"; // �ļ���ģ��
    [SerializeField] private bool encryptData;
    //private FileDataHandler fileDataHandler;

    private List<ISaveManager> saveManagers;
    //public int selectedSlotIndex=1;

    [ContextMenu("DeleteSavedData")]
    public void DeleteAllGame()
    {
        for (int i = 1; i <= 5; i++)
        {
           DeleteSavedData(i);
        }
    }
    public void DeleteSavedData(int slotIndex)
    {
        string fileName = string.Format(saveFileNameTemplate, slotIndex);
        FileDataHandler fileDataHandler = new FileDataHandler(Application.persistentDataPath, fileName, encryptData);
        fileDataHandler.DeleteData();
    }

    private void Awake()
    {
       // DeleteSavedData();
        if (instance != null)
        {
            Destroy(instance.gameObject);
        }
        instance = this;
        //DontDestroyOnLoad(gameObject);
        //��ǰ���λ��
        //fileDataHandler = new FileDataHandler(Application.dataPath, saveFileName);

        saveManagers = FindAllSaveManagers();
       LoadGame();
    }
    void Start()
    {
        
    }
   
    private void OnApplicationQuit()
    {
        SaveGame();
    }

    public void NewGame()
    {
        data = new GameData();
    }
    public void SaveGame(int slotIndex)
    {
        string fileName = string.Format(saveFileNameTemplate, slotIndex);
        //Debug.Log(fileName);
        FileDataHandler fileDataHandler = new FileDataHandler(
            Application.persistentDataPath,
            fileName,
            encryptData
        );

        data = new GameData(); // ���������ݻ�����������
        foreach (ISaveManager manager in saveManagers)
        {
            manager.SaveData(ref data);
        }
        fileDataHandler.SaveData(data);
    }
    public void SaveGame()
    {
        int selectedSlo = PlayerPrefs.GetInt("SelectedSaveSlot", -1);
       // Debug.Log("selectedSlotIndex" + selectedSlo);
        if (selectedSlo != -1)
        {
            SaveGame(selectedSlo);
        }
        else
        {
            SaveGame(1);
        }
    }
    public void LoadGame(int slotIndex)
    {
        string fileName = string.Format(saveFileNameTemplate, slotIndex);
        FileDataHandler fileDataHandler = new FileDataHandler(
            Application.persistentDataPath,
            fileName,
            encryptData
        );

        saveManagers = FindAllSaveManagers();
        data = fileDataHandler.LoadData();
       // Debug.Log(data);
        if (data == null)
        {
            NewGame();
        }
        foreach (ISaveManager manager in saveManagers)
        {
            manager.LoadData(data);
        }
    }
    public void LoadGame()
    {
        int selectedSlo = PlayerPrefs.GetInt("SelectedSaveSlot", -1);
     //  Debug.Log("selectedSlotIndex" + selectedSlo);
        if (selectedSlo != -1)
        {
            LoadGame(selectedSlo);
        }
        else
        {
            LoadGame(1);
        }
    }

    private List<ISaveManager> FindAllSaveManagers()
    {

        IEnumerable<ISaveManager> managers = FindObjectsOfType<MonoBehaviour>().OfType<ISaveManager>();
        

        return new List<ISaveManager>(managers);
    }

    // ���ĳ����λ�Ƿ���ڴ浵
    public bool HasSaveData(int slotIndex)
    {
        string fileName = string.Format(saveFileNameTemplate, slotIndex);
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        return File.Exists(filePath);
    }
    public GameData GetSaveMetadata(int slotIndex)
    {
        string fileName = string.Format(saveFileNameTemplate, slotIndex);
        FileDataHandler fileDataHandler = new FileDataHandler(
            Application.persistentDataPath,
            fileName,
            encryptData
        );
        return fileDataHandler.LoadData();
        // ������Ԫ�����ֶΣ��Ż����ܣ�
        //string json = fileDataHandler.LoadRawJson();
        //if (!string.IsNullOrEmpty(json))
        //{
        //    return JsonUtility.FromJson<GameData>(json);
        //}
       // return null;
    }

}
