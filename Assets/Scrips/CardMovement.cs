using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

/// <summary>
/// カードオブジェクトに付与されるスクリプト：カードの移動処理
/// </summary>
public class CardMovement : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    /// <summary>カードオブジェクトが属している親要素</summary>
    public Transform defaultParent;
    /// <summary>ドラッグ可否</summary>
    public bool isDragable;

    // ▼ イベント ---------------------------------------------
    void Start()
    {
        // カードオブジェクトの親を記憶
        defaultParent = transform.parent;
    }

    // ドラッグした時
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBeginDrag");
        // カードコストとPlayerコストを比較
        CardController card = GetComponent<CardController>();
        if (GameManager.instance.isPlayerTurn && !card.model.isFieldCard && card.model.cost <= GameManager.instance.player.manaCost)
        {
            // 手札 かつ Playerコスト以下のカード
            isDragable = true;
        }
        else if (GameManager.instance.isPlayerTurn && card.model.isFieldCard && card.model.canAttack)
        {
            // フィールドカード かつ アタック可能カード            
            isDragable = true;
        }        
        else
        {
            isDragable = false;
        }
        if (!isDragable)
        {
            // ドラッグ情報を空にする
            eventData.pointerDrag = null;
            return;
        }

        // 自身の親を取得する（出発点を記憶）
        defaultParent = transform.parent;
        // 親の親を自身の親に設定
        transform.SetParent(defaultParent.parent, false);
        // Raycasts：指定した場所から透明な光線を打ち、光線に当たったオブジェクトの情報を取得する
        // 光線に当たったオブジェクトの情報を取得をブロックしない
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    // ドラッグ中
    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragable)
        {
            return;
        }
        // カードの場所 ＝ ドラッグしている位置
        transform.position = eventData.position;
    }

    // ドラッグ解除
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragable)
        {
            return;
        }        
        // 親を自身の親に再設定（出発点を更新）
        transform.SetParent(defaultParent, false);
        // 光線に当たったオブジェクトの情報を取得をブロックする
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    // ▼ 呼び出し関数 ---------------------------------------------

    /// <summary>
    /// 手札からフィールドへ移動
    /// </summary>
    /// <param name="field"></param>
    /// <returns></returns>
    public IEnumerator MoveToField(Transform field)
    {
        // 一度親をCanvasに変更する
        transform.SetParent(defaultParent.parent);
        // DOTweenでカードをフィールドに移動
        transform.DOMove(field.position, 0.25f);
        yield return new WaitForSeconds(0.25f);
        // カードの親をフィールドに設定
        defaultParent = field;
        transform.SetParent(defaultParent);
    }

    /// <summary>
    /// 自分のフィールドから相手フィールドへ攻撃（移動）
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public IEnumerator MoveToTarget(Transform target)
    {
        // 現在の位置と並びを取得
        Vector3 currentPosition = transform.position;
        int siblingIndex = transform.GetSiblingIndex(); // 配列順を取得
        // 一度親をCanvasに変更する
        transform.SetParent(defaultParent.parent);
        // DOTweenでカードをターゲットに移動
        transform.DOMove(target.position, 0.25f);
        yield return new WaitForSeconds(0.25f);
        // 自分のフィールドへ戻る
        transform.DOMove(currentPosition, 0.25f);
        yield return new WaitForSeconds(0.25f);      
        // カードの親を本来の親（フィールド）に戻す
        transform.SetParent(defaultParent);
        transform.SetSiblingIndex(siblingIndex); // 配列順を設定

    }

}
