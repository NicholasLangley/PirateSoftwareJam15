using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Rendering.Universal;

public class LightSource : MonoBehaviour
{
    public enum LIGHT_TYPE {mundane, magical, silver, red, black, divine, none}
    [SerializeField]
    public LIGHT_TYPE currentLightType;
    public LIGHT_TYPE currentGrenadeLightType;
    LIGHT_TYPE queuedLightType;

    [Header("MeshREndering")]
    public List<CompositeCollider2D> groundTilemaps;
    List<Mesh> groundMeshes;

    PolygonCollider2D lightCollider;

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
    Light2D circleLight, coneLight, silverCircleLight, silverConeLight, highlightCircleLight, highlightConeLight;
    [SerializeField]
    LightColors lightColors, highlightColors;

    [Header("Shadows")]
    //[SerializeField]
    //GameObject shadowFirePrefab;
    float shadowFireTimer;
    [SerializeField]
    float shadowFireSpawnTime;
    [SerializeField]
    GameObject shadowFire;
    [SerializeField]
    ParticleSystem particleSystemA, particleSystemB;
    [SerializeField]
    GameObject shadowColliderPrefab;

    enum LIGHT_OWNER { player, creature, grenade, none }

    [Header("Light Ownership")]
    [SerializeField]
    LIGHT_OWNER lightOwner;
    [SerializeField]
    public Vector3 aimDirection;

    [Header("LightGrenade")]
    [SerializeField]
    float explosionDuration;
    [SerializeField]
    float growthDuration, startSize, maxSize;
    float growthTimer;
    [SerializeField]
    LightGrenade grenadePrefab;
    [SerializeField]
    float grenadeCooldown;
    float grenadeTimer;

    [Header("Silver Background HACK")]
    [SerializeField]
    TilemapRenderer magicalBackground, magicalGround;

    int frameCount;
    [SerializeField]
    Transform spriteAnchor;
    [SerializeField]
    GameObject spriteBody;

    public bool isDead;

    // Start is called before the first frame update
    void Start()
    {
        groundTilemaps = new List<CompositeCollider2D>();
        groundMeshes = new List<Mesh>();

        currentGrenadeLightType = LIGHT_TYPE.none;
        UpdateActiveTilemaps();
        growthTimer = 0;
        grenadeTimer = 0;

        frameCount = 0;

        SpawnCollider();
        getGroundMesh();
        changeLightType(currentLightType);
        DrawLightMesh();
    }

    // Update is called once per frame
    void Update()
    {
        frameCount++;
        grenadeTimer += Time.deltaTime;
        //DEBUG keys
        if (lightOwner == LIGHT_OWNER.player)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) { changeLightType(LIGHT_TYPE.mundane); }
            else if (Input.GetKeyDown(KeyCode.Alpha2)) { changeLightType(LIGHT_TYPE.magical); }
            else if (Input.GetKeyDown(KeyCode.Alpha3)) { changeLightType(LIGHT_TYPE.silver); }
            else if (Input.GetKeyDown(KeyCode.Alpha4)) { changeLightType(LIGHT_TYPE.red); }
            else if (Input.GetKeyDown(KeyCode.Alpha5)) { changeLightType(LIGHT_TYPE.black); }
            else if (Input.GetKeyDown(KeyCode.Alpha6)) { changeLightType(LIGHT_TYPE.divine); }

