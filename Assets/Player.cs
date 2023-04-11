using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : SerializedMonoBehaviour
{
    public int numActions;
    public int numBuys;
    public int numMoney;

    public List<Card> hand = new List<Card>();
    public Stack<Card> deck = new Stack<Card>();
    public Stack<Card> discard = new Stack<Card>();

    public bool isActiveTurn;

    public CardStats copper;
    public CardStats estate;

    GameObject deckObj, handObj, discardObj;

    public void InitializeDeck()
    {
        //add 7 coppers and 3 estates, then shuffle
        for(int i = 0; i < 7; i++)
        {
            Card card = Instantiate(GameManager.Instance.baseCard.GetComponent<Card>());
            card.stats = copper;
            card.InitializeCard();
            card.transform.parent = deckObj.transform;
            card.gameObject.name = card.stats.cardName;
            deck.Push(card);
        }
        for(int i = 0; i < 3; i++)
        {
            Card card = Instantiate(GameManager.Instance.baseCard.GetComponent<Card>());
            card.stats = estate;
            card.InitializeCard();
            card.transform.parent = deckObj.transform;
            card.gameObject.name = card.stats.cardName;
            deck.Push(card);
        }
        deck = GameManager.Instance.ShuffleCardStack(deck);
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
            hand.Add(deck.Peek());
            deck.Pop();
        }
    }

    private void Awake()
    {
        deckObj = new GameObject("Deck Object");
        deckObj.transform.parent = transform;

        handObj = new GameObject("Hand Object");
        handObj.transform.parent = transform;

        discardObj = new GameObject("Discard Object");
        discardObj.transform.parent = transform;
        InitializeDeck();
        DrawHand();
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
