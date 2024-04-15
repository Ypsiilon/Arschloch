using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Interface
{
    public void PreStart();
    public void ReceiveCards(List<GameObject> cards);
    public List<GameObject> PlayCards(
            bool turnZero, 
            List<GameObject> discardPile, 
            List<GameObject> previousTurn, 
            List<GameObject> tournamentPlayerOrder,
            List<GameObject> participatingAIsCurrentMatch, 
            List<GameObject> participatingAIsCurrentRound
    );
}
