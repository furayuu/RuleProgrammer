
using System.Collections.Generic;
using UnityEngine;

public class KiwiEffect : FruitEffectBase
{
    [Header("貫通設定")]
    public int maxPierceCount = 3;

    [Header("強化設定")]
    public int bonusDamage = 0;

    private int currentPierceCount = 0;

    private HashSet<EnemyHealth> hitEnemies =
        new HashSet<EnemyHealth>();

    // 同じ敵に二度命中しないようにする
    public override bool CanHitTarget(Transform target)
    {
        if (target == null)
            return false;

        EnemyHealth enemy =
            target.GetComponentInParent<EnemyHealth>();

        if (enemy == null)
            return false;

        return hitEnemies.Add(enemy);
    }

    // 貫通回数の上限に達したら弾を破棄する
    public override bool ShouldDestroyProjectile
    {
        get
        {
            return currentPierceCount >=
                Mathf.Max(1, maxPierceCount);
        }
    }

    public override void Apply(Transform target)
    {
        if (target == null)
            return;

        EnemyHealth enemy =
            target.GetComponentInParent<EnemyHealth>();

        if (enemy == null)
            return;

        // 強化による追加ダメージを与える
        if (bonusDamage > 0)
        {
            enemy.TakeDamage(bonusDamage);
        }

        // 命中回数を加算する
        currentPierceCount++;

        Debug.Log(
            "Kiwi Pierce! " +
            currentPierceCount + "/" + maxPierceCount
        );
    }

    // 貫通数を強化する
    public void UpgradePierceCount(int amount)
    {
        maxPierceCount =
            Mathf.Max(1, maxPierceCount + amount);
    }

    // 攻撃力を強化する
    public void UpgradeDamage(int amount)
    {
        bonusDamage += amount;
    }
}
