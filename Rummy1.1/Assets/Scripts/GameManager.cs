using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int deadWood = 500;
    public static GameManager GM;
    public Deck deck;
    public Drop drop;
    public Pile pile;
    public Player playerOne;
    public Player playerTwo;
    public Rules rules;
    public Player currentPlayer;
    public int firstPlayerWas = 0;
    public EventHandler eventHandler;
    public int round = 1;
    [SerializeField]
    private TextMeshProUGUI centralMessageTMP;

    public GameObject nextRoundPanel;
    public GameObject endGamePanel;
    public TextMeshProUGUI roundInfo;
    public TextMeshProUGUI playerOneInfo;
    public TextMeshProUGUI playerTwoInfo;
    public TextMeshProUGUI deadwoodInfo;
    
    public CentralMessage sendMessage;
    private void Awake()
    {
        if (GM == null)
            GM = this;
        sendMessage = centralMessageTMP.GetComponent<CentralMessage>();
        deck = GameObject.FindWithTag("Deck").GetComponent<Deck>();
        drop = GameObject.FindObjectOfType<Drop>();
        pile = GameObject.FindWithTag("Pile").GetComponent<Pile>();
        playerOne = GameObject.FindWithTag("PlayerOne").GetComponent<Player>();
        playerTwo = GameObject.FindWithTag("PlayerTwo").GetComponent<Player>();
        eventHandler = GetComponent<EventHandler>();
        LoadState();
        ChooseFirstPlayer();
        rules = GetComponent<Rules>();

    }

    private void Start()
    {
        
        RefreshGameInfo();
    }

    public void RefreshGameInfo()
    {
        roundInfo.text = "Round: " + round;
        playerOneInfo.text = "Player One: " + playerOne.points;
        playerTwoInfo.text = "Player Two: " + playerTwo.points;
        deadwoodInfo.text = "Deadwood: " + deadWood;
    }
    private void ChooseFirstPlayer()
    {   System.Random random = new System.Random();
        if (firstPlayerWas == 0)
        {
            if (random.Next(0, 2) == 0)
            {
                currentPlayer = playerOne;
                firstPlayerWas = 1;
                //sendMessage.Message("Player one is first");
            }
            else
            {
                currentPlayer = playerTwo;
                firstPlayerWas = 2;
                //sendMessage.Message("Player one is first");
            }
        }
        else
        {
            Debug.Log("Choose player from PlayerPrefs");
            if (firstPlayerWas == 1)
            {
                currentPlayer = playerTwo;
                firstPlayerWas = 2;
            }
            else
            {
                currentPlayer = playerOne;
                firstPlayerWas = 1;
            }
        }
    }

    public void NextTurn()
    {
        if (currentPlayer == playerOne)
        {
            currentPlayer = playerTwo;
            playerOne.AlreadyTakeCard = false;
            playerTwo.AlreadyTakeCard = false;
            sendMessage.Message("Player two next");
        }
        else
        {
            currentPlayer = playerOne;
            playerOne.AlreadyTakeCard = false;
            playerTwo.AlreadyTakeCard = false;
            sendMessage.Message("Player one next");
        }
    }

    public void ExitGameEvent()
    {
        Application.Quit();
    }

    public void NewGameEvent()
    {
        DeleteState();
        SceneManager.LoadScene("GameScene");
    }

    public void NextRoundEvent()
    {
        Debug.Log("Next round button");
        SceneManager.LoadScene("GameScene");
    }

    public void SaveState()
    {
        PlayerPrefs.SetInt("playerOnePoints", playerOne.points);
        PlayerPrefs.SetInt("playerTwoPoints", playerTwo.points);
        PlayerPrefs.SetInt("round", round);
        PlayerPrefs.SetInt("deadwood", deadWood);
        PlayerPrefs.SetInt("firstPlayerWas", firstPlayerWas);
        PlayerPrefs.SetInt("deadwood", deadWood);
        PlayerPrefs.Save();
    }
    public void LoadState()
    {
        if (PlayerPrefs.HasKey("round"))
        {
            round = PlayerPrefs.GetInt("round");
            playerOne.points = PlayerPrefs.GetInt("playerOnePoints");
            playerTwo.points = PlayerPrefs.GetInt("playerTwoPoints");
            deadWood = PlayerPrefs.GetInt("deadwood");
            firstPlayerWas = PlayerPrefs.GetInt("firstPlayerWas");
            deadWood = PlayerPrefs.GetInt("deadwood");
            RefreshGameInfo();
        }
    }

    public void DeleteState()
    {
        PlayerPrefs.DeleteAll();
    }
    
    
}
