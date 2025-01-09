#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Reactional.Core;
using UnityEditor;
using UnityEngine;

namespace Reactional.Experimental
{
    public static class GenerateAsset
    {

        static MusicData GetMusicData(TrackInfo ti)
        {
           string trackJson = AssetHelper.ExportAnalysisData(Application.streamingAssetsPath + "/Reactional/" + ti.bundleID + "/" + ti.trackHash);

            MusicData md = new MusicData();
            var jd = MiniJSON.Json.Deserialize(trackJson) as Dictionary<string, object>;
            // md.id = (string)jd["id"];
            // //            md.name = (string)jd["name"];
            // md.tempi = new List<int[]>();
            // foreach (List<object> l in (List<object>)jd["tempi"])
            // {
            //     int[] t = new int[l.Count];
            //     for (int i = 0; i < l.Count; i++)
            //     {
            //         t[i] = (int)(long)l[i];
            //     }
            //     md.tempi.Add(t);
            // }
            //
            // md.roots = new List<int[]>();
            // foreach (List<object> l in (List<object>)jd["roots"])
            // {
            //     int[] t = new int[l.Count];
            //     for (int i = 0; i < l.Count; i++)
            //     {
            //         t[i] = (int)(long)l[i];
            //     }
            //     md.roots.Add(t);
            // }
            //
            // md.bars = new List<int[]>();
            // foreach (List<object> l in (List<object>)jd["bars"])
            // {
            //     int[] t = new int[l.Count];
            //     for (int i = 0; i < l.Count; i++)
            //     {
            //         t[i] = (int)(long)l[i];
            //     }
            //     md.bars.Add(t);
            // }

            // md.beats = new List<int[]>();
            // foreach (List<object> l in (List<object>)jd["beats_in_bars"])
            // {
            //     int[] t = new int[l.Count];
            //     for (int i = 0; i < l.Count; i++)
            //     {
            //         t[i] = (int)(long)l[i];
            //     }
            //     md.beats.Add(t);
            // }

            // Access "deep_analysis_data" dictionary
            
            
            if(jd != null && !jd.ContainsKey("deep_analysis_data") && jd["deep_analysis_data"] != null)
            {
                return null;
            };
            
            
            var deepAnalysisData = jd["deep_analysis_data"] as Dictionary<string, object>;

            // Extract the "vocals" array from the "deep_analysis_data" dictionary
            var vocalsList = deepAnalysisData["vocals"] as List<object>;

            // Parse each entry in the "vocals" list
            md.vocals = new List<PitchEntry>();
            foreach (var item in vocalsList)
            {
                var vocalEntry = item as List<object>;
                long timeStamp = (long)vocalEntry[0]; // JSON numbers are deserialized as doubles

                var dataDict = vocalEntry[1] as Dictionary<string, object>;
                PitchData vocalData = new PitchData
                {
                    Offset = float.Parse(dataDict["offset"].ToString()),
                    Duration = float.Parse(dataDict["duration"].ToString()),
                    Note = float.Parse(dataDict["note"].ToString())
                };

                PitchEntry entry = new PitchEntry
                {
                    TimeStamp = timeStamp,
                    Data = vocalData
                };

                md.vocals.Add(entry);
            }

            var bassList = deepAnalysisData["bass"] as List<object>;
            md.bass = new List<PitchEntry>();
            foreach (var item in bassList)
            {
                var bassEntry = item as List<object>;
                long timeStamp = (long)bassEntry[0]; // JSON numbers are deserialized as doubles

                var dataDict = bassEntry[1] as Dictionary<string, object>;
                PitchData bassData = new PitchData
                {
                    Offset = float.Parse(dataDict["offset"].ToString()),
                    Duration = float.Parse(dataDict["duration"].ToString()),
                    Note = float.Parse(dataDict["note"].ToString())
                };

                PitchEntry entry = new PitchEntry
                {
                    TimeStamp = timeStamp,
                    Data = bassData
                };

                md.bass.Add(entry);
            }

            var drumsList = deepAnalysisData["drums"] as List<object>;
            md.drums = new List<DrumsEntry>();
            foreach (var item in drumsList)
            {
                var drumsEntry = item as List<object>;
                long timeStamp = (long)drumsEntry[0]; // JSON numbers are deserialized as doubles

                var dataDict = drumsEntry[1] as Dictionary<string, object>;
                DrumsData drumsData = new DrumsData
                {
                    Offset = float.Parse(dataDict["offset"].ToString())
                };

                DrumsEntry entry = new DrumsEntry
                {
                    TimeStamp = timeStamp,
                    Data = drumsData
                };

                md.drums.Add(entry);
            }

            var segmentsList = deepAnalysisData["segments"] as List<object>;
            md.segments = new List<SegmentsEntry>();
            foreach (var item in segmentsList)
            {
                var segmentsEntry = item as List<object>;
                long timeStamp = (long)segmentsEntry[0]; // JSON numbers are deserialized as doubles

                var dataDict = segmentsEntry[1] as Dictionary<string, object>;
                SegmentsData segmentsData = new SegmentsData
                {
                    Offset = float.Parse(dataDict["offset"].ToString()),
                    Duration = float.Parse(dataDict["duration"].ToString()),
                    Label = dataDict["label"].ToString()
                };

                SegmentsEntry entry = new SegmentsEntry
                {
                    TimeStamp = timeStamp,
                    Data = segmentsData
                };

                md.segments.Add(entry);
            }

            return md;
        }
        public static void Single(TrackInfo ti)
        {
            Debug.Log("Creating asset for: " + ti.trackName);
            MusicData musicData = GetMusicData(ti);
            if (musicData == null)
            {
                return;
            }
            
            OfflineMusicDataAsset asset = ScriptableObject.CreateInstance<OfflineMusicDataAsset>();
            asset.name = musicData.name;
            Debug.Log("Creating asset for: " + asset.name);
            float bpm = 0;
            float prev_bpm = 0;
            
            //TODO be david lägga till i objecktet
            
            //
            // foreach (var tempo in musicData.tempi)
            // {
            //     tempi t = new tempi();
            //     t.tempo = new float[2];
            //     t.tempo[0] = tempo[0] / 1000000;
            //     t.tempo[1] = (tempo[1] / 1000000f) * 60;
            //     if (prev_bpm != t.tempo[1])
            //         asset.tempi.Add(t);
            //     bpm += t.tempo[1];
            //     prev_bpm = t.tempo[1];
            // }
            //
            // //avarage bpm
            // asset.tempo_bpm = bpm / musicData.tempi.Count;
            //
            // foreach (var root in musicData.roots)
            // {
            //     roots r = new roots();
            //     r.root = new float[2];
            //     r.root[0] = root[0] / 1000000f;
            //     r.root[1] = root[1];
            //     asset.roots.Add(r);
            // }
            //
            // //bar beats
            // if (musicData.bars != null)
            // {
            //     foreach (var bar in musicData.bars)
            //     {
            //         bar b = new bar();
            //         b.offset = (float)(long)bar[0] / 1000000f;
            //         b.barIndex = (int)(long)bar[1];
            //         asset.bars.Add(b);
            //     }
            // }
            // if (musicData.beats != null)
            // {
            //     foreach (var beat in musicData.beats)
            //     {
            //         beat b = new beat();
            //         b.offset = (float)(long)beat[0] / 1000000f;
            //         b.beatIndex = (int)(long)beat[1];
            //
            //         // check which bar this beat belongs to
            //         for (int i = 0; i < asset.bars.Count - 1; i++)
            //         {
            //             if (b.offset >= asset.bars[i].offset && b.offset < asset.bars[i + 1].offset)
            //             {
            //                 asset.bars[i].beats.Add(b);
            //                 break;
            //             }
            //         }
            //     }
            // }
            if (musicData.vocals != null)
            {
                foreach (var vocal in musicData.vocals)
                {
                    vocals v = new vocals();
                    v.offset = vocal.TimeStamp / 1000000f;
                    v.offset_seconds = vocal.Data.Offset;
                    v.duration_seconds = vocal.Data.Duration;
                    v.note = vocal.Data.Note;
                    asset.vocals.Add(v);
                }
            }
            if (musicData.bass != null)
            {
                foreach (var bass in musicData.bass)
                {
                    bass b = new bass();
                    b.offset = bass.TimeStamp / 1000000f;
                    b.offset_seconds = bass.Data.Offset;
                    b.duration_seconds = bass.Data.Duration;
                    b.note = bass.Data.Note;
                    asset.bass.Add(b);
                }
            }
            if (musicData.drums != null)
            {
                foreach (var drum in musicData.drums)
                {
                    drums d = new drums();
                    d.offset = drum.TimeStamp / 1000000f;
                    d.offset_seconds = drum.Data.Offset;
                    asset.drums.Add(d);
                }
            }
            if (musicData.segments != null)
            {
                foreach (var segment in musicData.segments)
                {
                    segments s = new segments();
                    s.offset = segment.TimeStamp / 1000000f;
                    s.offset_seconds = segment.Data.Offset;
                    s.duration_seconds = segment.Data.Duration;
                    s.segment = segment.Data.Label;
                    asset.segments.Add(s);
                }
            }


            var pathname = GetUniqueFilename(ti.trackName + ti.trackHash);


            AssetDatabase.CreateAsset(asset, pathname);
            Debug.Log("Asset created at: " + pathname);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }


