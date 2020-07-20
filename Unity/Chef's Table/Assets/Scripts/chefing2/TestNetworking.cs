using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using TMPro;
using System.Text;
using UnityEngine;
using UnityEngine.XR.MagicLeap.Native;
using UnityEngine.XR.MagicLeap;

public class CaptureFromCamera : MonoBehaviour
{
    // Start is called before the first frame update

    // Update is called once per frame
    public GameObject requestCanvas;
    public GameObject responseCanvas;
    private float countDown = 3;
    private string response = "none";
    private string url = "http://35.188.99.45:5000/hello";
    TextMeshProUGUI reqText;
    TextMeshProUGUI respText;
    
    void Start()
    {
        GameObject reqDisplay = requestCanvas.transform.Find("display").gameObject;
        GameObject respDisplay = responseCanvas.transform.Find("display").gameObject;
        reqText = reqDisplay.GetComponent<TextMeshProUGUI>();
        respText = respDisplay.GetComponent<TextMeshProUGUI>();
        MLResult mr = new MLResult();
        mr = MLCamera.Start();
        MLCamera.CaptureRawImageAsync();
        MLCamera.Stop();
    }
    void Update()
    {
        
        //countDown -= Time.deltaTime;
        //if (countDown < 0)
        //{
        //    countDown = 3;
        //    // byte[] image = GetFileByteArray();
        //    UploadFile(url);

        //}
        //updateCanvas();
    }
    private void updateCanvas()
    {
        reqText.text = "Send request in " + countDown;
        respText.text = response;
    }

    //private byte[] GetFileByteArray()
    //{
    //    Texture2D image = Resources.Load<Texture2D>("Image/kitchen");
    //    return image.EncodeToJPG();
    //}
    private void UploadFile(string uri)
    {
        WebClient myWebClient = new WebClient();
        myWebClient.Headers.Add("Content-Type", "binary/octet-stream");
        myWebClient.Encoding = Encoding.UTF8;
        byte[] array = new byte[] { 1, 2, 3, 4, 5 };
        byte[] responseArray = myWebClient.UploadData(uri, array);  // send a byte array to the resource and returns a byte array containing any response
        response = Encoding.UTF8.GetString(responseArray);

    }
}
