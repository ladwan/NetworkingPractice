using ForeverFight.GameMechanics.Movement;
using ForeverFight.HelperScripts;
using ForeverFight.Interactable.Characters;
using ForeverFight.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ForeverFight.Interactable.PlayerInputInteractions
{
    public class BasePlayerInputInteraction : MonoBehaviour
    {
        [SerializeField] private DragMovement dragMovementREF;


        public static BasePlayerInputInteraction Instance { get; private set; }

        // Events
        public event System.Action<Vector2> OnTouchEnd;


        // Touch state
        private bool isTouching = false;
        private int activeTouchId = -1;

        private LayerMask dragCollisionLayer;
        private LayerMask gridCollisionLayer;
        private Camera playerInputCameraREF = null;


        private void Awake()
        {
            Instance = this;
            dragCollisionLayer = LayerMask.GetMask("Drag Movement");
            gridCollisionLayer = LayerMask.GetMask("Grid");
        }

        protected void Start()
        {
            StartCoroutine(LocalStoredNetworkData.WaitForCharacterAnimationReferences(SetCharacterCameraReference));
        }

        private void Update()
        {
            HandleInput();
        }

        private void HandleInput()
        {
            // Touch (mobile)
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        HandleTouchStart(touch.position);
                        activeTouchId = touch.fingerId;
                        break;

                    case TouchPhase.Moved:
                    case TouchPhase.Stationary:
                        if (isTouching && dragMovementREF.ValidDrag)
                        {
                            HandleTouchHold(touch.position);
                        }
                        break;

                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        if (touch.fingerId == activeTouchId)
                        {
                            HandleTouchEnd(touch.position);
                        }
                        break;
                }
            }

            // Mouse (desktop testing)
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    HandleTouchStart(Input.mousePosition);
                }
                else if (Input.GetMouseButton(0) && isTouching && dragMovementREF.ValidDrag)
                {
                    HandleTouchHold(Input.mousePosition);
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    HandleTouchEnd(Input.mousePosition);
                }
            }
        }

        private void HandleTouchStart(Vector2 screenPos)
        {
            isTouching = true;

            Ray ray = playerInputCameraREF.ScreenPointToRay(screenPos);

            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, dragCollisionLayer))
            {
                IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();
                interactable?.Clicked();
            }

        }

        //Right now this is ONLY looking for gridPoints,maybe in the future we will update it to be more generic
        private void HandleTouchHold(Vector2 screenPos)
        {
            Ray ray = playerInputCameraREF.ScreenPointToRay(screenPos);

            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, gridCollisionLayer))
            {
                IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();
                interactable?.Clicked();
            }
        }

        private void HandleTouchEnd(Vector2 screenPos)
        {
            OnTouchEnd?.Invoke(screenPos);
            isTouching = false;
            activeTouchId = -1;
        }

        private void SetCharacterCameraReference(CharacterAnimationReferences animationReferences)
        {
            playerInputCameraREF = animationReferences.CharacterCamera;
        }
    }
}
