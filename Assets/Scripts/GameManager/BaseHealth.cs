using UnityEngine;
using TMPro;

public class BaseHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHP = 10;

    [Header("HP UI")]
    public TMP_Text healthText;

    private int currentHP;
    private bool isDead = false;

    public int CurrentHP { get { return currentHP; } }

    public float HealthPercent
    {
        get { return maxHP > 0 ? (float)currentHP / maxHP : 0f; }
    }

    void Start()
    {
        maxHP = Mathf.Max(1, maxHP);
        currentHP = maxHP;

        UpdateHealthUI();
    }

    public void TakeDamage(int damage)
    {
        if (isDead || damage <= 0) return;

        currentHP -= damage;
        currentHP = Mathf.Max(currentHP, 0);

        Debug.Log("Base HP : " + currentHP);

        UpdateHealthUI();

        if (currentHP <= 0)
            GameOver();
    }

    public void Heal(int amount)
    {
        if (isDead || amount <= 0) return;

        currentHP += amount;
        currentHP = Mathf.Min(currentHP, maxHP);

        UpdateHealthUI();
    }

    void UpdateHealthUI()
    {
        if (healthText != null)
            healthText.text = "Base:" + currentHP + "/" + maxHP;
    }

    void GameOver()
    {
        isDead = true;
        Debug.Log("GameOver");

    }
}