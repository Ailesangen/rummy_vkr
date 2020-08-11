using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Rules : MonoBehaviour
{
    private Drop _drop;
    private List<Card> _cards;
    private char[] _ranks = new char[] {'A', '2', '3', '4', '5', '6', '7', '8', '9', 'T', 'J', 'Q', 'K', 'A'};
    private string _seqString = "A23456789TJQKA";
    public Dictionary<char, int> cardRankPos = new Dictionary<char, int>()
    {
        {'A', 1}, {'2', 2}, {'3', 3}, {'4', 4}, {'5', 5}, {'6', 6}, {'7', 7}, {'8', 8}, {'9', 9}, {'T', 10}, {'J', 11}, {'Q', 12}, {'K', 13}
    };
    private void Awake()
    {
        _drop = GetComponent<Drop>();
    }

    
    public bool ValidateSlot(List<GameObject> cards)
    {
        Debug.Log("Validating slot.......");
        if (cards.Count > 2)
        {
            _cards = MakeCardsFromGameObjects(cards);
            if (TwoJokersNear(_cards))
            {
                Debug.Log("Два джокера подряд в группе");
                return false;
            }
            else
            {
                if (GroupIsPotentialSet(_cards))
                {
                    //проверяем сет
                    if (CheckSet(_cards))
                    {
                        Debug.Log("SET IS CORRECT");
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    //проверяем последовательность
                    if (CheckSeq(_cards))
                    {
                        Debug.Log("SEQUENCE IS CORRECT");
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }
        else
        {
            Debug.Log("Group must have three or more cards");
            return false;
        }
    }


    public List<Card> MakeCardsFromGameObjects(List<GameObject> cards)
    {
        List<Card> cardsInSet = new List<Card>();
        foreach (var go in cards)
        {
            cardsInSet.Add(go.GetComponent<Card>());
        }
        return cardsInSet;
    }

    private bool TwoJokersNear(List<Card> cards)
    {   
        Debug.Log("Two jokers near check");
        for (int i = 0; i < cards.Count-1; i++)
        {
            if (cards[i].IsJoker && cards[i + 1].IsJoker)
                return true;
        }
        return false;
    }

    private bool GroupIsPotentialSet(List<Card> cards)
    {
        //если в списке все ранги одинаковые, то это потенциально сет
        Debug.Log("Check for potential set");
        List<Card> jcards = RemoveJokers(cards);
        for (int i = 0; i < jcards.Count-1; i++)
        {
            if (jcards[i].Rank == jcards[i + 1].Rank)
                return true;
        }
        return false;
    }

    private List<Card> RemoveJokers(List<Card> cards)
    {
        List<Card> jcards = new List<Card>();
        foreach (var card in cards)
        {
            if (!card.IsJoker)
            {
                jcards.Add(card);
            }
        }
        return jcards;
    }

    private bool CheckSet(List<Card> cards)
    {   
        Debug.Log("CheckSet");
        if (cards.Count > 4)
        {
            return false;
        }
        else
        {
            List<Card> set = RemoveJokers(cards);
            List<Char> suit = new List<char>();
            List<Char> rank = new List<char>();
            //нужно проверить, чтобы все ранги были одинаковые, а масти разные
            //QH QD
            
            //проверяем, что масти разные
            for (int i = 0; i < set.Count; i++)
            {
                suit.Add(set[i].Suit);
            }
            bool suitIsUnique = suit.Distinct().Count() == suit.Count();
            
            //проверяем, что ранги одинаковые
            for (int i = 0; i < set.Count; i++)
            {
                rank.Add(set[i].Rank);
            }
            bool rankIsDiff = rank.Any(o => o != rank[0]);
            if (suitIsUnique && !rankIsDiff)
            {
                //тут считаем очки для слота Для set
                return true;
            }
            else
            {
                return false;
            }

        }
        return false;
    } 
    private bool CheckSeq(List<Card> cards)
    {
        Debug.Log("Check seq");
        //если в сете есть джокеры, пытаемся заменить ранг джокера на тот, который он заменяет
        //    если замена есть, проверяем чтобы все ранги отображались в список
        //    если все ранги отображаются, проверяем чтобы у всех была одна масть, кроме карты джокера
        List<Card> jcards = new List<Card>();
        jcards = ReplaceJokersInSeq(cards);
        //char[] charToString = new char[jcards.Count];
        string rankSubString = "";
        string suitSubString = "";
        for (int i = 0; i < jcards.Count; i++)
        {
            Debug.Log(jcards[i].Rank);
            rankSubString += jcards[i].Rank;
            suitSubString += jcards[i].Suit;
        }
        Debug.Log("Substing: " + rankSubString);
        bool isSeq = _seqString.Contains(rankSubString);
        if (isSeq)
        {
            string sb = "";
            //проверяем, что все масти одинаковые
            foreach (var symbol in suitSubString)
            {
                if (symbol != 'X')
                    sb += symbol;
            }

            char checkChar = sb[0];
            foreach (var symbol in sb)
            {
                if (symbol != checkChar)
                    return false;
            }
            return true;

        }
        else
        {
            return false;
        }
    }
    

    private List<Card> ReplaceJokersInSeq(List<Card> cards)
    {
        if (cards.Any(c => c.IsJoker))
        {
            //здесь нет двух подряд идущих джокеров A X 3 X, Q X A, X 2 3, Q K X
            List<Card> set = new List<Card>(cards);
            for (int i = 0; i < set.Count; i++)
            {
                if (set[i].IsJoker)
                {
                    if (i == 0)
                    {
                        //если джокер стоит в начале, смотрим вторую карту и делаем замену ранга по списку
                        //X 2 3, X 5 6....
                        if(set[i + 1].Rank == 'A') continue;
                        int pos = cardRankPos[set[i + 1].Rank];    //позиция второй карты
                        char replaceRank = _ranks[pos-2];
                        set[i].Rank = replaceRank;
                    }

                    if (i == cards.Count - 1)
                    {
                        //если джокер стоит в конце, смотрим предпоследнюю карту
                        //2 3 X, Q K X, K A X
                        if (set[i - 1].Rank == 'A') continue;
                        if (set[i - 1].Rank == 'K')
                        {
                            set[i].Rank = 'A';
                            continue;
                        }
                        int pos = cardRankPos[set[i - 1].Rank];
                        char replaceRank = _ranks[pos];
                        set[i].Rank = replaceRank;
                    }
                    if (i != 0 && i != cards.Count - 1)
                    {
                        //если джокер стоит в серед., делаем замену по предыдущей карте
                        //2 X 4, A X 3, Q X A
                        int pos = cardRankPos[set[i - 1].Rank];
                        char replaceRank = _ranks[pos];
                        set[i].Rank = replaceRank;
                    }
                }
            }
            
            return set;
        }
        else
        {
            return cards;
        }
    }
    
    private List<Card> ReplaceJokersInSet(List<Card> cards)
    {
        //проверяем есть ли в сете джокеры
        if (cards.Any(c => c.IsJoker))
        {
            //находим первую карту не джокера, берем её ранг и заменяем ранг джокера на ранг этой карты
            char rank = 'X';
            List<Card> set = new List<Card>();
            foreach (var card in cards)
            {
                if (!card.IsJoker)
                {
                    rank = card.Rank;
                }
            }
            for (int i = 0; i < cards.Count; i++)
            {
                set.Add(cards[i]);
                if (cards[i].IsJoker)
                {
                    set[i].Rank = rank;
                }
            }
            return set;
        }
        else
        {
            return cards;
        }

    }
    
    public List<Card> GetCardListWithoutJokers(List<GameObject> cards, out bool isSet)
    {
        
        if (GroupIsPotentialSet(_cards))
        {
            isSet = true;
            return ReplaceJokersInSet(MakeCardsFromGameObjects(cards));
        }
        else
        {
            isSet = false;
            return ReplaceJokersInSeq(MakeCardsFromGameObjects(cards));
        }
    }
    
    
}
