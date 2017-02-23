using UnityEngine;
using System.Collections;

public class TouchSimulator : MonoBehaviour
{


    void Start()
    {

    }


    void Update()
    {
        GetComponent<LineRenderer>().SetPositions(my_positions());
    }

    Vector3[] my_positions()
    {
        RaycastHit _hit;
        Ray _ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Vector3 start_point = Camera.main.transform.position+new Vector3(1,0,0);
        Vector3 end_point = Camera.main.transform.position + _ray.direction * 10f;
        GetComponent<LineRenderer>().SetColors(Color.blue, Color.blue);
        if (Physics.Raycast(_ray, out _hit))
        {
             //Debug.Log("I hit " + _hit.collider.gameObject.name);
            end_point = _hit.point;
            GetComponent<LineRenderer>().SetColors(Color.red, Color.red);
            if (Input.GetMouseButtonDown(0)) {
                if (_hit.collider.gameObject.tag == "classui")
                {
                    _hit.collider.gameObject.SendMessage("set_active",GameObject.Find("TestEarth").transform);
                }
                if (_hit.collider.gameObject.tag == "Grabbable")
                {
                    _hit.collider.gameObject.SendMessage("OnGrab");
                }
            }
        }
        Vector3[] start_end = new Vector3[2] { start_point, end_point };
        return start_end;
    }

    
}
