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
        public static BasePlayerInputInteraction Instance { get; private set; }

        // Events
        public event System.Action<Vector2> OnTouchEnd;


        // Touch state
        private bool isTouching = false;
        private int activeTouchId = -1;

        private LayerMask dragCollisionLayer;
        private LayerMask groundCollisionLayer;
        private Camera playerInputCameraREF = null;


        private void Awake()
        {
            Instance = this;
            dragCollisionLayer = LayerMask.GetMask("Drag Movement");
            groundCollisionLayer = LayerMask.GetMask("Ground");
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
                        if (isTouching && MovementPlanner.Instance.IsDragging)
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
                else if (Input.GetMouseButton(0) && isTouching && MovementPlanner.Instance.IsDragging)
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

        // While a movement drag is held, samples the ground under the cursor and feeds
        // the planner (replaces the old per-grid-cell raycast).
        private void HandleTouchHold(Vector2 screenPos)
        {
            Ray ray = playerInputCameraREF.ScreenPointToRay(screenPos);

            if (NavPathUtility.SampleGround(ray, groundCollisionLayer, out Vector3 groundPoint))
            {
                MovementPlanner.Instance.UpdateDrag(groundPoint);
            }
        }

        private void HandleTouchEnd(Vector2 screenPos)
        {
            OnTouchEnd?.Invoke(screenPos);
            isTouching = false;
            activeTouchId = -1;
        }

        // Turn expiry mid-drag: clear touch state without confirming anything.
        public void ForceEndDrag()
        {
            isTouching = false;
            activeTouchId = -1;
        }

        private void SetCharacterCameraReference(CharacterAnimationReferences animationReferences)
        {
            playerInputCameraREF = animationReferences.CharacterCamera;
        }
    }
}
