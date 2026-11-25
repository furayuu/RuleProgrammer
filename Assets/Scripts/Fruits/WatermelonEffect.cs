
using UnityEngine;

public class WatermelonEffect : FruitEffectBase
{
    [Header("Fragment Settings")]
    public GameObject fragmentPrefab;
    public int fragmentCount = 6;
    public int fragmentDamage = 3;
    public float fragmentSpeed = 5f;

    private bool hasShattered = false;
    private Projectile projectile;

    void Awake()
    {
        // Projectileを取得する
        projectile = GetComponent<Projectile>();
    }

    // 敵に命中したときにスイカを破裂させる
    public override void Apply(Transform target)
    {
        Shatter();
    }

    // 地面に衝突したときにスイカを破裂させる
    public override void OnGroundHit()
    {
        Shatter();
    }

    // スイカを破裂させて小さなスイカを生成する
    private void Shatter()
    {
        if (hasShattered)
            return;

        hasShattered = true;

        if (fragmentPrefab != null)
        {
            for (int i = 0; i < fragmentCount; i++)
            {
                // 360度均等に散らす方向を計算する
                float angle =
                    i * (360f / fragmentCount);

                Vector2 direction = new Vector2(
                    Mathf.Cos(angle * Mathf.Deg2Rad),
                    Mathf.Sin(angle * Mathf.Deg2Rad)
                );

                // 小さなスイカを生成する
                GameObject fragment = Instantiate(
                    fragmentPrefab,
                    transform.position,
                    Quaternion.identity
                );

                // 小さなスイカのダメージを設定する
                Projectile fragmentProjectile =
                    fragment.GetComponent<Projectile>();

                if (fragmentProjectile != null)
                {
                    fragmentProjectile.damage =
                        fragmentDamage;
                }

                // 小さなスイカの移動速度を設定する
                Rigidbody2D fragmentRb =
                    fragment.GetComponent<Rigidbody2D>();

                if (fragmentRb != null)
                {
                    fragmentRb.velocity =
                        direction.normalized * fragmentSpeed;
                }
            }
        }
        else
        {
            Debug.LogWarning(
                "Watermelon Fragment Prefabが設定されていません。"
            );
        }

        Debug.Log("Watermelon Explosion!");

        Destroy(gameObject);
    }

    // ダメージを強化する
    public void UpgradeDamage(int amount)
    {
        if (projectile != null)
        {
            projectile.damage += amount;
        }

        fragmentDamage += amount;
    }

    // 小さなスイカの数を強化する
    public void UpgradeFragmentCount(int amount)
    {
        fragmentCount += amount;
    }
}
