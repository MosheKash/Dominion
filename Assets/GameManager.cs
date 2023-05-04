using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour
{
    public GameObject notYourTurn;
    public GameObject winner;

    public List<CardStats> actionCardRegistry;

    public List<CardStats> victoryCardRegistry;

    public List<CardStats> treasureCardRegistry;

    public NetworkVariable<bool> gameEnded = new NetworkVariable<bool>(false);

    public List<Player> playerRegistry;

    public List<UniformCardStack> cardShopStacks;

    public List<UniformCardStack> victoryCardStacks;

    public List<UniformCardStack> treasureCardStacks;

    public GameObject baseCard;

    public GameObject actionCardStackParent;
    public GameObject victoryCardStackParent;
    public GameObject treasureCardStackParent;

    public NetworkVariable<bool> gameStarted = new NetworkVariable<bool>(false);

    public NetworkVariable<int> nextPlayerId = new NetworkVariable<int>(0);

    public NetworkVariable<int> counter = new NetworkVariable<int>(0);

    public Camera mainCamera;

    public bool checker = false;

    public NetworkList<int> playerVictoryPoints;

    UniformCardStack provinces = null;

    public TextMeshProUGUI gameStatusText;

    public List<Player> players;

    public NetworkVariable<int> currentPlayer = new NetworkVariable<int>(0);

    public TextMeshProUGUI debugServerText;

    public GameObject gameUI;

    public Button startHost, addClient;
    public GameObject inputArea;
    int max = -1;
    [ServerRpc(RequireOwnership =false)]
    public void EndTurnServerRpc()
    {
        PulseClientRpc(); // dont know why i need this, but this function doesnt work w/o it...
        //ShowGameWinnerTextClientRpc("DEBUG");
        if (currentPlayer.Value < max)
        {
            currentPlayer.Value++;
        }
        else
        {
            currentPlayer.Value = 0;
        }
        if (CheckGameEnding())
        {
            int largestVP = 0;
            int largestIndex = 0;
            for(int i = 0; i < playerVictoryPoints.Count; i++)
            {
                if(playerVictoryPoints[i] >= largestVP)
                {
                    largestVP = playerVictoryPoints[i];
                    largestIndex = i;
                }
            }
            ShowGameWinnerTextClientRpc("REPLACE ME");
        }
    }

    [ClientRpc]
    public void PulseClientRpc()
    {
        Debug.Log("Pulse");
    }

    [ClientRpc]
    public void ShowGameWinnerTextClientRpc(string username)
    {
        winner.SetActive(true);
        winner.GetComponent<TextMeshProUGUI>().text = "Winner is: " + username;
    }

    public void SpawnPlayer(bool isHost)
    {
        if (isHost)
        {
            NetworkManager.Singleton.StartHost();
        }
        else if (!isHost)
        {
            NetworkManager.Singleton.StartClient();
        }
        inputArea.SetActive(false);
    }

    public IEnumerator NotYourTurn()
    {
        notYourTurn.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        notYourTurn.SetActive(false);
    }

    public void InitGame()
    {
        gameStarted.Value = true;
    }

    public static GameManager Instance { get; private set; }
    private void Awake()
    {
        
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        if (startHost != null)
        {
            startHost.onClick.AddListener(() =>
            {
                inputArea.SetActive(true);
                startHost.gameObject.SetActive(false);
                addClient.gameObject.SetActive(false);
                inputArea.GetComponent<TMP_InputField>().onEndEdit.AddListener(delegate
                {
                    SpawnPlayer(true);
                });
            });
        }
        if (addClient != null)
        {
            addClient.onClick.AddListener(() =>
            {
                inputArea.SetActive(true);
                startHost.gameObject.SetActive(false);
                addClient.gameObject.SetActive(false);
                inputArea.GetComponent<TMP_InputField>().onEndEdit.AddListener(delegate
                {

                    SpawnPlayer(false);
                });
            });
        }
        playerVictoryPoints = new NetworkList<int>();
    }
    public Stack<CardStats> ShuffleCardStack(Stack<CardStats> input)
    {
        Stack<CardStats> output = new Stack<CardStats>();
        List<CardStats> temp = input.ToList();
        while(temp.Count > 0)
        {
            int randomIndex = Random.Range(0, temp.Count);
            output.Push(temp[randomIndex]);
            temp.RemoveAt(randomIndex);
        }
        return output;
    }
    void InitializeCardRegistry()
    {
        actionCardRegistry = Resources.LoadAll<CardStats>("Card Data/Action").ToList();
        victoryCardRegistry = Resources.LoadAll<CardStats>("Card Data/Victory").ToList();
        treasureCardRegistry = Resources.LoadAll<CardStats>("Card Data/Treasure").ToList();
    }
    [ServerRpc(RequireOwnership = false)]
    public void IncreaseMaxIntServerRpc()
    {
        nextPlayerId.Value++;
    }
    /* 
     
    Player with id 0 is always the player at the machine (review this when doing server stuff) 

     */
    [ClientRpc]
    public void StartGameClientRpc ()
    {
        InitializeCardRegistry();
        cardShopStacks = actionCardStackParent.GetComponentsInChildren<UniformCardStack>().ToList();
        victoryCardStacks = victoryCardStackParent.GetComponentsInChildren<UniformCardStack>().ToList();
        treasureCardStacks = treasureCardStackParent.GetComponentsInChildren<UniformCardStack>().ToList();
        gameUI.SetActive(true);

        foreach(UniformCardStack gameObj in cardShopStacks)
        {
            gameObj.cardVisual.gameObject.SetActive(true);
        }
        foreach (UniformCardStack gameObj in victoryCardStacks)
        {
            gameObj.cardVisual.gameObject.SetActive(true);
        }
        foreach (UniformCardStack gameObj in treasureCardStacks)
        {
            gameObj.cardVisual.gameObject.SetActive(true);
        }
        for (int i = 0; i < victoryCardStacks.Count; i++)
        {
            if (victoryCardStacks[i].card.cardName.Equals("Province"))
            {
                provinces = victoryCardStacks[i];
                break;
            }
        }
        if (provinces == null)
        {
            Debug.LogWarning("Unable to find province stack, please investigate...");
        }
        List<CardStats> toChoose = CardSelection();
        for(int i = 0; i < cardShopStacks.Count; i++)
        {
            if (cardShopStacks[i].amount == 0)
            {
                Debug.LogWarning($"You have a stack with card type {cardShopStacks[i].card.cardName} that has a size of 0...");
            }
            cardShopStacks[i].cardVisual = cardShopStacks[i].GetComponentInChildren<Card>();
            cardShopStacks[i].card = toChoose[i];
            cardShopStacks[i].cardVisual.stats = cardShopStacks[i].card;
            cardShopStacks[i].cardVisual.isStoreCard = true;
            if (cardShopStacks[i].cardVisual.behavior != null)
            {
                cardShopStacks[i].cardVisual.behavior.InitBehaviour();
            }
            cardShopStacks[i].cardVisual.InitializeCard();
            cardShopStacks[i].cardVisual.transform.position = cardShopStacks[i].transform.position;
        }
        for(int i = 0; i < victoryCardStacks.Count; i++)
        {
            victoryCardStacks[i].cardVisual.stats = victoryCardStacks[i].card;
            victoryCardStacks[i].cardVisual.InitializeCard();
        }
        for(int i = 0; i < treasureCardStacks.Count; i++)
        {
            treasureCardStacks[i].cardVisual.stats = treasureCardStacks[i].card;
            treasureCardStacks[i].cardVisual.InitializeCard();
        }
        for(int i = 0; i < treasureCardStacks.Count; i++)
        {
            if (treasureCardStacks[i].card.cardName.Equals("Copper"))
            {
                treasureCardStacks[i].RemoveCard(7 * playerRegistry.Count);
            }
        }
        if(playerRegistry.Count == 2)
        {
            for(int i = 0; i < victoryCardStacks.Count; i++)
            {
                victoryCardStacks[i].amount = 8;
            }
        }
        for (int i = 0; i < victoryCardStacks.Count; i++)
        {
            if(victoryCardStacks[i].card.thisType == CardStats.type.CURSE)
            {
                victoryCardStacks[i].amount = 10 * (playerRegistry.Count - 1);
            }
        }
        gameStatusText.gameObject.SetActive(true);
        gameStatusText.text = "";
        max = FindObjectsOfType<Player>().Length-1;
        Debug.Log(max + " ka");
        if (IsServer)
        {
            gameStarted.Value = true;
        }
    }

    public List<CardStats> CardSelection()
    {
        List<CardStats> toReturn = new List<CardStats>();
        List<CardStats> toChoose = new List<CardStats>();
        for (int i = 0; i < actionCardRegistry.Count; i++)
        {
            toChoose.Add(actionCardRegistry[i]);
        }
        for(int i = 0; i < cardShopStacks.Count; i++)
        {
            int toRemove = Random.Range(0, toChoose.Count);
            toReturn.Add(toChoose[toRemove]);
            toChoose.RemoveAt(toRemove);
        }
        return toReturn;
    }

    //return true if the game is over
    public bool CheckGameEnding()
    {
        int counter = 0;
        for(int i = 0; i < cardShopStacks.Count; i++)
        {
            if(cardShopStacks[i].amount == 0)
            {
                counter++;
            }
        }
        for (int i = 0; i < victoryCardStacks.Count; i++)
        {
            if (victoryCardStacks[i].amount == 0)
            {
                counter++;
            }
        }
        for (int i = 0; i < treasureCardStacks.Count; i++)
        {
            if (treasureCardStacks[i].amount == 0)
            {
                counter++;
            }
        }

        return ((counter >= 3) || provinces.amount==0);
    }

    private void Update()
    {
        if(gameStarted.Value && !checker)
        {
            Debug.Log("Client");
            StartGameClientRpc();
            checker = true;
        }
        if (debugServerText != null)
        {
            Player[] players = FindObjectsOfType<Player>();
            debugServerText.text = "Amount of Players: "+players.Length.ToString();
        }


        

    }
    [ClientRpc]
    public void UpdateGameStatusClientRpc(string textUpdate)
    {
        if (gameStatusText.text != "")
        {
            gameStatusText.text += "\n";
        }
        gameStatusText.text += textUpdate;
    }

    [ServerRpc(RequireOwnership =false)]
    public void UpdateGameStatusTextServerRpc(string textUpdate)
    {
        UpdateGameStatusClientRpc(textUpdate);
    }

}
