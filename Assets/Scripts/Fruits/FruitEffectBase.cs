
using UnityEngine;

public abstract class FruitEffectBase : MonoBehaviour
{
    // 各フルーツの特殊効果を実行する共通メソッド
    public abstract void Apply(Transform target);
}
