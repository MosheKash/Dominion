using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class CardCloseup : MonoBehaviour
{
    public Card card;
    public TextMeshProUGUI amountRemaining;
    UniformCardStack cardStack;
    private void Awake()
    {
        card = GetComponentInChildren<Card>();
        transform.parent.gameObject.SetActive(false);
    }

    public void InitializeCard(UniformCardStack stack)
    {
        cardStack = stack;
        card.stats = stack.card;
        card.InitializeCard();
    }

    private void Update()
    {
        if (cardStack != null)
        {
            amountRemaining.text = "Amount Remaining: " + cardStack.amount.ToString();
        }
    }

    public void CloseUI()
    {
        gameObject.SetActive(false);
    }

    public void BuyCard()
    {
        if (GameManager.Instance.currentPlayer.Value == NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>().playerId)
        {
            if (GameManager.Instance.playerRegistry[GameManager.Instance.currentPlayer.Value].numMoney >= card.stats.cost && GameManager.Instance.playerRegistry[GameManager.Instance.currentPlayer.Value].numBuys > 0)
            {
                GameManager.Instance.UpdateGameStatusTextServerRpc($"{GameManager.Instance.playerRegistry[GameManager.Instance.currentPlayer.Value].userName} bought {card.stats.cardName}!");
                GameManager.Instance.playerRegistry[GameManager.Instance.currentPlayer.Value].EndActionPhase();
                GameManager.Instance.playerRegistry[GameManager.Instance.currentPlayer.Value].discard.Push(card.stats);
                cardStack.RemoveCard(1);
                GameManager.Instance.playerRegistry[GameManager.Instance.currentPlayer.Value].numMoney -= card.stats.cost;
                GameManager.Instance.playerRegistry[GameManager.Instance.currentPlayer.Value].numBuys--;
                GameManager.Instance.playerRegistry[GameManager.Instance.currentPlayer.Value].RecalculateHandGUI();
                Debug.Log(GameManager.Instance.playerRegistry[GameManager.Instance.currentPlayer.Value].discard.Peek());
            }
            else
            {
                Debug.LogWarning($"Not enough money to buy card {card.stats.cardName}. The card costs {card.stats.cost}, and you only have {GameManager.Instance.playerRegistry[GameManager.Instance.currentPlayer.Value].numMoney}");
            }
        }
        else
        {
           StartCoroutine(GameManager.Instance.NotYourTurn());
        }
    }

}
