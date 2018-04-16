using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(MeshRenderer)), RequireComponent(typeof(MeshCollider))]
[ExecuteInEditMode]
public abstract class AbstractMeshGenerator : MonoBehaviour 
{
	[SerializeField] protected Material material;

	protected List<Vector3> vertices;
	protected List<int> triangles;

	protected List<Vector3> normals;
	protected List<Vector4> tangents;
	protected List<Vector2> uvs;
	protected List<Color32> vertexColours;

	protected int numVertices;
	protected int numTriangles;

	private MeshFilter meshFilter;
	protected MeshRenderer meshRenderer;
	private MeshCollider meshCollider;
	private Mesh mesh;

	void Update()
	{
		meshFilter = GetComponent<MeshFilter> ();
		meshRenderer = GetComponent<MeshRenderer> ();
		meshCollider = GetComponent<MeshCollider> ();

		meshRenderer.material = material;

		//initialise
		InitMesh();
		SetMeshNums ();

		//create the mesh
		CreateMesh();
	}

	protected abstract void SetMeshNums ();

	private bool ValidateMesh()
	{
		//build a string containing errors
		string errorStr = "";

		//check there are the correct number of triangles and vertices
		errorStr += vertices.Count == numVertices ? "" : "Should be " + numVertices + " vertices, but there are " + vertices.Count + ". ";
		errorStr += triangles.Count == numTriangles ? "" : "Should be " + numTriangles + " triangles, but there are " + triangles.Count + ". ";

		//Check there are the correct number of normals - there should be the same number of normals as there are vertices. If we're not manually calculating normals, there will be 0.
		//Similarly for tangents, uvs, vertexColours
		errorStr += (normals.Count == numVertices || normals.Count == 0) ? "" : "Should be " + numVertices + " normals, but there are " + normals.Count + ". ";
		errorStr += (tangents.Count == numVertices || tangents.Count == 0) ? "" : "Should be " + numVertices + " tangents, but there are " + tangents.Count + ". ";
		errorStr += (uvs.Count == numVertices || uvs.Count == 0) ? "" : "Should be " + numVertices + " uvs, but there are " + uvs.Count + ". ";
		errorStr += (vertexColours.Count == numVertices || vertexColours.Count == 0) ? "" : "Should be " + numVertices + " vertexColours, but there are " + vertexColours.Count + ". ";

		bool isValid = string.IsNullOrEmpty (errorStr);
		if (!isValid)
		{
			Debug.LogError ("Not drawing mesh. " + errorStr);
		}

		return isValid;
	}

	private void InitMesh()
	{
		vertices = new List<Vector3>();
		triangles = new List<int> ();

		//optional
		normals = new List<Vector3> ();
		tangents = new List<Vector4> ();
		uvs = new List<Vector2> ();
		vertexColours = new List<Color32> ();
	}

	private void CreateMesh()
	{
		mesh = new Mesh ();

		SetVertices ();
		SetTriangles ();

		SetNormals ();
		SetUVs ();
		SetTangents ();
		SetVertexColours ();

		if (ValidateMesh ())
		{
			//This should always be done vertices first, triangles second - Unity requires this.
			mesh.SetVertices (vertices);
			mesh.SetTriangles (triangles, 0);

			if (normals.Count == 0)
			{
				mesh.RecalculateNormals ();
				normals.AddRange (mesh.normals);
			}

			mesh.SetNormals (normals);
			mesh.SetUVs (0, uvs);
			mesh.SetTangents (tangents);
			mesh.SetColors (vertexColours);

			meshFilter.mesh = mesh;
			meshCollider.sharedMesh = mesh;
		}
	}

	protected abstract void SetVertices();
	protected abstract void SetTriangles();

	protected abstract void SetNormals();
	protected abstract void SetTangents();
	protected abstract void SetUVs();
	protected abstract void SetVertexColours();

