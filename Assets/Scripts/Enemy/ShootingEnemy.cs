
using UnityEngine;

public class ShootingEnemy : MonoBehaviour
{
    [Header("Base Target")]
    [Tooltip("基地的 Transform。未設定の場合は BaseHealth から自動取得")]
    public Transform baseTarget;

    [Tooltip("基地の射撃範囲")]
    public float baseShootRange = 6f;

    [Header("Player Detection")]
    [Tooltip("プレイヤーを警戒し始める距離")]
    public float playerAlertDistance = 6f;

    [Tooltip("プレイヤーが近づきすぎた場合に後退する距離")]
    public float minDistance = 3f;

    [Tooltip("プレイヤーから離れすぎないための距離")]
    public float maxDistance = 6f;

    [Tooltip("プレイヤーがこの距離より遠くなると基地攻撃に戻るための追加距離")]
    public float returnDistanceMargin = 2f;

    [Header("Shoot")]
    [Tooltip("Player と Ally を攻撃できる最大距離")]
    public float attackRange = 8f;

    [Tooltip("射撃間隔")]
    public float shotInterval = 1f;

    public GameObject bulletPrefab;
    public Transform firePoint;

    [Header("Target Refresh")]
    public float targetRefreshTime = 0.2f;

    private EnemyMove enemyMove;

    private float shotTimer;
    private float targetTimer;

    private Transform attackTarget;
    private Transform playerTarget;

    private bool respondingToPlayer = false;

    void Start()
    {
        enemyMove = GetComponent<EnemyMove>();

        if (enemyMove == null)
        {
            Debug.LogError("ShootingEnemy requires EnemyMove!", this);
            enabled = false;
            return;
        }

        // Disable the default movement toward the player.
        enemyMove.autoMove = false;

        // Automatically find the base if it was not assigned.
        if (baseTarget == null)
        {
            BaseHealth baseHealth = FindObjectOfType<BaseHealth>();

            if (baseHealth != null)
                baseTarget = baseHealth.transform;
        }

        if (baseTarget == null)
        {
            Debug.LogError(
                "ShootingEnemy: Base not found. Assign Base Target in the Inspector.",
                this
            );
        }

        if (bulletPrefab == null)
        {
            Debug.LogError("ShootingEnemy: Bullet Prefab is not assigned.", this);
        }

        if (firePoint == null)
        {
            Debug.LogError("ShootingEnemy: Fire Point is not assigned.", this);
        }
    }

    void Update()
    {
        UpdatePlayerMode();
        MoveControl();
        UpdateTarget();
        ShotControl();
    }

    // Decide whether to defend against the approaching player.
    void UpdatePlayerMode()
    {
        playerTarget = enemyMove.GetPlayer();

        if (playerTarget == null)
        {
            respondingToPlayer = false;
            return;
        }

        float playerDistance = Mathf.Abs(
            playerTarget.position.x - transform.position.x
        );

        if (!respondingToPlayer &&
            playerDistance <= playerAlertDistance)
        {
            respondingToPlayer = true;
        }
        else if (respondingToPlayer &&
                 playerDistance > playerAlertDistance + returnDistanceMargin)
        {
            respondingToPlayer = false;
        }
    }

    // Player mode: maintain distance from the player.
    // Normal mode: advance toward the base and stop within shooting range.
    void MoveControl()
    {
        if (respondingToPlayer && playerTarget != null)
        {
            float distance = Mathf.Abs(
                playerTarget.position.x - transform.position.x
            );

            float direction = Mathf.Sign(
                playerTarget.position.x - transform.position.x
            );

            if (distance < minDistance)
            {
                // Too close: retreat.
                enemyMove.Move(-direction);
            }
            else if (distance > maxDistance)
            {
                // Still in player mode: move back toward the player.
                enemyMove.Move(direction);
            }
            else
            {
                // Maintain a suitable distance.
                enemyMove.Move(0f);
            }

            return;
        }

        if (baseTarget == null)
        {
            enemyMove.Move(0f);
            return;
        }

        float baseDistance = Mathf.Abs(
            baseTarget.position.x - transform.position.x
        );

        if (baseDistance > baseShootRange)
        {
            float direction = Mathf.Sign(
                baseTarget.position.x - transform.position.x
            );

            enemyMove.Move(direction);
        }
        else
        {
            // Reached the base shooting position.
            enemyMove.Move(0f);
        }
    }

    // Refresh the current attack target periodically.
    void UpdateTarget()
    {
        targetTimer += Time.deltaTime;

        if (targetTimer < targetRefreshTime)
            return;

        targetTimer = 0f;

        Transform nearestTarget = GetNearestTarget();

        if (nearestTarget != null)
        {
            // Attack the nearest Player or Ally in range.
            attackTarget = nearestTarget;
        }
        else if (baseTarget != null &&
                 GetHorizontalDistance(baseTarget) <= baseShootRange)
        {
            // If no Player or Ally is in range, attack the base.
            attackTarget = baseTarget;
        }
        else
        {
            attackTarget = null;
        }
    }

    void ShotControl()
    {
        if (attackTarget == null ||
            bulletPrefab == null ||
            firePoint == null)
        {
            return;
        }

        shotTimer += Time.deltaTime;

        if (shotTimer >= shotInterval)
        {
            shotTimer = 0f;
            Shoot(attackTarget);
        }
    }

    void Shoot(Transform target)
    {
        if (target == null)
            return;

        Vector2 direction =
            ((Vector2)target.position - (Vector2)firePoint.position).normalized;

        GameObject bullet = Instantiate(
            bulletPrefab,
            firePoint.position,
            Quaternion.identity
        );

        EnemyBullet enemyBullet = bullet.GetComponent<EnemyBullet>();

        if (enemyBullet != null)
        {
            enemyBullet.SetDirection(direction);
        }
        else
        {
            Debug.LogError(
                "ShootingEnemy: Bullet Prefab requires EnemyBullet!",
                bullet
            );
            Destroy(bullet);
        }
    }

    // Find the nearest Player or Ally within attackRange.
    Transform GetNearestTarget()
    {
        Transform nearest = null;
        float nearestDistance = attackRange;

        GameObject[] players =
            GameObject.FindGameObjectsWithTag("Player");

        GameObject[] allies =
            GameObject.FindGameObjectsWithTag("Ally");

        foreach (GameObject obj in players)
        {
            float distance = Vector2.Distance(
                transform.position,
                obj.transform.position
            );

            if (distance <= nearestDistance)
            {
                nearestDistance = distance;
                nearest = obj.transform;
            }
        }

        foreach (GameObject obj in allies)
        {
            float distance = Vector2.Distance(
                transform.position,
                obj.transform.position
            );

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = obj.transform;
            }
        }

        return nearest;
    }

    float GetHorizontalDistance(Transform target)
    {
        return Mathf.Abs(target.position.x - transform.position.x);
    }
}
