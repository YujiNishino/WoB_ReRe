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

    // 試合開始
    void StartGame()
    {
        // リザルト画面を隠す
        uiManager.HideResultPanel();

        // プレイヤーの初期化
        player.Init(new List<int> {0, 1, 2, 3, 4, 5, 6, 7, 8, 9});
        enemy.Init(new List<int> {0, 1, 2, 3, 4, 5, 8, 9});

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

    // 試合再開
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

    // 手札を配る
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
            return;
        }
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

    // ターン処理
    void TurnCalc()
    {
        // コルーチン全停止
        StopAllCoroutines();
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

    // カウントダウンタイマー
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

    // カードの対戦
    public void CardsBattle(CardController attacker, CardController defender)
    {
        // 対戦開始
        Debug.Log($"attacker:{attacker.model.hp} / defender:{defender.model.hp}");
        attacker.Attack(attacker);
        attacker.Attack(defender);
        Debug.Log($"attacker:{attacker.model.hp} / defender:{defender.model.hp}");
        // 結果反映
        attacker.CheckAlive();
        defender.CheckAlive();
    }

    // Heroへの攻撃
    public void AttackToHero(CardController attacker)
    {
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

    // Heroへの回復
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

    // 勝敗判定
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
