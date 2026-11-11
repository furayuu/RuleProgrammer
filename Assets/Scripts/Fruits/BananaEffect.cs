
using System.Collections.Generic;
using UnityEngine;

public class BananaEffect : FruitEffectBase
{
    [Header("分裂設定")]
    public bool canSplit = true;
    public int maxSplitCount = 3;

    [Header("索敵設定")]
    public float searchRadius = 5f;

    [Header("小さなバナナの設定")]
    public int miniBananaDamage = 1;
    public float miniBananaHeight = 2f;
    public float miniBananaDuration = 0.6f;

    public override void Apply(Transform target)
    {
        // 分裂できない場合は処理しない
        if (!canSplit || target == null)
            return;

        EnemyHealth firstEnemy =
            target.GetComponentInParent<EnemyHealth>();

        if (firstEnemy == null)
            return;

        // 分裂位置を保存する
        Vector3 splitPosition = transform.position;

        // 周囲の敵を検索する
        Collider2D[] colliders =
            Physics2D.OverlapCircleAll(splitPosition, searchRadius);

        List<EnemyHealth> enemies = new List<EnemyHealth>();

        HashSet<EnemyHealth> checkedEnemies =
            new HashSet<EnemyHealth>();

        foreach (Collider2D col in colliders)
        {
            EnemyHealth enemy =
                col.GetComponentInParent<EnemyHealth>();

            // 最初に命中した敵は除外する
            if (enemy == null || enemy == firstEnemy)
                continue;

            // 同じ敵を重複して登録しない
            if (checkedEnemies.Add(enemy))
            {
                enemies.Add(enemy);
            }
        }

        // 距離が近い敵から優先する
        enemies.Sort((a, b) =>
        {
            float distanceA =
                Vector2.Distance(splitPosition, a.transform.position);

            float distanceB =
                Vector2.Distance(splitPosition, b.transform.position);

            return distanceA.CompareTo(distanceB);
        });

        int splitCount =
            Mathf.Min(maxSplitCount, enemies.Count);

        // 敵ごとに小さなバナナを生成する
        for (int i = 0; i < splitCount; i++)
        {
            EnemyHealth enemy = enemies[i];

            if (enemy == null)
                continue;

            // 現在のバナナと同じPrefab構成を複製する
            GameObject miniBanana = Instantiate(
                gameObject,
                splitPosition,
                Quaternion.identity
            );

            ProjectileArc arc =
                miniBanana.GetComponent<ProjectileArc>();

            BananaEffect miniEffect =
                miniBanana.GetComponent<BananaEffect>();

            if (arc == null || miniEffect == null)
            {
                Debug.LogWarning(
                    "Banana Prefabに必要なコンポーネントがありません。"
                );

                Destroy(miniBanana);
                continue;
            }

            // 小さなバナナの再分裂を禁止する
            miniEffect.canSplit = false;

            // 小さなバナナのダメージを設定する
            arc.damage = miniBananaDamage;

            // 敵を追跡して投げる
            arc.InitializeTarget(
                splitPosition,
                enemy.transform,
                miniBananaHeight,
                miniBananaDuration
            );
        }
    }
}
