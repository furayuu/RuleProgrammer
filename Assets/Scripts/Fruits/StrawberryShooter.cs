
using UnityEngine;

public class StrawberryShooter : MonoBehaviour
{
    [Header("Shooting")]
    public GameObject strawberryPrefab;
    public Transform firePoint;

    public float fireRate = 1f;
    public float projectileSpeed = 8f;
    public float searchRadius = 10f;

    [Header("Healing")]
    public int healAmount = 10;

    private float nextFireTime = 0f;

    void Update()
    {
        // 発射時間を確認
        if (Time.time >= nextFireTime)
        {
            Shoot();
        }
    }

    // 味方に向かって発射
    void Shoot()
    {
        Collider2D[] allies =
            Physics2D.OverlapCircleAll(
                transform.position,
                searchRadius
            );

        AllyHealth nearestAlly = null;
        float nearestDistance = float.MaxValue;

        // 一番近い味方を探す
        foreach (Collider2D allyCollider in allies)
        {
            if (!allyCollider.CompareTag("Ally"))
            {
                continue;
            }

            AllyHealth ally =
                allyCollider.GetComponent<AllyHealth>();

            if (ally == null)
            {
                ally = allyCollider.GetComponentInParent<AllyHealth>();
            }

            if (ally == null) continue;

            float distance = Vector2.Distance(
                transform.position,
                ally.transform.position
            );

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestAlly = ally;
            }
        }

        // 味方がいなければ発射しない
        if (nearestAlly == null) return;

        // 発射方向を計算
        Vector2 direction =
            (nearestAlly.transform.position -
            firePoint.position).normalized;

        // イチゴを生成
        GameObject strawberry = Instantiate(
            strawberryPrefab,
            firePoint.position,
            Quaternion.identity
        );

        StrawberryProjectile projectile =
            strawberry.GetComponent<StrawberryProjectile>();

        if (projectile != null)
        {
            projectile.Initialize(
                direction,
                projectileSpeed,
                healAmount
            );
        }

        // 次の発射時間
        nextFireTime = Time.time + 1f / fireRate;
    }

    // 回復量の強化
    public void UpgradeHealAmount(int amount)
    {
        healAmount += amount;
    }

    // 発射速度の強化
    public void UpgradeFireRate(float amount)
    {
        fireRate += amount;
    }

    // 射程範囲を表示
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(
            transform.position,
            searchRadius
        );
    }
}
