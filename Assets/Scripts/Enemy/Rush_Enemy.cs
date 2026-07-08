using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rush_Enemy : MonoBehaviour
{
    Transform playerTr;
    [SerializeField] float speed = 2; 
    private Animator animator;

    private void Start()
    {
        playerTr = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
    }

    private void Update()
    {

        if (Vector2.Distance(transform.position, playerTr.position) < 0.1f)
            return;

        if (Vector2.Distance(transform.position, playerTr.position) < 3.0f)
        {
            speed = 4.0f;
        }

            transform.position = Vector2.MoveTowards(
            transform.position,
            new Vector2(playerTr.position.x, playerTr.position.y),
            speed * Time.deltaTime);

        animator.SetBool("IsRunning", true);
    }
}
