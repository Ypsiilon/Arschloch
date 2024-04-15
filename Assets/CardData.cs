using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CardData : MonoBehaviour
{

    public int colour; // 0 = Hearts, 1 = Diamonds, 2 = Clubs, 3 = Spades, 4 = Joker
    public int value; // 2-10 = 2-10, 11 = Jack, 12 = Queen, 13 = King, 14 = Ace, 15 = Joker

    public void setColour(int x) {
        if(this.colour != 4) colour = x;
    }
    public void setValue(int x) {
        if(value == 0 || this.colour == 4) value = x;
    }
}
