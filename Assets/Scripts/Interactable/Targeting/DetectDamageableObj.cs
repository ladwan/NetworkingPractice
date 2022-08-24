using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        private GameObject damageableObj = null;


        public GameObject DamageableObj { get => damageableObj; set => damageableObj = value; }


        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Damageable")
            {
                detectionCubeMeshRenderer.material = targetFoundMat;
                damageableObj = other.gameObject;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == "Damageable")
            {
                detectionCubeMeshRenderer.material = noTargetMat;
                damageableObj = null;
            }
        }

        private void OnMouseDown()
        {
            if (damageableObj)
            {
                //Code for dmg here eventually
            }
        }
    }
}