	protected void SetGeneralNormals()
	{
		int numGeometricTriangles = numTriangles / 3;
		Vector3[] norms = new Vector3[numVertices];
		int index = 0;
		for (int i=0; i<numGeometricTriangles; i++)
		{
			//the triangle ints that make up a geometric triangle 
			int triA = triangles [index];
			int triB = triangles [index + 1];
			int triC = triangles [index + 2];

			//directions from index-th vertex that make up the triangle
			Vector3 dirA = vertices[triB] - vertices[triA];
			Vector3 dirB = vertices[triC] - vertices[triA];

			//Normal needs to come out of the plane - use the left hand rule to work out the order of the cross product
			Vector3 normal = Vector3.Cross(dirA, dirB);

			//add the normals for each vertex cumulatively so that shared vertices are added together.
			norms[triA] += normal;
			norms[triB] += normal;
			norms[triC] += normal;

			index += 3;
		}

		//go through the vertices and normalise the norms (as they are sums)
		for (int i=0; i<numVertices; i++)
		{
			normals.Add (norms [i].normalized);
		}
	}

	protected void SetGeneralTangents()
	{
		if (uvs.Count == 0 || normals.Count == 0)
		{
			print ("Set UVs and Normals before adding tangents");
			return;
		}

		int numGeometricTriangles = numTriangles / 3;
		Vector3[] tans = new Vector3[numVertices];
		Vector3[] bitans = new Vector3[numVertices];
		int index = 0;
		for (int i = 0; i < numGeometricTriangles; i++) 
		{
			//the triangle ints that make up a geometric triangle 
			int triA = triangles [index];
			int triB = triangles [index + 1];
			int triC = triangles [index + 2];

			//the corresponding UVs
			Vector2 uvA = uvs[triA];
			Vector2 uvB = uvs[triB];
			Vector2 uvC = uvs[triC];

			//directions from index-th vertex that make up the triangle
			Vector3 dirA = vertices[triB] - vertices[triA];
			Vector3 dirB = vertices[triC] - vertices[triA];

			//from matrix equation
			Vector2 uvDiffA = new Vector2 (uvB.x - uvA.x, uvC.x - uvA.x);
			Vector2 uvDiffB = new Vector2 (uvB.y - uvA.y, uvC.y - uvA.y);

			float determinant = 1f / (uvDiffA.x * uvDiffB.y - uvDiffA.y * uvDiffB.x);
			Vector3 sDir = determinant * (new Vector3 (uvDiffB.y * dirA.x - uvDiffB.x * dirB.x, uvDiffB.y * dirA.y - uvDiffB.x * dirB.y, uvDiffB.y * dirA.z - uvDiffB.x * dirB.z));
			Vector3 tDir = determinant * (new Vector3 (uvDiffA.x * dirB.x - uvDiffA.y * dirA.x, uvDiffA.x * dirB.y - uvDiffA.y * dirA.y, uvDiffA.x * dirB.z - uvDiffA.y * dirA.z));

			//add the tangents for each vertex cumulatively so that all contributions are added
			tans[triA] += sDir;
			tans[triB] += sDir;
			tans[triC] += sDir;

			//and for bitans
			bitans[triA] += tDir;
			bitans[triB] += tDir;
			bitans[triC] += tDir;

			index += 3;
		}

		//go through the vertices and normalise the tangents (as they are sums)
		for (int i=0; i<numVertices; i++)
		{
			Vector3 normal = normals [i];
			Vector3 tan = tans [i];

			//Use the Gram-Schmidt algorithm to make normal and tan orthogonal, then normalise
			Vector3 tangent3 = (tan - Vector3.Dot(normal, tan) * normal).normalized;
			Vector4 tangent = tangent3;

			//calculate handedness
			tangent.w = Vector3.Dot(Vector3.Cross(normal, tan), bitans[i]) < 0f ? -1f : 1f;
			tangents.Add (tangent);
		}
	}

}
