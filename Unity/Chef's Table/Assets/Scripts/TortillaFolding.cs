using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TortillaFolding : MonoBehaviour
{
    // Start is called before the first frame update
    private Animator anim;
    public GameObject handle1;
    public GameObject handle2;
    public GameObject handle3;
    public GameObject handle4;
    private Vector3 startPos1;
    private Vector3 startPos2;
    private Vector3 startPos3;
    private Vector3 startPos4;
    private float radius;

    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        startPos1 = handle1.transform.position;
        startPos2 = handle2.transform.position;
        startPos3 = handle3.transform.position;
        startPos4 = handle4.transform.position;
        Vector3 tortilla_center = transform.TransformPoint(Vector3.zero);
        Vector3 world_pos_handle = handle1.transform.TransformPoint(handle1.transform.position);
        radius = (tortilla_center - world_pos_handle).magnitude * 1.1f;
    }

    // Update is called once per frame
    void Update()
    {

        float fold1 = calculateFoldPara(handle1, startPos1);
        anim.SetFloat("fold1", fold1);
        float fold2 = calculateFoldPara(handle2, startPos2);
        anim.SetFloat("fold2", fold2);
        float fold3 = calculateFoldPara(handle3, startPos3);
        anim.SetFloat("fold3", fold3);
        float fold4 = calculateFoldPara(handle4, startPos4);
        anim.SetFloat("fold4", fold4);
        if (fold1 > 1)
        {
            anim.SetBool("12", true);
        }
        if (fold2 > 1)
        {
            anim.SetBool("23", true);
        }
        if (fold3 > 1)
        {
            anim.SetBool("34", true);
        }
        if (fold4 > 1)
        {
            anim.SetBool("41", true);
        }
    }
    float calculateFoldPara(GameObject handle, Vector3 startPos)
    {
        
        Vector3 tortilla_center = transform.TransformPoint(Vector3.zero);
        Vector3 world_pos_handle = handle.transform.TransformPoint(handle.transform.position);
        Vector3 center_to_handle = world_pos_handle - tortilla_center;
        if (center_to_handle.magnitude > radius)
        {
            handle.transform.position = startPos;
            return 0;
        }
        float angle = Mathf.Rad2Deg * Mathf.Acos(Vector3.Dot(center_to_handle, Vector3.up) / center_to_handle.magnitude);
        if (angle > 90)
        {
            return 0;
        }
        Vector3 currentxz = new Vector3(world_pos_handle.x, 0, world_pos_handle.z);
        Vector3 startxz = new Vector3(startPos.x, 0, startPos.z);
        Vector3 tortillaxz = new Vector3(tortilla_center.x, 0, tortilla_center.z);
        if ((currentxz - startxz).magnitude >= (tortillaxz - startxz).magnitude)
        {
            return 1.1f;
        }
        return (90 - angle) / 90;
    }
    
}
