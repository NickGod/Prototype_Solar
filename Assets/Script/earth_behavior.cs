using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class earth_behavior : planet_behavior {
    
	void Start () {
        self_init(target_trail);
    }
	
	
	void Update () {
        highlight();
        transform.GetChild(0).Rotate(self_spin_spd * (-0.2f + 0.1f * Random.value) * Vector3.up);
        transform.GetChild(1).LookAt(Camera.main.transform);
        switch (my_type) {
            case planet_type.class_model:
                rotate_on_trail();
                transform.GetChild(3).Rotate(self_spin_spd * Vector3.up);
                if (Input.GetKeyDown(KeyCode.L)) {
                    OnGrab().GetComponent<earth_behavior>().highlighted=true;
                }
                return;
            case planet_type.in_hand:
                float inter_scale= interpolation(Trailmanager.instance.detection_dist, Trailmanager.instance.inventory);
                transform.localScale = Vector3.one * inter_scale;
                if (Input.GetKeyDown(KeyCode.M))
                {
                   // OnRelease(null).GetComponent<earth_behavior>().highlighted = true;
                }
                return;
            case planet_type.manipulating:
                transform.GetChild(3).Rotate(self_spin_spd * Vector3.up);
                update_UI();
                scale_me();
                if (Input.GetKeyDown(KeyCode.N))
                {
                    Trailmanager.instance.send_to_trail(this);
                }
                return;
            case planet_type.real_planet:
                //** Add one condition on whether it is on hand
                transform.GetChild(3).Rotate(self_spin_spd * Vector3.up);
                rotate_on_trail();
                update_UI();
                return;
            default:
                return;
        }   

    }

    void update_UI(bool specs=true)
    {
        Text earth_specs;
        earth_specs = GetComponentInChildren<Text>();
        if (specs)
            earth_specs.text = "Size:" + my_scale.ToString("F2") + " Spin Speed:" + self_spin_spd.ToString("F2");
        else
            earth_specs.text = "MyPlanet";
    }

    void rotate_on_trail() {
        trail_rotate(_xradius, _yradius, public_spin_spd);
    }
}
