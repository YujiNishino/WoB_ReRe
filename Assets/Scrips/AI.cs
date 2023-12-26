using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    GameManager gameManager;
    private void Start()
    {
        gameManager = GameManager.instance;
    }

    public IEnumerator EnemyTurn()
    {
        Debug.Log("EnemyTurn");

        // フィールドカードを攻撃可能にする
        CardController[] enemyFieldCardList = gameManager.enemyFieldTransform.GetComponentsInChildren<CardController>();
        gameManager.SettingCanAttackView(enemyFieldCardList, true);
        yield return new WaitForSeconds(1);

        /* 場にカードを出す */
        // 手札のカードリストを取得
        CardController[] handCardList = gameManager.enemyHandTransform.GetComponentsInChildren<CardController>();

        // コスト以下のカード（ モンスターカードならコストのみ or スペルならコストと使用可能かどうかCanUseSpell ）
        // があればカードをフィールドに出し続ける
        while (Array.Exists(handCardList, card => 
                (card.model.cost <= gameManager.enemy.manaCost)
                && (!card.IsSpell || (card.IsSpell && card.CanUseSpell()))))
        {
            // コスト以下のリストを取得
            CardController[] selctableHandCardList = Array.FindAll(handCardList, card => 
                                                            (card.model.cost <= gameManager.enemy.manaCost)
                                                        &&  (!card.IsSpell || (card.IsSpell && card.CanUseSpell())));            
            // カードの選択（先頭の１枚）
            CardController selectCard = selctableHandCardList[0];
            
            // カードを表にする
            selectCard.Show();

            // スペルカードか
            if (selectCard.IsSpell)
            {
                // スペルの発動
                StartCoroutine(CastSpellOf(selectCard));
            }
            else
            {
                // カードの移動
                StartCoroutine(selectCard.movement.MoveToField(gameManager.enemyFieldTransform));
                // カードの出力
                selectCard.OnFiled();
            }
            // 少し待つ
            yield return new WaitForSeconds(1);
            // 手札のカードリストを再取得
            handCardList = gameManager.enemyHandTransform.GetComponentsInChildren<CardController>();
        }

        /* 攻撃 */   
        // フィールドカードを取得
        CardController[] enemyfieldCardList = gameManager.enemyFieldTransform.GetComponentsInChildren<CardController>();
        // 攻撃可能なカードがあれば攻撃を繰り返す
        while (Array.Exists(enemyfieldCardList, card => card.model.canAttack))
        {
            // 攻撃可能カードリストを取得
            CardController[] enemyCanAttackCardList = Array.FindAll(enemyfieldCardList, card => card.model.canAttack);
            // 攻撃対象カードリストを取得
            CardController[] playerfieldCardList = gameManager.playerFieldTransform.GetComponentsInChildren<CardController>();     
            // attackerカードを選択        
            CardController attacker = enemyCanAttackCardList[0];

            if (playerfieldCardList.Length > 0)
            {
                // defenderカードを選択
                // ガーディアンカードのみ攻撃対象にする
                if (Array.Exists(playerfieldCardList, card => card.model.isBaseAbility(BASE_ABILITY.GUARDIAN)))
                {
                    playerfieldCardList = Array.FindAll(playerfieldCardList, card => card.model.isBaseAbility(BASE_ABILITY.GUARDIAN));
                }
                CardController defender = playerfieldCardList[0];     
                // カードの移動（コルーチンが終わるまで待機）
                yield return StartCoroutine(attacker.movement.MoveToTarget(defender.transform));
                // attackerとdefende（オブジェクトの破棄も伴う）
                gameManager.CardsBattle(attacker, defender);            
            }
            else
            {
                // カードの移動（コルーチンが終わるまで待機）
                yield return StartCoroutine(attacker.movement.MoveToTarget(gameManager.playerHero));
                // Heroへの攻撃
                gameManager.AttackToHero(attacker);
                yield return new WaitForSeconds(0.25f);                   
                // 体力チェック
                gameManager.CheckHeroHP();
            }
            // フィールドカードを再取得
            enemyfieldCardList = gameManager.enemyFieldTransform.GetComponentsInChildren<CardController>();
            // 少し待つ
            yield return new WaitForSeconds(1);
        }        
        // ターンエンド
        gameManager.CangeTurn(); 
    }    

    // スペル発動
    IEnumerator CastSpellOf(CardController card)
    {
        CardController target = null;
        Transform movePosition = null;
        switch (card.model.spell)
        {
            case SPELL.DAMAGE_ENEMY_CARD:
                target = gameManager.GetEnemyFieldCards(card.model.isPlayerCard)[0];
                movePosition = target.transform;
                break;
            case SPELL.DAMAGE_ENEMY_CARDS:
                movePosition = gameManager.playerFieldTransform;
                break;
            case SPELL.DAMAGE_ENEMY_HERO:
                movePosition = gameManager.playerHero;            
                break;
            case SPELL.HEAL_FRIEND_CARD:
                target = gameManager.GetFriendFieldCards(card.model.isPlayerCard)[0];
                movePosition = target.transform;
                break;                                
            case SPELL.HEAL_FRIEND_CARDS:
                movePosition = gameManager.enemyFieldTransform; 
                break;                                
            case SPELL.HEAL_FRIEND_HERO:
                movePosition = gameManager.enemyHero;            
                break;                                
        }
        // カードの移動（カードの種類に応じた移動先に対応）
        yield return StartCoroutine(card.movement.MoveToField(movePosition));

        card.UseSpellTo(target);
    }
}
