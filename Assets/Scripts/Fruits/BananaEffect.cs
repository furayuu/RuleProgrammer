
using UnityEngine;

public class BananaEffect : FruitEffectBase
{
    public override void Apply(Transform target)
    {
        if (target == null)
            return;

        // バナナの特殊効果を実行する
        Debug.Log("Banana Split Effect Triggered!");
    }
}
