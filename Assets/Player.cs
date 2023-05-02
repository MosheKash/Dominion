using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

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

    public GameObject deckObj, handObj;

    public TextMeshProUGUI currentStatsText;

    public bool actionPhase;

    public List<CardStats> playArea; // for transition between

    public int handVisualChunk = 0;

    public string userName;

    public GameObject gameUI;

    public Button startGame;

    public bool isHost;

    bool checker = false;

    public Button left, right;

    public Button endTurnButton;

    public CardCloseup cardCloseup;

    public Card discardPile;

    public int playerId;

    [ClientRpc]
    public void EnableCardsClientRpc()
    {
        GameManager.Instance.actionCardStackParent.transform.parent.gameObject.SetActive(true);
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            userName = GameManager.Instance.inputArea.GetComponent<TMP_InputField>().text;
            GameManager.Instance.inputArea.SetActive(false);
        }
        if (IsHost)
        {
            startGame = FindObjectOfType<LobbyCanvasShell>().start;
            if (startGame != null)
            {
                startGame.gameObject.SetActive(true);
                startGame.onClick.AddListener(() =>
                {
                    GameManager.Instance.InitGame();
                });
            }

        }
        else if (IsClient)
        {
            FindObjectOfType<LobbyCanvasShell>().waitText.SetActive(true);
        }
        GameManager.Instance.playerRegistry.Add(this);
        playerId = GameManager.Instance.nextPlayerId.Value;
        GameManager.Instance.IncreasePlayerIntServerRpc();
        Debug.Log("Player ID: " + playerId);
        base.OnNetworkSpawn();
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
    [ClientRpc]
    public void InitializeDeckClientRpc()
    {
        if (!IsOwner) return;
        if(startGame!=null) startGame.gameObject.SetActive(false);
        FindObjectOfType<LobbyCanvasShell>().waitText.SetActive(false);
        if (IsOwner) {
            deckObj = new GameObject("Deck Object");
            deckObj.transform.parent = transform;

            handObj = new GameObject("Hand Object");
            handObj.transform.parent = transform;

            discardPile = Instantiate(GameManager.Instance.baseCard).GetComponent<Card>();
            discardPile.transform.position = new Vector3(70, -30, 75);
            discardPile.transform.parent = transform;
        }
        //gameUI.SetActive(true);
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
        endTurnButton = GameObject.FindGameObjectWithTag("EndTurnButton").GetComponent<Button>();
        left = GameObject.FindGameObjectWithTag("LeftButton").GetComponent<Button>();
        right = GameObject.FindGameObjectWithTag("RightButton").GetComponent<Button>();
        cardCloseup = GameObject.FindGameObjectWithTag("CardCloseup").GetComponent<CardCloseup>();
        cardCloseup.gameObject.SetActive(false);
        currentStatsText = GameObject.FindGameObjectWithTag("PlayerInfo").GetComponent<TextMeshProUGUI>();
        endTurnButton.onClick.AddListener(() => EndTurn());
        left.onClick.AddListener(delegate { NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>().ShiftHandVisual(-1); });
        right.onClick.AddListener(delegate { NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>().ShiftHandVisual(1); });
    }

    public void EndTurn()
    {
        DrawHand();
        GameManager.Instance.ShiftTurnIndicatorServerRpc();
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
        //Debug.Log($"AAAA {NetworkManager.Singleton.LocalClient.PlayerObject.GetType()}, {this.GetType()}, {NetworkManager.Singleton.LocalClient.PlayerObject.Equals(this)}");
        while (amountDrawn>0 && deck.Count > 0)
        {
            //Debug.Log($"{NetworkManager.Singleton.LocalClient.PlayerObject}, {this}, {NetworkManager.Singleton.LocalClient.PlayerObject.Equals(this)}");
            if (NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>().Equals(this))
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
                if (NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>().Equals(this))
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
        if (NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>().Equals(this))
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
            discardPile.gameObject.SetActive(true);
            discardPile.stats = discard.Peek();
            discardPile.InitializeCard();
        }
        else
        {
            discardPile.gameObject.SetActive(false);
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
        if((handVisualChunk==0 && change<0) || (handVisualChunk==calculateMaxChunk() && change > 0))
        {
            return;
        }
        handVisualChunk += change;
        UpdateHandChunk(handVisualChunk);
    }
    int calculateMaxChunk()
    {
        float toReturn;
        toReturn = (((float)NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>().handVisual.Count) / 5) - 1;
        if (toReturn > (int)toReturn && toReturn < ((int)toReturn + 1))
        {
            toReturn += 1;
        }
        return (int)toReturn;
    }
    // Update is called once per frame
    void Update()
    {
        if(GameManager.Instance.gameStarted.Value && !checker && GameManager.Instance.checker)
        {
            InitializeDeckClientRpc();
            checker = true;
        }
        if (GameManager.Instance.gameStarted.Value)
        {
            if (currentStatsText != null)
            {
                currentStatsText.text = $"Actions: {numActions}\nBuys: {numBuys}\nMoney: {numMoney}\nDeck Size: {deck.Count}\nHand Size: {hand.Count}\nDiscard Size: {discard.Count}\nCurrent Player: {GameManager.Instance.playerRegistry[GameManager.Instance.currentPlayer.Value].userName}";
            }
            int maxChunk = calculateMaxChunk();
            if (NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>().handVisualChunk == 0)
            {
                left.gameObject.SetActive(false);
                if (NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>().handVisualChunk < maxChunk)
                {
                    right.gameObject.SetActive(true);
                }
            }
            if (NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>().handVisualChunk == maxChunk)
            {
                right.gameObject.SetActive(false);
                if (NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>().handVisualChunk > 0)
                {
                    right.gameObject.SetActive(true);
                }
            }
            else if (!(NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>().handVisualChunk == 0) && !(NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>().handVisualChunk == maxChunk))
            {
                left.gameObject.SetActive(true);
                right.gameObject.SetActive(true);
            }
        }
    }
}
