using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;



[System.Serializable]
public class AuthRequest
{
    public string userId;
    public string clientType;
}

[System.Serializable]
public class LoginRequest
{
    public string email;
    public string token;
}

[System.Serializable]
public class VerifyLoginRequest
{
    public string email;
    public string token;
    public string code;
    public string mfa;
}
[System.Serializable]
public class VerifyResponse
{
    public bool mfa;
    public string token;
}

[System.Serializable]
public class AuthResponse
{
    public string token;

}


[System.Serializable]
public class RefreshTokenResponse
{
    public string id_token;
    public string expires_in;
    public string error;
}

public class GamerConnect : MonoBehaviour
{

    private const string BaseUrl = "https://rm-connect-service-hjo73j5i7q-ew.a.run.app"; // Replace with your API endpoint
    private string idToken;
    private string userId;
    public const string EmailPlayerPrefsKey = "GamerConnect_Email";
    public const string AuthTokenPlayerPrefsKey = "GamerConnect_AuthToken";
    public const string UserIdPlayerPrefsKey = "GamerConnect_UserId";
    public const string IdTokenPlayerPrefsKey = "GamerConnect_IdToken";
    public const string RefreshTokenPlayerPrefsKey = "GamerConnect_RefreshToken";

    public async void RefreshTokenIfNeeded()
    {
        Debug.Log("Checking if refresh is needed...");

        // Your existing code to get the stored refresh token
        string storedAuthToken = PlayerPrefs.GetString("auth_token");
        string storedRefreshToken = PlayerPrefs.GetString(RefreshTokenPlayerPrefsKey);
        string storedUserId = PlayerPrefs.GetString(UserIdPlayerPrefsKey);

        if (!string.IsNullOrEmpty(storedAuthToken) && !string.IsNullOrEmpty(storedUserId))
        {
            AuthResponse authResponse = new AuthResponse { token = storedAuthToken };
            FirebaseTokenResponse tokenResponse = new FirebaseTokenResponse
            {
                idToken = storedAuthToken,
                refreshToken = storedRefreshToken
            };

            await RefreshIdToken(tokenResponse);

            
        }
    }


    public async Task RefreshIdToken(FirebaseTokenResponse tokenResponse)
    {
        Debug.Log(" Initiating token refresh...");
        if (tokenResponse != null && !string.IsNullOrEmpty(tokenResponse.refreshToken))
        {
            // Use ReactionalPrefs to get the stored refresh token
            string storedRefreshToken = PlayerPrefs.GetString(RefreshTokenPlayerPrefsKey);
            string newIdToken = await Remote.ExchangeRefreshToken(tokenResponse.refreshToken);

            if (!string.IsNullOrEmpty(newIdToken))
            {
                PlayerPrefs.SetString(IdTokenPlayerPrefsKey, newIdToken);
                Debug.Log("ID Token refreshed: " + newIdToken);
               
            }
            else
            {
                Debug.LogWarning("Token refresh failed");
            }
        }
        else
        {
            Debug.LogWarning("No refresh token found");
        }
    }

    public async Task<AuthResponse> RegisterApp(string userId)
    {
        string path = BaseUrl + "/auth/register";
        AuthRequest request = new AuthRequest { userId = userId, clientType = "owclient" };
        string jsonBody = JsonUtility.ToJson(request);

        return await SendPostRequest<AuthResponse>(path, jsonBody);
    }


    public string CreateRequestToken(int length)
    {
        string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        StringBuilder token = new StringBuilder();
        System.Random random = new System.Random();

        for (int i = 0; i < length; i++)
        {
            token.Append(chars[random.Next(chars.Length)]);
        }

        return token.ToString();

    }

    //for sending login request
    public async Task<AuthResponse> LoginRequest(string email)
    {
        string path = BaseUrl + "/auth/login";
        LoginRequest request = new LoginRequest { email = email, token = CreateRequestToken(32) };
        string jsonBody = JsonUtility.ToJson(request);

        PlayerPrefs.SetString("signin_request", jsonBody);

        return await SendPostRequest<AuthResponse>(path, jsonBody);
    }

