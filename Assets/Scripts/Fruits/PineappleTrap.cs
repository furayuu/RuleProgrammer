
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PineappleTrap : MonoBehaviour
{
    [Header("Trap Settings")]
    public float activationDelay = 0.5f;
    public float lifeTime = 10f;

    [Header("Explosion Settings")]
    public float explosionRadius = 3f;
    public int explosionDamage = 5;
    public float explosionHeight = 1.5f;
    public GameObject explosionEffect;

    private Collider2D[] trapColliders;
    private SpriteRenderer[] spriteRenderers;

    private bool isActive = false;
    private bool hasExploded = false;

    void Awake()
    {
        trapColliders = GetComponentsInChildren<Collider2D>();
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    }

    IEnumerator Start()
    {
        // トラップを一時的に無効にする
        SetTrapVisible(false);
        SetTrapCollider(false);

        // 設置後の起動待ち時間
        yield return new WaitForSeconds(activationDelay);

        SetTrapVisible(true);
        SetTrapCollider(true);

        isActive = true;

        // 制限時間後にトラップを破棄する
        yield return new WaitForSeconds(lifeTime);

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActive || hasExploded)
            return;

        if (!other.CompareTag("Enemy"))
            return;

        Explode();
    }

    // トラップを爆発させる
    private void Explode()
    {
        if (hasExploded)
            return;

        hasExploded = true;
        Vector3 explosionPosition = transform.position + Vector3.up * explosionHeight;

        // 爆発エフェクトを生成する
        if (explosionEffect != null)
        {
            
            Instantiate(
                explosionEffect,
                explosionPosition,
                Quaternion.identity
            );
        }

        // 爆発範囲内の敵を検索する
        Collider2D[] colliders =
         Physics2D.OverlapCircleAll(
                   explosionPosition,
                   explosionRadius
             );

        HashSet<EnemyHealth> damagedEnemies =
            new HashSet<EnemyHealth>();

        foreach (Collider2D col in colliders)
        {
            EnemyHealth enemy =
                col.GetComponentInParent<EnemyHealth>();

            if (enemy == null || enemy.IsDead)
                continue;

            // 同じ敵への重複ダメージを防ぐ
            if (!damagedEnemies.Add(enemy))
                continue;

            enemy.TakeDamage(explosionDamage);
        }

        Destroy(gameObject);
    }

    private void SetTrapVisible(bool visible)
    {
        foreach (SpriteRenderer sr in spriteRenderers)
        {
            sr.enabled = visible;
        }
    }

    private void SetTrapCollider(bool enabled)
    {
        foreach (Collider2D col in trapColliders)
        {
            col.enabled = enabled;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(
            transform.position,
            explosionRadius
        );
    }
}
