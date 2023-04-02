using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class Card : MonoBehaviour
{
    public CardStats stats;
    public Image cardMainImage;
    public TextMeshProUGUI cardUnboldedText;
    public TextMeshProUGUI cardBoldedText;
    public TextMeshProUGUI cardCostText;
    public TextMeshProUGUI cardTitle;
    public TextMeshProUGUI cardType;
    CardBehavior behavior;
    public Sprite nullTexture;
    public List<CardStats> cardList;

    private void Awake()
    {
        behavior = GetComponent<CardBehavior>();
        if(behavior == null)
        {
            behavior = gameObject.AddComponent<GenericBehavior>();
        }
        if(stats == null)
        {
            Debug.LogWarning("The card stats is returning null, please fix this");
            cardBoldedText.text = "NULL";
            cardCostText.text = "NULL";
            cardTitle.text = "NULL";
            cardType.text = "NULL";
            cardMainImage.sprite = nullTexture;
        }


        GatherData();
        StartCoroutine(CycleCards());
    }

    IEnumerator CycleCards()
    {
        while (true)
        {
            for(int i = 0; i < cardList.Count; i++)
            {
                stats = cardList[i];
                InitializeCard();
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        if (stats != null)
        {
            InitializeCard();
        }
    }

    void GatherData()
    {
        List<Object> obj = Resources.LoadAll("Card Data", typeof(CardStats)).ToList();
        Debug.Log(obj.Count);
        for(int i = 0; i < obj.Count; i++)
        {
            cardList.Add((CardStats)obj[i]);
        }
    }

    public void InitializeCard()
    {
        string bolded = "";
        if(stats.numActions > 0)
        {
            if(!bolded.Equals(""))
            {
                bolded += "\n";
            }
            bolded += "+" + stats.numActions + " Action";
            if (stats.numActions > 1)
            {
                bolded += "s";
            }
        }
        if (stats.numBuys > 0)
        {
            if (!bolded.Equals(""))
            {
                bolded += "\n";
            }
            bolded += "+" + stats.numBuys + " Buy";
            if (stats.numBuys > 1)
            {
                bolded += "s";
            }
        }
        if (stats.numCards > 0)
        {
            if (!bolded.Equals(""))
            {
                bolded += "\n";
            }
            bolded += "+" + stats.numCards + " Card";
            if (stats.numCards > 1)
            {
                bolded += "s";
            }
        }
        if (stats.numMoney > 0)
        {
            if (!bolded.Equals(""))
            {
                bolded += "\n";
            }
            bolded += "+<sprite=" + (stats.numMoney - 1)+">";
                /*"+" + stats.numMoney + " Money";
            if (stats.numMoney > 1)
            {
                bolded += "s";
            }*/
        }
        cardBoldedText.text = bolded;
        cardTitle.text = stats.cardName.ToUpper();
        cardCostText.text = stats.cost.ToString();
        cardMainImage.sprite = stats.cardBackground;
        cardType.text = stats.thisType.ToString();
        if(stats.thisSecondaryType.ToString() != "NONE")
        {
            cardType.text += " - " + stats.thisSecondaryType.ToString();
        }
        if (behavior == null)
        {
            cardUnboldedText.text = "NULL";
        }
        else
        {
            cardUnboldedText.text = stats.cardText;
        }
    }
}
