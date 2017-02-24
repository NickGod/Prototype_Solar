using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class interaction_button : MonoBehaviour {

    public string my_function;
    public void OnActivated(Transform target, hand _hand=null) {
        UI_Manager.instance.UI_messange_translator(my_function, target,_hand);    
    }

    IEnumerator highlight() {
        //if this attribute is added, set the UI shader to highlight;
        //else set it back to origin
        transform.GetComponentInChildren<Image>().color += new Color(0.1f, 0.1f, 0.1f);
        yield return new WaitForSeconds(0.1f);
        transform.GetComponentInChildren<Image>().color -= new Color(0.1f, 0.1f, 0.1f);
        UI_Manager.instance.UI_switch(0);
    }
}
