using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpdateWinLossText : MonoBehaviour
{
    [SerializeField] private TMP_Text winText = null;
    [SerializeField] private TMP_Text lossText = null;


    private void OnEnable()
    {
        winText.text = PlayerPrefs.GetInt(Constants.WinTextKey).ToString();
        lossText.text = PlayerPrefs.GetInt(Constants.LossTextKey).ToString();
    }
}
