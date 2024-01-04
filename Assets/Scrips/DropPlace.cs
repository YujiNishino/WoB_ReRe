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
        Debug.Log("DropPlace_OnDrop()");

        if (type == TYPE.HAND)
        {
            Debug.Log("DropPlace_OnDrop()_if (type == TYPE.HAND)_return");
            return;
        }

        // ドロップされたカードを取得（eventDataから取得）
        CardController dragCard = eventData.pointerDrag.GetComponent<CardController>();
        if(dragCard != null) 
        {
            // 移動できないならスルー
            if (!dragCard.movement.isDragable)
            {
                return;
            }
            // スペルカードならドロップしない
            if (dragCard.IsSpell)
            {
                Debug.Log("card.IsSpell:" + dragCard.IsSpell.ToString());
                return;
            }
            else
            {
                Debug.Log("card.IsSpell:" + dragCard.IsSpell.ToString());
            }
            // 移動先を移動元に設定
            dragCard.movement.defaultParent = this.transform;
            // すでにフィールドカードならスルー
            if (dragCard.model.isFieldCard)
            {
                Debug.Log("DropPlace_OnDrop()_if (dragCard.model.isFieldCard)_return");
                dragCard.movement.isDragable = false;
                return;
            }
            dragCard.OnFiled();
        }
    }
}
