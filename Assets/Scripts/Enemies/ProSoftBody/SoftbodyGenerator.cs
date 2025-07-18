using System.Collections.Generic;
using GK;
using UnityEngine;

public class SoftbodyGenerator : MonoBehaviour
{
    private MeshFilter originalMeshFilter;
    private List<Vector3> writableVertices { get; set; }
    private List<Vector3> writableVerticesConvaxed;//{ get; set; }
    private List<Vector3> writableNormals { get; set; }
    private List<Vector3> writableNormalsConvaxed;//{ get; set; }

    private readonly List<SphereCollider> sphereColliders = new();
    private int[] writableTris { get; set; }
    private List<int> writableTrisConvaxed;// { get; set; }
    private Mesh writableMesh;

    private List<GameObject> phyisicedVertexes;
    private Dictionary<int, int> vertexDictunery;
    /** public variable to controll softbody **/
    public bool runOptimizedVersion = false;
    public float _collissionSurfaceOffset = 0.1f;
    public float collissionSurfaceOffset
    {
        get => _collissionSurfaceOffset;
        set
        {
            _collissionSurfaceOffset = value;
            if (phyisicedVertexes != null)
            {
                foreach (SphereCollider sCollider in sphereColliders)
                {
                    sCollider.radius = _collissionSurfaceOffset;
                }
            }
        }
    }

    public SoftJointLimitSpring springlimit;
    public float _softness = 1f;
    public float softness
    {
        get => _softness;
        set
        {
            _softness = value;
            if (phyisicedVertexes != null)
            {
                foreach (GameObject gObject in phyisicedVertexes)
                {
                    gObject.GetComponent<SpringJoint>().spring = _softness;
                }
            }

            springlimit.spring = _softness;
        }
    }
    public float _damp = .2f;
    public float damp
    {
        get => _damp;
        set
        {
            _damp = value;
            if (phyisicedVertexes != null)
            {
                foreach (GameObject gObject in phyisicedVertexes)
                {
                    gObject.GetComponent<SpringJoint>().damper = _damp;
                }
            }

            springlimit.damper = _damp;
        }
    }
    public float _mass = 1f;
    public float mass
    {
        get => _mass;
        set
        {
            _mass = value;
            if (phyisicedVertexes != null)
            {
                foreach (GameObject gObject in phyisicedVertexes)
                {
                    gObject.GetComponent<Rigidbody>().mass = _mass;
                }
            }
        }
    }

    private bool _debugMode = false;
    public bool debugMode
    {
        get => _debugMode;
        set
        {
            _debugMode = value;
            if (_debugMode == false)
            {
                if (phyisicedVertexes != null)
                {
                    foreach (GameObject gObject in phyisicedVertexes)
                    {
                        gObject.hideFlags = HideFlags.HideAndDontSave;
                    }
                }

                if (centerOfMasObj != null)
                {
                    centerOfMasObj.hideFlags = HideFlags.HideAndDontSave;
                }
            }
            else
            {
                if (phyisicedVertexes != null)
                {
                    foreach (GameObject gObject in phyisicedVertexes)
                    {
                        gObject.hideFlags = HideFlags.None;
                    }
                }

                if (centerOfMasObj != null)
                {
                    centerOfMasObj.hideFlags = HideFlags.None;
                }
            }

        }
    }


