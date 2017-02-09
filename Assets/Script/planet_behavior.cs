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



    protected bool self_init(GameObject target) {
        _current_trail = target.GetComponent<planet_trail>();
        _center = target.transform.position;
        _trail_rotation = target.transform.rotation;
        _xradius = _current_trail.XRadius;
        _yradius = _current_trail.YRadius;
        //if it has parent, scale it
        transform.position = _trail_rotation * (_yradius * Vector3.up) + _center;
        return true;
    }

    protected void trail_rotate(float xradius, float yradius, float spd, Vector3 center) {
        Vector3 pos_vector =new Vector3(xradius * Mathf.Sin(spd * Time.time), yradius * Mathf.Cos(spd * Time.time), 0f);
        _trail_rotation = target_trail.transform.rotation;
        pos_vector = _trail_rotation * (pos_vector + center);
        Transform _parent = target_trail.transform;
        while (_parent.parent != null)
        {
           pos_vector *= _parent.parent.localScale.x;
            _parent = _parent.parent;
        }
        transform.position = center + pos_vector;
    }

    protected void scale_me() {
        transform.localScale = my_scale * Vector3.one;
    }
}

