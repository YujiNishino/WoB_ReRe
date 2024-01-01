using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CONST;

public class GamePlayerManager : MonoBehaviour
{
    // デッキ
    public List<int> deck = new List<int>();
    // Heroの体力
    public int heroHp;
    // ManaCost
    public int manaCost;
    // BaseManaCost
    public int baseManaCost;

    public void Init(List<int> cardDeck)
    {
        deck = cardDeck;
        heroHp = CONST.INITIALIZE.PLAYER_HP;
        manaCost = CONST.INITIALIZE.HAVE_MEMORY;
        baseManaCost = CONST.INITIALIZE.BASE_MEMORY;
    }

    public void incrementManaCost()
    {
        if (baseManaCost < CONST.MAX_MIN.MAX_COST) 
        {
            baseManaCost++;
            manaCost = baseManaCost;
        }
    }

}
