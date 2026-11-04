
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("Attack")]
    public int damage = 1;

    [Tooltip("2回の攻撃の間隔")]
    public float attackCooldown = 1f;

    private float attackTimer = 0f;

    private EnemyHealth enemyHealth;

    void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();
    }

    void Update()
    {
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }

        // 死亡した敵は攻撃できない
        if (enemyHealth != null && enemyHealth.IsDead)
            enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 死亡した敵は攻撃できない
        if (enemyHealth != null && enemyHealth.IsDead)
            return;

        if (attackTimer > 0)
            return;

        // プレイヤーへの攻撃
        PlayerHealth player = other.GetComponent<PlayerHealth>();

        if (player != null)
        {
            player.TakeDamage(damage);
            attackTimer = attackCooldown;
            return;
        }

        // Allyへの攻撃
        AllyHealth ally = other.GetComponent<AllyHealth>();

        if (ally != null)
        {
            ally.TakeDamage(damage);
            attackTimer = attackCooldown;
            return;
        }

        // 基地への攻撃
        BaseHealth baseHealth = other.GetComponent<BaseHealth>();

        if (baseHealth != null)
        {
            // 基地のHPを減らす
            baseHealth.TakeDamage(damage);

            // 敵を死亡させる（報酬なし）
            if (enemyHealth != null)
            {
                enemyHealth.Die(false);
            }
            else
            {
                Destroy(gameObject);
            }

            return;
        }
    }
}
