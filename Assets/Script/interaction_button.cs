using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class interaction_button : MonoBehaviour {

    public string my_function;
    public void OnActivated(Transform target) {
        UIManager.instance.UI_messange_translator(my_function, target);
    }
}
