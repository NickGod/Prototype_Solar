using UnityEngine;
using System.Collections;

public class planet_trail : MonoBehaviour {    
    public int SegmentCount;
    [Range(0f,8f)]
    public float XRadius;
    [Range(0f, 10f)]
    public float YRadius;
    public bool occupied;
    

    private LineRenderer _MyTrail;
    
    void Awake () {
        _MyTrail = GetComponent<LineRenderer>();
        occupied = false;
	}
     /// <summary>
     /// generate an eclipse trail with X line segments
     /// following formula (_CenterPosition + a * sin(theta), CenterPosition + b * cos (theta) )
     /// </summary>
    void genenrate_trail() {
        Vector3 _CenterPosition = transform.position;
        _MyTrail.SetVertexCount(SegmentCount + 1);
        _MyTrail.SetPositions(get_eclipse(XRadius, YRadius, SegmentCount, _CenterPosition));
        Color my_color = Random.ColorHSV();
        _MyTrail.SetColors(my_color, my_color);
    }

    /// <summary>
    /// get the interpolate positions for elipse with the first and the last node the same vector
    /// to get a closed shape;
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="x"></param>
    /// <returns></returns>
    Vector3[] get_eclipse(float a, float b, int x, Vector3 center_pos) {
        float _delta =2 * Mathf.PI / x;
        Vector3[] interpolates = new Vector3[x+1];
        for (int i = 0; i < x; i++) {
            float _theta = i * _delta;
            interpolates[i] = new Vector3( a * Mathf.Sin(_theta), b * Mathf.Cos(_theta), 0);
        }
        interpolates[x] = interpolates[0];
        return interpolates;
    } 
	
	// Update is called once per frame
	void Start() {
        genenrate_trail();
	}
}
