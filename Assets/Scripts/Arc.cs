using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
public class Arc : PoolObject {

    [Range(0.001f, float.MaxValue)]
	//public float startAngle = 0.0f;

    public float arcHeight = 0.1f;
	
	[Range(0.0f, 2.0f)]
	public float sidesPerAngle = 0.1f;

	[Range(1, 360)]
    public int arcAngle = 360;

	[Range(0, 36)]
	public int arcSides = 0;

    //[Range(0.001f, float.MaxValue)]
	public float arcWidth = 0.5f;

    //[Range(0.001f, float.MaxValue)]
	public float arcRadius = 1.5f;
	
	MeshFilter viewMeshFilter;
	MeshRenderer meshRenderer;
	Mesh viewMesh;
	public Color arcColor = Color.white;


	public float growingVelocity = 0.0f;
	public float rotationVelocity = 0.0f;

	Material arcMaterial;

	void Awake () {
		viewMeshFilter = GetComponent<MeshFilter> ();
		meshRenderer = GetComponent<MeshRenderer> ();
	}

	void Start () {
		DrawArcMesh ();
	}

	void FixedUpdate() {

	}


	void Update() {
		float deltaSize = growingVelocity * Time.deltaTime;
		arcRadius += deltaSize;
		RedrawArcMesh();

		Vector3 rotateAround = Vector3.zero;

		if (transform.parent != null) {
			rotateAround = transform.parent.position;
		}
		
		transform.RotateAround(rotateAround, Vector3.up, rotationVelocity);
		
	}


	public override void OnObjectReuse ()
	{
		base.OnObjectReuse ();
		growingVelocity = 0.0f;
		rotationVelocity = 0.0f;

		arcAngle = 360;
		arcSides = 0;
		arcWidth = 0.5f;
		arcRadius = 0.5f;
		//startAngle = 0.0f;
		sidesPerAngle = 0.1f;
	}


	public void DrawArcMesh() {

		int resSides = arcSides;
		if (arcSides < 3) {
			resSides = (int)Mathf.RoundToInt(arcAngle * sidesPerAngle);
		}

		if (viewMeshFilter == null) {
			viewMeshFilter = GetComponent<MeshFilter> ();
		}

		if (meshRenderer == null) {
			meshRenderer = GetComponent<MeshRenderer> ();
		}

		viewMesh = CreateArcMesh ("Square", resSides, arcWidth, arcRadius, arcAngle); 
		viewMeshFilter.mesh = viewMesh;

		arcMaterial = meshRenderer.sharedMaterial;
		arcMaterial.color = arcColor;

	}

	public void RedrawArcMesh() {
		DrawArcMesh ();
	}
	
	public Mesh CreateArcMesh(string name, int sides, float width, float radius, int arcAngle) {

		Mesh mesh = new Mesh ();
		mesh.name = name;

		sides = (int)Mathf.Clamp ((float)sides, 3, Mathf.Infinity);

        ArcMeshInfo arcInfo = new ArcMeshInfo (sides, width, arcAngle, radius, arcHeight);

		mesh.vertices = arcInfo.vertices;
		mesh.triangles = arcInfo.triangles;

		mesh.RecalculateNormals ();
		return mesh;
	}
	
