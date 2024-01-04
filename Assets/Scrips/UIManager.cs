using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject resultPanel;  
    [SerializeField] Text resultText;         
    [SerializeField] Text playerHeroHpText;
    [SerializeField] Text enemyHeroHpText;
    [SerializeField] Slider playerBaseManaCost;
    [SerializeField] Slider playerNowManaCost;
    [SerializeField] Text playerCostText;
    [SerializeField] Text enemyCostText;    
    [SerializeField] GameObject trunEndButton;
    [SerializeField] Text timeCountText;
    
    public void HideResultPanel()
    {
        resultPanel.SetActive(false);
    }

    public void UpdateTime(int timeCount)
    {
        timeCountText.text = timeCount.ToString();
    }

    public void HideTrunEndButton(bool isPlayerTurn)
    {
        trunEndButton.SetActive(isPlayerTurn);
    }


    public void ShowManaCost(int playerBaseManaCost, int playerNowManaCost, int enemyBaseManaCost, int enemyNowManaCost)
    {
        this.playerBaseManaCost.value = playerBaseManaCost;
        this.playerNowManaCost.value = playerNowManaCost;
        playerCostText.text = playerNowManaCost.ToString() + "/" + playerBaseManaCost.ToString();
        enemyCostText.text = enemyNowManaCost.ToString() + "/" + enemyBaseManaCost.ToString();
    }

    // HeroHPの表示更新
    public void ShowHeroHp(int playerHeroHp, int enemyHeroHp)
    {
        playerHeroHpText.text = playerHeroHp.ToString();
        enemyHeroHpText.text = enemyHeroHp.ToString();
    }    

    public void ShowResultPanel(int heroHp)
    {
        resultPanel.SetActive(true);
        if (heroHp <= 0)
        {
            resultText.text = "Lose";
        }
        else
        {
            resultText.text = "Win";                
        }        
    }

}
