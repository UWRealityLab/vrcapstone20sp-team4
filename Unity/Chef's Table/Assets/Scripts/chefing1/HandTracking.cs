using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;
using MagicLeap.Core.StarterKit;
using MagicLeapTools;

namespace MagicLeap
{
    // Component used to interact with objects that is tagged with "Interactable"
    // using hands.
    public class HandTracking : MonoBehaviour
    {

        //#pragma warning disable 414
        [SerializeField, Tooltip("The hand to visualize.")]
        private MLHandTracking.HandType _handType = MLHandTracking.HandType.Left;
        //#pragma warning restore 414

        // position of the keypoints, only 3 for now.
        private Transform _indexFinger = null;
        private Transform _thumb = null;
        private Transform _wrist = null;
        private Transform _middle = null;
        private Vector3 offset;

        private bool canIGrab = false;
        public GameObject selectedGameObject; // the gameObject being dragged or moved

        public enum HandPoses { Ok, Finger, Thumb, OpenHand, Pinch, NoPose, Point, L};
        public HandPoses pose = HandPoses.NoPose;

        private Vector3? prevPosition = null;
        public Transform  ctransform; // Camera's transform
        //private GameObject raycast;
        private GameObject onboarding;
        private bool thumbPoseChanged = false;
        private bool okPoseChanged = false;
        private MainScheduler2 mainScheduler;
        private GameObject movingPoint;
        private InterfaceManager interfaceManager;

        private Material movingPointNormalMat;
        private Material movingPointHighlightMat;

        private MLHandTracking.Hand Hand
        {
            get
            {
                if (_handType == MLHandTracking.HandType.Left)
                {
                    return MLHandTracking.Left;
                }
                else
                {
                    return MLHandTracking.Right;
                }
            }
        }

        private void Awake()
        {
            //wrappingSimulation = GameObject.Find("CuttingSimulation/wrappingSimulation");
        }

        // Calls Start on MLHandTrackingStarterKit and initializes the lists of hand transforms.
        void Start()
        {
            MLHandTracking.Start();
            Initialize();
            onboarding = GameObject.Find("OnBoardingInterface");
            mainScheduler = GameObject.Find("Scheduler").GetComponent<MainScheduler2>();
            interfaceManager = GameObject.Find("InterfaceManager").GetComponent<InterfaceManager>();
            GameObject headLockCanvas = GameObject.Find("HeadLockCanvas");
            Vector3 headLockCanvasPos = headLockCanvas.transform.position;
            movingPoint = GameObject.Find("MovingPoint");
            Vector3 haloObjectPos = movingPoint.transform.position;
            offset = haloObjectPos - headLockCanvasPos + new Vector3(0.01f, 0.09f, -0.035f);
            //offset = haloObjectPos - headLockCanvasPos + new Vector3(0, 0.06f, -0.035f);
            movingPointNormalMat = Resources.Load("Mat/Wireframe", typeof(Material)) as Material;
            movingPointHighlightMat = Resources.Load("Mat/MovingPointHighlightMat", typeof(Material)) as Material;
        }

        // Clean up.
        void OnDestroy()
        {
            if (MLHandTracking.IsStarted) {
                MLHandTracking.Stop();
            }
        }

