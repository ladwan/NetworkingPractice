using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ForeverFight.Interactable.Abilities
{
    public class AfterImage : MonoBehaviour
    {
        [SerializeField] bool RandomColor;
        [SerializeField] float delay;
        [SerializeField] string fadeProperty = "_Fade";
        [SerializeField] GameObject presetObj;
        [SerializeField] int poolSize = 1000;
        [SerializeField] Rigidbody rb;
        [SerializeField] float fadeSpeed = 3f;

        private Renderer[] renderers;
        private float[] fadeTimers;
        private float timer;
        private SkinnedMeshRenderer[] skinRenderers;
        private MeshRenderer[] meshRenderers;
        private Matrix4x4 matrix;
        private CombineInstance[] combine;
        private List<GameObject> objectPool;
        private MeshFilter[] poolMeshFilters;
        private MeshFilter[] meshFilters;
        private int waitTime = 3;

        // Pool for mesh objects to reuse
        private Mesh[] pooledMeshes;

        private bool isScriptActive = false; // Track if the script is active

        void Start()
        {
            InitializePools();
        }

        void InitializePools()
        {
            SetUpRenderers();

            // Initialize object pool
            objectPool = new List<GameObject>();
            poolMeshFilters = new MeshFilter[poolSize];
            renderers = new Renderer[poolSize];
            fadeTimers = new float[poolSize];

            // Initialize Mesh pool
            pooledMeshes = new Mesh[poolSize];
            for (int i = 0; i < pooledMeshes.Length; i++)
            {
                pooledMeshes[i] = new Mesh(); // Create a reusable Mesh
            }

            // Instantiate the pool of objects
            for (int i = 0; i < poolSize; i++)
            {
                GameObject obj = Instantiate(presetObj);
                poolMeshFilters[i] = obj.GetComponent<MeshFilter>();
                renderers[i] = obj.GetComponent<Renderer>();
                obj.SetActive(false);
                objectPool.Add(obj);
                //Debug.Log($"~~ Material Name: {obj.GetComponent<MeshRenderer>().material.name}");
            }
        }

        void SetUpRenderers()
        {
            // Get the skinned mesh renderers
            skinRenderers = transform.GetComponentsInChildren<SkinnedMeshRenderer>();
            // Get normal mesh renderers and their filters
            meshRenderers = transform.GetComponentsInChildren<MeshRenderer>();
            meshFilters = new MeshFilter[meshRenderers.Length];
            for (int i = 0; i < meshRenderers.Length; i++)
            {
                meshFilters[i] = meshRenderers[i].GetComponent<MeshFilter>();
            }
            // Create CombineInstances for every mesh we need to grab
            combine = new CombineInstance[skinRenderers.Length + meshRenderers.Length];
        }

        void Update()
        {
            if (!isScriptActive) return; // Skip processing if the script is disabled

            timer -= Time.deltaTime;

            // Only create image if timer is down
            if (timer < 0)
            {
                timer = delay;
                CreateAfterImage();
            }

            // Fade the property block
            for (int i = 0; i < poolSize; i++)
            {
                fadeTimers[i] -= Time.deltaTime * fadeSpeed;
                renderers[i].material.SetFloat(fadeProperty, fadeTimers[i]);
            }
        }

        // Get a gameobject from the pool, and its index
        public (GameObject, int) GetPooledObject()
        {
            for (int i = 0; i < objectPool.Count; i++)
            {
                if (!objectPool[i].activeInHierarchy)
                {
                    return (objectPool[i], i);
                }
            }

            return (null, -1);
        }

        void CreateAfterImage()
        {
            // Grab a pooled object
            (GameObject, int) obj = GetPooledObject();
            // If no object to assign, return
            if (obj.Item1 == null)
            {
                return;
            }

            // Current transform matrix
            matrix = transform.worldToLocalMatrix;

            // Create mesh snapshot for all skinned meshes
            for (int i = 0; i < skinRenderers.Length; i++)
            {
                Mesh mesh = pooledMeshes[obj.Item2]; // Reuse pooled mesh
                skinRenderers[i].BakeMesh(mesh);
                combine[i].mesh = mesh;
                combine[i].transform = matrix * skinRenderers[i].localToWorldMatrix;
            }

            // Also add normal meshes
            for (int i = 0; i < meshRenderers.Length; i++)
            {
                combine[skinRenderers.Length + i].mesh = meshFilters[i].sharedMesh;
                combine[skinRenderers.Length + i].transform = matrix * meshRenderers[i].transform.localToWorldMatrix;
            }

            // Set property block
            fadeTimers[obj.Item2] = 1f;
            renderers[obj.Item2].material.SetFloat(fadeProperty, fadeTimers[obj.Item2]);
            if (RandomColor)
            {
                renderers[obj.Item2].material.SetColor("_Color", Random.ColorHSV(0, 1, 0.5f, 1, 1, 1));
            }

            // Combine meshes into the right instance
            poolMeshFilters[obj.Item2].mesh.CombineMeshes(combine);

            // Set object to transform and active
            obj.Item1.transform.position = transform.position;
            obj.Item1.transform.rotation = transform.rotation;
            obj.Item1.SetActive(true);

            // Start coroutine to disable object
            StartCoroutine(DisableClone(obj.Item1));
        }

        IEnumerator DisableClone(GameObject obj)
        {
            yield return new WaitForSeconds(waitTime);
            obj.SetActive(false);
        }

        // Called when the script is disabled
        private void OnDisable()
        {
            isScriptActive = false; // Mark the script as disabled

            // Stop all coroutines and clear any other pending operations
            StopAllCoroutines();

            // Deactivate all objects in the pool
            foreach (var obj in objectPool)
            {
                if (obj != null)
                {
                    obj.SetActive(false);
                }
            }
        }

        // Called when the script is enabled again
        private void OnEnable()
        {
            if (!isScriptActive)
            {
                isScriptActive = true; // Mark the script as active again
                timer = delay; // Reset the timer so the images start appearing again
                fadeTimers = new float[poolSize]; // Reset fade timers for all pooled objects
            }
        }
    }
}
