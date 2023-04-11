using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniformCardStack : MonoBehaviour
{
    public CardStats card;
    public int amount;
    public bool isEmpty;

    GameObject cardVisual;
    private void Start()
    {
        if (amount == 0)
        {
            Debug.LogWarning($"You have a stack with card type {card.cardName} that has a size of 0...");
        }
        cardVisual = Instantiate(GameManager.Instance.baseCard, transform);
        cardVisual.transform.position = transform.position;
        cardVisual.GetComponent<Card>().stats = card;
    }

    public void RemoveCard(int amountToRemove)
    {
        if (amountToRemove > amount)
        {
            Debug.LogError($"Attempted to remove {amountToRemove} cards, when the stack only has {amount} cards in it...");
            return;
        }
        amount -= amountToRemove;
        if (amount == 0)
        {
            isEmpty = true;
            Destroy(cardVisual);
        }
    }
}
