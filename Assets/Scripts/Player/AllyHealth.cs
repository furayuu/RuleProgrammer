using UnityEngine;

public class AllyHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHP = 5;

    private int currentHP;
    private bool isDead = false;

    public int CurrentHP => currentHP;

    public float HealthPercent => (float)currentHP / maxHP;

    void Start()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
            return;

        currentHP -= damage;

        currentHP = Mathf.Max(currentHP, 0);

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
        isDead = true;

        Destroy(gameObject);
    }
}