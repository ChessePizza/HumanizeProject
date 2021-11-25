using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.IO;
using System;

public class ScreenShot : MonoBehaviour
{
    //Set Screenshot Resolution
    public int captureWidth = 1920;
    public int captureHeight = 1080;
    // Configure with raw, jpg, png or ppm
    public enum Format { RAW, JPG, PNG, PPM };
    public Format format = Format.JPG;

    //Folder to write output
    private string outputFolder;

    //private variables needed for screenshot
    private Rect rect;
    private RenderTexture renderTexture;
    private Texture2D screenShot;

    public bool isProcessing;

    private byte[] currentTexture;

    public Image showImage;
    public UnityEvent OnShowImage;

    void Start()
    {
        outputFolder = Application.persistentDataPath + "/Screenshot/";
        if (!Directory.Exists(outputFolder))
        {
            Directory.CreateDirectory(outputFolder);
            Debug.Log("Save Path will be : " + outputFolder);
        }
    }

    private string CreateFileName(int width, int height) 
    {
        //timestamp to append to the screenshot filename
        string timestamp = DateTime.Now.ToString("yyyyMMddTHHmmss");
        //use width, height, and timestamp for unique file
        var filename = string.Format("{0}/screen_{1}x{2}_{3}.{4}", outputFolder, width, height, timestamp, format.ToString().ToLower());
        //return filename
        return filename;
    }

    private void CaptureScreenShot() 
    {
        isProcessing = true;
        if (renderTexture == null)
        {
            //create off - screen render texture to be rendered into
            rect = new Rect(0, 0, captureWidth, captureHeight);
            renderTexture = new RenderTexture(captureWidth, captureHeight, 24);
            screenShot = new Texture2D(captureWidth, captureHeight, TextureFormat.RGB24, false);
        }

        //get main camera and render its output into the off - screen render texture created above
        Camera camera = Camera.main;
        camera.targetTexture = renderTexture;
        camera.Render();

        //mark the render texturn as active and read the current pixel data into the Texture2D
        RenderTexture.active = renderTexture;
        screenShot.ReadPixels(rect, 0, 0);

        //reset the textures and remove the render trxturn from Camera since were done reading the screen
        camera.targetTexture = null;
        RenderTexture.active = null;

        //get our filename
        string filename = CreateFileName((int)rect.width, (int)rect.height);

        //get file header/data bytes for the specified image format
        byte[] fileHeader = null;
        byte[] fileData = null;

        //set the format and encode based on it

        switch (format)
        {
            case Format.RAW:
                fileData = screenShot.GetRawTextureData();
                break;
            case Format.JPG:
                fileData = screenShot.EncodeToJPG();
                break;
            case Format.PNG:
                fileData = screenShot.EncodeToPNG();
                break;
            case Format.PPM: //for ppm file
                // create a file header - ppm files
                string headerStr = string.Format("P6\n{0} {1}\n255\n", rect.width, rect.height);
                fileHeader = System.Text.Encoding.ASCII.GetBytes(headerStr);
                fileData = screenShot.GetRawTextureData();
                break;
        }
        currentTexture = fileData;

        //create new thread to offload the saving from the main thread
        new System.Threading.Thread(() =>
        {
            var file = System.IO.File.Create(filename);
            if (fileHeader != null)
            {
                file.Write(fileHeader, 0, fileHeader.Length);
            }
            file.Write(fileData, 0, fileData.Length);
            file.Close();
            Debug.Log(string.Format("Screenshot Saved {0}, size {1}", filename, fileData.Length));
            isProcessing = false;
        }
        ).Start();

        StartCoroutine(ShowImage());

        //CleanUp
        Destroy(renderTexture);
        renderTexture = null;
        screenShot = null;
    }

    public void TakeScreenShot()
    {
        if (!isProcessing)
        {
            CaptureScreenShot();
        }
        else
        {
            Debug.Log("Currently Processing");
        }
    }

    public IEnumerator ShowImage() 
    {
        yield return new WaitForEndOfFrame();

        showImage.material.mainTexture = null;
        Texture2D texture = new Texture2D(captureWidth, captureHeight, TextureFormat.RGB24, false);
        texture.LoadImage(currentTexture);
        showImage.material.mainTexture = texture;
        //showImage.gameObject.SetActive(true);
        
        OnShowImage?.Invoke();
    }
    
}