using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Reactional.Core;
using UnityEngine;

namespace DashGames
{
 
    /// <summary>
    /// This Script should live out in the scene as a manager that checks every mesh with a tag in the Scene and pulse it on the beat decided in the tag
    /// Make sure pivots on the meshes are placed where you want them and then decide if you want to scale on x,y or z or all.
    /// What is a Beat <see href="https://en.wikipedia.org/wiki/Beat_(music)">read more about them here</see> 
    /// </summary>
    public class Reactional_PulseMesh_Manager : MonoBehaviour
    {
        //TODO if you want differen scaling parameters for different objects then add new variables and set them on the right beat in ScaleMeshes().
        
        // These headers simply shows what you should tag a mesh with out in the editor to make it pulse
        [Header("Tag mehses with 'PulseOnBeatBar' for pulsing on every beat")]
        [Header("Tag mehses with 'PulseOn4th' for pulsing on every beat")]
        [Header("Tag mehses with 'PulseOn8th' for pulsing on every beat")]
        [Header("Tag mehses with 'PulseOn16th' for pulsing on every beat")]
        
        // These are visible in editor only for the purpose of debuging objects, dont populate it in editor, use the tags. 
        [Header("List of Meshes with tags (List name)")]
        [SerializeField] private List<GameObject> PulseOnBeatBar = new List<GameObject>();
        [SerializeField] private List<GameObject> PulseOn4th = new List<GameObject>();
        [SerializeField] private List<GameObject> PulseOn8th = new List<GameObject>();
        [SerializeField] private List<GameObject> PulseOn16th = new List<GameObject>();

        [SerializeField] private Dictionary<string, float> TagAndQuantMap = new()
        {
            { "PulseOnBeatBar", 1f },
            { "PulseOn4th", 4f },
            { "PulseOn8th", 8f },
            { "PulseOn16th", 16f },
        };

        // Set to scale only on Y by default
        [Header("Mesh Scale Parameters")]
        [SerializeField] private float xScale = 1f;
        [SerializeField] private float yScale = 1.3f;
        [SerializeField] private float zScale = 1f;
        
        
        private ReactionalEngine reactional;

        private void Start()
        {
            // Get the ReactionalEngine from the scene
            reactional = ReactionalEngine.Instance;
            if (reactional == null)
            {
                Debug.LogError("ReactionalEngine not found in the scene!");
                return;
            }

            // Fill the list with tagged GameObjects
            AddTaggedMeshes();
            StoreOriginalScales();
        }

        private void Update()
        {
            ScaleMeshes();
            
        }


        private void AddTaggedMeshes()
        {
            PulseOnBeatBar.Clear();
            PulseOn4th.Clear();
            PulseOn8th.Clear();
            PulseOn16th.Clear();
            
            // Use AddRange to directly add all the tagged objects to the list
            PulseOnBeatBar.AddRange(GameObject.FindGameObjectsWithTag("PulseOnBeatBar"));
            PulseOn4th.AddRange(GameObject.FindGameObjectsWithTag("PulseOn4th"));
            PulseOn8th.AddRange(GameObject.FindGameObjectsWithTag("PulseOn8th"));
            PulseOn16th.AddRange(GameObject.FindGameObjectsWithTag("PulseOn16th"));
            
            
            // Store all lists in a dictionary with their names
            Dictionary<string, List<GameObject>> listCollection = new Dictionary<string, List<GameObject>>
            {
                { "PulseOnBeatBar", PulseOnBeatBar },
                { "PulseOn4th", PulseOn4th },
                { "PulseOn8th", PulseOn8th },
                { "PulseOn16th", PulseOn16th }
            };

            // Check if any list is null or empty and log its name
            foreach (var entry in listCollection)
            {
                if (entry.Value == null || entry.Value.Count == 0)
                {
                    Debug.LogWarning($"{entry.Key} list is null or empty.");
                }
            }
           
        }
        
        // Dictionary to store the original scales of all meshes
        private Dictionary<GameObject, Vector3> originalScalesMap = new Dictionary<GameObject, Vector3>();

        private void StoreOriginalScales()
        {
            // Clear the original scales dictionary to avoid duplicate entries
            originalScalesMap.Clear();

            // Store the original scales for each list of GameObjects
            StoreMeshScalesInMap(PulseOnBeatBar);
            StoreMeshScalesInMap(PulseOn4th);
            StoreMeshScalesInMap(PulseOn8th);
            StoreMeshScalesInMap(PulseOn16th);
        }

        private void StoreMeshScalesInMap(List<GameObject> meshList)
        {
            foreach (var mesh in meshList)
            {
                if (mesh != null && !originalScalesMap.ContainsKey(mesh))
                {
                    originalScalesMap[mesh] = mesh.transform.localScale;
                }
            }
        }

       
        private void ScaleMeshes()
        {
            ScaleMeshesInList(PulseOnBeatBar, "PulseOnBeatBar");
            ScaleMeshesInList(PulseOn4th, "PulseOn4th");
            ScaleMeshesInList(PulseOn8th, "PulseOn8th");
            ScaleMeshesInList(PulseOn16th, "PulseOn16th");
        }

        // private void ScaleMeshesInList(List<GameObject> meshList, string tag)
        // {
        //     foreach (GameObject mesh in meshList)
        //     {
        //         if (mesh != null && originalScalesMap.TryGetValue(mesh, out Vector3 originalScale))
        //         {
        //             Vector3 targetScale = new Vector3(originalScale.x * xScale, originalScale.y * yScale, originalScale.z * zScale) * NormalizeQuantizations(TagAndQuantMap[tag]);
        //             mesh.transform.localScale = targetScale;
        //         }
        //     }
        // }
        
        private void ScaleMeshesInList(List<GameObject> meshList, string tag)
        {
            foreach (GameObject mesh in meshList)
            {
                if (mesh != null && originalScalesMap.TryGetValue(mesh, out Vector3 originalScale))
                {
                    // Get the normalized quantization value
                    float quantValue = NormalizeQuantizations(TagAndQuantMap[tag]);

                    // Calculate the maximum target scale using the original scale and scaling factors
                    Vector3 maxScale = new Vector3(originalScale.x * xScale, originalScale.y * yScale, originalScale.z * zScale);

                    // Interpolate between the original scale and the maxScale using the quantValue
                    Vector3 targetScale = Vector3.Lerp(originalScale, maxScale, quantValue);

                    // Apply the calculated target scale to the mesh
                    mesh.transform.localScale = targetScale;
                }
            }
        }
        
        /// <summary>
        /// If Quant is 1 its on every beat
        /// After that use 4,8,16 to get the right qants for the beats
        /// </summary>
        /// <param name="quant"></param>
        /// <returns></returns>
        private float NormalizeQuantizations(float quant)
        {
            // Get the current beat, normalized to cycle between 0 and 1
            return Reactional.Playback.MusicSystem.GetTimeToBeat(1) * quant % 1;
         
        }

       
    }
}
