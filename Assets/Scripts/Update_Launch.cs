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

public class Update_Launch : MonoBehaviour
{
    [SerializeField] private Button bttn;
    //[SerializeField] private Text error_text;
    //[SerializeField] private Text progress_text;

    private string game_URL = "https://steelchunk.eu/games/releases/latest.zip";
    private string game_zip_path = Application.dataPath + "/latest.zip";
    private string game_folder_path = Application.dataPath + "/MyLittleExploree";
    private string game_exe_path = Application.dataPath + "/MyLittleExploree/MyLittleExploree.exe";

    void Awake()
    {
        //Debug.developerConsoleEnabled = true;
        //Debug.developerConsoleVisible = true;
        Application.targetFrameRate = 60;

        if (File.Exists(game_zip_path) && Directory.Exists(game_folder_path) && File.Exists(game_exe_path))
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
    {
        if (Process.GetProcessesByName("MyLittleExploree").Length > 0)
        {
            //error_text.text = "already running, disabling button";
            bttn.gameObject.GetComponent<Button>().interactable = false;
        }
        else
        {
            //error_text.text = "not running, enabling button";
            bttn.gameObject.GetComponent<Button>().interactable = true;
        }
    }

    void game_Update()
    {
        if (File.Exists(game_zip_path)) File.Delete(game_zip_path);
        if (Directory.Exists(game_zip_path)) Directory.Delete(game_zip_path, true);

        //Debug.Log("updating game!");
        StartCoroutine(downloader(game_URL, game_zip_path));

        //reprogram the download button to start the game.
        bttn.gameObject.GetComponent<Button>().interactable = false;
        bttn.GetComponent<Button>().onClick.RemoveAllListeners();
        bttn.GetComponent<Button>().onClick.AddListener(executeNow);
        bttn.gameObject.GetComponentInChildren<TMP_Text>().text = "Play Now!";
        bttn.gameObject.GetComponent<Button>().interactable = true;
    }

    void executeNow()
    {
        if (!Directory.Exists(game_folder_path) || !File.Exists(game_zip_path))
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
                File.WriteAllBytes(zip_path, last_version.downloadHandler.data);
                ZipFile.ExtractToDirectory(game_zip_path, game_folder_path);
                bttn.GetComponent<Button>().onClick.RemoveAllListeners();
                bttn.GetComponent<Button>().onClick.AddListener(executeNow);
                bttn.gameObject.GetComponentInChildren<TMP_Text>().text = "Play Now!";
                bttn.gameObject.GetComponent<Button>().interactable = true;
            }
        }
    }
}


