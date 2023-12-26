using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        heroHp = 20;
        manaCost = 10;
        baseManaCost = 10;
    }

    public void incrementManaCost()
    {
        baseManaCost++;
        manaCost = baseManaCost;
    }

}
