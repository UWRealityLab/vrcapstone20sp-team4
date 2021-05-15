using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;  
using UnityEngine;
using System.Net;
using System;

public class Epickitchen : MonoBehaviour
{
    // Start is called before the first frame update
    private SpriteRenderer spriteRenderer; 
    private string url = "http://oasis.cs.washington.edu:5000/get_images";
    void Start()
    {
        spriteRenderer = GameObject.Find("VisualCueDisplayContainer/Animation").GetComponent<SpriteRenderer>();
        getImages();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void getImages() {
        // NameValueCollection values = new NameValueCollection();
        // values.Add("action", "take plate");

        // using (WebClient client = new WebClient())
        // {
        //     client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
        //     byte[] result = client.UploadValues(url, "POST", values);
        //     string ResultAuthTicket = System.Text.Encoding.UTF8.GetString(result);
        //     Response res = JsonUtility.FromJson<Response>(ResultAuthTicket);
            
        //     string inputString = res.pic[0];
        //     Debug.Log(inputString);
        //     // byte[] imageBytes = System.Text.Encoding.UTF8.GetBytes(inputString);
        //     byte[] imageBytes = System.Convert.FromBase64String(inputString);
        //     Debug.Log(imageBytes.Length);
        //     Texture2D texture = new Texture2D(456, 256);
        //     texture.LoadImage(imageBytes);
        //     spriteRenderer.sprite = Sprite.Create(texture, new Rect(0,0,texture.width, texture.height), new Vector2(0.5f, 0.5f));
        // }


    }


}
