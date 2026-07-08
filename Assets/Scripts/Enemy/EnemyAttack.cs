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

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (attackTimer > 0)
            return;

        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth player =
                collision.gameObject.GetComponent<PlayerHealth>();

            if (player != null)
            {
                player.TakeDamage(damage);
                attackTimer = attackCooldown;
            }
        }
        else if (collision.gameObject.CompareTag("Ally"))
        {
            AllyHealth ally =
                collision.gameObject.GetComponent<AllyHealth>();

            if (ally != null)
            {
                ally.TakeDamage(damage);
                attackTimer = attackCooldown;
            }
        }
    }
}