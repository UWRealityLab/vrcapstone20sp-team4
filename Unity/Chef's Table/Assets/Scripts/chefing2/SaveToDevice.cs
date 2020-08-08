using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class SaveToDevice : MonoBehaviour
{
    private string record = "/documents/C2/record.txt";
    private string c2_path = "/documents/C2/";
    private string towrite = "hello";
    private float timer = 5;

    private void Start()
    {
        if (!File.Exists(record))
        {
            string init = "";
            File.WriteAllText(record, init);
        }
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            timer = 5;
            Debug.Log("2" + Directory.Exists("/documents/C2"));
            File.WriteAllText(c2_path + "hello.json", towrite);
            Debug.Log(File.Exists(c2_path + "hello.json"));
            string toprint = File.ReadAllText(c2_path + "hello.json");
            Debug.Log(toprint);
            saveRecipe("food", "requirements", "instructions", new byte[0], new List<byte[]>()); ;
        }

    }

    // name : name of the recipe
    // json : raw json string we get from the web, as long as we can deserialize
    // images: a image for each steps
    public void saveRecipe(string name, string requirments, string instructions, byte[] cover_image, List<byte[]> step_images)
    {
        
        if (Directory.Exists(c2_path + name)) return;
        string folder_path = c2_path + name + "/";
        File.WriteAllText(folder_path + "requirements.txt", requirments);
        File.WriteAllText(folder_path + "instructions.txt", instructions);
        File.WriteAllBytes(folder_path + "cover.jpg", cover_image);
        for (int i = 0; i < step_images.Count; i++)
        {
            File.WriteAllBytes(folder_path + "step" + i + ".jpg", step_images[i]);
        }
        string record_string = File.ReadAllText(record);
        record_string += name + ",";
        File.WriteAllText(record, record_string);
    }

    public List<previewRecipe> getRecipeList()
    {
        string record_string = File.ReadAllText(record);
        List<string> folders = new List<string>();
        string[] words = record_string.Split(' ');
        for (int i = 0; i < words.Length; i++)
        {   
            if (words[i] != "")
            {
                folders.Add(words[i]);
            }
            
        }
        return null;
    }

    public StoredRecipe getSingleRecipe()
    {
        return null;
    }

    public class previewRecipe
    {
        private string name;
        private string requirements;

        public string getName()
        {
            return name;
        }

        public string getRequirements()
        {
            return requirements;
        }
    }
    public class StoredRecipe
    {
        private string name;
        private string requirements;
        private string instuctions;
        private List<byte[]> images;

        public string getName()
        {
            return name;
        }

        public string getRequirements()
        {
            return requirements;
        }

        public string getInstructions()
        {
            return instuctions;
        }

        private List<byte[]> getImageOfStep()
        {
            return images;
        }
    }
}
