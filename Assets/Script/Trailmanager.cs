using UnityEngine;
using System.Collections;

public class Trailmanager : MonoBehaviour {
    const int MAXIMUM_COUNT=6;
    public float detection_dist;
    public int trail_count;
    public GameObject inventory;

    //================singleton================
    public static Trailmanager instance = null;
	void Awake () {
        if (instance == null)
        {
            instance = this;
        }
        else
            Destroy(gameObject);
	}
	//================singleton================
    
	
	void Start () {
        trail_count = 1;
        detection_dist = 0.8f;
        if (!inventory)
            inventory = GameObject.Find("Small_version");
	}

    //======================test use===============================
    void Update() {
        test_generate();

    }

    #region test functions
    void test_generate() {
        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    GameObject new_planet = Instantiate(GameObject.Find("Earth"));

        //    //If I failed to sent it onto trail,destroy myself
        //    if (!send_to_trail(new_planet.GetComponent<planet_behavior>()))
        //        Destroy(new_planet);
        //}
    }

    #endregion


    //init planet acoording to parameters of _pt
    public bool send_to_trail(planet_behavior _pb,hand this_hand)
    {
        GameObject _pt = search_blank();
        if (_pt == null)
            _pt = create_new_trail();

        if (_pt == null)
        {
            Destroy(_pb.gameObject);
            this_hand.GetOutOfList(_pb.gameObject.transform);
            return false;
        }

        _pb.self_init(_pt);
        _pb.my_type = planet_behavior.planet_type.real_planet;
        return true;

    }


    //search for empty trails
    GameObject search_blank() {
        planet_trail[] _pts = GetComponentsInChildren<planet_trail>();
        foreach (planet_trail _pt in _pts) {
            if (_pt.occupied == false) {
                return _pt.gameObject;
            }
        }
        return null;
    }


    //create a new trail according to index
    GameObject create_new_trail() {
        int _ind = transform.childCount;
        if (_ind > 6) {
            Debug.LogWarning("Too many trails, exceed the upper bound.");
            return null;
        }
        GameObject _trail = Instantiate(transform.GetChild(0).gameObject);
        
        //calculate parameters for new trail
        planet_trail last_trail = transform.GetChild(_ind - 1).GetComponent<planet_trail>();
        planet_trail new_trail = _trail.GetComponent<planet_trail>();
        float last_x = last_trail.XRadius;
        float last_y = last_trail.YRadius;

        new_trail.XRadius =last_x + 0.5f + 0.2f * Random.value;
        new_trail.YRadius =last_y + last_y/last_x *(0.5f+ 0.2f * Random.value);
        _trail.transform.localRotation=Random.rotation;
        _trail.transform.position = transform.GetChild(0).position;
        _trail.transform.parent = transform;
        return _trail;
    } 
}
