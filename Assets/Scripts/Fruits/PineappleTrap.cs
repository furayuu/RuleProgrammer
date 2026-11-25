
using UnityEngine;

public class PineappleTrap : MonoBehaviour
{
    [Header("Trap Settings")]
    public float fuseTime = 3f;
    public float explosionRadius = 3f;
    public int explosionDamage = 10;

    [Header("Explosion Effect")]
    public GameObject explosionEffect;

    private bool hasExploded = false;

    // トラップの設定
    public void Initialize(
        float time,
        float radius,
        int damage)
    {
        fuseTime = time;
        explosionRadius = radius;
        explosionDamage = damage;

        CancelInvoke(nameof(Explode));
        Invoke(nameof(Explode), fuseTime);
    }

    void Start()
    {
        // Initializeが呼ばれなかった場合の設定
        if (!IsInvoking(nameof(Explode)))
        {
            Invoke(nameof(Explode), fuseTime);
        }
    }

    // 爆発処理
    void Explode()
    {
        if (hasExploded) return;

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
        Collider2D[] enemies =
            Physics2D.OverlapCircleAll(
                transform.position,
                explosionRadius
            );

        // 範囲内の敵にダメージ
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
                enemy =
                    enemyCollider.GetComponentInParent<EnemyHealth>();
            }

            if (enemy != null)
            {
                enemy.TakeDamage(explosionDamage);
            }
        }

        Debug.Log("Pineapple Trap Explosion!");

        Destroy(gameObject);
    }

    // 起爆時間の強化
    public void UpgradeFuseTime(float amount)
    {
        fuseTime = Mathf.Max(0.1f, fuseTime - amount);
    }

    // 爆発範囲の強化
    public void UpgradeExplosionRadius(float amount)
    {
        explosionRadius += amount;
    }

    // Sceneビューに爆発範囲を表示
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(
            transform.position,
            explosionRadius
        );
    }
}
