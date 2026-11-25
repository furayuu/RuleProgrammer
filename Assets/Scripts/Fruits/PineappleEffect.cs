
using UnityEngine;

public class PineappleEffect : FruitEffectBase
{
    [Header("Trap Settings")]
    public GameObject trapPrefab;

    // 敵に命中した場合は追加効果を実行しない
    public override void Apply(Transform target)
    {
    }

    // 地面に着地したときにトラップを生成する
    public override void OnGroundHit()
    {
        if (trapPrefab == null)
        {
            Debug.LogWarning(
                "Pineapple Trap Prefabが設定されていません。"
            );
            return;
        }

        Instantiate(
            trapPrefab,
            transform.position,
            Quaternion.identity
        );

        Debug.Log("Pineapple Trap Created!");
    }
}
