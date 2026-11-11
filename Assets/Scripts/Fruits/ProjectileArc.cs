
using UnityEngine;

public class ProjectileArc : MonoBehaviour
{
    [Header("弾の設定")]
    public int damage = 1;

    private Vector3 startPos;
    private Vector3 targetPos;

    // Allyが攻撃する対象
    private Transform target;

    private float throwHeight;
    private float throwDuration;
    private float timer;

    private bool initialized = false;
    private bool isTargetLocked = false;
    private bool hasHit = false;

    private FruitEffectBase fruitEffect;

    void Awake()
    {
        // このオブジェクトに設定された特殊効果を取得する
        fruitEffect = GetComponent<FruitEffectBase>();
    }

    // プレイヤー用：指定した位置に投げる
    public void Initialize(
        Vector3 start,
        Vector3 target,
        float height,
        float duration)
    {
        startPos = start;
        targetPos = target;

        this.target = null;

        throwHeight = height;
        throwDuration = Mathf.Max(duration, 0.01f);

        timer = 0f;

        isTargetLocked = false;
        hasHit = false;
        initialized = true;
    }

    // Ally用：敵をロックオンして投げる
    public void InitializeTarget(
        Vector3 start,
        Transform target,
        float height,
        float duration)
    {
        if (target == null)
        {
            initialized = false;
            return;
        }

        startPos = start;

        this.target = target;
        targetPos = target.position;

        throwHeight = height;
        throwDuration = Mathf.Max(duration, 0.01f);

        timer = 0f;

        isTargetLocked = true;
        hasHit = false;
        initialized = true;
    }

    void Update()
    {
        if (!initialized)
            return;

        timer += Time.deltaTime;

        float t = timer / throwDuration;

        // ロックオンした敵の位置を更新する
        if (isTargetLocked && target != null)
        {
            targetPos = target.position;
        }

        // 投げ終わった場合
        if (t >= 1f)
        {
            transform.position = targetPos;

            // Allyのロックオン攻撃の場合
            if (isTargetLocked && target != null)
            {
                HitEnemy(target);
            }

            Destroy(gameObject);
            return;
        }

        // 放物線の位置を計算する
        Vector3 pos = Vector3.Lerp(
            startPos,
            targetPos,
            t
        );

        pos.y += Mathf.Sin(t * Mathf.PI) * throwHeight;

        transform.position = pos;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // ロックオン攻撃は到着時に命中判定を行う
        if (isTargetLocked)
            return;

        // すでに命中している場合は処理しない
        if (hasHit)
            return;

        // 敵に命中した場合
        if (other.CompareTag("Enemy"))
        {
            HitEnemy(other.transform);
            Destroy(gameObject);
        }
    }

    // 敵にダメージと特殊効果を与える
    private void HitEnemy(Transform hitTarget)
    {
        if (hasHit || hitTarget == null)
            return;

        EnemyHealth enemy =
            hitTarget.GetComponentInParent<EnemyHealth>();

        if (enemy == null)
            return;

        hasHit = true;

        // 基本ダメージを与える
        enemy.TakeDamage(damage);

        // フルーツ固有の特殊効果を実行する
        if (fruitEffect != null)
        {
            fruitEffect.Apply(enemy.transform);
        }
    }
}
