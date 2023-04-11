using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : SerializedMonoBehaviour
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

    int currentPlayer = 0;

    public Camera mainCamera;

    UniformCardStack provinces = null;
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
        //playerRegistry.Add(Instantiate(basePlayer).GetComponent<Player>());
    }
    public Stack<Card> ShuffleCardStack(Stack<Card> input)
    {
        Stack<Card> output = new Stack<Card>();
        List<Card> temp = input.ToList();
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

        return ((counter >= 3) || provinces.amount==0);
    }
}
