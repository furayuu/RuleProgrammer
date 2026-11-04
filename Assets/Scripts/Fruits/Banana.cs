
using UnityEngine;

public class Banana : MonoBehaviour
{
    [Header("çUåÇê›íË")]
    public float damage = 10f;
    public float speed = 8f;
    public float lifetime = 5f;

    [Header("ï™óÙê›íË")]
    public GameObject splitBananaPrefab;
    public int splitCount = 4;
    public float splitSearchRadius = 6f;

    private Rigidbody2D rb;
    private Vector2 moveDirection = Vector2.right;
    private bool isSplit;
    private bool hasHit;
    private Transform target;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        Destroy(gameObject, lifetime);

        if (isSplit)
        {
            FindNearestEnemy();
        }
    }

    public void SetDirection(Vector2 direction)
    {
        if (direction.sqrMagnitude > 0f)
        {
            moveDirection = direction.normalized;
        }
    }

    public void InitializeSplitBanana()
    {
        isSplit = true;
        FindNearestEnemy();
    }

    void Update()
    {
        if (hasHit)
        {
            return;
        }

        if (isSplit)
        {
            if (target == null)
            {
                FindNearestEnemy();
            }

            if (target != null)
            {
                Vector2 direction =
                    ((Vector2)target.position -
                    (Vector2)transform.position).normalized;

                moveDirection = direction;
            }
        }

        transform.position +=
            (Vector3)(moveDirection * speed * Time.deltaTime);
    }

    void FindNearestEnemy()
    {
        Collider2D[] enemies =
            Physics2D.OverlapCircleAll(
                transform.position,
                splitSearchRadius
            );

        float nearestDistance = float.MaxValue;
        target = null;

        foreach (Collider2D enemy in enemies)
        {
            if (!enemy.CompareTag("Enemy"))
            {
                continue;
            }

            float distance = Vector2.Distance(
                transform.position,
                enemy.transform.position
            );

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                target = enemy.transform;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit || !other.CompareTag("Enemy"))
        {
            return;
        }

        EnemyHealth enemy =
            other.GetComponentInParent<EnemyHealth>();

        if (enemy == null)
        {
            return;
        }

        hasHit = true;

        enemy.TakeDamage((int)damage);

        if (!isSplit && splitBananaPrefab != null)
        {
            SplitBananas();
        }

        Destroy(gameObject);
    }

    void SplitBananas()
    {
        for (int i = 0; i < splitCount; i++)
        {
            GameObject banana = Instantiate(
                splitBananaPrefab,
                transform.position,
                Quaternion.identity
            );

            Banana projectile =
                banana.GetComponent<Banana>();

            if (projectile != null)
            {
                projectile.InitializeSplitBanana();
            }
        }
    }
}
