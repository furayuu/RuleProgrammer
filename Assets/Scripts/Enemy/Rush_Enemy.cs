using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rush_Enemy : MonoBehaviour
{
    Transform playerTr; // プレイヤーのTransform
    [SerializeField] float speed = 2; // 敵の動くスピード
    private Animator animator;

    private void Start()
    {
        // プレイヤーのTransformを取得（プレイヤーのタグをPlayerに設定必要）
        playerTr = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
    }

    private void Update()
    {

        // プレイヤーとの距離が0.1f未満になったらそれ以上実行しない
        if (Vector2.Distance(transform.position, playerTr.position) < 0.1f)
            return;

        if (Vector2.Distance(transform.position, playerTr.position) < 3.0f)
        {
            speed = 4.0f;
        }

            // プレイヤーに向けて進む
            transform.position = Vector2.MoveTowards(
            transform.position,
            new Vector2(playerTr.position.x, playerTr.position.y),
            speed * Time.deltaTime);

        animator.SetBool("IsRunning", true);
    }
}
