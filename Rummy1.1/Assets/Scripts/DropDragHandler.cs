using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private float _startPosX;
    private float _startPosY;
    private bool _isSelected;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (transform.parent.CompareTag("Drop") && !transform.gameObject.GetComponent<Card>().isDropped)
        {
            _isSelected = true;
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _startPosX = mousePosition.x - transform.localPosition.x;
            _startPosY = mousePosition.y - transform.localPosition.y;
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (transform.parent.CompareTag("Drop") && !transform.gameObject.GetComponent<Card>().isDropped)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.localPosition = new Vector3(mousePosition.x - _startPosX, mousePosition.y - _startPosY, -10);
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (transform.parent.CompareTag("Drop") && _isSelected && !transform.gameObject.GetComponent<Card>().isDropped)
        {
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            _isSelected = false;
            GameObject placeObject = GetGameObjectAtMousePosition();
            if (placeObject != null)
            {
                GameManager.GM.eventHandler.DeterminateDropEvent(gameObject,placeObject);

            }
            gameObject.GetComponent<BoxCollider2D>().enabled = true;
        }
        GameManager.GM.drop.CalculatePosAllSlots();
    }
    
    private GameObject GetGameObjectAtMousePosition()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 30f;
        Vector3 startPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        startPoint.z = -30f;
        RaycastHit2D hit = Physics2D.Raycast(startPoint, mousePos - startPoint);
        GameObject objectHit = null;
        if (hit != null && hit.collider != null)
        {
            objectHit = hit.transform.gameObject;
            //Debug.Log("found " + objectHit.transform.name + " at distance: " + hit.distance);
        }
        return objectHit;
    }
}