        // This is calleed when the tip of index finger touches
        // an interactable object.
        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Interactable") {
                if (canIGrab == false) {
                    selectedGameObject = other.gameObject;
                    canIGrab = true;
                    movingPoint.GetComponent<Renderer>().material = movingPointHighlightMat;
                }
            }
        }

        // When the tip of the index finger leaves the interactable object.
        void OnTriggerExit(Collider other) {
            if (other.gameObject.tag == "Interactable") {
                canIGrab = false;
                prevPosition = null;
                movingPoint.GetComponent<Renderer>().material = movingPointNormalMat;
            }
        }

        /// Update the keypoint positions.
        void Update()
        {
            // handtracking and moving objects
            if (MLHandTracking.IsStarted)
            {
                //transform.position = Hand.Thumb.KeyPoints[2].Position;
                transform.position = Hand.Index.KeyPoints[2].Position;
                if (GetGesture(Hand, MLHandTracking.HandKeyPose.Ok))
                {
                    if (okPoseChanged == true)
                    {
                        /*
                        if (!interfaceManager.isActiveHeadLockCanvas())
                        {
                            interfaceManager.setActiveHeadLockCanvas(true);
                        }
                        else
                        {
                            interfaceManager.setActiveHeadLockCanvas(false);
                        }
                        */
                        pose = HandPoses.Ok;
                        okPoseChanged = false;
                    }
                }
                else if (GetGesture(Hand, MLHandTracking.HandKeyPose.Thumb))
                {
                    /*
                    if (thumbPoseChanged == true)
                    {

                        if (!raycast.activeSelf)
                        {
                            raycast.SetActive(true);
                        }
                        else
                        {
                            raycast.SetActive(false);
                        }
                        pose = HandPoses.Thumb;
                        thumbPoseChanged = false;
                    }*/
                }
                
                else if (GetGesture(Hand, MLHandTracking.HandKeyPose.OpenHand))
                {
                    //if (okPoseChanged == true)
                    //if (raycast.activeSelf) 
                    {
                        //pose = HandPoses.Ok;
                        //interf.GetComponent<PlaceInFront>().Place();
                        //onboarding.GetComponent<PlaceInFront>().Place();
                        //okPoseChanged = false;
                    }
                }
                
                else if (GetGesture(Hand, MLHandTracking.HandKeyPose.C) || GetGesture(Hand, MLHandTracking.HandKeyPose.L))
                {
                    if (canIGrab)
                    {
                        Vector3 currPos = Hand.Thumb.KeyPoints[2].Position;
                        if (prevPosition != null)
                        {
                            float smooth = 5.0f; // bad style 
                            float rotationSpeed = 3600.0f; // bad style for now
                            Vector3 rotationDirection = currPos - prevPosition.Value;
                            Vector3 cameraRight = ctransform.right;
                            rotationDirection = Vector3.Project(rotationDirection, cameraRight);
                            float left = Vector3.Dot(rotationDirection.normalized, cameraRight.normalized);
                            float magnitude = rotationDirection.magnitude * rotationSpeed;
                            if (left == 1.0f)
                            { // hand moving right
                                magnitude *= -1;
                                magnitude -= 5.0f; // fine-tuning
                            }
                            Quaternion prev = selectedGameObject.transform.rotation;
                            Quaternion target = Quaternion.Euler(0, prev.eulerAngles.y + magnitude, 0);
                            selectedGameObject.transform.rotation =
                            Quaternion.Slerp(selectedGameObject.transform.rotation, target, Time.deltaTime * smooth);
                        }
                        prevPosition = currPos;
                    }
                    pose = HandPoses.L;
                }
                else if (GetGesture(Hand, MLHandTracking.HandKeyPose.Pinch)) {
                    pose = HandPoses.Pinch;
                    if (canIGrab)
                    {
                        selectedGameObject.transform.localPosition = transform.position + offset;
                        // This is so that the object wont leave our hand while we're dragging it.
                        gameObject.GetComponent<SphereCollider>().radius = 0.04f;
                    }
                } else {
                    prevPosition = null;
                    pose = HandPoses.NoPose;
                    thumbPoseChanged = true;
                    okPoseChanged = true;
                    if (selectedGameObject && canIGrab == false)
                    {
                        selectedGameObject = null;
                        gameObject.GetComponent<SphereCollider>().radius = 0.01f;
                    }
                }
                _indexFinger.position = Hand.Index.KeyPoints[2].Position;
                _indexFinger.gameObject.SetActive(Hand.IsVisible);

                _thumb.gameObject.SetActive(false);
                _middle.gameObject.SetActive(false);


                //indexTip.transform.position = Hand.Index.KeyPoints[2].Position;
                //indexTip.SetActive(Hand.IsVisible);
            }
        }

        /// Initialize the available KeyPoints.
        private void Initialize()
        {
            // Index
            _indexFinger = CreateKeyPoint(Hand.Index.KeyPoints[2], Color.white).transform;

            // Thumb
            _thumb = CreateKeyPoint(Hand.Thumb.KeyPoints[2], Color.white).transform;

            // Wrist
            _wrist = CreateKeyPoint(Hand.Wrist.KeyPoints[0], Color.white).transform;
            _middle = CreateKeyPoint(Hand.Middle.KeyPoints[2], Color.white).transform;
        }

        /// Create a GameObject for the desired KeyPoint.
        private GameObject CreateKeyPoint(MLHandTracking.KeyPoint keyPoint, Color color)
        {
            GameObject newObject;
            newObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            newObject.SetActive(false);
            newObject.transform.SetParent(transform);
            newObject.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            newObject.name = keyPoint.ToString();
            newObject.GetComponent<Renderer>().material.color = color;
            return newObject;
        }

        // Checks to see whether the pose 'type' is being recognized.
        private bool GetGesture(MLHandTracking.Hand hand, MLHandTracking.HandKeyPose type)
        {
            if (hand != null)
            {
                if (hand.KeyPose == type)
                {
                    if (hand.HandKeyPoseConfidence > 0.9f)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
