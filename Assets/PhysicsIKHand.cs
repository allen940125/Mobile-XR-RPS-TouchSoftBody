using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PhysicsIKHand : MonoBehaviour
{
    [Header("目標設定")]
    [Tooltip("手想要去的位置 (通常是跟著動畫的手腕 ghost target)")]
    public Transform followTarget;

    [Tooltip("手臂的根部骨頭 (請拉入 Upper Arm / 大臂)")]
    public Transform armRoot; // <--- 新增這個欄位

    [Header("互動設定")]
    public string obstacleTag = "Interactable"; 

    [Header("物理參數")]
    public float followForce = 1000f;
    public float damping = 50f;

    [Header("骨骼限制")]
    [Tooltip("手臂的最大長度 (超過這個距離會強制被拉回)")]
    public float maxArmLength = 0.6f; // <--- 請根據你的模型調整這個數值

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false; 
        rb.linearDamping = 5f; 
        rb.angularDamping = 5f;

        // 如果忘記填 Arm Root，自動嘗試抓取父物件 (假設結構正確)
        if (armRoot == null)
        {
            Debug.LogWarning("請在 Inspector 指定 Arm Root (大臂)，目前暫時失效。");
        }
    }

    void FixedUpdate()
    {
        if (followTarget == null) return;

        // 1. 基礎物理跟隨 (讓手飛向目標)
        Vector3 positionDifference = followTarget.position - transform.position;
        rb.AddForce(positionDifference * followForce * Time.fixedDeltaTime);
        rb.linearVelocity *= (1f - damping * Time.fixedDeltaTime);

        // 2. === 骨骼長度限制 (防脫臼核心) ===
        if (armRoot != null)
        {
            // 計算「現在的手(IK)」離「大臂(Root)」有多遠
            float currentDistToRoot = Vector3.Distance(transform.position, armRoot.position);

            // 如果超過手臂總長度
            if (currentDistToRoot > maxArmLength)
            {
                // 算出方向：從大臂指向手
                Vector3 dirFromRoot = (transform.position - armRoot.position).normalized;
                
                // 強制把手的位置拉回到最大半徑的邊緣
                // 公式：大臂位置 + (方向 * 最大長度)
                rb.position = armRoot.position + dirFromRoot * maxArmLength;
                
                // 把向外的速度歸零，避免它一直想衝出去
                Vector3 velocityProjected = Vector3.Project(rb.linearVelocity, dirFromRoot);
                if (Vector3.Dot(velocityProjected, dirFromRoot) > 0)
                {
                    rb.linearVelocity -= velocityProjected;
                }
            }
        }
    }
}