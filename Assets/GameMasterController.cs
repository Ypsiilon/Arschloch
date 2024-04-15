using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class GameMasterController : MonoBehaviour
{
    public int tournamentCount = 5;
    public int matchCount = 5;

    public int[] points;
    public List<GameObject> deck;
    public List<GameObject> discardPile;
    public GameObject[] aIs;
    public List<int>[] stats;
    public List<GameObject>[] handAI;
    public List<GameObject> tournamentPlayerOrder;
    public List<GameObject> participatingAIsCurrentMatch;
    public List<GameObject> participatingAIsCurrentRound;

    private bool turnZero;
    private bool notFirstMatch;
    private GameObject[] winIndex2 = new GameObject[4];

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
        stats = new List<int>[4];

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
            StartCoroutine(StartTournament());
        }
        for (int i = 0; i < 4; i++)
        {
            Debug.Log("AI "+i+" got "+points[i]+" Points!");
        }
    }

    public IEnumerator StartTournament()
    {
        SetNewTournamentPlayerOrder();

        for (int l = 0; l < matchCount; l++)
        {
            if (l == 0)
            {
                notFirstMatch = false;
            }
            else
            {
                notFirstMatch = true;
            }
            Debug.Log("Match  " + l + " started.");
            //float ZW;
            //ZW = Time.realtimeSinceStartup;
            StartMatch();
            yield return new WaitForSeconds(0.002f);

            //Debug.Log("knollo"+(Time.realtimeSinceStartup - ZW));
        }

        yield return null;
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
        Debug.Log("deck:" + deck.Count);
        Debug.Log("discardPile: "+ discardPile.Count);
        GivePoints();
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
            for(int i = 0;i < participatingAIsCurrentRound.Count;i++)
            {
                if (roundWinner == participatingAIsCurrentRound[i])
                {
                    w = i;
                }
            }
        }

        while (participatingAIsCurrentRound.Count > 1 )
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
            //Debug.Log("AI0 Hand Cards"+gameObject.GetComponent<GameMasterController>().GetCardsString(currentPlayer.GetComponent<BasicAI>().sortedHandCards));
            //Debug.Log("Player " + 0 + "s Hand: " + GetCardsString(handAI[0]));
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
    public void GivePoints()
    {



        List<GameObject> winSup = new List<GameObject>();
        winSup.Add(aIs[0]);
        winSup.Add(aIs[1]);
        winSup.Add(aIs[2]);
        winSup.Add(aIs[3]);



        for (int i = 0; i < 3; i++)
        {
            winSup.Remove(winIndex[i]);
        }
        winIndex[3] = winSup[0];


        Debug.Log("k" + winIndex[0].name);
        Debug.Log("k" + winIndex[1].name);
        Debug.Log("k" + winIndex[2].name);
        Debug.Log("k" + winIndex[3].name);

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (winIndex[i] == aIs[j])
                {
                    points[j] += 3 - i;
                    Debug.Log("Rundenposition" + (i + 1) + "hat AI" + j + "belegt");

                }

            }
            winIndex2[i] = winIndex[i];
        }
        gameObject.transform.GetChild(4).GetChild(0).gameObject.GetComponent<GraphScript>().UpdateGraph(points);

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
            for (int j = 0; j < 14; j++)
            {
                handAI[i].Add(deck[0]);
                deck.RemoveAt(0);
            }
        }

        if (notFirstMatch)
        {
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

        int[] pos = new int[4];

        SortAllHands();

        if (winIndex2.Length != 0)
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (winIndex2[i] == aIs[j])
                    {
                        pos[i] = j;
                        Debug.Log(j);
                    }

                }

            }

            Debug.Log("Player " + 0 + "s Hand: " + GetCardsString(handAI[0]));
            Debug.Log("Player " + 1 + "s Hand: " + GetCardsString(handAI[1]));
            Debug.Log("Player " + 2 + "s Hand: " + GetCardsString(handAI[2]));
            Debug.Log("Player " + 3 + "s Hand: " + GetCardsString(handAI[3]));

            (handAI[pos[0]][0], handAI[pos[0]][1], handAI[pos[3]][12], handAI[pos[3]][13]) = (handAI[pos[3]][12], handAI[pos[3]][13], handAI[pos[0]][0], handAI[pos[0]][1]);

            (handAI[pos[1]][0], handAI[pos[2]][13]) = (handAI[pos[2]][13], handAI[pos[1]][0]);

            Debug.Log("Es haben Ai " + pos[0]+ " und " + pos[3]+" 2 Karten getauscht");
            Debug.Log("Es haben Ai " + pos[1] + " und " + pos[2] + " 1 Karte getauscht");


            Debug.Log("Player " + 0 + "s Hand: " + GetCardsString(handAI[0]));
            Debug.Log("Player " + 1 + "s Hand: " + GetCardsString(handAI[1]));
            Debug.Log("Player " + 2 + "s Hand: " + GetCardsString(handAI[2]));
            Debug.Log("Player " + 3 + "s Hand: " + GetCardsString(handAI[3]));
        }
        else
        {
            Debug.Log("wrong!");
        }


    }


    private void SortAllHands()
    {

        for (int j = 0; j < 4; j++)
        {
            List<GameObject> sortCards = new List<GameObject>();
            for (int i = 2; i < 16; i++)
            {
                foreach (GameObject handCard in handAI[j])
                {
                    if (handCard.GetComponent<CardData>().value == i)
                    {
                        sortCards.Add(handCard);
                    }

                }
            }

            handAI[j].Clear();
            handAI[j].AddRange(sortCards);
        }

    }

}
