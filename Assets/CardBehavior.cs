using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CardBehavior : MonoBehaviour
{
    public string description;
    Card card;
    CardStats stats;

    Player player;
    private void OnEnable()
    {
        card = GetComponent<Card>();
        player = card.player;
    }

    public abstract void React();

    public void Play()
    {
        /*
         * Do x amount of buys, actions, blah blah then call the custom function
         *
         */
        if(stats.thisType == CardStats.type.ACTION)
        {
            player.numActions += stats.numActions;
            player.numBuys += stats.numBuys;
            player.DrawCard(stats.numCards);
            player.numMoney += stats.numMoney;
        }
        else if(stats.thisType == CardStats.type.VICTORY)
        {
            // doesnt do anything...
        }
        else if(stats.thisType == CardStats.type.TREASURE)
        {
            player.numMoney += stats.moneyValue;
        }
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
