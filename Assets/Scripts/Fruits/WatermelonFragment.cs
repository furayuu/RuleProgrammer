
using System.Collections.Generic;
using UnityEngine;

public class WatermelonFragment : FruitEffectBase
{
    // 攻撃済みの敵を記録する
    private HashSet<EnemyHealth> hitEnemies =
        new HashSet<EnemyHealth>();

    // 同じ敵に二度命中しないようにする
    public override bool CanHitTarget(Transform target)
    {
        if (target == null)
            return false;

        EnemyHealth enemy =
            target.GetComponentInParent<EnemyHealth>();

        if (enemy == null || enemy.IsDead)
            return false;

        // 初めて命中した敵だけ攻撃を許可する
        return hitEnemies.Add(enemy);
    }

    // 小さなスイカには追加の特殊効果はない
    public override void Apply(Transform target)
    {
    }

    // 敵に命中しても弾を破棄しない
    public override bool ShouldDestroyProjectile => false;
}
