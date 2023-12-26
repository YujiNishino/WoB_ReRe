using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// 攻撃される側
public class AttackedHero : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        /* 攻撃 */   
        // attackerカードを選択
        CardController attacker = eventData.pointerDrag.GetComponent<CardController>();
        // attackerとdefenderを戦わせる
        if (attacker == null)
        {
            return;
        }
        // 敵フィールドにガーディアンカードがあれば攻撃できない
        CardController[] enemyFieldCards = GameManager.instance.GetEnemyFieldCards(attacker.model.isPlayerCard);
        if (Array.Exists(enemyFieldCards, card => card.model.isBaseAbility(BASE_ABILITY.GUARDIAN)))
        {
            return;
        }

        if (attacker.model.canAttack)
        {
            // attackerがheroに攻撃する
            GameManager.instance.AttackToHero(attacker);
            GameManager.instance.CheckHeroHP();    
        }
    }
}
