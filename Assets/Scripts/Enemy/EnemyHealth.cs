using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHP = 3;

    private int currentHP;
    private Animator animator;

    private bool isDead = false;

    void Start()
    {
        currentHP = maxHP;
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHP -= damage;

        animator.SetTrigger("Hit");

        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        animator.SetTrigger("Die");

        Destroy(gameObject, 0.5f);
    }
}