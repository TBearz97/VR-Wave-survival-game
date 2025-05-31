using UnityEngine;
using System.IO;
using System.Collections;

public class ScreenshotSaver : MonoBehaviour
{
    public RenderTexture renderTexture;

    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();

        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = renderTexture;

        Texture2D tex = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
        tex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        tex.Apply();

        byte[] bytes = tex.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/ControllerSprite.png", bytes);

        RenderTexture.active = currentRT;

        Debug.Log("Saved image to " + Application.dataPath + "/ControllerSprite.png");
    }
}
