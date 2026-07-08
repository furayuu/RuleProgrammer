using UnityEngine;

public class JumpEnemy : MonoBehaviour
{
    [Header("Jump")]
    public float jumpForce = 7f;

    private Rigidbody2D rb;
    private EnemyMove enemyMove;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyMove = GetComponent<EnemyMove>();

        if (enemyMove == null)
        {
            Debug.LogError("JumpEnemy requires EnemyMove!");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("JumpTrigger"))
            return;

        JumpTrigger trigger = other.GetComponent<JumpTrigger>();

        if (trigger == null)
            return;

        // 当前敌人面朝方向
        bool facingRight = transform.localScale.x > 0;

        // Trigger要求向右
        if (trigger.jumpDirection == JumpTrigger.JumpDirection.Right && !facingRight)
            return;

        // Trigger要求向左
        if (trigger.jumpDirection == JumpTrigger.JumpDirection.Left && facingRight)
            return;

        if (!enemyMove.IsGrounded())
            return;

        Jump();
    }

    void Jump()
    {
        rb.velocity = new Vector2(
            rb.velocity.x,
            jumpForce);
    }
}