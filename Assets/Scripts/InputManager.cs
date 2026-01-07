using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    private RockPaperScissors _playerChoice;
    
    [Header("物件綁定")]
    [SerializeField] private GameObject _playerLeftHand;
    [SerializeField] private GameObject _playerRightHand;
    [SerializeField] private GameObject _playerCam;

    [Header("移動參數")]
    [SerializeField] private float _handMoveRangeX = 0.5f;
    [SerializeField] private float _handMoveRangeY = 0.5f;
    [SerializeField] private float _handMoveRangeZ = 0.5f;
    [SerializeField] private float _handMoveSpeed = 2f;
    [SerializeField] private float _handRaiseSpeed = 2f;
    [SerializeField] private float _stickDeadZone = 0.1f;

    // 關鍵修改：我們不再記「世界座標」，而是記「相對於攝影機的初始偏移量」
    private Vector3 _leftHandStartOffset;
    private Vector3 _rightHandStartOffset;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // 算出：一開始手在攝影機的「哪裡」 (例如：攝影機的左邊0.3米、下面0.2米)
        // InverseTransformPoint = 把世界座標轉成局部座標
        if (_playerCam != null && _playerLeftHand != null)
            _leftHandStartOffset = _playerCam.transform.InverseTransformPoint(_playerLeftHand.transform.position);
            
        if (_playerCam != null && _playerRightHand != null)
            _rightHandStartOffset = _playerCam.transform.InverseTransformPoint(_playerRightHand.transform.position);
    }

    private void Update () 
    {
        if (GameManager.Instance == null) return;

        // 按鍵判定邏輯維持不變...
        if(GameManager.Instance.IsPlayerTurn)
        {
            if (Input.GetKeyDown ("joystick button 0") || Input.GetKeyDown(KeyCode.V)) GameManager.Instance.ConfirmPlayerChoice();
            if (Input.GetKeyDown ("joystick button 1") || Input.GetKeyDown(KeyCode.C)) { _playerChoice = RockPaperScissors.Rock; GameManager.Instance.SetPlayerChoice(_playerChoice); }
            if (Input.GetKeyDown ("joystick button 2") || Input.GetKeyDown(KeyCode.Z)) { _playerChoice = RockPaperScissors.Paper; GameManager.Instance.SetPlayerChoice(_playerChoice); }
            if (Input.GetKeyDown ("joystick button 3") || Input.GetKeyDown(KeyCode.X)) { _playerChoice = RockPaperScissors.Scissors; GameManager.Instance.SetPlayerChoice(_playerChoice); }
        }

        if(GameManager.Instance.IsPlayerLose || GameManager.Instance.IsPlayerCanMoveHand)
        {
            if (Input.GetKeyDown ("joystick button 0") || Input.GetKeyDown(KeyCode.V)) GameManager.Instance.NextTurn();
        }
        
        if(GameManager.Instance.IsPlayerCanMoveHand)
        {
            // 1. 取得所有輸入
            float triggerR = Input.GetAxis ("R_T");
            float triggerL = Input.GetAxis ("L_T");
            float lsh = Input.GetAxis ("L_Stick_H");
            float lsv = Input.GetAxis ("L_Stick_V");
            float rsh = Input.GetAxis ("R_Stick_H");
            float triL = Input.GetAxis ("L_Trigger");
            float triR = Input.GetAxis ("R_Trigger");

            // Keyboard Mapping
            if(Input.GetKey(KeyCode.F)) lsh = -1; else if(Input.GetKey(KeyCode.H)) lsh = 1;
            if(Input.GetKey(KeyCode.T)) lsv = -1; else if(Input.GetKey(KeyCode.G)) lsv = 1;
            if(Input.GetKey(KeyCode.J)) rsh = -1; else if(Input.GetKey(KeyCode.L)) rsh = 1;
            if(Input.GetKey(KeyCode.Y)) triL = -1;
            if(Input.GetKey(KeyCode.O)) triR = 1;
            
            // 死區過濾
            if (Mathf.Abs(triggerL) < _stickDeadZone) triggerL = 0;
            if (Mathf.Abs(triggerR) < _stickDeadZone) triggerR = 0;
            if (Mathf.Abs(lsh) < _stickDeadZone) lsh = 0;
            if (Mathf.Abs(lsv) < _stickDeadZone) lsv = 0;
            if (Mathf.Abs(rsh) < _stickDeadZone) rsh = 0;
            if (Mathf.Abs(triL) < _stickDeadZone) triL = 0;
            if (Mathf.Abs(triR) < _stickDeadZone) triR = 0;

            // 處理 Shoulder Buttons (RB/LB)
            float rightShoulder = (Input.GetKey("joystick button 5") || Input.GetKey(KeyCode.U)) ? -1f : 0f;
            float leftShoulder = (Input.GetKey("joystick button 4") || Input.GetKey(KeyCode.R)) ? -1f : 0f;

            // --- 核心修改開始 ---
            // 我們把「移動」和「限制」寫在一起處理，這樣邏輯最乾淨
            // 直接傳入所有的 Input，讓函式幫我們算出最後該去哪裡
            
            // 計算左手
            Vector3 leftInput = new Vector3(
                lsh + triL,          // X軸: 搖桿橫推 + 板機橫推
                triggerL + leftShoulder, // Y軸: 板機升 + LB降
                -lsv                 // Z軸: 搖桿直推 (往前是負所以加負號)
            );
            MoveAndClampHand(_playerLeftHand, leftInput, true);

            // 計算右手
            Vector3 rightInput = new Vector3(
                triR,                // X軸: 板機橫推
                triggerR + rightShoulder, // Y軸: 板機升 + RB降
                -rsh                 // Z軸: 隊友的特殊邏輯 (右搖桿橫推 = 前後移動)
            );
            MoveAndClampHand(_playerRightHand, rightInput, false);
            // --- 核心修改結束 ---
        }
    }

    // 這個函式負責：轉成局部 -> 移動 -> 限制 -> 轉回世界
    private void MoveAndClampHand(GameObject handObj, Vector3 inputDelta, bool isLeftHand)
    {
        if (handObj == null || _playerCam == null) return;

        // 1. 【關鍵】把現在手的位置，轉成「相對於攝影機」的座標
        // 這樣不管攝影機怎麼轉，LocalPos 的 Z 軸永遠是攝影機的前方
        Vector3 currentLocalPos = _playerCam.transform.InverseTransformPoint(handObj.transform.position);

        // 2. 加上輸入的移動量
        // 這裡直接改 LocalPos，等於是沿著攝影機的軸向移動
        currentLocalPos += inputDelta * Time.deltaTime * _handMoveSpeed; 
        // 註：Y軸如果你想用不同的速度，可以分開乘 (例如 inputDelta.y * _handRaiseSpeed)
        // 為了簡化我統一乘 MoveSpeed，你可以自己微調

        // 3. 限制範圍 (Clamp)
        // 拿「初始的相對位置」來當基準點
        Vector3 startOffset = isLeftHand ? _leftHandStartOffset : _rightHandStartOffset;

        // X軸限制
        currentLocalPos.x = Mathf.Clamp(currentLocalPos.x, startOffset.x - _handMoveRangeX, startOffset.x + _handMoveRangeX);
        
        // Y軸限制 (初始高度 上下移動)
        currentLocalPos.y = Mathf.Clamp(currentLocalPos.y, startOffset.y - _handMoveRangeY, startOffset.y + _handMoveRangeY);

        // Z軸限制 (前後)
        // 這裡可以解決你隊友 Z 軸很難搞的問題
        // startOffset.z 就是手一開始離相機多遠
        // 限制手不能離相機太近 (Min)，也不能太遠 (Max)
        float minZ = 0.547f; // 離相機最近距離 (絕對值，因為是 Local Z)
        float maxZ = startOffset.z + _handMoveRangeZ; // 最遠距離
        
        // 防呆：如果初始位置比 0.5 還近，就以初始位置為準
        if (startOffset.z < minZ) minZ = startOffset.z; 

        currentLocalPos.z = Mathf.Clamp(currentLocalPos.z, minZ, maxZ);

        // 4. 【關鍵】把算好的局部座標，轉回「世界座標」並套用
        handObj.transform.position = _playerCam.transform.TransformPoint(currentLocalPos);
    }
}