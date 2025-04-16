using System.Linq;
using UnityEngine;
using System.Collections;

public class WaterManager : MonoBehaviour
{
    //LineRenderer Body;
    public float set_LeftPosition, set_Width, set_TopPosition, set_BottomPosition;

    float[] xpositions;
    float[] ypositions;
    float[] velocities;
    float[] accelerations;

    GameObject[] meshobjects;
    GameObject[] colliders;
    Mesh[] meshes;

    public Material bodyMaterial;
    public GameObject watermesh;

    const float springconstant = 0.02f;         
    const float damping = 0.04f;                
    const float spread = 0.05f;                 
    //const float z = -1f;
    const float z = 2f;

    public float effectMassOnWave = 0.01f;      

    const int lineNodesOn1Width = 2;     // number of water node per unit   default = 5     
    const int numberOfWaterPhysicsPasses = 2;   // default = 8

    public float depthDeforeSubmerged = 0.1f;   
    public float displacementAmount = 3f;       


    float baseheight;
    float bottom;
    public float adjustHeight;
  

    Vector3 colliderPosition;
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
        set_LeftPosition = mainCamera.ViewportToWorldPoint(new Vector3(0.0f, 0.0f, 10.0f)).x - 10f;
        set_Width = mainCamera.orthographicSize * 2 * mainCamera.aspect + 20f;
        set_TopPosition = -2f;
        set_BottomPosition = -mainCamera.orthographicSize * 2f;

