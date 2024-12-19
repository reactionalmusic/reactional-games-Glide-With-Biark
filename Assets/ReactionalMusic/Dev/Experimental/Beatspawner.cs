using UnityEngine;
using System.Collections.Generic;
using Reactional.Core;

namespace Reactional.Experimental
{
    public class BeatSpawner : MonoBehaviour
    {
        public GameObject smallCubePrefab; // Assign the prefab for the small cube in the inspector
        public GameObject largeCubePrefab; // Assign the prefab for the large cube in the inspector

        public float moveSpeed = 1.0f; // Adjust this to set the speed at which cubes move towards the camera
        public Transform cameraTransform; // Assign the camera transform in the inspector
        private List<GameObject> spawnedCubes = new();

        public OfflineMusicDataAsset asset;

        float deltaBeat;

        public UnityEngine.UI.Text text;

        public GameObject parent;

        public void Create()
        {
            if (parent == null)
                parent = new GameObject();
                
            parent.transform.position = transform.position;
            parent.name = "Cubes";
            
            // Spawn large cubes for each bar
            foreach (var bar in asset.bars)
            {
                Vector3 position = new Vector3(transform.position.x + bar.offset, transform.position.y,
                    transform.position.z);
                GameObject largeCube = Instantiate(largeCubePrefab, position, Quaternion.identity);
                largeCube.GetComponentInChildren<UnityEngine.UI.Text>().text = bar.barIndex.ToString();
                largeCube.transform.parent = parent.transform;
                spawnedCubes.Add(largeCube);

                // Spawn small cubes for each beat in the bar
                foreach (var beat in bar.beats)
                {
                    Vector3 bposition = new Vector3(transform.position.x + beat.offset, transform.position.y,
                        transform.position.z);
                    GameObject smallCube = Instantiate(smallCubePrefab, bposition, Quaternion.identity);
                    smallCube.GetComponentInChildren<UnityEngine.UI.Text>().text = beat.beatIndex.ToString();
                    smallCube.transform.parent = parent.transform;
                    spawnedCubes.Add(smallCube);
                }
            }
        }

        public void Start()
        {
            if (parent == null)
                Create();
        }

        public void Update()
        {
            text.text = ReactionalEngine.Instance.CurrentBeat.ToString();
            
            cameraTransform.position = new Vector3(
                ReactionalEngine.Instance.CurrentBeat - 0.75f,
                cameraTransform.position.y,
                cameraTransform.position.z
            );
        }
    }
}
