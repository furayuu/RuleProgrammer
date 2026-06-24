using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Projectile")]
    public GameObject directProjectilePrefab;
    public GameObject throwProjectilePrefab;
    public Transform firePoint;

    [Header("Attack")]
    public float attackCooldown = 0.5f;

    private float attackTimer;

    [Header("Direct Attack")]
    public float bulletSpeed = 10f;

    [Header("Throw Attack")]

    [Tooltip("抛物线高度")]
    public float throwHeight = 3f;

    [Tooltip("基础飞行时间")]
    public float throwDuration = 0.8f;

    [Header("Ground Detection")]
    public LayerMask groundLayer;

    [Header("Upgrades")]

    // 攻击力倍率
    public float damageMultiplier = 1f;

    // 攻击速度倍率
    public float attackSpeedMultiplier = 1f;

    // 投掷速度倍率
    public float throwSpeedMultiplier = 1f;

    void Update()
    {
        FaceMouse();

        attackTimer -= Time.deltaTime;

        if (
            Input.GetMouseButtonDown(0)
            && attackTimer <= 0
        )
        {
            ShootDirect();

            attackTimer =
                attackCooldown
                / attackSpeedMultiplier;
        }

        if (
            Input.GetMouseButtonDown(1)
            && attackTimer <= 0
        )
        {
            ShootThrow();

            attackTimer =
                attackCooldown
                / attackSpeedMultiplier;
        }
    }

    void FaceMouse()
    {
        Vector3 mousePos =
            Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (mousePos.x > transform.position.x)
        {
            transform.localScale = Vector3.one;
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    // 左键：直射
    void ShootDirect()
    {
        Vector3 mousePos =
            Camera.main.ScreenToWorldPoint(Input.mousePosition);

        float direction =
            Mathf.Sign(mousePos.x - transform.position.x);

        GameObject bullet =
            Instantiate(
                directProjectilePrefab,
                firePoint.position,
                Quaternion.identity
            );
        Projectile projectile =
            bullet.GetComponent<Projectile>();

        if (projectile != null)
        {
            projectile.damage =
                Mathf.RoundToInt(
                    projectile.damage * damageMultiplier
                );
        }

        Rigidbody2D rb =
            bullet.GetComponent<Rigidbody2D>();

        rb.velocity = new Vector2(
            direction * bulletSpeed,
            0f
        );

        if (direction < 0)
        {
            bullet.transform.localScale =
                new Vector3(-1, 1, 1);
        }
    }

    // 右键：投掷
    void ShootThrow()
    {
        Vector3 mousePos =
            Camera.main.ScreenToWorldPoint(
                Input.mousePosition
            );

        mousePos.z = 0;

        RaycastHit2D hit =
            Physics2D.Raycast(
                mousePos,
                Vector2.down,
                100f,
                groundLayer
            );

        if (!hit)
        {
            return;
        }

        Vector3 targetPos = hit.point;

        GameObject bullet =
            Instantiate(
                throwProjectilePrefab,
                firePoint.position,
                Quaternion.identity
            );
        ProjectileArc arc =
            bullet.GetComponent<ProjectileArc>();

        if (arc != null)
        {
            arc.damage =
                Mathf.RoundToInt(
                    arc.damage * damageMultiplier
                );
        }

        arc.Initialize(
            firePoint.position,
            targetPos,
            throwHeight,
            throwDuration / throwSpeedMultiplier
        );
    }
}