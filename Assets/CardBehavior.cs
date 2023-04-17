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
            if (stats.thisType == CardStats.type.ACTION && player.actionPhase && player.numActions>0)
            {
                player.numActions += stats.numActions;
                player.numBuys += stats.numBuys;
                player.DrawCard(stats.numCards);
                player.numMoney += stats.numMoney;
                if(stats.thisSecondaryType == CardStats.secondaryType.ATTACK)
                {
                    Attack();
                }
                player.numActions--;
            }
            else if (stats.thisType == CardStats.type.VICTORY)
            {
                // doesnt do anything...
            }
            else if (stats.thisType == CardStats.type.TREASURE)
            {
                player.EndActionPhase();
                player.numMoney += stats.moneyValue;
            }
            CustomAction();
            if(player.numActions == 0)
            {
                player.EndActionPhase();
            }
            if(stats.thisType == CardStats.type.ACTION || stats.thisType == CardStats.type.TREASURE)
            {
                player.Discard(card);
            }
        }
        else if (card.isStoreCard)
        {
            if(player.numMoney >= stats.cost && player.numBuys>0)
            {
                player.EndActionPhase();
                player.discard.Push(stats);
                UniformCardStack stack = GetComponentInParent<UniformCardStack>();
                stack.amount--;
                player.numMoney -= stats.cost;
                player.numBuys--;
                player.RecalculateHandGUI();
                Debug.Log(player.discard.Peek());
            }
            else
            {
                Debug.LogWarning($"Not enough money to buy card {stats.cardName}. The card costs {stats.cost}, and you only have {player.numMoney}");
            }
        }
    }

    public abstract void Attack();

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
