using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventHandler : MonoBehaviour
{
    

    
    public void DeterminateDragEvent(GameObject draggedCard, GameObject placeObject)
    {
        if (CheckCardOwner(placeObject))    //если игрок кладет карту себе обратно в руку
        {
            //Debug.Log("Insert Card " + draggedCard.name + " to " + placeObject.name);
            GameManager.GM.currentPlayer.InsertCardBehind(draggedCard,placeObject);
            
        }

        if (placeObject.transform.parent == null) //если пустая куча или сброс
        {
            if (placeObject.transform.CompareTag("Pile"))
            {
                //Debug.Log("Drag to empty pile");
                GameManager.GM.pile.AddCardFromPlayer(draggedCard);
                
            }
        }
        else     //если это не пустая куча или сброс
        {
            if (placeObject.transform.parent.CompareTag("Pile"))
            {
                //Debug.Log("Drag to pile");
                GameManager.GM.pile.AddCardFromPlayer(draggedCard);
            }
            if (placeObject.transform.CompareTag("Drop"))    //сброс, в пустой слот
            {
                //Debug.Log("Drag to drop slot");
                placeObject.GetComponent<DropSlot>().AddCardToEmtySlotFromPlayer(draggedCard);
            }

            if (placeObject.transform.parent.CompareTag("Drop"))    //сброс, добавляем к карте
            {
                //Debug.Log("Drag to drop slot with card");
                
                placeObject.GetComponentInParent<DropSlot>().AddCardToCardFromPlayer(draggedCard, placeObject);
            }
        }
    }

    public void DeterminateClickEvent(GameObject _gameObject)
    {
        if (!CheckCardOwner(_gameObject))
        {
            if (_gameObject.transform.parent.CompareTag("Deck"))
            {
                //Debug.Log("Click on deck");
                GameManager.GM.currentPlayer.TakeCardFromDeck(_gameObject);
            }

            if (_gameObject.transform.parent.CompareTag("Pile"))
            {
                //Debug.Log("Click on pile");
                GameManager.GM.currentPlayer.TakeCardFromPile(_gameObject);
            }

            if (_gameObject.transform.CompareTag("Pile"))
            {
                //Debug.Log("Click on empty pile");
            }
        }
    }
    
    private bool CheckCardOwner(GameObject _gameObject)
    {
        if (_gameObject.transform.parent == GameManager.GM.currentPlayer.transform)
            return true;
        else
        {
            return false;
        }
    }

    public void DeterminateDropEvent(GameObject draggedCard, GameObject placeObject)
    {
        if (placeObject.transform.CompareTag("Drop"))
        {
            placeObject.GetComponent<DropSlot>().AddCardToEmtySlotFromDrop(draggedCard,placeObject);
        }
        if (placeObject.transform.parent.CompareTag("Drop"))
        {
            placeObject.transform.parent.GetComponent<DropSlot>().AddCardToCardFromDrop(draggedCard,placeObject);
        }

        if (placeObject.transform.parent.CompareTag("PlayerOne") || placeObject.transform.parent.CompareTag("PlayerTwo"))
        {
            Debug.Log("Drag card from drop to player");
        }
        
    }

    
    
    
}