        private static string GetUniqueFilename(string trackname)
        {
            var path = "Assets/ReactionalData";

            //Remove whitespaces
            var name = new string(trackname.ToCharArray()
                .Where(c => !char.IsWhiteSpace(c))
                .ToArray());

            //check for no name
            if (name.Length == 0) name = "Unnamed";


            //check if folder exists else create it
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            var version = 1;

            var finalFilename = path + "/" + name + "_" + version + ".asset";

            // Increment version number until we find a non-existing filename
            while (File.Exists(finalFilename))
            {
                version++;
                finalFilename = path + "/" + name + "_" + version + ".asset";
            }

            return finalFilename;
        }
    
   

        public static void All()
        {
            
            foreach (var b in  ReactionalManager.Instance.bundles)
            {
                foreach (var s in b.sections)
                {
                    foreach (var p in s.playlists)
                    {
                        foreach (var t in p.tracks)
                        {
#if UNITY_EDITOR
                            Debug.Log("Creating asset for: " + t.trackHash);
#endif
                            Single(t);
                        }
                    }
                }
            }
            
        }


        
        [UnityEditor.MenuItem("Tools/Reactional/DeepAnalysis/Export Analysis Data", false, 400)]
        public static void Init()
        {
//            var file = UnityEditor.Selection.activeObject.name;
            All();
        }
#endif

