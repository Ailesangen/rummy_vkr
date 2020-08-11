using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Drop : MonoBehaviour
{
    [SerializeField] private  Transform[] _slots;
    [SerializeField] private List<DropSlot> _dropSlots;
    [SerializeField] private  GameObject _cancelButton;
    void Awake()
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            _dropSlots.Add(_slots[i].GetComponent<DropSlot>());
        }

        //_cancelButton.SetActive(false);
    }
    
    public void CancelDrop()
    {
        foreach (var slot in _dropSlots)
        {
            slot.ReturnCards();
        }
    }

    public bool IsAllCardsNewInSlots()
    {
        
        foreach (var slot in _dropSlots)
        {
            if (!slot.IsAllCardsNewInSlot())
            {
                return false;
            }
        }
        return true;
    }
    
    public void LockSlots()
    {
        foreach (var slot in _dropSlots)
        {
            slot.LockSlot();
        }
    }
    public void CalculatePosAllSlots()
    {
        foreach (var slot in _dropSlots)
        {
            slot.CalculateCardPosition();
        }
    }

    public bool ValidateSlots()
    {
        foreach (var slot in _dropSlots)
        {
            if (!slot.ValidateSlot())
            {
                return false;
            }
        }
        return true;
    }

    public void DebugValidateSlots()
    {
        foreach (var slot in _dropSlots)
        {
            slot.ValidateSlot();
        }
    }

    public int GetPointsFromSlots()
    {
        int points = 0;
        foreach (var slot in _dropSlots)
        {
            points+=slot.CountPointSlot();
        }

        return points;
    }

    public bool IsNewCardAdded()
    {
        foreach (var slot in _dropSlots)
        {
            if (slot.NewCardWasAdded())
            {
                return true;
            }
        }

        return false;
    }
}
