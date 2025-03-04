using System.Collections;
using UnityEngine;
using TMPro;

public class EnemyUIManager : MonoBehaviour
{
    public static EnemyUIManager Instance;
    [Header("UI Elements")]
    public TextMeshProUGUI waveInfoText; // 显示波数信息的文本
    public TextMeshProUGUI enemyCountText; // 显示敌人数信息的文本
    public TextMeshProUGUI difficultyText; // 显示难度等级的文本
    public GameObject clearText; // “全部清除”提示

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
    // 更新波数信息
    public void UpdateWaveInfo(int currentWave, int totalWaves)
    {
        waveInfoText.text = $"波次: {currentWave+1}/{totalWaves}";
    }

    // 更新敌人数信息
    public void UpdateEnemyCount(int remainingEnemies, int totalEnemies)
    {
        enemyCountText.text = $"敌人: {remainingEnemies}/{totalEnemies}";
    }

    // 更新难度等级
    public void UpdateDifficulty(int difficulty)
    {
        difficultyText.text = $"难度: {difficulty}";
    }

    // 显示“全部清除”提示
    public void ShowClearText()
    {
        clearText.SetActive(true);
        Invoke(nameof(HideClearText), 2f); // 2秒后隐藏提示
    }

    // 隐藏“全部清除”提示
    private void HideClearText()
    {
        clearText.SetActive(false);
        gameObject.SetActive(false);
    }
}

