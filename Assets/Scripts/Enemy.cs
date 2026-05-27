using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Animator anim;

    private bool isDead = false;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;

        anim.SetTrigger("Die");

        GetComponent<Collider2D>().enabled = false;

        Destroy(gameObject, 1f);
    }
}