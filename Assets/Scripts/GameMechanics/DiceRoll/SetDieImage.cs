using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetDieImage : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> uiDieImages = new List<GameObject>();

    private void Awake()
    {
        int value = DistributedDieValue.distributedDieRollValue;
        if (value != 0)
        {
            switch (value)
            {
                case 1:
                    uiDieImages[value - 1].SetActive(true);
                    break;

                case 2:
                    uiDieImages[value - 1].SetActive(true);
                    break;

                case 3:
                    uiDieImages[value - 1].SetActive(true);
                    break;

                case 4:
                    uiDieImages[value - 1].SetActive(true);
                    break;

                case 5:
                    uiDieImages[value - 1].SetActive(true);
                    break;

                case 6:
                    uiDieImages[value - 1].SetActive(true);
                    break;
            }
        }
    }
}
