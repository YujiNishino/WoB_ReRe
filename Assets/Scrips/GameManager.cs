using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// ゲームシナリオを統括
public class GameManager : MonoBehaviour
{
    public GamePlayerManager player;
    public GamePlayerManager enemy;
    [SerializeField] AI enemyAI;  
    [SerializeField] UIManager uiManager;

    public Transform playerHandTransform; 
    public Transform playerFieldTransform;
    public Transform enemyHandTransform; 
    public Transform enemyFieldTransform;  

    [SerializeField] CardController cardPerfab;  

    public bool isPlayerTurn;

    // Hero
    public Transform playerHero;
    public Transform enemyHero;

    // 時間管理
    int timeCount;

    // シングルトン化（どこからでもアクセスを可能にする）
    public static GameManager instance;
    private void Awake()
    {
        if(instance == null) 
        {
            instance = this;
        }
    }

    void Start()
    {
        StartGame();
    }

    /// <summary>
    ///  試合開始
    /// </summary>
    void StartGame()
    {
        // リザルト画面を隠す
        uiManager.HideResultPanel();

        // プレイヤーの初期化
        player.Init(new List<int> {0, 1, 2, 3, 4, 5, 6, 7, 8, 9});
        enemy.Init(new List<int> {0, 1, 2, 3, 4, 6, 5, 8, 9});

        // ヒーローの初期体力
        uiManager.ShowHeroHp(player.heroHp, enemy.heroHp);

        // Costの初期値
        uiManager.ShowManaCost(player.baseManaCost, player.manaCost, enemy.baseManaCost, enemy.manaCost);

        // 制限時間
        timeCount = 30;
        uiManager.UpdateTime(timeCount);

        // カードをそれぞれ３枚配る
        SettingInitHand();
        isPlayerTurn = true;
        TurnCalc();
    }

    /// <summary>
    /// マナコスト減算
    /// </summary>
    /// <param name="cost">減算コスト</param>
    /// <param name="isPlayerCard">プレイヤーカードか？</param>
    public void ReduceManaCost(int cost, bool isPlayerCard)
    {
        if (isPlayerCard)
        {
            player.manaCost -= cost;
        }
        else
        {
            enemy.manaCost -= cost;
        }
        uiManager.ShowManaCost(player.baseManaCost, player.manaCost, enemy.baseManaCost, enemy.manaCost);
    }

    /// <summary>
    ///  試合のリセット
    /// </summary>
    public void Restart()
    {
        // handとFiledのカードを削除
        foreach (Transform card in playerHandTransform)
        {
            Destroy(card.gameObject);
        }
        foreach (Transform card in playerFieldTransform)
        {
            Destroy(card.gameObject);
        }
        foreach (Transform card in enemyHandTransform)
        {
            Destroy(card.gameObject);
        }
        foreach (Transform card in enemyFieldTransform)
        {
            Destroy(card.gameObject);
        }

        StartGame();
    }

    /// <summary>
    ///  手札を配る
    /// </summary>
    void SettingInitHand()
    {
        for(int i = 0; i < 3; i++) {
            GiveCardToHand(player.deck, playerHandTransform);
            GiveCardToHand(enemy.deck, enemyHandTransform);
        }
    }

    /// <summary>
    /// デッキからカードを引く
    /// </summary>
    /// <param name="deck"></param>
    /// <param name="hand"></param> 
    void GiveCardToHand(List<int> deck, Transform hand)
    {
        if (deck.Count == 0)
        {
            // TODO：デッキアウトは負け
            return;
        }

        // TODO：カードドローイベント
        
        int cardID = deck[0];
        deck.RemoveAt(0);
        CreateCard(cardID, hand);
    }
    
    /// <summary>
    /// カードの生成
    /// </summary>
    /// <param name="cardID"></param>
    /// <param name="hand"></param> 
    void CreateCard(int cardID, Transform hand)
    {
        // カードコントローラの実体化（オブジェクト, 親要素, 相対値）
        CardController card = Instantiate(cardPerfab, hand, false);

        // サイズの変更（縮小）
        card.transform.localScale = new Vector2(0.75f, 0.75f);

        // 生成されたカードの持ち主を設定
        if (hand.name == "PlayerHand")
        {
            card.Init(cardID, true);            
        }
        else
        {
            card.Init(cardID, false);
        }

    }

    /// <summary>
    ///  ターン切り替え
    /// </summary>
    void TurnCalc()
    {
        // コルーチン全停止
        StopAllCoroutines();

        // TODO：ターン切り替えアニメーション表示

        // TODO：ターン開始イベント
        CardController[] cards = GetFriendFieldCards(isPlayerTurn);
        foreach(CardController card in cards) 
        {
            card.TriggerTurnStartEvent();
        }

        // カウントダウンタイマーON
        StartCoroutine(CountDown());
        if (isPlayerTurn) 
        {
            PlayerTurn();
        } 
        else
        {
            StartCoroutine(enemyAI.EnemyTurn());
        }
    }

    /// <summary>
    /// カウントダウンタイマー
    /// </summary>
    IEnumerator CountDown()
    {
        // タイムリセット
        timeCount = 30;
        uiManager.UpdateTime(timeCount);

        // カウントダウン開始
        while(timeCount > 0)
        {
            // １秒待機
            yield return new WaitForSeconds(1);
            timeCount--;
            uiManager.UpdateTime(timeCount);
        }

        // TODO：ドラッグしているカードをもとの位置に戻す

        CangeTurn();
    }

