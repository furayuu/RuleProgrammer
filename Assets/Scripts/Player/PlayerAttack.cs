
using UnityEngine;
using UnityEngine.EventSystems;

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
    [Tooltip("抛物線高度")]
    public float throwHeight = 3f;

    [Tooltip("基本飛行時間")]
    public float throwDuration = 0.8f;

    [Header("Throw Range")]
    public float maxThrowDistance = 10f;

    [Header("Strawberry")]
    public float strawberryLockRadius = 3f;

    [Header("Ground Detection")]
    public LayerMask groundLayer;

    [Header("Upgrades")]
    public float damageMultiplier = 1f;
    public float attackSpeedMultiplier = 1f;
    public float throwSpeedMultiplier = 1f;

    void Update()
    {
        FaceMouse();

        if (EventSystem.current.IsPointerOverGameObject())
            return;

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

    // マウスの方向にプレイヤーを向ける
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

    // 直射攻撃
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

        // チェリーの爆発ダメージにも倍率を適用する
        CherryEffect cherryEffect =
            bullet.GetComponent<CherryEffect>();

        if (cherryEffect != null)
        {
            cherryEffect.damageMultiplier =
                damageMultiplier;
        }

        Rigidbody2D rb =
            bullet.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.velocity = new Vector2(
                direction * bulletSpeed,
                0f
            );
        }
    }

    // 投擲攻撃
    void ShootThrow()
    {
        Vector3 mousePos =
            Camera.main.ScreenToWorldPoint(
                Input.mousePosition
            );

        mousePos.z = 0;

        // イチゴの場合は味方を検索する
        if (
            throwProjectilePrefab != null &&
            throwProjectilePrefab.GetComponent<StrawberryEffect>() != null
        )
        {
            ShootStrawberry(mousePos);
            return;
        }

        // 通常の投擲処理
        Vector3 direction =
            mousePos - firePoint.position;

        direction.z = 0;

        if (direction.magnitude > maxThrowDistance)
        {
            direction =
                direction.normalized * maxThrowDistance;

            mousePos =
                firePoint.position + direction;
        }

        RaycastHit2D hit =
            Physics2D.Raycast(
                mousePos,
                Vector2.down,
                100f,
                groundLayer
            );

        if (!hit)
            return;

        Vector3 targetPos = hit.point;

        GameObject bullet =
            Instantiate(
                throwProjectilePrefab,
                firePoint.position,
                Quaternion.identity
            );

        ProjectileArc arc =
            bullet.GetComponent<ProjectileArc>();

        if (arc == null)
        {
            Destroy(bullet);
            return;
        }

        arc.damage =
            Mathf.RoundToInt(
                arc.damage * damageMultiplier
            );

        // チェリーの爆発ダメージにも倍率を適用する
        CherryEffect cherryEffect =
            bullet.GetComponent<CherryEffect>();

        if (cherryEffect != null)
        {
            cherryEffect.damageMultiplier =
                damageMultiplier;
        }

        arc.Initialize(
            firePoint.position,
            targetPos,
            throwHeight,
            throwDuration / throwSpeedMultiplier
        );
    }

    // イチゴを味方に向かって投げる
    void ShootStrawberry(Vector3 mousePos)
    {
        AllyHealth target = FindNearestAlly(mousePos);

        // 範囲内に味方がいない場合は発射しない
        if (target == null)
            return;

        // 最大投擲距離を確認する
        if (
            Vector2.Distance(
                firePoint.position,
                target.transform.position
            ) > maxThrowDistance
        )
            return;

        GameObject bullet =
            Instantiate(
                throwProjectilePrefab,
                firePoint.position,
                Quaternion.identity
            );

        ProjectileArc arc =
            bullet.GetComponent<ProjectileArc>();

        if (arc == null)
        {
            Destroy(bullet);
            return;
        }

        // 味方をロックオンして放物線で投げる
        arc.InitializeTarget(
            firePoint.position,
            target.transform,
            throwHeight,
            throwDuration / throwSpeedMultiplier
        );
    }

    // マウスに最も近い味方を探す
    AllyHealth FindNearestAlly(Vector3 mousePos)
    {
        Collider2D[] colliders =
            Physics2D.OverlapCircleAll(
                mousePos,
                strawberryLockRadius
            );

        AllyHealth nearestAlly = null;

        float nearestDistance = float.MaxValue;

        foreach (Collider2D col in colliders)
        {
            AllyHealth ally =
                col.GetComponentInParent<AllyHealth>();

            if (ally == null)
                continue;

            float distance =
                Vector2.Distance(
                    mousePos,
                    ally.transform.position
                );

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestAlly = ally;
            }
        }

        return nearestAlly;
    }
}
