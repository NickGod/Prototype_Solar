using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class hand : MonoBehaviour {

    float _spinSpeed = 0.0f;
    Quaternion _objRotation;
    public Transform _otherHand;

    float _inventoryTimer = 0;
    float _inventoryTime = 0.2f;
    static bool _isInventory = false;

    List<Transform> trfList = new List<Transform>();
    Transform _grabbedParent;
    Transform _grabbed;

    static Transform _onEditting = null;

    //States
    bool _isFist = false;
    bool _isGrabbing = false;
    //resizing
    static float _originDistance = 0.0f;
    static bool _onResizing = false;

    public bool RightHand;
    // Update is called once per frame

    void Update() {
        if (!RightHand) {
            //for left hand
            transform.localPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch);
            transform.localRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.LTouch);


            //calling inventory based on left hand index
            if (IsInventoryUp()) {
                _inventoryTimer += Time.deltaTime;
            } else {
                _inventoryTimer = 0.0f;
                _isInventory = false;
                Debug.Log("Inventory off");
                //TODO: inventory off animation and related mehanics here
            }
            if (_inventoryTimer >= _inventoryTime && !_isInventory) {
                // this function only call once for each index up
                //Debug.Log("Inventory on");
                //TODO: inventory on animation and related mechanics here
                _isInventory = true;
            }
        } else {
            //for right hand
            transform.localPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
            transform.localRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);


            //resizing
            if (IsBothFist() && _onEditting && !_onResizing) {
                _onResizing = true;
                _originDistance = Vector3.Distance(transform.position, _otherHand.position);
            } 
        
            if (_onResizing) {
                _onEditting.localScale = Vector3.Distance(transform.position, _otherHand.position) / _originDistance * Vector3.one;
            }

            if (!IsBothFist()) {
                _originDistance = 0.0f;
                _onResizing = false;
            }
            
            //Axis rotation
            if (_onEditting) {
                SetAxis();
            }
            
            //Rotating speed
            if (IsAxis2Touched() && _onEditting) {
                SetSpinSpeed();
            }
            
            //test
            if (IsAxis2Touched()) {
                SetSpinSpeed();
                Debug.Log("Spin: " + _spinSpeed);
            }
            if (IsShooting()) {
                Debug.Log("Shooting");
            }
            if (IsFist()) {
                Debug.Log("Fist");
            }
        }

        //grabbing
        _isGrabbing = false;
        if (IsFist() && !_isFist) {
            _isFist = true;
            _isGrabbing = true;
        }
        if (_isGrabbing) {
            Debug.Log("Naaaa: " + name);
            if (!_onEditting) {
                if (_grabbed == null) {
                    _grabbed = GetClosest();
                    if (_grabbed) {
                        if (_grabbed.parent == _otherHand) {
                            _grabbedParent = _otherHand.GetComponent<hand>().GetGrabbedParent();
                            _otherHand.GetComponent<hand>().LoseControl();
                        } else {
                            _grabbedParent = _grabbed.parent;
                        }
                        _grabbed.parent = transform;
                    }
                }
            } else {
                //TODO: grabbing based on if _onEditting is null and what you are grabbing
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
        if (!RightHand) {
            return !(OVRInput.Get(OVRInput.Touch.PrimaryIndexTrigger) || OVRInput.Get(OVRInput.NearTouch.PrimaryIndexTrigger));
        } else {
            return false;
        }
    }

    void SetAxis() {
        if (RightHand) {
            float yAngle = transform.localRotation.eulerAngles.y;
            _objRotation = Quaternion.AngleAxis(yAngle, Vector3.right);
        }
    }

    bool IsShooting() {
        if (RightHand) {
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
        if (RightHand) {
            return OVRInput.Get(OVRInput.Touch.SecondaryThumbstick);
        } else {
            return false;
        }
    }

    void SetSpinSpeed() {
        if (RightHand) {
            _spinSpeed += OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).x * Time.deltaTime;
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
        if (RightHand) {
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
            //TODO: check the current parent, if it is not hand, then the object will fly
            //to specific spot(either editting center or inventory), then update onEditting info
            _grabbed = null;
            _grabbedParent = null;
        }

    }
}
