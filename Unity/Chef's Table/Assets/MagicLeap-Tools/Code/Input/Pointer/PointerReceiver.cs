﻿// ---------------------------------------------------------------------
//
// Copyright (c) 2018-present, Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/terms/developer
//
// ---------------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

namespace MagicLeapTools
{
    /// <summary>
    /// Receives interactions and input from a pointer.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    //[RequireComponent(typeof(PursuitJoint))]
    public sealed class PointerReceiver : InputReceiver
    {
#if PLATFORM_LUMIN
        //Events:
        public CollisionGameObjectEvent OnDraggedCollisionEnter;
        public CollisionGameObjectEvent OnDraggedAlongSurface;
        public CollisionGameObjectEvent OnDraggedCollisionStay;
        public CollisionGameObjectEvent OnDraggedCollisionExit;

        //Public Variables:
        [Tooltip("Can we drag this?")]
        public bool draggable = true;
        [Tooltip("Toggles isKinematic on released from dragging so it will be imovable by other collisions.")]
        public bool kinematicWhileIdle = true;
        [Tooltip("Face camera while being dragged?")]
        public bool faceWhileDragging;
        [Tooltip("Face away from the the wall we are pointing at.")]
        public bool matchWallWhileDragging;
        [Tooltip("If this transform's forward does not match the direction of it's content turn this on.")]
        public bool invertForward;
        public string recipe_name;
        public bool exited;

        //Public Properties:
        public bool Dragging
        {
            get;
            private set;
        }

        //Private Variables:
        private readonly float _draggedAlongSurfaceDistanceThreshold = 0.00635f;
        private readonly float _defaultDrag = 10;
        private List<Pointer> _selectingPointers;
        private PursuitJoint _pursuitJoint;
        private Vector3 _dragOffset;
        private Rigidbody _rigidBody;
        private Vector3 _previousDraggedAlongPosition;
        private ScaleManipulator _scaleManipulator;
        private RotationManipulator _rotationManipulator;
        private bool _manuallyRotated;
        private bool _colliding;
        private Collision _currentCollision;
        private Pointer _draggingPointer;

        private Vector3? originalPos;
        private bool pointerExited = true;

        //Init:
        protected override void ResetInherited()
        {
            //setups:
            GetComponent<PursuitJoint>().enabled = false;
            GetComponent<Rigidbody>().drag = _defaultDrag;
        }

        protected override void AwakeInherited()
        {
            //refs:
            _rigidBody = GetComponent<Rigidbody>();
            _scaleManipulator = GetComponent<ScaleManipulator>();
            _rotationManipulator = GetComponent<RotationManipulator>();

            //dragging gui causes issues since scale is so much different so let's prevent it (kinda weird to let this happen any way):
            if (GetComponent<Image>() != null && draggable)
            {
                Debug.LogWarning("GUI can not be dragged - disabling drag ability on: " + name, this);
                draggable = false;
            }

            //refs:
            _pursuitJoint = GetComponent<PursuitJoint>();

            //setups:
            _pursuitJoint.enabled = false;
            if (kinematicWhileIdle)
            {
                _rigidBody.isKinematic = true;
            }
        }

        private void OnTargetEnterHandler()
        {
            TextMeshPro text = GetComponentInChildren(typeof(TextMeshPro)) as TextMeshPro;
            text.text = "TEXT CHANGED";
        }

        //Flow:
        protected override void OnEnableInherited()
        {
            _selectingPointers = new List<Pointer>();
        }

        //Loops:
        private void Update()
        {
            if (DraggedBy.Count > 0)
            {
                if (_selectingPointers.Count == 1)
                {
                    //if single drag then use that location:
                    _pursuitJoint.targetLocation = _selectingPointers[0].InternalInteractionPoint - _dragOffset;
                }
                else if (_selectingPointers.Count == 2)
                {
                    //if double drag (or more) use the location between the first two pointer tips:
                    _pursuitJoint.targetLocation = Vector3.Lerp(_selectingPointers[0].InternalInteractionPoint, _selectingPointers[1].InternalInteractionPoint, .5f);
                }

                if (_colliding)
                {
                    //match wall surface 
                    if (matchWallWhileDragging)
                    {
                        _manuallyRotated = true;
                        Vector3 lookNormal = Vector3.ProjectOnPlane(_currentCollision.GetContact(0).normal, Vector3.up);
                        if (invertForward)
                        {
                            lookNormal *= -1;
                        }
                        
                        if (lookNormal != Vector3.zero)
                        {
                            transform.rotation = Quaternion.LookRotation(lookNormal);
                        }
                    }
                }
                else
                {
                    //face the camera:
                    if (faceWhileDragging && !_manuallyRotated)
                    {
                        Vector3 toCamera = Vector3.ProjectOnPlane(_mainCamera.transform.position - transform.position, Vector3.up);
                        if (invertForward)
                        {
                            toCamera *= -1;
                        }
                        transform.rotation = Quaternion.LookRotation(toCamera);
                    }
                }
            }
        }

        //Overrides:
        public override void TargetExit(GameObject sender)
        {
            base.TargetExit(sender);

            //turn off joint:
            _pursuitJoint.enabled = false;

            //uncatalog:
            Pointer pointer = sender.GetComponent<Pointer>();
            if (pointer != null)
            {
                if (_selectingPointers.Contains(pointer))
                {
                    _selectingPointers.Remove(pointer);
                }
            }
            
            if (tag == "Button" || tag == "RecipeButton")
            {
                //transform.position = originalPos.Value;
                //pointerExited = true;
                Behaviour halo = (Behaviour)GetComponent("Halo");
                halo.enabled = false; // false
            }
        }

