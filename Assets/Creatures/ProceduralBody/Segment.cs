using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Segment : MonoBehaviour
{
    public float distanceToNextSegment;
    public float segmentSize;
    float minAngle;
    public Segment nextSegment;
    public Segment previousSegment;
    public SpriteRenderer sr;

    public Vector3 rightSide, leftSide, currentDirection;
    public Vector3 rightSideLimbGoal, leftSideLimbGoal;

    [SerializeField]
    public Segment anchor;

    void Start()
    {
        transform.localScale = new Vector3(segmentSize, segmentSize, 1);
        if(nextSegment != null)
        {
            nextSegment.previousSegment = this;
        }
        currentDirection = Vector3.right;
    }

    public void Constrain(Vector3 nextPos)
    {
        if(previousSegment == null)
        {
            //currentDirection = nextPos - transform.position;
            //currentDirection = currentDirection.normalized * segmentSize;
            //currentDirection.z = 0;
            transform.position = nextPos;
            currentDirection = transform.position - nextSegment.transform.position;
        }
        else
        {
            transform.position = nextPos;
            currentDirection = previousSegment.transform.position - transform.position;
        }

        if (nextSegment != null)
        {
            Vector3 vectorToNextSegment = nextSegment.transform.position - transform.position;
            vectorToNextSegment = vectorToNextSegment.normalized * distanceToNextSegment;

            //calculate angle
            if(previousSegment != null)
            {
                float interSegmentAngle = Vector3.SignedAngle(currentDirection, vectorToNextSegment, Vector3.back);
                if (Mathf.Abs(interSegmentAngle) < minAngle) 
                {
                    vectorToNextSegment = Quaternion.Euler(0, 0, interSegmentAngle >= 0 ? -minAngle : minAngle) * currentDirection;
                }
            }

            nextSegment.Constrain(transform.position + vectorToNextSegment);
        }

        CalculateSides();
    }

    public void ReverseConstrain(Vector3 nextPos)
    {
        Vector3 rDirection;
        if (nextSegment == null)
        {
            transform.position = nextPos;
            rDirection = transform.position - previousSegment.transform.position;
        }
        else
        {
            transform.position = nextPos;
            rDirection = nextSegment.transform.position - transform.position;
        }

        if (previousSegment != null)
        {
            Vector3 vectorToNextSegment = previousSegment.transform.position - transform.position;
            vectorToNextSegment = vectorToNextSegment.normalized * distanceToNextSegment;

            /*//calculate angle
            if (nextSegment != null)
            {
                float interSegmentAngle = Vector3.SignedAngle(rDirection, vectorToNextSegment, Vector3.back);
                if (Mathf.Abs(interSegmentAngle) < minAngle)
                {
                    vectorToNextSegment = Quaternion.Euler(0, 0, interSegmentAngle >= 0 ? -minAngle : minAngle) * rDirection;
                }
            }*/

            previousSegment.ReverseConstrain(transform.position + vectorToNextSegment);
        }

        CalculateSides();
    }

    public List<Vector3> GetLeftSideList()
    {
        List<Vector3> leftSidePoints;
        if (nextSegment == null)
        {
            leftSidePoints = new List<Vector3>();
        }
        else 
        {
            leftSidePoints = nextSegment.GetLeftSideList();
        }
        leftSidePoints.Add(leftSide);

        return leftSidePoints;
    }

    public List<Vector3> GetRightSideList()
    {
        List<Vector3> rightSidePoints;
        if (nextSegment == null)
        {
            rightSidePoints = new List<Vector3>();
        }
        else
        {
            rightSidePoints = nextSegment.GetRightSideList();
        }
        rightSidePoints.Add(rightSide);

        return rightSidePoints;
    }

    public List<Vector3> GetCenterList()
    {
        List<Vector3> centerPoints;
        if (nextSegment == null)
        {
            centerPoints = new List<Vector3>();
        }
        else
        {
            centerPoints = nextSegment.GetCenterList();
        }
        centerPoints.Add(transform.position);

        return centerPoints;
    }

    public Vector3 getHeadForwardPoint()
    {
        return transform.position + currentDirection.normalized * segmentSize / 2.0f;
    }

    public Vector3 getTailBackwardPoint()
    {
        if (nextSegment != null) { return nextSegment.getTailBackwardPoint(); }
     
        return transform.position - currentDirection.normalized * segmentSize / 2.0f;
    }

    void CalculateSides()
    {
        float currentAngle = Vector3.SignedAngle(currentDirection, Vector3.right, Vector3.back) + 180f;
        if(sr != null)
        {
            Vector3 newSpriteRotation = Vector3.zero;
            newSpriteRotation.z = currentAngle;
            sr.transform.rotation = Quaternion.Euler(newSpriteRotation);
        }

        float leftAngle = currentAngle + 90f;
        float rightAngle = currentAngle - 90f;

        leftAngle = NormalizeAngle(leftAngle);
        rightAngle = NormalizeAngle(rightAngle);

        leftSide = transform.position + CalculateSideOffset(leftAngle);
        rightSide = transform.position + CalculateSideOffset(rightAngle);

        leftSideLimbGoal = transform.position + CalculateSideOffset(leftAngle + 45) * 4;
        rightSideLimbGoal = transform.position + CalculateSideOffset(rightAngle - 45) * 4;
    }

    float NormalizeAngle(float angle)
    {
        if (angle < 0f) { return angle + 360f; }
        if (angle > 0f) { return angle - 360f; }
        return angle;
    }

    Vector3 CalculateSideOffset(float angle)
    {
        Vector3 offset = Vector3.zero;
        offset.x = segmentSize * Mathf.Cos(angle * Mathf.Deg2Rad) / 2.0f;
        offset.y = segmentSize * Mathf.Sin(angle * Mathf.Deg2Rad) / 2.0f;

        return offset;
    }

    public void SetMaxAngle(float angle)
    {
        minAngle = angle;
        if(nextSegment != null)
        {
            nextSegment.SetMaxAngle(angle);
        }
    }
}