        [System.Serializable]
        public class MusicData
        {
            public string id { get; set; }
            public string name { get; set; }
            public List<int[]> tempi { get; set; }
            public List<int[]> roots { get; set; }
            public List<int[]> bars { get; set; }
            public List<int[]> beats { get; set; }
            public List<List<object>> scales { get; set; }
            public List<List<object>> tunings { get; set; }
            public List<PitchEntry> vocals { get; set; }
            public List<PitchEntry> bass { get; set; }
            public List<DrumsEntry> drums { get; set; }
            public List<SegmentsEntry> segments { get; set; }
        }

        public class PitchEntry
        {
            public long TimeStamp { get; set; } // Corresponds to the first element in each list (13375480, 22599942, etc.)
            public PitchData Data { get; set; } // Corresponds to the dictionary with "offset", "duration", and "note"
        }

        public class PitchData
        {
            public float Offset { get; set; }
            public float Duration { get; set; }
            public float Note { get; set; }
        }

        public class DrumsEntry
        {
            public long TimeStamp { get; set; }
            public DrumsData Data { get; set; }
        }

        public class DrumsData
        {
            public float Offset { get; set; }
        }

        public class SegmentsEntry
        {
            public long TimeStamp { get; set; }
            public SegmentsData Data { get; set; }
        }

        public class SegmentsData
        {
            public float Offset { get; set; }
            public float Duration { get; set; }
            public string Label { get; set; }
        }
    }

}