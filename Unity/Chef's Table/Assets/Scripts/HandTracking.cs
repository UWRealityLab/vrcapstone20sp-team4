using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;
using MagicLeap.Core.StarterKit;

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

        private bool canIGrab = false;
        public GameObject selectedGameObject; // the gameObject being dragged or moved

        public enum HandPoses { Ok, Finger, Thumb, OpenHand, Pinch, NoPose, Point};
        public HandPoses pose = HandPoses.NoPose;

        private Vector3? prevPosition = null;
        public Transform  ctransform; // Camera's transform
        public GameObject handInterface;
        private GameObject raycast;

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

        // Calls Start on MLHandTrackingStarterKit and initializes the lists of hand transforms.
        void Start()
        {
            MLHandTracking.Start();
            Initialize();
            raycast = GameObject.Find("HandPointer");
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
            Debug.Log("onTriggerEnter");
            if (other.gameObject.tag == "Interactable") {
                if (canIGrab == false) {
                    selectedGameObject = other.gameObject;
                    canIGrab = true;
                    other.gameObject.GetComponent<Renderer>().material.color = Color.red;
                }
            }
        }

        // When the tip of the index finger leaves the interactable object.
        void OnTriggerExit(Collider other) {
            if (other.gameObject.tag == "Interactable") {
                canIGrab = false;
                prevPosition = null;
                other.gameObject.GetComponent<Renderer>().material.color = Color.white;
            }
        }

        /// Update the keypoint positions.
        void Update()
        {
            // handtracking and moving objects
            if (MLHandTracking.IsStarted)
            {
                transform.position = Hand.Thumb.KeyPoints[2].Position;
                
                if (GetGesture(Hand, MLHandTracking.HandKeyPose.Thumb))
                {
                    if (!raycast.activeSelf)
                    {
                        raycast.SetActive(true);
                    }
                    else
                    {
                        raycast.SetActive(false);
                    }
                }
                /*
                if (raycast.activeSelf)
                {
                    raycast.SetActive(false);
                }
                */
                if (GetGesture(Hand, MLHandTracking.HandKeyPose.C) || GetGesture(Hand, MLHandTracking.HandKeyPose.L)) {
                    if (canIGrab) {
                        Vector3 currPos = Hand.Thumb.KeyPoints[2].Position;
                        if (prevPosition != null) {
                            float smooth = 5.0f; // bad style 
                            float rotationSpeed = 3600.0f; // bad style for now
                            Vector3 rotationDirection = currPos - prevPosition.Value;
                            Vector3 cameraRight = ctransform.right;
                            rotationDirection = Vector3.Project(rotationDirection, cameraRight);
                            float left = Vector3.Dot(rotationDirection.normalized, cameraRight.normalized);
                            float magnitude = rotationDirection.magnitude * rotationSpeed;
                            if (left == 1.0f) { // hand moving right
                                magnitude *= -1;
                                magnitude -= 5.0f; // fine-tuning
                            }
                            Quaternion prev = selectedGameObject.transform.rotation;
                            Quaternion target = Quaternion.Euler(0, prev.eulerAngles.y + magnitude, 0);
                            selectedGameObject.transform.rotation = 
                            Quaternion.Slerp(selectedGameObject.transform.rotation, target,  Time.deltaTime * smooth);
                        }
                        prevPosition = currPos;
                    }
                    return;
                }
                prevPosition = null;
               
                if (GetGesture(Hand, MLHandTracking.HandKeyPose.Pinch)) {
                    pose = HandPoses.Pinch;
                    if (canIGrab) {
                        selectedGameObject.transform.position = transform.position;
                        // This is so that the object wont leave our hand while we're dragging it.
                        gameObject.GetComponent<SphereCollider>().radius = 0.2f;
                    }
                    return;
                }

                pose = HandPoses.NoPose;
                if (selectedGameObject && canIGrab == false) {
                    selectedGameObject = null;
                    gameObject.GetComponent<SphereCollider>().radius = 0.01f;
                }
                
                // Index
                _indexFinger.position = Hand.Index.KeyPoints[2].Position;
                _indexFinger.gameObject.SetActive(Hand.IsVisible);

                // Thumb
                _thumb.position = Hand.Thumb.KeyPoints[2].Position;
                _thumb.gameObject.SetActive(Hand.IsVisible);

                // Wrist
                _wrist.position = Hand.Wrist.KeyPoints[0].Position;
                _wrist.gameObject.SetActive(Hand.IsVisible);
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
