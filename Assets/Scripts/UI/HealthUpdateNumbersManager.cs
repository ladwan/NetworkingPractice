using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace ForeverFight.Ui
{
    public class HealthUpdateNumbersManager : MonoBehaviour
    {
        [Header("Local Player")]
        [SerializeField]
        private TMP_Text localPlayerHealthUpdateNumber = null;

        [Space]

        [Header("Opponent")]
        [SerializeField]
        private TMP_Text opponentHealthUpdateNumber = null;

        [Space]

        [Header("Shared")]
        [SerializeField]
        private Animator animator = null;
        [SerializeField]
        private Color healthIncreasedColor = new Color();
        [SerializeField]
        private Color healthDecreasedColor = new Color();


        public Animator Animator { get => animator; set => animator = value; }

        public TMP_Text LocalPlayerHealthUpdateNumber { get => localPlayerHealthUpdateNumber; set => localPlayerHealthUpdateNumber = value; }

        public TMP_Text OpponentHealthUpdateNumber { get => opponentHealthUpdateNumber; set => opponentHealthUpdateNumber = value; }


        // make Action of type int and Charather
        public void IncreasedHealth(int damageIdentifierIndex)
        {
            localPlayerHealthUpdateNumber.color = Color.red;
            //localPlayerHealthUpdateNumber.text = "-" + healthChangeValue;
        }

        public void HealthDecreased(TMP_Text healthText, int dmg)
        {
            healthText.color = healthDecreasedColor;
            healthText.text = $"-{dmg}";
            StartCoroutine(AnimationDelay(healthText));
        }

        private IEnumerator AnimationDelay(TMP_Text healthText)
        {
            yield return new WaitForSecondsRealtime(0.1f);
            yield return new WaitUntil(() => !animator.IsInTransition(0) && animator.GetCurrentAnimatorStateInfo(0).IsName("Empty State"));
            ResetHealthNumbers(healthText);
        }
        private void ResetHealthNumbers(TMP_Text healthText)
        {
            healthText.color = Color.grey;
            healthText.text = "";
        }

        //Create health events based off instances of damage
        //Decipher what type of health event has happend, then update the text color and symbol to match the event
        //Trigger the animation when a health event occurs 
        //Switch on damage identifer index 
        //Continue to add logic for healing health
    }
}
