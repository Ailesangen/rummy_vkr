using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class DropSlot : MonoBehaviour
{
    [SerializeField] private List<GameObject> cards;
    private float _zOffset = 0.03f;
    public bool isValid = true;
    public Dictionary<char, int> cardPoint = new Dictionary<char, int>()
    {
        {'A', 0}, {'2', 2}, {'3', 3}, {'4', 4}, {'5', 5}, {'6', 6}, {'7', 7}, {'8', 8}, {'9', 9}, {'T', 10}, {'J', 10}, {'Q', 10}, {'K', 10}
    };
   
    public void CalculateCardPosition()
    {
        //Debug.Log("calc pos");
        float zOffset = -1f;
        float xOffset = ( 0.3f);
        float startPos = -cards.Count / 15f;
        foreach (var card in cards)
        {
            //Debug.Log("Drop slot calc pos");
            card.transform.position = new Vector3(transform.position.x + startPos, transform.position.y, transform.position.z + zOffset-3f);
            zOffset -= _zOffset;
            startPos += xOffset;
        }
    }

    public bool NewCardWasAdded()
    {
        foreach (var card in cards)
        {
            if (card.GetComponent<Card>().isDropped == false)
                return true;
        }
        return false;
    }
    
    public int CountPointSlot()
    {   
        int points = 0;
        if (cards.Count != 0)
        {
            bool isSet;
            List<Card> jcards = GameManager.GM.rules.GetCardListWithoutJokers(cards, out isSet);
            if (jcards.Any(c => !c.isDropped))
            {
                int tempPoint = 0;
                //jcard список карт без джокера A 2 3, Q K A, A A A
                //если это сет, то туз стоит 10
                //если это посл., то туз стоит 1 или 15
                if (isSet)
                {
                    for (int i = 0; i < jcards.Count; i++)
                    {
                        tempPoint = 0;
                        cardPoint.TryGetValue(jcards[i].Rank, out tempPoint);
                        points += tempPoint;
                        if (jcards[i].Rank == 'A')
                            points += 10;
                    }
                }
                else
                {
                    for (int i = 0; i < jcards.Count; i++)
                    {
                        tempPoint = 0;
                        cardPoint.TryGetValue(jcards[i].Rank, out tempPoint);
                        points += tempPoint;
                        if (i == 0 && jcards[i].Rank == 'A')
                            points += 1;
                        if (i == jcards.Count - 1 && jcards[i].Rank == 'A')
                            points += 15;
                    }
                }
            }
        }
        return points;
    }
    
    

    public void AddCardToEmtySlotFromPlayer(GameObject card)
    {
        if (GameManager.GM.currentPlayer.AlreadyTakeCard)
        {
            GameManager.GM.currentPlayer.RemoveCard(card);
            cards.Add(card);
            card.transform.parent = transform;
            CalculateCardPosition();
        }
        else
        {
            Debug.Log("send message");
            GameManager.GM.sendMessage.Message("Take card first");
        }
    }

    public void AddCardToCardFromPlayer(GameObject card, GameObject placeCard)
    {
        if (GameManager.GM.currentPlayer.AlreadyTakeCard)
        {    //если игрок
            if (GameManager.GM.currentPlayer.getThirtyPoints == false)    //если игрок не преодолел лимит
            {
                if (placeCard.GetComponent<Card>().isDropped == true)
                {
                    GameManager.GM.sendMessage.Message("Cant add to locked slot with 30 limit");
                }
                else
                {
                    GameManager.GM.currentPlayer.RemoveCard(card);
                    card.transform.parent = transform;
                    cards.Insert(cards.IndexOf(placeCard) + 1, card);
                    CalculateCardPosition();
                }
            }
            else
            {
                GameManager.GM.currentPlayer.RemoveCard(card);
                card.transform.parent = transform;
                cards.Insert(cards.IndexOf(placeCard) + 1, card);
                CalculateCardPosition();
            }
        }
        else
        {
            GameManager.GM.sendMessage.Message("Take card first");
        }
    }

    public void AddCardToEmtySlotFromDrop(GameObject card, GameObject slot)
    {
        card.transform.parent.GetComponent<DropSlot>().RemoveCard(card);
        card.transform.parent = transform;
        slot.GetComponent<DropSlot>().cards.Add(card);
        CalculateCardPosition();
    }
    
    public void AddCardToCardFromDrop(GameObject dragCard, GameObject placeCard)
    {
        //добавляем карту перед другой картой в том же слоте или в другом
        dragCard.transform.parent.GetComponent<DropSlot>().cards.Remove(dragCard);
        dragCard.transform.parent = transform;
        cards.Insert(cards.IndexOf(placeCard)+1, dragCard);
        CalculateCardPosition();
    }
    
    public void ReturnCards()
    {   Player curPlayer = GameManager.GM.currentPlayer;
        //вернуть игроку карты не isDropped
        foreach (var card in cards)
        {
            if (!card.GetComponent<Card>().isDropped)
            {
                card.transform.parent = curPlayer.transform;
                curPlayer.AddCard(card);
            }
        }
        //удалить из списка cards карты isDropped
        for (int i = cards.Count - 1; i >= 0; i--)
        {
            if (!cards[i].GetComponent<Card>().isDropped)
            {
                cards.RemoveAt(i);
            }
        }
        CalculateCardPosition();
    }

    public bool IsAllCardsNewInSlot()
    {
        foreach (var card in cards)
        {
            if (card.GetComponent<Card>().isDropped)
            {
                return false;
            }
        }
        return true;
    }
    public void LockSlot()
    {
        foreach (var card in cards)
        {
            card.GetComponent<Card>().isDropped = true;
        }
    }
    public void RemoveCard(GameObject gameObject)
    {
        cards.Remove(gameObject);
        CalculateCardPosition();
    }

    public bool ValidateSlot()
    {
        if (cards.Count != 0)
        {
            isValid = GameManager.GM.rules.ValidateSlot(cards);
            return isValid;
        }
        else
        {
            isValid = true;
            return isValid;
        }
    }
    
}
