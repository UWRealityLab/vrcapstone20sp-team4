using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using MagicLeapTools;


public class ioTesting : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject io;
    void Start()
    {
        Texture2D tex = loadImage(@"../Chef's Table/Assets/Resources/Image/tutorial1_image");
        io.GetComponent<RawImage>().texture = tex;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    Texture2D loadImage(string pathToImage)
    {
        Texture2D tex = null;
        byte[] fileData;
        string completePath;
        if (File.Exists(pathToImage + ".png"))
        {
            completePath = pathToImage + ".png";
        }
        else if (File.Exists(pathToImage + ".jpg"))
        {
            completePath = pathToImage + ".jpg";
        }
        else
        {
            Debug.LogError("Image file not found: " + pathToImage + " .jpg/.png");
            return tex;
        }
        fileData = File.ReadAllBytes(completePath);
        tex = new Texture2D(2, 2);
        tex.LoadImage(fileData);
        return tex;
    }
}
