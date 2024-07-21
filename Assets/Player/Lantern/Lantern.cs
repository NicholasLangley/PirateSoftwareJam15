using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lantern : MonoBehaviour
{
    public CompositeCollider2D groundTilemap;
    Mesh groundMesh;
    [SerializeField] 
    MeshFilter lightMeshFilter;
    [SerializeField]
    PolygonCollider2D lightCollider;

    //doesn't move to reduce wierd effects
    [SerializeField]
    Transform staticLightSourceTransform;

    [SerializeField]
    float lanternSphereDistance, lanternConeDistance;

    [SerializeField]
    LayerMask lightBlockLayers;

    // Start is called before the first frame update
    void Start()
    {
        lightMeshFilter.mesh = new Mesh();
        getGroundMesh();
    }

    // Update is called once per frame
    void Update()
    {
        CounterParentTransform();
        DrawLightMesh();
    }

    void getGroundMesh()
    {
        groundMesh = groundTilemap.CreateMesh(false, false);
    }

    void DrawLightMesh()
    {
        List<Ray> rays = new List<Ray>();
        foreach (Vector3 vertex in groundMesh.vertices)
        {
             Vector3 direction = vertex - transform.position;
             Ray newRay = new Ray(transform.position, direction.normalized);
             float rayAngle = Vector3.Angle(newRay.direction, Vector3.right);

             Vector3 rotatedDirectionUp = Quaternion.Euler(0, 0, rayAngle + 0.005f) * Vector3.right;
             Vector3 rotatedDirectionDown = Quaternion.Euler(0, 0, rayAngle - 0.005f) * Vector3.right;
             Ray newRayUp = new Ray(transform.position, rotatedDirectionUp.normalized);
             Ray newRayDown = new Ray(transform.position, rotatedDirectionDown.normalized);

             rays.Add(newRay);
             rays.Add(newRayUp);
             rays.Add(newRayDown);
        }
        
        rays.Sort((r1, r2) =>{
            float angle = Vector3.SignedAngle(r1.direction, r2.direction, Vector3.back);

            if (angle > 0) { return -1; }
            else if(angle == 0) { return 0; }
            return 1;
        });

        //get light mesh points
        Vector2[] vertices = new Vector2[rays.Count];

        int i = 0;
        foreach(Ray ray in rays)
        {
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, lanternSphereDistance, lightBlockLayers);
            if (hit.collider != null)
            {
                vertices[i] = staticLightSourceTransform.InverseTransformPoint(hit.point);
            }
            else 
            {
                vertices[i] = staticLightSourceTransform.InverseTransformPoint(ray.GetPoint(lanternSphereDistance)); 
            }
            i++;
        }

        lightCollider.points = vertices;
        lightMeshFilter.mesh = lightCollider.CreateMesh(false, false);
    }

    void CounterParentTransform()
    {
        Transform pt = transform.parent;
        Vector3 newPos = Vector3.zero;
        transform.position = pt.position;

        Vector3 newScale = new Vector3(1.0f, 1.0f, 1.0f);
        transform.localScale = new Vector3(1.0f / pt.localScale.x, 1.0f / pt.localScale.y, 1.0f/ pt.localScale.z);
    }
}
