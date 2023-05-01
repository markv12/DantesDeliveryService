using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public static class NetUtility {
    private const bool useLocalhost = false;

    public static IEnumerator Get(string uri, Action<string> onComplete, Action onError = null, bool autoRetry = true) {

        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri)) {
            yield return webRequest.SendWebRequest();
            switch (webRequest.result) {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(uri + ": HTTP Error: " + webRequest.error);
                    if (autoRetry) {
                        yield return Get(uri, onComplete, onError, false);
                    } else {
                        onError?.Invoke();
                    }
                    break;
                case UnityWebRequest.Result.Success:
                    onComplete?.Invoke(webRequest.downloadHandler.text);
                    break;
            }
        }
    }

    public static IEnumerator PostPNGData(string uri, byte[] pngData, Action<string> onComplete, bool autoRetry = true) {

        using (UnityWebRequest webRequest = new UnityWebRequest(uri + UnityWebRequest.kHttpVerbPOST)) {
            UploadHandlerRaw UploadHandler = new UploadHandlerRaw(pngData);
            UploadHandler.contentType = "application/x-www-form-urlencoded";
            webRequest.uploadHandler = UploadHandler;
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            yield return webRequest.SendWebRequest();

            switch (webRequest.result) {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(uri + ": Error: " + webRequest.error);
                    if (autoRetry) yield return PostPNGData(uri, pngData, onComplete, false);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(uri + ": HTTP Error: " + webRequest.error);
                    if (autoRetry) yield return PostPNGData(uri, pngData, onComplete, false);
                    break;
                case UnityWebRequest.Result.Success:
                    onComplete?.Invoke(webRequest.downloadHandler.text);
                    break;
            }
        }
    }

    public static IEnumerator Post(string uri, ValueTuple<string, string>[] bodyParams, Action<bool, string> onComplete, bool autoRetry = true) {

        WWWForm form = new WWWForm();
        for (int i = 0; i < bodyParams.Length; i++) {
            ValueTuple<string, string> param = bodyParams[i];
            form.AddField(param.Item1, param.Item2);
        }
        using (UnityWebRequest webRequest = UnityWebRequest.Post(uri, form)) {
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            yield return webRequest.SendWebRequest();

            switch (webRequest.result) {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(uri + ": Error: " + webRequest.error);
                    if (autoRetry) yield return Post(uri, bodyParams, onComplete, false);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(uri + ": HTTP Error: " + webRequest.error);
                    if (webRequest.responseCode == 400) {
                        onComplete?.Invoke(false, webRequest.downloadHandler.text);
                    } else {
                        if (autoRetry) yield return Post(uri, bodyParams, onComplete, false);
                    }
                    break;
                case UnityWebRequest.Result.Success:
                    onComplete?.Invoke(true, webRequest.downloadHandler.text);
                    break;
            }
        }
    }
}
