using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DashGames
{
    public class Reactional_TriggerStinger : MonoBehaviour
    {
        
            // TODO As for now this script runs on a onClickEvent in the StartScreen Prefab /Playbuttonscore 
            // TODO Implement this script or any stinger with a manager script that calls Reactional.Playback.theme.TriggerStinger
            // TODO In a project you can se the theme stingers and the string names. You can also print all the Reactional variables
            // TODO via Reactional.Playback.Theme.GetControls()
            // TODO <see href="https://docs.reactionalmusic.com/Unity/Unity%20API/Playback.Theme#getcontrols">HERE</see>
        
            /// <summary>
            /// Set the Quant to 0 if you want stinger to play instantly.
            /// Quant set to 1 is on every beat on a bar , in a song on the 1 and the 2 and the 3 and the 4
            /// Qaunt .5 would then be 8th notes, and .25 would be 16th notes
            /// <see href="https://docs.reactionalmusic.com/Unity/">Reactional Unity Docs</see>
            /// </summary>
            /// <param name="quant"></param>
            public void TriggerStingerOnUIClick(float quant)
            {
                Reactional.Playback.Theme.TriggerStinger("positive, large", quant);
            
            
            }
        }
}
