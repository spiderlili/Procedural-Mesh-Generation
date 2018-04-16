using UnityEngine;
using System.Collections;

public class ProceduralLandscapeGenerator : AbstractLandscapeMeshGenerator 
{
	/*[SerializeField] private int xResolution = 20;
	[SerializeField] private int zResolution = 20;

	[SerializeField] private float meshScale = 1;
	[SerializeField] private float yScale = 1;

	[SerializeField, Range(1, 8)] private int octaves = 1;
	[SerializeField] private float lacunarity = 2;
	[SerializeField, Range(0, 1)] private float gain = 0.5f; //needs to be between 0 and 1 so that each octave contributes less to the final shape.
	[SerializeField] private float perlinScale = 1;

	[SerializeField] private FallOffType type;
	[SerializeField] private float fallOffSize;
	[SerializeField] private float seaLevel;*/

	[SerializeField] private float uvScale = 1;

	[SerializeField] private Gradient gradient;
	[SerializeField] private float gradMin = -2;
	[SerializeField] private float gradMax = 5;


	protected override void SetMeshNums ()
	{
		numVertices = (xResolution + 1) * (zResolution + 1);  //number of vertices in x direction multiplied by number in z
		numTriangles = 6 * xResolution * zResolution; //This is 3 ints per geometric triangle * 2 geometric triangles per square * the number of triangles needed in the x direction * in the z direction
	}

	protected override void SetVertices ()
	{
		float xx, y, zz = 0;
		NoiseGenerator noise = new NoiseGenerator (octaves, lacunarity, gain, perlinScale);

		for (int z=0; z <= zResolution; z++)
		{
			for (int x = 0; x <= xResolution; x++) 
			{
				xx = ((float)x / xResolution) * meshScale;
				zz = ((float)z / zResolution) * meshScale; 

				y = yScale * noise.GetFractalNoise (xx, zz);
				y = FallOff ((float)x, y, (float)z);
				vertices.Add (new Vector3 (xx, y, zz));
			}
		}
	}

	protected override void SetTriangles ()
	{
		int triCount = 0;
		for (int z = 0; z < zResolution; z++) 
		{
			for (int x = 0; x < xResolution; x++) 
			{
				triangles.Add (triCount);
				triangles.Add (triCount + xResolution + 1);
				triangles.Add (triCount + 1);

				triangles.Add (triCount + 1);
				triangles.Add (triCount + xResolution + 1);
				triangles.Add (triCount + xResolution + 2);

				triCount++;
			}
			triCount++;
		}
	}


	protected override void SetUVs ()
	{
		for (int z = 0; z <= zResolution; z++) 
		{
			for (int x = 0; x <= xResolution; x++) 
			{
				uvs.Add(new Vector2 (x / (uvScale * xResolution), z / (uvScale * zResolution)));
			}
		}
	}

	protected override void SetVertexColours ()	
	{
		float diff = gradMax - gradMin;
		for (int i=0; i<numVertices; i++)
		{
			vertexColours.Add(gradient.Evaluate ((vertices[i].y - gradMin)/diff));
		}
	}


	protected override void SetNormals ()	{	}
	protected override void SetTangents ()	{	}

}
