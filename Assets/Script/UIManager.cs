using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {
    public static UIManager instance = null;
    public GameObject UIParent_class;
    public GameObject UIParent_object;
    
    void Awake () {
        if (instance == null)
        {
            instance = this;
        }
        else
            Destroy(gameObject);
	}
	
	
	public void UI_messange_translator (string msg,Transform target) {
        //parse the message got from UI and pass it to target transform
        switch (msg){
            //should target be grabbale only?
            case "instantiate":
                //send to trail
                return;
            case "save_class":
                target.GetComponent<planet_behavior>().save_class(true);
                return;
            case "create_class":
                target.GetComponent<planet_behavior>().save_class(false);
                return;
               
            default:
                Debug.LogWarning("InRecognizable UI message.");
                return;
        }

	}

    public void UI_switch(int phase_index) {
        //controls UI-group on/off
        switch (phase_index) {
            case 0:
                //only sun
                return;
            case 1:
                //class modification
                UIParent_class.SetActive(true);
                UIParent_object.SetActive(false);
                return;
            case 2:
                //object modification
                UIParent_object.SetActive(true);
                UIParent_class.SetActive(false);
                return;
            default:
                Debug.Log("Nothing of UI to be changed.");
                return;
        }

    }
}
