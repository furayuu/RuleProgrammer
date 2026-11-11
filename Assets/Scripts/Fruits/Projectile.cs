
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
        // すでに命中している場合は処理しない
        if (hasHit)
            return;

        // 敵に命中した場合
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth enemy =
                other.GetComponentInParent<EnemyHealth>();

            if (enemy != null)
            {
                hasHit = true;

                // 基本ダメージを与える
                enemy.TakeDamage(damage);

                // フルーツ固有の特殊効果を実行する
                if (fruitEffect != null)
                {
                    fruitEffect.Apply(enemy.transform);
                }

                Destroy(gameObject);
            }
        }

        // 地面や壁に衝突した場合
        else if (other.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
