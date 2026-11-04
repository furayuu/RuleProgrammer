using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("敵の設定")]
    public GameObject[] enemyPrefabs = new GameObject[3];

    [Header("敵の出現確率")]
    public float[] spawnWeights = new float[3] { 50f, 30f, 20f };

    [Header("スポーン地点")]
    public Transform spawnPoint;

    [Header("ウェーブごとの敵の数")]
    public int baseEnemyCount = 5;
    public int extraEnemiesPerRound = 5;

    [Header("敵の生成間隔")]
    public float spawnInterval = 2f;
    public float intervalReductionPerRound = 0.15f;
    public float minSpawnInterval = 0.5f;

    private Coroutine spawnCoroutine;
    private float totalWeight;

    // 生成した敵のリストを管理する
    private readonly List<GameObject> spawnedEnemies = new List<GameObject>();

    // 敵を生成中かどうかを取得する
    public bool IsSpawning { get; private set; }

    // 現在生存している敵の数を取得する
    public int AliveEnemyCount
    {
        get
        {
            // 破棄された敵をリストから削除する
            spawnedEnemies.RemoveAll(enemy => enemy == null);
            return spawnedEnemies.Count;
        }
    }

    void Awake()
    {
        // 敵の出現確率の合計を計算する
        CalculateTotalWeight();
    }

    void OnValidate()
    {
        // Inspectorで設定を変更したときに確率を再計算する
        CalculateTotalWeight();

        // 設定値が不正な値にならないように調整する
        baseEnemyCount = Mathf.Max(1, baseEnemyCount);
        extraEnemiesPerRound = Mathf.Max(0, extraEnemiesPerRound);
        spawnInterval = Mathf.Max(0.1f, spawnInterval);
        intervalReductionPerRound = Mathf.Max(0f, intervalReductionPerRound);
        minSpawnInterval = Mathf.Max(0.1f, minSpawnInterval);
    }

    void CalculateTotalWeight()
    {
        // 有効な敵Prefabの出現確率を合計する
        totalWeight = 0f;

        if (enemyPrefabs == null || spawnWeights == null)
            return;

        int count = Mathf.Min(enemyPrefabs.Length, spawnWeights.Length);

        for (int i = 0; i < count; i++)
        {
            if (enemyPrefabs[i] != null && spawnWeights[i] > 0f)
            {
                totalWeight += spawnWeights[i];
            }
        }
    }

    public void StartRound(int round)
    {
        // 以前の生成処理が残っていれば停止する
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }

        // 破棄された敵をリストから削除する
        spawnedEnemies.RemoveAll(enemy => enemy == null);

        // 前のウェーブの敵が残っていないか確認する
        if (spawnedEnemies.Count > 0)
        {
            Debug.LogWarning("前のウェーブの敵がまだ残っています。");
            return;
        }

        // スポーン地点が設定されているか確認する
        if (spawnPoint == null)
        {
            Debug.LogError("EnemySpawnerのSpawn Pointが設定されていません。");
            return;
        }

        // ウェーブ番号に応じて敵の数を計算する
        int enemyCount = baseEnemyCount + (round - 1) * extraEnemiesPerRound;

        // ウェーブ番号に応じて敵の生成間隔を短くする
        float interval = Mathf.Max(
            minSpawnInterval,
            spawnInterval - (round - 1) * intervalReductionPerRound
        );

        // 敵の生成処理を開始する
        spawnCoroutine = StartCoroutine(SpawnEnemies(enemyCount, interval));
    }

    IEnumerator SpawnEnemies(int enemyCount, float interval)
    {
        // 敵の生成中フラグを有効にする
        IsSpawning = true;

        for (int i = 0; i < enemyCount; i++)
        {
            // 出現させる敵Prefabをランダムに選択する
            GameObject prefab = GetRandomEnemyPrefab();

            if (prefab != null)
            {
                // スポーン地点に敵を生成してリストに登録する
                GameObject enemy = Instantiate(
                    prefab,
                    spawnPoint.position,
                    Quaternion.identity
                );

                spawnedEnemies.Add(enemy);
            }
            else
            {
                Debug.LogWarning("生成可能な敵Prefabが設定されていません。");
            }

            // 次の敵を生成するまで待機する
            if (i < enemyCount - 1)
            {
                yield return new WaitForSeconds(interval);
            }
        }

        // 敵の生成が完了したことを通知する
        IsSpawning = false;
        spawnCoroutine = null;
    }

    GameObject GetRandomEnemyPrefab()
    {
        // 有効なPrefabと出現確率が存在するか確認する
        if (enemyPrefabs == null || spawnWeights == null || totalWeight <= 0f)
            return null;

        // 出現確率に基づいて敵を選択する
        float randomValue = Random.Range(0f, totalWeight);
        float cumulativeWeight = 0f;

        int count = Mathf.Min(enemyPrefabs.Length, spawnWeights.Length);

        for (int i = 0; i < count; i++)
        {
            // 未設定のPrefabや無効な確率は無視する
            if (enemyPrefabs[i] == null || spawnWeights[i] <= 0f)
                continue;

            cumulativeWeight += spawnWeights[i];

            if (randomValue < cumulativeWeight)
            {
                return enemyPrefabs[i];
            }
        }

        // 誤差などで選択できなかった場合は、最後の有効なPrefabを返す
        for (int i = count - 1; i >= 0; i--)
        {
            if (enemyPrefabs[i] != null && spawnWeights[i] > 0f)
            {
                return enemyPrefabs[i];
            }
        }

        return null;
    }

    public void SetSpawnWeights(float[] newWeights)
    {
        // 外部から出現確率を変更し、合計値を再計算する
        spawnWeights = newWeights;
        CalculateTotalWeight();
    }
}