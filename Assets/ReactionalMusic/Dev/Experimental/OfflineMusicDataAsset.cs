using System.Collections.Generic;
using UnityEngine;

namespace Reactional.Experimental
{
    public class OfflineMusicDataAsset : ScriptableObject
    {
        public new string name;
        public float tempo_bpm;
        public List<tempi> tempi = new List<tempi>();
        public List<roots> roots = new List<roots>();
        public List<bar> bars = new List<bar>();
        public List<vocals> vocals = new List<vocals>();
        public List<bass> bass = new List<bass>();
        public List<drums> drums = new List<drums>();
        public List<segments> segments = new List<segments>();
    }

    [System.Serializable]
    public class tempi
    {
        public float[] tempo;
    }

    [System.Serializable]
    public class roots
    {
        public float[] root;
    }

    [System.Serializable]
    public class bar
    {
        public float offset;
        public int barIndex;
        public List<beat> beats = new List<beat>();
    }

    [System.Serializable]
    public class beat
    {
        public float offset;
        public int beatIndex;
    }

    [System.Serializable]
    public class bass 
    {
        public float offset;
        public float offset_seconds;
        public float duration_seconds;
        public float note;
    }

    [System.Serializable]
    public class drums
    {
        public float offset;
        public float offset_seconds;
    }

    [System.Serializable]
    public class segments
    {
        public float offset;
        public float offset_seconds;
        public float duration_seconds;
        public string segment;
    }

    [System.Serializable]
    public class vocals
    {
        public float offset;
        public float offset_seconds;
        public float duration_seconds;
        public float note;
    }
}