    //for sending verification code
    public async Task<AuthResponse> VerifyLoginCode(string email, string token, string code, string mfa)
    {
        string path = BaseUrl + "/auth/verify-code";
        string storedRequestJson = PlayerPrefs.GetString("signin_request");

        if (string.IsNullOrEmpty(storedRequestJson))
        {
            throw new InvalidOperationException("Sign in request not found.");
        }

        LoginRequest storedRequest = JsonUtility.FromJson<LoginRequest>(storedRequestJson);
        VerifyLoginRequest request = new VerifyLoginRequest { email = storedRequest.email, token = storedRequest.token, code = code, mfa = mfa };

        string requestJson = JsonUtility.ToJson(request);
        Debug.Log("VerifyLoginCode Request JSON: " + requestJson);

        AuthResponse response = await SendPostRequest<AuthResponse>(path, requestJson);

        if (response != null)
        {
            Debug.Log("VerifyLoginCode Response JSON: " + JsonUtility.ToJson(response));
            Debug.Log("Login Successful");
            PlayerPrefs.SetString("auth_token", response.token);
            string result = await Remote.GetIdToken(response.token);
            idToken = result;
            userId = await Remote.GetUserId(idToken);
            Debug.Log("ID: " + userId + " Token: " + idToken);
            // Print idToken to the console
            Debug.Log("idToken: " + idToken);

            SaveLoginData(email, response.token, userId, idToken);



        }
        else
        {
            Debug.LogError("VerifyLoginCode Response is null or invalid.");
        }

        return response;
    }

    private async Task<T> SendPostRequest<T>(string url, string jsonBody) where T : class
    {
        //Debug.Log("SendPostRequest URL: " + url);
        //Debug.Log("SendPostRequest Request JSON: " + jsonBody);

        using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            var operation = www.SendWebRequest();
            try
            {
                while (!operation.isDone)
                {
                    await Task.Yield();
                }

                if (www.result != UnityWebRequest.Result.Success)
                {
                    //Debug.LogError("SendPostRequest Error: " + www.error);
                    return null;
                }
                else
                {
                    string responseJson = www.downloadHandler.text;
                    //Debug.Log("SendPostRequest Response JSON: " + responseJson);
                    return JsonUtility.FromJson<T>(responseJson);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("SendPostRequest Exception: " + ex.Message);
                return null;
            }
        }
    }



    //for saving login data
    private void SaveLoginData(string email, string authToken, string userId, string idToken)
    {
        PlayerPrefs.SetString(EmailPlayerPrefsKey, email);
        PlayerPrefs.SetString(AuthTokenPlayerPrefsKey, authToken);
        PlayerPrefs.SetString(UserIdPlayerPrefsKey, userId);
        PlayerPrefs.SetString(IdTokenPlayerPrefsKey, idToken);


    }
    public void SaveRefreshToken(string refreshToken)
    {
        // Save the refresh token securely
        PlayerPrefs.SetString(RefreshTokenPlayerPrefsKey, refreshToken);
    }
    //for logging out
    public void SignOut()
    {
        PlayerPrefs.DeleteKey(EmailPlayerPrefsKey);    // Clear the stored email
        PlayerPrefs.DeleteKey(AuthTokenPlayerPrefsKey); // Clear the stored authentication token
        PlayerPrefs.DeleteKey(UserIdPlayerPrefsKey);    // Clear the stored user ID
        PlayerPrefs.DeleteKey(IdTokenPlayerPrefsKey);// Clear the stored ID token
        Debug.Log("User signed out");

    }

    //For loading saved data
    public void LoadLoginData()
    {
        string savedEmail = PlayerPrefs.GetString(EmailPlayerPrefsKey);
        string savedAuthToken = PlayerPrefs.GetString(AuthTokenPlayerPrefsKey);
        string savedUserId = PlayerPrefs.GetString(UserIdPlayerPrefsKey);
        string savedIdToken = PlayerPrefs.GetString(IdTokenPlayerPrefsKey);

        Debug.Log(" Email: " + savedEmail);
        Debug.Log(" Auth Token: " + savedAuthToken);
        Debug.Log(" User ID: " + savedUserId);
        Debug.Log(" ID Token: " + savedIdToken);


    }


}