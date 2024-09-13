using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using UnityEngine.Networking;
using System;
using System.IO;
using System.IO.Compression;
using UnityEngine.Events;

public class Update_Launch : MonoBehaviour
{
    [SerializeField] private Button downloadBtn;
    [SerializeField] private Button playBtn;
    [SerializeField] private Text error_text;
    [SerializeField] private Text progress_text;

    string URL = "https://github.com/syl3n7/MyLittleExploree/releases/download/0.54/MyLittleExploreeV0.54.zip";

    void Awake()
    {
        //Makes the button to change its function to be launching the executable of the game.
        Button btn = downloadBtn.GetComponent<Button>();
        btn.onClick.AddListener(Launch);
        Debug.developerConsoleEnabled = true;
        Debug.developerConsoleVisible = true;
        Button btn2 = playBtn.GetComponent<Button>();
        btn2.onClick.AddListener(ExecuteNow);
    }

    void ExecuteNow()
    {
        Process.Start("Resources/MyLittleExploree/MyLittleExploree.exe", "");
    }

    void Launch()
    {
        Debug.Log("Self-update!");
        string launchPath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/'));
        StartCoroutine(Exec_Updater(URL, launchPath));
    }

    private IEnumerator Exec_Updater(string URL, string launchPath)
    {
        //Download and extract a zip file from somewhere and then call the extract funcion.

        UnityWebRequest last_version = UnityWebRequest.Get(URL);
        yield return last_version.SendWebRequest();
        progress_text.text = last_version.downloadedBytes.ToString();
        yield return new WaitForSecondsRealtime(1); 
        progress_text.text = last_version.downloadedBytes.ToString();
        yield return new WaitForSecondsRealtime(1); 
        progress_text.text = last_version.downloadedBytes.ToString();
        yield return new WaitForSecondsRealtime(1); 

        if (!last_version.downloadHandler.isDone) error_text.text = "Communication Error: "+ last_version.error;
        else 
        {

            File.WriteAllBytes("Resources/downloaded.zip", last_version.downloadHandler.data);
            
            ZipFile.ExtractToDirectory("Resources/downloaded.zip", "Resources/MyLittleExploree");

            playBtn.gameObject.GetComponent<Button>().interactable = true;
            downloadBtn.gameObject.GetComponent<Button>().interactable = false;

        }
    }
    
}
