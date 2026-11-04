
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    [Header("追従対象")]
    public Transform target;

    [Header("マップ範囲を取得する Tilemap")]
    public Tilemap boundaryTilemap;

    [Header("画面端の追従開始位置")]
    [Range(0f, 0.5f)]
    public float leftBoundary = 0.15f;

    [Range(0.5f, 1f)]
    public float rightBoundary = 0.85f;

    [Header("カメラ設定")]
    public float followSpeed = 5f;

    private Camera cam;
    private float fixedY;
    private float fixedZ;

    private float mapLeft;
    private float mapRight;
    private bool validMapBounds;

    void Start()
    {
        cam = GetComponent<Camera>();

        fixedY = transform.position.y;
        fixedZ = transform.position.z;

        // Target が未設定の場合、Player タグから追従対象を取得する
        if (target == null)
        {
            GameObject player =
                GameObject.FindGameObjectWithTag("Player");

            if (player != null)
                target = player.transform;
        }

        RefreshMapBounds();
        ClampCameraPosition();
    }

    // Tilemap の範囲からワールド座標上の左右端を計算する
    public void RefreshMapBounds()
    {
        if (boundaryTilemap == null)
        {
            validMapBounds = false;
            Debug.LogWarning(
                "CameraFollow: Boundary Tilemap を設定してください！");
            return;
        }

        Bounds bounds = boundaryTilemap.localBounds;

        Vector3 min = bounds.min;
        Vector3 max = bounds.max;

        // Tilemap のローカル座標をワールド座標に変換する
        Vector3[] corners =
        {
            boundaryTilemap.transform.TransformPoint(
                new Vector3(min.x, min.y, 0f)),

            boundaryTilemap.transform.TransformPoint(
                new Vector3(min.x, max.y, 0f)),

            boundaryTilemap.transform.TransformPoint(
                new Vector3(max.x, min.y, 0f)),

            boundaryTilemap.transform.TransformPoint(
                new Vector3(max.x, max.y, 0f))
        };

        mapLeft = corners[0].x;
        mapRight = corners[0].x;

        foreach (Vector3 corner in corners)
        {
            mapLeft = Mathf.Min(mapLeft, corner.x);
            mapRight = Mathf.Max(mapRight, corner.x);
        }

        validMapBounds = mapRight > mapLeft;

        if (!validMapBounds)
        {
            Debug.LogWarning(
                "CameraFollow: Tilemap の範囲が無効です。Tile が配置されているか確認してください。");
        }
    }

    void LateUpdate()
    {
        if (target == null || cam == null)
            return;

        // プレイヤーの画面上の横位置を取得する（左端が 0、右端が 1）
        float screenX =
            cam.WorldToViewportPoint(target.position).x;

        float moveX = 0f;

        // カメラに映るワールド座標上の横幅を計算する
        float worldWidth =
            cam.orthographicSize * 2f * cam.aspect;

        // プレイヤーが画面右端の追従開始位置を超えた場合
        if (screenX > rightBoundary)
        {
            moveX = (screenX - rightBoundary) * worldWidth;
        }
        // プレイヤーが画面左端の追従開始位置を超えた場合
        else if (screenX < leftBoundary)
        {
            moveX = (screenX - leftBoundary) * worldWidth;
        }

        Vector3 pos = transform.position;

        // 追従が必要な場合のみカメラを移動させる
        if (Mathf.Abs(moveX) > 0.001f)
        {
            float smoothMove = Mathf.Lerp(
                0f,
                moveX,
                Mathf.Clamp01(followSpeed * Time.deltaTime)
            );

            pos.x += smoothMove;
        }

        pos.y = fixedY;
        pos.z = fixedZ;

        transform.position = pos;

        // 毎フレーム、カメラがマップの範囲外に出ないように制限する
        ClampCameraPosition();
    }

    void ClampCameraPosition()
    {
        if (cam == null || !cam.orthographic || !validMapBounds)
            return;

        // カメラの表示範囲の半分を計算する
        float halfCameraWidth =
            cam.orthographicSize * cam.aspect;

        float minCameraX = mapLeft + halfCameraWidth;
        float maxCameraX = mapRight - halfCameraWidth;

        Vector3 pos = transform.position;

        // マップの幅がカメラの表示幅より狭い場合、カメラを中央に配置する
        if (minCameraX > maxCameraX)
        {
            pos.x = (mapLeft + mapRight) * 0.5f;
        }
        else
        {
            // カメラの中心がマップの左右端を超えないように制限する
            pos.x = Mathf.Clamp(
                pos.x,
                minCameraX,
                maxCameraX
            );
        }

        pos.y = fixedY;
        pos.z = fixedZ;

        transform.position = pos;
    }
}
