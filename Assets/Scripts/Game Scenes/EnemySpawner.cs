using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;

    public Transform spawnPoint;

    public float spawnInterval = 2f;

    private Coroutine spawnCoroutine;

    public void StartRound(int round)
    {
        // 前ラウンドの生成停止
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }

        spawnCoroutine = StartCoroutine(SpawnEnemies(round));
    }

    IEnumerator SpawnEnemies(int round)
    {
        // ラウンド×5体生成
        int enemyCount = round * 5;

        for (int i = 0; i < enemyCount; i++)
        {
            Instantiate(enemyPrefab,
                        spawnPoint.position,
                        Quaternion.identity);

            yield return new WaitForSeconds(spawnInterval);
        }
    }
}