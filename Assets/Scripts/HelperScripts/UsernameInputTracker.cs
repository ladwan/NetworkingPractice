using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ForverFight.HelperScripts
{
    public class UsernameInputTracker : MonoBehaviour
    {
        [SerializeField]
        private Button confirmButton = null;
        [SerializeField]
        private InputField usernameInputField = null;

        [NonSerialized]
        private int currentCharacterCount = 0;

        public void UpdateUsername()
        {
            var updateCharacterCount = usernameInputField.text.Length > currentCharacterCount ? currentCharacterCount++ : currentCharacterCount--;


            if (!IsAllSpaces(usernameInputField.text))
            {
                var confirmButtonToggle = currentCharacterCount >= 3 ? confirmButton.interactable = true : confirmButton.interactable = false;
            }
        }

        private bool IsAllSpaces(string value)
        {
            if (value == "   ")
            {
                return true;
            }
            if (value == "    ")
            {
                return true;
            }
            if (value == "     ")
            {
                return true;
            }
            if (value == "      ")
            {
                return true;
            }
            if (value == "       ")
            {
                return true;
            }
            if (value == "        ")
            {
                return true;
            }
            if (value == "         ")
            {
                return true;
            }
            if (value == "          ")
            {
                return true;
            }

            if (value.StartsWith(" "))
            {
                return true;
            }

            return false;

        }
    }
}
