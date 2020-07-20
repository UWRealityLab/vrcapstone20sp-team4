using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    private Text textInstructions;

    // Start is called before the first frame update
    void Start()
    {
        textInstructions = GameObject.Find("MainInstructions").GetComponent<Text>();
    }

    void updateText(string text) {
        textInstructions.text = text;
    }
}
