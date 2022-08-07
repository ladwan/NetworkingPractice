using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetDieImage : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> uiDieImages = new List<GameObject>();

    protected void OnEnable()
    {
        int value = DistributedDieValue.distributedDieRollValue;
        Debug.Log(value + " Hey this is what value equals !");
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
    }


    private void DisableDieImages()
    {
        for (int i = 0; i < uiDieImages.Count; i++)
        {
            uiDieImages[i].SetActive(false);
        }
    }
}
