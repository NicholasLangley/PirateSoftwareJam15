using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadController : MonoBehaviour
{
    [SerializeField]
    Segment head;

    LineRenderer leftLR, rightLR, mainLR;
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
    PolygonCollider2D collider;

    // Start is called before the first frame update
    void Start()
    {
        Material mat = gameObject.GetComponent<MeshRenderer>().material;

        if (isMeshType)
        {
            GameObject go = new GameObject();
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
            mainLR = go.AddComponent<LineRenderer>();
            mainLR.startWidth = lineWidth;
            mainLR.endWidth = lineWidth;

            mainLR.material = mat;
            mainLR.material.SetColor("_Color", outlineColor);


            go = new GameObject();
            leftLR = go.AddComponent<LineRenderer>();
            leftLR.startWidth = lineWidth;
            leftLR.endWidth = lineWidth;

            leftLR.material = mat;
            leftLR.material.SetColor("_Color", outlineColor);


            go = new GameObject();
            rightLR = go.AddComponent<LineRenderer>();
            rightLR.startWidth = lineWidth;
            rightLR.endWidth = lineWidth;

            rightLR.material = mat;
            rightLR.material.SetColor("_Color", outlineColor);
        }

        head.SetMaxAngle(minAngle);

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newHeadPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        newHeadPos.z = head.transform.position.z;

        head.Constrain(newHeadPos);

        if (isMeshType) { DrawMesh(); }
        else { DrawLines(); }
    }

    void DrawLines()
    {
        List<Vector3> centerList = head.GetCenterList();
        mainLR.positionCount = centerList.Count;
        mainLR.SetPositions(centerList.ToArray());

        List<Vector3> leftList = head.GetLeftSideList();
        leftLR.positionCount = leftList.Count;
        leftLR.SetPositions(leftList.ToArray());

        List<Vector3> rightList = head.GetRightSideList();
        rightLR.positionCount = rightList.Count;
        rightLR.SetPositions(rightList.ToArray());
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
            mainLR.positionCount = meshPoints.Count;
            mainLR.SetPositions(meshPoints.ToArray());
        }

        Vector2[] vertices = new Vector2[meshPoints.Count];
        int i = 0;
        foreach(Vector3 point in meshPoints)
        {
            vertices[i++] = point;
        }

        collider.points = vertices;
        meshFilter.mesh = collider.CreateMesh(false, false);
    }
}
