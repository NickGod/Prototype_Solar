using UnityEngine;
using System.Collections;

public class planet_behavior : MonoBehaviour {
    public GameObject target_trail;
    [Range(0.5f, 1.5f)]
    public float my_scale;
    [Range(0.5f,5f)]
    public float self_spin_spd;
    [Range(0.2f, 1f)]
    public float public_spin_spd;
    public enum planet_type {class_model, real_planet};
    public planet_type my_type;

    // put myself on trail
    // moving on the trail
    // self-spin
    protected planet_trail _current_trail;
    protected float _xradius;
    protected float _yradius;
    protected Vector3 _center;
    protected Quaternion _trail_rotation;
    protected float _start_time;

    private LineRenderer _that_renderer;
    private int _rotator=0;



    protected bool self_init(GameObject target) {
        _current_trail = target.GetComponent<planet_trail>();
        _center = target.transform.position;
        _trail_rotation = target.transform.rotation;
        _xradius = _current_trail.XRadius;
        _yradius = _current_trail.YRadius;
        //if it has parent, scale it
        transform.position = _trail_rotation * (_yradius * Vector3.up) + _center;
        _that_renderer = target_trail.gameObject.GetComponent<LineRenderer>();
        return true;
    }

    protected void trail_rotate(float xradius, float yradius, float spd) {
        _trail_rotation = target_trail.transform.rotation;
        Vector3 pos_vector =new Vector3(xradius * Mathf.Sin(spd * Time.time), yradius * Mathf.Cos(spd * Time.time), 0f);
        Transform _parent = target_trail.transform;
        while (_parent.parent != null)
        {
            pos_vector *= _parent.parent.localScale.x;
            _parent = _parent.parent;
        }
        pos_vector = _trail_rotation * pos_vector;
        _center = target_trail.transform.position;
        transform.position =  _center +pos_vector;

    }

    //small_model_rotate will directly put the model on to the line_renderer of the track
    //every call rotator+1
    
    protected void small_model_rotate() {
          //  int max_index = _that_renderer.
    }


    protected void scale_me() {
        transform.localScale = my_scale * Vector3.one;
    }
}

