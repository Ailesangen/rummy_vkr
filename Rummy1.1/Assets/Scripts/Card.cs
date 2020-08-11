using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour, IComparable<Card>
{
    private char _suit;
    public char Suit
    {
        get => _suit;
        set => _suit = value;
    }
    
    private char _rank;
    public char Rank
    {
        get => _rank;
        set => _rank = value;
    }

    private bool _isJoker = false;
    public bool IsJoker
    {
        get => _isJoker;
        set => _isJoker = value;
    }

    public bool isDropped = false;
    private CardDragHandler _cardDragHandler;
    private SpriteRenderer _spriteRenderer;
    private Transform _transform;
    public void SetCard(char suit, char rank, bool isJoker = false)
    {
        _suit = suit;
        _rank = rank;
        _isJoker = isJoker;
    }

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _transform = GetComponent<Transform>();
        _cardDragHandler = GetComponent<CardDragHandler>();
    }
    
    public void RemoveBack()
    {
        if (_isJoker)
        {
            Sprite sprite = Resources.Load<Sprite>("cards/XX");
            _spriteRenderer.sprite = sprite;
        }
        else
        {
            Sprite sprite = Resources.Load<Sprite>("cards/" + _suit + _rank);
            _spriteRenderer.sprite = sprite;
        }
        
    }

    public int CompareTo(Card other)
    {
        if (this._rank < other._rank) return -1;
        else if (this._rank > other._rank) return 1;
        else return 0;
    }

    
}
