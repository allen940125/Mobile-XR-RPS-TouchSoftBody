
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    private RockPaperScissors _playerChoice;
    private RockPaperScissors _computerChoice;
    private int _playerWins = 0;
    public bool IsPlayerTurn { get; private set; }
    public bool IsPlayerCanMoveHand { get; private set; }

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

    private void Start()
    {
        _currentTurn = 0;
        _playerWins = 0;
        IsPlayerTurn = false;
        IsPlayerCanMoveHand = true;
        _playerChoice = RockPaperScissors.None;
        _computerChoice = RockPaperScissors.None;
    }

    public void NextTurn()
    {
        if (_currentTurn < _maxTurns)
        {
            _currentTurn++;
            Debug.Log($"Turn {_currentTurn} started.");

            IsPlayerTurn = true;
            IsPlayerCanMoveHand = false;
        }
        else
        {
            Debug.Log("Maximum turns reached.");

            // Game Over
            _currentTurn = 0;
            _playerWins = 0;
            IsPlayerTurn = false;
            IsPlayerCanMoveHand = true;
            _playerChoice = RockPaperScissors.None;
            _computerChoice = RockPaperScissors.None;
            SceneManager.LoadScene("HelloDangla");
        }
    }

    public void SetPlayerChoice(RockPaperScissors choice)
    {
        _playerChoice = choice;
        Debug.Log($"Player chose: {_playerChoice}");
    }

    public void ConfirmPlayerChoice()
    {
        if(_playerChoice != RockPaperScissors.None)
        {
            Debug.Log($"Player confirmed choice: {_playerChoice}");
            IsPlayerTurn = false;
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
        yield return new WaitForSeconds(_checkWinnerTime);

        if(_playerChoice == _computerChoice)
        {
            Debug.Log("It's a tie!");
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
        }

        _playerChoice = RockPaperScissors.None;
        _computerChoice = RockPaperScissors.None;

        IsPlayerCanMoveHand = true;
    }

    private void WhenPlayerWinsGame()
    {
        _playerWins++;
        switch(_playerWins)
        {
            case 1:
                Debug.Log("First Victory!");
                break;
            case 2:
                Debug.Log("Second Victory!");
                break;
            case 3:
                Debug.Log("Third Victory! You win the match!");
                break;
        }
    }
}