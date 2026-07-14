using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace ForeverFight.GameMechanics.Movement
{
    /// <summary>
    /// Renders the planned move: an animated (scrolling-texture) line along the planned
    /// waypoints, an endpoint marker and a world-space cost label ("7.3m - 2 AP").
    /// The marker tints amber when the plan is capped at the AP distance limit.
    /// All visuals are built in code by Initialize() - nothing is wired in the editor.
    /// </summary>
    public class MovementGuideLine : MonoBehaviour
    {
        [SerializeField] private float lineWidth = 0.2f;
        [SerializeField] private float lineHeight = 0.05f;
        [SerializeField] private float scrollSpeed = 1.5f;
        [SerializeField] private float densifyStep = 0.5f;
        [SerializeField] private Color lineColor = new Color(0.3f, 0.85f, 1f, 0.9f);
        [SerializeField] private Color cappedColor = new Color(1f, 0.7f, 0.15f, 0.95f);

        private LineRenderer lineRenderer = null;
        private Material lineMaterialInstance = null;
        private GameObject endpointMarker = null;
        private Renderer endpointRenderer = null;
        private TextMeshPro costLabel = null;
        private readonly List<Vector3> densifiedPoints = new List<Vector3>();
        private bool visible = false;

        public static MovementGuideLine Instance { get; private set; }

        public Transform EndpointTransform => endpointMarker != null ? endpointMarker.transform : null;


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
            if (visible && lineMaterialInstance != null)
            {
                lineMaterialInstance.mainTextureOffset -= new Vector2(scrollSpeed * Time.deltaTime, 0f);
            }
        }

        private void LateUpdate()
        {
            if (visible && costLabel != null && Camera.main != null)
            {
                costLabel.transform.forward = Camera.main.transform.forward;
            }
        }


        private void HandlePlanUpdated(float pathLength, int apCost)
        {
            var planner = MovementPlanner.Instance;
            if (planner == null || planner.PlannedWaypoints.Count < 2)
            {
                Hide();
                return;
            }

            visible = true;
            lineRenderer.enabled = true;
            endpointMarker.SetActive(true);

            Densify(planner.PlannedWaypoints);
            lineRenderer.positionCount = densifiedPoints.Count;
            for (int i = 0; i < densifiedPoints.Count; i++)
            {
                lineRenderer.SetPosition(i, densifiedPoints[i] + Vector3.up * lineHeight);
            }

            Vector3 endpoint = planner.PlannedWaypoints[planner.PlannedWaypoints.Count - 1];
            endpointMarker.transform.position = endpoint + Vector3.up * lineHeight;

            bool capped = pathLength >= ApDistanceBank.Instance.MaxPlannableDistance() - 0.01f;
            Color color = capped ? cappedColor : lineColor;
            endpointRenderer.material.color = color;
            lineMaterialInstance.color = color;

            costLabel.text = $"{pathLength:0.0}m  -  {apCost} AP";
        }

        private void HandleMoveConfirmed(float pathDistance)
        {
            Hide();
        }

        private void Hide()
        {
            visible = false;
            if (lineRenderer != null)
            {
                lineRenderer.enabled = false;
            }
            if (endpointMarker != null)
            {
                endpointMarker.SetActive(false);
            }
        }

        private void Densify(IReadOnlyList<Vector3> waypoints)
        {
            densifiedPoints.Clear();
            for (int i = 1; i < waypoints.Count; i++)
            {
                Vector3 from = waypoints[i - 1];
                Vector3 to = waypoints[i];
                float length = Vector3.Distance(from, to);
                int steps = Mathf.Max(1, Mathf.CeilToInt(length / densifyStep));

                for (int s = 0; s < steps; s++)
                {
                    densifiedPoints.Add(Vector3.Lerp(from, to, (float)s / steps));
                }
            }

            if (waypoints.Count > 0)
            {
                densifiedPoints.Add(waypoints[waypoints.Count - 1]);
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

            lineMaterialInstance = CreateLineMaterial();
            lineRenderer.material = lineMaterialInstance;

            endpointMarker = GameObject.CreatePrimitive(PrimitiveType.Quad);
            endpointMarker.name = "Endpoint Marker";
            endpointMarker.transform.SetParent(transform, false);
            endpointMarker.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
            endpointMarker.transform.rotation = Quaternion.Euler(90f, 0f, 0f); // Flat on the floor.
            Destroy(endpointMarker.GetComponent<Collider>());
            endpointRenderer = endpointMarker.GetComponent<Renderer>();
            endpointRenderer.material = CreateLineMaterial();
            endpointRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            var labelObject = new GameObject("Cost Label");
            labelObject.transform.SetParent(endpointMarker.transform, false);
            labelObject.transform.localPosition = new Vector3(0f, 0f, -1.2f); // Above the marker (quad is rotated).
            labelObject.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
            costLabel = labelObject.AddComponent<TextMeshPro>();
            costLabel.fontSize = 3f;
            costLabel.alignment = TextAlignmentOptions.Center;
            costLabel.color = Color.white;
        }

        private Material CreateLineMaterial()
        {
            // Sprites/Default renders LineRenderers with texture + vertex color in built-in
            // and URP alike; no asset dependency needed.
            var shader = Shader.Find("Sprites/Default");
            var material = new Material(shader)
            {
                mainTexture = CreateChevronTexture(),
                color = lineColor
            };
            material.mainTexture.wrapMode = TextureWrapMode.Repeat;
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
