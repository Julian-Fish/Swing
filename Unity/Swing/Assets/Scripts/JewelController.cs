using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JewelController : MonoBehaviour
{
	static public JewelController Instance;

	public Transform lengthShift;

	void Awake()
	{
		Instance = this;
	}

	void Start()
    {
        
    }

    void Update()
    {

	}
}
