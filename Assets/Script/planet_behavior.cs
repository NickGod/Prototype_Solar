using UnityEngine;
using System.Collections;

public class planet_behavior : MonoBehaviour {
    public GameObject target_trail;
    [Range(0.5f, 1.5f)]
    public float my_scale;
    [Range(-5f,5f)]
    public float self_spin_spd;
    [Range(0.2f, 1f)]
    public float public_spin_spd;
    public enum planet_type {class_model,in_hand,manipulating, real_planet};
    public planet_type my_type;
    public bool highlighted;

    // put myself on trail
    // moving on the trail
    // self-spin
    protected planet_trail _current_trail;
    protected float _xradius;
    protected float _yradius;
    protected Vector3 _center;
    protected Quaternion _trail_rotation;
    protected float _start_time;

    private float inve_scale=0.2f;


    #region public methods
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
            me_clone.position = transform.position;
            me_clone.rotation = transform.rotation;
            Transform _parent = transform;
            //multiply parents' scale
            while (_parent.parent != null)
            {
                me_clone.localScale *= _parent.parent.localScale.x;
                _parent = _parent.parent;
            }
            me_clone.GetComponent<planet_behavior>().my_type = planet_type.in_hand;
            me_clone.GetComponent<planet_behavior>().target_trail = null;
            return me_clone;
        }
        else if (my_type == planet_type.real_planet)
        {
            return transform;
        }
        //if it is manipulating, becomes in_hand
        my_type = planet_type.in_hand;
        return transform;  
    }
    /// <summary>
    /// thing in hand responds to when release the trigger
    /// </summary>
    /// <param name="is_editing"></param>
    /// <returns></returns>
    public Transform OnRelease(Transform edit_trf, hand hand_to_call) {
        switch (my_type) {
            case planet_type.in_hand:
                //judge where to go according to is_editing,
                //and distance ( we could check the calculated sale instead)
                if (edit_trf == null)
                {
                    //should go to manipulate spot
                    if (Mathf.Abs(transform.localScale.x - my_scale) < 1e-5)
                    {
                        my_type = planet_type.manipulating;
                        //func/coroutine? fly_to(target point)
                        transform.position = 0.5f*Vector3.one;
                        Debug.Log("Goes to manipulate.");
                        return transform;
                    }
                    else
                    {
                        Destroy(gameObject);
                        hand_to_call.GetOutOfList(gameObject.transform);
                        return edit_trf;
                    }
                }
                //func fly_to(target_point)
                Destroy(gameObject);
                hand_to_call.GetOutOfList(gameObject.transform);
                Debug.Log("Goes back to inventory.");
                return edit_trf;
        

            // if I grab planet from orbit, do nothing to the flag.
            //switch in case protection needed 
            default:
                return edit_trf;
        }
    } 

    #endregion

    #region protected methods
    float mul_vector = 0.02f;
    protected void trail_rotate(float xradius, float yradius, float spd) {
        if (target_trail)
        {

            _trail_rotation = target_trail.transform.rotation;
            Vector3 pos_vector = new Vector3(xradius * Mathf.Sin(spd * Time.time), yradius * Mathf.Cos(spd * Time.time), 0f);
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

    protected void scale_me() {
        transform.localScale = my_scale * Vector3.one;
    }

    protected void highlight(){
        Material mat = GetComponent<MeshRenderer>().material; 
        mat.shader = highlighted?Shader.Find("Custom/RimSelection") : Shader.Find("Standard");
    }

    protected float interpolation(float max_distance, GameObject inventory) {
        //get linear interpolation between current scale and final scale

        float c_scale =inve_scale * my_scale;
        float _slope = (my_scale - c_scale) / max_distance;
        if (inventory.activeInHierarchy == false)
            return my_scale;
        float _dis = Vector3.Distance(inventory.transform.position, transform.position);
        return _dis >= max_distance ? my_scale: (c_scale+_slope * _dis);
    }
    #endregion
}

