using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAI : MonoBehaviour
{
    private List<GameObject> handCards;
    public List<GameObject> swappedCards;
    public int[] winIndex;

    private bool played;

    private List<GameObject> sortedHandCards;
    private List<GameObject> cardsToPlay;


    void Start()
    {
        
    }
    public void PreStart()
    {
        handCards = new List<GameObject>();
        swappedCards = new List<GameObject>();
        cardsToPlay = new List<GameObject>();
        sortedHandCards =  new List<GameObject>();
    }
    public void ReceiveCards(List<GameObject> cards) 
    {
        handCards.Clear();
        handCards.AddRange(cards);
        JokerToAce();
        SortHandCards();
    }
    public List<GameObject> PlayCards(bool turnZero, List<GameObject> discardPile, List<GameObject> previousTurn, List<GameObject> tournamentPlayerOrder,List<GameObject> participatingAIsCurrentMatch, List<GameObject> participatingAIsCurrentRound)
    {
        played=false;
        cardsToPlay.Clear();

        if(turnZero == true)
        {
            int count=0;
            int lowestValue=0;

            lowestValue=sortedHandCards[0].GetComponent<CardData>().value;
            int i = 0;
            while(sortedHandCards[i].GetComponent<CardData>().value == lowestValue)
            {
                count++;
                i++;
            }
            for (int j =0 ;j < count ;j++)
            {
            cardsToPlay.Add(sortedHandCards[j]);
            }

            for (int j =0 ;j <= count ;j++)
            {
            sortedHandCards.RemoveAt(0);
            }
        }

        else
        {
            int valuePlayed;
            int previousTurnCartsCounter;

            previousTurnCartsCounter=previousTurn.Count;
            valuePlayed=previousTurn[0].GetComponent<CardData>().value;

            for(int i = 0; i < sortedHandCards.Count-previousTurn.Count+1; i++) 
            {
                if(sortedHandCards[i].GetComponent<CardData>().value >= valuePlayed)
                {
                   if(sortedHandCards[i].GetComponent<CardData>().value==sortedHandCards[i+previousTurn.Count-1].GetComponent<CardData>().value)
                   {
                            for (int j =i ;j <= i+previousTurn.Count-1 ;j++)
                            {
                                cardsToPlay.Add(sortedHandCards[j]);
                                played = true;
                            }

                            for (int j =i ;j <= i+previousTurn.Count-1 ;j++)
                            {
                                sortedHandCards.RemoveAt(i);
                            }

                            if(played)
                            {
                                return cardsToPlay;
                            }
                   }     
                }    

            }
        }

        return cardsToPlay;
    }

    private void SortHandCards()
    {
        sortedHandCards.Clear();

        for(int i=2; i < 16; i++)
        {
            foreach (GameObject handCard in handCards)
            {
                if(handCard.GetComponent<CardData>().value == i)
                {
                    sortedHandCards.Add(handCard);
                }

            }
        }
    }

    private void JokerToAce()
    {
         foreach (GameObject handCard in handCards)
            {
                if(handCard.GetComponent<CardData>().value == 15)
                {
                    handCard.GetComponent<CardData>().value = 14;
                }

            }

    }





}
