using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="CardEntity", menuName ="Create CardEntity")] 
// カード基底クラス
public class CardEntity :ScriptableObject
{
    public int id;
    public new string name;
    public int hp;
    public int at;
    public int cost;
    public Sprite icon;
    public BASE_ABILITY[] baseAbility;
    public SPELL spell;
}

// 基本アビリティ
public enum BASE_ABILITY
{
    QUICK,
    GUARDIAN,
}
// スペル
public enum SPELL
{
    NONE,
    DAMAGE_ENEMY_CARD,
    DAMAGE_ENEMY_CARDS,
    DAMAGE_ENEMY_HERO,
    HEAL_FRIEND_CARD,
    HEAL_FRIEND_CARDS,
    HEAL_FRIEND_HERO,            
}