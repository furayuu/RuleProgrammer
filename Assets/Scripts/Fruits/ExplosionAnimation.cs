
using UnityEngine;

public class ExplosionAnimation : MonoBehaviour
{
    [Header("回転設定")]
    public float rotationAngle = 25f;
    public float rotationSpeed = 720f;

    [Header("サイズ設定")]
    public float startScale = 0.8f;
    public float endScale = 1.2f;

    [Header("アニメーション設定")]
    public float duration = 0.5f;

    private SpriteRenderer spriteRenderer;
    private float timer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        timer += Time.deltaTime;

        float t = Mathf.Clamp01(timer / duration);

        // 左右に回転させる
        float angle =
            Mathf.Sin(timer * rotationSpeed * Mathf.Deg2Rad)
            * rotationAngle;

        transform.rotation =
            Quaternion.Euler(0f, 0f, angle);

        // 徐々に拡大する
        float scale =
            Mathf.Lerp(startScale, endScale, t);

        transform.localScale =
            Vector3.one * scale;

        // 徐々に透明にする
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = 1f - t;
            spriteRenderer.color = color;
        }

        // アニメーション終了時に削除する
        if (timer >= duration)
        {
            Destroy(gameObject);
        }
    }
}
