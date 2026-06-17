using UnityEngine;

public class ProjectileArc : MonoBehaviour
{
    [Header("Projectile")]
    public int damage = 1;

    private Vector3 startPos;
    private Vector3 targetPos;

    private float throwHeight;
    private float throwDuration;

    private float timer;

    private bool initialized = false;

    public void Initialize(
        Vector3 start,
        Vector3 target,
        float height,
        float duration)
    {
        startPos = start;
        targetPos = target;

        throwHeight = height;
        throwDuration = duration;

        initialized = true;
    }

    void Update()
    {
        if (!initialized)
            return;

        timer += Time.deltaTime;

        float t = timer / throwDuration;

        if (t >= 1f)
        {
            transform.position = targetPos;

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

    private void OnTriggerEnter2D(
        Collider2D other
    )
    {
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