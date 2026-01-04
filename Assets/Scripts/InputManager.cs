using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    private RockPaperScissors _playerChoice;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update () 
    {
        if(GameManager.Instance.IsPlayerTurn)
        {
            //A
            if (Input.GetKeyDown ("joystick button 0"))
            {
                Debug.Log ("A");
                GameManager.Instance.ConfirmPlayerChoice();
            }

            //B
            if (Input.GetKeyDown ("joystick button 1"))
            {
                Debug.Log ("B");
                _playerChoice = RockPaperScissors.Rock;
                GameManager.Instance.SetPlayerChoice(_playerChoice);
            }

            //X
            if (Input.GetKeyDown ("joystick button 2"))
            {
                Debug.Log ("X");
                _playerChoice = RockPaperScissors.Paper;
                GameManager.Instance.SetPlayerChoice(_playerChoice);
            }

            //Y
            if (Input.GetKeyDown ("joystick button 3"))
            {
                Debug.Log ("Y");
                _playerChoice = RockPaperScissors.Scissors;
                GameManager.Instance.SetPlayerChoice(_playerChoice);
            }
        }
        
        if(GameManager.Instance.IsPlayerCanMoveHand)
        {
            //A
            if (Input.GetKeyDown ("joystick button 0"))
            {
                Debug.Log ("Player Press A, Start Next Turn");
                GameManager.Instance.NextTurn();
            }

            //LB
            if (Input.GetKeyDown ("joystick button 4"))
            {
                Debug.Log ("LB");
            }
            
            //RB
            if (Input.GetKeyDown ("joystick button 5"))
            {
                Debug.Log ("RB");
            }

            //選擇鍵
            if (Input.GetKeyDown ("joystick button 6"))
            {
                Debug.Log ("Back");
            }

            //開始鍵
            if (Input.GetKeyDown ("joystick button 7"))
            {
                Debug.Log ("Start");
            }

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
            if(( lsh != 0) || (lsv != 0 ))
            {
                Debug.Log ("L stick:"+lsh+","+lsv );
            }

            //右搖桿
            float rsh = Input.GetAxis ("R_Stick_H");
            float rsv = Input.GetAxis ("R_Stick_V");
            if(( rsh != 0 ) || (rsv != 0 ))
            {
                Debug.Log ("R stick:"+rsh+","+rsv );
            }

            //十字鍵
            float dph = Input.GetAxis ("D_Pad_H");
            float dpv = Input.GetAxis ("D_Pad_V");
            if(( dph != 0 ) || ( dpv != 0 ))
            {
                Debug.Log ("D Pad:"+dph+","+dpv );
            }

            //Trigger
            float tri = Input.GetAxis ("L_R_Trigger");
            if( tri > 0 ) //右板機
            {
                Debug.Log ("R trigger:"+tri );
            }
            else if( tri < 0 ) //左板機
            {
                Debug.Log ("L trigger:"+tri );
            }
        }
    }
}