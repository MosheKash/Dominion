using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CardBehavior : MonoBehaviour
{
    public string description;
    Card card;
    CardStats stats;
    private void OnEnable()
    {
        Debug.Log("BRUH");
        card = GetComponent<Card>();
    }

    public abstract void React();

    public void Play()
    {
        /*
         * Do x amount of buys, actions, blah blah then call the custom function
         *
         */

        CustomAction();
    }

    public void UpdateDescription()
    {
        if (card != null)
        {
            stats = card.stats;
        }
        if (card != null && stats!=null)
        {
            description = stats.cardText;
            card.cardUnboldedText.text = description;
        }
        else
        {
            Debug.Log(card.GetType() + ", " + stats.GetType());
        }
    }

    public abstract void CustomAction();

}
