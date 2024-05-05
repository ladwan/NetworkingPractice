using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ForeverFight.Movement
{
    public class Movement : MonoBehaviour
    {
        public static Movement instance;

        public Transform objToMove;
        public BoxCollider playArea;

        public int movementIndex = 0;


        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Debug.Log("Instance already exsists, destroying object!");
                Destroy(this);
            }
        }
        // Update is called once per frame
        void Update()
        {
            /*
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                movementIndex = 1;
                ClientSend.UpdatePlayerCurrentPostition();
                //objToMove.position += new Vector3(1, 0, 0);
                if (objToMove.position.x > 13)
                {
                    objToMove.position += new Vector3(-1, 0, 0);
                }
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                movementIndex = 2;
                ClientSend.UpdatePlayerCurrentPostition();
                //objToMove.position += new Vector3(-1, 0, 0);
                if (objToMove.position.x < 1)
                {
                    objToMove.position += new Vector3(1, 0, 0);
                }
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                movementIndex = 3;
                ClientSend.UpdatePlayerCurrentPostition();
                //objToMove.position += new Vector3(0, 0, 1);
                if (objToMove.position.z > 13)
                {
                    objToMove.position += new Vector3(0, 0, -1);
                }
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                movementIndex = 4;
                ClientSend.UpdatePlayerCurrentPostition();
                //objToMove.position += new Vector3(0, 0, -1);
                if (objToMove.position.z < 1)
                {
                    objToMove.position += new Vector3(0, 0, 1);
                }
            }*/
        }
        public void ServerMovePlayer(int _movementData)
        {
            switch (_movementData)
            {
                case 1:
                    objToMove.position += new Vector3(1, 0, 0);
                    break;
                case 2:
                    objToMove.position += new Vector3(-1, 0, 0);
                    break;
                case 3:
                    objToMove.position += new Vector3(0, 0, 1);
                    break;
                case 4:
                    objToMove.position += new Vector3(0, 0, -1);
                    break;
            }

        }
    }
}