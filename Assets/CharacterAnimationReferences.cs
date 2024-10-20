using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ForeverFight.Interactable.Characters
{
    public class CharacterAnimationReferences : MonoBehaviour
    {
        [SerializeField] private Animator characterAnimator = null;
        [SerializeField] private GameObject characterCameraParent = null;
        [SerializeField] private Camera characterCamera = null;
        [SerializeField] private AudioListener characterCameraAudioListener = null;


        public Animator CharacterAnimator { get => characterAnimator; }

        public GameObject CharacterCameraParent { get => characterCameraParent; }

        public Camera CharacterCamera { get => characterCamera; }

        public AudioListener CharacterCameraAudioListener { get => characterCameraAudioListener; set => characterCameraAudioListener = value; }
    }
}
