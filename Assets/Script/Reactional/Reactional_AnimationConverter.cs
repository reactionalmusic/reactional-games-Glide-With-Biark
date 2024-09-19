using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DashGames
{
    
    //TODO The idea here is to slot this to a prefab that runs different animations. And the script checks every animation name in that 
    // TODO state machine, And scales the length of the animation to whatever beat is stored in currBeat variable. 
    // TODO To make it work with only one clip in a sertan situation modification is needed on your side to set up as you please. 
    // TODO One more script is added underneeth , but commented out, that only takes one clip an asset in the world uses. 
    // TODO Call the animation when needed prefarably and not in update if you dont want to repeat it. 

    public class Reactional_Animation : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] RuntimeAnimatorController controller;
        private Dictionary<string, AnimationClip> animationClips;

        private void Start()
        {
            // Get the Animator's controller and all its animation clips and save them in a Dictionary
            controller = animator.runtimeAnimatorController;
            animationClips = new Dictionary<string, AnimationClip>();

            foreach (var animClip in controller.animationClips)
            {
                animationClips[animClip.name] = animClip;
            }
        }

        private void Update()
        {
            PlayAnimation();
        }
        
        /// <summary>
        /// Saves the beat you want to scale the animation with. There is plenty more beats over witch you can sacle an animation
        /// Ask or visit https://docs.reactionalmusic.com/Unity/Unity%20API/Overview to see what other functioncalls you can use and save
        /// </summary>
        private void PlayAnimation()
        {
            
            // Check if a track is loaded
            if (Reactional.Playback.Playlist.IsLoaded())
            {
                // Set the beat you want to scale animation over
                float currBeat = Reactional.Playback.MusicSystem.GetCurrentBeat();
                // Play all animations using the current beat
                foreach (var animClip in animationClips)
                {
                    animator.Play(animClip.Key, 0, currBeat % 1);
                }

                animator.Update(0);
            }
            
        }
        
        
    }

    
}



//-------------------- For only one animation clip use this ------------------------------------
    // public class Reactional_Animation : MonoBehaviour
    // {
    //
    //     [SerializeField] private Animator animator;
    //     [SerializeField] private AnimationClip clip;
    //     [SerializeField] private Controller player;
    //
    //     private void PlayAnimationClip()
    //     {
    //         if (Reactional.Playback.Playlist.IsLoaded()) 
    //         {
    //             float currBeat = Reactional.Playback.MusicSystem.GetCurrentBeat();
    //             animator.Play(clip.name, 0, currBeat % 1);
    //             animator.Update(0);
    //         }
    //     }
    // }


