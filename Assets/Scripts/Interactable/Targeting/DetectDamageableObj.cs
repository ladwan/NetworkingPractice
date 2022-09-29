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


        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Damageable")
            {
                detectionCubeMeshRenderer.material = targetFoundMat;
                damageableObj = other.gameObject;
                activeTargetCube.SetActive(true);
                LocalStoredNetworkData.localPlayerAttackConfirmButton.interactable = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == "Damageable")
            {
                detectionCubeMeshRenderer.material = noTargetMat;
                damageableObj = null;
                activeTargetCube.SetActive(false);
                LocalStoredNetworkData.localPlayerAttackConfirmButton.interactable = false;
            }
        }

        private void OnMouseDown()
        {
            if (damageableObj)
            {
                fireAbilityEvent?.Invoke();
            }
        }
    }
}
