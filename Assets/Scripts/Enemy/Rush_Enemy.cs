using System.Collections;
using UnityEngine;

public class Rush_Enemy : MonoBehaviour
{
    [Header("Rush Settings")]
    public float detectDistance = 4f;         
    public float preferredDistance = 2f;       
    public float chargeTime = 0.5f;
    public float rushSpeed = 6f;
    public float rushTargetOffset = 2f;        
    public float cooldown = 1.5f;

    private EnemyMove enemyMove;
    private Rigidbody2D rb;
    private Transform player;

    private bool isBusy = false;
    private float rushDirection;
    private float targetX;

    void Start()
    {
        enemyMove = GetComponent<EnemyMove>();
        rb = enemyMove.GetRigidBody();
        player = enemyMove.GetPlayer();

        enemyMove.autoMove = false;
    }

    void Update()
    {
        if (player == null || isBusy)
            return;

        KeepDistance();

        float distance = Mathf.Abs(player.position.x - transform.position.x);

        if (distance <= detectDistance)
        {
            StartCoroutine(RushRoutine());
        }
    }
    void KeepDistance()
    {
        float distance = Mathf.Abs(player.position.x - transform.position.x);
        float direction = Mathf.Sign(player.position.x - transform.position.x);

        if (distance > preferredDistance + 0.2f)
        {
            enemyMove.Move(direction);
        }
        else if (distance < preferredDistance - 0.2f)
        {
            enemyMove.Move(-direction);
        }
        else
        {
            enemyMove.Move(0);
        }
    }

    IEnumerator RushRoutine()
    {
        isBusy = true;

        enemyMove.canMove = false;

        rushDirection = Mathf.Sign(player.position.x - transform.position.x);

        targetX = player.position.x + rushDirection * rushTargetOffset;

        rb.velocity = Vector2.zero;

        yield return new WaitForSeconds(chargeTime);

        while (Mathf.Abs(transform.position.x - targetX) > 0.1f)
        {
            rb.velocity = new Vector2(
                rushDirection * rushSpeed,
                rb.velocity.y);

            yield return null;
        }

        rb.velocity = new Vector2(0, rb.velocity.y);

        enemyMove.canMove = true;

        yield return new WaitForSeconds(cooldown);

        isBusy = false;
    }
}