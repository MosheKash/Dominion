using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CardBehavior : MonoBehaviour
{
    public string description;
    Card card;
    CardStats stats;

    Player player;

    public void InitBehaviour()
    {
        card = GetComponent<Card>();
        player = GameManager.Instance.playerRegistry[0];
        stats = card.stats;
    }

    public abstract void React();

    public void Play()
    {
        if (card.isClickable)
        {
            /*
             * Do x amount of buys, actions, blah blah then call the custom function
             *
             */
            Debug.Log($"!! {stats}, {card}");
            Debug.Log($"Clicked on {stats.cardName}!");
            if (stats.thisType == CardStats.type.ACTION)
            {
                player.numActions += stats.numActions;
                player.numBuys += stats.numBuys;
                player.DrawCard(stats.numCards);
                player.numMoney += stats.numMoney;

            }
            else if (stats.thisType == CardStats.type.VICTORY)
            {
                // doesnt do anything...
            }
            else if (stats.thisType == CardStats.type.TREASURE)
            {
                player.numMoney += stats.moneyValue;
            }
            CustomAction();
        }
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
