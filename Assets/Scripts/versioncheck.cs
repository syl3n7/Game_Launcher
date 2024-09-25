using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class checkanddisplayversion : MonoBehaviour
{
    [SerializeField] private TMP_Text tMP_Text;
    private void Start()
    {
        tMP_Text.text = "Version: " + Application.version + "\nGUID: " + Application.buildGUID.ToString();
    }
}
