using UnityEngine;

public class ProjectileArc : MonoBehaviour
{
    [Header("Projectile")]
    public int damage = 1;

    private Vector3 startPos;
    private Vector3 targetPos;

    // 新增：锁定目标（Ally使用）
    private Transform target;

    private float throwHeight;
    private float throwDuration;

    private float timer;

    private bool initialized = false;

    // ----------------------------
    // 玩家使用（保持原来的）
    // ----------------------------
    public void Initialize(
        Vector3 start,
        Vector3 target,
        float height,
        float duration)
    {
        startPos = start;
        targetPos = target;

        this.target = null;

        throwHeight = height;
        throwDuration = duration;

        timer = 0f;
        initialized = true;
    }

    // ----------------------------
    // Ally使用（锁定敌人）
    // ----------------------------
    public void InitializeTarget(
        Vector3 start,
        Transform target,
        float height,
        float duration)
    {
        startPos = start;

        this.target = target;
        targetPos = target.position;

        throwHeight = height;
        throwDuration = duration;

        timer = 0f;
        initialized = true;
    }

    void Update()
    {
        if (!initialized)
            return;

        timer += Time.deltaTime;

        float t = timer / throwDuration;

        // Ally实时更新终点
        if (target != null)
        {
            targetPos = target.position;
        }

        if (t >= 1f)
        {
            transform.position = targetPos;

            // Ally命中
            if (target != null)
            {
                EnemyHealth enemy = target.GetComponent<EnemyHealth>();

                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                }
            }

            Destroy(gameObject);

            return;
        }

        Vector3 pos =
            Vector3.Lerp(
                startPos,
                targetPos,
                t
            );

        pos.y +=
            Mathf.Sin(
                t * Mathf.PI
            ) * throwHeight;

        transform.position = pos;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (target != null)
            return;

        if (other.CompareTag("Enemy"))
        {
            EnemyHealth enemy =
                other.GetComponent<EnemyHealth>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
    }
}