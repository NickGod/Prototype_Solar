using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class planet_behavior : MonoBehaviour {
   
    static readonly string[] usable_attributes = { "ring", "moon", "color", "size" };

    public GameObject target_trail;
    public enum planet_type {class_model,in_hand,class_change,manipulating,real_planet};
    public planet_type my_type;
    public bool highlighted;

    protected const float self_spin_spd = 0.965f;
    protected const float public_spin_spd = 0.606f;

    // put myself on trail
    // moving on the trail
    // self-spin
    protected planet_trail _current_trail;
    protected float _xradius;
    protected float _yradius;
    protected Vector3 _center;
    protected Quaternion _trail_rotation;

    //instantiated by which class parent
    public Transform come_from;
  
    //whether the attribute activated
    protected Dictionary<string, bool> activate_attribute= new Dictionary<string, bool>();
    protected Dictionary<string, float> attribute_value = new Dictionary<string, float>();
    protected List<string> attrList;
    protected List<Transform> my_children;

    //register myself to parent's children list when instantiated
    //de-register myself when destroyed
    //if I am the class it self, I will delete my self and grab my parent here.

    private float inve_scale=0.2f;
    private int roulette = 0;

    #region public methods
    public bool attribute_init(Dictionary<string, bool> clone_from=null,bool init=true) {

        for (int i = 0; i < usable_attributes.GetLength(0); i++)
        {
            string str = usable_attributes[i];
            activate_attribute[str] = (clone_from == null) ? false : clone_from[str];
        }
        // set default attribute active value
       
        attribute_value["ring"] = 0f;
        attribute_value["moon"] = 0f;
        attribute_value["color"] = 0f;
        attribute_value["size"] = 0f;
        update_attrList();
        update_look();
        return true;
    }

  

    public bool self_init(GameObject target) {
        if (target==null){
            _xradius = 0f;
            _yradius = 0f;
            _center = transform.position;
            _trail_rotation = transform.rotation;
        }
        else {
            target_trail = target;
            _current_trail = target.GetComponent<planet_trail>();
            _xradius = _current_trail.XRadius;
            _yradius = _current_trail.YRadius;
            _center = target.transform.position;
            _trail_rotation = target.transform.rotation;
            _current_trail.occupied = true;
        }
        return true;
    }

    public List<string> update_attrList() {
        if (attrList == null) attrList = new List<string>();
        attrList.Clear();   
        for (int i = 0; i < usable_attributes.GetLength(0); i++)
        {
            string str = usable_attributes[i];
            if (activate_attribute[str] == true) {
                attrList.Add(str);
            }
        }
        return attrList;
    }

    public bool activate(string to_activate) {

        if (activate_attribute.ContainsKey(to_activate))
        {
            activate_attribute[to_activate] = !activate_attribute[to_activate];
            update_attrList();
            update_look();
            //change all my children
            foreach (Transform ch in come_from.GetComponent<planet_behavior>().my_children) {
                ch.GetComponent<planet_behavior>().attribute_init(activate_attribute,false);
                Debug.Log(gameObject.name + "should be changed by" + come_from.name);
                Debug.Log("parent class is being changed.");
            }
            return true;
        }
        Debug.LogError("Should change key values correctly. Check UI names and planet initializations.");
        return false;
    }


    //if the planet is rotating on a trail, then set the trail flat occupied to be false
    public void delete_me() {
        if (target_trail != null) {
            target_trail.GetComponent<planet_trail>().occupied = false;
        }
        Destroy(gameObject);
    }

    /// <summary>
    /// When grab, call this. 
    /// </summary>
    /// <returns>a copy if it is a model, other itself</returns>
    public Transform OnGrab() {
        if (my_type == planet_type.class_model)
        {
            Transform me_clone = Instantiate(gameObject).transform;
            me_clone.parent = null;
            me_clone.GetComponent<planet_behavior>().come_from = transform;
            me_clone.position = transform.position;
            me_clone.rotation = transform.rotation;
            Transform _parent = transform;
            //multiply parents' scale
            while (_parent.parent != null)
            {
                me_clone.localScale *= _parent.parent.localScale.x;
                _parent = _parent.parent;
            }
            //initialize attribute value
            me_clone.GetComponent<planet_behavior>().attribute_init(activate_attribute);
            me_clone.GetComponent<planet_behavior>().my_type = planet_type.in_hand;
            me_clone.GetComponent<planet_behavior>().target_trail = null;
            return me_clone;
        }
        else if (my_type == planet_type.real_planet)
        {
            mul_vector = 0.0f;
            //CHANGE THIS TO MOVE TO SOME SPOT
            return transform;
        }
        //if it is manipulating, becomes in_hand
        Debug.LogError("Impossible Call!!");
        return null;  
    }
    /// <summary>
    /// thing in hand responds to when release the trigger
    /// </summary>
    /// <param name="is_editing"></param>
    /// <returns></returns>
    public Transform OnRelease(hand hand_to_call) {
        switch (my_type) {
            case planet_type.in_hand:
                //judge where to go according to is_editing,
                //and distance ( we could check the calculated sale instead)
                //should go to manipulate spot
                transform.parent = null;
                transform.localScale = Vector3.one;
                if (transform.position.x<-0.2f)
                {
                    my_type = planet_type.class_change;
                    UI_Manager.instance.UI_switch(1);
                    hand_to_call.GetOutOfList(gameObject.transform);
                    Debug.Log("Goes to class change.");
                    transform.position = GameObject.Find("edit_spot_class").transform.position + 0.3f*Vector3.up;
                    transform.localScale = 0.3f*Vector3.one;
                    return transform;
                }
                else
                {
                    my_type = planet_type.manipulating;
                    //add myself from my parent
                    UI_Manager.instance.UI_switch(2);
                    come_from.GetComponent<planet_behavior>().my_children.Add(transform);
                    Debug.Log("I am added in to my parent:" + come_from.name);
                    hand_to_call.GetOutOfList(gameObject.transform);
                    Debug.Log("Goes to manipulate.");
                    transform.position = GameObject.Find("edit_spot_changing").transform.position + Vector3.up;
                    return transform;
                }      

            case planet_type.real_planet:
                mul_vector = 0.02f;
                return transform;
            // if I grab planet from orbit, do nothing to the flag.
            //switch in case protection needed 
            default:
                return transform;
        }
    }

    public bool change_attribute(string name, float val) {
        if (name == null) {
            Debug.Log("Nothing is being changed.");
            return false;
        }
        if (!activate_attribute.ContainsKey(name)) {
            Debug.LogWarning("Attribute does not exist.");
            return false;
        }
        if (activate_attribute[name] == false) {
            Debug.LogWarning("Attribute is not added, check roulette.");
            return false;
        }
        float temp = attribute_value[name];
        temp += val;
        attribute_value[name] =Mathf.Clamp (temp,0f,1f);
        update_value(name);
        return true;
    }

    public string current_attribute(int val) {

        if (attrList.Count == 0)
            return null;
        else
            roulette += val;
            return attrList[roulette % attrList.Count];        
    }

    public void save_class(bool overwrite = false) {
        if (overwrite)
        {
            come_from.GetComponent<planet_behavior>().attribute_init(activate_attribute);
            Destroy(gameObject);
        }
        else if (Trailmanager.instance.inventory.transform.childCount < 4)
        {
            //create new class
            transform.SetParent(Trailmanager.instance.inventory.transform);
            self_init(Trailmanager.instance.inventory.transform.GetChild(0).gameObject);
            transform.localScale = Trailmanager.instance.inventory.transform.GetChild(1).localScale;
            offset = 1.7f * (Trailmanager.instance.inventory.transform.childCount - 2);
            my_type = planet_type.class_model;
            revert_changes();
            come_from = null;
            transform.localPosition = Vector3.zero;
        }
        else {
            //=+++++++++++before destroying it ,revert all parent's children list status
            Destroy(gameObject);
            revert_changes();
            Debug.Log("Not enough spot for classes.");
        }
    }

    public void move_towards(Vector3 taret_pos) {
        transform.position += 0.05f*(taret_pos - transform.position);

    }
    #endregion

    #region protected methods
    float mul_vector = 0.02f;
    float offset = 0f;
    protected void trail_rotate(float xradius, float yradius, float spd) {
        if (target_trail)
        {

            _trail_rotation = target_trail.transform.rotation;
            Vector3 pos_vector = new Vector3(xradius * Mathf.Sin(spd * Time.time+offset), yradius * Mathf.Cos(spd * Time.time+offset), 0f);
            Transform _parent = target_trail.transform;
            //multiply parents' scale
            while (_parent.parent != null)
            {
                pos_vector *= _parent.parent.localScale.x;
                _parent = _parent.parent;
            }
            pos_vector = _trail_rotation * pos_vector;
            _center = target_trail.transform.position;

            transform.position += mul_vector * (_center + pos_vector - transform.position);
        }

    }

    private void update_value(string str)
    {
        if (activate_attribute[str] == true)
          //mapping the values in to attribute
            switch (str)
            {
                case "moon":
                    update_my_moon();
                    return;
                case "ring":
                    update_my_ring();
                    return;
                case "color":
                    update_my_color();
                    return;
                case "size":
                    update_my_size();
                    return;
                default:
                    return;
            }
        
    }

    private void revert_changes() {
        foreach (Transform ch in come_from.GetComponent<planet_behavior>().my_children) {
            ch.GetComponent<planet_behavior>().attribute_init(come_from.GetComponent<planet_behavior>().activate_attribute, false);
            Debug.LogWarning("Revert all my parent's children");
        }
    }

    //} ============================================obsolete============================
    //protected float interpolation(float max_distance, GameObject inventory) {
    //    //get linear interpolation between current scale and final scale

    //    float c_scale =inve_scale * my_scale;
    //    float _slope = (my_scale - c_scale) / max_distance;
    //    if (inventory.activeInHierarchy == false)
    //        return my_scale;
    //    float _dis = Vector3.Distance(inventory.transform.position, transform.position);
    //    return _dis >= max_distance ? my_scale: (c_scale+_slope * _dis);
    //} ============================================obsolete============================
    #endregion


    #region virtual methods
    protected virtual void update_my_moon() { }

    protected virtual void update_my_ring() { }

    protected virtual void update_my_color() { }

    protected virtual void update_my_size() { }

    protected virtual void highlight(){ }

    protected virtual void update_look() { }


    #endregion

    void OnDestroy()
     {
        if (come_from == null)
        {
            Debug.Log("Something does not have parent class is destroying..check the code");
        }
        else {
            List<Transform> parent_children = come_from.GetComponent<planet_behavior>().my_children;
            if(parent_children.Contains(transform))
                parent_children.Remove(transform);
        }        

      }
    
}

