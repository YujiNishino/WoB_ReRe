using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    private bool firstPush = false;

    public void PressStart()
    {
        Debug.Log("PressStartButton!!");
        if (!firstPush)
        {
            // 次のシーンへ行く
            Debug.Log("Next Scene!!");   
            SceneManager.LoadScene("deckScene");         
            firstPush = true;
        }

    }
}
