using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum ActionStatus
    {
        AIM,
        SWING,
        STUN
    }

    public enum RopeStatus
    {
        AIM,
        FIRE,
        RECLAIM,
        ATTACH,
    }

    public enum ControlDevice
    {
        GAMEPAD,
        KEYBOARD,
    }

    public ControlDevice controlDevice;

    public PhysicsMaterial2D SwingBounciness;
    public PhysicsMaterial2D AimBounciness;

	public static PlayerController Instance;
    public bool canControl;
	public bool isGameover;

    public GameObject head_GB;
    Transform head_TF;
    Collider2D head_Collider;
	RaycastHit2D attachRaycastHit;
    HeadRotateController headRotateController;

    public GameObject jewel_gameObject;
	Transform jewel_TF;
    Rigidbody2D jewel_rg2d;
    Collider2D jewel_collider;
    DistanceJoint2D jewel_distanceJoint2D;
    Rigidbody2D player_rg2d;
    Collider2D player_collider;

    Vector2 jewelInitPos;
    Vector2 headInitPos;

    public RopeLineController ropeController;
	public float ropeShiftSpeed;
    public float ropeFireSpeedMultiply;
    [SerializeField]
	float currentLength;
    [SerializeField]
    float warpLength;
	[SerializeField]
	public float minLength;
	public float maxLength;
    public float initMaxLength;
    public float swingForce;
    public float minStunSpeed;
    Vector2 attachPoint;

    public ActionStatus actionStatus;
    public RopeStatus ropeStatus;


	[SerializeField]
    Vector2 inputAimDir;
    [SerializeField]
    Vector2 currentAimDir;
    [SerializeField]
    float rotateAngle;
    [SerializeField]

    float inputSwingAxisX;
    float inputSwingAxisY;
    [SerializeField]
    Vector2 swingForceDir;

    void Awake()
	{
		Instance = this;
	}
	void Start()
    {
        //canControl = true;

        head_TF = head_GB.transform;
        head_Collider = head_GB.GetComponent<CircleCollider2D>();
        headRotateController = head_GB.GetComponent<HeadRotateController>();

        jewel_TF = jewel_gameObject.transform;
        jewel_rg2d = jewel_gameObject.GetComponent<Rigidbody2D>();
        jewel_distanceJoint2D = jewel_gameObject.GetComponent<DistanceJoint2D>();
        jewel_collider = jewel_rg2d.gameObject.GetComponent<PolygonCollider2D>();

        player_rg2d = this.GetComponent<Rigidbody2D>();
        player_collider = this.gameObject.GetComponent<PolygonCollider2D>();

        jewelInitPos = this.transform.position;
        headInitPos = head_TF.position;

        // init para
        initPlayer();
    }

	void FixedUpdate()
    {
        if (Input.GetButtonDown("Restart") && canControl)
        {
            if (StageController.Instance.GetStageName() == "GameClear")
            {
                StageController.Instance.NextStage();
            }
            else if (isGameover)
            {
                Debug.Log("Restart");
                StageController.Instance.Restart();
                if (SoundPlayer.Instance)
                {
                    SoundPlayer.Instance.ResetBool();
                }
                initPlayer();
            }

        }
        switch (actionStatus)
        {
            case ActionStatus.AIM:
                Aim();
                break;
            case ActionStatus.SWING:
                Swing();
                break;
        }
    }

	void Swing()
	{
		// extend
		if (Input.GetAxis("Extend") == 1 && canControl && !isGameover)
        {
            if(!Physics2D.Raycast(this.transform.position, -1.0f * ropeController.getSwingLineDir(), 0.4f, LayerMask.GetMask("Maze")))
            {
                jewel_rg2d.AddForce(Vector2.right * 0.01f);
				if (maxLength - warpLength < currentLength + ropeShiftSpeed)
				{
					currentLength = maxLength - warpLength;
					// red lightning on
					//ropeController.SetColor(Color.red);
				}
				else
				{
					currentLength = currentLength + ropeShiftSpeed;
				}
				jewel_distanceJoint2D.distance = currentLength;

            }
        }
        // shorten
        else if (Input.GetAxis("Shorten") == 1 && canControl && !isGameover)
        {
            jewel_rg2d.AddForce(Vector2.right * 0.01f);
            if (minLength > currentLength - ropeShiftSpeed)
			{
				currentLength = minLength;
			}
			else
			{
				currentLength = currentLength - ropeShiftSpeed;
				// red lightning off
				//ropeController.SetColor(Color.white);
			}
            jewel_distanceJoint2D.distance = currentLength;
        }

        if (Input.GetAxis("Reclaim") == 1 && canControl && !isGameover)
        {
			//Debug.Break();
            // reclaim
            ropeStatus = RopeStatus.RECLAIM;
			head_TF.up = head_TF.position - jewel_TF.position;
			currentLength = Vector2.Distance(head_TF.position, jewel_TF.position);
            if (SoundPlayer.Instance)
            {
                SoundPlayer.Instance.PlaySE("reclaim");
            }
            ChangeActionStatus(ActionStatus.AIM);
        }

        if (canControl)
        {
            swingForceDir.Set(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            swingForceDir.Normalize();
        }


        // use keyboard (wasd) to swing
        if (swingForceDir != Vector2.zero)
        {
            jewel_rg2d.AddForce(swingForce * swingForceDir);
        }
    }

    void Aim()
    {
        // use raycast to get incoming collide point
        attachRaycastHit = Physics2D.Raycast(jewel_TF.position, head_TF.up, currentLength + 0.5f, LayerMask.GetMask("Maze"));

		if (ropeStatus == RopeStatus.AIM)
        {
            if (Input.GetAxis("Fire") == 1 && canControl && !isGameover)
            {
                ropeStatus = RopeStatus.FIRE;
                ropeController.SetLineRendererEnable(true);
            }
        }
        else
        {
            // rope fire until maxLength
            if (ropeStatus == RopeStatus.FIRE)
            {
                if (attachRaycastHit)
                {
                    currentLength = Vector2.Distance(attachRaycastHit.point, jewel_TF.position);
                    head_TF.localPosition = head_TF.up * currentLength;
                    ropeStatus = PlayerController.RopeStatus.ATTACH;
                    ChangeActionStatus(PlayerController.ActionStatus.SWING);

					// init distance
					currentLength = Mathf.Max(currentLength, minLength);
					jewel_distanceJoint2D.distance = currentLength;
				}
                else
                {
                    currentLength = currentLength + ropeShiftSpeed * ropeFireSpeedMultiply;
                    if (currentLength > maxLength)
                    {
                        currentLength = maxLength;
                        ropeStatus = RopeStatus.RECLAIM;
                        if (SoundPlayer.Instance)
                        {
                            SoundPlayer.Instance.PlaySE("reclaim");
                        }
                    }
                    head_TF.localPosition = head_TF.up * currentLength;
                }
            }
            // rope reclaim until minLength
            else if (ropeStatus == RopeStatus.RECLAIM)
            {
                // no attach maze, recalim rope
                currentLength = currentLength - ropeShiftSpeed * ropeFireSpeedMultiply * 2.0f;
                if (currentLength < minLength)
                {
                    currentLength = minLength;
                    ropeStatus = RopeStatus.AIM;
                    // set line renderer active off
                    ropeController.SetLineRendererEnable(false);
                }
                head_GB.transform.localPosition = head_TF.up * currentLength;

                // reset rope line and warp length
                ropeController.ResetRopeLine();
                warpLength = 0.0f;
            }

        }
    }

    public void IncWarpLength(float length)
    {
        //Debug.Log("increase warp length " + length);
        warpLength += length;
        currentLength -= length;
    }

    public void DecWarpLength(float length)
    {
        //Debug.Log("decrease warp length " + length);
        warpLength -= length;
        currentLength += length;
    }

    public void UpdateJointAnchor(Vector2 newAnchorPos, float distance)
    {
        //Debug.Log("joint new anchor pos:" + newAnchorPos);
        jewel_distanceJoint2D.connectedAnchor = newAnchorPos;
        if (distance + warpLength > maxLength)
        {
            distance = maxLength - warpLength;
        }
        jewel_distanceJoint2D.distance = distance;
    }

    public void UpdateDistanceJointConnectedAnchor()
    {
        jewel_gameObject.GetComponent<DistanceJoint2D>().connectedAnchor = head_GB.transform.localPosition;
    }

    public void ChangeActionStatus(ActionStatus status)
    {
        switch (status)
        {
            case ActionStatus.AIM:
                // aim
                actionStatus = ActionStatus.AIM;
                // set head parent in jewel
                head_TF.SetParent(jewel_TF);

                // jewel joint, rigidbody, collider turn off
                // Bounciness off
                jewel_distanceJoint2D.enabled = false;
                jewel_rg2d.sharedMaterial = AimBounciness;

                break;
            case ActionStatus.SWING:
                // swing
                actionStatus = ActionStatus.SWING;
                // set head parent out of jewel
                head_TF.SetParent(jewel_TF.parent);

                // jewel joint, rigidbody, collider on
                // Bounciness on
                jewel_distanceJoint2D.enabled = true;
                jewel_rg2d.sharedMaterial = SwingBounciness;

                // joint init
                currentLength = Mathf.Min(currentLength, initMaxLength);
                jewel_distanceJoint2D.distance = currentLength;
                UpdateDistanceJointConnectedAnchor();

                // play attach se
                if (SoundPlayer.Instance)
                {
                    SoundPlayer.Instance.PlaySE("attach");
                }

                break;
        }
    }

    public void SetAttachPoint(Vector2 vec2)
    {
        attachPoint = vec2;
    }

    public float GetSpeed()
    {
        return jewel_rg2d.velocity.magnitude;
    }

	public Vector2 GetSpeedDir()
	{
		return jewel_rg2d.velocity.normalized;
	}

    void initPlayer()
    {
        // init para
        currentLength = minLength;
        currentAimDir = Vector2.up;
        warpLength = 0.0f;
		ropeStatus = RopeStatus.AIM;
        ChangeActionStatus(ActionStatus.AIM);
        this.transform.position = jewelInitPos;
        head_TF.position = headInitPos;
        jewel_rg2d.velocity = Vector3.zero;
        headRotateController.ResetRotate();
        ropeController.ResetRopeLine();
        FaceController.Instance.ResetRotate();
        // set line renderer active off
		if (StageController.Instance.GetStageName() != "Title")
		{
			ropeController.SetLineRendererEnable(false);
		}
		isGameover = false;
	}

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Goal" && PlayerController.Instance.ropeStatus == PlayerController.RopeStatus.AIM && jewel_rg2d.velocity.magnitude < 0.1f)
        {
            //Debug.Log("Clear");
            StageController.Instance.NextStage();
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Maze" && canControl && SoundPlayer.Instance)
        {
            SoundPlayer.Instance.PlaySE("collide");
        }
    }

public void AddForceToJewel(Vector2 force)
    {
        jewel_rg2d.AddForce(force);
    }

    public void SetControl(bool b)
    {
        canControl = b;
    }
}