	public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal) {
		if (!angleIsGlobal) {
			angleInDegrees += transform.eulerAngles.y;
		}
		return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad),0,Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
	}

	public struct ArcMeshInfo {
		public Vector3[] vertices;
		public int[] triangles;

        struct MeshInfo {
            public Vector3[] vertices;
            public int[] triangles;
        }

        public static Vector3 DirFromAngle(float angleInDegrees) {
			return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad),0,Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
		}
		
        public ArcMeshInfo(int sides, float width, int arcAngle, float radius, float arcHeight) {

            vertices = new Vector3[0];
            triangles = new int[0];

			bool isCircle = arcAngle == 360;
            float angleStep = arcAngle / sides;
            //int steps = isCircle ? sides : sides + 1;
            int steps = sides + 1;

            MeshInfo top =    GetArcMesh(angleStep, sides, width, radius, steps, arcHeight, false, isCircle);
            MeshInfo bottom = GetArcMesh(angleStep, sides, width, radius, steps, 0, true, isCircle);

            for (int i = 0; i < bottom.triangles.Length; i++) {
                bottom.triangles[i] += top.vertices.Length;
            }

            List<int> tris = new List<int>();
            tris.AddRange(top.triangles);
            tris.AddRange(bottom.triangles);
            int[] _tris = tris.ToArray();

            List<Vector3> verts = new List<Vector3>();
            verts.AddRange(top.vertices);
            verts.AddRange(bottom.vertices);
            Vector3[] _verts = verts.ToArray();


            vertices = _verts;
            triangles = _tris;
		}

        static MeshInfo GetArcMesh(float angleStep,
                        int sides,
                        float width,
                        float radius,
                        int steps,
                        float yPos,
                        bool reverse,
                        bool isCircle)
        {
            
            int vertexCount = sides * 2;
            //int arcVertex = isCircle ? sides * 2 : sides * 2 + 2;
            int arcVertex = sides * 2 + 2;

            Vector3[] _vertices = new Vector3[arcVertex];
            int[] _triangles = new int[vertexCount * 3];



            for (int i = 0; i < steps; i++)
            {

                float angle = i * angleStep;
                Vector3 dir = DirFromAngle(angle);

                Vector3 inner = dir * (width > radius ? 0 : radius - width);
                Vector3 outer = dir * radius;

                inner.y = yPos;
                outer.y = yPos;

                _vertices[i * 2] = inner;
                _vertices[i * 2 + 1] = outer;
                int ti = i * 6; //triangle index



                //if (!isCircle)
                {
                    if (i < sides) {

                        if (reverse) {
                            
                            _triangles[ti]     = i * 2 + 1;
                            _triangles[ti + 1] = i * 2 + 0;
                            _triangles[ti + 2] = i * 2 + 2;

                            _triangles[ti + 3] = i * 2 + 3;
                            _triangles[ti + 4] = i * 2 + 1;
                            _triangles[ti + 5] = i * 2 + 2;

                        } else {
                            
                            _triangles[ti]     = i * 2 + 1;
                            _triangles[ti + 1] = i * 2 + 2;
                            _triangles[ti + 2] = i * 2 + 0;

                            _triangles[ti + 3] = i * 2 + 3;
                            _triangles[ti + 4] = i * 2 + 2;
                            _triangles[ti + 5] = i * 2 + 1;
                        }
                    }

                }
                //else {

                    //if (i == sides - 1)
                    //{

                    //    if (reverse) {

                    //        _triangles[ti]     = i * 2 + 1;
                    //        _triangles[ti + 1] = i * 2;
                    //        _triangles[ti + 2] = 0;

                    //        _triangles[ti + 3] = 1;
                    //        _triangles[ti + 4] = i * 2 + 1;
                    //        _triangles[ti + 5] = 0;


                    //    } else {
                    //        _triangles[ti] = i * 2 + 1;
                    //        _triangles[ti + 1] = 0;
                    //        _triangles[ti + 2] = i * 2;

                    //        _triangles[ti + 3] = 1;
                    //        _triangles[ti + 4] = 0;
                    //        _triangles[ti + 5] = i * 2 + 1;
                    //    }
                    //}
                    //else
                    //{

                    //    if (reverse)
                    //    {

                    //        _triangles[ti] = i * 2 + 1;
                    //        _triangles[ti + 1] = i * 2 + 0;
                    //        _triangles[ti + 2] = i * 2 + 2;

                    //        _triangles[ti + 3] = i * 2 + 3;
                    //        _triangles[ti + 4] = i * 2 + 1;
                    //        _triangles[ti + 5] = i * 2 + 2;

                    //    }
                    //    else
                    //    {

                    //        _triangles[ti] = i * 2 + 1;
                    //        _triangles[ti + 1] = i * 2 + 2;
                    //        _triangles[ti + 2] = i * 2 + 0;

                    //        _triangles[ti + 3] = i * 2 + 3;
                    //        _triangles[ti + 4] = i * 2 + 2;
                    //        _triangles[ti + 5] = i * 2 + 1;
                    //    }

                       
                    //}
                //}
            }

            MeshInfo info = new MeshInfo();
            info.triangles = _triangles;
            info.vertices = _vertices;
            return info;
        }
	}
}
