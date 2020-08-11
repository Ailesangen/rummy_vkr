using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    private float startPosX;
    private float startPosY;
    private bool _isSelected;
    private Player player;     //тот игрок, чью карту перетаскиваем
    //temp
    private Vector3 hitpoint;
    
    
    public bool IsSelected
    {
        get => _isSelected;
        set => _isSelected = value;
    }
    
    //перетаскивание должно работать только для карт игрока
    //карту с кучи и колоды берем по клику
    //карту в кучу и сброс кладем перетаскиванием
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (gameObject.transform.parent == GameManager.GM.currentPlayer.transform && !gameObject.transform.parent.CompareTag("Drop"))
        {
            _isSelected = true;
            player = transform.parent.GetComponent<Player>();
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            startPosX = mousePosition.x - transform.localPosition.x;
            startPosY = mousePosition.y - transform.localPosition.y;
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (gameObject.transform.parent == GameManager.GM.currentPlayer.transform && !gameObject.transform.parent.CompareTag("Drop"))
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.localPosition = new Vector3(mousePosition.x - startPosX, mousePosition.y - startPosY, 0);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (gameObject.transform.parent == GameManager.GM.currentPlayer.transform && !gameObject.transform.parent.CompareTag("Drop"))
        {
            _isSelected = false;
            GameObject placeObject = GetGameObjectAtMousePosition();
            if (placeObject != null)
            {
                GameManager.GM.eventHandler.DeterminateDragEvent(gameObject, placeObject);
            }

            player.CalculateCardPosition();
            if (player != null)
            {
                player = null;
            }
            gameObject.GetComponent<BoxCollider2D>().enabled = true;
        }

    }
    
    private GameObject GetGameObjectAtMousePosition()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 30f;
        Vector3 startPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        startPoint.z = -30f;
        RaycastHit2D hit = Physics2D.Raycast(startPoint, mousePos - startPoint);
        hitpoint = hit.point;
        GameObject objectHit = null;
        if (hit != null && hit.collider != null)
        {
            objectHit = hit.transform.gameObject;
            //Debug.Log("found " + objectHit.transform.name + " at distance: " + hit.distance);
        }

        return objectHit;
    }

    
    
    
    private void OnDrawGizmos()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 30f;
        Vector3 startPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        startPoint.z = -30f;
        Debug.DrawRay(startPoint, mousePos - startPoint, Color.magenta);
        
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(hitpoint, 0.2f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GameManager.GM.eventHandler.DeterminateClickEvent(gameObject);
    }
}
