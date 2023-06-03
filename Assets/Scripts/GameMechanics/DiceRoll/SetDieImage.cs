using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ForverFight.Ui;
using ForverFight.Networking;

public class SetDieImage : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> uiDieImages = new List<GameObject>();
    [SerializeField]
    private TMP_Text tmpApDisplay = null;

    protected void OnEnable()
    {
        int value = DistributedDieValue.distributedDieRollValue;
        if (value != 0)
        {
            switch (value)
            {
                case 1:
                    DisableDieImages();
                    uiDieImages[value - 1].SetActive(true);
                    break;

                case 2:
                    DisableDieImages();
                    uiDieImages[value - 1].SetActive(true);
                    break;

                case 3:
                    DisableDieImages();
                    uiDieImages[value - 1].SetActive(true);
                    break;

                case 4:
                    DisableDieImages();
                    uiDieImages[value - 1].SetActive(true);
                    break;

                case 5:
                    DisableDieImages();
                    uiDieImages[value - 1].SetActive(true);
                    break;

                case 6:
                    DisableDieImages();
                    uiDieImages[value - 1].SetActive(true);
                    break;
            }
        }

        tmpApDisplay.text = LocalStoredNetworkData.localPlayerCurrentAP.ToString();
    }


    private void DisableDieImages()
    {
        for (int i = 0; i < uiDieImages.Count; i++)
        {
            uiDieImages[i].SetActive(false);
        }
    }
}
