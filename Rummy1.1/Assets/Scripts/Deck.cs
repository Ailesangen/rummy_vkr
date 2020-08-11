using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public static char[] Suit = new char[] {'C', 'D', 'H', 'S'};
    public static char[] Rank = new char[] {'A', '2', '3', '4', '5', '6', '7', '8', '9', 'T', 'J', 'Q', 'K'};

    
    
    [SerializeField]
    private GameObject cardPrefab;
    [SerializeField]
    private List<GameObject> cards;

    private Player _playerOne;
    private Player _playerTwo;
    private Transform _transform;
    private float _zOffset = 0.03f;
    
    private void Start()
    {
        _playerOne = GameObject.FindWithTag("PlayerOne").GetComponent<Player>();
        _playerTwo = GameObject.FindWithTag("PlayerTwo").GetComponent<Player>();
        _transform = GetComponent<Transform>();
        StartGameEvents();
    }

    
    
    private void GenerateDeck()
    {
        
        for (int p = 0; p < 2; p++)
        {
            //для каждого пака
            for (int s = 0; s < Deck.Suit.Length; s++)
            {
                //для всех мастей
                for (int r = 0; r < Deck.Rank.Length; r++)
                {
                    GameObject newCardGo = Instantiate(cardPrefab, transform.position, Quaternion.identity, _transform);
                    newCardGo.name = "" + Deck.Suit[s] + Deck.Rank[r];
                    newCardGo.GetComponent<Card>().SetCard(Deck.Suit[s], Deck.Rank[r]);
                    cards.Add(newCardGo);
                }
            }
        }
        //добавляем джокера
        for (int i = 0; i < 16; i++)
        {
            GameObject newCardGo = Instantiate(cardPrefab, transform.position, Quaternion.identity, _transform);
            newCardGo.name = "XX";
            newCardGo.GetComponent<Card>().SetCard('X', 'X',true);
            cards.Add(newCardGo);
        }
        
    }

    private void StartGameEvents()
    {
        GenerateDeck();
        ShuffleDeck(cards);
        CalculatePosition();
        DealCardsToPlayer(_playerOne);
        DealCardsToPlayer(_playerTwo);
        _playerOne.SortCards();
        _playerTwo.SortCards();
        SetFirstCardToPile();
        GameManager.GM.sendMessage.Message(GameManager.GM.currentPlayer.name + " first");
        
    }

    
    //при next round сохраняем в Playerprefs раунд, очки игроков, дедвуд
    //перезагружаем сцену
    //загружаем данные
    //добавляем к раунду +1
    //добавляем очки игрокам
    //меняем deadwood
    
    
    public void ShuffleDeck<T>(List<T> list)
    {
        System.Random random = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            T temp = list[k];
            list[k] = list[n];
            list[n] = temp;
        }
    }
    
    private void CalculatePosition()
    {
        float zOffset = 20;
        foreach (var card in cards)
        {
            card.transform.position = new Vector3(transform.position.x, transform.position.y, zOffset);
            zOffset += _zOffset;
            
        }
    }
    private void DealCardsToPlayer(Player player)
    {
        for (int i = 0; i < 13; i++)
        {
            GameObject dealCard = cards[i];
            dealCard.transform.SetParent(player.transform);
            player.AddCard(dealCard);
            dealCard.GetComponent<Card>().RemoveBack();
            cards.RemoveAt(i);
        }
    }
    public void RemoveCard(GameObject gameObject)
    {
        cards.Remove(gameObject);
        CalculatePosition();
    }

    private void SetFirstCardToPile()
    {
        GameObject dealCard = cards[0];
        dealCard.transform.SetParent(GameManager.GM.pile.transform);
        GameManager.GM.pile.AddCard(dealCard);
        dealCard.GetComponent<Card>().RemoveBack();
        cards.RemoveAt(0);
        CalculatePosition();
    }
    
}
