using System.Collections.Generic;
using UnityEngine;
using Reactional.Core;
using Reactional.Experimental;
using System;
public class ChunkSpawner : MonoBehaviour
{
    public SongChunker songChunker;
    public GameObject boxPrefab;
    public GameObject mediumBoxPrefab;
    public GameObject bigBoxPrefab;
    private List<float> accumulatedPositions = new List<float>();
    private List<GameObject> accumulatedObjects = new List<GameObject>();
    private List<int> accumulatedEntries = new List<int>();

    private List<float> accumulatedBassPositions = new List<float>();
    private List<GameObject> accumulatedBassObjects = new List<GameObject>();
    private List<int> accumulatedBassEntries = new List<int>();
    private int accumulatedEntry = 0;

    public bool autoChunk = true;

    public OfflineMusicDataAsset offlineMusicDataAsset;

    void Start()
    {
        if (autoChunk)
        {
            SpawnVocals();
            //ChunkSong();
            return;
        }

        foreach (var chunk in songChunker.chunkAssets)
        {
            SpawnChunk(chunk);
        }
    }

    void ChunkSong()
    {
        songChunker.chunkAssets.Clear();
        ChunkAsset chunk = new ChunkAsset();
        chunk.intList = new List<int>();
        foreach (var bar in offlineMusicDataAsset.bars)
        {
            foreach (var beat in bar.beats)
            {
                chunk.intList.Add(0);
            }
        }

        foreach (var vocal in offlineMusicDataAsset.vocals)
        {
            // round vocal.beat to nearest beat
            int beatIndex = Mathf.RoundToInt(vocal.offset);
            chunk.intList[beatIndex] = UnityEngine.Random.Range(1, 4);
        }

        songChunker.chunkAssets.Add(chunk);
    }

