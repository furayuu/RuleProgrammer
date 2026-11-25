
using UnityEngine;

public abstract class FruitEffectBase : MonoBehaviour
{
    // 命中対象に攻撃できるか確認する
    public virtual bool CanHitTarget(Transform target)
    {
        return true;
    }

    // 各フルーツの特殊効果を実行する共通メソッド
    public abstract void Apply(Transform target);

    // 命中後に弾を破棄するかどうか
    public virtual bool ShouldDestroyProjectile => true;

    // 地面に衝突したときの特殊効果
    public virtual void OnGroundHit()
    {
    }
}
