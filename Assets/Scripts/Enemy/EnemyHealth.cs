using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHP = 3;
    [Header("Reward")]
    public int moneyReward = 5;

    private int currentHP;
    private bool isDead = false;

    private Animator animator;

    public int CurrentHP => currentHP;

    public float HealthPercent => (float)currentHP / maxHP;

    GameManager gameManager;

    void Start()
    {
        currentHP = maxHP;
        animator = GetComponent<Animator>();

        gameManager = FindFirstObjectByType<GameManager>();
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
            return;

        currentHP -= damage;

        currentHP = Mathf.Max(currentHP, 0);

        animator.SetTrigger("Hit");

        if (currentHP <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        if (isDead)
            return;

        currentHP += amount;

        if (currentHP > maxHP)
            currentHP = maxHP;
    }

    void Die()
    {
        GameManager.Instance.AddMoney(moneyReward);

        isDead = true;

        animator.SetTrigger("Die");

        Destroy(gameObject, 0.5f);
    }
}