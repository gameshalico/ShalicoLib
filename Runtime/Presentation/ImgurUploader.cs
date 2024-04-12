using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace ShalicoLib.Presentation
{
    [Serializable]
    public struct ImgurResponse
    {
        public bool success;
        public ImgurData data;

        [Serializable]
        public struct ImgurData
        {
            public string link;
        }
    }

    public class ImgurUploader : MonoBehaviour
    {
        [SerializeField] private string _clientId;
        [SerializeField] private string _clientSecret;

        public async UniTask<string> UploadImage(Texture2D image)
        {
            var imageBytes = image.EncodeToPNG();
            var base64Image = Convert.ToBase64String(imageBytes);

            var form = new WWWForm();
            form.AddField("image", base64Image);

            using var request = UnityWebRequest.Post("https://api.imgur.com/3/image", form);
            request.SetRequestHeader("AUTHORIZATION", $"Client-ID {_clientId}");

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
                throw new Exception(request.error);
            }

            var response = JsonUtility.FromJson<ImgurResponse>(request.downloadHandler.text);

            if (!response.success)
            {
                Debug.LogError("Failed to upload image to imgur");
                throw new Exception("Failed to upload image to imgur");
            }

            return response.data.link;
        }
    }
}