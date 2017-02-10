using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class earth_behavior : planet_behavior {


	void Start () {
        self_init(target_trail);
    }
	
	
	void Update () {
        switch (my_type) {
            case planet_type.class_model:
                real_planet_behave();
                return;
            case planet_type.real_planet:
                //real planet behavior
                real_planet_behave();
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

    void real_planet_behave() {
        trail_rotate(_xradius, _yradius, public_spin_spd);
        transform.Rotate(self_spin_spd * Vector3.up);
        //cloud spin
        transform.GetChild(0).Rotate(self_spin_spd * (-0.2f + 0.1f * Random.value) * Vector3.up);
        transform.GetChild(1).LookAt(Camera.main.transform);
        scale_me();
       
    }
}
