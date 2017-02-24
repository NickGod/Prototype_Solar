using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class earth_behavior : planet_behavior {
    public static readonly Color[] mycolors = { Color.white, Color.red, Color.blue, Color.green };
    void Awake() {
        attribute_init();
        my_children = new List<Transform>();
    }

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
                transform.GetChild(3).Rotate(-self_spin_spd * Vector3.up);
                if (Input.GetKeyDown(KeyCode.L)) {
                    OnGrab().GetComponent<earth_behavior>().highlighted=true;
                }
                return;
            case planet_type.in_hand:
                
                return;
            case planet_type.class_change:
                //behavior when changing class
                update_UI(1);
                return;

            case planet_type.manipulating:
                transform.GetChild(3).Rotate(-self_spin_spd * Vector3.up);
                update_UI(2);
                if (Input.GetKeyDown(KeyCode.A)) {
                    change_attribute("moon", 0.2f);
                }           
                return;

            case planet_type.real_planet:
                //** Add one condition on whether it is on hand
                transform.GetChild(3).Rotate(-self_spin_spd * Vector3.up);
                rotate_on_trail();
                update_UI(2);
                if (Vector3.Distance(Trailmanager.instance.blackhole.transform.position, transform.position) < 2f) {
                    
                    delete_me();        
                }
                return;
            default:
                return;
        }   

    }

    void update_UI(int specs=0)
    {
        Text earth_specs;
        earth_specs = GetComponentInChildren<Text>();
        if (specs == 1)
        {
            string buffer = "Activating:";
            foreach (string str in attrList)
            {
                buffer += str;
                buffer += " ";
            }
            earth_specs.text = buffer;
        }
        else if (specs == 2) {
            string buffer = " ";
            foreach (string str in attrList)
            {
                buffer += str;
                buffer += ":" + attribute_value[str].ToString("F4")+" ";
            }
            earth_specs.text = buffer;
        }
        else
            earth_specs.text = "MyPlanet";
    }

    void rotate_on_trail() {
        trail_rotate(_xradius, _yradius, public_spin_spd);
    }

    protected override void highlight()
    {
        Material mat =transform.GetChild(3).GetComponent<MeshRenderer>().material;
        mat.shader = highlighted ? Shader.Find("Custom/RimSelection") : Shader.Find("Standard");
    }

    protected override void update_my_ring()
    {
        transform.FindChild("Rings").Rotate(3f* attribute_value["ring"]*Vector3.left); 
    }

    protected override void update_my_moon()
    {
        //CHANGE HERE update moon numbers accordingly
        Transform moons = transform.FindChild("Moons");
        if (attribute_value["moon"] < 0.33f)
        {
            moons.GetChild(0).gameObject.SetActive(true);
            moons.GetChild(1).gameObject.SetActive(false);
            moons.GetChild(2).gameObject.SetActive(false);
        }
        else if (attribute_value["moon"] < 0.66f)
        {
            moons.GetChild(0).gameObject.SetActive(true);
            moons.GetChild(1).gameObject.SetActive(true);
            moons.GetChild(2).gameObject.SetActive(false);
        }
        else {
            moons.GetChild(0).gameObject.SetActive(true);
            moons.GetChild(1).gameObject.SetActive(true);
            moons.GetChild(2).gameObject.SetActive(true);
        }

    }

   

    protected override void update_my_color()
    {
        int i = Mathf.FloorToInt(attribute_value["color"] / 0.2f) % 4;
        Material mat = transform.GetChild(3).GetComponent<MeshRenderer>().material;
        mat.SetColor("_Color",mycolors[i]);
    }

    protected override void update_my_size() {
        transform.localScale= (1f + 0.5f * +attribute_value["size"]) * Vector3.one;
    }


    protected override void update_look()
    {
        transform.GetChild(4).gameObject.SetActive(activate_attribute["ring"]);
        transform.GetChild(5).gameObject.SetActive(activate_attribute["moon"]);
        if (!activate_attribute["color"]) {
            transform.GetChild(3).GetComponent<MeshRenderer>().material.SetColor("_Color", Color.grey);
        }
    }

}
