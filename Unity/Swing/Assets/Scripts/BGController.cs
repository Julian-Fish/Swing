using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGController : MonoBehaviour
{
    public Transform[] bg_TF;
    public Camera mainCamera;
    public float scrollSpeedX;
    public float interval;
	public bool outBG;
    // Start is called before the first frame update
    void Start()
    {
		mainCamera = Camera.main;
        // get childs
        for (int i = 0; i < this.transform.childCount; ++i)
        {
            bg_TF[i] = this.transform.GetChild(i).transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
		if (!outBG)
		{
			this.transform.position = new Vector2(mainCamera.transform.position.x * scrollSpeedX, mainCamera.transform.position.y * 0.7f);

			for (int i = 0; i < bg_TF.Length; ++i)
			{
				if (bg_TF[i].transform.position.x < mainCamera.transform.position.x - interval)
				{
					bg_TF[i].transform.localPosition = new Vector2(bg_TF[i].transform.localPosition.x + 2.0f * interval, 0.0f);
				}
				else if (bg_TF[i].transform.position.x > mainCamera.transform.position.x + interval)
				{
					bg_TF[i].transform.localPosition = new Vector2(bg_TF[i].transform.localPosition.x - 2.0f * interval, 0.0f);
				}
			}
		}
		else
		{
			this.transform.position = new Vector2(mainCamera.transform.position.x, mainCamera.transform.position.y);
		}

	}
}
