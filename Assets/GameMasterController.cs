using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameMasterController : MonoBehaviour
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
    private bool matchZero;
    private int matchIndex;

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
        for (int i = 0; i < 4; i++)
        {
            Debug.Log("AI " + i + " got " + points[i] + " Points!");
        }
    }

    public void StartTournament()
    {
        SetNewTournamentPlayerOrder();
        matchZero = true;
        for (int l = 0; l < matchCount; l++)
        {
            Debug.Log("Match  " + l + " started.");
            StartMatch();
            matchIndex++;
        }
    }


    public void StartMatch()
    {
        // Set Match Participants
        participatingAIsCurrentMatch.AddRange(tournamentPlayerOrder);

        roundWinner = null;

        Debug.Log(GetCardsString(deck));
        deck = ShuffleCards(deck);
        Debug.Log(GetCardsString(deck));
        GiveCards();
        matchZero = true;
        while (participatingAIsCurrentMatch.Count > 1)
        {
            StartRound();
            matchZero = false;
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
        Debug.Log("deck:" + deck.Count);
        Debug.Log("discardPile: " + discardPile.Count);
        GivePoints();
        ResetMatchVariables();
    }

    public void StartRound()
    {

        turnZero = true;
        int w = 0;
        participatingAIsCurrentRound.Clear();
        participatingAIsCurrentRound.AddRange(participatingAIsCurrentMatch);

        if (roundWinner != null)
        {
            for (int i = 0; i < participatingAIsCurrentRound.Count; i++)
            {
                if (roundWinner == participatingAIsCurrentRound[i])
                {
                    w = i;
                }
            }
        }

        while (participatingAIsCurrentRound.Count > 1)
        {
            int tempCount = participatingAIsCurrentRound.Count;

            StartTurn(w);
            turnZero = false;

            if (tempCount == participatingAIsCurrentRound.Count)
            {
                w++;
            }
            w = w % participatingAIsCurrentRound.Count;

        }
        // bestimme roundWinner
        roundWinner = participatingAIsCurrentRound[0];
        Debug.Log("RoundWinner is: " + roundWinner.name);
        for (int i = 0; i < handAI.Length; i++)
        {
            //Debug.Log("Player " + i + "s Hand: " + GetCardsString(handAI[i]));
        }

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
            Debug.Log("AI0 Hand Cards" + gameObject.GetComponent<GameMasterController>().GetCardsString(currentPlayer.GetComponent<BasicAI>().sortedHandCards));
            Debug.Log("Player " + 0 + "s Hand: " + GetCardsString(handAI[0]));
            lastCardsPlayed.AddRange(currentPlayer.GetComponent<BasicAI>().PlayCards(turnZero, discardPile, previousTurn, tournamentPlayerOrder, participatingAIsCurrentMatch, participatingAIsCurrentRound));
            for (int i = 0; i < lastCardsPlayed.Count; i++)
            {
                handAI[0].Remove(lastCardsPlayed[i]);
            }
            if (lastCardsPlayed.Count == 0)
            {
                participatingAIsCurrentRound.Remove(currentPlayer);
                Debug.Log("Player 0 passed");
            }
            else
            {
                String play = GetCardsString(lastCardsPlayed);
                Debug.Log("Player 0 played: " + play);
            }
        }
        else if (currentPlayer.CompareTag("1"))
        {
            lastCardsPlayed.Clear();
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
            lastCardsPlayed.AddRange(currentPlayer.GetComponent<BasicAI>().PlayCards(turnZero, discardPile, previousTurn, tournamentPlayerOrder, participatingAIsCurrentMatch, participatingAIsCurrentRound));
            for (int i = 0; i < lastCardsPlayed.Count; i++)
            {
                handAI[1].Remove(lastCardsPlayed[i]);
            }
            if (lastCardsPlayed.Count == 0)
            {
                participatingAIsCurrentRound.Remove(currentPlayer);
                Debug.Log("Player 1 passed");
            }
            else
            {
                String play = GetCardsString(lastCardsPlayed);
                Debug.Log("Player 1 played: " + play);
            }
        }
        else if (currentPlayer.CompareTag("2"))
        {
            lastCardsPlayed.Clear();
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
            lastCardsPlayed.AddRange(currentPlayer.GetComponent<BasicAI>().PlayCards(turnZero, discardPile, previousTurn, tournamentPlayerOrder, participatingAIsCurrentMatch, participatingAIsCurrentRound));
            for (int i = 0; i < lastCardsPlayed.Count; i++)
            {
                handAI[2].Remove(lastCardsPlayed[i]);
            }
            if (lastCardsPlayed.Count == 0)
            {
                participatingAIsCurrentRound.Remove(currentPlayer);
                Debug.Log("Player 2 passed");
            }
            else
            {
                String play = GetCardsString(lastCardsPlayed);
                Debug.Log("Player 2 played: " + play);
            }
        }
        else if (currentPlayer.CompareTag("3"))
        {
            lastCardsPlayed.Clear();
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
            lastCardsPlayed.AddRange(currentPlayer.GetComponent<BasicAI>().PlayCards(turnZero, discardPile, previousTurn, tournamentPlayerOrder, participatingAIsCurrentMatch, participatingAIsCurrentRound));
            for (int i = 0; i < lastCardsPlayed.Count; i++)
            {
                handAI[3].Remove(lastCardsPlayed[i]);
            }
            if (lastCardsPlayed.Count == 0)
            {
                participatingAIsCurrentRound.Remove(currentPlayer);
                Debug.Log("Player 3 passed");
            }
            else
            {
                String play = GetCardsString(lastCardsPlayed);
                Debug.Log("Player 3 played: " + play);
            }
        }
        discardPile.AddRange(lastCardsPlayed);
    }

    // -------------------------------------------------------
    // Basale Funktionen innerhalb eines Matches

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
    public String GetCardsString(List<GameObject> cards)
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
                tmp += "Joker\n";
            }
            else
            {
                tmp += cards[g].GetComponent<CardData>().value.ToString() + " of";
            }
            if (cards[g].GetComponent<CardData>().colour == 0)
            {
                tmp += " Hearts\n";
            }
            if (cards[g].GetComponent<CardData>().colour == 1)
            {
                tmp += " Diamonds\n";
            }
            if (cards[g].GetComponent<CardData>().colour == 2)
            {
                tmp += " Clubs\n";
            }
            if (cards[g].GetComponent<CardData>().colour == 3)
            {
                tmp += " Spades\n";
            }
        }
        return tmp;
    }

    public void ResetMatchVariables()
    {
        // Reset
        participatingAIsCurrentMatch.Clear();
        participatingAIsCurrentRound.Clear();

        // Check for CurrentAIs in Match
        if (participatingAIsCurrentMatch.Count > 0)
        {
            Debug.Log("Oops. Something went wrong. The Match is over, but there are players left");
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
        else if (discardPile.Count != 56)
        {
            Debug.Log("Oooops. Something went wrong. The discardPile has not 56 cards, where it should have 56 cards");
        }
        else if (deck.Count == 0 && discardPile.Count == 56)
        {
            deck.AddRange(discardPile);
            discardPile.Clear();

            Debug.Log("DiscardPile was turned into new deck");
        }


    }

    public void SetNewTournamentPlayerOrder()
    {
        int[] permutation = new int[4];
        permutation[0] = 0;
        permutation[1] = 1;
        permutation[2] = 2;
        permutation[3] = 3;

        for (int i = 0; i < Random.Range(15, 20); i++)
        {
            int a = Random.Range(0, 4);
            int b = Random.Range(0, 4);
            (permutation[a], permutation[b]) = (permutation[b], permutation[a]);
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
            for (int j = 0; j < 14; j++)
            {
                handAI[i].Add(deck[0]);
                deck.RemoveAt(0);
            }
        }
        if (!matchZero) {
            SwapCards();
        }
        for (int i = 0; i < 4; i++)
        {
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
        // Get two worst cards of winner
        List<GameObject> worstTwoCards = SortHandCards(handAI[GetAIIndex(winIndex[0])]).GetRange(12, 13);
        for (int i = 0; i > worstTwoCards.Count; i++)
        {
            handAI[GetAIIndex(winIndex[0])].Remove(worstTwoCards[i]);
        }
        if (winIndex[0].CompareTag("0"))
        {
            winIndex[0].GetComponent<BasicAI>().swappedCards = worstTwoCards;
        }
        else if (winIndex[0].CompareTag("1"))
        {
            winIndex[0].GetComponent<BasicAI>().swappedCards = worstTwoCards;
        }
        else if (winIndex[0].CompareTag("2"))
        {
            winIndex[0].GetComponent<BasicAI>().swappedCards = worstTwoCards;
        }
        else if (winIndex[0].CompareTag("3"))
        {
            winIndex[0].GetComponent<BasicAI>().swappedCards = worstTwoCards;
        }

        // Get worst card of second place
        List<GameObject> worstCard = SortHandCards(handAI[GetAIIndex(winIndex[1])]).GetRange(13, 13);
        for (int i = 0; i > worstCard.Count; i++)
        {
            handAI[GetAIIndex(winIndex[1])].Remove(worstCard[i]);
        }
        if (winIndex[1].CompareTag("0"))
        {
            winIndex[1].GetComponent<BasicAI>().swappedCards = worstCard;
        }
        else if (winIndex[1].CompareTag("1"))
        {
            winIndex[1].GetComponent<BasicAI>().swappedCards = worstCard;
        }
        else if (winIndex[1].CompareTag("2"))
        {
            winIndex[1].GetComponent<BasicAI>().swappedCards = worstCard;
        }
        else if (winIndex[1].CompareTag("3"))
        {
            winIndex[1].GetComponent<BasicAI>().swappedCards = worstCard;
        }

        // Get best card of third place
        List<GameObject> bestCard = SortHandCards(handAI[GetAIIndex(winIndex[2])]).GetRange(0, 0);
        for (int i = 0; i > bestCard.Count; i++)
        {
            handAI[GetAIIndex(winIndex[2])].Remove(bestCard[i]);
        }
        if (winIndex[2].CompareTag("0"))
        {
            winIndex[2].GetComponent<BasicAI>().swappedCards = bestCard;
        }
        else if (winIndex[2].CompareTag("1"))
        {
            winIndex[2].GetComponent<BasicAI>().swappedCards = bestCard;
        }
        else if (winIndex[2].CompareTag("2"))
        {
            winIndex[2].GetComponent<BasicAI>().swappedCards = bestCard;
        }
        else if (winIndex[2].CompareTag("3"))
        {
            winIndex[2].GetComponent<BasicAI>().swappedCards = bestCard;
        }

        // Get best two cards of Arschloch
        List<GameObject> bestTwoCards = SortHandCards(handAI[GetAIIndex(winIndex[3])]).GetRange(0, 1);
        for (int i = 0; i > bestTwoCards.Count; i++)
        {
            handAI[GetAIIndex(winIndex[3])].Remove(bestTwoCards[i]);
        }
        if (winIndex[3].CompareTag("0"))
        {
            winIndex[3].GetComponent<BasicAI>().swappedCards = bestTwoCards;
        }
        else if (winIndex[3].CompareTag("1"))
        {
            winIndex[3].GetComponent<BasicAI>().swappedCards = bestTwoCards;
        }
        else if (winIndex[3].CompareTag("2"))
        {
            winIndex[3].GetComponent<BasicAI>().swappedCards = bestTwoCards;
        }
        else if (winIndex[3].CompareTag("3"))
        {
            winIndex[3].GetComponent<BasicAI>().swappedCards = bestTwoCards;
        }
        handAI[GetAIIndex(winIndex[0])].AddRange(bestTwoCards);
        handAI[GetAIIndex(winIndex[1])].AddRange(bestCard);
        handAI[GetAIIndex(winIndex[2])].AddRange(worstCard);
        handAI[GetAIIndex(winIndex[3])].AddRange(worstTwoCards);
        Debug.Log("Winner and Arschloch swapped: " + GetCardsString(worstTwoCards) + " for "  + GetCardsString(bestTwoCards));
        Debug.Log("Second place and third place swapped: " + GetCardsString(worstCard) + " for "  + GetCardsString(bestCard));
    }
    private List<GameObject> SortHandCards(List<GameObject> handCards)
    {
        List<GameObject> sortedHandCards = new List<GameObject>();

        for (int i = 2; i < 16; i++)
        {
            foreach (GameObject handCard in handCards)
            {
                if (handCard.GetComponent<CardData>().value == i)
                {
                    sortedHandCards.Add(handCard);
                }
            }
        }
        return sortedHandCards;
    }
    private int GetAIIndex(GameObject aI)
    {
        if (aI.CompareTag("0"))
        {
            return 0;
        }
        else if (aI.CompareTag("1"))
        {
            return 1;
        }
        else if (aI.CompareTag("2"))
        {
            return 2;
        }
        else if (aI.CompareTag("3"))
        {
            return 3;
        }
        else
        {
            throw new Exception("There is an imposter among us");
        }
    }
}
