using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropPlace : MonoBehaviour, IDropHandler
{
    // 配置位置
    public enum TYPE
    {
        HAND,
        FIELD,
    }
    public TYPE type;


    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("DropPlace:OnDrop");

        if (type == TYPE.HAND)
        {
            Debug.Log("DropPlace:OnDrop + return");
            return;
        }

        // ドロップされたカードを取得（eventDataから取得）
        CardController card = eventData.pointerDrag.GetComponent<CardController>();
        if(card != null) 
        {
            // 移動できないならスルー
            if (!card.movement.isDragable)
            {
                return;
            }
            // スペルカードならドロップしない
            if (card.IsSpell)
            {
                Debug.Log("card.IsSpell:" + card.IsSpell.ToString());
                return;
            }
            else
            {
                Debug.Log("card.IsSpell:" + card.IsSpell.ToString());
            }
            // 移動先を移動元に設定
            card.movement.defaultParent = this.transform;
            // すでにフィールドカードならスルー
            if (card.model.isFieldCard)
            {
                return;
            }
            card.OnFiled();
        }
    }
}
