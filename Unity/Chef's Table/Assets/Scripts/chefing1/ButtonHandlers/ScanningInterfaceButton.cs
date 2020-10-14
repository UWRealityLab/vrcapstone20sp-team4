using UnityEngine;
using UnityEngine.UI;
using System.Linq;


public class ScanningInterfaceButton : MonoBehaviour
{
    AudioSource buttonClip;
    InterfaceManager interfaceManager;
    DetectionPipeline pipeline;
    GameObject getRecipeButton;

    GameObject ScanScreen;
    GameObject startScanning;
    GameObject pauseScanning;
    GameObject PlaceIngredientsText;
    GameObject exitButton;
    GameObject keyBoardSwitch;
    GameObject VirtualKeyBoard;

    private string ingredientListString = "";

    private GetInstructions recipeApi;

    private ScanningInterfaceController controller;

    void Awake()
    {
        pipeline = GameObject.Find("pipeline").GetComponent<DetectionPipeline>();
        ScanScreen = GameObject.Find("ScanScreen");
        PlaceIngredientsText = ScanScreen.transform.Find("PlaceIngredientsText").gameObject;
        getRecipeButton = GameObject.Find("GetRecipes");
        startScanning = GameObject.Find("StartScanning");
        pauseScanning = GameObject.Find("PauseScanning");
        exitButton = GameObject.Find("Exit");
        keyBoardSwitch = GameObject.Find("KeyboardSwitch");
        VirtualKeyBoard = GameObject.Find("VirtualHandKeyboard");
        interfaceManager = GameObject.Find("InterfaceManager").GetComponent<InterfaceManager>();
        buttonClip = GameObject.Find("Button_Click").GetComponent<AudioSource>();
        recipeApi = GameObject.Find("RecipeAPI").GetComponent<GetInstructions>();
        controller = GameObject.Find("ScanningContainer").GetComponent<ScanningInterfaceController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Hand" && interfaceManager.clickOk())
        {
            gameObject.GetComponent<Button>().onClick.Invoke();
            interfaceManager.clickButton();
        }
    }

    public void clicked()
    {
        if (name == "TrashButtonScript")
        {
            string whichIngredient = this.transform.parent.parent.gameObject.name;
            Debug.Log(whichIngredient);
            int index = whichIngredient.Last() - '0';
            controller.removeIngredient(index);
            AudioSource.PlayClipAtPoint(buttonClip.clip, this.transform.parent.position);
        }
        else if (name == "StartScanningButton")
        {
            AudioSource.PlayClipAtPoint(buttonClip.clip, startScanning.transform.position);
            startScanning.SetActive(false);
            pauseScanning.SetActive(true);
            pipeline.startPipeline(true);
            PlaceIngredientsText.SetActive(false);
        }
        else if (name == "PauseScanningButton") {
            AudioSource.PlayClipAtPoint(buttonClip.clip, pauseScanning.transform.position);
            startScanning.SetActive(true);
            pauseScanning.SetActive(false);
            pipeline.stopPipeline();
            PlaceIngredientsText.SetActive(true);
        }
        else if (name == "GetRecipesButton")
        {
            AudioSource.PlayClipAtPoint(buttonClip.clip, getRecipeButton.transform.position);
            ingredientListString = string.Join(",", controller.array());
            recipeApi.GetIngredientsList(ingredientListString);
            interfaceManager.setActiveOnboardingInterface(true);
            interfaceManager.setActiveScanningInterface(false);
            pipeline.stopPipeline();
        } 
        else if (name == "ClearAllButtonScript") {
            AudioSource.PlayClipAtPoint(buttonClip.clip, this.transform.parent.position);
            controller.clearMemory();
        }
        else if (name == "ExitScript")
        {
            AudioSource.PlayClipAtPoint(buttonClip.clip, exitButton.transform.position);
            interfaceManager.setActiveWelcomeInterface(true);
            interfaceManager.setActiveScanningInterface(false);
            pipeline.stopPipeline();
            
        }
        else if (name == "KeyboardSwitchButton")
        {
            AudioSource.PlayClipAtPoint(buttonClip.clip, keyBoardSwitch.transform.position);
            if (VirtualKeyBoard.activeSelf) {
                // set "scanning component" active
                keyboardSwitchFunc(false);     
            } else {
                pipeline.stopPipeline(); // do not scan during keyboard input
                keyboardSwitchFunc(true);
            }
        }
        else
        {
            Debug.Log("Unknown button");
        }
    }

    public void keyboardSwitchFunc(bool b) {
        ScanScreen.SetActive(!b);
        startScanning.SetActive(!b);
        pauseScanning.SetActive(b);
        VirtualKeyBoard.SetActive(b);
        if (b) {
            keyBoardSwitch.transform.Find("IconAndText/Icon").gameObject.GetComponent<Renderer>().material = Resources.Load("Mat/CameraIconMaterial", typeof(Material)) as Material;
        } else {
            keyBoardSwitch.transform.Find("IconAndText/Icon").gameObject.GetComponent<Renderer>().material = Resources.Load("Mat/SearchIconMaterial", typeof(Material)) as Material;
        }
    }
}