using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    [Header("Move")]
    public float moveSpeed = 2f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [HideInInspector]
    public bool canMove = true;

    // true：自动追玩家（默认）
    // false：由其它脚本调用 Move()
    [HideInInspector]
    public bool autoMove = true;

    protected bool isGrounded;

    private Transform player;
    private Rigidbody2D rb;
    private Animator animator;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
            player = playerObj.transform;

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer);

        animator.SetBool("IsGrounded", isGrounded);

        if (!canMove)
            return;

        if (autoMove)
        {
            MoveToPlayer();
        }
    }

    void MoveToPlayer()
    {
        if (player == null)
            return;

        float direction =
            Mathf.Sign(player.position.x - transform.position.x);

        Move(direction);
    }

    public void Move(float direction)
    {
        rb.velocity = new Vector2(
            direction * moveSpeed,
            rb.velocity.y);

        if (direction != 0)
        {
            transform.localScale = new Vector3(
                direction > 0 ? 1 : -1,
                1,
                1);
        }

        animator.SetBool(
            "IsRunning",
            Mathf.Abs(direction) > 0.01f);
    }

    public bool IsGrounded()
    {
        return isGrounded;
    }

    public Transform GetPlayer()
    {
        return player;
    }

    public Rigidbody2D GetRigidBody()
    {
        return rb;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
            return;

        Gizmos.color = isGrounded ? Color.green : Color.red;

        Gizmos.DrawWireSphere(
            groundCheck.position,
            groundCheckRadius);
    }
}