using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

public class NetworkingMain : MonoBehaviour
{
    // Start is called before the first frame update
    public Texture2D image;
    private string filename = "\\Users\\wayne\\Desktop\\NetworkingTest\\Assets\\Resources\\kitchen.jpg";
    private string url = "http://35.225.232.30:5000/predict";
    private string currResponse = "";

    private int W;
    private int H;
    private Raycast rc;

    private void Start()
    {
        rc = GameObject.Find("RaycastMain").GetComponent<Raycast>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            byte[] encoded_img = GetFileByteArray(filename);
            UploadFile(url, encoded_img);
        }
        if (!currResponse.Equals(""))
        {
            currResponse = "{\"detections\": " + currResponse + " }";
            DetectionList DL = JsonUtility.FromJson<DetectionList>(currResponse);
            Dictionary<string, Vector2> rays = new Dictionary<string, Vector2>();
            foreach(var detection in DL.detections)
            {
                int x, y, width, height;
                x = detection.boxes[0];
                y = detection.boxes[1];
                width = detection.boxes[2];
                height = detection.boxes[3];
                Vector2 center = getNormalizedCenter(x, y, width, height, W, H);
                rays[detection.label] = center;
            }
            rc.makeRayCast(rays, true);
        }
    }

    private void getImageSize(byte[] image)
    {
        Texture2D temp = new Texture2D(2, 2);
        temp.LoadImage(image);
        W = temp.width;
        H = temp.height;
        Debug.Log(W + " " + H);
    }
    private byte[] GetFileByteArray(string filename)
    {
        FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
        byte[] fileByteArrayData = new byte[fs.Length];
        fs.Read(fileByteArrayData, 0, System.Convert.ToInt32(fs.Length));
        fs.Close();
        getImageSize(fileByteArrayData);
        return fileByteArrayData;
    }

    private async void UploadFile(string uri, byte[] rawImage)
    {
        WebClient myWebClient = new WebClient();
        myWebClient.Headers.Add("Content-Type", "binary/octet-stream");
        myWebClient.Encoding = Encoding.UTF8;
        byte[] responseArray = await myWebClient.UploadDataTaskAsync(uri, rawImage); ;  // send a byte array to the resource and returns a byte array containing any response
        currResponse = Encoding.UTF8.GetString(responseArray);
    }
    
    private Vector2 getNormalizedCenter(int x, int y, int width, int height, int W, int H)
    {
        return new Vector2((x + width / 2f) / W, (y + height / 2f) / H);
    }

    [Serializable]
    public class DetectionList
    {
        public List<Detection> detections;
    }

    [Serializable]
    public class Detection
    {
        public List<int> boxes;
        public List<int> color;
        public float confidence;
        public string label;
    }
}
