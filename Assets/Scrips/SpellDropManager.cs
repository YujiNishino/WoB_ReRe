using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// スペル発動
public class SpellDropManager : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    { 
        Debug.Log("SpellDropManager:OnDrop");

        // ドラッグ中のカードを取得（spellCard）
        CardController spellCard = eventData.pointerDrag.GetComponent<CardController>();
        // ドロップ先のカードを取得（target）
        CardController target = GetComponent<CardController>(); // nullの可能性あり       
        // spellCardのチェック
        if (spellCard == null)
        {
            // カードを取得できていない
            Debug.Log("SpellDropManager:OnDrop + return");
            return;
        }
        if (spellCard.CanUseSpell())
        {
            spellCard.UseSpellTo(target);
        }

    }
}

