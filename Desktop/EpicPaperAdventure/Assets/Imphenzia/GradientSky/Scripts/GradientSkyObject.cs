// Use this script on an empty gameobject to create a gradient mesh. Scale the object as necessary.
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
// Use a unique namespace to ensure there are no name conflicts
namespace Imphenzia
{
    // Requrie MeshFilter and MeshRenderer components
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]

    // Execute in edit mode so gradient is updated if the color gradient is changed
    [ExecuteInEditMode]

    // This class inherits from GradientSkyCommon.cs where there is common and reusable code
    public class GradientSkyObject : GradientSkyCommon {

        // Keep a reference to the MeshFilter component for increased performance - but hide in Inspector since we don't need to see it
        // We keep it public instead of private so it becomes serialized and maintains persistence between edit/play mode.
        [HideInInspector]
        public MeshFilter meshFilter;
        private float timer = 4f;
        public bool enableChange = true;
        public float colorChangeSpeed = 0.5f;
        public float colorChangeTime = 4f;
        public List<Color> colorList = new List<Color>();
        private int colorIndex = 0;

        private void Start()
        {
            timer = 4;
        }
        /// <summary>
        /// Reset is called when a component is added or when reset is pressed for the object in the inspector.    
        /// </summary>
        void Reset()
        {
            // Create a default gradient (see GradientSkyCommon.cs)
            CreateDefaultGradient();

            // Get a reference for the MeshFilter component (for better performance since we need to check it in every Update()
            meshFilter = GetComponent<MeshFilter>();

            // Create a mesh and set it to become the mesh of the MeshFilter (see GradientSkyCommon.cs)
            meshFilter.sharedMesh = CreateMesh();

            // Create a MeshRenderer (see GradientSkyCommon.cs)
            CreateMeshRenderer();

            // Set the Material to the shader with without any Zwrite and RenderQueue tags
            Material _material = new Material(Shader.Find("Custom/VertexColorObject"));
            GetComponent<MeshRenderer>().sharedMaterial = _material;

            // Set an initial scale of the object so the sky becomes easily visible
            transform.localScale = new Vector3(20, 10, 1);
        }

        /// <summary>
        /// Check if the gradient values and cached gradient values are different - if they are, recreate the mesh to update the gradient.
        /// </summary>
        void Update()
        {
            if (enableChange) {
                timer += Time.deltaTime;
                if (timer >= colorChangeTime)
                {
                    timer = 0;
                    int oldIndex = colorIndex;
                    colorIndex = Random.Range(0, colorList.Count);
                    if (colorIndex == oldIndex)
                    {
                        colorIndex = Random.Range(0, colorList.Count); //Could happen twice but not likely...
                    }
                }
                gradient.SetKeys(new GradientColorKey[] { gradient.colorKeys[0], new GradientColorKey(Color.Lerp(gradient.colorKeys[1].color, colorList[colorIndex], Time.deltaTime * colorChangeSpeed), gradient.colorKeys[1].time), gradient.colorKeys[2] }, gradient.alphaKeys);
                meshFilter.sharedMesh = CreateMesh();
            }
        }
    }
}