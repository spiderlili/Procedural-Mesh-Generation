using UnityEngine;
using System.Collections;

//abstract as shouldn't be able to instantiate this class
public abstract class AbstractLandscapeMeshGenerator : AbstractMeshGenerator 
{
	[SerializeField] protected int xResolution = 20;
	[SerializeField] protected int zResolution = 20;

	[SerializeField] protected float meshScale = 1;
	[SerializeField] protected float yScale = 1;

	[SerializeField, Range(1, 8)] protected int octaves = 1;
	[SerializeField] protected float lacunarity = 2;
	[SerializeField, Range(0, 1)] protected float gain = 0.5f; //needs to be between 0 and 1 so that each octave contributes less to the final shape.
	[SerializeField] protected float perlinScale = 1;

	[SerializeField] protected FallOffType type;
	[SerializeField] protected float fallOffSize;
	[SerializeField] protected float seaLevel;

	protected float FallOff(float x, float height, float z)
	{
		//shift the coordinates to the centre
		x = x - xResolution / 2f;
		z = z - zResolution / 2f;

		float fallOff = 0;

		switch (type)
		{
		case FallOffType.None:
			return height;
		case FallOffType.Circular:
			fallOff = Mathf.Sqrt (x * x + z * z) / fallOffSize;
			return GetHeight (fallOff, height);
		case FallOffType.RoundedSquare:
			fallOff = Mathf.Sqrt (x * x * x * x + z * z * z * z) / fallOffSize;
			return GetHeight (fallOff, height);
		default:
			print ("Unrecognised Falloff type: " + type);
			return height;
		}
	}

	private float GetHeight(float fallOff, float height)
	{
		//falloff is 0,0 in the centre and then increases outwards
		if (fallOff < 1)
		{
			//Make the gradient softer. This is the smoothstep function
			fallOff = fallOff * fallOff * (3 - 2 * fallOff);

			//gradiate the height
			height = height - fallOff * (height - seaLevel);
		}
		else
		{
			height = seaLevel;
		}

		return height;
	}

	protected override void SetMeshNums (){}
	protected override void SetVertices (){}
	protected override void SetTriangles (){}
	protected override void SetUVs (){}
	protected override void SetNormals ()	{	}
	protected override void SetTangents ()	{	}
	protected override void SetVertexColours ()	{}
}
