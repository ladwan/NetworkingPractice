using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ForeverFight.Interactable.PlayerInputInteractions;

namespace ForeverFight.GameMechanics.Movement
{
    /// <summary>
    /// Renders the planned move as an airborne trail that animates out from the character
    /// toward the drag point: the line's head travels along the planned waypoints at
    /// travelSpeed, carrying the endpoint marker and the world-space cost label
    /// ("7.3m - 2 AP") with it. The scrolling chevron texture keeps the revealed portion
    /// animated, and the marker tints amber when the plan is capped at the AP limit.
    /// All visuals are built in code by Awake() - nothing is wired in the editor.
    /// </summary>
    public class MovementGuideLine : MonoBehaviour
    {
        [Header("Trail Shape")]
        [SerializeField] private float lineWidth = 0.2f;
        [Tooltip("Height above the ground the trail floats at.")]
        [SerializeField] private float airHeight = 1.2f;
        [SerializeField] private float densifyStep = 0.5f;

        [Header("Animation")]
        [Tooltip("How fast the trail head lerps from the character to the drag point, in units/sec.")]
        [SerializeField] private float travelSpeed = 10f;
        [Tooltip("Scroll speed of the chevron texture along the revealed trail.")]
        [SerializeField] private float scrollSpeed = 1.5f;
        [Tooltip("Marker scale pulse amount (0 = no pulse).")]
        [SerializeField] private float markerPulseAmount = 0.12f;
        [SerializeField] private float markerPulseSpeed = 4f;

        [Header("Colors")]
        [SerializeField] private Color lineColor = new Color(0.3f, 0.85f, 1f, 0.9f);
        [SerializeField] private Color cappedColor = new Color(1f, 0.7f, 0.15f, 0.95f);

        [Header("Endpoint Re-Grab")]
        [Tooltip("Radius of the click target on a released plan's endpoint.")]
        [SerializeField] private float endpointGrabRadius = 0.8f;

        private LineRenderer lineRenderer = null;
        private Material lineMaterialInstance = null;
        private GameObject endpointMarker = null;
        private Renderer endpointRenderer = null;
        private TextMeshPro costLabel = null;
        private Transform plannedEndpointAnchor = null;
        private GameObject endpointGrabHandle = null;
        private Vector3 markerBaseScale = Vector3.one;
        private string texturePropertyName = "_MainTex";
        private string colorPropertyName = "_Color";
        private float scrollOffset = 0f;
        private readonly List<Vector3> densifiedPoints = new List<Vector3>();
        private readonly List<float> cumulativeDistances = new List<float>();
        private readonly List<Vector3> revealedPoints = new List<Vector3>();
        private float revealDistance = 0f;
        private float totalTrailLength = 0f;
        private bool visible = false;

        public static MovementGuideLine Instance { get; private set; }

        /// <summary>The final planned destination (not the animated head) - used by the camera lerp.</summary>
        public Transform EndpointTransform => plannedEndpointAnchor;


        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.Log("More than 1 MovementGuideLine detected, destroying self...");
                Destroy(this);
                return;
            }

