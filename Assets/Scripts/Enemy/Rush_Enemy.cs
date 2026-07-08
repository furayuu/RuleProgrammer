using System.Collections;
using UnityEngine;

public class Rush_Enemy : MonoBehaviour
{
    [Header("Rush Settings")]
    public float detectDistance = 3f;     
    public float chargeTime = 0.5f;       
    public float rushSpeed = 6f;          
    public float rushTime = 1f;           
    public float cooldown = 1.5f;         

    private EnemyMove enemyMove;
    private Rigidbody2D rb;
    private Transform player;

    private bool isBusy = false;
    private float rushDirection;

    void Start()
    {
        enemyMove = GetComponent<EnemyMove>();
        rb = enemyMove.GetRigidBody();
        player = enemyMove.GetPlayer();
    }

    void Update()
    {
        if (player == null || isBusy)
            return;

        float distance = Mathf.Abs(player.position.x - transform.position.x);

        if (distance <= detectDistance)
        {
            StartCoroutine(RushRoutine());
        }
    }

    IEnumerator RushRoutine()
    {
        isBusy = true;

        enemyMove.canMove = false;

        rushDirection = Mathf.Sign(player.position.x - transform.position.x);

        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(chargeTime);

        float timer = 0f;

        while (timer < rushTime)
        {
            rb.velocity = new Vector2(
                rushDirection * rushSpeed,
                rb.velocity.y
            );

            timer += Time.deltaTime;
            yield return null;
        }

        rb.velocity = new Vector2(0, rb.velocity.y);

        enemyMove.canMove = true;

        yield return new WaitForSeconds(cooldown);

        isBusy = false;
    }
}