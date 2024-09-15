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

    private string app_URL = "https://steelchunk.eu/releases/latest.zip";
    private string app_zip_path = Application.streamingAssetsPath + "/latest.zip";
    private string app_folder_path = Application.streamingAssetsPath + "/";
    private string app_exe_path = Application.streamingAssetsPath + "/Game_Launcher.exe";
    private string game_URL = "https://github.com/syl3n7/MyLittleExploree/releases/download/0.54/MyLittleExploreeV0.54.zip";
    private string game_zip_path = Application.streamingAssetsPath + "/MyLittleExploreeV0.54.zip";
    private string game_folder_path = Application.streamingAssetsPath + "/MyLittleExploree";
    private string game_exe_path = Application.streamingAssetsPath + "/MyLittleExploree/MyLittleExploreeV0.54.exe";
        
    void Awake()
    {
        //Makes the button to change its function to be launching the executable of the game.
        Button btn = downloadBtn.GetComponent<Button>();
        btn.onClick.AddListener(game_Update);
        Debug.developerConsoleEnabled = true;
        Debug.developerConsoleVisible = true;
        Button btn2 = playBtn.GetComponent<Button>();
        btn2.onClick.AddListener(executeNow);
    }

    void Start()
    {
        self_update();
    }
    
    void self_update()
    {
        Debug.Log("Self-update!");
        StartCoroutine(exec_Updater(app_URL, app_zip_path, app_folder_path));
    }
    
    void game_Update()
    {
        Debug.Log("updating game!");
        StartCoroutine(exec_Updater(game_URL, game_zip_path, game_folder_path));
    }
    
    void executeNow()
    {
        Debug.Log("execute now!");
        Process.Start(game_exe_path);
    }

    private IEnumerator exec_Updater(string URL, string zip_path, string folder_path)
    {
        //Download and extract a zip file from somewhere and then call the extract funcion.

        UnityWebRequest last_version = UnityWebRequest.Get(URL);
        yield return last_version.SendWebRequest();

        if (!last_version.downloadHandler.isDone) error_text.text = "Communication Error: "+ last_version.error;
        else 
        {

            File.WriteAllBytes(zip_path, last_version.downloadHandler.data);
            
            ZipFile.ExtractToDirectory(zip_path, folder_path);

            playBtn.gameObject.GetComponent<Button>().interactable = true;
            downloadBtn.gameObject.GetComponent<Button>().interactable = false;

        }
    }
    
}
