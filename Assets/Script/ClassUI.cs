using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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

    IEnumerator highlight() {
        MeshRenderer[] children_rends = GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < 10; i++) {
            foreach (MeshRenderer _r in children_rends) {
                _r.material.SetColor("_EmissionColor", Color.Lerp(Color.black, Color.white, 1f - Mathf.Abs(i - 5) * 0.2f));
            }
            yield return new WaitForEndOfFrame();
        }
    }

}