    void SpawnVocals()
    {
        float prev_offset = 0;
        float prev_pitch = 0;
        float prev_end = 0;
        foreach (var vocal in offlineMusicDataAsset.vocals)
        {
            // round to closes 0.25f
            float offset = (float)Math.Round(vocal.offset * 4) / 4f;

            if (Mathf.Round(vocal.note) % 12 == prev_pitch % 12 && vocal.offset_seconds + 0.3f < prev_end)
            {
                continue;
            }

            //int beatIndex = Mathf.RoundToInt(vocal.offset);
            if ((offset % 1 != 0f || offset % 1 != 0.5f) && vocal.duration_seconds < 0.1f)
            {
                //Debug.Log("Skipping vocal with duration_seconds < 0.1f");
                //continue;
            }

            // if (offset % 1 == 0.25f || offset % 1 == 0.125f)
            // {
            //     offset = Mathf.Round(offset);
            // }

            if (offset <= prev_offset + 0.25f)
            {
                continue;
            }

            if (offset == prev_offset)
            {
                continue;
            }
            
            if (offset > prev_offset + 4)
            {
                // round to closest bar
               // offset = Mathf.Round(offset / 4) * 4;
            }

            prev_offset = offset;

            boxPrefab.GetComponent<pitchdata>().pitch = vocal.note;

            Vector3 position = new Vector3((vocal.note % 12) / 4, offset * 5, 0);
            var obj = Instantiate(boxPrefab, position, Quaternion.identity, gameObject.transform);
            accumulatedObjects.Add(obj);
            accumulatedPositions.Add(position.y);
            accumulatedEntries.Add(accumulatedEntry);
        }
        prev_offset = 0;
        prev_pitch = 0;
        prev_end = 0;
        foreach (var bass in offlineMusicDataAsset.bass)
        {
            if (Mathf.Round(bass.note) % 12 == prev_pitch % 12 && bass.offset_seconds + 0.3f < prev_end)
            {
                continue;
            }
            // round to closes 0.25f
            float offset = (float)Math.Round(bass.offset * 4) / 4f;

            //int beatIndex = Mathf.RoundToInt(vocal.offset);
            if ((offset % 1 != 0f || offset % 1 != 0.5f) && bass.duration_seconds < 0.15f)
            {
                Debug.Log("Skipping vocal with duration_seconds < 0.1f");
                continue;
            }

            if (offset % 1 == 0.25f || offset % 1 == 0.75f)
            {
                offset = Mathf.Round(offset);
            }

            if (offset <= prev_offset + 0.25f)
            {
                continue;
            }

            if (offset == prev_offset)
            {
                continue;
            }
            
            if (offset > prev_offset + 4)
            {
                // round to closest bar
                //offset = Mathf.Round(offset / 4) * 4;
            }

            prev_offset = offset;
            prev_pitch = Mathf.Round(bass.note);
            prev_end = bass.offset_seconds + bass.duration_seconds;
            mediumBoxPrefab.GetComponent<pitchdata>().pitch = bass.note;

            Vector3 position = new Vector3((bass.note % 12) / 4 - 4, offset * 5, 0);
            var obj = Instantiate(mediumBoxPrefab, position, Quaternion.identity, gameObject.transform);
            accumulatedObjects.Add(obj);
            accumulatedPositions.Add(position.y);
            accumulatedEntries.Add(accumulatedEntry);
        }
        prev_offset = 0;
        foreach (var drums in offlineMusicDataAsset.drums){
            // round to closes 0.25f
            float offset = (float)Math.Round(drums.offset * 4) / 4f;

            //int beatIndex = Mathf.RoundToInt(vocal.offset);
            // if ((offset % 1 != 0f || offset % 1 != 0.5f) && drums.duration_seconds < 0.1f)
            // {
            //     //Debug.Log("Skipping vocal with duration_seconds < 0.1f");
            //     //continue;
            // }

            // if (offset % 1 == 0.25f || offset % 1 == 0.125f)
            // {
            //     offset = Mathf.Round(offset);
            // }

            if (offset <= prev_offset + 0.25f)
            {
               continue;
            }

            if (offset == prev_offset)
            {
                continue;
            }
            
            if (offset > prev_offset + 4)
            {
                //round to closest bar
                offset = Mathf.Round(offset / 4) * 4;
            }

            prev_offset = offset;

            bigBoxPrefab.GetComponent<pitchdata>().pitch = 128;

            Vector3 position = new Vector3(-5, offset * 5, 0);
            var obj = Instantiate(bigBoxPrefab, position, Quaternion.identity, gameObject.transform);
            accumulatedObjects.Add(obj);
            accumulatedPositions.Add(position.y);
            accumulatedEntries.Add(accumulatedEntry);
        }
    }

    void SpawnChunk(ChunkAsset chunk)
    {
        for (int i = 0; i < chunk.intList.Count; i++)
        {
            int value = chunk.intList[i];
            if (value > 0)
            {
                GameObject pref = boxPrefab;
                if (i == 0)
                {
                    pref = bigBoxPrefab;
                }
                else if (i == 8)
                {
                    pref = mediumBoxPrefab;
                }
                Vector3 position = new Vector3((value * 2) - 5, accumulatedEntry * 5, 0);
                var obj = Instantiate(pref, position, Quaternion.identity, gameObject.transform);
                accumulatedObjects.Add(obj);
                accumulatedPositions.Add(position.y);
                accumulatedEntries.Add(accumulatedEntry);
            }
            accumulatedEntry++;
        }
    }

    void Update()
    {
        if (ReactionalEngine.Instance.CurrentBeat > 0)
        {
            MoveChunks();
        }

    }

    void MoveChunks()
    {
        float beatPosition = ReactionalEngine.Instance.CurrentBeat * 5;
        for (int i = 0; i < accumulatedObjects.Count; i++)
        {
            float position = accumulatedPositions[i];
            accumulatedObjects[i].transform.position = new Vector3(accumulatedObjects[i].transform.position.x, position - beatPosition, accumulatedObjects[i].transform.position.z);
        }
        for (int i = 0; i < accumulatedBassObjects.Count; i++)
        {
            float position = accumulatedBassPositions[i];
            accumulatedBassObjects[i].transform.position = new Vector3(accumulatedBassObjects[i].transform.position.x, position - beatPosition, accumulatedBassObjects[i].transform.position.z);
        }
    }
}