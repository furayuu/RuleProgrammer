using System.Collections;
using UnityEngine;

public class Rush_Enemy : MonoBehaviour
{
    [Header("Base Target")]
    [Tooltip("基地の Transform。未設定の場合は BaseHealth から自動取得")]
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

    private bool isBusy = false;
    private bool isDead = false;

    void Start()
    {
        enemyMove = GetComponent<EnemyMove>();

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

        // EnemyMove にプレイヤーを追わせず、このスクリプトで移動を制御する
        enemyMove.autoMove = false;

        // Inspector で基地が設定されていなければ BaseHealth から探す
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
        if (isDead || baseTarget == null || isBusy)
            return;

        // 基地との水平方向の距離
        float distance = Mathf.Abs(
            baseTarget.position.x - transform.position.x
        );

        // 接近するまでは通常移動
        if (distance > chargeStartDistance)
        {
            MoveToBase();
        }
        else
        {
            // 基地に近づいたら突進開始
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

        // 蓄力中はいったん停止
        rb.velocity = new Vector2(0f, rb.velocity.y);
        enemyMove.Move(0f);

        yield return new WaitForSeconds(chargeTime);

        if (isDead || baseTarget == null)
            yield break;

        // 蓄力後、基地の方向へ突進
        float direction = Mathf.Sign(
            baseTarget.position.x - transform.position.x
        );

        float timer = 0f;

        while (timer < rushDuration && !isDead)
        {
            if (baseTarget == null)
                break;

            // 突進中も基地の方向を確認
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

        if (isDead)
            yield break;

        // 突進終了後は停止
        rb.velocity = new Vector2(0f, rb.velocity.y);

        enemyMove.canMove = true;

        yield return new WaitForSeconds(cooldown);

        isBusy = false;
    }

    // 敵が基地に接触して EnemyAttack から破棄される前に、
    // 必要に応じて死亡状態を通知できるようにする
    public void MarkDead()
    {
        isDead = true;
        StopAllCoroutines();
    }
}