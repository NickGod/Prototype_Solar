using UnityEngine;
using System.Collections;

public class ClassUI : MonoBehaviour {
    public string attribute_name;

    public bool set_active(Transform edit_trf) {
        if (edit_trf == null) {
            Debug.LogWarning("this should not be shown.");
            return false;
        }
        edit_trf.SendMessage("activate",attribute_name);
        return true;
    }

}
