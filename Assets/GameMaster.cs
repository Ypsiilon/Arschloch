using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class NewBehaviourScript : MonoBehaviour
{
    public int tournamentCount = 5;
    public int matchCount = 5;

    public int[] points;
    public List<GameObject> deck;
    public List<GameObject> discardPile;
    public GameObject[] aIs;

    public List<GameObject>[] handAI;
    public List<GameObject> tournamentPlayerOrder;
    public List<GameObject> participatingAIsCurrentMatch;
    public List<GameObject> participatingAIsCurrentRound;

    private bool turnZero;

    private GameObject roundWinner;

    public GameObject[] winIndex;

    private List<GameObject> previousTurn;
    private List<GameObject> lastCardsPlayed;
    public GameObject cardPrefab;
    void Start()
    {
        deck = InitDeck();
        lastCardsPlayed = new List<GameObject>();
        previousTurn = new List<GameObject>();
        points = new int[4];
        winIndex = new GameObject[4];

        for (int i = 0; i < 4; i++)
        {
            points[i] = 0;
        }
        handAI = new List<GameObject>[4];
        for (int i = 0; i < 4; i++)
        {
            handAI[i] = new List<GameObject>();
        }
        StartOlympiade();
    }

    // -------------------------------------------------------
    // Olympiade, Tournament, Match, Round, Turn

    public void StartOlympiade()
    {
        for (int i = 0; i < tournamentCount; i++)
        {
            Debug.Log("Tournament " + i + " started.");
            StartTournament();
        }
    }

    public void StartTournament()
    {
        SetNewTournamentPlayerOrder();

        for (int i = 0; i < matchCount; i++)
        {
            Debug.Log("Match " + i + " started.");
            StartMatch();
        }
    }

    public static String GetCardsString(List<GameObject> cards)
    {
        String tmp = "";
        for (int g = 0; g < cards.Count; g++)
        {
            if (cards[g].GetComponent<CardData>().value == 11)
            {
                tmp += "Jack of";
            }
            if (cards[g].GetComponent<CardData>().value == 12)
            {
                tmp += "Queen of";
            }
            if (cards[g].GetComponent<CardData>().value == 13)
            {
                tmp += "King of";
            }
            if (cards[g].GetComponent<CardData>().value == 14)
            {
                tmp += "Ace of";
            }
            if (cards[g].GetComponent<CardData>().value == 15)
            {
                tmp += "Joker";
            }
            else
            {
                tmp += cards[g].GetComponent<CardData>().value.ToString() + " of";
            }
            if (cards[g].GetComponent<CardData>().colour == 0)
            {
                tmp += " Hearts";
            }
            if (cards[g].GetComponent<CardData>().colour == 1)
            {
                tmp += " Diamonds";
            }
            if (cards[g].GetComponent<CardData>().colour == 2)
            {
                tmp += " Clubs";
            }
            if (cards[g].GetComponent<CardData>().colour == 3)
            {
                tmp += " Spades";
            }
        }
        return tmp;
    }

    public void StartMatch()
    {
        // Set Match Participants
        participatingAIsCurrentMatch.AddRange(tournamentPlayerOrder);

        roundWinner = new GameObject(); 

        Debug.Log(GetCardsString(deck));
        deck = ShuffleCards(deck);
        Debug.Log(GetCardsString(deck));
        GiveCards();

        while (participatingAIsCurrentMatch.Count > 1)
        {
            StartRound();
        }

        // Letzter Spieler hat Karten uebrig. Lege diese Karten auf DiscardPile
        for (int i = 0; i < 4; i++)
        {
            if (participatingAIsCurrentMatch[0] == aIs[i])
            {
                discardPile.AddRange(handAI[i]);
                handAI[i].Clear();

                Debug.Log("Last player returned his cards to the discardPile.");
            }
        }
        ResetMatchVariables();
    }

    public void StartRound()
    {
        turnZero = true;
        int w = 0;
        participatingAIsCurrentRound.Clear();
        participatingAIsCurrentRound.AddRange(participatingAIsCurrentMatch);
        
        if (roundWinner!=null)
        { 
            for(int i = 0;i < 4;i++) 
            {
                if (roundWinner == participatingAIsCurrentRound[i])
                {
                    w = i;
                }
            }
        }
        int r = 0;

        while (participatingAIsCurrentRound.Count > 1 && r < 50 )
        {
            int tempCount = participatingAIsCurrentRound.Count;

            StartTurn(w);
            turnZero = false;

            if (tempCount == participatingAIsCurrentRound.Count)
            {
                w++;
            }
            r++;
            w = w % participatingAIsCurrentRound.Count;
            
        }
        // bestimme roundWinner
        roundWinner = participatingAIsCurrentRound[0];
        Debug.Log("RoundWinner is: " + roundWinner.name);

    }

    public void GivePoints()
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (winIndex[i] == aIs[j])
                {
                    points[j] += 3 - i;
                    Debug.Log("Rundenposition" + i + 1 + "hat AI" + j + "belegt");

                }

            }

        }
        winIndex = new GameObject[4];
    }

    public void StartTurn(int aIIndex)
    {
        GameObject currentPlayer = participatingAIsCurrentRound[aIIndex];

        if (lastCardsPlayed.Count != 0)
        {
            previousTurn.Clear();
            previousTurn.AddRange(lastCardsPlayed);
        }
        if (currentPlayer.CompareTag("0"))
        {
            lastCardsPlayed.Clear();
            lastCardsPlayed.AddRange(currentPlayer.GetComponent<BasicAI>().PlayCards(turnZero, discardPile, previousTurn, tournamentPlayerOrder, participatingAIsCurrentMatch, participatingAIsCurrentRound));
            for (int i = 0; i < lastCardsPlayed.Count; i++)
            {
                handAI[0].Remove(lastCardsPlayed[i]);
            }
            if (handAI[0].Count == 0)
            {
                participatingAIsCurrentMatch.Remove(currentPlayer);
                participatingAIsCurrentRound.Remove(currentPlayer);
                Debug.Log("Player 0 finished");
                for (int k = 0; k < winIndex.Length; k++)
                {
                    if (winIndex[k] == null)
                    {
                        winIndex[k] = currentPlayer;
                        break;
                    }
                }
            }
            if (lastCardsPlayed.Count == 0)
            {
                participatingAIsCurrentRound.Remove(currentPlayer);
                Debug.Log("Player 0 passed");
            }
            else
            {
                String play = "";
                String tmp = "";
                for (int g = 0; g < lastCardsPlayed.Count; g++)
                {
                    if (lastCardsPlayed[g].GetComponent<CardData>().value == 11)
                    {
                        tmp += "Jack of";
                    }
                    if (lastCardsPlayed[g].GetComponent<CardData>().value == 12)
                    {
                        tmp += "Queen of";
                    }
                    if (lastCardsPlayed[g].GetComponent<CardData>().value == 13)
                    {
                        tmp += "King of";
                    }
                    if (lastCardsPlayed[g].GetComponent<CardData>().value == 14)
                    {
                        tmp += "Ace of";
                    }
                    if (lastCardsPlayed[g].GetComponent<CardData>().value == 15)
                    {
                        tmp += "Joker";
                    }
                    else
                    {
                        tmp += lastCardsPlayed[g].GetComponent<CardData>().value.ToString();
                    }
                    if (lastCardsPlayed[g].GetComponent<CardData>().colour == 0)
                    {
                        tmp += "Hearts";
                    }
                    if (lastCardsPlayed[g].GetComponent<CardData>().colour == 1)
                    {
                        tmp += "Diamonds";
                    }
                    if (lastCardsPlayed[g].GetComponent<CardData>().colour == 2)
                    {
                        tmp += "Clubs";
                    }
                    if (lastCardsPlayed[g].GetComponent<CardData>().colour == 3)
                    {
                        tmp += "Spades";
                    }
                    play = tmp;
                }
                Debug.Log("Player 0 played: " + play);
            }
        }
        else if (currentPlayer.CompareTag("1"))
        {
            lastCardsPlayed.Clear();
            lastCardsPlayed.AddRange(currentPlayer.GetComponent<BasicAI>().PlayCards(turnZero, discardPile, previousTurn, tournamentPlayerOrder, participatingAIsCurrentMatch, participatingAIsCurrentRound));
            for (int i = 0; i < lastCardsPlayed.Count; i++)
            {
                handAI[1].Remove(lastCardsPlayed[i]);
            }
            if (handAI[1].Count == 0)
            {
                participatingAIsCurrentMatch.Remove(currentPlayer);
                participatingAIsCurrentRound.Remove(currentPlayer);
                Debug.Log("Player 1 finished");
                for (int k = 0; k < winIndex.Length; k++)
                {
                    if (winIndex[k] == null)
                    {
                        winIndex[k] = currentPlayer;
                        break;
                    }
                }
            }
            if (lastCardsPlayed.Count == 0)
            {
                participatingAIsCurrentRound.Remove(currentPlayer);
                Debug.Log("Player 1 passed");
            }
            else
            {
                String play = "";
                String tmp = "";
                for (int g = 0; g < lastCardsPlayed.Count; g++)
                {
                    if (lastCardsPlayed[g].GetComponent<CardData>().value == 11)
                    {
                        tmp += "Jack of";
                    }
                    if (lastCardsPlayed[g].GetComponent<CardData>().value == 12)
                    {
                        tmp += "Queen of";
                    }
                    if (lastCardsPlayed[g].GetComponent<CardData>().value == 13)
                    {
                        tmp += "King of";
                    }
                    if (lastCardsPlayed[g].GetComponent<CardData>().value == 14)
                    {
                        tmp += "Ace of";
                    }
                    if (lastCardsPlayed[g].GetComponent<CardData>().value == 15)
                    {
                        tmp += "Joker";
                    }
                    else
                    {
                        tmp += lastCardsPlayed[g].GetComponent<CardData>().value.ToString();
                    }
                    if (lastCardsPlayed[g].GetComponent<CardData>().colour == 0)
                    {
                        tmp += "Hearts";
                    }
                    if (lastCardsPlayed[g].GetComponent<CardData>().colour == 1)
                    {
                        tmp += "Diamonds";
                    }
                    if (lastCardsPlayed[g].GetComponent<CardData>().colour == 2)
                    {
                        tmp += "Clubs";
                    }
                    if (lastCardsPlayed[g].GetComponent<CardData>().colour == 3)
                    {
                        tmp += "Spades";
                    }
                    play = tmp;
                }
                Debug.Log("Player 1 played: " + play);
            }
        }
        else if (currentPlayer.CompareTag("2"))
        {
            lastCardsPlayed.Clear();
            lastCardsPlayed.AddRange(currentPlayer.GetComponent<BasicAI>().PlayCards(turnZero, discardPile, previousTurn, tournamentPlayerOrder, participatingAIsCurrentMatch, participatingAIsCurrentRound));
            for (int i = 0; i < lastCardsPlayed.Count; i++)
            {
                handAI[2].Remove(lastCardsPlayed[i]);
            }
            if (handAI[2].Count == 0)
            {
                participatingAIsCurrentMatch.Remove(currentPlayer);
                participatingAIsCurrentRound.Remove(currentPlayer);
                Debug.Log("Player 2 finished");
                for (int k = 0; k < winIndex.Length; k++)
                {
                    if (winIndex[k] == null)
                    {
                        winIndex[k] = currentPlayer;
                        break;
                    }
                }
            }
            if (lastCardsPlayed.Count == 0)
            {
                participatingAIsCurrentRound.Remove(currentPlayer);
                Debug.Log("Player 2 passed");
            }
            else
            {
                String play = "";
                String tmp = "";
                for (int g = 0; g < lastCardsPlayed.Count; g++)
                {
                    if (lastCardsPlayed[g].GetComponent<CardData>().value == 11)
                    {
                        tmp += "Jack of";
                    }
                    if (lastCardsPlayed[g].GetComponent<CardData>().value == 12)
                    {
                        tmp += "Queen of";
                    }
                    if (lastCardsPlayed[g].GetComponent<CardData>().value == 13)
                    {
                        tmp += "King of";
                    }
                    if (lastCardsPlayed[g].GetComponent<CardData>().value == 14)
                    {
                        tmp += "Ace of";
                    }
                    if (lastCardsPlayed[g].GetComponent<CardData>().value == 15)
                    {
                        tmp += "Joker";
                    }
                    else
                    {
                        tmp += lastCardsPlayed[g].GetComponent<CardData>().value.ToString();
                    }
                    if (lastCardsPlayed[g].GetComponent<CardData>().colour == 0)
                    {
                        tmp += "Hearts";
                    }
                    if (lastCardsPlayed[g].GetComponent<CardData>().colour == 1)
                    {
                        tmp += "Diamonds";
                    }
                    if (lastCardsPlayed[g].GetComponent<CardData>().colour == 2)
                    {
                        tmp += "Clubs";
                    }
                    if (lastCardsPlayed[g].GetComponent<CardData>().colour == 3)
                    {
                        tmp += "Spades";
                    }
                    play = tmp;
                }
                Debug.Log("Player 2 played: " + play);
            }
        }
        else if (currentPlayer.CompareTag("3"))
        {
            lastCardsPlayed.Clear();
            lastCardsPlayed.AddRange(currentPlayer.GetComponent<BasicAI>().PlayCards(turnZero, discardPile, previousTurn, tournamentPlayerOrder, participatingAIsCurrentMatch, participatingAIsCurrentRound));
            for (int i = 0; i < lastCardsPlayed.Count; i++)
            {
                handAI[3].Remove(lastCardsPlayed[i]);
            }
            if (handAI[3].Count == 0)
            {
                participatingAIsCurrentMatch.Remove(currentPlayer);
                participatingAIsCurrentRound.Remove(currentPlayer);
                Debug.Log("Player 3 finished");
                for (int k = 0; k < winIndex.Length; k++)
                {
                    if (winIndex[k] == null)
                    {
                        winIndex[k] = currentPlayer;
                        break;
                    }
                }
            }
            if (lastCardsPlayed.Count == 0)
            {
                participatingAIsCurrentRound.Remove(currentPlayer);
                Debug.Log("Player 3 passed");
            }
            else
            {
                String play = "";
                String tmp = "";
                for (int g = 0; g < lastCardsPlayed.Count; g++)
                {
                    if (lastCardsPlayed[g].GetComponent<CardData>().value == 11)
                    {
                        tmp += "Jack of ";
                    }
                    if (lastCardsPlayed[g].GetComponent<CardData>().value == 12)
                    {
                        tmp += "Queen of ";
                    }
                    if (lastCardsPlayed[g].GetComponent<CardData>().value == 13)
                    {
                        tmp += "King of ";
                    }
                    if (lastCardsPlayed[g].GetComponent<CardData>().value == 14)
                    {
                        tmp += "Ace of ";
                    }
                    if (lastCardsPlayed[g].GetComponent<CardData>().value == 15)
                    {
                        tmp += "Joker";
                    }
                    else
                    {
                        tmp += lastCardsPlayed[g].GetComponent<CardData>().value.ToString() + " of ";
                    }
                    if (lastCardsPlayed[g].GetComponent<CardData>().colour == 0)
                    {
                        tmp += "Hearts";
                    }
                    if (lastCardsPlayed[g].GetComponent<CardData>().colour == 1)
                    {
                        tmp += "Diamonds";
                    }
                    if (lastCardsPlayed[g].GetComponent<CardData>().colour == 2)
                    {
                        tmp += "Clubs";
                    }
                    if (lastCardsPlayed[g].GetComponent<CardData>().colour == 3)
                    {
                        tmp += "Spades";
                    }
                    play = tmp;
                }
                Debug.Log("Player 3 played: " + play);
            }
        }
    }

    // -------------------------------------------------------
    // Basale Funktionen innerhalb eines Matches 


    public void ResetMatchVariables()
    {
        // Check for CurrentAIs in Match
        if (participatingAIsCurrentMatch.Count != 0)
        {
            Debug.Log("Oops. Something went wrong. The Matc is over, but there are players left");
        }

        // Check for HandsEmpty
        for (int i = 0; i < 4; i++)
        {
            if (handAI[i].Count != 0)
            {
                Debug.Log("Oooops. Something went wrong. Player " + i + " has " + handAI[i].Count + " cards in its hand, where it should have zero cards  in its hand");
            }
        }

        // Reset Deck
        if (deck.Count != 0)
        {
            Debug.Log("Oooops. Something went wrong. The deck has not zero cards, where it should have zero cards");
        }
        else if (discardPile.Count != 55)
        {
            Debug.Log("Oooops. Something went wrong. The discardPile has not 55 cards, where it should have 55 cards");
        }
        else if (deck.Count == 0 && discardPile.Count == 55)
        {
            deck.AddRange(discardPile);
            discardPile.Clear();

            Debug.Log("DiscardPile was turned into new deck");
        }

        // Reset 
        participatingAIsCurrentMatch.Clear();
        participatingAIsCurrentRound.Clear();

    }

    public void SetNewTournamentPlayerOrder()
{
        int[] permutation = new int[4];
        permutation[0]=0;
        permutation[1]=1;
        permutation[2]=2;
        permutation[3]=3;

        for(int i = 0; i < Random.Range(15,20); i++) 
        {
            int a =Random.Range(0,4);
            int b=Random.Range(0,4);
            (permutation[a],permutation[b])=(permutation[b],permutation[a]);
        }
        
        tournamentPlayerOrder.Add(aIs[permutation[0]]);
        tournamentPlayerOrder.Add(aIs[permutation[1]]);
        tournamentPlayerOrder.Add(aIs[permutation[2]]);
        tournamentPlayerOrder.Add(aIs[permutation[3]]);

        Debug.Log("Tournament Player Order shuffeled (shown in next 4 debuglogs):");
        Debug.Log(tournamentPlayerOrder[0].name);
        Debug.Log(tournamentPlayerOrder[1].name);
        Debug.Log(tournamentPlayerOrder[2].name);
        Debug.Log(tournamentPlayerOrder[3].name);


    }

    public List<GameObject> ShuffleCards(List<GameObject> deck)
    {
        int i = 0;
        int t = deck.Count;
        int r = 0;
        GameObject p = null;
        List<GameObject> tempList = new List<GameObject>();
        tempList.AddRange(deck);
        while (i < t)
        {
            r = Random.Range(i, tempList.Count);
            p = tempList[i];
            tempList[i] = tempList[r];
            tempList[r] = p;
            i++;
        }
        return tempList;
    }
    public List<GameObject> InitDeck()
    {
        List<GameObject> deck = new List<GameObject>();
        for (int colour = 0; colour < 4; colour++)
        {
            for (int value = 2; value < 15; value++)
            {
                GameObject card = Instantiate(cardPrefab);
                card.GetComponent<CardData>().setColour(colour);
                card.GetComponent<CardData>().setValue(value);
                deck.Add(card);
            }
        }
        for (int joker = 0; joker < 4; joker++)
        {
            GameObject card = Instantiate(cardPrefab);
            card.GetComponent<CardData>().setColour(4);
            card.GetComponent<CardData>().setValue(15);
            deck.Add(card);
        }
        return deck;
    }
    public void GiveCards()
    {
        for (int i = 0; i < 4; i++)
        {
            if (aIs[i].CompareTag("0"))
            {
                aIs[i].GetComponent<BasicAI>().PreStart();
            }
            else if (aIs[i].CompareTag("1"))
            {
                aIs[i].GetComponent<BasicAI>().PreStart();
            }
            else if (aIs[i].CompareTag("2"))
            {
                aIs[i].GetComponent<BasicAI>().PreStart();
            }
            else if (aIs[i].CompareTag("3"))
            {
                aIs[i].GetComponent<BasicAI>().PreStart();
            }
            for (int j = 0; j < 13; j++)
            {
                handAI[i].Add(deck[0]);
                deck.RemoveAt(0);
            }
            if (aIs[i].CompareTag("0"))
            {
                aIs[i].GetComponent<BasicAI>().ReceiveCards(handAI[i]);
            }
            else if (aIs[i].CompareTag("1"))
            {
                aIs[i].GetComponent<BasicAI>().ReceiveCards(handAI[i]);
            }
            else if (aIs[i].CompareTag("2"))
            {
                aIs[i].GetComponent<BasicAI>().ReceiveCards(handAI[i]);
            }
            else if (aIs[i].CompareTag("3"))
            {
                aIs[i].GetComponent<BasicAI>().ReceiveCards(handAI[i]);
            }
        }
    }
    public void SwapCards()
    {

    }

}
