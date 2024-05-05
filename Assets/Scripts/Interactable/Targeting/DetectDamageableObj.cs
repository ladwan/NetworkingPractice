using ForeverFight.Networking;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace ForeverFight.Interactable
{
    public class DetectDamageableObj : MonoBehaviour
    {
        [SerializeField]
        private MeshRenderer detectionCubeMeshRenderer = null;
        [SerializeField]
        private Material noTargetMat = null;
        [SerializeField]
        private Material targetFoundMat = null;
        [SerializeField]
        private GameObject activeTargetCube = null;
        [SerializeField]
        private bool isTargetingSelf = false;


        [NonSerialized]
        private GameObject damageableObj = null;


        public GameObject DamageableObj { get => damageableObj; set => damageableObj = value; }


        protected void OnEnable()
        {
            if (isTargetingSelf)
            {
                LocalStoredNetworkData.damageableObjectDetected = true;
            }
        }

        protected void OnDisable()
        {
            detectionCubeMeshRenderer.material = noTargetMat;
            damageableObj = null;
            activeTargetCube.SetActive(false);
            LocalStoredNetworkData.damageableObjectDetected = false;
        }


        private void OnTriggerEnter(Collider other)
        {
            if (!isTargetingSelf)
            {
                if (other.gameObject.tag == "Damageable")
                {
                    detectionCubeMeshRenderer.material = targetFoundMat;
                    damageableObj = other.gameObject;
                    activeTargetCube.SetActive(true);
                    LocalStoredNetworkData.damageableObjectDetected = true;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!isTargetingSelf)
            {
                if (other.gameObject.tag == "Damageable")
                {
                    detectionCubeMeshRenderer.material = noTargetMat;
                    damageableObj = null;
                    activeTargetCube.SetActive(false);
                    LocalStoredNetworkData.damageableObjectDetected = false;
                }
            }
        }
    }
}
