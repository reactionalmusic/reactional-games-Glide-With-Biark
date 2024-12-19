#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Reactional.Experimental
{
    [CustomEditor(typeof(BeatSpawner))]
    public class BeatspawnerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            // Button to generate the asset
            if (GUILayout.Button("Generate Boxes"))
            {
                // Get the Beatspawner component
                BeatSpawner beatSpawner = (BeatSpawner)target;
                // Call the Create method
                beatSpawner.Create();
            }
        }
    }
}
#endif
