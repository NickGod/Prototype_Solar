using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (IsInShootingGesture()) {

        }
	}
    bool sdtest = false;
    int count = 0;
    bool IsAiming() {
        return !(OVRInput.Get(OVRInput.Touch.SecondaryIndexTrigger) || OVRInput.Get(OVRInput.NearTouch.SecondaryIndexTrigger));
    }

    bool IsInShootingGesture() {
        if (IsAiming()) {
            if (OVRInput.Get(OVRInput.Button.SecondaryHandTrigger)) {
                if (sdtest == false) {
                    Debug.Log("in hand trigger");
                    sdtest = true;
                    count = 0;
                }
                count++;
                Debug.Log("IN shooting");
                return true;
            } else {
                if (sdtest) {
                    Debug.Log("not in hand triger");
                    sdtest = false;
                    count = 0;
                }
                count++;
                return false;
            }
        }
        return false;
    }
}
