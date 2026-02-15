using UnityEngine;
using UnityEngine.UI;

public class EnemyUISuppressionView : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private EnemySystem enemy;
    [SerializeField] private Image targetImage;

    [Header("ブロック単位アニメ")]
    [SerializeField] private int blockCountX = 80;
    [SerializeField] private int blockCountY = 45;

    [Tooltip("1秒あたり何ブロック反転するか")]
    [SerializeField] private float blocksPerSecond = 180f;

    private static readonly int ProgressId = Shader.PropertyToID("_Progress");

    private Material runtimeMat;

    // 今表示しているブロック数
    private int currentBlocks = 0;

    // 次のブロック反転までの残り時間
    private float stepTimer = 0f;

    void Awake()
    {
        if (targetImage == null) targetImage = GetComponent<Image>();

        // 共有を避ける（この敵だけの進捗を持たせる）
        if (targetImage != null && targetImage.material != null)
        {
            runtimeMat = Instantiate(targetImage.material);
            targetImage.material = runtimeMat;
        }

        ApplyImmediate();
    }

    void Update()
    {
        if (enemy == null || runtimeMat == null) return;

        int totalBlocks = Mathf.Max(1, blockCountX * blockCountY);

        // 目標ブロック数（制圧率に応じて）
        float targetProgress = Mathf.Clamp01(enemy.SuppressionPercent / 100f);
        int targetBlocks = Mathf.Clamp(Mathf.FloorToInt(targetProgress * totalBlocks), 0, totalBlocks);

        if (currentBlocks == targetBlocks) return;

        float bps = Mathf.Max(1f, blocksPerSecond);
        float interval = 1f / bps;

        // 定間隔で「ちょうど1ブロック」進める
        stepTimer += Time.deltaTime;

        while (stepTimer >= interval && currentBlocks != targetBlocks)
        {
            stepTimer -= interval;

            if (currentBlocks < targetBlocks) currentBlocks += 1;
            else currentBlocks -= 1; // 制圧率が戻る仕様があるなら
        }

        float progress = (float)currentBlocks / totalBlocks;
        runtimeMat.SetFloat(ProgressId, progress);
    }

    public void ApplyImmediate()
    {
        if (enemy == null || runtimeMat == null) return;

        int totalBlocks = Mathf.Max(1, blockCountX * blockCountY);
        float p = Mathf.Clamp01(enemy.SuppressionPercent / 100f);
        currentBlocks = Mathf.Clamp(Mathf.FloorToInt(p * totalBlocks), 0, totalBlocks);

        runtimeMat.SetFloat(ProgressId, (float)currentBlocks / totalBlocks);
    }
}