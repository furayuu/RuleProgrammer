using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public PowerState currentState = PowerState.Vulnerable;

    private Animator anim;
    private Rigidbody2D rb;

    private bool isDead = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // 测试用：按 E 切换状态
        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleState();
        }
    }

    void ToggleState()
    {
        if (currentState == PowerState.Vulnerable)
        {
            currentState = PowerState.Dominant;
        }
        else
        {
            currentState = PowerState.Vulnerable;
        }

        Debug.Log("Current State: " + currentState);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();

            if (currentState == PowerState.Vulnerable)
            {
                Die();
            }
            else
            {
                enemy.Die();
            }
        }
    }

    public void Die()
    {
        isDead = true;

        // 播放死亡动画
        anim.SetTrigger("Die");

        // 停止移动
        rb.velocity = Vector2.zero;

        // 禁止继续移动
        GetComponent<PlayerMove>().enabled = false;

        // 关闭碰撞
        GetComponent<Collider2D>().enabled = false;

        // 延迟销毁
        Destroy(gameObject, 1f);
    }
}

public enum PowerState
{
    Vulnerable,
    Dominant
}