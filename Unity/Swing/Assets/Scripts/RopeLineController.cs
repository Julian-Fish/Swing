using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeLineController : MonoBehaviour
{
    LineRenderer lineRenderer;
    RaycastHit2D raycastHit1;
    RaycastHit2D raycastHit2;
    RaycastHit2D[] raycastMid_List;
    Vector2 raycastHitPos;
    Vector2 rayStartPos1;
    Vector2 rayStartPos2;
    Vector2 rayDir;
    Vector2 raycastMidDir;
    Vector2 warpPointPos;
	float rayDistance;
    Vector2 prevRopeDir;
    public int currentPos;

    public Transform head_TF;
    public Transform jewel_TF;
    float currentFrameSwingAngle;
    float prevFrameSwingAngle;
	float swingAngleMulti;
    public LayerMask ropeLayerMask;


    private void Awake()
    {
        lineRenderer = this.GetComponent<LineRenderer>();
	}
    // Start is called before the first frame update
    void Start()
    {
        currentPos = 1;
        currentFrameSwingAngle = Mathf.Infinity;
        prevFrameSwingAngle = Mathf.Infinity;
        warpPointPos = Vector2.zero;
        //raycastResult = null;
        //contactFilter2D.SetLayerMask(ropeLayerMask);
        //contactFilter2D.useLayerMask = true;
    }

    void Update()
    {
		setLinePointPosition();
		if (PlayerController.Instance.actionStatus == PlayerController.ActionStatus.SWING)
        {
            lineRaycast();
        }
    }


    void setLinePointPosition()
    {
        lineRenderer.SetPosition(0, head_TF.position);
        lineRenderer.SetPosition(currentPos, jewel_TF.position);
    }

    void lineRaycast()
    {
		// lineRenderer current position as start position
		// vector cur->cur - 1 as direction (jewel -> warp point)
		rayStartPos1 = lineRenderer.GetPosition(currentPos);
		rayStartPos2 = lineRenderer.GetPosition(currentPos - 1) ;
        rayDir = rayStartPos2 - rayStartPos1;
        rayStartPos2 -= (Vector2)rayDir.normalized * 0.01f;

        rayDistance = rayDir.magnitude - 0.01f;
		raycastHit1 = Physics2D.Raycast(rayStartPos1, rayDir, rayDistance, ropeLayerMask);
		raycastHit2 = Physics2D.Raycast(rayStartPos2, -rayDir, rayDistance, ropeLayerMask);

        Debug.DrawRay(rayStartPos1, rayDir);
		Debug.DrawRay(rayStartPos2, -rayDir);
		//Debug.Log("raycastHit1 point :" + raycastHit1.point);
		//Debug.Log("raycastHit2 point :" + raycastHit2.point);
		// add warp point 
		if (raycastHit1 && raycastHit2)
        {
            raycastMidDir = raycastHit2.point - raycastHit1.point;
            raycastMid_List = Physics2D.RaycastAll(raycastHit1.point, raycastMidDir,
                raycastMidDir.magnitude, ropeLayerMask);
            Debug.DrawRay(raycastHit1.point, raycastMidDir, Color.red);
            if (raycastMid_List.Length >= 2)
            {
                warpPointPos = getClosestPoly2DPoint(raycastHit1);
            }
            else
            {
                warpPointPos = getWarpPointWithHits();
            }

			if (warpPointPos != rayStartPos2 && warpPointPos.magnitude != Mathf.Infinity)
            {
                addLinePoint(warpPointPos);
            }
        }
        // delete warp point 
        else if (currentPos >= 2)
        {
            // angle judge
            prevRopeDir = lineRenderer.GetPosition(currentPos - 1) - lineRenderer.GetPosition(currentPos - 2);
            currentFrameSwingAngle = Vector2.SignedAngle(prevRopeDir.normalized, -rayDir.normalized);
			swingAngleMulti = currentFrameSwingAngle * prevFrameSwingAngle;
			//Debug.Log("prevFrameSwingAngle :" + prevFrameSwingAngle);
			//Debug.Log("currentFrameSwingAngle :" + currentFrameSwingAngle);
			if (swingAngleMulti <= 0.0f && swingAngleMulti != Mathf.Infinity && swingAngleMulti != Mathf.NegativeInfinity)
            {
				Debug.Log("delete warp point");
				deleteLinePoint();
                if (currentPos >= 2)
                {
                    prevRopeDir = lineRenderer.GetPosition(currentPos - 1) - lineRenderer.GetPosition(currentPos - 2);
                    currentFrameSwingAngle = Vector2.SignedAngle(prevRopeDir.normalized, -rayDir.normalized);
                }
                else
                {
                    currentFrameSwingAngle = Mathf.Infinity;
                }
            }
            prevFrameSwingAngle = currentFrameSwingAngle;
        }
    }

    void addLinePoint(Vector2 pos)
    {
		// add new position in lineRenderer
        lineRenderer.positionCount = ++currentPos + 1;
        lineRenderer.SetPosition(currentPos, lineRenderer.GetPosition(currentPos - 1));
        lineRenderer.SetPosition(currentPos - 1, pos);

        // calcute new distance and warp length
        float distance = Vector2.Distance(lineRenderer.GetPosition(currentPos), lineRenderer.GetPosition(currentPos - 1));
        float warpLength = Vector2.Distance(lineRenderer.GetPosition(currentPos - 1), lineRenderer.GetPosition(currentPos - 2));
        PlayerController.Instance.IncWarpLength(warpLength);
        PlayerController.Instance.UpdateJointAnchor(lineRenderer.GetPosition(currentPos - 1), distance);
    }

    void deleteLinePoint()
    {
        // calcute warp length
        float warpLength = Vector2.Distance(lineRenderer.GetPosition(currentPos - 1), lineRenderer.GetPosition(currentPos - 2));
        // delete point
        lineRenderer.SetPosition(currentPos - 1, lineRenderer.GetPosition(currentPos));
        lineRenderer.positionCount = currentPos;
        --currentPos;

        // calcute new distance
        float distance = Vector2.Distance(lineRenderer.GetPosition(currentPos), lineRenderer.GetPosition(currentPos - 1));
        PlayerController.Instance.DecWarpLength(warpLength);
        PlayerController.Instance.UpdateJointAnchor(lineRenderer.GetPosition(currentPos - 1), distance);
    }

	Vector2 getWarpPointWithHits()
	{
		Vector2 midPoint = Vector2.Lerp(raycastHit1.point, raycastHit2.point, 0.5f);
		Vector2 speedDir = PlayerController.Instance.GetSpeedDir().normalized * 0.1f;
		Vector2 startPos = midPoint - speedDir;
        RaycastHit2D hit = Physics2D.Raycast(startPos, speedDir, speedDir.magnitude, ropeLayerMask);
        Debug.DrawRay(startPos, speedDir);
        if (hit && hit.collider.name == raycastHit1.collider.name)
        {
            return getClosestPoly2DPoint(hit);
        }
        else
        {
            return Vector2.negativeInfinity;
        }
    }

	Vector2 getClosestPoly2DPoint(RaycastHit2D raycastHit)
    {
		// get closest line of poly2D
		PolygonCollider2D polyCollider = raycastHit.collider as PolygonCollider2D;
        //Vector2 hitPoint = raycastHit.point - (Vector2)polyCollider.transform.localPosition;
		Vector2 hitPoint = polyCollider.transform.InverseTransformPoint(raycastHit.point);
		float closestDistance = Mathf.Infinity;
		float math_sin = 0.0f;
		float distanceToLine = 0.0f;
		int closestLineIndex = 0;
        for(int i = 0; i < polyCollider.points.Length; ++i)
        {
			math_sin = Mathf.Sin(Vector2.Angle(hitPoint - polyCollider.points[i], polyCollider.points[(i + 1) % polyCollider.points.Length] - polyCollider.points[i]));
			distanceToLine = Mathf.Abs(math_sin * Vector2.Distance(hitPoint, polyCollider.points[i]));
			if (distanceToLine < closestDistance)
            {
				closestDistance = distanceToLine;
				closestLineIndex = i;
            }
        }
		Vector2 point1 = polyCollider.points[closestLineIndex];
		Vector2 point2 = polyCollider.points[(closestLineIndex + 1) % polyCollider.points.Length];
		point1 = polyCollider.transform.TransformPoint(point1); // fix to world
		point2 = polyCollider.transform.TransformPoint(point2); // fix to world
        //float distance1 = Vector2.Distance(jewel_TF.position, point1);
        //float distance2 = Vector2.Distance(jewel_TF.position, point2);
        float distance1 = Vector2.Distance(raycastHit.point, point1);
        float distance2 = Vector2.Distance(raycastHit.point, point2);
        Vector2 closePoint = Vector2.zero;
		Vector2 farPoint = Vector2.zero;
		if (distance1 < distance2)
		{
			closePoint = point1;
			farPoint = point2;
		}
		else
		{
			closePoint = point2;
			farPoint = point1;
		}

		// return world position
		if (closePoint != (Vector2)lineRenderer.GetPosition(currentPos - 1))
		{
			Debug.Log("return close point");
			return closePoint;
		}
		else
		{
			Debug.Log("return far point");
			return farPoint;
		}
	}

    public void ResetRopeLine()
    {
        // clear warp point
        currentPos = 1;
        lineRenderer.positionCount = currentPos + 1;
        lineRenderer.SetPosition(0, head_TF.position);
        lineRenderer.SetPosition(currentPos, jewel_TF.position);
        currentFrameSwingAngle = Mathf.Infinity;
        prevFrameSwingAngle = Mathf.Infinity;
    }

    public Vector2 getSwingLineDir()
    {
        return lineRenderer.GetPosition(currentPos - 1) - lineRenderer.GetPosition(currentPos);
    }

	public void SetColor(Color color)
	{
		lineRenderer.startColor = color;
		lineRenderer.endColor= color;
	}

    public void SetLineRendererEnable(bool b)
    {
        lineRenderer.enabled = b;
    }
}
