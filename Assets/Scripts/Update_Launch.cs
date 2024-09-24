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
    
    private string game_URL = "https://steelchunk.eu/releases/latest.zip";
    private string game_zip_path = Application.streamingAssetsPath + "/MyLittleExploreeV0.54.zip";
    private string game_folder_path = Application.streamingAssetsPath + "/MyLittleExploree";
    private string game_exe_path = Application.streamingAssetsPath + "/MyLittleExploree/MyLittleExploreeV0.54.exe";
        
    void Awake()
    {
        Debug.developerConsoleEnabled = true;
        Debug.developerConsoleVisible = true;
        Button btn = downloadBtn.GetComponent<Button>();
        btn.onClick.AddListener(check_for_update);
        downloadBtn.gameObject.GetComponent<Button>().interactable = false;
        Button btn2 = playBtn.GetComponent<Button>();
        btn2.onClick.AddListener(executeNow);
        playBtn.gameObject.GetComponent<Button>().interactable = false;
    }
    
    void game_Update()
    {
        Debug.Log("updating game!");
        StartCoroutine(exec_Updater(game_URL, game_zip_path, game_folder_path));
    }

    void executeNow()
    {
        Debug.Log("execute now!");
        Process foo = new Process();
        foo.StartInfo.FileName = "MyLittleExploreeV0.54.exe";
        foo.StartInfo.Arguments = game_exe_path;
        foo.Start();
    }

    private IEnumerator exec_Updater(string URL, string zip_path, string folder_path)
    {
        //Download and extract a zip file from somewhere and then call the extract funcion.
        if (URL == game_URL)
        {
            UnityWebRequest last_version = UnityWebRequest.Get(URL);
            yield return last_version.SendWebRequest();
            progress_text.text = last_version.downloadProgress.ToString();

            if (!last_version.downloadHandler.isDone)
            {
                error_text.text = "Communication Error: " + last_version.error;
                downloadBtn.gameObject.GetComponent<Button>().interactable = true;
            }
            else
            {
                if (last_version.downloadHandler.isDone && last_version.downloadHandler.nativeData.Length > 0)
                {
                    if (File.Exists(folder_path)) File.Delete(folder_path);
                    else File.Create(folder_path);

                    File.WriteAllBytes(zip_path, last_version.downloadHandler.data);

                    ZipFile.ExtractToDirectory(zip_path, folder_path);

                    playBtn.gameObject.GetComponent<Button>().interactable = true;
                    downloadBtn.gameObject.GetComponent<Button>().interactable = false;
                }
            }
        }
        else
        {
            Application.Quit();
        }
    }
}
