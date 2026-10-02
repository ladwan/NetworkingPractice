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
        [SerializeField] private float lineWidth = 0.22f;
        [Tooltip("Height above the ground the trail floats at.")]
        [SerializeField] private float airHeight = 1.2f;
        [SerializeField] private float densifyStep = 0.5f;
        // The chevron texture tiles once per world unit and its point shifts a quarter tile over
        // half the width, so a 0.25 unit point gives the head the same angle as the arrows.
        [Tooltip("Length of the pointed head of the trail, in units.")]
        [SerializeField] private float tipLength = 0.25f;
        [Tooltip("Fraction of the trail's width, centered, that stays fully solid before the sides fade out.")]
        [Range(0f, 1f)]
        [SerializeField] private float solidCenterWidth = 1f / 9f;

        [Header("Animation")]
        [Tooltip("How fast the trail head lerps from the character to the drag point, in units/sec.")]
        [SerializeField] private float travelSpeed = 10f;
        [Tooltip("Scroll speed of the chevron texture along the revealed trail.")]
        [SerializeField] private float scrollSpeed = 1.5f;
        [Tooltip("Endpoint marker: how many waves leave the center each second.")]
        [SerializeField] private float markerWaveSpeed = 1.2f;
        [Tooltip("Endpoint marker: thickness of the wave ring, as a fraction of the marker's radius.")]
        [Range(0.01f, 0.5f)]
        [SerializeField] private float markerRingWidth = 0.08f;
        [Tooltip("Endpoint marker: size in world units. The wave reaches the marker's edge, so this sets its diameter.")]
        [SerializeField] private float markerSize = 0.726f;

        // Where the cost label sat when the marker was 0.6 wide, kept fixed as markerSize changes.
        private const float LabelWorldOffset = 0.54f;
        private const float LabelWorldScale = 0.6f;

        [Header("Colors")]
        [Tooltip("The part of the trail paid for with passive AP (spent first).")]
        [SerializeField] private Color passiveColor = new Color(0.25f, 0.55f, 1f, 0.9f);
        [Tooltip("The part of the trail paid for with normal AP.")]
        [SerializeField] private Color mainColor = new Color(0.3f, 1f, 0.45f, 0.9f);
        [SerializeField] private Color cappedColor = new Color(1f, 0.7f, 0.15f, 0.95f);
        [Tooltip("How many units the passive color takes to blend into the normal color.")]
        [SerializeField] private float colorBlendDistance = 0.8f;
        [Tooltip("Outline around the cost label so it stays readable over white things.")]
        [SerializeField] private Color labelOutlineColor = new Color(0f, 0f, 0f, 1f);
        [Range(0f, 1f)]
        [SerializeField] private float labelOutlineWidth = 0.25f;

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
        private string texturePropertyName = "_MainTex";
        private string colorPropertyName = "_Color";
        private string trailTexturePropertyName = "_BaseMap";
        private string endpointColorPropertyName = "_BaseColor";
        private float scrollOffset = 0f;
        private readonly List<Vector3> densifiedPoints = new List<Vector3>();
        private readonly List<float> cumulativeDistances = new List<float>();
        private readonly List<Vector3> revealedPoints = new List<Vector3>();
        private float revealDistance = 0f;
        private float totalTrailLength = 0f;
        private float passiveDistance = 0f;
        private Color endColor = Color.white;
        private readonly Gradient trailGradient = new Gradient();
        // Reused every rebuild so drawing the trail allocates nothing per frame (no GC spikes on mobile).
        private readonly float[] gradientTimes = new float[4];
        private readonly GradientColorKey[] gradientColorKeys = new GradientColorKey[4];
        private readonly GradientAlphaKey[] gradientAlphaKeys = new GradientAlphaKey[4];
        private Vector3[] positionBuffer = new Vector3[64];
        private Material endpointMaterial = null;
        private int shownLabelTenths = -1;
        private int shownLabelCost = -1;
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

            // Only rebuild while the head is still animating out. Plan changes rebuild in
            // HandlePlanUpdated, so a finished, unchanged trail costs nothing but the scroll.
            float previousRevealDistance = revealDistance;
            revealDistance = Mathf.MoveTowards(revealDistance, totalTrailLength, travelSpeed * Time.deltaTime);
            if (revealDistance != previousRevealDistance)
            {
                RebuildRevealedTrail();
            }

            if (lineMaterialInstance != null)
            {
                scrollOffset = Mathf.Repeat(scrollOffset - scrollSpeed * Time.deltaTime, 1f);
                lineMaterialInstance.SetTextureOffset(trailTexturePropertyName, new Vector2(scrollOffset, 0f));
#if UNITY_EDITOR
                lineMaterialInstance.SetFloat("_SolidCenterWidth", solidCenterWidth); // Live tuning from the Inspector, editor only.
                lineMaterialInstance.SetFloat("_TipLength", tipLength);
                endpointMaterial.SetFloat("_WaveSpeed", markerWaveSpeed);
                endpointMaterial.SetFloat("_RingWidth", markerRingWidth);
#endif
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

            // Capped swaps the normal AP color for the warning color; the passive part stays blue.
            bool capped = pathLength >= ApDistanceBank.Instance.MaxPlannableDistance() - 0.01f;
            endColor = capped ? cappedColor : mainColor;
            passiveDistance = Mathf.Min(ApDistanceBank.Instance.PendingPassiveDistance, totalTrailLength);

            // Only rebuild the label string (and TMP mesh) when what it shows actually changes.
            int labelTenths = Mathf.RoundToInt(pathLength * 10f);
            if (labelTenths != shownLabelTenths || apCost != shownLabelCost)
            {
                shownLabelTenths = labelTenths;
                shownLabelCost = apCost;
                costLabel.text = $"{pathLength:0.0}m  -  {apCost} AP";
            }

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

            // One SetPositions call instead of one SetPosition per point.
            if (positionBuffer.Length < revealedPoints.Count)
            {
                positionBuffer = new Vector3[Mathf.NextPowerOfTwo(revealedPoints.Count)];
            }
            revealedPoints.CopyTo(positionBuffer);
            lineRenderer.positionCount = revealedPoints.Count;
            lineRenderer.SetPositions(positionBuffer);

            float revealedLength = Mathf.Min(revealDistance, totalTrailLength);
            ApplyTrailGradient(revealedLength);
            lineMaterialInstance.SetFloat("_TrailLength", revealedLength); // The shader cuts the pointed tip from this.
            endpointMaterial.SetColor(endpointColorPropertyName, ColorAtDistance(revealedLength));

            endpointMarker.transform.position = head;
        }

        /// <summary>
        /// Blue for the stretch paid by passive AP, blending into the normal AP color after it.
        /// The gradient spans only the revealed part of the trail, so its keys are rescaled
        /// to that length as the head animates out.
        /// </summary>
        private void ApplyTrailGradient(float revealedLength)
        {
            if (revealedLength <= 0.0001f)
            {
                return;
            }

            gradientTimes[0] = 0f;
            gradientTimes[1] = Mathf.Clamp01(passiveDistance / revealedLength);
            gradientTimes[2] = Mathf.Clamp01((passiveDistance + colorBlendDistance) / revealedLength);
            gradientTimes[3] = 1f;

            for (int i = 0; i < gradientTimes.Length; i++)
            {
                Color color = ColorAtDistance(gradientTimes[i] * revealedLength);
                gradientColorKeys[i] = new GradientColorKey(color, gradientTimes[i]);
                gradientAlphaKeys[i] = new GradientAlphaKey(color.a, gradientTimes[i]);
            }

            trailGradient.SetKeys(gradientColorKeys, gradientAlphaKeys);
            lineRenderer.colorGradient = trailGradient;
        }
        private Color ColorAtDistance(float distance)
        {
            if (passiveDistance <= 0f)
            {
                return endColor;
            }

            float t = Mathf.InverseLerp(passiveDistance, passiveDistance + colorBlendDistance, distance);
            return Color.Lerp(passiveColor, endColor, t);
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
            lineRenderer.widthMultiplier = lineWidth;
            lineRenderer.numCornerVertices = 4;
            lineRenderer.numCapVertices = 0; // No rounded caps: the trail shader cuts the head into a point instead.
            lineRenderer.textureMode = LineTextureMode.Tile;
            lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            lineRenderer.receiveShadows = false;
            lineRenderer.alignment = LineAlignment.View; // Ribbon faces the camera since it floats in the air.

            lineMaterialInstance = CreateTrailMaterial();
            lineRenderer.material = lineMaterialInstance;

            endpointMarker = GameObject.CreatePrimitive(PrimitiveType.Quad);
            endpointMarker.name = "Endpoint Marker";
            endpointMarker.transform.SetParent(transform, false);
            // The wave ring's width is a fraction of the marker's radius, so scaling the marker grows
            // the ring's diameter and thickness together.
            endpointMarker.transform.localScale = Vector3.one * markerSize;
            Destroy(endpointMarker.GetComponent<Collider>());
            endpointRenderer = endpointMarker.GetComponent<Renderer>();
            endpointMaterial = CreateEndpointWaveMaterial();
            endpointRenderer.sharedMaterial = endpointMaterial;
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
            // The label is a child of the marker, so undo the marker's size to keep the label the same
            // size and at the same distance (0.54 world units) whatever markerSize is.
            labelObject.transform.localPosition = new Vector3(0f, LabelWorldOffset / markerSize, -0.05f);
            labelObject.transform.localScale = Vector3.one * (LabelWorldScale / markerSize);
            costLabel = labelObject.AddComponent<TextMeshPro>();
            costLabel.fontSize = 6f;
            costLabel.alignment = TextAlignmentOptions.Center;
            costLabel.color = Color.white;

            // The overlay shader skips the depth test so the label draws over the character and
            // the arena, and the dark outline keeps the white text readable over white things.
            // The shader is in Graphics Settings' Always Included Shaders so Shader.Find works in builds.
            var overlayShader = Shader.Find("TextMeshPro/Distance Field Overlay");
            if (overlayShader != null)
            {
                costLabel.fontMaterial.shader = overlayShader;
            }
            else
            {
                Debug.LogError("TMP Distance Field Overlay shader missing, the cost label can be hidden behind objects");
            }
            costLabel.outlineWidth = labelOutlineWidth;
            costLabel.outlineColor = labelOutlineColor;
        }

        // The trail uses its own shader (Resources/GuideLineTrail.shader): URP's Unlit ignores
        // the LineRenderer's vertex colors (the passive/normal AP gradient), and its
        // Particles/Unlit ignores texture offset (the chevron scroll). The custom shader does
        // both, and fades the trail's sides. Falls back to URP Unlit if it's missing.
        private Material CreateTrailMaterial()
        {
            var trailShader = Resources.Load<Shader>("GuideLineTrail");
            if (trailShader == null)
            {
                Debug.LogError("GuideLineTrail shader missing from Resources, the trail falls back to URP Unlit");
                var fallback = CreateLineMaterial();
                trailTexturePropertyName = texturePropertyName;
                return fallback;
            }

            var material = new Material(trailShader);
            var texture = CreateChevronTexture();
            texture.wrapMode = TextureWrapMode.Repeat;
            material.SetTexture("_BaseMap", texture);
            material.SetColor("_BaseColor", Color.white);
            material.SetFloat("_SolidCenterWidth", solidCenterWidth);
            material.SetFloat("_TipLength", tipLength);
            return material;
        }

        // The endpoint marker's expanding wave ring (Resources/GuideLineEndpointWave.shader).
        // Falls back to the plain line material if the shader is missing.
        private Material CreateEndpointWaveMaterial()
        {
            var waveShader = Resources.Load<Shader>("GuideLineEndpointWave");
            if (waveShader == null)
            {
                Debug.LogError("GuideLineEndpointWave shader missing from Resources, the endpoint marker falls back to URP Unlit");
                var fallback = CreateLineMaterial();
                endpointColorPropertyName = colorPropertyName;
                return fallback;
            }

            var material = new Material(waveShader);
            material.SetColor("_BaseColor", mainColor);
            material.SetFloat("_WaveSpeed", markerWaveSpeed);
            material.SetFloat("_RingWidth", markerRingWidth);
            return material;
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
            material.SetColor(colorPropertyName, Color.white);
            return material;
        }

        /// <summary>Generates a small dash/chevron strip so the scroll reads as flow.</summary>
        // Worked out from each pixel's center in 0..1 space so the arrow's point lands exactly on
        // the middle of the width. The old integer version measured from row height/2, which
        // is half a pixel off center on an even row count and pushed the arrows to one side.
        private static Texture2D CreateChevronTexture()
        {
            const int width = 64;
            const int height = 32;
            // How far (in tiles) the arrow's edges trail its point. 0.25 is the original shape and
            // what the trail shader's 0.25 unit tip is matched to.
            const float pointDepth = 0.25f;
            var texture = new Texture2D(width, height, TextureFormat.RGBA32, false);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float u = (x + 0.5f) / width;
                    float v = (y + 0.5f) / height;

                    // Diagonal band pointing along +X: opaque where the wrapped diagonal falls.
                    float band = Mathf.Repeat(u + Mathf.Abs(v - 0.5f) * 2f * pointDepth, 1f);
                    bool solid = band < 0.5f;
                    texture.SetPixel(x, y, solid ? Color.white : new Color(1f, 1f, 1f, 0.15f));
                }
            }

            texture.Apply();
            texture.filterMode = FilterMode.Bilinear;
            return texture;
        }
    }
}
