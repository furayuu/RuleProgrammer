
using UnityEngine;

public class StrawberryProjectile : MonoBehaviour
{
    [Header("Projectile")]
    public FruitType fruitType;
    public float lifeTime = 3f;
    public float speed = 8f;

    [Header("Healing")]
    public int healAmount = 10;

    private Rigidbody2D rb;
    private bool hasHit = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    // ’e‚ÌˆÚ“®•ûŒü‚ğİ’è
    public void Initialize(
        Vector2 direction,
        float projectileSpeed,
        int healing)
    {
        speed = projectileSpeed;
        healAmount = healing;

        if (rb != null)
        {
            rb.velocity = direction.normalized * speed;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;

        // –¡•û‚É–½’†
        if (other.CompareTag("Ally"))
        {
            AllyHealth ally =
                other.GetComponent<AllyHealth>();

            if (ally == null)
            {
                ally = other.GetComponentInParent<AllyHealth>();
            }

            if (ally != null)
            {
                hasHit = true;

                // HP‚ğ‰ñ•œ
                ally.Heal(healAmount);

                Debug.Log("Strawberry Heal!");

                Destroy(gameObject);
            }
        }

        // ’n–Ê‚â•Ç‚ÉÕ“Ë
        if (other.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }

    // ‰ñ•œ—Ê‚Ì‹­‰»
    public void UpgradeHealAmount(int amount)
    {
        healAmount += amount;
    }
}
