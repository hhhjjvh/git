using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;


public class GameManager : MonoBehaviour, ISaveManager
{
    public static GameManager instance;
    public AssetReference currentScenceSo;
    public AssetReference firstScenceSo;
    [SerializeField] private CheckPoint[] checkPoints;
    private CheckPoint[] LoadcheckPoints;
    [SerializeField] private string closestCheckPointLoad;
    GameData data;
    private void Awake()
    {

        if (instance != null)
        {
            Destroy(instance.gameObject);
        }
        instance = this;
        checkPoints = FindObjectsByType<CheckPoint>(FindObjectsSortMode.None);
        //Debug.Log(checkPoints.Length);

        //checkPoints = FindObjectsOfType<CheckPoint>();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //checkPoints = FindObjectsByType<CheckPoint>(FindObjectsSortMode.None);
        //Debug.Log(checkPoints.Length);
    }
    public void RestartScence()
    {
        //SaveManager.instance.SaveGame();

        Addressables.LoadSceneAsync(currentScenceSo);
    }

    public void ExitGame()
    {
        Addressables.LoadSceneAsync(firstScenceSo);
    }
    public void LoadData(GameData data)
    {
        this.data = data;
        //Debug.Log("Load Data");

    }
    public void SetPlayerPosition()
    {

        //Debug.Log("Set Player Position");
        StartCoroutine(CheckPointsSet(data, 0.1f));


    }

    private IEnumerator CheckPointsSet(GameData data, float time)
    {

        PlayerManager.instance.player.transform.position = data.position;


        yield return new WaitForSeconds(time);
        checkPoints = FindObjectsByType<CheckPoint>(FindObjectsSortMode.None);
        bool check = false;
        //Debug.Log(checkPoints.Length);
        if (checkPoints.Length != 0)
        {// Debug.Log("Load Data");
            foreach (KeyValuePair<string, bool> checkPoint in data.checkPoints)
            {
                // Debug.Log(checkPoint.Key + " " + checkPoint.Value);
                //Debug.Log(checkPoints.Length);
                foreach (CheckPoint point in checkPoints)
                {
                    if (point.checkPointID == checkPoint.Key)
                    {
                        if (checkPoint.Value)
                        {
                            check = true;
                            // Debug.Log("Activate");
                            point.ActivateCheckPoint();
                        }
                    }
                }
            }
            closestCheckPointLoad = data.closestCheckPoint;
            Invoke("PlacePlayerAtClosestCheckpoint", 0.1f);
        }
       if(!check)
        {
            var points = GameObject.FindGameObjectWithTag("Point");
            // Debug.Log(points.transform.position);
            if (points != null)
            {
                PlayerManager.instance.player.transform.position = points.transform.position;
            }
        }




       
    }

    private void PlacePlayerAtClosestCheckpoint()
    {
        foreach (CheckPoint point in checkPoints)
        {
            if (point.checkPointID == closestCheckPointLoad)
            {
                PlayerManager.instance.player.transform.position = point.transform.position;
            }
        }
    }

    public void SaveData(ref GameData data)
    {
        checkPoints = FindObjectsByType<CheckPoint>(FindObjectsSortMode.None);
        //Debug.Log(checkPoints.Length);
        if (checkPoints.Length == 0 || FindCloseCheckPoint() == null) { return; }
        data.closestCheckPoint = FindCloseCheckPoint().checkPointID;
        data.checkPoints.Clear();
        foreach (var checkPoint in checkPoints)
        {
            data.checkPoints.Add(checkPoint.checkPointID, checkPoint.actived);
        }
    }

    public CheckPoint FindCloseCheckPoint()
    {
        float minDistance = Mathf.Infinity;
        CheckPoint closestCheckPoint = null;
        foreach (CheckPoint checkPoint in checkPoints)
        {
            float distance = Vector3.Distance(checkPoint.transform.position, PlayerManager.instance.player.transform.position);
            if (distance < minDistance && checkPoint.actived)
            {
                minDistance = distance;
                closestCheckPoint = checkPoint;
            }
        }
        // Debug.Log("Closest CheckPoint: " + closestCheckPoint);
        return closestCheckPoint;
    }

    public void PauseGame(bool pause)
    {
       // Time.timeScale = pause ? 0 : 1;
       if (pause)
        {
            TimeManager.Instance.SetGlobalTimeScale(0, true);
        }
        else
        {
            TimeManager.Instance.SetGlobalTimeScale(1, true);
        }
    }
}
