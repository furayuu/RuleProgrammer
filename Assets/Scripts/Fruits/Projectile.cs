
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("弾の設定")]
    public int damage = 1;
    public float lifeTime = 3f;

    private FruitEffectBase fruitEffect;
    private bool hasHit = false;

    void Awake()
    {
        // このオブジェクトに設定された特殊効果を取得する
        fruitEffect = GetComponent<FruitEffectBase>();
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // すでに処理済みの弾は無視する
        if (hasHit)
            return;

        // 敵に命中した場合
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth enemy =
                other.GetComponentInParent<EnemyHealth>();

            if (enemy == null)
                return;

            // 同じ敵への重複攻撃を防ぐ
            if (
                fruitEffect != null &&
                !fruitEffect.CanHitTarget(enemy.transform)
            )
                return;

            // 基本ダメージを与える
            enemy.TakeDamage(damage);

            // フルーツ固有の特殊効果を実行する
            if (fruitEffect != null)
            {
                fruitEffect.Apply(enemy.transform);
            }

            // 特殊効果の設定に応じて弾を破棄する
            if (
                fruitEffect == null ||
                fruitEffect.ShouldDestroyProjectile
            )
            {
                hasHit = true;
                Destroy(gameObject);
            }
        }

        // 地面に衝突した場合
        else if (other.CompareTag("Ground"))
        {
            hasHit = true;

            // 地面に衝突したときの特殊効果を実行する
            if (fruitEffect != null)
            {
                fruitEffect.OnGroundHit();
            }

            Destroy(gameObject);
        }
    }
}
