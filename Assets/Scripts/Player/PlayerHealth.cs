using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHP = 10;

    [Header("HP UI")]
    public TMP_Text healthText;

    private int currentHP;
    private bool isDead = false;
    private Animator animator;

    public int CurrentHP { get { return currentHP; } }

    public float HealthPercent
    {
        get { return maxHP > 0 ? (float)currentHP / maxHP : 0f; }
    }

    void Start()
    {
        maxHP = Mathf.Max(1, maxHP);
        currentHP = maxHP;
        animator = GetComponent<Animator>();

        UpdateHealthUI();
    }

    public void TakeDamage(int damage)
    {
        if (isDead || damage <= 0) return;

        currentHP -= damage;
        currentHP = Mathf.Max(currentHP, 0);

        Debug.Log("Player HP : " + currentHP);

        if (animator != null)
            animator.SetTrigger("Hit");

        UpdateHealthUI();

        if (currentHP <= 0)
            Die();
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
            healthText.text = "HP:" + currentHP + "/" + maxHP;
    }

    void Die()
    {
        isDead = true;
        Debug.Log("Game Over");
        gameObject.SetActive(false);
    }
}