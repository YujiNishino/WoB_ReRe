using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// カードデータとその処理
public class CardModel
{
    public string name;
    public int hp;
    public int at;
    public int cost;
    public Sprite icon;
    public BASE_ABILITY[] baseAbility;
    public SPELL spell;

    public bool isAlive;
    public bool canAttack;
    public bool isFieldCard;
    public bool isPlayerCard;

    public CardModel (int cardID, bool isPlayer) 
    {
        string path = "CardEntityList/Card_";        
        string id = cardID.ToString().PadLeft(3, '0');
        CardEntity cardEntity = Resources.Load<CardEntity>(path+id);
        name = cardEntity.name;
        hp = cardEntity.hp;
        at = cardEntity.at;
        cost = cardEntity.cost;
        icon = cardEntity.icon;
        baseAbility = cardEntity.baseAbility;
        spell = cardEntity.spell;

        isAlive = true;
        isPlayerCard = isPlayer;
    }

    void Damage(int dmg)
    {
        hp -= dmg;
        if (hp <= 0)
        {
            hp = 0;
            isAlive = false;
        }
    }

    /// <summary>
    /// 自身を回復
    /// </summary>
    /// <param name="point"></param>
    void RecoveryHP(int point)
    {
        hp += point;
    }

    public void Attack(CardController card)
    {
        card.model.Damage(at);
    }

    /// <summary>
    /// cardを回復させる
    /// </summary>
    /// <param name="card"></param>
    public void Heal(CardController card)
    {
        card.model.RecoveryHP(at);
    }

    /// <summary>
    /// 基本アビリティの確認
    /// </summary>
    /// <param name="checkAbility">確認アビリティ</param>
    /// <returns>有:True 無:False</returns>
    public bool isBaseAbility (BASE_ABILITY checkAbility)
    {
        if (baseAbility.Length > 0)
        {
            if (System.Array.IndexOf(baseAbility, checkAbility) > -1)
            {
                return true;  
            }
        }   
        return false;  
    }
}
