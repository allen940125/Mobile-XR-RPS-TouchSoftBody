
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;

public enum RockPaperScissors
{
    Rock,
    Paper,
    Scissors,
    None
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private int _maxTurns = 3;
    [SerializeField] private int _currentTurn = 0;
    [SerializeField] private float _computerThinkingTime = 1.0f;
    [SerializeField] private float _checkWinnerTime = 1.0f;
    [SerializeField] private float _showWinnerTime = 1.0f;
    private RockPaperScissors _playerChoice;
    private RockPaperScissors _computerChoice;
    private int _playerWins = 0;
    public bool IsPlayerTurn { get; private set; }
    public bool IsPlayerCanMoveHand { get; private set; }
    public bool IsPlayerLose { get; private set; }
    [SerializeField] private GameObject _playerRockPaperScissorsObject;
    [SerializeField] private List<GameObject> _playerChoiceDisplay;
    [SerializeField] private List<Sprite> _rockPaperScissorsSprites;
    [SerializeField] private GameObject _checkWinnerObject;
    [SerializeField] private Image _playerChoiceImage;
    [SerializeField] private Image _computerChoiceImage;
    [SerializeField] private GameObject _playerWinObject;
    [SerializeField] private GameObject _computerWinObject;
    [SerializeField] private Text _dialogueText;
    [SerializeField] private GameObject _dialogueObject;

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
        DefalutSetting();
    }

    private void DefalutSetting()
    {
        _currentTurn = 0;
        _playerWins = 0;
        IsPlayerTurn = false;
        IsPlayerCanMoveHand = true;
        IsPlayerLose = false;
        _playerChoice = RockPaperScissors.None;
        _computerChoice = RockPaperScissors.None;
        _playerRockPaperScissorsObject.SetActive(false);
        _checkWinnerObject.SetActive(false);
        _playerWinObject.SetActive(false);
        _computerWinObject.SetActive(false);
        _dialogueText.text = "按A鍵開始遊戲";
        _dialogueObject.SetActive(true);
    }

    public void NextTurn()
    {
        _dialogueObject.SetActive(false);

        IsPlayerLose = false;

        if (_currentTurn < _maxTurns)
        {
            _currentTurn++;
            Debug.Log($"Turn {_currentTurn} started.");

            IsPlayerTurn = true;
            IsPlayerCanMoveHand = false;
            
            for (int i = 0; i < _playerChoiceDisplay.Count; i++)
            {
                _playerChoiceDisplay[i].GetComponent<MeshRenderer>().material.color = Color.white;
            }

            _playerRockPaperScissorsObject.SetActive(true);
        }
        else
        {
            Debug.Log("Maximum turns reached.");

            // Game Over
            DefalutSetting();
            SceneManager.LoadScene("HelloDangla");
        }
    }

    public void SetPlayerChoice(RockPaperScissors choice)
    {
        for (int i = 0; i < _playerChoiceDisplay.Count; i++)
        {
            _playerChoiceDisplay[i].GetComponent<MeshRenderer>().material.color = Color.white;
        }

        _playerChoiceDisplay[(int)choice].GetComponent<MeshRenderer>().material.color = Color.yellow;
        _playerChoiceImage.sprite = _rockPaperScissorsSprites[(int)choice];
        _playerChoice = (RockPaperScissors)choice;
        Debug.Log($"Player chose: {_playerChoice}");
    }

    public void ConfirmPlayerChoice()
    {
        if(_playerChoice != RockPaperScissors.None)
        {
            Debug.Log($"Player confirmed choice: {_playerChoice}");
            IsPlayerTurn = false;
            _playerChoiceImage.sprite = _rockPaperScissorsSprites[(int)_playerChoice];
            _computerChoiceImage.sprite = null;
            _playerRockPaperScissorsObject.SetActive(false);
            _checkWinnerObject.SetActive(true);
            StartCoroutine(SetComputerChoice());
        }
    }

    private IEnumerator SetComputerChoice()
    {
        yield return new WaitForSeconds(_computerThinkingTime);
        
        int choice = Random.Range(0, 3);
        _computerChoice = (RockPaperScissors)choice;
        Debug.Log($"Computer chose: {_computerChoice}");
        StartCoroutine(CheckWinner());
    }

    private IEnumerator CheckWinner()
    {
        _computerChoiceImage.sprite = _rockPaperScissorsSprites[(int)_computerChoice];
    
        yield return new WaitForSeconds(_checkWinnerTime);

        if(_playerChoice == _computerChoice)
        {
            WhenPlayerTiesGame();
        }
        else if((_playerChoice == RockPaperScissors.Rock && _computerChoice == RockPaperScissors.Scissors) ||
                (_playerChoice == RockPaperScissors.Paper && _computerChoice == RockPaperScissors.Rock) ||
                (_playerChoice == RockPaperScissors.Scissors && _computerChoice == RockPaperScissors.Paper))
        {
            WhenPlayerWinsGame(); 
        }
        else
        {
            Debug.Log("Computer wins the round!");
            WhenPlayerLoseGame();
        }

        _playerChoice = RockPaperScissors.None;
        _computerChoice = RockPaperScissors.None;

        yield return new WaitForSeconds(_showWinnerTime);

        _playerWinObject.SetActive(false);
        _computerWinObject.SetActive(false);
        _checkWinnerObject.SetActive(false);

        _dialogueObject.SetActive(true);
    }

    private void WhenPlayerTiesGame()
    {
        IsPlayerLose = true;

        Debug.Log("It's a tie!");
        switch(_currentTurn)
        {
            case 1:
                _dialogueText.text = "可惜還有兩次機會!加油!";
                break;
            case 2:
                _dialogueText.text = "可惜還有一次機會!加油!";
                break;
            case 3:
                _dialogueText.text = "可惜沒機會了，要再一次嗎?(按A鍵重新開始)";
                break;
        }
    }

    private void WhenPlayerLoseGame()
    {
        _computerWinObject.SetActive(true);
        IsPlayerLose = true;

        switch(_currentTurn)
        {
            case 1:
                Debug.Log("CPU Win 1");
                _dialogueText.text = "可惜還有兩次機會!加油!";
                break;
            case 2:
                Debug.Log("CPU Win 2");
                _dialogueText.text = "可惜還有一次機會!加油!";
                break;
            case 3:
                _dialogueText.text = "可惜沒機會了，要再一次嗎?(按A鍵重新開始)";
                break;
        }
    }

    private void WhenPlayerWinsGame()
    {
        _playerWinObject.SetActive(true);
        IsPlayerCanMoveHand = true;
        _playerWins++;
        switch(_playerWins)
        {
            case 1:
                Debug.Log("First Victory!");
                _dialogueText.text = "按A鍵繼續下一回合";
                break;
            case 2:
                Debug.Log("Second Victory!");
                _dialogueText.text = "按A鍵繼續下一回合";
                break;
            case 3:
                Debug.Log("Third Victory! You win the match!");
                _dialogueText.text = "按A鍵重新開始";
                break;
        }
    }
}