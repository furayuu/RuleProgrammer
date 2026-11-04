
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHP = 3;

    [Header("Reward")]
    public int moneyReward = 5;

    private int currentHP;
    private bool isDead = false;

    private Animator animator;

    public int CurrentHP => currentHP;
    public float HealthPercent => (float)currentHP / maxHP;

    // 他のスクリプトから敵の死亡状態を確認する
    public bool IsDead => isDead;

    void Start()
    {
        currentHP = maxHP;
        animator = GetComponent<Animator>();
    }

    // プレイヤーから攻撃を受けたときに呼び出す
    public void TakeDamage(int damage)
    {
        if (isDead || damage <= 0)
            return;

        currentHP -= damage;
        currentHP = Mathf.Max(currentHP, 0);

        if (animator != null)
            animator.SetTrigger("Hit");

        if (currentHP <= 0)
        {
            Die(true);
        }
    }

    public void Heal(int amount)
    {
        if (isDead || amount <= 0)
            return;

        currentHP += amount;
        currentHP = Mathf.Min(currentHP, maxHP);
    }

    // 敵の死亡処理
    // giveReward = true：報酬を獲得する
    // giveReward = false：報酬を獲得しない
    public void Die(bool giveReward)
    {
        if (isDead)
            return;

        isDead = true;

        // Rush_Enemy の突進を停止する
        Rush_Enemy rushEnemy = GetComponent<Rush_Enemy>();

        if (rushEnemy != null)
            rushEnemy.MarkDead();
        
        // 通常の移動を停止する
        EnemyMove enemyMove = GetComponent<EnemyMove>();

        if (enemyMove != null)
        {
            enemyMove.canMove = false;
            enemyMove.Move(0f);
        }

        // 物理的な移動を停止する
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        if (rb != null)
            rb.velocity = Vector2.zero;
        
        // 死亡アニメーションを再生する
        if (animator != null)
            animator.SetTrigger("Hit");

        // 報酬を付与する
        if (giveReward && GameManager.Instance != null)
            GameManager.Instance.AddMoney(moneyReward);

        // アニメーション再生後に敵を削除する
        Destroy(gameObject, 0.5f);
    }
}
