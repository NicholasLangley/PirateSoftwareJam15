using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadController : MonoBehaviour
{
    [SerializeField]
    public Segment head, tail;

    [Header("FABRIK")]
    [SerializeField]
    bool isLimb, isRightLimb, otherwiseHasAnchor;
    [SerializeField]
    float limbReachLength;
    Vector3 fabrikStart, fabrikGoal;
    float lerpTimer;
    public float lerpDuration;


    LineRenderer leftLR, rightLR, mainLR;
    [Header("Lines and Angles")]
    [SerializeField]
    public float lineWidth;
    public float minAngle;

    [Header("MeshType")]
    [SerializeField]
    bool isMeshType, hasOutline;
    [SerializeField]
    Color meshColor, outlineColor;
    [SerializeField]
    MeshFilter meshFilter;
    [SerializeField]
    PolygonCollider2D bodyCollider;

    [SerializeField]
    GameObject leftEye, rightEye;

    public Vector3 headPosition;
    public Vector3 anchorPos;

    // Start is called before the first frame update
    void Start()
    {
        Material mat = gameObject.GetComponent<MeshRenderer>().material;

        if (isMeshType)
        {
            GameObject go = new GameObject();
            go.transform.parent = transform;
            mainLR = go.AddComponent<LineRenderer>();
            mainLR.startWidth = lineWidth;
            mainLR.endWidth = lineWidth;

            mat.SetColor("_Color", meshColor);

            mainLR.material = mat;
            mainLR.material.SetColor("_Color", outlineColor);
        }

        else
        {
            GameObject go = new GameObject();
            go.transform.parent = transform;
            mainLR = go.AddComponent<LineRenderer>();
            mainLR.startWidth = lineWidth;
            mainLR.endWidth = lineWidth;

            mainLR.material = mat;
            mainLR.material.SetColor("_Color", outlineColor);


            go = new GameObject();
            go.transform.parent = transform;
            leftLR = go.AddComponent<LineRenderer>();
            leftLR.startWidth = lineWidth;
            leftLR.endWidth = lineWidth;

            leftLR.material = mat;
            leftLR.material.SetColor("_Color", outlineColor);


            go = new GameObject();
            go.transform.parent = transform;
            rightLR = go.AddComponent<LineRenderer>();
            rightLR.startWidth = lineWidth;
            rightLR.endWidth = lineWidth;

            rightLR.material = mat;
            rightLR.material.SetColor("_Color", outlineColor);
        }

        head.SetMaxAngle(minAngle);
        lerpTimer = 0;

        if (isLimb) { updateGoalPosition(); }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLimb)
        {
            if (otherwiseHasAnchor) { FABRIK_non_limb(headPosition); }
            else { head.Constrain(headPosition); }

            if (leftEye != null && rightEye != null) { MoveEyes(); }
        }
        else
        {
            FABRIK();
        }

        if (isMeshType) { DrawMesh(); }
        else { DrawLines(); }

    }

    void DrawLines()
    {
        List<Vector3> centerList = head.GetCenterList();
        DrawLine(mainLR, centerList);

        List<Vector3> leftList = head.GetLeftSideList();
        DrawLine(leftLR, leftList);

        List<Vector3> rightList = head.GetRightSideList();
        DrawLine(rightLR, rightList);
    }

    void DrawMesh()
    {
        List<Vector3> meshPoints = new List<Vector3>();
        meshPoints.Add(head.getHeadForwardPoint());
        List<Vector3> reversedRight = head.GetRightSideList();
        reversedRight.Reverse();
        meshPoints.AddRange(reversedRight);
        meshPoints.Add(head.getTailBackwardPoint());
        meshPoints.AddRange(head.GetLeftSideList());
        meshPoints.Add(head.getHeadForwardPoint());

        if (hasOutline)
        {
            DrawLine(mainLR, meshPoints);
        }

        Vector2[] vertices = new Vector2[meshPoints.Count];
        int i = 0;
        foreach(Vector3 point in meshPoints)
        {
            vertices[i++] = point;
        }

        bodyCollider.points = vertices;
        meshFilter.mesh = bodyCollider.CreateMesh(false, false);
    }

    //Forward And Backward Reaching Inverse Kinematics
    void FABRIK()
    {
        lerpTimer += Time.deltaTime;

        head.Constrain(Vector3.Lerp(fabrikStart, fabrikGoal, lerpTimer/lerpDuration));
        tail.ReverseConstrain(isRightLimb ? tail.anchor.rightSide : tail.anchor.leftSide);

        //check for distance
        Vector3 distanceFromGoal = fabrikGoal - head.transform.position;
        if (distanceFromGoal.magnitude > limbReachLength && lerpTimer > lerpDuration) { updateGoalPosition(); }
    }

    void FABRIK_non_limb(Vector3 pos)
    {
        head.Constrain(pos);
        tail.ReverseConstrain(anchorPos);
    }

    public void updateGoalPosition()
    {
        fabrikStart = head.transform.position;
        fabrikGoal = isRightLimb ? tail.anchor.rightSideLimbGoal : tail.anchor.leftSideLimbGoal;

        lerpTimer = 0f;
    }

    void MoveEyes()
    {
        leftEye.transform.position = Vector3.Lerp(head.transform.position, head.leftSide, 0.5f) + transform.position;
        rightEye.transform.position = Vector3.Lerp(head.transform.position, head.rightSide, 0.5f) + transform.position;
    }

    public void setHeadPosition(Vector3 pos)
    {
        headPosition = pos;
    }

    void DrawLine(LineRenderer lr, List<Vector3> worldPoints)
    {
        List<Vector3> transformedPoints = new List<Vector3>();

        foreach (Vector3 worldPoint in worldPoints)
        {
            transformedPoints.Add(worldPoint + transform.position);
        }
        lr.positionCount = transformedPoints.Count;
        lr.SetPositions(transformedPoints.ToArray());
    }
}
