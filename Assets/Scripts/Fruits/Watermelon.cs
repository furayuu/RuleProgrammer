
using UnityEngine;

public class Watermelon : MonoBehaviour
{
    [Header("Projectile")]
    public FruitType fruitType;
    public int damage = 10;
    public float lifeTime = 5f;
    public float speed = 4f;

    [Header("Fragment Settings")]
    public GameObject fragmentPrefab;
    public int fragmentCount = 6;
    public int fragmentDamage = 3;
    public float fragmentSpeed = 5f;

    private Rigidbody2D rb;
    private bool hasShattered = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    // �e�̈ړ�������ݒ�
    public void SetDirection(Vector2 direction)
    {
        if (rb != null)
        {
            rb.velocity = direction.normalized * speed;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // �G�ɖ���
        if (other.CompareTag("Enemy"))
        {
            if (hasShattered) return;

            EnemyHealth enemy =
                other.GetComponent<EnemyHealth>();

            if (enemy == null)
            {
                enemy = other.GetComponentInParent<EnemyHealth>();
            }

            // �{�̂̃_���[�W
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            // �j�􏈗�
            Shatter();
        }

        // �n�ʂ�ǂɏՓ�
        if (other.CompareTag("Ground"))
        {
            if (!hasShattered)
            {
                Shatter();
            }
        }
    }

    // �X�C�J���ӂ��鏈��
    void Shatter()
    {
        if (hasShattered) return;

        hasShattered = true;

        // �j�Ђ𐶐�
        if (fragmentPrefab != null)
        {
            for (int i = 0; i < fragmentCount; i++)
            {
                float angle = i * (360f / fragmentCount);

                Vector2 direction = new Vector2(
                    Mathf.Cos(angle * Mathf.Deg2Rad),
                    Mathf.Sin(angle * Mathf.Deg2Rad)
                );

                GameObject fragment = Instantiate(
                    fragmentPrefab,
                    transform.position,
                    Quaternion.identity
                );

                WatermelonFragment fragmentScript =
                    fragment.GetComponent<WatermelonFragment>();

                if (fragmentScript != null)
                {
                    fragmentScript.Initialize(
                        direction,
                        fragmentSpeed,
                        fragmentDamage
                    );
                }
            }
        }

        Debug.Log("Watermelon Explosion!");

        Destroy(gameObject);
    }

    // �U���͂̋���
    public void UpgradeDamage(int amount)
    {
        damage += amount;
        fragmentDamage += amount;
    }

    // �j�А��̋���
    public void UpgradeFragmentCount(int amount)
    {
        fragmentCount += amount;
    }
}
