using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
public class Arc : PoolObject {

	[Range(0.0f, 360.0f)]
	public float startAngle = 0.0f;
	
	[Range(0.0f, 2.0f)]
	public float sidesPerAngle = 0.1f;

	[Range(0.1f, 360.0f)]
	public float arcAngle = 360.0f;

	[Range(0, 36)]
	public int arcSides = 0;

	public float arcWidth = 0.5f;
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

		arcAngle = 360.0f;
		arcSides = 0;
		arcWidth = 0.5f;
		arcRadius = 0.5f;
		startAngle = 0.0f;
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

		viewMesh = CreateArcMesh ("Square", startAngle, resSides, arcWidth, arcRadius, arcAngle); 
		viewMeshFilter.mesh = viewMesh;

		arcMaterial = meshRenderer.sharedMaterial;
		arcMaterial.color = arcColor;

	}

	public void RedrawArcMesh() {
		DrawArcMesh ();
	}
	
	public Mesh CreateArcMesh(string name, float startAngle, int sides, float width, float radius, float arcAngle) {

		Mesh mesh = new Mesh ();
		mesh.name = name;

		sides = (int)Mathf.Clamp ((float)sides, 3, Mathf.Infinity);

		ArcMeshInfo arcInfo = new ArcMeshInfo (startAngle, sides, width, arcAngle, radius);

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

		public Vector3 DirFromAngle(float angleInDegrees) {
			return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad),0,Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
		}
		
		public ArcMeshInfo(float startAngle, int sides, float width, float arcAngle, float radius) {
			int vertexCount = sides * 2;
			bool isCircle = arcAngle == 360.0f;
			int arcVertex = isCircle ? sides * 2 : sides * 2 + 2;
			vertices = new Vector3[arcVertex];
			triangles = new int[vertexCount * 3];
			float angleStep = arcAngle / sides;
			int steps = isCircle ? sides : sides + 1;
			for (int i = 0; i < steps; i++) {
				
				float angle = i * angleStep + startAngle;
				Vector3 dir = DirFromAngle(angle);
				vertices[i * 2] = dir * (width > radius ? 0 : radius - width);
				vertices[i * 2 + 1] = dir * radius;
				int ti = i * 6; //triangle index
				
				if (!isCircle) {

					if (i < sides) {
						triangles[ti] = i * 2 + 1;
						triangles[ti + 1] = i * 2 + 2;
						triangles[ti + 2] = i * 2;
						
						triangles[ti + 3] = i * 2 + 3;
						triangles[ti + 4] = i * 2 + 2;
						triangles[ti + 5] = i * 2 + 1;
					}
					
				} else {
					
					if (i == sides - 1) {
						triangles[ti] = i * 2 + 1;
						triangles[ti + 1] = 0;
						triangles[ti + 2] = i * 2;
						
						triangles[ti + 3] = 1;
						triangles[ti + 4] = 0;
						triangles[ti + 5] = i * 2 + 1;
						
					} else {
						
						triangles[ti] = i * 2 + 1;
						triangles[ti + 1] = i * 2 + 2;
						triangles[ti + 2] = i * 2;
						
						triangles[ti + 3] = i * 2 + 3;
						triangles[ti + 4] = i * 2 + 2;
						triangles[ti + 5] = i * 2 + 1;
					}
				}
			}
		}
	}
}
