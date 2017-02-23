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
    static State _myState = State.Idle;//TODO: change back to Prepare after testing

    //aiming and grabbing/pointing
    bool _isFist = false;
    bool _isGrabbing = false;
    static float distance = 10.0f;

    //joystick
    float _joyStick1DVal = 0.0f;
    float _joyStick1DValMax = 1.0f;
    static string _selection;
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
            if (_myState == State.OnObject) {
                if (!_confirmed) {
                    //on selecting which attribute to change
                    float val = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).x * Time.deltaTime;
                    if (!_isSelected) {
                        if (val > 0.0f) {
                            _isSelected = true;
                            _selection = _editTrf.GetComponent<planet_behavior>().current_attribute(1);
                            //INDO
                        } else if (val < 0.0f) {
                            _isSelected = true;
                            _selection = _editTrf.GetComponent<planet_behavior>().current_attribute(-1);
                            //INDO
                        }
                    }
                    if (val == 0.0f) {
                        _isSelected = false;
                    }
                } else {
                    //on changing value of the attribute
                    SetJoyStick1DVal();
                    _editTrf.GetComponent<planet_behavior>().change_attribute(_selection, _joyStick1DVal);
                    //INDO: call luna function with parameter _selection and value
                }
                if (IsConfirmed()) {
                    _confirmed = !_confirmed;
                }
                
            }
            

            //INDO: shooting
            if (IsAiming() && isIndexFound) {
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
                            //INDO: point planet from orbit 
                            GameObject underCtrObj = hit.collider.gameObject;
                            //INDO: check the type of planet and enable control only when
                            // it is a real planet object
                            if (underCtrObj.tag == Tags.Grabbable && !_grabbed
                                && underCtrObj.GetComponent<planet_behavior>().my_type == planet_behavior.planet_type.real_planet) {
                                _pointingTrf = underCtrObj.transform;
                                _pointingTrf = _pointingTrf.GetComponent<planet_behavior>().OnGrab();
                                Debug.Log("pointint and dragging");
                            }
                            
                        } else if (_myState == State.OnClass) {
                            //INDO: 1. selecting attribute to the current editing class
                            Transform _hittedStuff = hit.collider.transform;
                            if (_hittedStuff.tag == Tags.ClassUI) {
                                _hittedStuff.GetComponent<ClassUI>().set_active(_editTrf);
                            }
                            //2. choosing button to either creating a new class or modifying current class
                            if (_hittedStuff.tag == Tags.Button) {
                                //if (buttonname == "save") {
                                    _editTrf.GetComponent<planet_behavior>().save_class();
                                //} else if (buttonname == "create") {
                                //    //TODO: create a new class
                                //}
                                _editTrf = null;
                                _myState = State.Idle;
                            }
                        } else if (_myState == State.OnObject) {
                            //INDO: clicking button of "instantiate"
                            //if (clicking button) {
                                Trailmanager.instance.send_to_trail(_editTrf.GetComponent<planet_behavior>(), this);
                                _myState = State.Idle;
                                _editTrf = null;
                            //}
                        }
                    }
                }
            } else {
                lineRender.enabled = false;
            }
        }
        if (_myState == State.Idle) {
            if (isRightHand) {
                if (_pointingTrf) {
                    RaycastHit hit;
                    Ray draggingRay = new Ray(_rightIndex.position, _rightIndex.right);

                    Vector3 _target;
                    if (Physics.Raycast(draggingRay, out hit)) {
                        GameObject _hitObj = hit.collider.gameObject;
                        if (_hitObj.tag == Tags.Reachable) {
                            _target = _hitObj.transform.position;
                        } else {
                            _target = _rightIndex.position + _rightIndex.right * distance;
                        }
                    } else {
                        _target = _rightIndex.position + _rightIndex.right * distance;
                    }
                    //INDO: update pointTrf position and check if it should be deleted
                    _pointingTrf.GetComponent<planet_behavior>().move_towards(_target);
                }

                //pointing and dragging release
                if (!IsInShootingGesture() && _pointingTrf) {
                    _pointingTrf.GetComponent<planet_behavior>().OnRelease(this);
                    _pointingTrf = null;
                }
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
    bool test = false;
    int count = 0;
    bool IsInShootingGesture() {
        if (isRightHand) {
            if (IsAiming()) {
                if (OVRInput.Get(OVRInput.Button.SecondaryHandTrigger)) {
                    return true;
                }
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
            _joyStick1DVal = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).x * Time.deltaTime;
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
                _editTrf = _grabbed.GetComponent<planet_behavior>().OnRelease(this);
                
                if (_editTrf.GetComponent<planet_behavior>().my_type == planet_behavior.planet_type.class_change) {
                    _myState = State.OnClass;
                    if (isRightHand) {
                        _otherHand.GetChild(0).gameObject.SetActive(false);
                    }
                } else if (_editTrf.GetComponent<planet_behavior>().my_type == planet_behavior.planet_type.manipulating) {
                    //INDO:
                    _selection = _editTrf.GetComponent<planet_behavior>().current_attribute(0);
                    _isSelected = false;
                    _confirmed = false;
                    _myState = State.OnObject;
                    if (isRightHand) {
                        _otherHand.GetChild(0).gameObject.SetActive(false);
                    }
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
