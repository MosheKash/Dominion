using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class Card : MonoBehaviour
{
    public CardStats stats;

    /* Action Cards */
    public Image cardMainImage;
    public TextMeshProUGUI cardUnboldedText;
    public TextMeshProUGUI cardBoldedText;
    public TextMeshProUGUI cardCostText;
    public TextMeshProUGUI cardTitle;
    public TextMeshProUGUI cardType;

    /* Other Cards */
    public Image largeMainImage;

    CardBehavior behavior;
    public Sprite nullTexture;

    public GameObject universalCardDetails; // isFlipped = false
    public GameObject actionCardDetails; // isFlipped = false
    public GameObject otherCardDetails; // isFlipped = false
    public GameObject cardBack; // isFlipped = true

    public bool isFlipped = false; //true means the dominion side is up, false means the card details are up

    public Color victoryCardColor;
    public Color defaultCardColor;
    public Color treasureCardColor;
    public Color curseCardColor;

    public Sprite cardDescSprite;
    public Sprite cardNoDescSprite;

    public bool isClickable = false;
    public bool cycleCards;

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
        
    }

    IEnumerator CycleCards()
    {
        while (true)
        {
            for(int i = 0; i < GameManager.Instance.actionCardRegistry.Count; i++)
            {
                stats = GameManager.Instance.actionCardRegistry[i];
                InitializeCard();
                yield return new WaitForSeconds(0.5f);
            }
            for (int i = 0; i < GameManager.Instance.victoryCardRegistry.Count; i++)
            {
                stats = GameManager.Instance.victoryCardRegistry[i];
                InitializeCard();
                yield return new WaitForSeconds(0.5f);
            }
            for (int i = 0; i < GameManager.Instance.treasureCardRegistry.Count; i++)
            {
                stats = GameManager.Instance.treasureCardRegistry[i];
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
        if (cycleCards)
        {
            StartCoroutine(CycleCards());
        }
        behavior.InitBehaviour();
    }

    public void FlipCard()
    {
        isFlipped = !isFlipped;

        if (isFlipped)
        {
            cardBack.SetActive(true);
            universalCardDetails.SetActive(false);
            if(stats.thisType == CardStats.type.ACTION) {
                actionCardDetails.SetActive(false);
            }
            else
            {
                otherCardDetails.SetActive(false);
            }
            
        }
        else
        {
            cardBack.SetActive(false);
            universalCardDetails.SetActive(true);
            if (stats.thisType == CardStats.type.ACTION)
            {
                actionCardDetails.SetActive(true);
            }
            else
            {
                otherCardDetails.SetActive(true);
            }
        }
    }

    private void OnMouseDown()
    {
        behavior.Play();
    }

    public void InitializeCard()
    {
        foreach(Canvas canvas in GetComponentsInChildren<Canvas>())
        {
            canvas.worldCamera = GameManager.Instance.mainCamera;
        }
        universalCardDetails.SetActive(true);
        actionCardDetails.SetActive(false);
        otherCardDetails.SetActive(false);
        if (stats.thisType == CardStats.type.ACTION)
        {
            actionCardDetails.SetActive(true);
            SpriteRenderer rend = GetComponent<SpriteRenderer>();
            rend.sprite = cardDescSprite;
            rend.color = defaultCardColor;
            rend.UpdateGIMaterials();
            string bolded = "";
            if (stats.numActions > 0)
            {
                if (!bolded.Equals(""))
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
                bolded += "+<sprite=" + (stats.numMoney - 1) + ">";
            }
            cardBoldedText.text = bolded;

        }
        else
        {
            otherCardDetails.SetActive(true);
            SpriteRenderer rend = GetComponent<SpriteRenderer>();
            rend.sprite = cardNoDescSprite;
            rend.UpdateGIMaterials();
            if (stats.thisType == CardStats.type.VICTORY)
            {
                rend.color = victoryCardColor;
            }
            else if(stats.thisType == CardStats.type.CURSE)
            {
                rend.color = curseCardColor;
            }
            else
            {
                rend.color = treasureCardColor;
            }
            largeMainImage.sprite = stats.cardBackground;
        }
        cardTitle.text = stats.cardName.ToUpper();
        cardCostText.text = stats.cost.ToString();
        cardMainImage.sprite = stats.cardBackground;
        cardType.text = stats.thisType.ToString();
        if (stats.thisSecondaryType.ToString() != "NONE")
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
            cardUnboldedText.transform.localPosition = new Vector3(cardUnboldedText.transform.localPosition.x, -4.5f, cardUnboldedText.transform.localPosition.z);
            cardUnboldedText.verticalAlignment = VerticalAlignmentOptions.Middle;
        }
        else if (cardUnboldedText.text.Equals(""))
        {
            cardBoldedText.transform.localPosition = new Vector3(cardBoldedText.transform.localPosition.x, -4.5f, cardBoldedText.transform.localPosition.z);
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
                cardBoldedText.transform.localPosition = new Vector3(cardBoldedText.transform.localPosition.x, -((float)(cardBoldedText.textInfo.lineCount+0.5)), cardBoldedText.transform.localPosition.z);
                cardUnboldedText.transform.localPosition = new Vector3(cardUnboldedText.transform.localPosition.x, -(((float)(cardBoldedText.textInfo.lineCount + 0.5))+1), cardUnboldedText.transform.localPosition.z);
            }
            else if(amountOfSpaces == 1)
            {
                cardBoldedText.transform.localPosition = new Vector3(cardBoldedText.transform.localPosition.x,  - ((float)(cardBoldedText.textInfo.lineCount + 0.5)), cardBoldedText.transform.localPosition.z);
                cardUnboldedText.transform.localPosition = new Vector3(cardUnboldedText.transform.localPosition.x, -1 -(((float)(cardBoldedText.textInfo.lineCount + 0.5)) + 1), cardUnboldedText.transform.localPosition.z);
            }
            else if(amountOfSpaces%2 == 0)
            {
                amountOfSpaces--;
                int bottomSpace = amountOfSpaces / 2 + 1;
                int topSpace = amountOfSpaces / 2;
                cardBoldedText.transform.localPosition = new Vector3(cardBoldedText.transform.localPosition.x, -1.5f-topSpace, cardBoldedText.transform.localPosition.z);
                cardBoldedText.verticalAlignment = VerticalAlignmentOptions.Top;
                cardUnboldedText.transform.localPosition = new Vector3(cardUnboldedText.transform.localPosition.x, -7.5f+bottomSpace, cardUnboldedText.transform.localPosition.z);
                cardUnboldedText.verticalAlignment = VerticalAlignmentOptions.Bottom;
            }
            else if (amountOfSpaces % 2 == 1)
            {
                int space = amountOfSpaces / 2;
                cardBoldedText.transform.localPosition = new Vector3(cardBoldedText.transform.localPosition.x, -1.5f-space, cardBoldedText.transform.localPosition.z);
                cardBoldedText.verticalAlignment = VerticalAlignmentOptions.Top;
                cardUnboldedText.transform.localPosition = new Vector3(cardUnboldedText.transform.localPosition.x, -7.5f+space, cardUnboldedText.transform.localPosition.z);
                cardUnboldedText.verticalAlignment = VerticalAlignmentOptions.Bottom;
            }
        }
    } 
}
