using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WinPositionTrigger : MonoBehaviour
{
    [Header("控制對象")]
    public Transform targetToMove; // 你的 Hand Target

    [Header("位置設定")]
    [Tooltip("對應勝場數的位置 (Element 0 = 0勝, Element 1 = 1勝...)")]
    public List<Transform> progressPoints;

    [Header("移動參數")]
    public float moveDuration = 1.0f; // 移動需要幾秒

    // 用來記錄上一次的勝場數，預設 -1 代表剛開始一定要觸發一次
    private int _lastRecordedWins = -1; 

    [SerializeField] GameObject winGameObj;
    
    void Start()
    {
        if (targetToMove == null) targetToMove = this.transform;
    }

    void Update()
    {
        if (GameManager.Instance == null) return;

        // 1. 取得現在 GameManager 的勝場數
        // (記得 GameManager 裡要有 public int PlayerWins => _playerWins;)
        int currentWins = GameManager.Instance.PlayerWins;

        // 2. 檢測：如果「現在的勝場」跟「上一次記錄」不一樣，代表狀態改變了！
        if (currentWins != _lastRecordedWins)
        {
            // 更新記錄
            _lastRecordedWins = currentWins;

            // 觸發移動邏輯
            TriggerMove(currentWins);
        }
    }

    void TriggerMove(int winCount)
    {
        // 防呆：如果沒設位置點，就不做
        if (progressPoints == null || progressPoints.Count == 0) return;

        // 限制索引範圍 (避免贏太多次超過陣列)
        int index = Mathf.Clamp(winCount, 0, progressPoints.Count - 1);
        Transform destination = progressPoints[index];

        if (destination != null)
        {
            // 先停止之前的移動 (如果還在跑的話)，避免衝突
            StopAllCoroutines();
            
            // 開始新的移動
            StartCoroutine(MoveRoutine(destination));
            
            Debug.Log($"[WinTrigger] 勝場變為 {winCount}，開始移動到 {destination.name}");
        }

        if (winCount == 3)
        {
            winGameObj.gameObject.SetActive(true);
        }
    }

    // 協程：負責平滑移動，移動完就自動結束，不再佔用資源
    IEnumerator MoveRoutine(Transform dest)
    {
        Vector3 startPos = targetToMove.position;
        Quaternion startRot = targetToMove.rotation;
        
        float timer = 0f;

        while (timer < moveDuration)
        {
            timer += Time.deltaTime;
            float t = timer / moveDuration;

            // 使用 SmoothStep 讓移動起步和停止更滑順
            t = Mathf.SmoothStep(0f, 1f, t);

            targetToMove.position = Vector3.Lerp(startPos, dest.position, t);
            targetToMove.rotation = Quaternion.Lerp(startRot, dest.rotation, t);

            yield return null; // 等待下一幀
        }

        // 確保最後精準到達
        targetToMove.position = dest.position;
        targetToMove.rotation = dest.rotation;
        
        // 跑完這裡就結束了，不會再有任何 Update 干擾
    }
}