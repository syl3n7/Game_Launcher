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
    [SerializeField] private Button bttn;
    [SerializeField] private Text error_text;
    [SerializeField] private Text progress_text;

    private string game_URL = "https://steelchunk.eu/games/releases/latest.zip";
    private string game_zip_path = Application.dataPath + "/latest.zip";
    private string game_folder_path = Application.dataPath + "/MyLittleExploree";

    void Awake()
    {
        if (File.Exists(game_zip_path) && Directory.Exists(game_folder_path) && File.Exists(game_folder_path + "/MyLittleExploree.exe"))
        {
            error_text.text = "Files already present, you can play now";
            //need to implement a md5 check to see if files are older than whats on server, so that you only download it if theres a new version.
            bttn.gameObject.GetComponent<Button>().interactable = false;
            bttn.GetComponent<Button>().onClick.AddListener(executeNow);
            bttn.gameObject.GetComponentInChildren<TMP_Text>().text = "Play Now!";
            bttn.gameObject.GetComponent<Button>().interactable = true;
        }
        else
        {
            bttn.GetComponent<Button>().onClick.AddListener(game_Update);
        }
        Debug.developerConsoleEnabled = true;
        Debug.developerConsoleVisible = true;
        bttn.gameObject.GetComponent<Button>().interactable = true;
    }

    void game_Update()
    {
        //Debug.Log("updating game!");
        StartCoroutine(exec_Updater(game_URL, game_zip_path, game_folder_path));
    }

    void executeNow()
    {
        //Debug.Log("execute now!");
        if (Directory.Exists(game_folder_path + "/MyLittleExploree_Data") && Directory.Exists(game_folder_path) && File.Exists(game_folder_path + "/MyLittleExploree.exe"))
        {
            Process foo = new Process();
            foo.StartInfo.FileName = game_folder_path + "/MyLittleExploree.exe";
            foo.Start();
        }
        else
        {
            error_text.text = "Files are not present, downloading again!";
            bttn.gameObject.GetComponent<Button>().interactable = false;
            bttn.GetComponent<Button>().onClick.AddListener(executeNow);
            bttn.gameObject.GetComponentInChildren<TMP_Text>().text = "Downloading..";
            game_Update();
        }

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
                bttn.gameObject.GetComponentInChildren<TMP_Text>().text = "Download";
            }
            else
            {
                if (last_version.downloadHandler.isDone && last_version.downloadHandler.nativeData.Length > 0)
                {
                    //if the directory already exists, delete it and the contents inside it.
                    if (Directory.Exists(folder_path)) Directory.Delete(folder_path, true);
                    //else it creates the directory
                    else Directory.CreateDirectory(folder_path);
                    //gather the downloaded data from RAM and put it into a zip file.
                    File.WriteAllBytes(zip_path, last_version.downloadHandler.data);
                    //extract the data from the zip file into the created directory.
                    ZipFile.ExtractToDirectory(zip_path, folder_path);

                    //reprogram the download button to start the game.
                    bttn.gameObject.GetComponent<Button>().interactable = false;
                    bttn.GetComponent<Button>().onClick.AddListener(executeNow);
                    bttn.gameObject.GetComponentInChildren<TMP_Text>().text = "Play Now!";
                    bttn.gameObject.GetComponent<Button>().interactable = true;
                }
            }
        }
        else
        {
            //need to implement exeption and show pop up when error occurs
            Application.Quit();
        }
    }
}
