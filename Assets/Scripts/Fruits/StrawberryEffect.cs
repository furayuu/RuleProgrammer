
using UnityEngine;

public class StrawberryEffect : FruitEffectBase
{
    [Header("回復設定")]
    public int healAmount = 5;

    // 味方ユニットを回復する
    public override void Apply(Transform target)
    {
        if (target == null)
            return;

        // AllyHealthを取得する
        AllyHealth ally =
            target.GetComponentInParent<AllyHealth>();

        if (ally == null)
            return;

        // 味方のHPを回復する
        ally.Heal(healAmount);

        Debug.Log("Strawberry Heal: " + healAmount);
    }
}
