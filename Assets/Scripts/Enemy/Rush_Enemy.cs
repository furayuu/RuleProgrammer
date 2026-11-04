
using System.Collections;
using UnityEngine;

public class Rush_Enemy : MonoBehaviour
{
    [Header("Base Target")]
    [Tooltip("基地のTransform。未設定の場合はBaseHealthから自動取得")]
    public Transform baseTarget;

    [Header("Move Settings")]
    [Tooltip("基地に向かって移動する速度")]
    public float moveSpeed = 2f;

    [Header("Rush Settings")]
    [Tooltip("この距離まで基地に近づくと突進する")]
    public float chargeStartDistance = 4f;

    [Tooltip("突進前のため時間")]
    public float chargeTime = 0.5f;

    [Tooltip("突進速度")]
    public float rushSpeed = 6f;

    [Tooltip("突進を続ける最大時間")]
    public float rushDuration = 1.5f;

    [Tooltip("次の突進までの待ち時間")]
    public float cooldown = 1f;

    private EnemyMove enemyMove;
    private Rigidbody2D rb;
    private EnemyHealth enemyHealth;

    private bool isBusy = false;
    private bool isDead = false;

    void Start()
    {
        enemyMove = GetComponent<EnemyMove>();
        enemyHealth = GetComponent<EnemyHealth>();

        if (enemyMove == null)
        {
            Debug.LogError("Rush_Enemy requires EnemyMove!");
            enabled = false;
            return;
        }

        rb = enemyMove.GetRigidBody();

        if (rb == null)
        {
            Debug.LogError("Rush_Enemy requires Rigidbody2D!");
            enabled = false;
            return;
        }

        // EnemyMoveによるプレイヤー追跡を無効にする
        enemyMove.autoMove = false;

        // Inspectorで基地が設定されていない場合は自動取得する
        if (baseTarget == null)
        {
            BaseHealth baseHealth = FindObjectOfType<BaseHealth>();

            if (baseHealth != null)
            {
                baseTarget = baseHealth.transform;
            }
        }

        if (baseTarget == null)
        {
            Debug.LogError(
                "Rush_Enemy: Base target not found. " +
                "Assign the base Transform in the Inspector."
            );
        }
    }

    void Update()
    {
        // 死亡した敵は移動しない
        if (isDead || (enemyHealth != null && enemyHealth.IsDead)
            || baseTarget == null || isBusy)
            return;

        // 基地との水平方向の距離を計算する
        float distance = Mathf.Abs(
            baseTarget.position.x - transform.position.x
        );

        // 基地に近づくまでは通常移動する
        if (distance > chargeStartDistance)
        {
            MoveToBase();
        }
        else
        {
            // 基地に近づいたら突進を開始する
            StartCoroutine(RushRoutine());
        }
    }

    void MoveToBase()
    {
        float direction = Mathf.Sign(
            baseTarget.position.x - transform.position.x
        );

        enemyMove.Move(direction);
    }

    IEnumerator RushRoutine()
    {
        isBusy = true;
        enemyMove.canMove = false;

        // 突進前に停止する
        rb.velocity = new Vector2(0f, rb.velocity.y);
        enemyMove.Move(0f);

        yield return new WaitForSeconds(chargeTime);

        if (isDead || (enemyHealth != null && enemyHealth.IsDead)
            || baseTarget == null)
            yield break;

        // 蓄力後、基地に向かって突進する
        float direction = Mathf.Sign(
            baseTarget.position.x - transform.position.x
        );

        float timer = 0f;

        while (timer < rushDuration && !isDead
            && (enemyHealth == null || !enemyHealth.IsDead))
        {
            if (baseTarget == null)
                break;

            // 突進中も基地の方向を確認する
            direction = Mathf.Sign(
                baseTarget.position.x - transform.position.x
            );

            rb.velocity = new Vector2(
                direction * rushSpeed,
                rb.velocity.y
            );

            timer += Time.deltaTime;
            yield return null;
        }

        if (isDead || (enemyHealth != null && enemyHealth.IsDead))
            yield break;

        // 突進終了後に停止する
        rb.velocity = new Vector2(0f, rb.velocity.y);

        enemyMove.canMove = true;

        yield return new WaitForSeconds(cooldown);

        if (isDead || (enemyHealth != null && enemyHealth.IsDead))
            yield break;

        isBusy = false;
    }

    // 敵の死亡時に呼び出す
    public void MarkDead()
    {
        isDead = true;

        // 実行中のコルーチンを停止する
        StopAllCoroutines();

        // 物理的な移動を停止する
        if (rb != null)
            rb.velocity = Vector2.zero;
    }
}
