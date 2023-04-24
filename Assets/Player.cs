using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
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

    public TextMeshProUGUI tmp;

    public bool actionPhase;

    public List<CardStats> playArea; // for transition between

    public int handVisualChunk = 0;

    public string userName;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            userName = GameManager.Instance.inputArea.GetComponent<TMP_InputField>().text;
            GameManager.Instance.inputArea.SetActive(false);
        }
        base.OnNetworkSpawn();
    }

    public bool Equals(Player player)
    {
        return player.userName.Equals(userName);
    }

    public void EndActionPhase()
    {
        actionPhase = false;
        for(int i = 0; i < playArea.Count; i++)
        {
            discard.Push(playArea[i]);
        }
        playArea.Clear();
    }

    public string GenerateRandomString(int amount)
    {
        string x = "";
        for(int i = 0; i < amount; i++)
        {
            x += (char)Random.Range(0, 100);
        }
        return x;
    }

    public void InitializeDeck()
    {
        if (userName.Equals(""))
        {
            userName = GenerateRandomString(5);
        }
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
                Destroy(handVisual[i].gameObject);
            }
            hand.Clear();
            handVisual.Clear();
        }

        DrawCard(5);
        actionPhase = true;
        numActions = 1;
        numBuys = 1;
        numMoney = 0;
    }

    public void Discard(Card card)
    {
        if (!actionPhase)
        {
            discard.Push(card.stats);
        }
        else
        {
            playArea.Add(card.stats);
        }
        hand.Remove(card.stats);
        handVisual.Remove(card);
        Destroy(card.gameObject);
        RecalculateHandGUI();
    }

    public void DrawCard(int amountDrawn)
    {
        while(amountDrawn>0 && deck.Count > 0)
        {
            if (GameManager.Instance.playerRegistry[0].Equals(this))
            {
                Card card = Instantiate(GameManager.Instance.baseCard).GetComponent<Card>();
                card.stats = deck.Peek();
                card.InitializeCard();
                card.isClickable = true;
                handVisual.Add(card);
            }
            hand.Add(deck.Peek());
            deck.Pop();
            amountDrawn--;
        }
        if (amountDrawn > 0)
        {
            while (discard.Count > 0)
            {
                deck.Push(discard.Pop());
            }
            deck = GameManager.Instance.ShuffleCardStack(deck);
            for (int i = 0; i < amountDrawn; i++)
            {
                if (GameManager.Instance.playerRegistry[0].Equals(this))
                {
                    Card card = Instantiate(GameManager.Instance.baseCard).GetComponent<Card>();
                    card.stats = deck.Peek();
                    card.InitializeCard();
                    card.isClickable = true;
                    handVisual.Add(card);
                }

                hand.Add(deck.Peek());
                deck.Pop();
            }
        }
        if (GameManager.Instance.playerRegistry[0].Equals(this))
        {
            RecalculateHandGUI();
        }
    }

    public void RecalculateHandGUI()
    {
        int counter = 0;
        for(int i = 0; i < handVisual.Count; i++)
        {
            handVisual[i].transform.position = new Vector3(-30 + (15 * (i%5)), -20, 50);
            if (counter % 4 == 0)
            {
                counter = 0;
            }
            else
            {
                counter++;
            }
        }

        

        if (discard.Count > 0)
        {
            GameManager.Instance.discardPile.gameObject.SetActive(true);
            GameManager.Instance.discardPile.stats = discard.Peek();
            GameManager.Instance.discardPile.InitializeCard();
        }
        else
        {
            GameManager.Instance.discardPile.gameObject.SetActive(false);
        }
        UpdateHandChunk(handVisualChunk);
    }
    void UpdateHandChunk(int chunkID)
    {
        for(int i = 0; i < handVisual.Count; i++)
        {
            handVisual[i].gameObject.SetActive(false);
        }
        for(int i = 5*chunkID; i < (5*chunkID)+5; i++)
        {
            if (i == handVisual.Count)
            {
                break;
            }
            handVisual[i].gameObject.SetActive(true);
        }
    }

    public void ShiftHandVisual(int change)
    {
        Debug.Log("C");
        handVisualChunk += change;
        UpdateHandChunk(handVisualChunk);
    }

    // Update is called once per frame
    void Update()
    {
        if (tmp != null)
        {
            tmp.text = $"Actions: {numActions}\nBuys: {numBuys}\nMoney: {numMoney}\nDeck Size: {deck.Count}\nHand Size: {hand.Count}\nDiscard Size: {discard.Count}\nCurrent Player: {GameManager.Instance.playerRegistry[GameManager.Instance.currentPlayer].userName}";
        }
    }
}
