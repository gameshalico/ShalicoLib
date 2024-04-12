using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ShalicoLib.Utility
{
    public static class ScreenShot
    {
        public static async UniTask<Texture2D> CaptureScreenshot()
        {
            await UniTask.WaitForEndOfFrame();

            return ScreenCapture.CaptureScreenshotAsTexture();
        }

        public static Texture2D CaptureCamera(Camera target)
        {
            var renderTexture = new RenderTexture(target.pixelWidth, target.pixelHeight, 24);

            var prevTargetTexture = target.targetTexture;
            target.targetTexture = renderTexture;
            target.Render();
            target.targetTexture = prevTargetTexture;

            var screenShot = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);

            var prevActive = RenderTexture.active;
            RenderTexture.active = renderTexture;
            screenShot.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            screenShot.Apply();
            RenderTexture.active = prevActive;

            return screenShot;
        }
    }
}