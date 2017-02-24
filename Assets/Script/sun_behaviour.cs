using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class sun_behaviour : planet_behavior {

	// Use this for initialization
	void Start () {
        attribute_init();
        activate("color");
        activate("size");
        my_type = planet_type.manipulating;   
	}
	
	// Update is called once per frame
	void Update () {
        switch (my_type)
        {
            case planet_type.manipulating:
                update_UI(1);
                return;
            case planet_type.real_planet:
                trail_rotate(_xradius, _yradius, public_spin_spd);
                return;
            default:
                return;
        }

    }

    void update_UI(int specs = 0)
    {
        Text _specs;
        _specs = GetComponentInChildren<Text>();
        if (specs == 1)
        {
            string buffer = " ";
            foreach (string str in attrList)
            {
                buffer += str;
                buffer += ":" + attribute_value[str].ToString("F2") + " ";
            }
            _specs.text = buffer;
        }
        else
            _specs.text = "MyPlanet";
    }

    protected override void update_my_color()
    {
        int i = Mathf.FloorToInt(attribute_value["color"] / 0.2f) % 4;
        Material mat = transform.GetComponent<MeshRenderer>().material;
        mat.SetColor("_Color", earth_behavior.mycolors[i]);
        mat.SetColor("_EmissionColor", earth_behavior.mycolors[i]);
    }

    protected override void update_my_size()
    {
        transform.localScale = (1f + 0.5f * +attribute_value["size"]) * Vector3.one;
    }

}
