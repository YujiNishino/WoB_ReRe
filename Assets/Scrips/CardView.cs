using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text hpText;
    [SerializeField] Text atText;
    [SerializeField] Text costText;
    [SerializeField] Image iconImage;
    [SerializeField] GameObject selectablePanel;
    [SerializeField] GameObject guardianPanel;
    [SerializeField] GameObject maskPanel;

    public void SetCard(CardModel cardModel) 
    {
        nameText.text = cardModel.name;
        hpText.text = cardModel.hp.ToString();
        atText.text = cardModel.at.ToString();
        costText.text = cardModel.cost.ToString();
        iconImage.sprite = cardModel.icon;
        maskPanel.SetActive(!cardModel.isPlayerCard);

        // 基本アビリティの確認（クイック）
        if (cardModel.isBaseAbility(BASE_ABILITY.GUARDIAN))
        {
            guardianPanel.SetActive(true);
        }   
        else
        {
            guardianPanel.SetActive(false);            
        }

        // スペルカード確認
        if (cardModel.spell != SPELL.NONE)
        {
            hpText.gameObject.SetActive(false);
        }
    }

    public void Show()
    {
        maskPanel.SetActive(false);
    }

    public void Refresh(CardModel cardModel) 
    {
        hpText.text = cardModel.hp.ToString();
        atText.text = cardModel.at.ToString();
    }

    public void SetActiveSelectabelePanel(bool flag)
    {
        selectablePanel.SetActive(flag);
    }

}
