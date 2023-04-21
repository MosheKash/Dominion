using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardCloseup : MonoBehaviour
{
    public Card card;
    public TextMeshProUGUI amountRemaining;
    UniformCardStack cardStack;
    private void Awake()
    {
        card = GetComponentInChildren<Card>();
    }

    public void InitializeCard(UniformCardStack stack)
    {
        cardStack = stack;
        card.stats = stack.card;
        card.InitializeCard();
    }

    private void Update()
    {
        amountRemaining.text = "Amount Remaining: " + cardStack.amount.ToString();
    }

    public void CloseUI()
    {
        gameObject.SetActive(false);
    }

    public void BuyCard()
    {
        if (GameManager.Instance.currentPlayer == 0)
        {
            if (GameManager.Instance.playerRegistry[GameManager.Instance.currentPlayer].numMoney >= card.stats.cost && GameManager.Instance.playerRegistry[GameManager.Instance.currentPlayer].numBuys > 0)
            {
                GameManager.Instance.UpdateGameStatusText($"{GameManager.Instance.playerRegistry[GameManager.Instance.currentPlayer].userName} bought {card.stats.cardName}!");
                GameManager.Instance.playerRegistry[GameManager.Instance.currentPlayer].EndActionPhase();
                GameManager.Instance.playerRegistry[GameManager.Instance.currentPlayer].discard.Push(card.stats);
                cardStack.RemoveCard(1);
                GameManager.Instance.playerRegistry[GameManager.Instance.currentPlayer].numMoney -= card.stats.cost;
                GameManager.Instance.playerRegistry[GameManager.Instance.currentPlayer].numBuys--;
                GameManager.Instance.playerRegistry[GameManager.Instance.currentPlayer].RecalculateHandGUI();
                Debug.Log(GameManager.Instance.playerRegistry[GameManager.Instance.currentPlayer].discard.Peek());
            }
            else
            {
                Debug.LogWarning($"Not enough money to buy card {card.stats.cardName}. The card costs {card.stats.cost}, and you only have {GameManager.Instance.playerRegistry[GameManager.Instance.currentPlayer].numMoney}");
            }
        }
        else
        {
            Debug.LogWarning("NOT YOUR TURN!!!");
        }
    }

}
