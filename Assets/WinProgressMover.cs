using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WinPositionTrigger : MonoBehaviour
{
    [System.Serializable]
    public class WinStage
    {
        [Header("第一階段移動")]
        public Transform firstPoint;
        [Tooltip("第一段移動要花幾秒")]
        public float firstDuration = 1.0f;

        [Header("中間停頓")]
        [Tooltip("移動完第一段後，要等幾秒才開始第二段?")]
        public float delayTime = 0.5f;

        [Header("第二階段移動")]
        public Transform secondPoint;
        [Tooltip("第二段移動要花幾秒")]
        public float secondDuration = 1.0f;
    }

    [Header("控制對象")]
    public Transform targetToMove;

    [Header("各勝場的移動設定")]
    [Tooltip("Element 0 = 0勝設定, Element 1 = 1勝設定...")]
    public List<WinStage> winStages;

    // 用來記錄上一次的勝場數
    private int _lastRecordedWins = -1; 

    [SerializeField] GameObject winGameObj;
    
    void Start()
    {
        if (targetToMove == null) targetToMove = this.transform;
    }

    void Update()
    {
        if (GameManager.Instance == null) return;

        int currentWins = GameManager.Instance.PlayerWins;

        if (currentWins != _lastRecordedWins)
        {
            _lastRecordedWins = currentWins;
            TriggerMove(currentWins);
        }
    }

    void TriggerMove(int winCount)
    {
        // 1. 防呆檢查
        if (winStages == null || winStages.Count == 0) return;

        // 2. 限制索引範圍
        int index = Mathf.Clamp(winCount, 0, winStages.Count - 1);
        WinStage currentStage = winStages[index];

        // 3. 執行連續移動
        StopAllCoroutines(); // 打斷之前的動作
        StartCoroutine(DoubleMoveRoutine(currentStage));

        Debug.Log($"[WinTrigger] 勝場變為 {winCount}，執行兩段式位移");

        // 4. 特殊勝利事件
        if (winCount == 3)
        {
            winGameObj.gameObject.SetActive(true);
        }
    }

    // 負責處理「移動 A -> 等待 -> 移動 B」的完整流程
    IEnumerator DoubleMoveRoutine(WinStage stage)
    {
        // --- 第一段移動 ---
        if (stage.firstPoint != null)
        {
            yield return StartCoroutine(SingleMove(stage.firstPoint, stage.firstDuration));
        }

        // --- 中間停頓 ---
        if (stage.delayTime > 0)
        {
            yield return new WaitForSeconds(stage.delayTime);
        }

        // --- 第二段移動 ---
        if (stage.secondPoint != null)
        {
            yield return StartCoroutine(SingleMove(stage.secondPoint, stage.secondDuration));
        }
    }

    // 單次移動的通用功能 (被上面呼叫)
    IEnumerator SingleMove(Transform targetDest, float duration)
    {
        Vector3 startPos = targetToMove.position;
        Quaternion startRot = targetToMove.rotation;
        
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            t = Mathf.SmoothStep(0f, 1f, t); // 平滑曲線

            targetToMove.position = Vector3.Lerp(startPos, targetDest.position, t);
            targetToMove.rotation = Quaternion.Lerp(startRot, targetDest.rotation, t);

            yield return null;
        }

        // 確保精準到達
        targetToMove.position = targetDest.position;
        targetToMove.rotation = targetDest.rotation;
    }
}