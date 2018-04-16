using UnityEngine;
using System.Collections;

public class TerrainGenerator2D : AbstractMeshGenerator 
{
	[SerializeField] private int resolution = 20;

	[SerializeField] private float xScale = 1;
	[SerializeField] private float yScale = 1;

	[SerializeField] private float meshHeight = 1;

	[SerializeField, Range(1, 8)] private int octaves = 1;
	[SerializeField] private float lacunarity = 2;
	[SerializeField, Range(0, 1)] private float gain = 0.5f; //needs to be between 0 and 1 so that each octave contributes less to the final shape.
	[SerializeField] private float perlinScale = 1;

	[SerializeField] private int seed;

	[SerializeField] private bool uvFollowSurface;
	[SerializeField] private float uvScale = 1;
	[SerializeField] private float numTexPerSquare = 1;

	[SerializeField] private int sortingOrder = 0;

	protected override void SetMeshNums ()
	{
		numVertices = 2 * resolution; //there are resolution number of vertices across the top, *2 for top+bottom
		numTriangles = 6 * (resolution - 1); //This is 3 ints per geometric triangle * 2 geometric triangles per square * the resolution - 1 (number of vertices across the top - 1 to get the number of sections)
	}

	protected override void SetVertices ()
	{
		float x, y = 0;
		Vector3[] vs = new Vector3[numVertices];

		Random.InitState (seed);
		NoiseGenerator noise = new NoiseGenerator (octaves, lacunarity, gain, perlinScale);

		for (int i=0; i<resolution; i++)
		{
			x = ((float)i / resolution) * xScale;
			y = yScale * noise.GetFractalNoise (x, 0);

			//top
			vs [i] = new Vector3 (x, y, 0);
			//bottom
			vs [i + resolution] = new Vector3 (x, y - meshHeight, 0);
		}

		vertices.AddRange (vs);
	}

	protected override void SetTriangles ()
	{
		for (int i = 0; i < resolution-1; i++) 
		{
			triangles.Add (i);
			triangles.Add (i + resolution + 1);
			triangles.Add (i + resolution);

			triangles.Add (i);
			triangles.Add (i + 1);
			triangles.Add (i + resolution + 1);
		}
	}


	protected override void SetUVs ()
	{
		meshRenderer.sortingOrder = sortingOrder;

		Vector2[] uvsArray = new Vector2[numVertices];

		for (int i=0; i<resolution; i++)
		{
			if (uvFollowSurface)
			{
				uvsArray [i] = new Vector2 (i * numTexPerSquare / uvScale, 1);
				uvsArray [i + resolution] = new Vector2 (i * numTexPerSquare / uvScale, 0);	
			}
			else
			{
				uvsArray [i] = new Vector2 (vertices [i].x/uvScale, vertices [i].y/uvScale);
				uvsArray [i + resolution] = new Vector2 (vertices [i].x/uvScale, vertices [i + resolution].y/uvScale);
			}
		}

		uvs.AddRange (uvsArray);
	}

	protected override void SetNormals ()	
	{
		SetGeneralNormals ();
	}
	protected override void SetTangents ()	
	{
		SetGeneralTangents ();
	}
	protected override void SetVertexColours ()	{	}
}
