using UnityEngine;
using TMPro; // 引入 TextMesh Pro 命名空间
using UnityEngine.UI; // 引入 UI 命名空间

public class BossUIManager : MonoBehaviour
{
    public Slider healthSlider;          // 血条
    public TextMeshProUGUI bossNameText; // Boss 名字（使用 TextMeshPro）
    public Image bossImage;              // Boss 图片
    public TextMeshProUGUI bossHealthText; // Boss 血量（使用 TextMeshPro）
    public TextMeshProUGUI bossLevelText; // Boss 等级（使用 TextMeshPro）
    public GameObject bossUI;            // 包含所有 Boss UI 元素的 GameObject
    float health = 0;
    public GameObject clearText; // “全部清除”提示
    private EnemyStats enemyStats;
    /// </summary>
    /// <param name="bossName"></param>
    /// <param name="bossSprite"></param>
    /// <param name="maxHealth"></param>
    /// <param name="bossLevel"></param>
    /// <param name="enemyStats"></param>

    // 设置 Boss UI（名字、图片、血条）
    public void SetBossUI(string bossName, Sprite bossSprite, float maxHealth, int bossLevel, EnemyStats enemyStats)
    {
        // 设置 Boss 名字
        bossNameText.text = bossName;

        // 设置 Boss 图片
        if (bossSprite != null)
        {
            bossImage.sprite = bossSprite;
        }

        // 设置血条的最大值
        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;
        health = maxHealth;
        this.enemyStats = enemyStats;

       
        bossLevelText.text = "Boss Level: " + bossLevel.ToString();

        // 显示 Boss UI
        bossUI.SetActive(true);
    }

    // 更新血条
    public void UpdateHealth(float currentHealth)
    {
        healthSlider.maxValue = enemyStats.GetMaxHealth();
        healthSlider.value = currentHealth;
        bossHealthText.text = currentHealth.ToString() + "/" + enemyStats.GetMaxHealth().ToString();
        if (currentHealth <= 0)
        {
            HideBossUI();
        }
    }

    // 隐藏 Boss UI
    public void HideBossUI()
    {
       
        clearText.SetActive(true);
        Invoke(nameof(HideClearText), 2f); // 2秒后隐藏提示
    }
   
    // 隐藏“全部清除”提示
    private void HideClearText()
    {
        clearText.SetActive(false);
        bossUI.SetActive(false);
    }
}
