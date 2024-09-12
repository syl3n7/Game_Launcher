using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor.PackageManager;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using UnityEngine.Networking;

public class Update_Launch : MonoBehaviour
{
    [SerializeField] private Button launchBtn;
    string URL = "https://github.com/syl3n7/MyLittleExploree/releases/download/0.54/MyLittleExploreeV0.54.zip";
    public Text Error;
    void Awake()
    {
        //Makes the button to change its function to be launching the executable of the game.
        Button btn = launchBtn.GetComponent<Button>();
        btn.onClick.AddListener(Launch);
        Debug.developerConsoleEnabled = true;
    }

    void Launch()
    {
        Debug.Log("Checking for update!");
        string launchPath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/'));
        StartCoroutine(Exec_Updater(URL, launchPath));
        Debug.Log("Launch Success!");
        //Process.Start("la", "");
    }

    private IEnumerator Exec_Updater(string URL, string launchPath)
    {
        //Download and extract a zip file from somewhere and then call the extract funcion.

        UnityWebRequest last_version = UnityWebRequest.Get(URL);
        yield return last_version.SendWebRequest();
        if (last_version.result != UnityWebRequest.Result.Success) Error.text = "Communication Error: "+ last_version.error;
        else
        {
            Debug.Log(last_version.downloadHandler.text);
        }
        
    }
    
}
