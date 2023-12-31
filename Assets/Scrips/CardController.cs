using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public delegate void TurnStartEvent(CardController card);   // ターン開始イベント用のデリゲート
public delegate void UnitSpawnEvent(CardController card);   // ユニット出現イベント用のデリゲート
public delegate void UnitAttackEvent(CardController card);  // ユニット攻撃イベント用のデリゲート

public class CardController : MonoBehaviour
{
    GameManager gameManager;

    CardView view;                  // 見かけ(view)に関することを操作
    public CardModel model;         // データ(model)に関することを操作
    public CardMovement movement;   // 移動(movement)に関することを操作

    public CardEventHandler eventHandler;
    public event TurnStartEvent OnTurnStart;    // ターン開始イベント
    public event UnitSpawnEvent OnUnitSpawn;    // ユニット出現イベント
    public event UnitAttackEvent OnUnitAttack;  // ユニット攻撃イベント

    // イベント発火
    public void TriggerTurnStartEvent()
    {
        if (OnTurnStart != null)
        {
            OnTurnStart(this);
        }
    }
    public void TriggerUnitSpawnEvent()
    {
        if (OnUnitSpawn != null)
        {
            OnUnitSpawn(this);
        }
    }
    public void TriggerUnitAttackEvent()
    {
        if (OnUnitAttack != null)
        {
            OnUnitAttack(this);
        }
    }    

    /// <summary>
    /// 自身のカードタイプ：スペルか
    /// </summary>
    /// <value></value>
    public bool IsSpell
    {
        get { return model.spell != SPELL.NONE; }
    }

    public void  Awake() 
    {
        // 表示する要素を取得
        view = GetComponent<CardView>();
        movement = GetComponent<CardMovement>();
        gameManager = GameManager.instance;
    }

    public void Init(int cardID, bool isPlayer)
    {
        // カードモデルをインスタンス化
        model = new CardModel(cardID, isPlayer);

        // イベントのセット
        SetEventHandler();

        // インスタンス化したカードを表示
        view.SetCard(model);
    }

    /// <summary>
    /// TODO：イベントのセット
    /// </summary>
    public void SetEventHandler()
    {
        Debug.Log("Card_Init_cardID_" + model.id);
        CardEventHandler cardEvent = new CardEventHandler();
        switch (model.id)
        {
            case 0:
                Debug.Log("TurnStartEventHandler1");
                OnTurnStart += cardEvent.TurnStartEventHandler_000;
                break;
            case 1:
                Debug.Log("TurnStartEventHandler1");
                Debug.Log("UnitSpawnEventHandler1");
                OnTurnStart += cardEvent.TurnStartEventHandler_001;
                OnUnitSpawn += cardEvent.UnitSpawnEventHandler_001;
                break;
            default:
                break;
        }
    }

    public void Attack(CardController enemyCard)
    {
        model.Attack(enemyCard);
        SetCanAttack(false);
    }

    public void Heal(CardController friendCard)
    {
        model.Heal(friendCard);
        friendCard.RefreshView();
    }

    public void Show()
    {
        view.Show();
    }

    public void RefreshView()
    {
        view.Refresh(model);
    }

    public void SetCanAttack(bool canAttack)
    {
        model.canAttack = canAttack;        
        view.SetActiveSelectabelePanel(canAttack);
    }

    /// <summary>
    /// フィールドへカードを出した（マナ減算、基本アビリティ）
    /// </summary>
    public void OnFiled()
    {
        Debug.Log("CardController_OnFiled()");
        // TODO：固有アビリティ（ログイン）
        TriggerUnitSpawnEvent();
        // コストの減算
        gameManager.ReduceManaCost(model.cost, model.isPlayerCard);
        // フィールドカードに設定
        model.isFieldCard = true;
        // サイズの変更（縮小）
        transform.localScale = new Vector2(0.75f, 0.75f);
        // 基本アビリティの確認（クイック）
        if (model.isBaseAbility(BASE_ABILITY.QUICK))
        {
            SetCanAttack(true);
        }
    }

    /// <summary>
    /// カード破壊確認
    /// </summary> 
    public void CheckAlive()
    {
        if (model.isAlive)
        {
            RefreshView();
        }
        else
        {
            // TODO：固有アビリティ（ログアウト）

            // TODO：ソウルを＋１

            // カードの破棄
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// スペルの使用可否判定
    /// </summary>
    /// <returns>True：使用可能</returns>
    public bool CanUseSpell()
    {
        switch (model.spell)
        {
            case SPELL.DAMAGE_ENEMY_CARD:
            case SPELL.DAMAGE_ENEMY_CARDS:
                // 相手フィールドの全カードに攻撃する
                CardController[] enemyCards = gameManager.GetEnemyFieldCards(this.model.isPlayerCard);
                if (enemyCards.Length > 0)
                {
                    return true;
                }
                return false;
            case SPELL.DAMAGE_ENEMY_HERO:
            case SPELL.HEAL_FRIEND_HERO:            
                return true;
            case SPELL.HEAL_FRIEND_CARD:
            case SPELL.HEAL_FRIEND_CARDS:
                // 自分フィールドの全カードを回復する
                CardController[] friendCards = gameManager.GetFriendFieldCards(this.model.isPlayerCard);
                if (friendCards.Length > 0)
                {
                    return true;
                }
                return false;
            case SPELL.NONE:
                return false;
        }
        return false;
    }    

    public void UseSpellTo(CardController target)
    {
        switch (model.spell)
        {
            case SPELL.DAMAGE_ENEMY_CARD:
                // 特定の敵を攻撃する
                if (target == null)
                {
                    return;
                }
                if (target.model.isPlayerCard == model.isPlayerCard)
                {
                    return;
                }
                Attack(target);
                target.CheckAlive();
                break;
            case SPELL.DAMAGE_ENEMY_CARDS:
                // 相手フィールドの全カードに攻撃する
                CardController[] enemyCards = gameManager.GetEnemyFieldCards(this.model.isPlayerCard);
                foreach(CardController enemyCard in enemyCards) 
                {
                    Attack(enemyCard);
                }
                foreach(CardController enemyCard in enemyCards) 
                {
                    enemyCard.CheckAlive();
                }
                break;
            case SPELL.DAMAGE_ENEMY_HERO:
                // 相手ヒーローを攻撃する
                gameManager.AttackToHero(this);
                break;
            case SPELL.HEAL_FRIEND_CARD:
                // 特定のカードを回復する
                if (target == null)
                {
                    return;
                } 
                if (target.model.isPlayerCard != model.isPlayerCard)
                {
                    return;
                }                            
                Heal(target);
                break;
            case SPELL.HEAL_FRIEND_CARDS:
                // 自分フィールドの全カードを回復する
                CardController[] friendCards = gameManager.GetFriendFieldCards(this.model.isPlayerCard);
                foreach(CardController enemyCard in friendCards) 
                {
                    Heal(enemyCard);
                }
                break;
            case SPELL.HEAL_FRIEND_HERO:
                // 自分ヒーローを回復する
                gameManager.HealToHero(this);
                break;
            case SPELL.NONE:
                return;
        }
        // コストの減算
        gameManager.ReduceManaCost(model.cost, model.isPlayerCard);
         
        Destroy(this.gameObject);
    }
}
