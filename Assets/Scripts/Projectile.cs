using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile")]
    public FruitType fruitType;

    public int damage = 1;
    public float lifeTime = 3f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 命中敌人
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth enemy =
                other.GetComponent<EnemyHealth>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            ApplyFruitEffect(other);

            Destroy(gameObject);
        }

        // 撞到地面或墙体
        if (other.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }

    void ApplyFruitEffect(Collider2D target)
    {
        switch (fruitType)
        {
            case FruitType.Apple:
                AppleEffect(target);
                break;

            case FruitType.Banana:
                BananaEffect(target);
                break;

            case FruitType.Cherry:
                CherryEffect(target);
                break;

            case FruitType.Kiwi:
                KiwiEffect(target);
                break;

            case FruitType.Watermelon:
                WatermelonEffect(target);
                break;

            case FruitType.Orange:
                OrangeEffect(target);
                break;

            case FruitType.Strawberry:
                StrawberryEffect(target);
                break;

            case FruitType.Pineapple:
                PineappleEffect(target);
                break;
        }
    }

    // ===== Fruit Effects =====

    void AppleEffect(Collider2D target)
    {
        // 普通攻击
    }

    void BananaEffect(Collider2D target)
    {
        Debug.Log("Banana Split");
    }

    void CherryEffect(Collider2D target)
    {
        Debug.Log("Cherry Explosion");
    }

    void KiwiEffect(Collider2D target)
    {
        Debug.Log("Kiwi Slow");
    }

    void WatermelonEffect(Collider2D target)
    {
        Debug.Log("Watermelon Pierce");
    }

    void OrangeEffect(Collider2D target)
    {
        Debug.Log("Orange Bounce");
    }

    void StrawberryEffect(Collider2D target)
    {
        Debug.Log("Strawberry Heal");
    }

    void PineappleEffect(Collider2D target)
    {
        Debug.Log("Pineapple Trap");
    }
}