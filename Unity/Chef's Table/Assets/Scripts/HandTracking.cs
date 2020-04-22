using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;
using System.Collections;

namespace MagicLeap
{
    // Component used to interact with objects that is tagged with "Interactable"
    // using hands.
    public class HandTracking : MonoBehaviour
    {
        private static bool RAYCAST_ENABLED = true;

        //#pragma warning disable 414
        [SerializeField, Tooltip("The hand to visualize.")]
        private MLHandTracking.HandType _handType = MLHandTracking.HandType.Left;
        //#pragma warning restore 414

        // position of the keypoints, only 3 for now.
        private Transform _indexFinger = null;
        private Transform _thumb = null;
        private Transform _wrist = null;

        private bool canIPlace = false;
        private bool canIGrab = false;
        public GameObject selectedGameObject; // the gameObject being dragged or moved

        public enum HandPoses { Ok, Finger, Thumb, OpenHand, Pinch, NoPose };
        public HandPoses pose = HandPoses.NoPose;
        public GameObject objectToPlace; // Object to place when thumb pose is detected.

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
            if (RAYCAST_ENABLED) {
                MLRaycast.Start();
            }
            Initialize();
        }

        // Clean up.
        void OnDestroy()
        {
            if (MLHandTracking.IsStarted) {
                MLHandTracking.Stop();
            }

            if (MLRaycast.IsStarted) {
                MLRaycast.Stop();
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
                other.gameObject.GetComponent<Renderer>().material.color = Color.white;
            }
        }

        /// Update the keypoint positions.
        void Update()
        {
            // handtracking and moving objects
            if (MLHandTracking.IsStarted)
            {
                transform.position = Hand.Index.KeyPoints[2].Position;
                if (GetGesture(Hand, MLHandTracking.HandKeyPose.Thumb)) {
                    pose = HandPoses.Thumb;
                    if (canIPlace == true) {
                        GameObject placedOject = Instantiate(objectToPlace, Hand.Index.KeyPoints[2].Position, transform.rotation);
                        canIPlace = false;
                    }
                    return;
                }

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
                canIPlace = true;
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

            // raycasting
            if (MLRaycast.IsStarted) {
                MLRaycast.QueryParams _raycastParams = new MLRaycast.QueryParams {
                    // update the parameters with the index finger's direction
                    // What about when index finder's tip is not detected or when it's hiddenn.
                    Position = _indexFinger.position,
                    Direction = _indexFinger.position - Hand.Index.KeyPoints[1].Position,
                    UpVector = _indexFinger.up,
                    Width = 1,
                    Height = 1
                };
                MLRaycast.Raycast(_raycastParams, HandleOnReceiveRaycast);
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

        // Callback function for raycasting.
        // 'point' is the hit point.
        void HandleOnReceiveRaycast(MLRaycast.ResultState state, UnityEngine.Vector3 point, UnityEngine.Vector3 normal, float confidence) {
            if (state == MLRaycast.ResultState.HitObserved) {
                // do something in here. This is a callback function.
            }
        }
    }
}
