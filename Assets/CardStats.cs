using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Card", menuName = "Scriptable Objects/New Card")]
public class CardStats : ScriptableObject
{
    public int cost;
    public enum type {VICTORY, ACTION, TREASURE, CURSE};
    public enum secondaryType {NONE, ATTACK, REACTION };
    public type thisType;
    public secondaryType thisSecondaryType;
    public string cardName;
    public Sprite cardBackground;
    public string cardText;
    public int numBuys;
    public int numActions;
    public int numCards;
    public int numMoney;
    public int numVictoryPoints;
    public int moneyValue;
    

}