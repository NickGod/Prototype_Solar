using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class earth_behavior : planet_behavior {


	void Start () {
        self_init(target_trail);
    }
	
	
	void Update () {
        trail_rotate(_xradius, _yradius, public_spin_spd, _center);
        transform.Rotate(self_spin_spd* Vector3.up);
        //cloud spin
        transform.GetChild(0).Rotate(self_spin_spd * (-0.2f+0.1f * Random.value) * Vector3.up);
        transform.GetChild(1).LookAt(Camera.main.transform);
        scale_me();

        switch (my_type) {
            case planet_type.class_model:
                //class Model behavior
                return;
            case planet_type.real_planet:
                //real planet behavior
                update_UI();
                return;
            default:
                return;
        }   

    }

    void update_UI()
    {
        Text earth_specs;
        earth_specs = GetComponentInChildren<Text>();
        earth_specs.text = "Size:" + my_scale.ToString("F2") + " Spin Speed:" + self_spin_spd.ToString("F2");
    }
}
