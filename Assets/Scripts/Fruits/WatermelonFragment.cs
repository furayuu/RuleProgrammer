
using UnityEngine;

public class WatermelonFragment : MonoBehaviour
{
    [Header("Fragment")]
    public float lifeTime = 2f;

    private Rigidbody2D rb;
    private int damage;
    private bool hasHit = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    // ”j•Ğ‚ÌˆÚ“®•ûŒü‚ÆUŒ‚—Í‚ğİ’è
    public void Initialize(
        Vector2 direction,
        float speed,
        int fragmentDamage)
    {
        damage = fragmentDamage;

        if (rb != null)
        {
            rb.velocity = direction.normalized * speed;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;

        // “G‚É–½’†
        EnemyHealth enemy =
            other.GetComponent<EnemyHealth>();

        if (enemy == null)
        {
            enemy = other.GetComponentInParent<EnemyHealth>();
        }

        if (enemy != null)
        {
            hasHit = true;

            enemy.TakeDamage(damage);

            Debug.Log("Watermelon Fragment Hit!");

            Destroy(gameObject);
        }

        // ’n–Ê‚â•Ç‚ÉÕ“Ë
        else if (other.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
