using UnityEngine;

public class ShootingEnemy : MonoBehaviour
{
    [Header("Distance")]
    public float minDistance = 5f;
    public float maxDistance = 8f;

    [Header("Shoot")]
    public float shotInterval = 1f;

    [Header("Target Refresh")]
    public float targetRefreshTime = 0.2f;

    public GameObject bulletPrefab;
    public Transform firePoint;

    private EnemyMove enemyMove;

    private float shotTimer;
    private float targetTimer;

    private Transform attackTarget;

    void Start()
    {
        enemyMove = GetComponent<EnemyMove>();

        enemyMove.autoMove = false;
    }

    void Update()
    {
        UpdateTarget();

        MoveControl();

        ShotControl();
    }

    void MoveControl()
    {
        Debug.Log(enemyMove.GetPlayer());
        Transform player = enemyMove.GetPlayer();

        if (player == null)
            return;

        float distance = Mathf.Abs(
            player.position.x - transform.position.x);

        float direction =
            Mathf.Sign(player.position.x - transform.position.x);

        if (distance > maxDistance)
        {
            enemyMove.Move(direction);
        }
        else if (distance < minDistance)
        {
            enemyMove.Move(-direction);
        }
        else
        {
            enemyMove.Move(0);
        }
    }

    void UpdateTarget()
    {
        targetTimer += Time.deltaTime;

        if (targetTimer >= targetRefreshTime)
        {
            targetTimer = 0;

            attackTarget = GetNearestTarget();
        }
    }

    void ShotControl()
    {
        Debug.Log(attackTarget);
        if (attackTarget == null)
            return;

        shotTimer += Time.deltaTime;

        if (shotTimer >= shotInterval)
        {
            shotTimer = 0;

            Shoot(attackTarget);
        }
    }

    void Shoot(Transform target)
    {
        Debug.Log("Shoot");
        Vector2 dir =
            (target.position - firePoint.position).normalized;

        GameObject bullet =
            Instantiate(
                bulletPrefab,
                firePoint.position,
                Quaternion.identity);

        bullet.GetComponent<EnemyBullet>().SetDirection(dir);
    }
    Transform GetNearestTarget()
    {
        GameObject[] players =
            GameObject.FindGameObjectsWithTag("Player");

        GameObject[] allies =
            GameObject.FindGameObjectsWithTag("Ally");

        Transform nearest = null;

        float min = Mathf.Infinity;

        foreach (GameObject obj in players)
        {
            float d =
                Vector2.Distance(
                    transform.position,
                    obj.transform.position);

            if (d < min)
            {
                min = d;
                nearest = obj.transform;
            }
        }

        foreach (GameObject obj in allies)
        {
            float d =
                Vector2.Distance(
                    transform.position,
                    obj.transform.position);

            if (d < min)
            {
                min = d;
                nearest = obj.transform;
            }
        }

        return nearest;
    }
}