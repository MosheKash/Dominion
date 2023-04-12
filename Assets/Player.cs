using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int numActions;
    public int numBuys;
    public int numMoney;

    public List<CardStats> hand = new List<CardStats>();
    public Stack<CardStats> deck = new Stack<CardStats>();
    public Stack<CardStats> discard = new Stack<CardStats>();

    public List<Card> handVisual = new List<Card>();

    public bool isActiveTurn;

    public CardStats copper;
    public CardStats estate;

    public GameObject deckObj, handObj, discardObj;

    public void InitializeDeck()
    {
        //add 7 coppers and 3 estates, then shuffle
        for(int i = 0; i < 7; i++)
        {
            deck.Push(copper);
        }
        for(int i = 0; i < 3; i++)
        {
            deck.Push(estate);
        }
        deck = GameManager.Instance.ShuffleCardStack(deck);
        DrawHand();
    }

    public void DrawHand()
    {
        //Put entire hand into discard pile, then call DrawCard(5)
        if (hand.Count > 0) {
            for (int i = 0; i < hand.Count; i++)
            {
                discard.Push(hand[i]);
                Destroy(hand[i]);
            }
            hand.Clear();
        }
        DrawCard(5);
    }

    public void DrawCard(int amountDrawn)
    {
        for(int i = 0; i <amountDrawn; i++)
        {
            if (GameManager.Instance.playerRegistry[0].Equals(this)) {
                Card card = Instantiate(GameManager.Instance.baseCard).GetComponent<Card>();
                card.stats = deck.Peek();
                card.InitializeCard();
                card.isClickable = true;
                handVisual.Add(card);
            }

            hand.Add(deck.Peek());
            deck.Pop();
        }
        if (GameManager.Instance.playerRegistry[0].Equals(this))
        {
            RecalculateHandGUI();
        }
    }

    void RecalculateHandGUI()
    {
        for(int i = 0; i < handVisual.Count; i++)
        {
            handVisual[i].transform.position = new Vector3(-30 + 15 * i, -20, 50);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
