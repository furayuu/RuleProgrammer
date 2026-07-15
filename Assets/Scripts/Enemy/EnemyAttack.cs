using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("Attack")]
    public int damage = 1;

    [Tooltip("两次伤害之间的间隔")]
    public float attackCooldown = 1f;

    private float attackTimer = 0f;

    void Update()
    {
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (attackTimer > 0)
            return;

        PlayerHealth player = other.GetComponent<PlayerHealth>();

        if (player != null)
        {
            player.TakeDamage(damage);
            attackTimer = attackCooldown;
            return;
        }

        AllyHealth ally = other.GetComponent<AllyHealth>();

        if (ally != null)
        {
            ally.TakeDamage(damage);
            attackTimer = attackCooldown;
        }
    }
}