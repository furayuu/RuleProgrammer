
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerAttack : MonoBehaviour
{
    // 直射水果の種類
    public enum DirectFruitType
    {
        Apple,
        Kiwi,
        Orange,
        Watermelon
    }

    // 投擲水果の種類
    public enum ThrowFruitType
    {
        Apple,
        Banana,
        Cherry,
        Strawberry,
        Pineapple
    }

    [Header("Direct Fruit List")]
    [Tooltip("順番：AppleShoot / KiwiShoot / OrangeShoot / Melon")]
    public GameObject[] directFruitPrefabs;

    public DirectFruitType selectedDirectFruit;

    [Header("Throw Fruit List")]
    [Tooltip("順番：AppleArc / BananaArc / CherryArc / Strawberry / PineappleShoot")]
    public GameObject[] throwFruitPrefabs;

    public ThrowFruitType selectedThrowFruit;

    [Header("Projectile")]
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

    // 選択中の直射フルーツを取得する
    GameObject GetDirectFruitPrefab()
    {
        int index = (int)selectedDirectFruit;

        if (
            directFruitPrefabs == null ||
            index >= directFruitPrefabs.Length
        )
            return null;

        return directFruitPrefabs[index];
    }

    // 選択中の投擲フルーツを取得する
    GameObject GetThrowFruitPrefab()
    {
        int index = (int)selectedThrowFruit;

        if (
            throwFruitPrefabs == null ||
            index >= throwFruitPrefabs.Length
        )
            return null;

        return throwFruitPrefabs[index];
    }

    void Update()
    {
        FaceMouse();

        if (EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject())
            return;

        attackTimer -= Time.deltaTime;

        if (
            Input.GetMouseButtonDown(0) &&
            attackTimer <= 0
        )
        {
            ShootDirect();

            attackTimer =
                attackCooldown / attackSpeedMultiplier;
        }

        if (
            Input.GetMouseButtonDown(1) &&
            attackTimer <= 0
        )
        {
            ShootThrow();

            attackTimer =
                attackCooldown / attackSpeedMultiplier;
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
        GameObject prefab = GetDirectFruitPrefab();

        if (prefab == null)
        {
            Debug.LogWarning("直射フルーツが設定されていません。");
            return;
        }

        Vector3 mousePos =
            Camera.main.ScreenToWorldPoint(Input.mousePosition);

        float direction =
            Mathf.Sign(mousePos.x - transform.position.x);

        GameObject bullet =
            Instantiate(
                prefab,
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
        GameObject prefab = GetThrowFruitPrefab();

        if (prefab == null)
        {
            Debug.LogWarning("投擲フルーツが設定されていません。");
            return;
        }

        Vector3 mousePos =
            Camera.main.ScreenToWorldPoint(
                Input.mousePosition
            );

        mousePos.z = 0;

        // イチゴの場合は味方を検索する
        if (prefab.GetComponent<StrawberryEffect>() != null)
        {
            ShootStrawberry(mousePos, prefab);
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
                prefab,
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
    void ShootStrawberry(
        Vector3 mousePos,
        GameObject prefab
    )
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
                prefab,
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
