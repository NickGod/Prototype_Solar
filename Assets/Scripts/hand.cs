using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class hand : MonoBehaviour {
    public Transform _rightHand;
    static Transform _rightIndex;
    // accept three elements, first is inventory slot
    // second is demonstrate spot
    // third is orbitting slot
 

    public Transform _otherHand;

    static float _inventoryTimer = 0;
    static float _inventoryTime = 0.2f;
    static bool _isInventory = false;

    public List<Transform> trfList = new List<Transform>();

    Transform _grabbedParent;
    Transform _grabbed;
    static Transform _pointingTrf = null;
    
    static Transform _editTrf = null;

    //States
    enum State {
        Prepare,
        Idle,
        OnClass,
        OnObject
    }
    static State _myState = State.Prepare;

    //aiming and grabbing/pointing
    bool _isFist = false;
    bool _isGrabbing = false;
    static float distance = 10.0f;

    //joystick
    float _joyStick1DVal = 0.0f;
    float _joyStick1DValMax = 1.0f;
    static planet_behavior.planet_type _selection;
    static bool _isSelected = false; //for activate joystick only once every move
    static bool _confirmed = false;

    //resizing
    //static float _originDistance = 0.0f;
    //static bool _onResizing = false;

    public bool isRightHand;
    // Update is called once per frame
    
    //right index finger
    static bool isIndexFound = false;
    LineRenderer lineRender;

    void Start() {
        lineRender = GetComponent<LineRenderer>();
    }

    void Update() {
        if (!isRightHand) {
            ////for left hand


            //transform.localPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch);
            //transform.localRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.LTouch);


            //calling inventory based on left hand index
            if (_myState == State.Idle) {
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
            }
        } else {
            //for right hand
            //transform.localPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
            //transform.localRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);
            
            if (!isIndexFound) {
                try {
                    Transform _indexParent = _rightHand.GetChild(0).GetChild(0).GetChild(0);
                    foreach (Transform finger in _indexParent) {
                        if (finger.name.Contains("index")) {
                            _rightIndex = finger;
                            isIndexFound = true;
                        }
                    }
                } catch (UnityException) {
                    ;
                }
            }

            //resizing
            //if (IsBothFist() && _isEditting && !_onResizing) {
            //    _onResizing = true;
            //    _originDistance = Vector3.Distance(transform.position, _otherHand.position);
            //}

            //if (!IsBothFist()){
            //    _onResizing = false;
            //    _originDistance = 0.0f;
            //}

            //if (_onResizing) {
            //    float realScale = Vector3.Distance(transform.position, _otherHand.position) / _originDistance;
            //    if (realScale >= 1.5f) {
            //        realScale = 1.49f;
            //    } else if (realScale <= 0.5f) {
            //        realScale = 0.51f;
            //    }
            //    _editTrf.GetComponent<planet_behavior>().my_scale = realScale;
            //}

            ////Axis rotation
            //if (_isEditting) {
            //    _editTrf.up = SetAxis();
            //}

            //changing parameter by moving joysticks
            if (IsAxis2Touched() && _myState == State.OnObject) {
                if (!_confirmed) {
                    //on selecting which attribute to change
                    float val = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).x * Time.deltaTime;
                    if (!_isSelected) {
                        if (val > 0.5f) {
                            _isSelected = true;
                            val = 1;
                            //TODO:
                            //_selection = luna_function();
                        } else if (val < -0.5f) {
                            _isSelected = true;
                            val = -1;
                            //TODO:
                            //_selection = luna_function();
                        }
                    }
                    if (val <= 0.2f && val >= -0.2f) {
                        _isSelected = false;
                    }
                } else {
                    //on changing value of the attribute
                    SetJoyStick1DVal();
                    //TODO: call luna function with parameter _selection and value
                }
                if (IsConfirmed()) {
                    _confirmed = !_confirmed;
                }
                
            }

            //TODO: keep mechanism but call when it hits the black hole
            //if (OVRInput.GetDown(OVRInput.Button.Two) && _isEditting) {
            //    //TODO: fly to orbit
            //    planet_behavior pb = _editTrf.GetComponent<planet_behavior>();
            //    Trailmanager.instance.send_to_trail(pb,this);
            //    _isEditting = false;
            //    _editTrf = null;
            //}

            //TODO: shooting
            if (IsAiming() && isIndexFound) {
                //TODO: check this, this is a new change after Mim's test
                //TODO: change the size of planet after you release it, when you hold it, it doesn't change the size
                lineRender.enabled = true;
                Vector3[] positions = new Vector3[2] { _rightIndex.position, _rightIndex.right * 1000f + _rightIndex.position };
                lineRender.SetPositions(positions);


                if (IsShooting()) {
                    RaycastHit hit;
                    Ray shootingRay = new Ray(_rightIndex.position, _rightIndex.right);

                    if (Physics.Raycast(shootingRay, out hit)) {
                        if (_myState == State.Prepare) {
                            if (hit.collider.gameObject.name == "SetUpSolarSystem") {
                                //TODO: enable solar system
                            } else if (hit.collider.gameObject.name == "SetUpSun") {
                                //TODO: enable sun
                                //TODO: set _editTrf to sun
                                //_editTrf = <Sun> 
                                //_selection = lunaFunction.getselection(0)
                                _isSelected = false;
                                _confirmed = false;
                                _myState = State.OnObject;
                            }
                        } else if (_myState == State.Idle) {
                            //TODO: point planet from orbit 
                            GameObject underCtrObj = hit.collider.gameObject;
                            //TODO: check the type of planet and enable control only when
                            // it is a real planet object
                            if (underCtrObj.tag == Tags.Grabbable && realplanet && !_grabbed) {
                                _pointingTrf = underCtrObj.transform;
                                _pointingTrf = _pointingTrf.GetComponent<planet_behavior>().OnGrab();
                            }
                            
                        } else if (_myState == State.OnClass) {
                            //TODO: 1. selecting attribute to the current editing class
                            //2. choosing button to either creating a new class or modifying current class
                            //if (clicking button) {
                            //    _myState = State.Idle;
                            //}
                        } else if (_myState == State.OnObject) {
                            //TODO: clicking button of "instantiate"
                            //if (clicking button) {
                            //    _myState = State.Idle;
                            //}
                        }
                    }
                }
                //TODO: if release middle finger call release
            } else {
                lineRender.enabled = false;
                _pointingTrf = null;
                //TODO: if press index trigger also call release
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
        if (_myState == State.Idle) {
            if (_pointingTrf) {
                RaycastHit hit;
                Ray draggingRay = new Ray(_rightIndex.position, _rightIndex.right);

                Vector3 _target;
                if (Physics.Raycast(draggingRay, out hit)) {
                    GameObject _hitObj = hit.collider.gameObject;
                    if (_hitObj.tag == Tags.Reachable) {
                        _target = _hitObj.transform.position;
                    }
                } else {
                    _target = _rightIndex.position + _rightIndex.right * distance;
                }
                //TODO: update pointTrf position and check if it should be deleted


                //planet_behavior pb = hit.collider.gameobject.getcomponent<planet_behavior>();
                //if (pb && pb.my_type == planet_behavior.planet_type.real_planet) {
                //    pb.delete_me();
                //}
            }

            //pointing and dragging release
            if (!IsInShootingGesture() && _pointingTrf) {
                _pointingTrf.GetComponent<planet_behavior>().OnRelease(this);
                _pointingTrf = null;
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
                    if (target) {
                        _grabbed = target.GetComponent<planet_behavior>().OnGrab();
                        if (!trfList.Contains(_grabbed)) {
                            trfList.Add(_grabbed);
                        }
                        if (_grabbed) {
                            if (_grabbed.parent == _otherHand) {
                                _otherHand.GetComponent<hand>().LoseControl();
                            }
                            _grabbedParent = null;
                            _grabbed.parent = transform;
                        }
                    }
                }
            }

            //grabbing release
            if (!IsFist()) {
                if (_grabbed) {
                    _grabbed.parent = _grabbedParent;
                    LoseControl();
                }
                _isFist = false;
            }
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

    //Vector3 SetAxis() {
    //    if (isRightHand) {
    //        return transform.up;
    //    }
    //    return Vector3.up;
    //}

    bool IsAiming() {
        return !(OVRInput.Get(OVRInput.Touch.SecondaryIndexTrigger) || OVRInput.Get(OVRInput.NearTouch.SecondaryIndexTrigger));
    }

    bool IsShooting() {
        if (isRightHand) {
            if (IsAiming()) {
                return OVRInput.GetDown(OVRInput.Button.SecondaryHandTrigger);
            }
        }
        return false;
    }

    bool IsInShootingGesture() {
        if (isRightHand) {
            if (IsAiming()) {
                return OVRInput.Get(OVRInput.Button.SecondaryHandTrigger);
            }
        }
        return false;
    }

    bool IsConfirmed() {
        if (isRightHand) {
            return OVRInput.GetDown(OVRInput.Button.SecondaryHandTrigger);
        }
        return false;
    }

    bool IsAxis2Touched() {
        if (isRightHand) {
            return OVRInput.Get(OVRInput.Touch.SecondaryThumbstick);
        } else {
            return false;
        }
    }

    void SetJoyStick1DVal() {
        if (isRightHand) {
            _joyStick1DVal += OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).x * Time.deltaTime;
            if (_joyStick1DVal > _joyStick1DValMax) {
                _joyStick1DVal = _joyStick1DValMax;
            } else if (_joyStick1DVal < 0) {
                _joyStick1DVal = 0;
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

    //bool IsBothFist() {
    //    bool index = OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger) && OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger);
    //    bool middle = OVRInput.Get(OVRInput.Button.SecondaryHandTrigger) && OVRInput.Get(OVRInput.Button.PrimaryHandTrigger);
    //    bool thumb = OVRInput.Get(OVRInput.Touch.SecondaryThumbRest) || OVRInput.Get(OVRInput.Touch.One) || OVRInput.Get(OVRInput.Touch.Two);
    //    thumb = thumb && (OVRInput.Get(OVRInput.Touch.PrimaryThumbRest) || OVRInput.Get(OVRInput.Touch.Three) || OVRInput.Get(OVRInput.Touch.Four));
    //    return index && middle && thumb;
    //}

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
                _editTrf = _grabbed.GetComponent<planet_behavior>().OnRelease(this);

                //TODO: update state from _editTrf
                if (_editTrf.State == LunaState.OnClass) {
                    _myState = State.OnClass;
                } else if (_editTrf.State == LunaState.OnObject) {
                    //TODO:
                    //_selection = lunaFunction.getselection(0)
                    _isSelected = false;
                    _confirmed = false;
                    _myState = State.OnObject;

                }
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

    public void SetPointingTrf(Transform trf) {
        _pointingTrf = trf;
    }
}
