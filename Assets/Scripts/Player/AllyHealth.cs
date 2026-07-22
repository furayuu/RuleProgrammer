using UnityEngine;

public class AllyHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHP = 5;

    private int currentHP;
    private bool isDead = false;

    private Animator animator;

    // 新增
    [HideInInspector]
    public BuildPoint ownerBuildPoint;

    public int CurrentHP => currentHP;

    public float HealthPercent => (float)currentHP / maxHP;

    void Start()
    {
        currentHP = maxHP;
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(int damage)
    {
        StopAllCoroutines();

        if (isDead)
            return;

        currentHP -= damage;
        currentHP = Mathf.Max(currentHP, 0);

        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }

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

        if (ownerBuildPoint != null)
        {
            ownerBuildPoint.hasAlly = false;
            ownerBuildPoint.currentAlly = null;
        }

        Destroy(gameObject);
    }
}