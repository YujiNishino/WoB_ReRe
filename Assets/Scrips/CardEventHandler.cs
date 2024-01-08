using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEventHandler
{
    GameManager gameManager;

    public void  Awake() 
    {
        gameManager = GameManager.instance;
    }

    // ▼ ターン開始イベント ---------------------------------------
    public void TurnStartEventHandler_000(CardController card)
    {
        // TODO：対象イベントが発生した場合の処理（固有アビリティ処理）
        Debug.Log("Card 1 - Turn Start Event_" + card.model.id);
    }

    public void TurnStartEventHandler_001(CardController card)
    {
        Debug.Log("Card 2 - Turn Start Event_" + card.model.id);
    }

    // ▼ ユニット出現イベント ---------------------------------------
    public void UnitSpawnEventHandler_000(CardController card)
    {
        Debug.Log("Card 1 - Unit Spawn Event_" + card.model.id);
    }

    public void UnitSpawnEventHandler_001(CardController card)
    {
        Debug.Log("Card 2 - Unit Spawn Event_" + card.model.id);
    }

    // ▼ ユニット攻撃イベント ---------------------------------------
    public void UnitAttackEventHandler_000(CardController card)
    {
        Debug.Log("Card 1 - Unit Attack Event_" + card.model.id);
    }

    public void UnitAttackEventHandler_001(CardController card)
    {
        Debug.Log("Card 2 - Unit Attack Event_" + card.model.id);
    }

}
