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
using System.Runtime.ExceptionServices;
using Unity.VisualScripting;
using static System.Net.WebRequestMethods;

public class Update_Launch : MonoBehaviour
{
    [SerializeField] private Button bttn;
    //[SerializeField] private Text error_text;
    //[SerializeField] private Text progress_text;
    [SerializeField] private TMPro.TMP_Dropdown dpdown;

    private string game_URL = "https://steelchunk.eu/games/releases/latest.zip";
    private string zip_path = Application.dataPath + "/latest.zip";
    private string game_folder_path = Application.dataPath + "/MyLittleExploree";
    private string game_exe_path = Application.dataPath + "/MyLittleExploree/MyLittleExploree.exe";

    private string[] URLs = {"https://steelchunk.eu/games/releases/MyLittleExploree/latest.zip", "https://steelchunk.eu/games/releases/CatchMeIfYouCan/latest.zip", "https://steelchunk.eu/games/releases/CloudShooter/latest.zip"};
    private string[] games_folder_path = {"/MyLittleExploree.zip", "/CatchMeIfYouCan", "/CloudShooter"};
    private string[] games_exe_path = {"/MyLittleExploree/MyLittleExplore.exe", "/CatchMeIfYouCan/CatchMeIfYouCan.exe", "/CloudShooter/CloudShooter.exe"};

    void Awake()
    {
        //Debug.developerConsoleEnabled = true;
        //Debug.developerConsoleVisible = true;
        Application.targetFrameRate = 60;

        if (System.IO.File.Exists(zip_path) && Directory.Exists(game_folder_path) && System.IO.File.Exists(game_exe_path))
        {
            //error_text.text = "Files already present, you can play now";
            //need to implement a md5 check to see if files are older than whats on server, so that you only download it if theres a new version.
            bttn.gameObject.GetComponent<Button>().interactable = false;
            bttn.GetComponent<Button>().onClick.RemoveAllListeners();
            bttn.GetComponent<Button>().onClick.AddListener(executeNow);
            bttn.gameObject.GetComponentInChildren<TMP_Text>().text = "Play Now!";
            bttn.gameObject.GetComponent<Button>().interactable = true;
        }
        else
        {
            bttn.gameObject.GetComponentInChildren<TMP_Text>().text = "Download";
            bttn.gameObject.GetComponent<Button>().interactable = true;
            bttn.GetComponent<Button>().onClick.RemoveAllListeners();
            bttn.GetComponent<Button>().onClick.AddListener(game_Update);
        }
    }

    void Update()
    { //this method is probably really cpu intensive but i dont know a solution without rewritting the games.
        if (Process.GetProcessesByName("MyLittleExploree").Length > 0 || Process.GetProcessesByName("CatchMeIfYouCan").Length > 0 || Process.GetProcessesByName("CloudShooter").Length > 0)
        {
            //error_text.text = "already running, disabling button";
            bttn.gameObject.GetComponent<Button>().interactable = false;
            dpdown.interactable = false;
        }
        else
        {
            //error_text.text = "not running, enabling button";
            bttn.gameObject.GetComponent<Button>().interactable = true;
            dpdown.interactable = true;
        }
        
        //this is not ok, need to optimize.
        if (dpdown.value == 0) 
        {
            Debug.Log("Downloading MLE"); //igualar vars para MLE
            game_URL = URLs[0];
            game_folder_path = games_folder_path[0];
            game_exe_path = games_exe_path[0];
        }
        
        if (dpdown.value == 1) 
        {
            Debug.Log("Downloading CMIYC"); //igualar vars para CMIYC
            game_URL = URLs[1];
            game_folder_path = games_folder_path[1];
            game_exe_path = games_exe_path[1];
        }
        
        if (dpdown.value == 2) 
        {
            Debug.Log("Downloading CLS"); //igualar vars para CLS
            game_URL = URLs[2];
            game_folder_path = games_folder_path[2];
            game_exe_path = games_exe_path[2];
        }
    }

    void game_Update()
    {
        if (System.IO.File.Exists(zip_path)) System.IO.File.Delete(zip_path);
        if (Directory.Exists(zip_path)) Directory.Delete(zip_path, true);

        //Debug.Log("updating game!");
        StartCoroutine(downloader(game_URL, zip_path));

        //reprogram the download button to start the game.
        bttn.gameObject.GetComponent<Button>().interactable = false;
        bttn.GetComponent<Button>().onClick.RemoveAllListeners();
        bttn.GetComponent<Button>().onClick.AddListener(executeNow);
        bttn.gameObject.GetComponentInChildren<TMP_Text>().text = "Play Now!";
        bttn.gameObject.GetComponent<Button>().interactable = true;
    }

    void executeNow()
    {
        if (!Directory.Exists(game_folder_path) || !System.IO.File.Exists(zip_path))
        {
            //error_text.text = "Files are not present, downloading again!";
            bttn.gameObject.GetComponentInChildren<TMP_Text>().text = "Downloading..";
            bttn.gameObject.GetComponent<Button>().interactable = false;
            game_Update();
        }
        else
        {
            Process foo = new Process();
            foo.StartInfo.FileName = game_exe_path;
            foo.Start();
        }

    }

    private IEnumerator downloader(string URL, string zip_path)
    {
        bttn.gameObject.GetComponent<Button>().interactable = false;
        //Download and extract a zip file from somewhere and then call the extract funcion.
        UnityWebRequest last_version = UnityWebRequest.Get(URL);
        yield return last_version.SendWebRequest();

        if (!last_version.downloadHandler.isDone)
        {
            //error_text.text = "Communication Error: " + last_version.error;
            bttn.gameObject.GetComponentInChildren<TMP_Text>().text = "Download";
        }
        else
        {
            if (last_version.downloadHandler.isDone && last_version.downloadHandler.nativeData.Length > 0)
            {
                yield return last_version.downloadHandler.data;

                //gather the downloaded data from RAM and put it into a zip file.
                System.IO.File.WriteAllBytes(zip_path, last_version.downloadHandler.data);
                ZipFile.ExtractToDirectory( zip_path, game_folder_path);
                bttn.GetComponent<Button>().onClick.RemoveAllListeners();
                bttn.GetComponent<Button>().onClick.AddListener(executeNow);
                bttn.gameObject.GetComponentInChildren<TMP_Text>().text = "Play Now!";
                bttn.gameObject.GetComponent<Button>().interactable = true;
            }
        }
    }
}


