using UnityEngine;
using System.Collections;

public class LowPolyLandscapeGenerator : AbstractLandscapeMeshGenerator 
{
	protected override void SetMeshNums ()
	{
		numTriangles = 6 * xResolution * zResolution; //This is 3 ints per geometric triangle * 2 geometric triangles per square * the number of triangles needed in the x direction * in the z direction
		numVertices = numTriangles; //3 vertices per geometric triangle.
	}

	protected override void SetVertices ()
	{
		NoiseGenerator noise = new NoiseGenerator (octaves, lacunarity, gain, perlinScale);

		int xx = 0;
		int zz = 0;
		bool isBottomTriangle = false;
		for (int vertexIndex = 0; vertexIndex < numVertices; vertexIndex++)
		{
			//Increment xx and zz appropriately

			//Check if it's a bottom or top of a triangle
			if (IsNewRow (vertexIndex))
			{
				isBottomTriangle = !isBottomTriangle;
			}

			//increase xx by one when it's a new position
			if (!IsNewRow (vertexIndex))
			{
				if (isBottomTriangle)
				{
					if (vertexIndex % 3 == 1)
					{
						xx++;
					}
				}
				else
				{
					if (vertexIndex % 3 == 2)
					{
						xx++;
					}
				}
			}

			//increase zz by one when it's a new row. Reset xx to zero.
			if (IsNewRow (vertexIndex))
			{
				//reset xx on new row
				xx = 0;

				//actually go up a level
				if (!isBottomTriangle)
				{
					zz++;
				}
			}

			//set vertex
			float xVal = ((float)xx / xResolution) * meshScale;
			float zVal = ((float)zz / zResolution) * meshScale; 

			float y = yScale * noise.GetFractalNoise (xVal, zVal);
			y = FallOff ((float)xx, y, (float)zz);
			vertices.Add (new Vector3 (xVal, y, zVal));
		}

	}

	private bool IsNewRow(int vertexIndex)
	{
		return vertexIndex % (3 * xResolution) == 0;
	}

	protected override void SetTriangles ()
	{
		int triCount = 0;
		for (int z = 0; z < zResolution; z++) 
		{
			for (int x = 0; x < xResolution; x++) 
			{
				triangles.Add (triCount);
				triangles.Add (triCount + 3*xResolution);
				triangles.Add (triCount + 1);

				triangles.Add (triCount + 2);
				triangles.Add (triCount + 3*xResolution + 1);
				triangles.Add (triCount + 3*xResolution + 2);

				triCount += 3;
			}
			triCount += 3 * xResolution;
		}
	}

	protected override void SetUVs (){}
	protected override void SetNormals ()	{	}
	protected override void SetTangents ()	{	}
	protected override void SetVertexColours ()	{}
}
