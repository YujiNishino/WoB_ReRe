using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// 攻撃される側
public class AttackedCard : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        /* 攻撃 */   
        // ドラッグ中のカードを取得（attacker）
        CardController attacker = eventData.pointerDrag.GetComponent<CardController>();
        // ドロップ先のカードを取得（defender）
        CardController defender = GetComponent<CardController>();       
        // attackerとdefenderのチェック
        if (attacker == null || defender == null)
        {
            // カードを取得できていない
            return;
        }
        if (attacker.model.isPlayerCard == defender.model.isPlayerCard)
        {
            // ドロップ先が自分のカード
            return;
        }
        // ガーディアンカードがあればガーディアンカード以外攻撃できない
        CardController[] enemyFieldCards = GameManager.instance.GetEnemyFieldCards(attacker.model.isPlayerCard);
        if (Array.Exists(enemyFieldCards, card => card.model.isBaseAbility(BASE_ABILITY.GUARDIAN)) && !defender.model.isBaseAbility(BASE_ABILITY.GUARDIAN))
        {
            return;
        }        
        // バトル開始
        if (attacker.model.canAttack)
        {
            GameManager.instance.CardsBattle(attacker, defender);       
        }
    }
}
