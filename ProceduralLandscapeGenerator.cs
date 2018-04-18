using UnityEngine;
using System.Collections;

public class ProceduralLandscapeGenerator : AbstractLandscapeMeshGenerator 
{
	//control both the x and z resolutions
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


	protected override void SetMeshNums (){
	//form a grid
		numVertices = (xResolution + 1) * (zResolution + 1);  //set the number of vertices in x direction - multiplied by number in z
		numTriangles = 6 * xResolution * zResolution; //This is 3 ints per geometric triangle * 2 geometric triangles per square * the number of triangles needed in the x direction * in the z direction
	}

	protected override void SetVertices ()
	{
		float xx, y, zz = 0; //used to calculate thte grid and the noise
		NoiseGenerator noise = new NoiseGenerator (octaves, lacunarity, gain, perlinScale);

		for (int z=0; z <= zResolution; z++)
		{
			for (int x = 0; x <= xResolution; x++) 
			{
			//meshScale can't eqaul the resolution variables - it makes the x and z values ints when put into the perlin noise calculation
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
		int triCount = 0; //add the correct number of triangles which increase in each x and z loop
		for (int z = 0; z < zResolution; z++) 
		{
			for (int x = 0; x < xResolution; x++) 
			{
			//grid is drawn from the bottom left to the right and forwards
			//the first bottom left triangle
				triangles.Add (triCount);
				triangles.Add (triCount + xResolution + 1);
				triangles.Add (triCount + 1);

			//the other triangle
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
			//add a vector2 directly to the UVs list and space the uvs out on a mesh
				uvs.Add(new Vector2 (x / (uvScale * xResolution), z / (uvScale * zResolution)));
			}
		}
	}

	protected override void SetVertexColours ()	
	{
	//use an editable gradient to pick which colour to drawn when
		float diff = gradMax - gradMin; //control how the gradient is used
		for (int i=0; i<numVertices; i++)
		{
		//add the vertexColours list
		//pass in the vertices height at the current vertex - the minimum value / diff
			vertexColours.Add(gradient.Evaluate ((vertices[i].y - gradMin)/diff));
		}
	}


	protected override void SetNormals ()	{	}
	protected override void SetTangents ()	{	} //add the ability to use normal maps

}
