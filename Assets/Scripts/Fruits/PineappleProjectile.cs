
using UnityEngine;

public class PineappleProjectile : MonoBehaviour
{
    [Header("Projectile")]
    public FruitType fruitType;
    public float lifeTime = 5f;

    [Header("Trap Settings")]
    public GameObject trapPrefab;
    public float fuseTime = 3f;
    public float explosionRadius = 3f;
    public int explosionDamage = 10;

    private Rigidbody2D rb;
    private bool hasPlaced = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    // 投げる方向を設定
    public void SetDirection(Vector2 direction, float speed)
    {
        if (rb != null)
        {
            rb.velocity = direction.normalized * speed;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 地面に当たったらトラップを設置
        if (other.CompareTag("Ground"))
        {
            if (hasPlaced) return;

            hasPlaced = true;

            if (trapPrefab != null)
            {
                GameObject trap = Instantiate(
                    trapPrefab,
                    transform.position,
                    Quaternion.identity
                );

                PineappleTrap trapScript =
                    trap.GetComponent<PineappleTrap>();

                if (trapScript != null)
                {
                    trapScript.Initialize(
                        fuseTime,
                        explosionRadius,
                        explosionDamage
                    );
                }
            }

            Destroy(gameObject);
        }
    }
}