            BuildVisuals();
            Hide();
        }

        private void Start()
        {
            if (MovementPlanner.Instance != null)
            {
                MovementPlanner.Instance.OnPlanUpdated += HandlePlanUpdated;
                MovementPlanner.Instance.OnPlanCleared += Hide;
                MovementPlanner.Instance.OnMoveConfirmed += HandleMoveConfirmed;
            }
        }

        private void OnDestroy()
        {
            if (MovementPlanner.Instance != null)
            {
                MovementPlanner.Instance.OnPlanUpdated -= HandlePlanUpdated;
                MovementPlanner.Instance.OnPlanCleared -= Hide;
                MovementPlanner.Instance.OnMoveConfirmed -= HandleMoveConfirmed;
            }

            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void Update()
        {
            if (!visible)
            {
                return;
            }

            revealDistance = Mathf.MoveTowards(revealDistance, totalTrailLength, travelSpeed * Time.deltaTime);
            RebuildRevealedTrail();

            if (lineMaterialInstance != null)
            {
                scrollOffset = Mathf.Repeat(scrollOffset - scrollSpeed * Time.deltaTime, 1f);
                lineMaterialInstance.SetTextureOffset(texturePropertyName, new Vector2(scrollOffset, 0f));
            }

            if (markerPulseAmount > 0f)
            {
                float pulse = 1f + Mathf.Sin(Time.time * markerPulseSpeed) * markerPulseAmount;
                endpointMarker.transform.localScale = markerBaseScale * pulse;
            }
        }

        private void LateUpdate()
        {
            if (!visible)
            {
                return;
            }

            Camera cam = null;
            if (BasePlayerInputInteraction.Instance != null)
            {
                cam = BasePlayerInputInteraction.Instance.PlayerInputCamera;
            }
            if (cam == null)
            {
                cam = Camera.main;
            }
            if (cam == null)
            {
                return;
            }

            // Bird's-eye view: lay the marker (and its child cost label) flat facing
            // straight up, yawed so the text reads upright on the player's screen.
            Vector3 screenUp = Vector3.ProjectOnPlane(cam.transform.up, Vector3.up);
            if (screenUp.sqrMagnitude < 0.001f)
            {
                screenUp = Vector3.ProjectOnPlane(-cam.transform.forward, Vector3.up);
            }

            endpointMarker.transform.rotation = Quaternion.LookRotation(Vector3.down, screenUp.normalized);
        }


        private void HandlePlanUpdated(float pathLength, int apCost)
        {
            var planner = MovementPlanner.Instance;
            if (planner == null || planner.PlannedWaypoints.Count < 2)
            {
                Hide();
                return;
            }

            bool startingFresh = !visible;
            visible = true;
            lineRenderer.enabled = true;
            endpointMarker.SetActive(true);

            Densify(planner.PlannedWaypoints);

            if (startingFresh)
            {
                revealDistance = 0f; // Animate out from the character on a brand-new plan.
            }
            revealDistance = Mathf.Min(revealDistance, totalTrailLength);

            bool capped = pathLength >= ApDistanceBank.Instance.MaxPlannableDistance() - 0.01f;
            Color color = capped ? cappedColor : lineColor;
            endpointRenderer.material.SetColor(colorPropertyName, color);
            lineMaterialInstance.SetColor(colorPropertyName, color);

            costLabel.text = $"{pathLength:0.0}m  -  {apCost} AP";

            var waypoints = planner.PlannedWaypoints;
            plannedEndpointAnchor.position = waypoints[waypoints.Count - 1];
            endpointGrabHandle.SetActive(true);

            RebuildRevealedTrail();
        }

        private void HandleMoveConfirmed(float pathDistance)
        {
            Hide();
        }

        private void Hide()
        {
            visible = false;
            revealDistance = 0f;
            if (lineRenderer != null)
            {
                lineRenderer.enabled = false;
            }
            if (endpointMarker != null)
            {
                endpointMarker.SetActive(false);
            }
            if (endpointGrabHandle != null)
            {
                endpointGrabHandle.SetActive(false);
            }
        }

        /// <summary>
        /// Writes the portion of the trail between the character and the traveling head
        /// (revealDistance along the path) into the LineRenderer, and parks the marker
        /// at the head so the whole effect lerps toward the drag point.
        /// </summary>
        private void RebuildRevealedTrail()
        {
            if (densifiedPoints.Count < 2)
            {
                return;
            }

            revealedPoints.Clear();
            revealedPoints.Add(densifiedPoints[0]);

            Vector3 head = densifiedPoints[densifiedPoints.Count - 1];
            for (int i = 1; i < densifiedPoints.Count; i++)
            {
                if (cumulativeDistances[i] <= revealDistance)
                {
                    revealedPoints.Add(densifiedPoints[i]);
                    continue;
                }

                // Head lands partway along this segment - interpolate the exact point.
                float segmentLength = cumulativeDistances[i] - cumulativeDistances[i - 1];
                float t = segmentLength > 0.0001f
                    ? (revealDistance - cumulativeDistances[i - 1]) / segmentLength
                    : 1f;
                head = Vector3.Lerp(densifiedPoints[i - 1], densifiedPoints[i], t);
                revealedPoints.Add(head);
                break;
            }

            lineRenderer.positionCount = revealedPoints.Count;
            for (int i = 0; i < revealedPoints.Count; i++)
            {
                lineRenderer.SetPosition(i, revealedPoints[i]);
            }

            endpointMarker.transform.position = head;
        }

        /// <summary>Subdivides the waypoints, lifts them to airHeight and caches cumulative distances.</summary>
        private void Densify(IReadOnlyList<Vector3> waypoints)
        {
            densifiedPoints.Clear();
            cumulativeDistances.Clear();

            Vector3 lift = Vector3.up * airHeight;
            for (int i = 1; i < waypoints.Count; i++)
            {
                Vector3 from = waypoints[i - 1];
                Vector3 to = waypoints[i];
                float length = Vector3.Distance(from, to);
                int steps = Mathf.Max(1, Mathf.CeilToInt(length / densifyStep));

                for (int s = 0; s < steps; s++)
                {
                    densifiedPoints.Add(Vector3.Lerp(from, to, (float)s / steps) + lift);
                }
            }

            if (waypoints.Count > 0)
            {
                densifiedPoints.Add(waypoints[waypoints.Count - 1] + lift);
            }

            totalTrailLength = 0f;
            cumulativeDistances.Add(0f);
            for (int i = 1; i < densifiedPoints.Count; i++)
            {
                totalTrailLength += Vector3.Distance(densifiedPoints[i - 1], densifiedPoints[i]);
                cumulativeDistances.Add(totalTrailLength);
            }
        }

        private void BuildVisuals()
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.useWorldSpace = true;
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;
            lineRenderer.numCornerVertices = 4;
            lineRenderer.numCapVertices = 4;
            lineRenderer.textureMode = LineTextureMode.Tile;
            lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            lineRenderer.receiveShadows = false;
            lineRenderer.alignment = LineAlignment.View; // Ribbon faces the camera since it floats in the air.

            lineMaterialInstance = CreateLineMaterial();
            lineRenderer.material = lineMaterialInstance;

            endpointMarker = GameObject.CreatePrimitive(PrimitiveType.Quad);
            endpointMarker.name = "Endpoint Marker";
            endpointMarker.transform.SetParent(transform, false);
            endpointMarker.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
            markerBaseScale = endpointMarker.transform.localScale;
            Destroy(endpointMarker.GetComponent<Collider>());
            endpointRenderer = endpointMarker.GetComponent<Renderer>();
            endpointRenderer.material = CreateLineMaterial();
            endpointRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            plannedEndpointAnchor = new GameObject("Planned Endpoint").transform;
            plannedEndpointAnchor.SetParent(transform, false);

            // Click target so a released plan's endpoint can be grabbed to resume the drag.
            endpointGrabHandle = new GameObject("Endpoint Grab Handle");
            endpointGrabHandle.transform.SetParent(plannedEndpointAnchor, false);
            int dragLayer = LayerMask.NameToLayer("Drag Movement");
            if (dragLayer >= 0)
            {
                endpointGrabHandle.layer = dragLayer;
            }
            var grabCollider = endpointGrabHandle.AddComponent<SphereCollider>();
            grabCollider.isTrigger = true;
            grabCollider.radius = endpointGrabRadius;
            endpointGrabHandle.AddComponent<MoveEndpointInteractable>();
            endpointGrabHandle.SetActive(false);

            var labelObject = new GameObject("Cost Label");
            labelObject.transform.SetParent(endpointMarker.transform, false);
            // Local +Y = screen-up (past the marker), local -Z = world-up (slight lift so the
            // flat label never z-fights the marker or the trail ribbon).
            labelObject.transform.localPosition = new Vector3(0f, 0.9f, -0.05f);
            costLabel = labelObject.AddComponent<TextMeshPro>();
            costLabel.fontSize = 3f;
            costLabel.alignment = TextAlignmentOptions.Center;
            costLabel.color = Color.white;
        }

        private Material CreateLineMaterial()
        {
            // URP's Unlit shader respects texture tiling/offset, which the scrolling
            // chevron animation depends on. Sprites/Default ignores _MainTex_ST, so a
            // scrolled offset renders static there - only used as a last-resort fallback.
            Material material;
            var urpShader = Shader.Find("Universal Render Pipeline/Unlit");
            if (urpShader != null)
            {
                material = new Material(urpShader);
                texturePropertyName = "_BaseMap";
                colorPropertyName = "_BaseColor";
                material.SetFloat("_Surface", 1f); // Transparent
                material.SetFloat("_Blend", 0f);   // Alpha blend
                material.SetOverrideTag("RenderType", "Transparent");
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
                material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            }
            else
            {
                material = new Material(Shader.Find("Sprites/Default"));
            }

            var texture = CreateChevronTexture();
            texture.wrapMode = TextureWrapMode.Repeat;
            material.SetTexture(texturePropertyName, texture);
            material.SetColor(colorPropertyName, lineColor);
            return material;
        }

        /// <summary>Generates a small dash/chevron strip so the scroll reads as flow.</summary>
        private static Texture2D CreateChevronTexture()
        {
            const int width = 16;
            const int height = 8;
            var texture = new Texture2D(width, height, TextureFormat.RGBA32, false);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    // Diagonal band pointing along +X: opaque where the wrapped diagonal falls.
                    int band = (x + Mathf.Abs(y - height / 2)) % width;
                    bool solid = band < width / 2;
                    texture.SetPixel(x, y, solid ? Color.white : new Color(1f, 1f, 1f, 0.15f));
                }
            }

            texture.Apply();
            texture.filterMode = FilterMode.Bilinear;
            return texture;
        }
    }
}
