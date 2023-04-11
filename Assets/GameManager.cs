using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : SerializedMonoBehaviour
{
    public List<CardStats> cardRegistry;

    public List<Player> playerRegistry;

    public List<Stack<Card>> cardShopStacks;

    public GameObject baseCard;

    public GameObject basePlayer;

    int currentPlayer = 0;
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
        cardRegistry = Resources.LoadAll<CardStats>("").ToList();
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
