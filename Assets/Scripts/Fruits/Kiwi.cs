
using UnityEngine;
using System.Collections.Generic;

public class Kiwi : MonoBehaviour
{
    [Header("Projectile")]
    public FruitType fruitType;
    public int damage = 1;
    public float lifeTime = 3f;
    public float speed = 10f;

    [Header("Pierce Settings")]
    public int maxPierceCount = 3;

    private int currentPierceCount = 0;
    private HashSet<EnemyHealth> hitEnemies =
        new HashSet<EnemyHealth>();

    private Rigidbody2D rb;

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

            if (enemy != null && !hitEnemies.Contains(enemy))
            {
                // ƒ_ƒ[ƒW‚ğ—^‚¦‚é
                enemy.TakeDamage(damage);

                // –½’†‚µ‚½“G‚ğ‹L˜^
                hitEnemies.Add(enemy);

                currentPierceCount++;

                Debug.Log("Kiwi Pierce!");

                // ŠÑ’Ê‰ñ”‚ÌãŒÀ‚É’B‚µ‚½‚çÁ–Å
                if (currentPierceCount >= maxPierceCount)
                {
                    Destroy(gameObject);
                }
            }
        }

        // ’n–Ê‚â•Ç‚ÉÕ“Ë
        if (other.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }

    // ŠÑ’Ê”‚Ì‹­‰»
    public void UpgradePierceCount(int amount)
    {
        maxPierceCount += amount;
    }

    // UŒ‚—Í‚Ì‹­‰»
    public void UpgradeDamage(int amount)
    {
        damage += amount;
    }
}
