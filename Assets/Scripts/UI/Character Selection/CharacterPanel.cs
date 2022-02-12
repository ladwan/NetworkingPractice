using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ForverFight.Ui.CharacterSelection
{
    public class CharacterPanel : MonoBehaviour
    {
        [SerializeField]
        private GameObject parent;
        [SerializeField]
        private GameObject highlight;
        [SerializeField]
        private CharacterInfo info;
        [SerializeField]
        private Color player1Color = Color.red;
        [SerializeField]
        private Color player2Color = Color.blue;

        public void OnEnable()
        {
            if (ClientInfo.playerNumber == 1)
            {
                highlight.GetComponent<Image>().color = player1Color;
            }
            else if (ClientInfo.playerNumber == 2)
            {
                highlight.GetComponent<Image>().color = player2Color;
            }
        }

        public GameObject Parent { get => parent; set => parent = value; }

        public GameObject Highlight { get => highlight; set => highlight = value; }

        public CharacterInfo Info { get => info; set => info = value; }

        public Color Player1Color { get => player1Color; set => player1Color = value; }

        public Color Player2Color { get => player2Color; set => player2Color = value; }
    }
}
