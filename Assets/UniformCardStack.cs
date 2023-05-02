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

    public Card cardVisual;

    public GameObject emptyCardStackSprite;
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
