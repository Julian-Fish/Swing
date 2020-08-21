using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
	public PolygonCollider2D poly;
	RaycastHit2D raycastHit;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
		Debug.Log(Mathf.Sin(Vector2.Angle(this.transform.position, poly.points[4]) * Vector2.Distance(this.transform.position, poly.points[4])));
	}
}
