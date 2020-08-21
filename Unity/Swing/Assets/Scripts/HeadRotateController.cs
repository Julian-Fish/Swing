using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadRotateController : MonoBehaviour
{
    public Transform jewel_TF;
    Vector2 baseAxis;
	LineRenderer lineR;
	float rotateZ;

    [SerializeField]
    Vector2 mouseAimDir;
    [SerializeField]
    Vector2 swingDir;
    Vector2 currentAimDir;
    float rotateAngleAim;

    Vector2 mousePosition_Vec2;

    void Start()
    {
        baseAxis = jewel_TF.position - this.transform.position;
        currentAimDir = this.transform.up;
        ResetRotate();
    }

    void Update()
    {
        if (PlayerController.Instance.actionStatus == PlayerController.ActionStatus.SWING && PlayerController.Instance.ropeController.currentPos < 2)
        {
            headSwingRotate();
        }
        else if (PlayerController.Instance.actionStatus == PlayerController.ActionStatus.AIM && PlayerController.Instance.canControl)
        {
            headAimRotate();
        }

        swingDir.Set(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    void headSwingRotate()
	{
		rotateZ = Vector2.SignedAngle(jewel_TF.position - this.transform.position, baseAxis.normalized);
		this.transform.localEulerAngles = new Vector3(0.0f, 0.0f, -rotateZ);
        //Debug.Log(rotateZ);
	}

    void headAimRotate()
    {
        // use mouse to aim
        if (PlayerController.Instance.controlDevice == PlayerController.ControlDevice.KEYBOARD)
        {
            if (PlayerController.Instance.ropeStatus == PlayerController.RopeStatus.AIM)
            {
                mouseAimDir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - jewel_TF.position;
                rotateAngleAim = Mathf.Lerp(0.0f, Vector2.SignedAngle(currentAimDir, mouseAimDir), 0.2f);
                this.transform.RotateAround(jewel_TF.position, Vector3.forward, rotateAngleAim);
                currentAimDir = this.transform.up;
            }
        }
        else if (PlayerController.Instance.controlDevice == PlayerController.ControlDevice.GAMEPAD)
        {
            if (PlayerController.Instance.ropeStatus == PlayerController.RopeStatus.AIM)
            {
                mouseAimDir.Set(Input.GetAxis("AimHorizontal"), Input.GetAxis("AimVertical"));
                rotateAngleAim = Mathf.Lerp(0.0f, Vector2.SignedAngle(currentAimDir, mouseAimDir), 0.2f);
                this.transform.RotateAround(jewel_TF.position, Vector3.forward, rotateAngleAim);
                currentAimDir = this.transform.up;
            }
        }


    }

    public void ResetRotate()
    {
        this.transform.localEulerAngles = Vector3.zero;
    }

    public void SetAim(Vector2 direction)
    {
        mouseAimDir = direction;
        rotateAngleAim = Vector2.SignedAngle(currentAimDir, mouseAimDir);
        this.transform.RotateAround(jewel_TF.position, Vector3.forward, rotateAngleAim);
        currentAimDir = this.transform.up;
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    // rope collide maze when rope is firing, make rope attach
    //    if (PlayerController.Instance.ropeStatus == PlayerController.RopeStatus.FIRE)
    //    {
    //        //Vector2 attachPoint = Physics2D.Raycast(PlayerController.Instance.transform.position, this.transform.up, 
    //        //    Mathf.Infinity, LayerMask.GetMask("Maze")).point;
    //        //this.transform.transform.position = (Vector3)attachPoint;
    //        //Debug.DrawRay(PlayerController.Instance.transform.position, this.transform.up, Color.red);
    //        //Debug.Break();
    //        //PlayerController.Instance.SetAttachPoint(attachPoint);
    //        PlayerController.Instance.ropeStatus = PlayerController.RopeStatus.ATTACH;
    //        PlayerController.Instance.ChangeActionStatus(PlayerController.ActionStatus.SWING);
    //    }
    //}
}
