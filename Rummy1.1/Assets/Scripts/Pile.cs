using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Pile : MonoBehaviour
{
    [SerializeField] private List<GameObject> cards;
    private float _zOffset = 0.03f;
    private int losePoints = 0;
    
    public void CalculateCardPosition()
    {
        float zOffset = 17f;
        foreach (var card in cards)
        {
            card.transform.position = new Vector3(transform.position.x, transform.position.y, zOffset);
            zOffset -= _zOffset;
        }
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

    public void AddCardFromPlayer(GameObject _gameObject)
    {
        if (GameManager.GM.currentPlayer.AlreadyTakeCard)
        {
            if (GameManager.GM.drop.ValidateSlots())
            {
                Debug.Log("Points in slot: " + GameManager.GM.drop.GetPointsFromSlots());
                if (GameManager.GM.currentPlayer.getThirtyPoints == false && GameManager.GM.drop.IsNewCardAdded())
                {
                    if (GameManager.GM.drop.GetPointsFromSlots() >= 30)
                    {
                        GameManager.GM.currentPlayer.getThirtyPoints = true;
                        GameManager.GM.sendMessage.Message("Get thirty limit");
                        PutCardToPile(_gameObject);
                    }
                    else
                    {
                        GameManager.GM.sendMessage.Message("Group must have at least 30 points");
                    }
                }
                else
                {
                    PutCardToPile(_gameObject);
                }
            }
            else
            {
                Debug.Log("Invalid group in slot");
                GameManager.GM.sendMessage.Message("Incorrect group");
            }
        }
        else
        {
            Debug.Log("First take card");
        }
    }

    private void PutCardToPile(GameObject _gameObject)
    {
        _gameObject.transform.parent = transform;
        cards.Add(_gameObject);
        GameManager.GM.currentPlayer.RemoveCard(_gameObject);
        if (GameManager.GM.currentPlayer.IsEmtyHand())
        {
            GameManager.GM.currentPlayer.WinEvent();
            Debug.Log("WIN");
        }
        GameManager.GM.drop.LockSlots();
        
        //здесь будет проверка на количество карт на руке у игрока, если 0 то победа
        GameManager.GM.NextTurn();
        CalculateCardPosition();
    }
}
