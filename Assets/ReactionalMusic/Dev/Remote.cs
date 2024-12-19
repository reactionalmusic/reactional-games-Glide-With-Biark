using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Reactional.Core;
using UnityEngine;
using UnityEngine.Networking;

public class BundleRequest
{
    public string[] themes;
    public string[] tracks;
}
public class Remote
{
    //public const string BaseUrl = "https://services-dee2ezbepq-ew.a.run.app"; // <- test platform
    //public const string ContentUrl = "https://rm-content-service-emjnmryoya-ew.a.run.app"; // <- test platform
    //public const string BaseUrl = "https://services-vvg4il4tua-ew.a.run.app"; // <- production platform
    public const string ContentUrl = "https://rm-content-service-4yxjwdijga-ew.a.run.app"; // <- production platform
    private const string apiKey = "AIzaSyCp3i_dFg0hSctcYiWluPhVdJrhESbBez0";

    private const string BaseUrl = "https://rm-content-service-4yxjwdijga-ew.a.run.app"; // Replace with your API base URL


    public static async Task<string> GetIdToken(string accessToken)
    {
        string url = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithCustomToken?key={apiKey}";

        // Create the request payload
        string jsonPayload = $"{{\"token\": \"{accessToken}\", \"returnSecureToken\": true}}";
        //Debug.Log(jsonPayload);
        // Create the web request
        using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            //Debug.Log("Auth Token");
            // Send the web request and wait for response
            var operation = www.SendWebRequest();
            while (!operation.isDone)
            {
                await Task.Yield();
            }

            // Check for errors
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning("Reactional: Please log in");
                return "logged-out";
            }
            else
            {
                FirebaseTokenResponse tokenResponse = JsonUtility.FromJson<FirebaseTokenResponse>(www.downloadHandler.text);
                //Debug.Log("Reactional: Already logged in");
                // TODO: Parse and use the ID token from the response
                //Debug.Log(www.downloadHandler.text);
                // Save the refresh token in PlayerPrefs
                PlayerPrefs.SetString(GamerConnect.RefreshTokenPlayerPrefsKey, tokenResponse.refreshToken);
                PlayerPrefs.Save(); // Save the changes

                //Debug.Log($"Refresh Token: {tokenResponse.refreshToken}");
                return tokenResponse.idToken;
            }
        }


    }

    public static async Task<string> ExchangeRefreshToken(string refreshToken)
    {

        string baseUrl = "https://securetoken.googleapis.com/v1/token";
        string apiKey = "AIzaSyCp3i_dFg0hSctcYiWluPhVdJrhESbBez0";

        string url = baseUrl + $"?key={apiKey}";

        using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
        {
            // Include the grant_type parameter
            string jsonBody = $"grant_type=refresh_token&refresh_token={refreshToken}";
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded"); // Change content type

            var operation = www.SendWebRequest();

            while (!operation.isDone)
            {
                await Task.Yield();
            }

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"ExchangeRefreshToken Error: {www.error}");
                return null;
            }
            else
            {
                string responseJson = www.downloadHandler.text;
                Debug.Log($"ExchangeRefreshToken Response JSON: {responseJson}");
                var response = JsonUtility.FromJson<RefreshTokenResponse>(responseJson);
                return response.id_token;
            }
        }

    }

    public static async Task<string> GetUserId(string idToken)
    {
        string url = $"https://identitytoolkit.googleapis.com/v1/accounts:lookup?key={apiKey}";

        // Create the request payload
        string jsonPayload = $"{{\"idToken\": \"{idToken}\"}}";

        // Create the web request
        using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            // Send the web request and wait for response
            var operation = www.SendWebRequest();
            while (!operation.isDone)
            {
                await Task.Yield();
            }

            // Check for errors
            if (www.result != UnityWebRequest.Result.Success)
            {
                //Debug.LogWarning("Reactional: Please log in");
                return "logged-out ";
            }
            else
            {
                var response = MiniJSON.Json.Deserialize(www.downloadHandler.text);
                Dictionary<string, object> dict = response as Dictionary<string, object>;
                List<object> users = dict["users"] as List<object>;
                Dictionary<string, object> user = users[0] as Dictionary<string, object>;
                string userId = user["localId"] as string;

                return userId;


            }
        }
    }

    public async Task<string> GetManifest(string[] themes, string[] tracks, string authToken)
    {
        string path = BaseUrl + "/bundles/get-manifest";

        var requestData = new BundleRequest
        {
            themes = themes,
            tracks = tracks
        };

        string jsonBody = JsonUtility.ToJson(requestData);

        Dictionary<string, string> headers = new Dictionary<string, string>
    {
        { "Authorization", "Bearer " + authToken },
        { "Content-Type", "application/json" }
    };

        UnityWebRequest www = new UnityWebRequest(path, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();

        foreach (var header in headers)
        {
            www.SetRequestHeader(header.Key, header.Value);
        }

        var operation = www.SendWebRequest();

        while (!operation.isDone)
        {
            await Task.Yield();
        }

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Request failed. Error: " + www.error);
            return null;
        }
        else
        {
            string json = www.downloadHandler.text;
            var manifest = MiniJSON.Json.Deserialize(json) as Dictionary<string, object>;
            manifest = manifest["manifest"] as Dictionary<string, object>;
            string jsonString = MiniJSON.Json.Serialize(manifest);
            //Debug.Log(jsonString);
            string directory = Application.streamingAssetsPath;
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer )
            {
                directory = Application.persistentDataPath;
            }

            string saveDirectory = Path.Combine(directory, "Reactional", $"Track_{tracks[0]}");
            string fileName = "manifest.json";

            if (!Directory.Exists(saveDirectory))
            {
                Directory.CreateDirectory(saveDirectory);
            }

            string savePath = Path.Combine(saveDirectory, fileName);
            File.WriteAllText(savePath, jsonString);
            Debug.Log("Manifest JSON saved to: " + savePath);

            return jsonString;
        }
    }

    private void SaveResponseToFile(string response)
    {
        try
        {
            string jsonData = JsonUtility.ToJson(response);
            string savePath = Path.Combine(Application.persistentDataPath, "bundle_manifest.json");
            File.WriteAllText(savePath, jsonData);
            Debug.Log("Bundle manifest saved to: " + savePath);
        }
        catch (Exception e)
        {
            Debug.LogError("Error saving bundle manifest: " + e.Message);
        }
    }

    public static async Task DownloadMissingAssets(string projectId, string idToken)
    {
        string directory = Application.streamingAssetsPath;
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            directory = Application.persistentDataPath;
        }

        List<string> tracksList = new List<string>();
        List<string> themesList = new List<string>();
        Dictionary<string, object> data = null;

        try
        {
            // Read and parse the local manifest file
            string localManifestPath = directory + "/Reactional/" + projectId + "/manifest.json";
            string file = System.IO.File.ReadAllText(localManifestPath);
            var manifest = MiniJSON.Json.Deserialize(file) as Dictionary<string, object>;

            var sections = manifest["sections"] as Dictionary<string, object>;
            foreach (var section in sections.Keys)
            {
                var sectionDict = sections[section] as Dictionary<string, object>;
                var playlists = sectionDict["playlists"] as List<object>;
                foreach (var p in playlists)
                {
                    var playlistDict = p as Dictionary<string, object>;
                    var tracks = playlistDict["tracks"] as List<object>;
                    foreach (var track in tracks)
                    {
                        string trackId = track as string;
                        tracksList.Add(trackId);
                    }
                }

                var themesObjectList = sectionDict["themes"] as List<object>;
                if (themesObjectList != null)
                {
                    foreach (var themeObject in themesObjectList)
                    {
                        string theme = themeObject as string;
                        if (theme != null)
                        {
                            themesList.Add(theme);
                        }
                    }
                }
            }

            data = manifest["data"] as Dictionary<string, object>;
        }
        catch (Exception e)
        {
            Debug.LogError("Error reading and parsing manifest file: " + e.Message);
            return;
        }

        // Download tracks
        foreach (string trackId in tracksList)
        {
            try
            {
                string path = directory + "/Reactional/" + projectId + "/" + trackId;
                if (!System.IO.File.Exists(path))
                {
                    byte[] metadata = await GetTrackReactionalMetadataBytes(trackId, idToken, isHash: true, projectId: projectId);
                    if (metadata != null)
                        System.IO.File.WriteAllBytes(path, StripHash(metadata));

                    byte[] assetData = await GetAsset(trackId, idToken: idToken, isHash: true);
                    string hashString = GetHash(assetData);
                    path = directory + "/Reactional/" + projectId + "/" + hashString;
                    if (assetData != null)
                        System.IO.File.WriteAllBytes(path, StripHash(assetData));
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error downloading track asset: " + trackId + ", Error: " + e.Message);
            }
        }

        // Download themes
        foreach (string themeId in themesList)
        {
            try
            {
                string path = directory + "/Reactional/" + projectId + "/" + themeId;
                if (!System.IO.File.Exists(path))
                {
                    byte[] metadata = await GetTrackReactionalMetadataBytes(themeId, idToken, isTheme: true, isHash: true, projectId: projectId);
                    if (metadata != null)
                        System.IO.File.WriteAllBytes(path, StripHash(metadata));
                }

                List<string> assets = data[themeId] as List<string>;
                string themeIdReversed = new string(themeId.ToCharArray());
                System.Array.Reverse(themeIdReversed.ToCharArray());
                foreach (string asset in assets)
                {
                    if (asset == themeIdReversed)
                        continue;

                    path = directory + "/Reactional/" + projectId + "/" + asset;
                    if (System.IO.File.Exists(path))
                        continue;

                    byte[] assetData = await GetAsset(themeId, assetId: asset, idToken: idToken, isTheme: true, isHash: true);
                    if (assetData != null)
                        System.IO.File.WriteAllBytes(path, StripHash(assetData));
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error downloading theme asset: " + themeId + ", Error: " + e.Message);
            }
        }
    }


    public static async Task<byte[]> GetTrackReactionalMetadataBytes(string trackId, string authToken, string target = "unlocked", string version = "latest", string url = "", bool isTheme = false, bool isHash = false, string projectId = "")
    {
        string extension = isTheme ? "themes" : "tracks";
        string hashAppend = isHash ? $"&hash=true&projectId={projectId}" : "";
        string path = $"{ContentUrl}/{extension}/{trackId}/reactional-metadata?version={version}&target={target}{hashAppend}";
        if (url != "")
        {
            path = $"{url}/{extension}/{trackId}/reactional-metadata?version={version}&target={target}";
        }

        using (UnityWebRequest www = UnityWebRequest.Get(path))
        {
            www.SetRequestHeader("Authorization", "Bearer " + authToken);
            www.SetRequestHeader("Content-Type", "application/json");


            var operation = www.SendWebRequest();
            while (!operation.isDone)
            {
                await Task.Yield();
            }

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning("Reactional: Could not get metadata; are you logged in?");
                Debug.Log(path);
                Debug.Log(www.error);
                return null;
            }
            else
            {
                byte[] metadata = www.downloadHandler.data;

                //string jsonString = Reactional.Core.AssetManager.ValidateRemoteTrack(metadata);

                return metadata;
            }
        }
    }

    public static async Task<byte[]> GetAsset(string trackId, string assetId = "", string idToken = null, bool isTheme = false, bool isHash = false)
    {
        string hashQuery = "";
        if (isHash)
            hashQuery = "?hash=true";

        string path = $"{ContentUrl}/tracks/{trackId}/audio{hashQuery}";
        if (isTheme)
            path = $"{ContentUrl}/themes/{trackId}/assets/{assetId}{hashQuery}";
        Debug.Log(path);
        List<byte[]> bytes = new List<byte[]>();
        int chunk = 0;
        string requestId = null;
        bool done = false;
        while (true)
        {
            string joinChar = isHash ? "&" : "?";

            string requestUrl = $"{path}{joinChar}chunk={chunk}";
            if (requestId != null)
            {
                requestUrl += $"&req={requestId}";
            }

            using (UnityWebRequest www = UnityWebRequest.Get(requestUrl))
            {
                www.SetRequestHeader("Authorization", "Bearer " + idToken);
                www.SetRequestHeader("Content-Type", "application/json");
                var operation = www.SendWebRequest();

                while (!operation.isDone)
                {
                    await Task.Yield();
                }

                if (www.result != UnityWebRequest.Result.Success && www.result != UnityWebRequest.Result.InProgress)
                {
                    Debug.LogError(www.error);
                    return null;
                }
                else
                {

                    byte[] data = www.downloadHandler.data;
                    //Debug.Log("data length: " + data.Length);
                    if (www.responseCode == 206)
                    {
                        const int REQUEST_ID_SIZE = 36;
                        requestId = System.Text.Encoding.UTF8.GetString(data, 0, REQUEST_ID_SIZE);
                        byte[] actualData = new byte[data.Length - REQUEST_ID_SIZE];
                        System.Buffer.BlockCopy(data, REQUEST_ID_SIZE, actualData, 0, data.Length - REQUEST_ID_SIZE);
                        bytes.Add(actualData);
                    }
                    else
                    {
                        bytes.Add(data);
                        done = true;
                    }

                    chunk++;
                }
            }

            if (done)
                break;
        }

        if (bytes.Count == 1)
        {
            //Debug.Log(bytes[0].Length);
            return bytes[0];
        }
        else
        {
            //Debug.Log("Combined");
            return Combine(bytes);

        }
    }

    public static string GetHash(byte[] bytes)
    {
        string hashString = "";
        int hashStartIndex = bytes.Length - (64 + 256);
        byte[] hashBytes = new byte[64];
        System.Buffer.BlockCopy(bytes, hashStartIndex, hashBytes, 0, 64);
        hashString = System.Text.Encoding.UTF8.GetString(hashBytes);
        return hashString;
    }
    public static byte[] StripHash(byte[] bytes)
    {
        int hashStartIndex = bytes.Length - (64 + 256);
        int newSize = bytes.Length - 64; // New size after removing 64 bytes

        byte[] result = new byte[newSize];

        // Copy the first part of the original array
        System.Array.Copy(bytes, 0, result, 0, hashStartIndex);

        // Copy the second part of the original array, skipping the 64 bytes
        System.Array.Copy(bytes, hashStartIndex + 64, result, hashStartIndex, newSize - hashStartIndex);

        return result;
    }

    private static byte[] Combine(List<byte[]> bytes)
    {
        int size = 0;
        for (int i = 0; i < bytes.Count; i++)
        {
            size += bytes[i].Length;
        }
        byte[] ret = new byte[size];
        int offset = 0;
        foreach (byte[] data in bytes)
        {
            System.Buffer.BlockCopy(data, 0, ret, offset, data.Length);
            offset += data.Length;
        }
        return ret;
    }

}
   


[System.Serializable]
public class FirebaseTokenResponse
{
    public string kind;
    public string idToken;
    public string refreshToken;
    public string expiresIn;
    public bool isNewUser;
}
