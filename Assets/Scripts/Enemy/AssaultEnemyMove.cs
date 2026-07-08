using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssaultEnemyMove : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Move")]
    public float moveSpeed = 3f;

    [Header("Jump")]
    public float jumpForce = 7f;
    public float jumpTargetHeight = 1f;

    [Header("Detection")]
    public float allyDetectRange = 8f;

    [Header("Platform Detection")]
    public float rayDistance = 5f;
    public LayerMask platformLayer;
    public float allySearchRadius = 2f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    public Transform platformCheck;

    [Header("Ally Detection")]
    public float allyDetect = 2f;
    private Rigidbody2D rb;
    private Transform currentTarget;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

            if (playerObj != null)
                player = playerObj.transform;
        }
    }

    void Update()
    {
        //SelectTarget();

        currentTarget = player;

        if (currentTarget == null)
            return;

        if (IsAllyNearby())
        {
            return;
        }

        MoveToTarget();

        CheckJump();
    }

    void MoveToTarget()
    {
        float direction =
            Mathf.Sign(
                currentTarget.position.x -
                transform.position.x);

        rb.velocity = new Vector2(direction * moveSpeed, rb.velocity.y);

        Vector3 scale = transform.localScale;

        if (direction > 0)
            scale.x = Mathf.Abs(scale.x);
        else
            scale.x = -Mathf.Abs(scale.x);

        transform.localScale = scale;
    }

    void CheckJump()
    {
        Debug.Log("Grounded : " + IsGrounded());
        if (!IsGrounded())
            return;

        float dir = Mathf.Sign(transform.localScale.x);

        Vector2 rayDirection =
            new Vector2(dir, 2f).normalized;

        RaycastHit2D platformHit =
            Physics2D.Raycast(
                transform.position,
                rayDirection,
                rayDistance,
                platformLayer);

        if (platformHit.collider != null)
        {
            Collider2D[] allies =
                Physics2D.OverlapCircleAll(
                    platformHit.point,
                    allySearchRadius);

            foreach (Collider2D col in allies)
            {
                if (!col.CompareTag("Ally"))
                    continue;

                Jump();
                return;
            }
        }
    }

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    bool IsGrounded()
    {
        return Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer);
    }

    void OnDrawGizmosSelected()
    {
        float dir = Mathf.Sign(transform.localScale.x);

        Vector2 rayDirection =
            new Vector2(dir, 2f).normalized;

        Gizmos.color = Color.yellow;

        Gizmos.DrawLine(
            transform.position,
            transform.position +
            (Vector3)(rayDirection * rayDistance));

        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(
            transform.position,
            allyDetectRange);
    }

    bool IsAllyNearby()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            allyDetectRange);

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Ally"))
            {
                return true;
            }
        }

        return false;
    }
}