        //Debug.Log(set_LeftPosition + " || " + set_Width + " || " + set_TopPosition + " || " + set_BottomPosition);
    }

    void Start()
    {
        SpawnWater(set_LeftPosition, set_Width, set_TopPosition, set_BottomPosition);
        //SpawnWater(-50, 100, 0, -10);
    }

    public void Splash(float xpos, float velocity)
    {

        if (xpos >= xpositions[0] && xpos <= xpositions[xpositions.Length - 1])
        {
            xpos -= xpositions[0];

            int index = Mathf.RoundToInt((xpositions.Length - 1) * (xpos / (xpositions[xpositions.Length - 1] - xpositions[0])));

            velocities[index] += velocity;
        }
    }

    public void SpawnWater(float Left, float Width, float Top, float Bottom)
    {
        gameObject.AddComponent<BoxCollider2D>();
        gameObject.GetComponent<BoxCollider2D>().offset = new Vector2(Left + Width / 2, (Top + Bottom) / 2);
        gameObject.GetComponent<BoxCollider2D>().size = new Vector2(Width, Top - Bottom);
        gameObject.GetComponent<BoxCollider2D>().isTrigger = true;


        int edgecount = Mathf.RoundToInt(Width) * lineNodesOn1Width;
        int nodecount = edgecount + 1;  // +1 because it is use to set the position of mesh object.
        /*Body = gameObject.AddComponent<LineRenderer>();
        Body.material = bodyMaterial;
        Body.material.renderQueue = 1000;
        Body.positionCount = nodecount;
        Body.startWidth = 0.1f;
        Body.endWidth = 0.1f;*/

        xpositions = new float[nodecount];
        ypositions = new float[nodecount];
        velocities = new float[nodecount];
        accelerations = new float[nodecount];

        meshobjects = new GameObject[edgecount];
        meshes = new Mesh[edgecount];
        colliders = new GameObject[edgecount];

        baseheight = Top;
        bottom = Bottom;

        for (int i = 0; i < nodecount; i++)
        {
            ypositions[i] = Top;
            xpositions[i] = Left + Width * i / edgecount;
            //Body.SetPosition(i, new Vector3(xpositions[i], Top, z));
            accelerations[i] = 0;
            velocities[i] = 0;
        }

        for (int i = 0; i < edgecount; i++)
        {
            meshes[i] = new Mesh();

            Vector3[] Vertices = new Vector3[4];
            Vertices[0] = new Vector3(xpositions[i], ypositions[i], z);
            Vertices[1] = new Vector3(xpositions[i + 1], ypositions[i + 1], z);
            Vertices[2] = new Vector3(xpositions[i], bottom, z);
            Vertices[3] = new Vector3(xpositions[i + 1], bottom, z);

            Vector2[] UVs = new Vector2[4];
            UVs[0] = new Vector2(0, 1);    
            UVs[1] = new Vector2(1, 1);         
            UVs[2] = new Vector2(0, 0);         
            UVs[3] = new Vector2(1, 0);


            int[] tris = new int[6] { 0, 1, 3, 3, 2, 0 };

            meshes[i].vertices = Vertices;
            meshes[i].uv = UVs;
            meshes[i].triangles = tris;

            meshobjects[i] = Instantiate(watermesh, Vector3.zero, Quaternion.identity) as GameObject;
            meshobjects[i].GetComponent<MeshFilter>().mesh = meshes[i];
            meshobjects[i].transform.parent = transform;
            
            colliders[i] = new GameObject();
            colliders[i].name = "Trigger";
            colliders[i].AddComponent<BoxCollider2D>();
            colliders[i].gameObject.tag = "Trigger";
            colliders[i].transform.parent = transform;

            colliders[i].transform.position = new Vector3(Left + Width * (i + 0.5f) / edgecount, Top - 0.5f, 0);
            colliders[i].transform.localScale = new Vector3(Width / edgecount, 1, 1);
           colliders[i].GetComponent<BoxCollider2D>().isTrigger = true;
            colliders[i].AddComponent<WaterDetector>();
        }
    }

    void UpdateMeshes()
    {
        for (int i = 0; i < meshes.Length; i++)
        {
          //  if (adjustHeight >= 0)
            {
                Vector3[] Vertices = new Vector3[4];
                Vertices[0] = new Vector3(xpositions[i], ypositions[i] + adjustHeight, z);
                Vertices[1] = new Vector3(xpositions[i + 1], ypositions[i + 1] + adjustHeight, z);
                Vertices[2] = new Vector3(xpositions[i], bottom - adjustHeight, z);
                Vertices[3] = new Vector3(xpositions[i + 1], bottom - adjustHeight, z);
                meshes[i].vertices = Vertices;
            }
        }
        for (int i = 0; i < meshes.Length; i++)
        {
          //  if (adjustHeight >= 0)
            {
                colliderPosition = Vector3.zero;
                Vector3[] Vertices = new Vector3[4];
                Vertices[0] = new Vector3(xpositions[i], ypositions[i], z);
                colliderPosition += Vertices[0];
                Vertices[1] = new Vector3(xpositions[i + 1], ypositions[i + 1], z);
                colliderPosition += Vertices[1];
                Vertices[2] = new Vector3(xpositions[i], bottom, z);
                colliderPosition += Vertices[2];
                Vertices[3] = new Vector3(xpositions[i + 1], bottom, z);
                colliderPosition += Vertices[3];
                colliderPosition /= 4;
                colliders[i].transform.position = new Vector3(colliderPosition.x, colliderPosition.y + 3.55f + adjustHeight, colliderPosition.z);
            }

        }
        GetComponent<BoxCollider2D>().offset = new Vector2(0, -6 + (2 * adjustHeight));
    }
   public void AdjustHeight()
    {
        adjustHeight += 0.01f;
     
        for (int i=0;i<meshobjects.Length;i++)
        {
          
            meshobjects[i].transform.position = new Vector2(meshobjects[i].transform.position.x, meshobjects[i].transform.position.y + 0.01f);
        }

        if (adjustHeight>=1.5f)
        {
            StartCoroutine(DrainWater());
           
        }
      

    }
    IEnumerator DrainWater()
    {
        if (adjustHeight >= -1)
        {
            adjustHeight -= 0.05f;
            for (int i = 0; i < meshobjects.Length; i++)
            {
              
                meshobjects[i].transform.position = new Vector2(meshobjects[i].transform.position.x, meshobjects[i].transform.position.y - 0.05f);
            }
            
            yield return new WaitForSeconds(0.25f);
            StartCoroutine(DrainWater());
        }

    }

    void FixedUpdate()
    {
        for (int i = 0; i < xpositions.Length; i++)
        {
            float force = springconstant * (ypositions[i] - baseheight) + velocities[i] * damping;
            accelerations[i] = -force;
            ypositions[i] += velocities[i];
            velocities[i] += accelerations[i];
            //Body.SetPosition(i, new Vector3(xpositions[i], ypositions[i], z));
        }

        float[] leftDeltas = new float[xpositions.Length];
        float[] rightDeltas = new float[xpositions.Length];

        for (int j = 0; j < numberOfWaterPhysicsPasses; j++)
        {
            for (int i = 0; i < xpositions.Length; i++)
            {
                if (i > 0)
                {
                    leftDeltas[i] = spread * (ypositions[i] - ypositions[i - 1]);
                    velocities[i - 1] += leftDeltas[i];
                }
                if (i < xpositions.Length - 1)
                {
                    rightDeltas[i] = spread * (ypositions[i] - ypositions[i + 1]);
                    velocities[i + 1] += rightDeltas[i];
                }
            }

            for (int i = 0; i < xpositions.Length; i++)
            {
                if (i > 0)
                    ypositions[i - 1] += leftDeltas[i];
                if (i < xpositions.Length - 1)
                    ypositions[i + 1] += rightDeltas[i];
            }
        }
        UpdateMeshes();
    }

    public float GetWaterLevel(float _x)
    {
        for (int i = 0; i < xpositions.Length; i++)
        {
            if (xpositions[i] > _x)
            {
                return (ypositions[i] + ypositions[i - 1]) / 2;
            }
        }

        return 0;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("SoftBody"))
        {
            AdjustHeight();
            collision.gameObject.SetActive(false);
        }
        
    Rigidbody2D rigidbody2D = collision.GetComponent<Rigidbody2D>();
    float waveHeight = GetWaterLevel(rigidbody2D.transform.position.x);

          float displacementMultiplier = Mathf.Clamp((((waveHeight - rigidbody2D.transform.position.y) / depthDeforeSubmerged) * displacementAmount),-2,2);

    rigidbody2D.AddForceAtPosition(new Vector2(0f, Mathf.Abs(Physics2D.gravity.y * displacementMultiplier) /**rigidbody.mass*/),
        rigidbody2D.transform.position);

    rigidbody2D.AddForce(displacementMultiplier * -rigidbody2D.velocity * 0.99f * Time.fixedDeltaTime);
    rigidbody2D.AddTorque(displacementMultiplier * -rigidbody2D.angularVelocity * 0.5f * Time.fixedDeltaTime);
      
       
    }
}
