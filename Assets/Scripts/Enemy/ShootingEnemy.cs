using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ShootingEnemy : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float changeDirectionTime = 2f;
    public float moveTime = 2f;
    public float stopTime = 1f;

    private float timer;
    private bool isMoving = true;
    private float moveDirection; // -1:ç∂ 1:âE 0:í‚é~

    public float shotInterval = 1f;
    private float shotTimer;
    [SerializeField] private float bulletSpeed = 2f;

    private Rigidbody2D rb;
    private Animator animator;

    public GameObject bulletPrefab;
    public Transform firePoint;

    EnemyMove enemymove;

    public enum ShootType
    {
        StopOnly,      // é~Ç‹Ç¡ÇƒÇ¢ÇÈéûÇæÇØåÇÇ¬
        Always         // èÌÇ…åÇÇ¬
    }

    public ShootType shootType;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //animator = GetComponent<Animator>();

        moveDirection = Random.Range(0, 2) == 0 ? -1f : 1f;
    }

    void Update()
    {
        RandomMove();

        ShotControl();
    }

    void RandomMove()
    {
        timer += Time.deltaTime;

        if (isMoving)
        {
            Move(moveDirection);

            if (timer >= moveTime)
            {
                timer = 0;
                isMoving = false;
            }
        }
        else
        {
            Move(0);

            if (timer >= stopTime)
            {
                timer = 0;
                isMoving = true;

                moveDirection = Random.Range(0, 2) == 0 ? -1f : 1f;
            }
        }
    }

    void Move(float direction)
    {
        rb.velocity = new Vector2(direction * moveSpeed, rb.velocity.y);

        if (direction != 0)
        {
            transform.localScale = new Vector3(
                direction > 0 ? 1 : -1,
                1,
                1
            );
        }
    }

    void FaceTarget(Transform target)
    {
        float direction = Mathf.Sign(target.position.x - transform.position.x);

        transform.localScale = new Vector3(
            direction > 0 ? 1 : -1,
            1,
            1
        );
    }

    void ShotControl()
    {
        shotTimer += Time.deltaTime;

        Transform target = GetNearestTarget();
        
        switch (shootType)
        {
            // ìÆÇ´Ç»Ç™ÇÁåÇÇ¬
            case ShootType.Always:

                if (shotTimer >= shotInterval)
                {
                    shotTimer = 0;
                    Shoot();
                }

                break;
            // é~Ç‹Ç¡ÇƒåÇÇ¬
            case ShootType.StopOnly:

                if (!isMoving && shotTimer >= shotInterval)
                {
                    shotTimer = 0;
                    FaceTarget(target);
                    Shoot();
                }

                break;
        }
    }

    void Shoot()
    {
        Transform target = GetNearestTarget();

        if (target == null) return;

        GameObject bulletObj = Instantiate(
            bulletPrefab,
            firePoint.position,
            Quaternion.identity);

        Vector2 dir = (target.position - firePoint.position).normalized;

        bulletObj.GetComponent<EnemyBullet>().SetDirection(dir);
    }

    Transform GetNearestTarget()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject[] allies = GameObject.FindGameObjectsWithTag("Ally");

        Transform nearest = null;
        float minDistance = Mathf.Infinity;

        // PlayerÇåüçı
        foreach (GameObject obj in players)
        {
            float distance = Vector2.Distance(transform.position, obj.transform.position);

            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = obj.transform;
            }
        }

        // AllyÇåüçı
        foreach (GameObject obj in allies)
        {
            float distance = Vector2.Distance(transform.position, obj.transform.position);

            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = obj.transform;
            }
        }

        return nearest;
    }
}
