using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SongChunker", menuName = "Reactional/Deep Analysis/SongChunker", order = 1)]
public class SongChunker : ScriptableObject
{
    public List<ChunkAsset> chunkAssets;
}