        public override void TargetEnter(GameObject sender)
        {
            base.TargetEnter(sender);
            if (tag == "Button" || tag == "RecipeButton") 
            {
                //TextMeshProUGUI text = GetComponentInChildren(typeof(TextMeshProUGUI)) as TextMeshProUGUI;
                //text.color = Color.red;
                //if (!pointerExited && originalPos != null)
                //{
                    //transform.position = originalPos.Value;
                //}
                //originalPos = transform.position;
                Behaviour halo = (Behaviour)GetComponent("Halo");
                halo.enabled = true; // false
                //transform.position = originalPos.Value - Camera.main.transform.forward.normalized * 0.1f; //; new Vector3(0, 0, -0.05f);
                //pointerExited = false;
            }
        }

        public override void Fire0DownReceived(GameObject sender)
        {
            base.Fire0DownReceived(sender);

            //catalog:
            Pointer pointer = sender.GetComponent<Pointer>();
            if (pointer != null)
            {
                _selectingPointers.Add(pointer);
            }
            if (tag == "Button")
            {
                GetComponent<Button>().onClick.Invoke();
            }
        }
        /*
        public override void Fire1DownReceived(GameObject sender)
        {
            base.Fire1DownReceived(sender);
            if (tag == "RecipeButton")
            {
                GameObject sche = GameObject.Find("Scheduler");
                MainScheduler ms = sche.GetComponent<MainScheduler>();
                ms.startTutorial(recipe_name);
                GameObject obinterface = GameObject.Find("OnBoardingInterface");
                obinterface.SetActive(false);
                Debug.Log(recipe_name);
            }
        }
        */
        public override void Fire0UpReceived(GameObject sender)
        {
            base.Fire0UpReceived(sender);
            
            if (_selectingPointers.Count == 0)
            {
                _pursuitJoint.enabled = false;
                return;
            }

            _colliding = false;

            //uncatalog:
            Pointer pointer = sender.GetComponent<Pointer>();
            if (pointer != null)
            {
                _selectingPointers.Remove(pointer);
            }
        }

        public override void Fire2DownReceived(GameObject sender)
        {
            //reset any manipulations:
            base.Fire2DownReceived(sender);

            if (_selectingPointers.Count <= 1)
            {
                _scaleManipulator?.ResetManipulation();
                _rotationManipulator?.ResetManipulation();
            }
        }

        public override void LeftReceived(GameObject sender)
        {
            //scale down:
            base.LeftReceived(sender);

            if (_selectingPointers.Count <= 1)
            {
                _scaleManipulator?.ScaleDown();
            }
        }

        public override void RightReceived(GameObject sender)
        {
            //scale up:
            base.RightReceived(sender);

            if (_selectingPointers.Count <= 1)
            {
                _scaleManipulator?.ScaleUp();
            }
        }

        public override void RadialDragReceived(float angleDelta, GameObject sender)
        {
            //rotate:
            base.RadialDragReceived(angleDelta, sender);

            if (_selectingPointers.Count <= 1)
            {
                _manuallyRotated = true;
                _rotationManipulator?.Rotate(angleDelta);
            }
        }

        public override void DragBegin(GameObject sender)
        {
            base.DragBegin(sender);

            //still set these so we can get intent events for dragging even if we don't physically drag:
            Dragging = true;
            _manuallyRotated = false;

            if (draggable)
            {
                //make sure we CAN drag:
                _rigidBody.isKinematic = false;

                //get an offset so we don't pop the location:
                Pointer pointer = sender.GetComponent<Pointer>();
                if (pointer != null)
                {
                    _draggingPointer = pointer;
                }
                
                _dragOffset = _draggingPointer.InternalInteractionPoint - transform.position;

                //turn on joint:
                _pursuitJoint.enabled = true;
            }
        }

        public override void DragEnd(GameObject sender)
        {
            base.DragEnd(sender);

            if (!Dragging)
            {
                return;
            }

            //someone is still holding onto us:
            if (_selectingPointers.Count > 0)
            {
                return;
            }

            //set kinematic status:
            if (kinematicWhileIdle)
            {
                _rigidBody.isKinematic = true;
            }
            else
            {
                _rigidBody.isKinematic = false;
            }

            Dragging = false;

            //turn off joint:
            _pursuitJoint.enabled = false;
        }

        //Event Handlers:
        private void OnCollisionEnter(Collision collision)
        {
            if (!Dragging)
            {
                return;
            }

            _currentCollision = collision;
            _manuallyRotated = false;
            _previousDraggedAlongPosition = transform.position;

            if (!_colliding)
            {
                _colliding = true;
                OnDraggedCollisionEnter?.Invoke(collision, _draggingPointer.gameObject);
            }
        }

        private void OnCollisionStay(Collision collision)
        {
            if (!Dragging)
            {
                return;
            }

            _currentCollision = collision;
            OnDraggedCollisionStay?.Invoke(collision, _draggingPointer.gameObject);

            //are we dragging along a surface?
            if (Vector3.Distance(_previousDraggedAlongPosition, transform.position) > _draggedAlongSurfaceDistanceThreshold)
            {
                OnDraggedAlongSurface?.Invoke(collision, _draggingPointer.gameObject);
            }

            //cache:
            _previousDraggedAlongPosition = transform.position;
        }

        private void OnCollisionExit(Collision collision)
        {
            if (!Dragging)
            {
                return;
            }

            if (_colliding)
            {
                _colliding = false;
                OnDraggedCollisionExit?.Invoke(collision, _draggingPointer.gameObject);
            }

            _currentCollision = collision;
            _previousDraggedAlongPosition = transform.position;
        }
#endif
    }
}