
using System.Collections.Generic;
using UnityEngine;

public class CherryEffect : FruitEffectBase
{
    [Header("爆発設定")]
    public float explosionRadius = 3f;
    public int explosionDamage = 2;
    public float damageMultiplier = 1f;
    public GameObject explosionEffect;

    // 爆発が重複して発生しないようにする
    private bool hasExploded = false;

    public override void Apply(Transform target)
    {
        // すでに爆発している場合は処理しない
        if (hasExploded || target == null)
            return;

        // 爆発処理を実行する
        Explode();
    }

    // 爆発処理
    private void Explode()
    {
        hasExploded = true;

        // 爆発エフェクトを生成する
        if (explosionEffect != null)
        {
            Instantiate(
                explosionEffect,
                transform.position,
                Quaternion.identity
            );
        }

        // 爆発範囲内の敵を検索する
        Collider2D[] colliders =
            Physics2D.OverlapCircleAll(
                transform.position,
                explosionRadius
            );

        // 同じ敵に重複してダメージを与えない
        HashSet<EnemyHealth> checkedEnemies =
            new HashSet<EnemyHealth>();

        // プレイヤーの攻撃倍率を適用する
        int finalDamage =
            Mathf.RoundToInt(
                explosionDamage * damageMultiplier
            );

        foreach (Collider2D col in colliders)
        {
            EnemyHealth enemy =
                col.GetComponentInParent<EnemyHealth>();

            if (enemy == null)
                continue;

            // 同じ敵は一度だけ処理する
            if (!checkedEnemies.Add(enemy))
                continue;

            // 爆発ダメージを与える
            enemy.TakeDamage(finalDamage);
        }

        Debug.Log("Cherry Explosion!");
    }

    // 爆発範囲をSceneビューに表示する
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

    // 爆発範囲を強化する
    public void UpgradeExplosionRadius(float amount)
    {
        explosionRadius += amount;
    }
}
