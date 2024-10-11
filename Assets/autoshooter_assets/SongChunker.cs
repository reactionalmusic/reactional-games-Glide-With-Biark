using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSongChunker", menuName = "Song Chunker", order = 1)]
public class SongChunker : ScriptableObject
{
    public List<ChunkAsset> chunkAssets;
}