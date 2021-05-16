using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;  
using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.Net;


public class EpicKitchen : MonoBehaviour
{
    private string epicKitchenURL = "http://oasis.cs.washington.edu:5000/get_images";

    Dictionary<string, List<Sprite>> epicKitchenVideos = new Dictionary<string, List<Sprite>>();
    Dictionary<string, List<byte[]>> byteVideos = new Dictionary<string, List<byte[]>>();

    private List<string> actions = new List<string>(){"cut bell pepper", "spoon avocado"};
    private bool done = false;
    private int playFrame = 0;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            loadAllActionVideos();
        }

    }

    void FixedUpdate() {
        if (done) {

            playFrame++;
            List<Sprite> video = epicKitchenVideos["spoon avocado"];
            SpriteRenderer spriteRenderer = GameObject.Find("VisualCueDisplayContainer/Animation").GetComponent<SpriteRenderer>();
            int temp2 = playFrame % video.Count;
            spriteRenderer.sprite = video[temp2];

        }
    }

    void loadAllActionVideos() {
        
        Task.Run( async () => {
            foreach (string action in actions) {
                if (action != null && action.Length != 0 && !epicKitchenVideos.ContainsKey(action)) {
                    List<byte[]> video = new List<byte[]>();
                    NameValueCollection values = new NameValueCollection();
                    values.Add("action", action);
                    using (WebClient client = new WebClient())
                    {
                        client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                        byte[] result = await client.UploadValuesTaskAsync(epicKitchenURL, "POST", values);
                        string ResultAuthTicket = System.Text.Encoding.UTF8.GetString(result);
                        Response res = JsonUtility.FromJson<Response>(ResultAuthTicket);
                        if (res.pic.Count > 0) {
                            foreach (string frame in res.pic) {
                                byte[] imageBytes = System.Convert.FromBase64String(frame);
                                video.Add(imageBytes);
                            }
                            byteVideos.Add(action, video);
                            Debug.Log(action + " loaded");
                        }
                    }
                }
            }
            done = true;
        });
        StartCoroutine(toSprites());
    }

    IEnumerator toSprites() {
        while(!done) {
            yield return null;
        }
        foreach (string action in byteVideos.Keys) {
            List<Sprite> videoSprite = new List<Sprite>();
            foreach (byte[] frame in byteVideos[action]) {
                Texture2D texture = new Texture2D(456, 256);
                texture.LoadImage(frame);
                Sprite sprite = Sprite.Create(texture, new Rect(0,0,texture.width, texture.height), new Vector2(0.5f, 0.5f));
                videoSprite.Add(sprite);
                yield return null;
            }
            epicKitchenVideos[action] = videoSprite;
        }
        Debug.Log("Sprite conversion done"); 
    }

    [Serializable]
    private class Response {
        public List<string> pic;
    }
}
