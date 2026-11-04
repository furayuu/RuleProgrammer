
using UnityEngine;

public class JumpEnemy : MonoBehaviour
{
    [Header("Jump Settings")]
    [Tooltip("ジャンプ力")]
    public float jumpForce = 7f;

    [Tooltip("ジャンプ後、次のジャンプまでの待ち時間")]
    public float jumpCooldown = 0.5f;

    [Header("Base Target")]
    [Tooltip("未設定の場合は BaseHealth から自動取得")]
    public Transform baseTarget;

    private Rigidbody2D rb;
    private EnemyMove enemyMove;

    private float jumpTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyMove = GetComponent<EnemyMove>();

        if (rb == null)
        {
            Debug.LogError("JumpEnemy requires Rigidbody2D!");
            enabled = false;
            return;
        }

        if (enemyMove == null)
        {
            Debug.LogError("JumpEnemy requires EnemyMove!");
            enabled = false;
            return;
        }

        // EnemyMove にプレイヤーを追わせず、このスクリプトで移動する
        enemyMove.autoMove = false;

        // 基地を自動取得
        if (baseTarget == null)
        {
            BaseHealth baseHealth = FindObjectOfType<BaseHealth>();

            if (baseHealth != null)
            {
                baseTarget = baseHealth.transform;
            }
        }

        if (baseTarget == null)
        {
            Debug.LogError(
                "JumpEnemy: Base target not found. " +
                "Assign Base Target in the Inspector."
            );
        }
    }

    void Update()
    {
        if (jumpTimer > 0f)
        {
            jumpTimer -= Time.deltaTime;
        }

        // 基地に向かって移動
        if (baseTarget != null)
        {
            float direction = Mathf.Sign(
                baseTarget.position.x - transform.position.x
            );

            enemyMove.Move(direction);
        }
        else
        {
            enemyMove.Move(0f);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("JumpTrigger"))
            return;

        if (jumpTimer > 0f)
            return;

        JumpTrigger trigger = other.GetComponent<JumpTrigger>();

        if (trigger == null)
            return;

        // 現在の進行方向
        bool facingRight = transform.localScale.x > 0f;

        // 右向き専用のトリガー
        if (trigger.jumpDirection == JumpTrigger.JumpDirection.Right
            && !facingRight)
        {
            return;
        }

        // 左向き専用のトリガー
        if (trigger.jumpDirection == JumpTrigger.JumpDirection.Left
            && facingRight)
        {
            return;
        }

        // 空中では再ジャンプしない
        if (!enemyMove.IsGrounded())
            return;

        Jump();
    }

    void Jump()
    {
        rb.velocity = new Vector2(
            rb.velocity.x,
            jumpForce
        );

        jumpTimer = jumpCooldown;
    }
}
