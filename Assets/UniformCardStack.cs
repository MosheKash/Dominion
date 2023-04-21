using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UniformCardStack : MonoBehaviour
{
    public CardStats card;
    public int amount;
    public bool isEmpty;

    public TextMeshProUGUI amountText;

    Card cardVisual;

    public GameObject emptyCardStackSprite;

    private void Start()
    {
        if (amount == 0)
        {
            Debug.LogWarning($"You have a stack with card type {card.cardName} that has a size of 0...");
        }
        cardVisual = GetComponentInChildren<Card>();
        cardVisual.transform.position = transform.position;
        cardVisual.stats = card;
        cardVisual.isStoreCard = true;
        cardVisual.behavior.InitBehaviour();
        cardVisual.InitializeCard();
    }

    public void RemoveCard(int amountToRemove)
    {
        if (amountToRemove > amount)
        {
            Debug.LogError($"Attempted to remove {amountToRemove} cards, when the stack only has {amount} cards in it...");
            return;
        }
        amount -= amountToRemove;
        if (amount <= 0)
        {
            isEmpty = true;
            emptyCardStackSprite.SetActive(true);
            cardVisual.gameObject.SetActive(false);
        }
    }
}
