using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private List<GameObject> cards;
    private float yPosition;
    private int maxCardInHand = 15;
    private float _zOffset = 0.3f;
    [SerializeField]
    private bool _alreadyTakeCard;
    
    public bool getThirtyPoints = false;
    public int points = 0;
    
    public Dictionary<char, int> cardPoint = new Dictionary<char, int>()
    {
        {'A', 15}, {'2', 2}, {'3', 3}, {'4', 4}, {'5', 5}, {'6', 6}, {'7', 7}, {'8', 8}, {'9', 9}, {'T', 10}, {'J', 10}, {'Q', 10}, {'K', 10}, {'X', 30}
    };
    public bool AlreadyTakeCard
    {
        get => _alreadyTakeCard;
        set => _alreadyTakeCard = value;
    }

    private void Awake()
    {
        yPosition = transform.position.y;
        
    }

    public bool IsEmtyHand()
    {
        if (cards.Count == 0)
        {
            return true;
        }
        return false;
    }

    public void WinEvent()
    {
        int losePoints = 0;
        Player losePlayer;
        //победа current player
        //считаем очки для игроков
        if (GameManager.GM.playerOne == GameManager.GM.currentPlayer)
        {
            //победил второй игрок, считаем очки для него
            losePlayer = GameManager.GM.playerTwo;
            losePoints = losePlayer.CalculateHandPoints();
        }
        else
        {
            losePlayer = GameManager.GM.playerOne;
            losePoints = losePlayer.CalculateHandPoints();
        }
        if (GameManager.GM.drop.IsAllCardsNewInSlots())
        {
            //удваиваем посчитанные очки
            GameManager.GM.sendMessage.Message("Full drop");
            Debug.Log("FULL DROP!");
            losePoints *= 2;
        }
        //сохраняем очки
        losePlayer.points += losePoints;
        
        //проверяем преодолен ли дедвуд
        if (losePlayer.points >= GameManager.GM.deadWood)
        {
            Debug.Log("Deadwood reached, " + losePlayer.name + " win");
            GameManager.GM.RefreshGameInfo();
            GameManager.GM.endGamePanel.SetActive(true);
        }
        else
        {
            GameManager.GM.round += 1;
            Debug.Log("New round");
            GameManager.GM.RefreshGameInfo();
            GameManager.GM.SaveState();
            GameManager.GM.nextRoundPanel.SetActive(true);
        }
        //если преодолен - новая игра
        //если нет - новый раунд
    }
    
    public int CalculateHandPoints()
    {
        int p = 0;
        int points = 0;
        foreach (var card in cards)
        {
            p = 0;
            cardPoint.TryGetValue(card.GetComponent<Card>().Rank, out p);
            points += p;
        }
        return points;
    }
    public void CalculateCardPosition()
    {
        //Debug.Log("calc pos");
        float zOffset = 10;
        float xOffset = (cards.Count / 1.5f) / cards.Count;
        float startPos = -cards.Count / 3f;
        foreach (var card in cards)
        {
            card.transform.position = new Vector3(startPos, transform.position.y, zOffset);
            zOffset += -_zOffset;
            startPos += xOffset;
        }
    }
    
    public void InsertCardBehind(GameObject gameObject, GameObject place)
    {
        cards.Remove(gameObject);
        cards.Insert(cards.IndexOf(place)+1, gameObject);
        CalculateCardPosition();
    }
    
    public void AddCard(GameObject gameObject)
    {
        cards.Add(gameObject);
        CalculateCardPosition();
    }

    public void RemoveCard(GameObject gameObject)
    {
        cards.Remove(gameObject);
        CalculateCardPosition();
    }
    
    public void TakeCardFromDeck(GameObject gameObject)
    {
        if (cards.Count >= maxCardInHand || _alreadyTakeCard)
        {
            Debug.Log("You already take card");
        }
        else
        {
            gameObject.transform.parent = transform;
            AddCard(gameObject);
            GameManager.GM.deck.RemoveCard(gameObject);
            gameObject.GetComponent<Card>().RemoveBack();
            _alreadyTakeCard = true;
        }
    }
    
    public void TakeCardFromPile(GameObject gameObject)
    {
        if (cards.Count >= maxCardInHand || _alreadyTakeCard)
        {
            Debug.Log("You already take card");
        }
        else
        {
            gameObject.transform.parent = transform;
            AddCard(gameObject);
            GameManager.GM.pile.RemoveCard(gameObject);
            gameObject.GetComponent<Card>().RemoveBack();
            _alreadyTakeCard = true;
        }
    }

    public void SortCards()
    {
        List<Card> cardsList = new List<Card>();
        foreach (var go in cards)
        {
            cardsList.Add(go.GetComponent<Card>());
        }
        cardsList.Sort();
        cards.Clear();
        foreach (var card in cardsList)
        {
            cards.Add(card.gameObject);
        }
        CalculateCardPosition();
    }
    
}
