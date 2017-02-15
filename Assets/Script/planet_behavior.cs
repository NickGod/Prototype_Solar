﻿using UnityEngine;
using System.Collections;

public class planet_behavior : MonoBehaviour {
    public GameObject target_trail;
    [Range(0.5f, 1.5f)]
    public float my_scale;
    [Range(0f,10f)]
    public float self_spin_spd;
    [Range(0.2f, 1f)]
    public float public_spin_spd;
    public enum planet_type {class_model, real_planet};
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

    private int _rotator=0;

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

    #endregion
}

