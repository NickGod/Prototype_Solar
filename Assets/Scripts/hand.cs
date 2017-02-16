using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class hand : MonoBehaviour {
    public Transform _rightHand;
    Transform _rightIndex;
    // accept three elements, first is inventory slot
    // second is demonstrate spot
    // third is orbitting slot
 
    float _spinSpeed = 0.0f;
    float _spinVeloMax = 5.0f;

    public Transform _otherHand;

    float _inventoryTimer = 0;
    float _inventoryTime = 0.2f;
    static bool _isInventory = false;

    public List<Transform> trfList = new List<Transform>();

    Transform _grabbedParent;
    public Transform _grabbed;

    static bool _isEditting = false;
    static Transform _editTrf = null;

    //States
    bool _isFist = false;
    bool _isGrabbing = false;
    //resizing
    static float _originDistance = 0.0f;
    static bool _onResizing = false;

    public bool isRightHand;
    // Update is called once per frame

    // automatic move back
    bool isMoving = false;
    Transform movingTar = null;

    //void Start() {
    //    if (isRightHand) {
    //        Transform _indexParent = _rightHand;
    //        foreach (Transform finger in _indexParent) {
    //            Debug.Log(finger.name);
    //            if (finger.name.Contains("index")) {
    //                _rightIndex = finger;
    //            }
    //        }
    //    }
    //}

    void Update() {
        if (!isRightHand) {
            ////for left hand


            //transform.localPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch);
            //transform.localRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.LTouch);


            //calling inventory based on left hand index
            if (IsInventoryUp()) {
                _inventoryTimer += Time.deltaTime;
            } else {
                _inventoryTimer = 0.0f;
                if (_isInventory) {
                    //TODO: inventory off animation and related mehanics here
                    transform.GetChild(0).gameObject.SetActive(false);
                }
                _isInventory = false;
            }
            if (_inventoryTimer >= _inventoryTime && !_isInventory) {
                // this function only call once for each index up
                // there is up time for inventory to appear
                //TODO: inventory on animation and related mechanics here
                _isInventory = true;
                transform.GetChild(0).gameObject.SetActive(true);
            }
        } else {
            //for right hand
            //transform.localPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
            //transform.localRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);


            //resizing
            if (IsBothFist() && _isEditting && !_onResizing) {
                _onResizing = true;
                _originDistance = Vector3.Distance(transform.position, _otherHand.position);
            }

            if (!IsBothFist()){
                _onResizing = false;
                _originDistance = 0.0f;
            }
        
            if (_onResizing) {
                float realScale = Vector3.Distance(transform.position, _otherHand.position) / _originDistance;
                if (realScale >= 1.5f) {
                    realScale = 1.49f;
                } else if (realScale <= 0.5f) {
                    realScale = 0.51f;
                }
                _editTrf.GetComponent<planet_behavior>().my_scale = realScale;
            }
            
            //Axis rotation
            if (_isEditting) {
                _editTrf.up = SetAxis();
            }
            
            //Rotating speed
            if (IsAxis2Touched() && _isEditting) {
                SetSpinSpeed();
                _editTrf.GetComponent<planet_behavior>().self_spin_spd = _spinSpeed;
                //TODO: assign spin speed to editting obj
            }

            if (OVRInput.GetDown(OVRInput.Button.Two) && _isEditting) {
                //TODO: fly to orbit
                planet_behavior pb = _editTrf.GetComponent<planet_behavior>();
                Trailmanager.instance.send_to_trail(pb,this);
                _isEditting = false;
                _editTrf = null;
            }

            //TODO: shooting
            if (IsShooting()) {
                
            }
 

            
            ////test
            //if (IsAxis2Touched()) {
            //    SetSpinSpeed();
            //    Debug.Log("Spin: " + _spinSpeed);
            //}
            //if (IsShooting()) {
            //    Debug.Log("Shooting");
            //}
            //if (IsFist()) {
            //    Debug.Log("Fist");
            //}
        }

        //grabbing
        _isGrabbing = false;
        if (IsFist() && !_isFist) {
            _isFist = true;
            _isGrabbing = true;
        }
        if (_isGrabbing) {
            if (_grabbed == null) {
                int count = trfList.Count;
                for (int i = count - 1; i >= 0; i--) {
                    Transform trf = trfList[i];
                    if (trf == null || !trf.gameObject.activeSelf) {
                        trfList.Remove(trf);
                    }
                }
                Transform target = GetClosest();
                //test luna merge
                if (target) {
                    _isEditting = false;
                    _grabbed = target.GetComponent<planet_behavior>().OnGrab();
                    if (!trfList.Contains(_grabbed)) {
                        trfList.Add(_grabbed);
                    }
                    if (_grabbed) {
                        if (_grabbed == _editTrf) {
                            _editTrf = null;
                        }
                        if (_grabbed.parent == _otherHand) {
                            _otherHand.GetComponent<hand>().LoseControl();
                        }
                        _grabbedParent = null;
                        _grabbed.parent = transform;
                    }
                }
            }
        } 
        
        if (!IsFist()){
            if (_grabbed) {
                _grabbed.parent = _grabbedParent;
                LoseControl();
            }
            _isFist = false;
        }

    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == Tags.Grabbable && !trfList.Contains(other.transform)) {
            trfList.Add(other.transform);
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == Tags.Grabbable) {
            trfList.Remove(other.transform);
        }
    }

    Transform GetClosest() {
        Transform select = null;
        float _minDistance = float.MaxValue;
        foreach (var trf in trfList) {
            float dis = Vector3.Distance(trf.position, transform.position);
            if (dis < _minDistance) {
                _minDistance = dis;
                select = trf;
            }
        }
        return select;
    }

    bool IsInventoryUp() {
        if (!isRightHand) {
            return !(OVRInput.Get(OVRInput.Touch.PrimaryIndexTrigger) || OVRInput.Get(OVRInput.NearTouch.PrimaryIndexTrigger));
        } else {
            return false;
        }
    }

    Vector3 SetAxis() {
        if (isRightHand) {
            return transform.up;
        }
        return Vector3.up;
    }

    bool IsShooting() {
        if (isRightHand) {
            bool thumbRelease = !(OVRInput.Get(OVRInput.Touch.SecondaryThumbRest) || OVRInput.Get(OVRInput.NearTouch.SecondaryThumbButtons));
            bool indexRelease = !(OVRInput.Get(OVRInput.Touch.SecondaryIndexTrigger) || OVRInput.Get(OVRInput.NearTouch.SecondaryIndexTrigger));

            if (thumbRelease && indexRelease) {
                return OVRInput.GetDown(OVRInput.Button.SecondaryHandTrigger);
            }
            return false;
        } else {
            return false;
        }
    }

    bool IsAxis2Touched() {
        if (isRightHand) {
            return OVRInput.Get(OVRInput.Touch.SecondaryThumbstick);
        } else {
            return false;
        }
    }

    void SetSpinSpeed() {
        if (isRightHand) {
            _spinSpeed += OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).x * Time.deltaTime;
            if (_spinSpeed > _spinVeloMax) {
                _spinSpeed = _spinVeloMax;
            } else if (_spinSpeed < -_spinVeloMax) {
                _spinSpeed = -_spinVeloMax;
            }
        }
    }

    //bool IsFistUp() {
    //    bool index;
    //    bool middle;
    //    bool thumb;
    //    if (RightHand) {
    //        index = OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger);
    //        middle = OVRInput.GetDown(OVRInput.Button.SecondaryHandTrigger);
    //        thumb = OVRInput.GetDown(OVRInput.Touch.SecondaryThumbRest) || OVRInput.GetDown(OVRInput.Touch.One) || OVRInput.GetDown(OVRInput.Touch.Two);
    //    } else {
    //        index = OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger);
    //        middle = OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger);
    //        thumb = OVRInput.GetDown(OVRInput.Touch.PrimaryThumbRest) || OVRInput.GetDown(OVRInput.Touch.Three) || OVRInput.GetDown(OVRInput.Touch.Four);
    //    }
    //    return index && middle && thumb;
    //}

    bool IsFist() {
        OVRInput.Button indexFinger;
        OVRInput.Button middleFinger;

        bool thumb;
        if (isRightHand) {
            indexFinger = OVRInput.Button.SecondaryIndexTrigger;
            middleFinger = OVRInput.Button.SecondaryHandTrigger;
            thumb = OVRInput.Get(OVRInput.Touch.SecondaryThumbRest) || OVRInput.Get(OVRInput.Touch.One) || OVRInput.Get(OVRInput.Touch.Two);
        } else {
            indexFinger = OVRInput.Button.PrimaryIndexTrigger;
            middleFinger = OVRInput.Button.PrimaryHandTrigger;
            thumb = OVRInput.Get(OVRInput.Touch.PrimaryThumbRest) || OVRInput.Get(OVRInput.Touch.Three) || OVRInput.Get(OVRInput.Touch.Four);
        }
        bool index = OVRInput.Get(indexFinger);
        bool middle = OVRInput.Get(middleFinger);

        return index && middle && thumb;
    }

    bool IsBothFist() {
        bool index = OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger) && OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger);
        bool middle = OVRInput.Get(OVRInput.Button.SecondaryHandTrigger) && OVRInput.Get(OVRInput.Button.PrimaryHandTrigger);
        bool thumb = OVRInput.Get(OVRInput.Touch.SecondaryThumbRest) || OVRInput.Get(OVRInput.Touch.One) || OVRInput.Get(OVRInput.Touch.Two);
        thumb = thumb && (OVRInput.Get(OVRInput.Touch.PrimaryThumbRest) || OVRInput.Get(OVRInput.Touch.Three) || OVRInput.Get(OVRInput.Touch.Four));
        return index && middle && thumb;
    }

    public Transform GetGrabbedParent() {
        return _grabbedParent;
    }

    public void LoseControl() {
        if (_grabbed) {
            if (_grabbed.parent != transform && _grabbed.parent != _otherHand) {
                //TODO: check the current parent, if it is not hand, then the object will fly
                //to specific spot(either editting spot or inventory or orbit trail), then update onEditting info
                //if fly to editting spot, _editTrf should be set as the grabbed obj
                //Question? should we change the isEditting to true after the planet is exactly in the place
                _editTrf = _grabbed.GetComponent<planet_behavior>().OnRelease(_editTrf, this);

                ////TODO: do current test
                //if (trfList.Contains(_editTrf)) {
                //    trfList.Remove(_editTrf);
                //}
                _isEditting = _editTrf ? true : false;
            }
            _grabbed = null;
            _grabbedParent = null;
        }
    }

    public void GetOutOfList(Transform trf) {
        if (trfList.Contains(trf)) {
            trfList.Remove(trf);
        }
    }
}
