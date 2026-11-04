using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("ウェーブ設定")]
    public int maxRound = 5;
    public float firstWaveCountdown = 10f;
    public float intermissionDuration = 10f;
    public int currentRound = 1;

    [Header("入力設定")]
    public KeyCode startWaveKey = KeyCode.E;

    [Header("ゴールド設定")]
    public int gold = 100;

    [Header("UI設定")]
    public TMP_Text timerText;
    public TMP_Text roundText;
    public TMP_Text goldText;

    [Header("参照設定")]
    public EnemySpawner enemySpawner;

    private float timer;
    private bool isIntermission = true;
    private bool isWaveActive = false;
    private bool gameFinished = false;

    public bool IsGameFinished => gameFinished;

    void Awake()
    {
        // GameManagerのシングルトンを設定する
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        // EnemySpawnerが設定されているか確認する
        if (enemySpawner == null)
        {
            Debug.LogError("EnemySpawnerが設定されていません。");
            enabled = false;
            return;
        }

        // ゲーム開始時の状態を初期化する
        currentRound = 1;
        timer = firstWaveCountdown;
        isIntermission = true;
        isWaveActive = false;
        gameFinished = false;

        // UIを更新する
        UpdateRoundUI();
        UpdateTimerUI();
        UpdateMoneyUI();
    }

    void Update()
    {
        // ゲーム終了後は処理しない
        if (gameFinished)
            return;

        // ウェーブ間の準備時間を処理する
        if (isIntermission)
        {
            // Eキーが押されたらカウントダウンをスキップする
            if (Input.GetKeyDown(startWaveKey))
            {
                StartCurrentWave();
                return;
            }

            // カウントダウンを更新する
            timer -= Time.deltaTime;

            if (timer <= 0f)
            {
                timer = 0f;
                UpdateTimerUI();

                // カウントダウン終了後、自動的にウェーブを開始する
                StartCurrentWave();
            }
            else
            {
                UpdateTimerUI();
            }

            return;
        }

        // 敵の生成が終了し、残っている敵もいなければウェーブ終了
        if (isWaveActive &&
            !enemySpawner.IsSpawning &&
            enemySpawner.AliveEnemyCount == 0)
        {
            OnWaveCleared();
        }
    }
    void StartCurrentWave()
    {
        // ゲーム終了中、または準備時間以外は開始しない
        if (gameFinished || isWaveActive || !isIntermission)
            return;

        // 最大ウェーブ数を超えていたらステージを終了する
        if (currentRound > maxRound)
        {
            FinishStage();
            return;
        }

        // 準備状態を終了し、ウェーブを開始する
        isIntermission = false;
        isWaveActive = true;

        // UIを更新する
        UpdateRoundUI();

        if (timerText != null)
            timerText.text = "Wave in progress";

        Debug.Log("Wave " + currentRound + " を開始しました。");

        // 敵の生成を開始する
        enemySpawner.StartRound(currentRound);
    }

    public void StartWaveEarly()
    {
        // 外部からウェーブを早く開始したい場合に使用する
        if (gameFinished || !isIntermission || isWaveActive)
            return;

        StartCurrentWave();
    }

    void OnWaveCleared()
    {
        // 現在のウェーブを終了する
        isWaveActive = false;

        Debug.Log("Wave " + currentRound + " をクリアしました。");

        // 最終ウェーブならステージを終了する
        if (currentRound >= maxRound)
        {
            FinishStage();
        }
        else
        {
            // 次のウェーブに進み、準備時間を設定する
            currentRound++;
            isIntermission = true;
            timer = intermissionDuration;

            // UIを更新する
            UpdateRoundUI();
            UpdateTimerUI();
        }
    }

    void FinishStage()
    {
        // ステージクリア状態にする
        gameFinished = true;
        isWaveActive = false;
        isIntermission = false;

        // クリア表示を更新する
        if (roundText != null)
            roundText.text = "Stage Clear!";

        if (timerText != null)
            timerText.text = "";

        Debug.Log("Stage Clear!");
    }

    void UpdateRoundUI()
    {
        // 現在のウェーブまたは次のウェーブを表示する
        if (roundText == null)
            return;

        if (isIntermission)
        {
            roundText.text = "Next: Wave " + currentRound + " / " + maxRound;
        }
        else
        {
            roundText.text = "Wave " + currentRound + " / " + maxRound;
        }
    }

    void UpdateTimerUI()
    {
        // 準備時間の残り秒数を表示する
        if (timerText == null)
            return;

        if (isIntermission)
        {
            int remainingSeconds = Mathf.CeilToInt(Mathf.Max(timer, 0f));
            timerText.text = "Next wave: " + remainingSeconds;
        }
    }

    public void UpdateMoneyUI()
    {
        // お金の所持数をUIに反映する
        if (goldText != null)
            goldText.text = "Money: " + gold;
    }

    public void AddMoney(int amount)
    {
        // お金を追加する
        gold += amount;
        UpdateMoneyUI();
    }

    public bool SpendMoney(int amount)
    {
        // 所持お金が不足している場合は購入できない 
        if (gold < amount)
            return false;

        // お金を消費してUIを更新する
        gold -= amount;
        UpdateMoneyUI();

        return true;
    }
}