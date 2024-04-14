using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CardData : MonoBehaviour
{

    public int colour; // 0 = Hearts, 1 = Diamonds, 2 = Clubs, 3 = Spades, 4 = Joker
    public int value; // 2-10 = 2-10, 11 = Jack, 12 = Queen, 13 = King, 14 = Ace, 15 = Joker

    // Start is called before the first frame update
    void Start()
    {

    }
    public void setColour(int x) {
        colour = x;
    }
    public void setValue(int x) {
        value = x;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
