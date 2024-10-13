using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChunkAsset", menuName = "Reactional/Deep Analysis/ChunkAsset", order = 1)]
public class ChunkAsset : ScriptableObject
{
    public List<int> intList = new List<int>(new int[16]);
}