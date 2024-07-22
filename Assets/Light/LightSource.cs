using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightSource : MonoBehaviour
{
    public enum LIGHT_TYPE {mundane, magical, silver, red, black, divine}
    LIGHT_TYPE currentLightType;

    [Header("MeshREndering")]
    public CompositeCollider2D groundTilemap;
    Mesh groundMesh;
    [SerializeField] 
    MeshFilter lightMeshFilter;
    [SerializeField]
    MeshRenderer lightMeshRenderer;
    [SerializeField]
    PolygonCollider2D lightCollider;

    //doesn't move to reduce wierd effects
    [SerializeField]
    Transform staticLightSourceTransform;

    [Header("Light properties")]
    [SerializeField]
    float lanternSphereDistance, lanternConeDistance;

    [SerializeField]
    LayerMask lightBlockLayers;

    [SerializeField]
    int fillerRayCount;

    [SerializeField]
    float LightConeAngle;

    [SerializeField]
    Light2D circleLight, coneLight, silverCircleLight, silverConeLight;
    [SerializeField]
    LightColors lightColors;

    [SerializeField]
    HiddenObjectManager hiddenObjectManager;

    // Start is called before the first frame update
    void Start()
    {
        lightMeshFilter.mesh = new Mesh();
        getGroundMesh();
        changeLightType(LIGHT_TYPE.mundane);
    }

    // Update is called once per frame
    void Update()
    {
        //CounterParentTransform();
        if (Input.GetKeyDown(KeyCode.Alpha1)) { changeLightType(LIGHT_TYPE.mundane); }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) { changeLightType(LIGHT_TYPE.magical); }
        else if (Input.GetKeyDown(KeyCode.Alpha3)) { changeLightType(LIGHT_TYPE.silver); }
        else if (Input.GetKeyDown(KeyCode.Alpha4)) { changeLightType(LIGHT_TYPE.red); }
        else if (Input.GetKeyDown(KeyCode.Alpha5)) { changeLightType(LIGHT_TYPE.black); }
        else if (Input.GetKeyDown(KeyCode.Alpha6)) { changeLightType(LIGHT_TYPE.divine); }
        DrawLightMesh();
    }

    void getGroundMesh()
    {
        groundMesh = groundTilemap.CreateMesh(false, false);
    }

    void DrawLightMesh()
    {
        List<Ray> rays = new List<Ray>();
        //1 ray per vertice of light blocking meshes, 2 additional rays slightly offset from vertice to extend around colors
        foreach (Vector3 vertex in groundMesh.vertices)
        {
             Vector3 direction = vertex - transform.position;
            direction.z = 0;
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

        //generate filler rays to make mesh more circular
        float anglePerRay = 360.0f / fillerRayCount;
        for(int j = 0; j < fillerRayCount;  j++)
        {
            Vector3 rayDirection = Quaternion.Euler(0, 0, anglePerRay * j) * Vector3.right;
            Ray newRay = new Ray(transform.position, rayDirection.normalized);

            rays.Add(newRay);
        }

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        

        Vector3 aimDirection = mousePos - transform.position;
        Vector3 maxAimConeDirection = Quaternion.Euler(0, 0, LightConeAngle / 2.0f) * aimDirection;
        Vector3 minAimConeDirection = Quaternion.Euler(0, 0, -LightConeAngle / 2.0f) * aimDirection;

        //rotate spotlight to match

        coneLight.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Vector3.SignedAngle(aimDirection, Vector3.up, Vector3.back)));
        silverConeLight.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Vector3.SignedAngle(aimDirection, Vector3.up, Vector3.back)));

        Ray maxConeRay = new Ray(transform.position, maxAimConeDirection.normalized);
        Ray minConeRay = new Ray(transform.position, minAimConeDirection.normalized);

        rays.Add(maxConeRay);
        rays.Add(minConeRay);

        //more filler rays to make light cone less choppy
        anglePerRay = Vector3.SignedAngle(maxConeRay.direction, minConeRay.direction, Vector3.back) / fillerRayCount;
        for (int j = 0; j < fillerRayCount; j++)
        {
            Vector3 rayDirection = Quaternion.Euler(0, 0, anglePerRay * j) * minConeRay.direction;
            Ray newRay = new Ray(transform.position, rayDirection.normalized);

            rays.Add(newRay);
        }

        //sort rays by angle so that the polygon can be drawn in correct order
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
            float lightRayDistance = IsRayInbetweenRays(ray, minConeRay, maxConeRay) ? lanternConeDistance : lanternSphereDistance;

            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, lightRayDistance, lightBlockLayers);
            if (hit.collider != null)
            {
                vertices[i] = staticLightSourceTransform.InverseTransformPoint(hit.point);
            }
            else 
            {
                vertices[i] = staticLightSourceTransform.InverseTransformPoint(ray.GetPoint(lightRayDistance)); 
            }
            i++;
        }

        //generate light mesh
        lightCollider.points = vertices;
        //lightMeshFilter.mesh = lightCollider.CreateMesh(false, false);
    }

    bool IsRayInbetweenRays(Ray targetRay, Ray minAngleRay, Ray maxAngleRay)
    {
        float minAngle = Vector3.SignedAngle(targetRay.direction, minAngleRay.direction, Vector3.back);
        float maxAngle = Vector3.SignedAngle(targetRay.direction, maxAngleRay.direction, Vector3.back);

        if (minAngle >= 0 && maxAngle <= 0) { return true; }
        return false;
    }

    void CounterParentTransform()
    {
        Transform pt = transform.parent;
        Vector3 newPos = Vector3.zero;
        transform.position = pt.position;

        Vector3 newScale = new Vector3(1.0f, 1.0f, 1.0f);
        transform.localScale = new Vector3(1.0f / pt.localScale.x, 1.0f / pt.localScale.y, 1.0f/ pt.localScale.z);
    }

    public void changeLightType(LIGHT_TYPE type)
    {
        hiddenObjectManager.DisableAll();

        silverCircleLight.enabled = false;
        silverConeLight.enabled = false;

        switch(type)
        {
            case LIGHT_TYPE.mundane:
                currentLightType = LIGHT_TYPE.mundane;
                break;
            case LIGHT_TYPE.magical:
                currentLightType = LIGHT_TYPE.magical;
                break;
            case LIGHT_TYPE.silver:
                currentLightType = LIGHT_TYPE.silver;
                hiddenObjectManager.EnableAll();
                silverCircleLight.enabled = true;
                silverConeLight.enabled = true;
                break;
            case LIGHT_TYPE.red:
                currentLightType = LIGHT_TYPE.red;
                break;
            case LIGHT_TYPE.black:
                currentLightType = LIGHT_TYPE.black;
                break;
            case LIGHT_TYPE.divine:
                currentLightType = LIGHT_TYPE.divine;
                break;
        }
        circleLight.color = lightColors.GetColor(type);
        coneLight.color = lightColors.GetColor(type);
    }
}
