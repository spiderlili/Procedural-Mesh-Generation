using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
public class MeshTangentDisplayer : MonoBehaviour 
{
	[SerializeField] private bool drawTangents;
	[SerializeField] private float tangentLength = 0.5f;

	void OnDrawGizmosSelected()
	{
		if (drawTangents)
		{
			Mesh mesh = GetComponent<MeshFilter> ().sharedMesh;

			if (mesh != null)
			{
				for (int i=0; i<mesh.vertexCount; i++)
				{
					//change these to world space so they display tangents when move transform
					Vector3 vertex = transform.TransformPoint (mesh.vertices [i]);
					Vector3 tangent = transform.TransformDirection (mesh.tangents [i]);

					Gizmos.color = Color.red;
					Gizmos.DrawLine (vertex, vertex + tangentLength * tangent);
				}
			}
		}
	}
}