            if (Input.GetKeyDown(KeyCode.Mouse0)) { SpawnExplosive(); }
        }
        else if (lightOwner == LIGHT_OWNER.grenade)
        {
            Explosion();
        }

        if (Time.timeScale > 0f)
        {
            if (currentLightType == LIGHT_TYPE.black)
            {
                shadowFireTimer += Time.deltaTime;
                if (shadowFireTimer >= shadowFireSpawnTime)
                {
                    SpawnShadowFire();
                    if (lightOwner == LIGHT_OWNER.player) { CalculateAimDirection(); }
                }
            }
            else
            {
                if (lightOwner == LIGHT_OWNER.player)
                {
                    CalculateAimDirection();
                }
                if (lightOwner != LIGHT_OWNER.grenade)
                {
                    //rotate spotlight to match
                    Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, Vector3.SignedAngle(aimDirection, Vector3.up, Vector3.back)));
                    if (!isDead) { spriteBody.transform.localRotation = Quaternion.identity; }
                    //coneLight.transform.rotation = rotation;
                    //silverConeLight.transform.rotation = rotation;
                    //highlightConeLight.transform.rotation = rotation;

                    if (spriteAnchor != null && !isDead) { spriteAnchor.rotation = rotation; }
                }
                    if (frameCount % 10 == 0) { DrawLightMesh(); frameCount = 0; }
            }
        }
    }

    public void UpdateActiveTilemaps()
    {
        groundTilemaps.Clear();
        foreach (GameObject tilemapObject in GameObject.FindGameObjectsWithTag("GroundTilemap"))
        {
            if (tilemapObject.activeInHierarchy) { groundTilemaps.Add(tilemapObject.GetComponent<CompositeCollider2D>()); }
        }
    }

    void getGroundMesh()
    {
        groundMeshes.Clear();
        foreach(CompositeCollider2D tilemap in groundTilemaps)
        {
            groundMeshes.Add(tilemap.CreateMesh(false, false));
        }
        
    }

    void DrawLightMesh()
    {
        List<Ray> rays = new List<Ray>();

        //1 ray per vertice of light blocking meshes, 2 additional rays slightly offset from vertice to extend around colors
        foreach(Mesh groundMesh in groundMeshes)
        {
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
        }

        //generate filler rays to make mesh more circular
        float anglePerRay = 360.0f / fillerRayCount;
        for(int j = 0; j < fillerRayCount;  j++)
        {
            Vector3 rayDirection = Quaternion.Euler(0, 0, anglePerRay * j) * Vector3.right;
            Ray newRay = new Ray(transform.position, rayDirection.normalized);

            rays.Add(newRay);
        }

        Vector3 maxAimConeDirection = Quaternion.Euler(0, 0, LightConeAngle / 2.0f) * aimDirection;
        Vector3 minAimConeDirection = Quaternion.Euler(0, 0, -LightConeAngle / 2.0f) * aimDirection;

        Ray maxConeRay = new Ray(transform.position, maxAimConeDirection.normalized);
        Ray minConeRay = new Ray(transform.position, minAimConeDirection.normalized);

        if (lightOwner != LIGHT_OWNER.grenade)
        {
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
        foreach (Ray ray in rays)
        {
            float lightRayDistance;
            if (lightOwner != LIGHT_OWNER.grenade)
            {
                lightRayDistance = IsRayInbetweenRays(ray, minConeRay, maxConeRay) ? lanternConeDistance : lanternSphereDistance;
            }
            else 
            {
                lightRayDistance = lanternSphereDistance;
            }

            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, lightRayDistance, lightBlockLayers);
            if (hit.collider != null)
            {
                vertices[i] = lightCollider.transform.InverseTransformPoint(hit.point);
            }
            else 
            {
                vertices[i] = lightCollider.transform.InverseTransformPoint(ray.GetPoint(lightRayDistance)); 
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

    void CalculateAimDirection()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        aimDirection = mousePos - transform.position;
    }

    void CounterParentTransform()
    {
        Transform pt = transform.parent;
        Vector3 newPos = Vector3.zero;
        transform.position = pt.position;

        Vector3 newScale = new Vector3(1.0f, 1.0f, 1.0f);
        transform.localScale = new Vector3(1.0f / pt.localScale.x, 1.0f / pt.localScale.y, 1.0f/ pt.localScale.z);
    }

    void SpawnCollider()
    {
        if (lightCollider != null) { Destroy(lightCollider.gameObject); }
        GameObject lightColliderObject = new GameObject();
        lightColliderObject.name = "light collider";
        lightCollider = lightColliderObject.AddComponent<PolygonCollider2D>();
        lightCollider.isTrigger = true;
    }

    public void changeLightType(LIGHT_TYPE type)
    {
        //HACK to make background appear once player first changes light types
        if(type == LIGHT_TYPE.magical && magicalBackground != null && magicalGround != null)
        {
            magicalBackground.sortingLayerName = "Default";
            magicalGround.sortingLayerName = "Default";
        }


        queuedLightType = type;
        circleLight.enabled = true;
        silverCircleLight.enabled = false;

        if (lightOwner != LIGHT_OWNER.grenade) 
        {
            coneLight.enabled = true;
            silverConeLight.enabled = false;
            coneLight.blendStyleIndex = 0;
        }

        lightCollider.gameObject.tag = "Light";

        if(currentLightType == LIGHT_TYPE.black)
        {
            SpawnShadowFire();
            shadowFire.SetActive(false);
        }
        

        switch (type)
        {
            case LIGHT_TYPE.mundane:
                currentLightType = LIGHT_TYPE.mundane;
                break;
            case LIGHT_TYPE.magical:
                currentLightType = LIGHT_TYPE.magical;
                break;
            case LIGHT_TYPE.silver:
                currentLightType = LIGHT_TYPE.silver;
                silverCircleLight.enabled = true;
                if (lightOwner != LIGHT_OWNER.grenade) { silverConeLight.enabled = true; }
                break;
            case LIGHT_TYPE.red:
                currentLightType = LIGHT_TYPE.red;
                coneLight.blendStyleIndex = 1;
                lightCollider.gameObject.tag = "RedLight";
                break;
            case LIGHT_TYPE.black:
                currentLightType = LIGHT_TYPE.black;
                circleLight.enabled = false;
                if (lightOwner != LIGHT_OWNER.grenade) { coneLight.enabled = false; }
                lightCollider.gameObject.tag = "Untagged";
                shadowFire.SetActive(true);
                SpawnShadowFire();
                break;
            case LIGHT_TYPE.divine:
                lightCollider.gameObject.tag = "DivineLight";
                currentLightType = LIGHT_TYPE.divine;
                break;
        }
        circleLight.color = lightColors.GetColor(type);
        if (lightOwner != LIGHT_OWNER.grenade) { coneLight.color = lightColors.GetColor(type); spriteBody.GetComponent<SpriteRenderer>().color = lightColors.GetColor(type); }
        highlightCircleLight.color = highlightColors.GetColor(type);
        if (lightOwner != LIGHT_OWNER.grenade) { highlightConeLight.color = highlightColors.GetColor(type); }
    }

    public void queueNewLightType(LIGHT_TYPE type)
    {
        queuedLightType = type;
        Debug.Log(queuedLightType);
    }
        void SpawnShadowFire()
    {
        particleSystemA.Emit(1);
        particleSystemB.Emit(1);

        GameObject go = Instantiate(shadowColliderPrefab);
        go.transform.position = transform.position;

        shadowFireTimer = 0.0f;
    }

    public void ChangeGrenadeLightType(LIGHT_TYPE type)
    {
        currentGrenadeLightType = type;
    }


    void Explosion()
    {
        growthTimer += Time.deltaTime;
        float newSize;
        if(growthTimer < growthDuration + explosionDuration)
        {
            newSize = Mathf.Lerp(startSize, maxSize, growthTimer / growthDuration);
        }
        else
        {
            newSize = Mathf.Lerp(maxSize, startSize, growthTimer - growthDuration - explosionDuration / growthDuration);
        }

        if (currentLightType == LIGHT_TYPE.black)
        {
            newSize /= 2.0f;
            shadowFire.transform.localScale = new Vector3(newSize, newSize, 1);
        }
        else
        {
            circleLight.shapeLightFalloffSize = newSize + 3f;
            silverCircleLight.shapeLightFalloffSize = newSize + 3f;
            highlightCircleLight.shapeLightFalloffSize = newSize + 3f;

            lanternSphereDistance = newSize - 0.8f;
        }

        if (growthTimer > growthDuration + explosionDuration + growthDuration)
        {
            Destroy(lightCollider.gameObject);
            Destroy(gameObject);
        }
    }


    void SpawnExplosive()
    {
        if (currentGrenadeLightType == LIGHT_TYPE.none || Time.deltaTime == 0 || grenadeTimer < grenadeCooldown) { return; }
        LightGrenade grenade = Instantiate<LightGrenade>(grenadePrefab);
        grenade.transform.position = transform.position;
        grenade.Initialize(aimDirection, currentGrenadeLightType);
        grenadeTimer = 0;
    }

    public void Kill()
    {
        isDead = true;
        spriteBody.GetComponent<Rigidbody2D>().gravityScale = 1f;
        spriteBody.GetComponent<BoxCollider2D>().enabled = true;
    }

    public void Respawn()
    {
        isDead = false;
        spriteBody.GetComponent<Rigidbody2D>().gravityScale = 0;
        spriteBody.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        spriteBody.transform.localPosition = new Vector3(0, 0.266f, 0);
        spriteBody.GetComponent<BoxCollider2D>().enabled = false;
    }
}
