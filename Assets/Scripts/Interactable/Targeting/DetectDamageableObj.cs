using ForverFight.Networking;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace ForverFight.Interactable
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
        private UnityEvent fireAbilityEvent = new UnityEvent();

        [NonSerialized]
        private GameObject damageableObj = null;


        public GameObject DamageableObj { get => damageableObj; set => damageableObj = value; }


        protected void OnDisable()
        {
            detectionCubeMeshRenderer.material = noTargetMat;
            damageableObj = null;
            activeTargetCube.SetActive(false);
            LocalStoredNetworkData.damageableObjectDetected = false;
        }


        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Damageable")
            {
                detectionCubeMeshRenderer.material = targetFoundMat;
                damageableObj = other.gameObject;
                activeTargetCube.SetActive(true);
                LocalStoredNetworkData.damageableObjectDetected = true;
            }
        }

        private void OnTriggerExit(Collider other)
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