    /// <summary>
    /// 相手フィールドの全カードを取得
    /// </summary>
    /// <param name="isPlayer"></param>
    /// <returns></returns>
    public CardController[] GetEnemyFieldCards(bool isPlayer)
    {
        if (isPlayer)
        {
            return enemyFieldTransform.GetComponentsInChildren<CardController>();            
        }
        else
        {
            return playerFieldTransform.GetComponentsInChildren<CardController>();  
        }

    }
    /// <summary>
    /// 自分フィールドの全カードを取得
    /// </summary>
    /// <param name="isPlayer"></param>
    /// <returns></returns>
    public CardController[] GetFriendFieldCards(bool isPlayer)
    {
        if (isPlayer)
        {
            return playerFieldTransform.GetComponentsInChildren<CardController>();  
        }
        else
        {
            return enemyFieldTransform.GetComponentsInChildren<CardController>();            
        }

    }
    /// <summary>
    ///  ターンエンドボタン押下
    /// </summary>
    public void OnClickTurnEndButton()
    {
        if (isPlayerTurn)
        {
            CangeTurn();
        }
    }

    /// <summary>
    /// ターン交代
    /// </summary>
    public void CangeTurn()
    {
        // TODO：ターン終了イベント

        // ターン切り替え
        isPlayerTurn = !isPlayerTurn;
        // ボタンの活性非活性
        uiManager.HideTrunEndButton(isPlayerTurn);

        // 全フィールドカードの攻撃可否を不可にリセット
        CardController[] playerFieldCardList = playerFieldTransform.GetComponentsInChildren<CardController>();
        SettingCanAttackView(playerFieldCardList, false);
        CardController[] enemyFieldCardList = enemyFieldTransform.GetComponentsInChildren<CardController>();
        SettingCanAttackView(enemyFieldCardList, false);

        if (isPlayerTurn)
        {
            player.incrementManaCost();
            GiveCardToHand(player.deck, playerHandTransform);
        }
        else
        {
            enemy.incrementManaCost();
            GiveCardToHand(enemy.deck, enemyHandTransform);
        }
        uiManager.ShowManaCost(player.baseManaCost, player.manaCost, enemy.baseManaCost, enemy.manaCost);
        TurnCalc();
    }

    /// <summary>
    /// フィールドカード 攻撃可否設定
    /// </summary>
    /// <param name="fieldCardList">フィールドカードリスト</param>
    /// <param name="canAttack">true:攻撃可能 false:攻撃不可</param>
    public void SettingCanAttackView(CardController[] fieldCardList, bool canAttack)
    {
        foreach (CardController card in fieldCardList) 
        {
            // cardを攻撃可能にする
            card.SetCanAttack(canAttack);
        }
    }

    /// <summary>
    /// 自分のターン
    /// </summary>
    void PlayerTurn()
    {
        Debug.Log("PlayerTurn");
        // フィールドカードを攻撃可能にする
        CardController[] playerFieldCardList = playerFieldTransform.GetComponentsInChildren<CardController>();
        SettingCanAttackView(playerFieldCardList, true);
    }

    /// <summary>
    /// カード交戦処理
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="defender"></param>
    public void CardsBattle(CardController attacker, CardController defender)
    {
        // TODO：ユニット交戦イベント

        // 対戦開始
        Debug.Log($"attacker:{attacker.model.hp} / defender:{defender.model.hp}");
        attacker.Attack(attacker);
        attacker.Attack(defender);
        Debug.Log($"attacker:{attacker.model.hp} / defender:{defender.model.hp}");
        // 結果反映
        attacker.CheckAlive();

        // TODO：リワード判定

        defender.CheckAlive();
    }

    /// <summary>
    /// Heroへの攻撃
    /// </summary>
    /// <param name="attacker"></param>
    public void AttackToHero(CardController attacker)
    {
        // TODO：プレイヤー攻撃イベント

        if (attacker.model.isPlayerCard)
        {
            enemy.heroHp -= attacker.model.at;
        }
        else
        {
            player.heroHp -= attacker.model.at;
        }
        attacker.SetCanAttack(false);
        uiManager.ShowHeroHp(player.heroHp, enemy.heroHp);
    }    

    /// <summary>
    /// Heroへの回復
    /// </summary>
    /// <param name="healer"></param>//  
    public void HealToHero(CardController healer)
    {
        if (healer.model.isPlayerCard)
        {
            player.heroHp += healer.model.at;
        }
        else
        {
            enemy.heroHp += healer.model.at;
        }
        uiManager.ShowHeroHp(player.heroHp, enemy.heroHp);
    } 

    /// <summary>
    /// 勝敗判定
    /// </summary>
    public void CheckHeroHP()
    {
        if (player.heroHp <= 0 || enemy.heroHp <= 0)
        {
            ShowResultPanel(player.heroHp);
        }
    }

    /// <summary>
    /// リザルト画面の表示
    /// </summary>
    /// <param name="heroHp"></param> 
    void ShowResultPanel(int heroHp)
    {
        StopAllCoroutines();
        uiManager.ShowResultPanel(heroHp);
    }



}
