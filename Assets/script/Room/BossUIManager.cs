using UnityEngine;
using TMPro; // ���� TextMesh Pro �����ռ�
using UnityEngine.UI; // ���� UI �����ռ�

public class BossUIManager : MonoBehaviour
{
    public Slider healthSlider;          // Ѫ��
    public TextMeshProUGUI bossNameText; // Boss ���֣�ʹ�� TextMeshPro��
    public Image bossImage;              // Boss ͼƬ
    public TextMeshProUGUI bossHealthText; // Boss Ѫ����ʹ�� TextMeshPro��
    public TextMeshProUGUI bossLevelText; // Boss �ȼ���ʹ�� TextMeshPro��
    public GameObject bossUI;            // �������� Boss UI Ԫ�ص� GameObject
    float health = 0;
    public GameObject clearText; // ��ȫ���������ʾ
    private EnemyStats enemyStats;
    /// </summary>
    /// <param name="bossName"></param>
    /// <param name="bossSprite"></param>
    /// <param name="maxHealth"></param>
    /// <param name="bossLevel"></param>
    /// <param name="enemyStats"></param>

    // ���� Boss UI�����֡�ͼƬ��Ѫ����
    public void SetBossUI(string bossName, Sprite bossSprite, float maxHealth, int bossLevel, EnemyStats enemyStats)
    {
        // ���� Boss ����
        bossNameText.text = bossName;

        // ���� Boss ͼƬ
        if (bossSprite != null)
        {
            bossImage.sprite = bossSprite;
        }

        // ����Ѫ�������ֵ
        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;
        health = maxHealth;
        this.enemyStats = enemyStats;

       
        bossLevelText.text = "Boss Level: " + bossLevel.ToString();

        // ��ʾ Boss UI
        bossUI.SetActive(true);
    }

    // ����Ѫ��
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

    // ���� Boss UI
    public void HideBossUI()
    {
       
        clearText.SetActive(true);
        Invoke(nameof(HideClearText), 2f); // 2���������ʾ
    }
   
    // ���ء�ȫ���������ʾ
    private void HideClearText()
    {
        clearText.SetActive(false);
        bossUI.SetActive(false);
    }
}
