
using UnityEngine;
using UnityEngine.UI;

public class HardCode : MonoBehaviour
{
    public GameObject mainCam;
    public GameObject applicationState;
    private ApplicationState As;
    
    public GameObject knife;
    public GameObject bowl;
    public GameObject fork;
    public GameObject microwave;

    public GameObject indicator;

    public GameObject scanningContainer;
    private ScanningInterfaceController controller;
    // Update is called once per frame

    void Start() {
        controller = scanningContainer.GetComponent<ScanningInterfaceController>();
        As = applicationState.GetComponent<ApplicationState>();
    }


    public void hardcodeSetState(string name, GameObject go) {
        As.setLocation(name, go.transform.position);
        GameObject debugObject = Instantiate(indicator, go.transform.position, Quaternion.identity);
        debugObject.transform.LookAt(mainCam.transform.position);
        debugObject.transform.FindChild("Canvas").FindChild("Text").gameObject.GetComponent<Text>().text = name;
        Destroy(debugObject, 3f); 
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) {
            hardcodeSetState("microwave", microwave);
        }
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (fork.transform.localScale.x == 0) {
                fork.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                knife.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                microwave.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                bowl.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            } else {
                fork.transform.localScale = Vector3.zero;
                knife.transform.localScale =Vector3.zero;
                microwave.transform.localScale = Vector3.zero;
                bowl.transform.localScale = Vector3.zero;
            }
        }
        if (Input.GetKeyDown(KeyCode.B)) {
            hardcodeSetState("knife", knife);
            hardcodeSetState("bowl", bowl);
        } 
        if (Input.GetKeyDown(KeyCode.C)) {
           // hardcodeSetState("fork", fork);
        } 
        if (Input.GetKeyDown(KeyCode.D)) {
            controller.updateIngredientListByScanning("[\"tomato\"]");
        } 
        if (Input.GetKeyDown(KeyCode.E)) {
            controller.updateIngredientListByScanning("[\"egg\"]");
        } 
    }
}
