using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [Header("Bullet")]
    public float speed = 8f;
    public int damage = 1;
    public float lifeTime = 5f;

    private Vector2 direction;

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Player"))
        {
            PlayerHealth player = collision.GetComponent<PlayerHealth>();

            if (player != null)
            {
                player.TakeDamage(damage);
            }

            Destroy(gameObject);
            return;
        }

        if (collision.CompareTag("Ally"))
        {
            AllyHealth ally = collision.GetComponent<AllyHealth>();

            if (ally != null)
            {
                ally.TakeDamage(damage);
            }

            Destroy(gameObject);
            return;
        }

        if (collision.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}