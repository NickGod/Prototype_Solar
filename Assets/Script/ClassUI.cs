using UnityEngine;
using System.Collections;

public class ClassUI : MonoBehaviour {
    public string attribute_name;

    bool set_active(Transform edit_trf) {
        edit_trf.SendMessage("activate",attribute_name);
        return true;
    }

}
