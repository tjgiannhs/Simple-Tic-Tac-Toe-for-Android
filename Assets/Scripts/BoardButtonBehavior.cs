using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BoardButtonBehavior : MonoBehaviour
{

    TextMeshProUGUI myText;
    GameManagerBehavior gameManagerBehavior;

    // Start is called before the first frame update
    void Start()
    {
        myText = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        gameManagerBehavior = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManagerBehavior>();
    }

    public void OnClick()
    {
        if(!gameManagerBehavior.CanPlayerAct()) return;
        
        if(myText.text == "")
        {
            myText.text = "X";
            gameManagerBehavior.AfterPlayerAction();
        }
    }
}
