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
        //StartCoroutine(CycleCards());
    }

    IEnumerator CycleCards()
    {
        while (true)
        {
            for(int i = 0; i < GameManager.Instance.cardRegistry.Count; i++)
            {
                stats = GameManager.Instance.cardRegistry[i];
                InitializeCard();
                if (!cardUnboldedText.text.Equals(""))
                {
                    Debug.Log("Line Count: " + cardUnboldedText.textInfo.lineCount + ", Text is: " + cardUnboldedText.text);
                }
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
        cardUnboldedText.ForceMeshUpdate();
        cardBoldedText.ForceMeshUpdate();
        SpaceOutCardText();
    }
    void SpaceOutCardText()
    {
        if (cardBoldedText.text.Equals(""))
        {
            cardUnboldedText.transform.position = new Vector3(cardUnboldedText.transform.position.x, -3.5f, cardUnboldedText.transform.position.z);
            cardUnboldedText.verticalAlignment = VerticalAlignmentOptions.Middle;
        }
        else if (cardUnboldedText.text.Equals(""))
        {
            cardBoldedText.transform.position = new Vector3(cardBoldedText.transform.position.x, -3.5f, cardBoldedText.transform.position.z);
            cardBoldedText.verticalAlignment = VerticalAlignmentOptions.Middle;
        }
        else
        {
            int amountOfLines = cardUnboldedText.textInfo.lineCount + cardBoldedText.textInfo.lineCount;
            int amountOfSpaces = 7 - amountOfLines;
            cardBoldedText.verticalAlignment = VerticalAlignmentOptions.Bottom;
            cardUnboldedText.verticalAlignment = VerticalAlignmentOptions.Top;
            if (amountOfSpaces < 0)
            {
                Debug.Log("Something is wrong with the description of Card " + stats.cardName);
            }
            else if(amountOfSpaces == 0)
            {
                cardBoldedText.transform.position = new Vector3(cardBoldedText.transform.position.x, 1-((float)(cardBoldedText.textInfo.lineCount+0.5)), cardBoldedText.transform.position.z);
                cardUnboldedText.transform.position = new Vector3(cardUnboldedText.transform.position.x, 1-(((float)(cardBoldedText.textInfo.lineCount + 0.5))+1), cardUnboldedText.transform.position.z);
            }
            else if(amountOfSpaces == 1)
            {
                cardBoldedText.transform.position = new Vector3(cardBoldedText.transform.position.x, 1 - ((float)(cardBoldedText.textInfo.lineCount + 0.5)), cardBoldedText.transform.position.z);
                cardUnboldedText.transform.position = new Vector3(cardUnboldedText.transform.position.x, -(((float)(cardBoldedText.textInfo.lineCount + 0.5)) + 1), cardUnboldedText.transform.position.z);
            }
            else if(amountOfSpaces%2 == 0)
            {
                amountOfSpaces--;
                int bottomSpace = amountOfSpaces / 2 + 1;
                int topSpace = amountOfSpaces / 2;
                cardBoldedText.transform.position = new Vector3(cardBoldedText.transform.position.x, -0.5f-topSpace, cardBoldedText.transform.position.z);
                cardBoldedText.verticalAlignment = VerticalAlignmentOptions.Top;
                cardUnboldedText.transform.position = new Vector3(cardUnboldedText.transform.position.x, -6.5f+bottomSpace, cardUnboldedText.transform.position.z);
                cardUnboldedText.verticalAlignment = VerticalAlignmentOptions.Bottom;
            }
            else if (amountOfSpaces % 2 == 1)
            {
                int space = amountOfSpaces / 2;
                cardBoldedText.transform.position = new Vector3(cardBoldedText.transform.position.x, -0.5f-space, cardBoldedText.transform.position.z);
                cardBoldedText.verticalAlignment = VerticalAlignmentOptions.Top;
                cardUnboldedText.transform.position = new Vector3(cardUnboldedText.transform.position.x, -6.5f+space, cardUnboldedText.transform.position.z);
                cardUnboldedText.verticalAlignment = VerticalAlignmentOptions.Bottom;
            }
        }
    } 
}
