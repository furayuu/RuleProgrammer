using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public GameObject appleBulletPrefab;
    public Transform firePoint;

    public float bulletSpeed = 10f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        Vector3 mousePos =
            Camera.main.ScreenToWorldPoint(Input.mousePosition);

        mousePos.z = 0;

        Vector2 direction =
            (mousePos - firePoint.position).normalized;

        GameObject bullet =
            Instantiate(
                appleBulletPrefab,
                firePoint.position,
                Quaternion.identity
            );

        Rigidbody2D rb =
            bullet.GetComponent<Rigidbody2D>();

        rb.linearVelocity = direction * bulletSpeed;
    }
}