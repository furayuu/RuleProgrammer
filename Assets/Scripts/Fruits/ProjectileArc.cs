
using UnityEngine;

public class ProjectileArc : MonoBehaviour
{
    [Header("弾の設定")]
    public int damage = 1;

    private Vector3 startPos;
    private Vector3 targetPos;
    private Transform target;

    private float throwHeight;
    private float throwDuration;
    private float timer;

    private bool initialized = false;
    private bool isTargetLocked = false;
    private bool followTarget = false;
    private bool hasHit = false;

    private FruitEffectBase fruitEffect;

    void Awake()
    {
        // 特殊効果を取得する
        fruitEffect = GetComponent<FruitEffectBase>();
    }

    // 指定した位置に投げる
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
        followTarget = false;
        hasHit = false;
        initialized = true;
    }

    // 対象を追跡して投げる
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
        followTarget = true;
        hasHit = false;
        initialized = true;
    }

    void Update()
    {
        if (!initialized)
            return;

        timer += Time.deltaTime;

        float t = timer / throwDuration;

        // 追跡対象の位置を更新する
        if (followTarget && target != null)
        {
            targetPos = target.position;
        }

        // 投げ終わった場合
        if (t >= 1f)
        {
            transform.position = targetPos;

            if (isTargetLocked && target != null)
            {
                HitTarget(target);
            }
            else if (fruitEffect != null)
            {
                // 地面に着地したときの特殊効果を実行する
                fruitEffect.OnGroundHit();
            }

            Destroy(gameObject);
            return;
        }

        // 放物線の位置を計算する
        Vector3 pos = Vector3.Lerp(startPos, targetPos, t);

        pos.y += Mathf.Sin(t * Mathf.PI) * throwHeight;

        transform.position = pos;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 対象指定型の攻撃は到着時に命中判定を行う
        if (isTargetLocked || hasHit)
            return;

        if (other.CompareTag("Enemy"))
        {
            // パイナップルは敵に当たっても着地するまで飛び続ける
            if (fruitEffect is PineappleEffect)
                return;

            HitTarget(other.transform);
            Destroy(gameObject);
        }
    }

    // 敵または味方に命中した場合の処理
    private void HitTarget(Transform hitTarget)
    {
        if (hasHit || hitTarget == null)
            return;

        EnemyHealth enemy =
            hitTarget.GetComponentInParent<EnemyHealth>();

        AllyHealth ally =
            hitTarget.GetComponentInParent<AllyHealth>();

        // 敵でも味方でもない場合は処理しない
        if (enemy == null && ally == null)
            return;

        // 特殊効果で命中可能か確認する
        if (
            fruitEffect != null &&
            !fruitEffect.CanHitTarget(
                enemy != null ? enemy.transform : ally.transform
            )
        )
            return;

        hasHit = true;

        // 敵に命中した場合はダメージを与える
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }

        // 特殊効果を実行する
        if (fruitEffect != null)
        {
            Transform effectTarget =
                enemy != null ? enemy.transform : ally.transform;

            fruitEffect.Apply(effectTarget);
        }
    }
}