    private float _physicsRoughness = 4;
    public float physicsRoughness
    {
        get => _physicsRoughness;
        set
        {
            _physicsRoughness = value;
            if (phyisicedVertexes != null)
            {
                foreach (GameObject gObject in phyisicedVertexes)
                {
                    gObject.GetComponent<Rigidbody>().linearDamping = physicsRoughness;
                }
            }
        }
    }
    private bool _gravity = true;
    public bool gravity
    {
        get => _gravity;
        set
        {
            _gravity = value;
            if (phyisicedVertexes != null)
            {
                foreach (GameObject gObject in phyisicedVertexes)
                {
                    gObject.GetComponent<Rigidbody>().useGravity = _gravity;
                }
            }

            if (centerOfMasObj != null)
            {
                centerOfMasObj.GetComponent<Rigidbody>().useGravity = _gravity;
            }
        }
    }
    public GameObject centerOfMasObj = null;
    private void Awake()
    {


        writableVertices = new List<Vector3>();
        writableVerticesConvaxed = new List<Vector3>();
        writableNormals = new List<Vector3>();
        writableNormalsConvaxed = new List<Vector3>();
        phyisicedVertexes = new List<GameObject>();

        writableTrisConvaxed = new List<int>();

        originalMeshFilter = GetComponent<MeshFilter>();
        originalMeshFilter.mesh.GetVertices(writableVertices);
        originalMeshFilter.mesh.GetNormals(writableNormals);
        writableTris = originalMeshFilter.mesh.triangles;



        Matrix4x4 localToWorld = transform.localToWorldMatrix;
        for (int i = 0; i < writableVertices.Count; ++i)
        {
            writableVertices[i] = localToWorld.MultiplyPoint3x4(writableVertices[i]);
        }

        if (runOptimizedVersion)
        {
            new ConvexHullCalculator().GenerateHull(
                 writableVertices
                 , false
                 , ref writableVerticesConvaxed, ref writableTrisConvaxed, ref writableNormalsConvaxed
                 );
            writableVertices = writableVerticesConvaxed;
            writableNormals = writableNormalsConvaxed;
            writableTris = writableTrisConvaxed.ToArray();
        }

        writableMesh = new Mesh();
        writableMesh.MarkDynamic();
        writableMesh.SetVertices(writableVertices);
        writableMesh.SetNormals(writableNormals);
        writableMesh.triangles = writableTris;
        originalMeshFilter.mesh = writableMesh;
        // remove duplicated vertex
        List<Vector3> _optimizedVertex = new();

        // first column = original vertex index , last column = optimized vertex index 
        vertexDictunery = new Dictionary<int, int>();
        for (int i = 0; i < writableVertices.Count; i++)
        {
            bool isVertexDuplicated = false;
            for (int j = 0; j < _optimizedVertex.Count; j++)
            {
                if (_optimizedVertex[j] == writableVertices[i])
                {
                    isVertexDuplicated = true;
                    vertexDictunery.Add(i, j);
                    break;
                }
            }

            if (!isVertexDuplicated)
            {
                _optimizedVertex.Add(writableVertices[i]);
                vertexDictunery.Add(i, _optimizedVertex.Count - 1);
            }
        }


        // create balls at each of vertex also center of mass
        foreach (Vector3 vertecs in _optimizedVertex)
        {
            GameObject _tempObj = new("Point " + _optimizedVertex.IndexOf(vertecs));

            if (!debugMode)
            {
                _tempObj.hideFlags = HideFlags.HideAndDontSave;
            }

            _tempObj.transform.parent = transform;
            _tempObj.transform.position = vertecs;


            // add collider to each of vertex ( sphere collider )
            SphereCollider sphereColider = _tempObj.AddComponent<SphereCollider>();
            sphereColider.radius = collissionSurfaceOffset;
            // add current collider to Collider list ;
            sphereColliders.Add(sphereColider);


            // add rigidBody to each of vertex
            Rigidbody _tempRigidBody = _tempObj.AddComponent<Rigidbody>();
            _tempRigidBody.mass = mass / _optimizedVertex.Count;
            _tempRigidBody.linearDamping = physicsRoughness;





            _tempObj.AddComponent<DebugColorGameObject>().Color = Random.ColorHSV();


            phyisicedVertexes.Add(_tempObj);
        }



        // calculate center of mass
        Vector3 tempCenter = Vector3.zero;

        foreach (Vector3 vertecs in _optimizedVertex)
        {
            tempCenter = new Vector3(tempCenter.x + vertecs.x, tempCenter.y + vertecs.y, tempCenter.z + vertecs.z);
        }

        Vector3 centerOfMass = new(
              tempCenter.x / _optimizedVertex.Count
            , tempCenter.y / _optimizedVertex.Count
            , tempCenter.z / _optimizedVertex.Count
        );
        // add center of mass vertex to OptimizedVertex list
        {
            GameObject _tempObj = new("centerOfMass");

            if (!debugMode)
            {
                _tempObj.hideFlags = HideFlags.HideAndDontSave;
            }

            _tempObj.transform.parent = transform;
            _tempObj.transform.position = centerOfMass;

            // add collider to center of mass as a sphere collider
            SphereCollider sphereColider = _tempObj.AddComponent<SphereCollider>();
            sphereColider.radius = collissionSurfaceOffset;
            // add current collider to Collider list ;
            sphereColliders.Add(sphereColider);

            // add rigidBody to center of mass as a sphere collider
            _ = _tempObj.AddComponent<Rigidbody>();

            centerOfMasObj = _tempObj;
        }

        // IGNORE COLLISTION BETWEEN ALL OF THE VERTEXES AND CENTER OFF MASS
        foreach (SphereCollider collider1 in sphereColliders)
        {
            foreach (SphereCollider collider2 in sphereColliders)
            {
                Physics.IgnoreCollision(collider1, collider2, true);
            }
        }

        // Extract Lines from quad of mesh
        List<Vector2Int> tempListOfSprings = new();
        bool isFirstTrisOfQuad = true;
        for (int i = 0; i < writableTris.Length; i += 3)
        {
            int index0 = vertexDictunery[writableTris[i]];
            int index1 = vertexDictunery[writableTris[i + 1]];
            int index2 = vertexDictunery[writableTris[i + 2]];

            tempListOfSprings.Add(new Vector2Int(index1, index2));
            // this System convert Tris To Quad
            if (isFirstTrisOfQuad)
            {
                tempListOfSprings.Add(new Vector2Int(index0, index1));
                isFirstTrisOfQuad = false;
            }
            else
            {
                tempListOfSprings.Add(new Vector2Int(index2, index0));
                isFirstTrisOfQuad = true;
            }
        }


        // distinct normal Duplicates with check revers
        for (int i = 0; i < tempListOfSprings.Count; i++)
        {
            bool isDuplicated = false;
            Vector2Int normal = tempListOfSprings[i];
            Vector2Int reversed = new(tempListOfSprings[i].y, tempListOfSprings[i].x);
            for (int j = 0; j < noDupesListOfSprings.Count; j++)
            {
                if (normal == tempListOfSprings[j])
                {
                    isDuplicated = true;
                    break;
                }
                else if (reversed == tempListOfSprings[j])
                {
                    isDuplicated = true;
                    break;
                }

            }
            if (isDuplicated == false)
            {
                noDupesListOfSprings.Add(tempListOfSprings[i]);
            }
        }

        // making Springs bodies
        foreach (Vector2Int jointIndex in noDupesListOfSprings)
        {
            GameObject thisGameObject = phyisicedVertexes[jointIndex.x];
            CharacterJoint thisBodyJoint = thisGameObject.AddComponent<CharacterJoint>();
            Rigidbody destinationBody = phyisicedVertexes[jointIndex.y].GetComponent<Rigidbody>();
            float distanceBetween = Vector3.Distance(thisGameObject.transform.position, destinationBody.transform.position);


            // configure current spring joint
            thisBodyJoint.connectedBody = destinationBody;
            SoftJointLimit jointlimitHihj = new()
            {
                bounciness = 1.1f,
                contactDistance = distanceBetween,
                limit = 10
            };

            SoftJointLimit jointlimitLow = new()
            {
                bounciness = 1.1f,
                contactDistance = distanceBetween,
                limit = -10
            };


            thisBodyJoint.highTwistLimit = jointlimitHihj;
            thisBodyJoint.lowTwistLimit = jointlimitLow;
            thisBodyJoint.swing1Limit = jointlimitLow;
            thisBodyJoint.swing2Limit = jointlimitHihj;


            //thisBodyJoint.

            springlimit.damper = damp;
            springlimit.spring = softness;

            thisBodyJoint.swingLimitSpring = springlimit;
            thisBodyJoint.twistLimitSpring = springlimit;

            if (!runOptimizedVersion)
            {
                thisBodyJoint.enableCollision = true;
            }
        }

        // Decelare Center of mass variable
        foreach (GameObject jointIndex in phyisicedVertexes)
        {
            SpringJoint destinationBodyJoint = jointIndex.AddComponent<SpringJoint>();
            _ = Vector3.Distance(
                  centerOfMasObj.transform.localPosition
                , destinationBodyJoint.transform.localPosition
            );

            destinationBodyJoint.connectedBody = centerOfMasObj.GetComponent<Rigidbody>();
            destinationBodyJoint.spring = softness;
            destinationBodyJoint.damper = damp;

            //destinationBodyJoint.massScale = 0.001f;
            //destinationBodyJoint.connectedMassScale = 0.001f;

            if (!runOptimizedVersion)
            {
                destinationBodyJoint.enableCollision = true;
            }
        }


    }
    private readonly List<Vector2Int> noDupesListOfSprings = new();
    public void Update()
    {
        if (debugMode)
        {
            foreach (Vector2Int jointIndex in noDupesListOfSprings)
            {
                Debug.DrawLine(
                    phyisicedVertexes[jointIndex.x].transform.position
                    , phyisicedVertexes[jointIndex.y].transform.position
                    , phyisicedVertexes[jointIndex.x].GetComponent<DebugColorGameObject>().Color
                );

            }
            foreach (Vector2Int jointIndex in noDupesListOfSprings)
            {
                Debug.DrawLine(
                      phyisicedVertexes[jointIndex.x].transform.position
                    , centerOfMasObj.transform.position
                    , Color.red
                );

            }
        }
        Vector3[] tempVertexes = new Vector3[originalMeshFilter.mesh.vertices.Length];
        for (int i = 0; i < tempVertexes.Length; i++)
        {
            tempVertexes[i] = phyisicedVertexes[vertexDictunery[i]].transform.localPosition;

        }
        originalMeshFilter.mesh.vertices = tempVertexes;
        originalMeshFilter.mesh.RecalculateBounds();
        //originalMeshFilter.mesh.RecalculateTangents();
        originalMeshFilter.mesh.RecalculateNormals();
    }

}

public class DebugColorGameObject : MonoBehaviour
{
    public Color Color { get; set; }
}

//[CustomEditor(typeof(SoftbodyGenerator))]
//public class LookAtPointEditor : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        SoftbodyGenerator softbody = target as SoftbodyGenerator;       
        
//        softbody.debugMode = EditorGUILayout.Toggle("#Debug mod", softbody.debugMode);
//        EditorGUILayout.Space();

//        string[] options = new string[] { "  version 1", "  version 2" };
        

//        softbody.gravity = EditorGUILayout.Toggle("Gravity", softbody.gravity);
//        softbody.mass = EditorGUILayout.FloatField("Mass(KG)", softbody.mass);
//        softbody.physicsRoughness = EditorGUILayout.FloatField("Drag (roughness)", softbody.physicsRoughness);
//        softbody.softness = EditorGUILayout.FloatField("Softbody hardness", softbody.softness);
//        softbody.damp = EditorGUILayout.FloatField("Softbody damper", softbody.damp);
//        softbody.collissionSurfaceOffset = EditorGUILayout.FloatField("Softbody Offset", softbody.collissionSurfaceOffset);
        
//    }
//}
