using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class Update_Launch : MonoBehaviour
{
    [SerializeField] private Button launchBtn;
    void Awake()
    {
        //Makes the button to change its function to be launching the executable of the game.
        Button btn = launchBtn.GetComponent<Button>();
        btn.onClick.AddListener(Launch);
    }

    void Launch()
    {
        Debug.Log("Launch Success!");
        Process.Start("cmd.exe", "/c start launch");
        //https://github.com/syl3n7/MyLittleExploree/releases/download/0.54/MyLittleExploreeV0.54.zip
    }
    
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
