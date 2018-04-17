using UnityEngine;
using System.Collections;

//generate interesting terrain made up of quad shapes in a long line for a flat 2D mesh  

public class TerrainGenerator2D : AbstractMeshGenerator 
{
//private variables which are visible in the inspector for generating fractal noise
	[SerializeField] private int resolution = 20;

	[SerializeField] private float xScale = 1;
	[SerializeField] private float yScale = 1;

	[SerializeField] private float meshHeight = 1;

	[SerializeField, Range(1, 8)] private int octaves = 1; //won't notice the effect if higher than 8
	//lacunarity determines how quickly the frequency increases for each successive octave in a Perlin-noise function
	[SerializeField] private float lacunarity = 2; //For best results, set the lacunarity to a number between 1.5 and 3.5.
	[SerializeField, Range(0, 1)] private float gain = 0.5f; //needs to be between 0 and 1 so that each octave contributes less to the final shape.
	[SerializeField] private float perlinScale = 1;

	[SerializeField] private int seed;

	[SerializeField] private bool uvFollowSurface;
	[SerializeField] private float uvScale = 1;
	[SerializeField] private float numTexPerSquare = 1;

	[SerializeField] private int sortingOrder = 0;

	protected override void SetMeshNums ()
	{
		numVertices = 2 * resolution; //resolution number of vertices across the top, *2 for top+bottom - Twice the resolution
		numTriangles = 6 * (resolution - 1); //This is 3 ints per geometric triangle * 2 geometric triangles per square * the resolution - 1 (number of vertices across the top - 1 to get the number of sections)
	}

	protected override void SetVertices ()
	{
		float x, y = 0;
		Vector3[] vs = new Vector3[numVertices]; //store the vertices in the order of top row then bottom row

		Random.InitState (seed); //editable control for variation - different seeds give different series of random numbers 
		NoiseGenerator noise = new NoiseGenerator (octaves, lacunarity, gain, perlinScale); 
		
		
		for (int i=0; i<resolution; i++)
		{
			x = ((float)i / resolution) * xScale; //cast i to a float
			y = yScale * noise.GetFractalNoise (x, 0); //noise.GetPerlinNoise

			//top - add the top vertices to the ith index of vs, z component is 0 for 2D
			vs [i] = new Vector3 (x, y, 0);
			//bottom
			vs [i + resolution] = new Vector3 (x, y - meshHeight, 0);
		}

		vertices.AddRange (vs); //add all of the vs to the vertices list 
	}

	protected override void SetTriangles ()
	{
		for (int i = 0; i < resolution-1; i++) 
		{
		//makes the bottom left triangle
			triangles.Add (i); //add the triangles for one quad shape in each loop
			triangles.Add (i + resolution + 1);
			triangles.Add (i + resolution);
			
		//makes the top right triangle
			triangles.Add (i);
			triangles.Add (i + 1);
			triangles.Add (i + resolution + 1);
		}
	}

	//add simple UVs to the 2D terrain to be able to add a textured material.
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
			//top row of the UVs - the x value = the ith vertices' x component, the y value = the ith vertices' y component
			//divide by uvScale for more control
				uvsArray [i] = new Vector2 (vertices [i].x/uvScale, vertices [i].y/uvScale);
			//bottomo row of the UVs - the same vector but the y component should be from i+resolution element's vertices
			//these UVs will mean the texture is simply repeated and oriented upright
				uvsArray [i + resolution] = new Vector2 (vertices [i].x/uvScale, vertices [i + resolution].y/uvScale);
			}
		}
		//add all of the UV's array to the uvs list
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
