
using System.Collections.Generic;
using UnityEngine;

public class OrangeEffect : FruitEffectBase
{
    [Header("跳ね返り設定")]
    public int maxBounceCount = 2;
    public float searchRadius = 6f;

    [Header("跳ね返り弾のPrefab")]
    public GameObject bounceArcPrefab;

    [Header("放物線設定")]
    public float bounceHeight = 2f;
    public float bounceDuration = 0.6f;

    // 連続攻撃で共有するデータ
    private class OrangeChainState
    {
        public int currentBounceCount = 0;
        public int maxBounceCount;

        public float searchRadius;
        public float bounceHeight;
        public float bounceDuration;

        public GameObject bounceArcPrefab;

        public HashSet<EnemyHealth> hitEnemies =
            new HashSet<EnemyHealth>();
    }

    private OrangeChainState chainState;

    void Awake()
    {
        // 共有データを初期化する
        InitializeState();
    }

    // 共有データを初期化する
    private void InitializeState()
    {
        if (chainState != null)
            return;

        chainState = new OrangeChainState();

        chainState.maxBounceCount = maxBounceCount;
        chainState.searchRadius = searchRadius;
        chainState.bounceHeight = bounceHeight;
        chainState.bounceDuration = bounceDuration;
        chainState.bounceArcPrefab = bounceArcPrefab;
    }

    // 前の弾から攻撃データを引き継ぐ
    public void CopyStateFrom(OrangeEffect previous)
    {
        if (previous == null)
            return;

        previous.InitializeState();

        chainState = previous.chainState;
    }

    // 同じ敵への重複攻撃を防ぐ
    public override bool CanHitTarget(Transform target)
    {
        if (target == null)
            return false;

        InitializeState();

        EnemyHealth enemy =
            target.GetComponentInParent<EnemyHealth>();

        if (enemy == null || enemy.IsDead)
            return false;

        return chainState.hitEnemies.Add(enemy);
    }

    // 命中後に次の敵を探す
    public override void Apply(Transform target)
    {
        if (target == null)
            return;

        InitializeState();

        // 跳ね返り回数を確認する
        if (
            chainState.currentBounceCount >=
            chainState.maxBounceCount
        )
            return;

        // 次の敵を探す
        EnemyHealth nextEnemy = FindNextEnemy();

        if (nextEnemy == null)
            return;

        if (chainState.bounceArcPrefab == null)
        {
            Debug.LogWarning(
                "OrangeArc Prefabが設定されていません。"
            );
            return;
        }

        // 次の弾を生成する
        GameObject nextBullet = Instantiate(
            chainState.bounceArcPrefab,
            transform.position,
            Quaternion.identity
        );

        ProjectileArc arc =
            nextBullet.GetComponent<ProjectileArc>();

        OrangeEffect nextEffect =
            nextBullet.GetComponent<OrangeEffect>();

        if (arc == null || nextEffect == null)
        {
            Debug.LogWarning(
                "OrangeArcにProjectileArcまたはOrangeEffectがありません。"
            );

            Destroy(nextBullet);
            return;
        }

        // 攻撃データを引き継ぐ
        nextEffect.CopyStateFrom(this);

        // 前の弾と同じダメージを設定する
        Projectile projectile =
            GetComponent<Projectile>();

        ProjectileArc previousArc =
            GetComponent<ProjectileArc>();

        if (projectile != null)
        {
            arc.damage = projectile.damage;
        }
        else if (previousArc != null)
        {
            arc.damage = previousArc.damage;
        }

        // 跳ね返り回数を加算する
        chainState.currentBounceCount++;

        // 次の敵を追跡して投げる
        arc.InitializeTarget(
            transform.position,
            nextEnemy.transform,
            chainState.bounceHeight,
            chainState.bounceDuration
        );
    }

    // 最も近い未命中の敵を探す
    private EnemyHealth FindNextEnemy()
    {
        Collider2D[] colliders =
            Physics2D.OverlapCircleAll(
                transform.position,
                chainState.searchRadius
            );

        EnemyHealth nearestEnemy = null;

        float nearestDistance = float.MaxValue;

        HashSet<EnemyHealth> checkedEnemies =
            new HashSet<EnemyHealth>();

        foreach (Collider2D col in colliders)
        {
            EnemyHealth enemy =
                col.GetComponentInParent<EnemyHealth>();

            if (
                enemy == null ||
                enemy.IsDead ||
                chainState.hitEnemies.Contains(enemy)
            )
                continue;

            // 同じ敵の複数Colliderを除外する
            if (!checkedEnemies.Add(enemy))
                continue;

            float distance = Vector2.Distance(
                transform.position,
                enemy.transform.position
            );

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestEnemy = enemy;
            }
        }

        return nearestEnemy;
    }

    // 跳ね返り回数を強化する
    public void UpgradeBounceCount(int amount)
    {
        InitializeState();

        chainState.maxBounceCount =
            Mathf.Max(
                0,
                chainState.maxBounceCount + amount
            );
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            transform.position,
            searchRadius
        );
    }
}
