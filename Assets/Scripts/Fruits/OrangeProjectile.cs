
using UnityEngine;
using System.Collections.Generic;

public class OrangeProjectile : MonoBehaviour
{
    [Header("Projectile")]
    public FruitType fruitType;
    public int damage = 3;
    public float lifeTime = 5f;
    public float speed = 8f;

    [Header("Bounce Settings")]
    public int maxBounceCount = 3;
    public float bounceSearchRadius = 6f;

    private Rigidbody2D rb;
    private int currentBounceCount = 0;

    private HashSet<EnemyHealth> hitEnemies =
        new HashSet<EnemyHealth>();

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    // ’e‚ÌˆÚ“®•ûŒü‚ğİ’è
    public void SetDirection(Vector2 direction)
    {
        if (rb != null)
        {
            rb.velocity = direction.normalized * speed;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // “G‚É–½’†
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth enemy =
                other.GetComponent<EnemyHealth>();

            if (enemy == null)
            {
                enemy = other.GetComponentInParent<EnemyHealth>();
            }

            if (enemy == null || hitEnemies.Contains(enemy))
            {
                return;
            }

            // ƒ_ƒ[ƒW‚ğ—^‚¦‚é
            enemy.TakeDamage(damage);

            // –½’†‚µ‚½“G‚ğ‹L˜^
            hitEnemies.Add(enemy);

            Debug.Log("Orange Bounce!");

            // ’µ’e‰ñ”‚ğŠm”F
            if (currentBounceCount < maxBounceCount)
            {
                // Ÿ‚Ì“G‚ğ’T‚·
                FindNextEnemy();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // ’n–Ê‚â•Ç‚ÉÕ“Ë
        if (other.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }

    // Ÿ‚Ì“G‚ğ’T‚µ‚Ä’µ’e‚·‚é
    void FindNextEnemy()
    {
        Collider2D[] enemies =
            Physics2D.OverlapCircleAll(
                transform.position,
                bounceSearchRadius
            );

        EnemyHealth nearestEnemy = null;
        float nearestDistance = float.MaxValue;

        foreach (Collider2D enemyCollider in enemies)
        {
            if (!enemyCollider.CompareTag("Enemy"))
            {
                continue;
            }

            EnemyHealth enemy =
                enemyCollider.GetComponent<EnemyHealth>();

            if (enemy == null)
            {
                enemy = enemyCollider.GetComponentInParent<EnemyHealth>();
            }

            if (enemy == null || hitEnemies.Contains(enemy))
            {
                continue;
            }

            float distance = Vector2.Distance(
                transform.position,
                enemy.transform.position
            );

            // ˆê”Ô‹ß‚¢“G‚ğ‘I‘ğ
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestEnemy = enemy;
            }
        }

        // Ÿ‚Ì“G‚ªŒ©‚Â‚©‚Á‚½ê‡
        if (nearestEnemy != null)
        {
            currentBounceCount++;

            Vector2 direction =
                (nearestEnemy.transform.position -
                transform.position).normalized;

            // ’e‚ÌŒü‚«‚ğ•ÏX
            if (rb != null)
            {
                rb.velocity = direction * speed;
            }
        }
        else
        {
            // Ÿ‚Ì“G‚ª‚¢‚È‚¢ê‡‚ÍÁ–Å
            Destroy(gameObject);
        }
    }

    // ’µ’e‰ñ”‚Ì‹­‰»
    public void UpgradeBounceCount(int amount)
    {
        maxBounceCount += amount;
    }

    // UŒ‚—Í‚Ì‹­‰»
    public void UpgradeDamage(int amount)
    {
        damage += amount;
    }

    // ’µ’e”ÍˆÍ‚ğSceneƒrƒ…[‚É•\¦
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            transform.position,
            bounceSearchRadius
        );
    }
}
