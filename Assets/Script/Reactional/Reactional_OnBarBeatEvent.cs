using System.Collections;
using System.Collections.Generic;
using Reactional.Core;
using UnityEngine;

namespace DashGames
{
    public class Reactional_OnBarBeatEvent : MonoBehaviour
    {
        
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

           
        }
        private void UpdateBarBeat(double offset, int bar, int beatIndex)
        {
            // Scale the objects when the bar beat event is triggered
            if (reactional != null)
            {
                
                Debug.Log($"Bar: {bar}, Beat Index: {beatIndex}");
            }
        }
        
        //---------------Handle Events------------------
      
        private void OnEnable() 
        {
            
            reactional.onBarBeat += UpdateBarBeat;
        }

        private void OnDisable()
        {
            reactional.onBarBeat -= UpdateBarBeat;
        }
    }
}
