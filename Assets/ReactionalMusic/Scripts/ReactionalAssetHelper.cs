using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MiniJSON;
using UnityEngine;
using UnityEngine.Networking;

namespace Reactional.Core
{
    internal class AssetHelper : MonoBehaviour
    {
        List<string> themes = new List<string>();
        List<string> tracks = new List<string>();

        // TODO: Make async?
        public static int AddTrackFromPath(Engine engine, string path)
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(path);
                www.SendWebRequest();
                while (!www.isDone)
                {
                }
                return CheckTrackID(engine.AddTrackFromBytes(www.downloadHandler.data));
            }
            else
                return CheckTrackID(engine.AddTrackFromPath(path));
        }

        static Dictionary<string, string> ReadTrackMetadataFromPath(string path, string trackpath, Engine engine = null)
        {
            var trackPath = path + "/" + trackpath;

            string jsonText;
            if (Application.platform == RuntimePlatform.Android)
            {
                UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(trackPath);
                www.SendWebRequest();
                while (!www.isDone)
                {
                }
                jsonText = Reactional.Core.Engine.GetTrackMetadata(www.downloadHandler.data);
            }
            else
            {
                byte[] data = File.ReadAllBytes(trackPath);
                jsonText = Reactional.Core.Engine.GetTrackMetadata(data);
            }

            var dict = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(jsonText))
            {
                var meta = Json.Deserialize(jsonText) as Dictionary<string, object>;
                Debug.Log(jsonText);
                // var tempArtists = meta["artists"] as List<object>;
                dict.Add("name", meta.ContainsKey("title") && meta["title"] != null ? meta["title"].ToString() : " ");
                dict.Add("artists",
                    meta.ContainsKey("artists") && meta["artists"] != null ? meta["artists"].ToString() : " ");
                dict.Add("title", meta.ContainsKey("title") && meta["title"] != null ? meta["title"].ToString() : " ");
                dict.Add("album", meta.ContainsKey("album") && meta["album"] != null ? meta["album"].ToString() : " ");
                dict.Add("genre", meta.ContainsKey("genre") && meta["genre"] != null ? meta["genre"].ToString() : " ");
                dict.Add("bpm", meta.ContainsKey("bpm") && meta["bpm"] != null ? meta["bpm"].ToString() : " ");
                dict.Add("cover", meta.ContainsKey("cover") && meta["cover"] != null ? meta["cover"].ToString() : " ");
            }
            else
            {
                dict.Add("name", ""); // fallback
            }

            return dict;
        }

        static int CheckTrackID(int id)
        {            
            if (id >= 0) return id;
            try
            {   
                throw new EngineErrorException(id);
            }
            catch (EngineErrorException e)
            {
                if (e.Error == -70)
                    Debug.LogWarning("Track timestamp has expired; please download a new version of the track. \nUpgrade your project for longer timestamp validity. License this track to remove timestamp restrictions.");
                else
                    Debug.LogError("Track validation failed: " + e.Message);
            }
            return -1;
        }

        public static List<Section> ParseBundle(string path)
        {
            string jsonText;
            if (Application.platform == RuntimePlatform.Android)
            {
                UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(path + "/manifest.json");
                www.SendWebRequest();
                while (!www.isDone)
                {
                }
                jsonText = www.downloadHandler.text;
            }
            else
            {
                jsonText = File.ReadAllText(path + "/manifest.json");
            }

            List<TrackInfo> themes = new List<TrackInfo>();
            List<TrackInfo> tracks = new List<TrackInfo>();
            List<Section> sectionsList = new List<Section>();
            int idCount = 0;

            var json = MiniJSON.Json.Deserialize(jsonText) as Dictionary<string, object>;

            Dictionary<string, object> sections = (Dictionary<string, object>)json["sections"];
            foreach (KeyValuePair<string, object> sectionPair in sections)
            {
                Section s = new Section();
                s.name = sectionPair.Key;
                Dictionary<string, object> sectionData = sectionPair.Value as Dictionary<string, object>;

                List<object> themes_list = sectionData["themes"] as List<object>;
                for (int i = 0; i < themes_list.Count; i++)
                {
                    string thm = themes_list[i] as string;

                    TrackInfo ti = new TrackInfo();
                    ti.trackHash = thm;
                    Dictionary<string, string> trackInfoDict = ReadTrackMetadataFromPath(path, ti.trackHash);

                    ti.trackID = -1;
                    idCount++;

                    ti.trackName = trackInfoDict["name"];
                    ti.bundleID = System.IO.Path.GetFileName(path);
                    ti.name = ti.trackName;
                    themes.Add(ti);
                    s.themes.Add(ti);
                }

                List<object> playlists = sectionData["playlists"] as List<object>;
                foreach (var playlistObject in playlists)
                {
                    Dictionary<string, object> playlist = playlistObject as Dictionary<string, object>;
                    Playlist pl = new Playlist();
                    string playlistName = playlist["name"] as string;
                    pl.name = playlistName;

                    List<object> tracks_list = playlist["tracks"] as List<object>;
                    for (int i = 0; i < tracks_list.Count; i++)
                    {
                        string trck = tracks_list[i] as string;

                        TrackInfo ti = ParseTrack(path, trck);
                        idCount++;

                        pl.tracks.Add(ti);
                    }
                    s.playlists.Add(pl);
                }

                sectionsList.Add(s);
            }
            return sectionsList;
        }

        public static TrackInfo ParseTrack(string bundlePath, string trck)
        {
            TrackInfo ti = new TrackInfo();
            ti.trackHash = trck;
            ti.trackID = -1;

            var trackDict = ReadTrackMetadataFromPath(bundlePath, ti.trackHash);
            ti.trackName = trackDict["name"];
            ti.trackArtist = trackDict["artists"];
            ti.trackAlbum = trackDict["album"];
            ti.trackGenre = trackDict["genre"];                
            ti.trackBPM = trackDict["bpm"];
            ti.trackCover = trackDict["cover"];
            ti.bundleID = System.IO.Path.GetFileName(bundlePath);
            ti.name = ti.trackName;
            return ti;
        }

        public static async Task LoadTrackAssets(Engine engine, int trackid, string projectPath, bool loadAsync = true, bool streaming = false)
        {            
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < engine.GetNumAssets(trackid); i++)
            {
                string id = engine.GetAssetId(trackid, i);
                string type = engine.GetAssetType(trackid, i);
                string uri = engine.GetAssetUri(trackid, i);

                if (loadAsync)
                    tasks.Add(Task.Run(() => SetAssetData(engine, trackid, id, uri, type, projectPath, streaming: streaming)));
                else
                    SetAssetData(engine, trackid, id, uri, type, projectPath, loadAsync: false, streaming: streaming);
            }

            await Task.WhenAll(tasks);
        }

        private static void SetAssetData(Engine engine, int trackid, string assetID, string uri, string type, string path, bool loadFromRemote = false, bool loadAsync = true, bool streaming = false)
        {
            path = path + "/" + uri;

            if (System.IO.File.Exists(uri)) path = uri;

            if (Application.platform == RuntimePlatform.Android)
                loadFromRemote = true;

            if (!loadFromRemote)
            {
                if (streaming)
                {                                    
                    engine.SetAssetPath(trackid, assetID, type);
                    return;
                }
                else
                {
                    byte[] data = File.ReadAllBytes(path);
                    if (data != null)
                    {
                        engine.SetAssetData(trackid, assetID, type, data, null);
                    }
                    data = null;
                }
            }
            else
            {
                if (Application.platform != RuntimePlatform.Android)
                {
                    path = "file://" + path;
                }
                path = path.Replace("#", "%23");

                ReactionalEngine.Instance.StartCoroutine(AsyncWebLoader(path, type, assetID, engine, trackid));
            }
        }

        private static IEnumerator AsyncWebLoader(string path, string type, string assetID, Engine engine, int trackid)
        {
            using (UnityWebRequest www = UnityWebRequest.Get(path))
            {
                var operation = www.SendWebRequest();
                yield return operation;

                bool requestValid = true;
                
                if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.Log(www.error);                    
                    requestValid = false;
                }

                if (requestValid)
                {
                    byte[] data = www.downloadHandler.data;
                    if (data != null)
                    {
                        engine.SetAssetData(trackid, assetID, type, data, null);
                    }
                    data = null;
                }
            }
        }
    }
    /// <summary>
    /// Members of TrackInfo
    /// <list type="bullet">
    /// <item> public string name;</item>
    /// <item> public int trackID;</item>
    /// <item> public string trackHash;</item>
    /// <item> public string trackName;</item>
    /// <item> public string trackArtist;</item>
    /// <item> public string trackAlbum;</item>
    /// <item> public string trackGenre;</item>
    /// <item> public string trackCover;</item>
    /// <item> public string trackBPM;</item>
    /// <item> public string bundleID;</item>
    /// </list>
    /// </summary>
    [Serializable]
    public class TrackInfo
    {
        public string name;
        public int trackID;
        [HideInInspector] public string trackHash;
        [HideInInspector] public string trackName;
        public string trackArtist;
        [HideInInspector] public string trackAlbum;
        [HideInInspector] public string trackGenre;
        public string trackCover;
        [HideInInspector] public string trackBPM;
        [HideInInspector] public string bundleID;

    }
    /// <summary>
    /// Members of Bundle
    /// <list type="bullet">
    /// <item> public string name;</item>
    /// <item> public string path;</item>
    /// <item> public List&lt;Section&gt; sections;</item>
    /// </list>
    /// </summary>
    [Serializable]
    public class Bundle
    {
        [HideInInspector] public string name;
        [HideInInspector] public string path;
        public List<Section> sections = new List<Section>();
    }
    /// <summary>
    /// Members of Section
    /// <list type="bullet">
    /// <item> public string name;</item>
    /// <item> public List&lt;TrackInfo&gt; themes;</item>
    /// <item> public List&lt;Playlist&gt; playlists;</item>
    /// </list>
    /// </summary>
    [Serializable]
    public class Section
    {
        [HideInInspector] public string name;
        public List<TrackInfo> themes = new List<TrackInfo>();
        public List<Playlist> playlists = new List<Playlist>();
    }
    /// <summary>
    /// Members of PlayList
    /// <list type="bullet">
    /// <item> public string name;</item>
    /// <item> public List&lt;TrackInfo&gt; tracks;</item>
    /// </list>
    /// </summary>
    [Serializable]
    public class Playlist
    {
        [HideInInspector] public string name;
        public List<TrackInfo> tracks = new List<TrackInfo>();
    }

}