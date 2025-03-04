using System.Collections;
using UnityEngine;
using TMPro;

public class EnemyUIManager : MonoBehaviour
{
    public static EnemyUIManager Instance;
    [Header("UI Elements")]
    public TextMeshProUGUI waveInfoText; // ��ʾ������Ϣ���ı�
    public TextMeshProUGUI enemyCountText; // ��ʾ��������Ϣ���ı�
    public TextMeshProUGUI difficultyText; // ��ʾ�Ѷȵȼ����ı�
    public GameObject clearText; // ��ȫ���������ʾ

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        gameObject.SetActive(false);
    }
    void Start()
    {
        
    }
    // ���²�����Ϣ
    public void UpdateWaveInfo(int currentWave, int totalWaves)
    {
        waveInfoText.text = $"����: {currentWave+1}/{totalWaves}";
    }

    // ���µ�������Ϣ
    public void UpdateEnemyCount(int remainingEnemies, int totalEnemies)
    {
        enemyCountText.text = $"����: {remainingEnemies}/{totalEnemies}";
    }

    // �����Ѷȵȼ�
    public void UpdateDifficulty(int difficulty)
    {
        difficultyText.text = $"�Ѷ�: {difficulty}";
    }

    // ��ʾ��ȫ���������ʾ
    public void ShowClearText()
    {
        clearText.SetActive(true);
        Invoke(nameof(HideClearText), 2f); // 2���������ʾ
    }

    // ���ء�ȫ���������ʾ
    private void HideClearText()
    {
        clearText.SetActive(false);
        gameObject.SetActive(false);
    }
}

