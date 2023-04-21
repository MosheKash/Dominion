using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public List<CardStats> actionCardRegistry;

    public List<CardStats> victoryCardRegistry;

    public List<CardStats> treasureCardRegistry;

    public List<Player> playerRegistry;

    public List<UniformCardStack> cardShopStacks;

    public List<UniformCardStack> victoryCardStacks;

    public List<UniformCardStack> treasureCardStacks;

    public GameObject baseCard;

    public GameObject basePlayer;

    public GameObject actionCardStackParent;
    public GameObject victoryCardStackParent;
    public GameObject treasureCardStackParent;

    public Camera mainCamera;

    public Card discardPile;

    public TextMeshProUGUI tmp;

    UniformCardStack provinces = null;

    public Button left, right;

    public TextMeshProUGUI gameStatusText;

    public List<Player> players;

    public int currentPlayer;

    public Button endTurnButton;

    public CardCloseup cardCloseup;
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
        InitializeCardRegistry();
        cardShopStacks = actionCardStackParent.GetComponentsInChildren<UniformCardStack>().ToList();
        victoryCardStacks = victoryCardStackParent.GetComponentsInChildren<UniformCardStack>().ToList();
        treasureCardStacks = treasureCardStackParent.GetComponentsInChildren<UniformCardStack>().ToList();
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
        StartGame(2);
        left.onClick.AddListener(delegate { playerRegistry[0].ShiftHandVisual(-1); });
        right.onClick.AddListener(delegate { playerRegistry[0].ShiftHandVisual(1); });
        gameStatusText.text = "";
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

    /* 
     
    Player with id 0 is always the player at the machine (review this when doing server stuff) 

     */
    void StartGame(int numPlayers)
    {
        if (numPlayers < 2)
        {
            Debug.LogWarning($"Cannot start with less than 2 players, attempted to start with {numPlayers} players...");
            return;
        }
        List<CardStats> toChoose = new List<CardStats>();
        for(int i = 0; i < actionCardRegistry.Count; i++)
        {
            toChoose.Add(actionCardRegistry[i]);
        }
        for(int i = 0; i < cardShopStacks.Count; i++)
        {
            int randomIndex = Random.Range(0, toChoose.Count);
            cardShopStacks[i].card = toChoose[randomIndex];
            toChoose.RemoveAt(randomIndex);
        }
        GameObject playerRoot = new GameObject("Players");
        for(int i = 0; i < numPlayers; i++)
        {
            Player player = Instantiate(basePlayer).GetComponent<Player>();
            playerRegistry.Add(player);
            
            player.transform.parent = playerRoot.transform;
            if (i == 0)
            {
                playerRegistry[i].deckObj = new GameObject("Deck Object");
                playerRegistry[i].deckObj.transform.parent = transform;

                playerRegistry[i].handObj = new GameObject("Hand Object");
                playerRegistry[i].handObj.transform.parent = transform;

                playerRegistry[i].discardObj = new GameObject("Discard Object");
                playerRegistry[i].discardObj.transform.parent = transform;

                playerRegistry[i].tmp = tmp;
            }
            playerRegistry[i].InitializeDeck();
        }
        for(int i = 0; i < treasureCardStacks.Count; i++)
        {
            if (treasureCardStacks[i].card.cardName.Equals("Copper"))
            {
                treasureCardStacks[i].RemoveCard(7 * numPlayers);
            }
        }
        if(numPlayers == 2)
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
                victoryCardStacks[i].amount = 10 * (numPlayers - 1);
            }
        }
        Debug.Log(playerRegistry[0]+", "+playerRegistry[0].userName);
        endTurnButton.onClick.AddListener(() => GameManager.Instance.EndTurn(playerRegistry[0]));
    }

    public void EndTurn(Player player)
    {
        Debug.Log("Called");
        if (player.Equals(playerRegistry[currentPlayer]))
        {

            playerRegistry[currentPlayer].DrawHand();
            currentPlayer++;
        }
        else
        {
            Debug.LogError("Please Investigate!!");
        }
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

    int calculateMaxChunk()
    {
        float toReturn;
        toReturn = (((float)playerRegistry[0].handVisual.Count) / 5)-1;
        if(toReturn>(int)toReturn && toReturn < ((int)toReturn + 1)){
            toReturn += 1;
        }
        return (int)toReturn;
    }

    private void Update()
    {
        int maxChunk = calculateMaxChunk();
        if (playerRegistry[0].handVisualChunk == 0)
        {
            left.gameObject.SetActive(false);
            if(playerRegistry[0].handVisualChunk < maxChunk)
            {
                right.gameObject.SetActive(true);
            }
        }
        if(playerRegistry[0].handVisualChunk == maxChunk)
        {
            right.gameObject.SetActive(false);
            if (playerRegistry[0].handVisualChunk > 0)
            {
                right.gameObject.SetActive(true);
            }
        }
        else if(!(playerRegistry[0].handVisualChunk == 0) && !(playerRegistry[0].handVisualChunk == maxChunk))
        {
            left.gameObject.SetActive(true);
            right.gameObject.SetActive(true);
        }
    }

    public void UpdateGameStatusText(string textUpdate)
    {
        if(gameStatusText.text != "")
        {
            gameStatusText.text += "\n";
        }
        gameStatusText.text += textUpdate;
    }

}
