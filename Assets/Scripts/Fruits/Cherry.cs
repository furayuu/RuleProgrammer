
using UnityEngine;

public class Cherry : MonoBehaviour
{
    [Header("Projectile")]
    public FruitType fruitType;
    public int damage = 1;
    public float lifeTime = 3f;

    [Header("Explosion")]
    public float explosionRadius = 3f;
    public int explosionDamage = 3;
    public GameObject explosionEffect;

    private bool hasExploded = false;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 敵に命中
        if (other.CompareTag("Enemy"))
        {
            if (!hasExploded)
            {
                Explode();
            }
        }

        // 地面や壁に衝突
        if (other.CompareTag("Ground"))
        {
            if (!hasExploded)
            {
                Explode();
            }
        }
    }

    // 爆発処理
    void Explode()
    {
        hasExploded = true;

        // 爆発エフェクトを生成
        if (explosionEffect != null)
        {
            Instantiate(
                explosionEffect,
                transform.position,
                Quaternion.identity
            );
        }

        // 爆発範囲内の敵を取得
        Collider2D[] enemies = Physics2D.OverlapCircleAll(
            transform.position,
            explosionRadius
        );

        // 範囲内の敵全員にダメージ
        foreach (Collider2D enemyCollider in enemies)
        {
            if (enemyCollider.CompareTag("Enemy"))
            {
                EnemyHealth enemy =
                    enemyCollider.GetComponent<EnemyHealth>();

                if (enemy != null)
                {
                    enemy.TakeDamage(explosionDamage);
                }
            }
        }

        Debug.Log("Cherry Explosion!");

        Destroy(gameObject);
    }

    // 爆発範囲をSceneビューに表示
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

    // 爆発範囲の強化
    public void UpgradeExplosionRadius(float amount)
    {
        explosionRadius += amount;
    }
}
