using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    private RockPaperScissors _playerChoice;
    [SerializeField] private GameObject _playerLeftHand;
    [SerializeField] private GameObject _playerRightHand;
    [SerializeField] private GameObject _playerCam;
    [SerializeField] private float _handMoveRangeX = 0.5f;
    [SerializeField] private float _handMoveRangeY = 0.5f;
    [SerializeField] private float _handMoveRangeZ = 0.5f;
    [SerializeField] private float _handMoveSpeed = 2f;
    [SerializeField] private float _handRaiseSpeed = 2f;
    private Vector3 _leftHandStartPos;
    private Vector3 _rightHandStartPos;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        _leftHandStartPos = _playerLeftHand.transform.localPosition;
        _rightHandStartPos = _playerRightHand.transform.localPosition;
    }

    private void Update () 
    {
        if(GameManager.Instance.IsPlayerTurn)
        {
            //A
            if (Input.GetKeyDown ("joystick button 0") || Input.GetKeyDown(KeyCode.V))
            {
                Debug.Log ("A");
                GameManager.Instance.ConfirmPlayerChoice();
            }

            //B
            if (Input.GetKeyDown ("joystick button 1") || Input.GetKeyDown(KeyCode.C))
            {
                Debug.Log ("B");
                _playerChoice = RockPaperScissors.Rock;
                GameManager.Instance.SetPlayerChoice(_playerChoice);
            }

            //X
            if (Input.GetKeyDown ("joystick button 2") || Input.GetKeyDown(KeyCode.Z))
            {
                Debug.Log ("X");
                _playerChoice = RockPaperScissors.Paper;
                GameManager.Instance.SetPlayerChoice(_playerChoice);
            }

            //Y
            if (Input.GetKeyDown ("joystick button 3") || Input.GetKeyDown(KeyCode.X))
            {
                Debug.Log ("Y");
                _playerChoice = RockPaperScissors.Scissors;
                GameManager.Instance.SetPlayerChoice(_playerChoice);
            }
        }

        if(GameManager.Instance.IsPlayerLose)
        {
            //A
            if (Input.GetKeyDown ("joystick button 0") || Input.GetKeyDown(KeyCode.V))
            {
                Debug.Log ("Player Press A, Start Next Turn");
                GameManager.Instance.NextTurn();
            }
        }
        
        if(GameManager.Instance.IsPlayerCanMoveHand)
        {
            //A
            if (Input.GetKeyDown ("joystick button 0") || Input.GetKeyDown(KeyCode.V))
            {
                Debug.Log ("Player Press A, Start Next Turn");
                GameManager.Instance.NextTurn();
            }

            // //選擇鍵
            // if (Input.GetKeyDown ("joystick button 6"))
            // {
            //     Debug.Log ("Back");
            // }

            // //開始鍵
            // if (Input.GetKeyDown ("joystick button 7"))
            // {
            //     Debug.Log ("Start");
            // }

            // //左搖桿按下
            // if (Input.GetKeyDown ("joystick button 8"))
            // {
            //     Debug.Log ("LS");
            // }

            // //右搖桿按下
            // if (Input.GetKeyDown ("joystick button 9"))
            // {
            //     Debug.Log ("RS");
            // }

            //左搖桿
            float lsh = Input.GetAxis ("L_Stick_H");
            float lsv = Input.GetAxis ("L_Stick_V");
            //Keyboard mapping
            if(Input.GetKey(KeyCode.F))
            {
                lsh = -1;
            }
            else if(Input.GetKey(KeyCode.H))
            {
                lsh = 1;
            }
            if(Input.GetKey(KeyCode.T))
            {
                lsv = -1;
            }
            else if(Input.GetKey(KeyCode.G))
            {
                lsv = 1;
            }

            if(( lsh != 0) || (lsv != 0 ))
            {
                Debug.Log ("L stick:"+lsh+","+lsv );
                _playerLeftHand.transform.Translate(new Vector3(lsh, 0, -lsv) * Time.deltaTime * _handMoveSpeed);
            }

            //右搖桿
            float rsh = Input.GetAxis ("R_Stick_H");
            float rsv = Input.GetAxis ("R_Stick_V");
            //Keyboard mapping
            if(Input.GetKey(KeyCode.J))
            {
                rsh = -1;
            }
            else if(Input.GetKey(KeyCode.L))
            {
                rsh = 1;
            }
            if(Input.GetKey(KeyCode.I))
            {
                rsv = -1;
            }
            else if(Input.GetKey(KeyCode.K))
            {
                rsv = 1;
            }

            if(( rsh != 0 ) || (rsv != 0 ))
            {
                Debug.Log ("R stick:"+rsh+","+rsv );
                _playerRightHand.transform.Translate(new Vector3(rsh, 0, -rsv) * Time.deltaTime * _handMoveSpeed);
            }

            // //十字鍵
            // float dph = Input.GetAxis ("D_Pad_H");
            // float dpv = Input.GetAxis ("D_Pad_V");
            // if(( dph != 0 ) || ( dpv != 0 ))
            // {
            //     Debug.Log ("D Pad:"+dph+","+dpv );
            // }

            //Trigger L
            float triL = Input.GetAxis ("L_Trigger");
            //Keyboard mapping
            if(Input.GetKey(KeyCode.Y))
            {
                triL = -1;
            }

            if( triL < 0 ) //左板機
            {
                Debug.Log ("L trigger:"+triL );
                _playerLeftHand.transform.Translate(new Vector3(0, -triL, 0) * Time.deltaTime * _handRaiseSpeed);
            }

            //Trigger R
            float triR = Input.GetAxis ("R_Trigger");
            //Keyboard mapping
            if(Input.GetKey(KeyCode.O))
            {
                triR = 1;
            }

            if( triR > 0 ) //右板機
            {
                Debug.Log ("R trigger:"+triR );
                _playerRightHand.transform.Translate(new Vector3(0, triR, 0) * Time.deltaTime * _handRaiseSpeed);
            }

            //RB
            if (Input.GetKey("joystick button 5") || Input.GetKey(KeyCode.U))
            {
                Debug.Log ("RB");
                _playerRightHand.transform.Translate(new Vector3(0, -1, 0) * Time.deltaTime * _handRaiseSpeed);
            }

            //LB
            if (Input.GetKey("joystick button 4") || Input.GetKey(KeyCode.R))
            {
                Debug.Log ("LB");
                _playerLeftHand.transform.Translate(new Vector3(0, -1, 0) * Time.deltaTime * _handRaiseSpeed);
            }

            ClampHandPosition(_playerLeftHand.transform, true);
            ClampHandPosition(_playerRightHand.transform, false);
        }
    }

    private void ClampHandPosition(Transform hand, bool isLeftHand)
    {
        Vector3 camPos = _playerCam.transform.localPosition;
        Vector3 handPos = hand.localPosition;
        Vector3 startPos = isLeftHand ? _leftHandStartPos : _rightHandStartPos;

        handPos.x = Mathf.Clamp(handPos.x, startPos.x - _handMoveRangeX, startPos.x + _handMoveRangeX);
        handPos.y = Mathf.Clamp(handPos.y, camPos.y - _handMoveRangeY, camPos.y + _handMoveRangeY);
        handPos.z = Mathf.Clamp(handPos.z, camPos.z + 0.547f, camPos.z + _handMoveRangeZ); //這行比較特別是因為不能讓手移到攝影機後面

        hand.localPosition = handPos;
    }
}