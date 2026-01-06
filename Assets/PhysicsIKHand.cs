using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PhysicsIKHand : MonoBehaviour
{
    [Header("目標設定")]
    public Transform followTarget; 
    public Transform armRoot;      

    [Header("物理參數 (移動)")]
    public float followForce = 1000f;
    public float damping = 50f;

    // --- 新增：旋轉相關參數 ---
    [Header("物理參數 (旋轉)")]
    [Tooltip("旋轉的力道 (手腕扭力)")]
    public float rotateForce = 100f; // 建議值：50 ~ 200
    [Tooltip("旋轉阻尼 (避免轉過頭一直抖)")]
    public float rotDamping = 10f;   // 建議值：5 ~ 20
    // -------------------------

    [Header("防穿模 / 重置設定")]
    public float maxArmLength = 0.6f;
    public float maxDistanceError = 0.8f; 

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        
        // 設定移動阻尼
        rb.linearDamping = 5f; 
        
        // --- 修改：設定旋轉阻尼 ---
        // 提高 Angular Damping 可以讓旋轉更穩定，不會瘋狂抖動
        rb.angularDamping = rotDamping; 
    }

    void FixedUpdate()
    {
        if (followTarget == null) return;

        // 1. === 嚴重誤差重置 ===
        float distToTarget = Vector3.Distance(transform.position, followTarget.position);
        if (distToTarget > maxDistanceError)
        {
            if (armRoot != null) rb.position = armRoot.position;
            else rb.position = followTarget.position;

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            return; 
        }

        // 2. === 基礎物理跟隨 (移動) ===
        Vector3 positionDifference = followTarget.position - transform.position;
        rb.AddForce(positionDifference * followForce * Time.fixedDeltaTime);
        rb.linearVelocity *= (1f - damping * Time.fixedDeltaTime);

        // 3. === 【新增】物理旋轉跟隨 (扭力) ===
        ApplyRotationForce();

        // 4. === 骨骼長度限制 ===
        if (armRoot != null)
        {
            float currentDistToRoot = Vector3.Distance(transform.position, armRoot.position);
            if (currentDistToRoot > maxArmLength)
            {
                Vector3 dirFromRoot = (transform.position - armRoot.position).normalized;
                rb.position = armRoot.position + dirFromRoot * maxArmLength;
                
                Vector3 velocityProjected = Vector3.Project(rb.linearVelocity, dirFromRoot);
                if (Vector3.Dot(velocityProjected, dirFromRoot) > 0)
                {
                    rb.linearVelocity -= velocityProjected;
                }
            }
        }
    }

    // 計算並施加旋轉力
    void ApplyRotationForce()
    {
        // 算出「目前角度」跟「目標角度」差了多少 (Quaternion math)
        Quaternion rotationDifference = followTarget.rotation * Quaternion.Inverse(transform.rotation);
        
        rotationDifference.ToAngleAxis(out float angleInDegrees, out Vector3 rotationAxis);

        // 修正角度範圍，讓它永遠走最近的路徑 (例如 -10度 而不是 +350度)
        if (angleInDegrees > 180f) angleInDegrees -= 360f;

        // 如果角度很小就忽略 (避免微小抖動)
        if (Mathf.Abs(angleInDegrees) > 1f) 
        {
            // 將角度轉為弧度並施加扭力 (AddTorque)
            // 這裡的公式是： 軸向 * (角度差 * 力道) - (目前的旋轉速度 * 阻尼)
            // 類似 PD 控制器 (Proportional-Derivative Controller)
            
            // 簡化版寫法 (依賴 Rigidbody 的 angularDamping)
            Vector3 torqueToApply = rotationAxis * (angleInDegrees * rotateForce * Time.fixedDeltaTime);
            rb.AddTorque(torqueToApply);
        }
    }
}