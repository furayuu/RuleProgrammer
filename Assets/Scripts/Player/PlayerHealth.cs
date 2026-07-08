using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHP = 10;

    private int currentHP;
    private bool isDead = false;

    private Animator animator;

    // 当前血量
    public int CurrentHP
    {
        get { return currentHP; }
    }

    // 当前血量百分比（0~1）
    public float HealthPercent
    {
        get { return (float)currentHP / maxHP; }
    }

    void Start()
    {
        currentHP = maxHP;
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
            return;

        currentHP -= damage;

        // 防止血量小于0
        currentHP = Mathf.Max(currentHP, 0);

        Debug.Log("Player HP : " + currentHP);

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

        Debug.Log("Player HP : " + currentHP);
    }

    void Die()
    {
        isDead = true;

        Debug.Log("Game Over");

        // 如果以后有死亡动画，可以改成播放动画
        // animator.SetTrigger("Die");

        // Prototype阶段先禁用玩家
        gameObject.SetActive(false);

        // 以后这里可以通知GameManager
        // GameManager.Instance.GameOver();
    }
}