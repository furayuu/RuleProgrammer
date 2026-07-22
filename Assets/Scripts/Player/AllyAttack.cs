using UnityEngine;

public class AllyAttack : MonoBehaviour
{
    [Header("Attack")]
    public float attackRange = 8f;
    public float attackInterval = 1.5f;

    [Header("Throw")]
    public float throwHeight = 2f;
    public float throwDuration = 0.5f;

    [Header("Projectile")]
    public GameObject projectilePrefab;
    public Transform firePoint;

    private float attackTimer;

    private Transform currentTarget;

    void Update()
    {
        if (currentTarget == null || !TargetValid(currentTarget))
        {
            currentTarget = GetNearestEnemy();
        }


        FaceTarget();

        attackTimer += Time.deltaTime;

        if (attackTimer >= attackInterval)
        {
            attackTimer = 0;

            if (currentTarget != null)
            {
                Shoot(currentTarget);
            }
        }
    }

    void Shoot(Transform target)
    {
        GameObject obj = Instantiate(
            projectilePrefab,
            firePoint.position,
            Quaternion.identity);

        ProjectileArc arc =
            obj.GetComponent<ProjectileArc>();

        if (arc != null)
        {
            arc.InitializeTarget(
                firePoint.position,
                target,
                throwHeight,
                throwDuration);
        }
    }
    void FaceTarget()
    {
        if (currentTarget == null)
            return;

        float direction =
            currentTarget.position.x - transform.position.x;

        transform.localScale = new Vector3(
            direction > 0 ? 1 : -1,
            1,
            1);
    }

    bool TargetValid(Transform target)
    {
        if (target == null)
            return false;

        float distance = Vector2.Distance(
            transform.position,
            target.position);

        return distance <= attackRange;
    }

    Transform GetNearestEnemy()
    {
        GameObject[] enemies =
            GameObject.FindGameObjectsWithTag("Enemy");

        Transform nearest = null;

        float minDistance = attackRange;

        foreach (GameObject enemy in enemies)
        {
            if (enemy == null)
                continue;

            float distance =
                Vector2.Distance(
                    transform.position,
                    enemy.transform.position);

            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = enemy.transform;
            }
        }

        return nearest;
    }
}