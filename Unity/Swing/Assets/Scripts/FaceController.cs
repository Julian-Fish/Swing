using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceController : MonoBehaviour
{
    public static FaceController Instance;

    public Transform eyebrowL_TF;
    public Transform eyebrowR_TF;
    public float maxEyebrowAngle;
    public float maxAngleSpeed;
    float currentEyebrowAngle;

	private void Awake()
	{
        Instance = this;
	}

	// Start is called before the first frame update
	void Start()
    {
        ResetRotate();
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerController.Instance.actionStatus == PlayerController.ActionStatus.SWING)
        {
            // change eyebrwo angle in swing state
            currentEyebrowAngle = Mathf.Lerp(currentEyebrowAngle, maxEyebrowAngle * PlayerController.Instance.GetSpeed() / maxAngleSpeed, 0.1f);
            eyebrowL_TF.localEulerAngles = -1.0f * Vector3.forward * currentEyebrowAngle;
            eyebrowR_TF.localEulerAngles = Vector3.forward * currentEyebrowAngle;
        }
    }

    public void ResetRotate()
    {
        currentEyebrowAngle = 0;
    }
}
