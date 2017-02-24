using UnityEngine;
using System.Collections;

public class ClassUI : MonoBehaviour {
    public string attribute_name;
    public bool cur_attr;

    private Shader _origin;

    public bool set_active(Transform edit_trf) {
        if (edit_trf == null) {
            Debug.LogWarning("this should not be shown.");
            return false;
        }
        edit_trf.SendMessage("activate",attribute_name);
        return true;
    }

    void hightlight() {
        //if this attribute is added, set the UI shader to highlight;
        //else set it back to origin

    }

}
