using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("敵のプレハブ（最大3つ）")]
    public GameObject[] enemyPrefabs = new GameObject[3];

    [Header("生成ウェイト（数値が大きいほど出現率が高くなる）")]
    [Tooltip("例：50, 30, 20 の場合、最初の敵が約50%の確率で出現します")]
    public float[] spawnWeights = new float[3] { 50f, 30f, 20f };

    public Transform spawnPoint;
    public float spawnInterval = 2f;
    [HideInInspector]
    public float roundDuration = 30f;

    private Coroutine spawnCoroutine;
    private float totalWeight;

    private void Awake()
    {
        CalculateTotalWeight();
    }

    // Inspectorで値を変更したときに自動で総ウェイトを再計算する
    private void OnValidate()
    {
        CalculateTotalWeight();
    }

    // 総ウェイトを計算する
    private void CalculateTotalWeight()
    {
        totalWeight = 0f;
        for (int i = 0; i < spawnWeights.Length && i < enemyPrefabs.Length; i++)
        {
            if (enemyPrefabs[i] != null && spawnWeights[i] > 0)
            {
                totalWeight += spawnWeights[i];
            }
        }
    }

    public void StartRound(int round)
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }
        spawnCoroutine = StartCoroutine(SpawnEnemies(round));
    }

    /// <summary>
    /// 重みに基づいてランダムに敵のプレハブを返す
    /// </summary>
    private GameObject GetRandomEnemyPrefab()
    {
        if (totalWeight <= 0 || enemyPrefabs.Length == 0)
        {
            Debug.LogWarning("有効な敵のプレハブが設定されていません！");
            return null;
        }

        float randomValue = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        for (int i = 0; i < enemyPrefabs.Length; i++)
        {
            if (enemyPrefabs[i] != null && spawnWeights[i] > 0)
            {
                cumulative += spawnWeights[i];
                if (randomValue < cumulative)
                {
                    return enemyPrefabs[i];
                }
            }
        }

        // フォールバック：最初に見つかった有効なプレハブを返す
        for (int i = 0; i < enemyPrefabs.Length; i++)
        {
            if (enemyPrefabs[i] != null)
                return enemyPrefabs[i];
        }
        return null;
    }

    IEnumerator SpawnEnemies(int round)
    {
        int enemyCount = round * 5;

        spawnInterval = roundDuration / enemyCount;

        for (int i = 0; i < enemyCount; i++)
        {
            GameObject prefabToSpawn = GetRandomEnemyPrefab();

            if (prefabToSpawn != null && spawnPoint != null)
            {
                Instantiate(prefabToSpawn, spawnPoint.position, Quaternion.identity);
            }
            else
            {
                Debug.LogWarning("敵を生成できません：プレハブまたはスポーンポイントが設定されていません");
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    // 実行時に動的に生成確率を変更したい場合に使用（任意）
    public void SetSpawnWeights(float[] newWeights)
    {
        if (newWeights != null && newWeights.Length == spawnWeights.Length)
        {
            spawnWeights = newWeights;
            CalculateTotalWeight();
        }
    }
}