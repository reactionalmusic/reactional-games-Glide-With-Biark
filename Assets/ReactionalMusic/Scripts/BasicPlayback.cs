using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace Reactional.Core
{
    [DefaultExecutionOrder(5000)]
    public class BasicPlayback : MonoBehaviour
    {
        [SerializeField] private bool _autoplayTheme;
        [SerializeField] private bool _autoplayTrack;

        [Range(0f,1f)]
        public float _themeVolume = 1f;
        [Range(0f,1f)]
        public float _playlistVolume = 0.6f;

        public void Start()
        {
            if (Reactional.Setup.IsValid)
            {                
                Play();                
            } else {
                Debug.LogWarning("Reactional is not setup correctly. Please check the setup guide.");
            }
        }

        private async void Play()
        {           
            await Task.Delay(100);
            
            // Reactional.Setup.UpdateBundles();                                    // Check for new bundles in StreamingAssets

            await Reactional.Setup.LoadBundles();                                // Load everything in StreamingAssets
            // await Reactional.Setup.LoadBundle("BundleName");                     // Load everything in a specific bundle
            
            // await Reactional.Setup.LoadSection("BundleName","Default");          // Load specific section in specific bundle
            
            // await Reactional.Setup.LoadSection();
            
            // TODO: Load "Default Section" from inspector, or first defined section in first bundle, Deload it if using any other way
            // await Reactional.Setup.LoadTheme("BundleName","Default","ThemeName");// Load specific theme in specific bundle
            // await Reactional.Setup.LoadTheme("Reactional_ScifiBoss copy");                        // Find and load specifc theme in any bundle
            Reactional.Playback.Theme.SetControl("part: silence");
            
            // TODO theme är fucked för kunde haft flera.
            
            //Reactional.Playback.Theme.
            //await Reactional.Setup.LoadTheme();                                   // Load the first theme defined in first bundle

            // await Reactional.Setup.LoadPlaylist("BundleName","Default");         // Load specific playlist in specific bundle
            // await Reactional.Setup.LoadPlaylist("Default");                      // Find and load specifc playlist in any bundle
            // await Reactional.Setup.LoadPlaylist();                               // Load the first playlist defined in first bundle

             //await Reactional.Setup.LoadTrack("GlideWithBiark","Daft Punk - Giorgio by Moroder"); // Load specific track in specific bundle, LOADS ONE TRACK INTO RAM, NO OTHER TRACK WILL BE LOADED

            if (_autoplayTheme)
                Reactional.Playback.Theme.Play();
            if (_autoplayTrack)
                Reactional.Playback.Playlist.Play();
            
                 
            //Reactional.Playback.Playlist.PlayTrack("Daft Punk - Within");
            //Debug.Log("Track playing is: " +Reactional.Playback.Playlist.GetCurrentTrackInfo().trackName);  // NO STRING?
                
            // Important to call this; otherwise there will be a samplerate mismatch; time will drift and music sound bad
            Reactional.Setup.InitAudio();           
            
            // Optionally set volume of theme and playlist
            Reactional.Playback.Theme.Volume = _themeVolume;
            Reactional.Playback.Playlist.Volume = _playlistVolume;

            await Task.Delay(200);
            Reactional.Playback.MusicSystem.PlaybackAllowed = true;
        }
    }
}