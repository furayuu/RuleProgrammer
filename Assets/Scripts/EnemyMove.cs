using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    public float moveSpeed = 2f;

    private Transform player;
    private Rigidbody2D rb;
    private Animator animator;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        MoveToPlayer();
    }

    void MoveToPlayer()
    {
        if (player == null) return;

        float direction =
            Mathf.Sign(player.position.x - transform.position.x);

        rb.velocity = new Vector2(
            direction * moveSpeed,
            rb.velocity.y
        );

        // 翻转朝向
        if (direction > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        // Animator
        animator.SetBool("IsRunning", true);
    }
}