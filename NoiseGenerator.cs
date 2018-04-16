using UnityEngine;
using System.Collections;

public class NoiseGenerator
{
	private int octaves; //can also be called layers. Increasing this increases the level of detail in the terrain.
	private  float lacunarity; //determines how fast the frequency changes for each octave.
	private float gain; //determines how fast the amplitude changes for each octave. Also called persistence.
	private float perlinScale;

	public NoiseGenerator(){}

	public NoiseGenerator(int octaves, float lacunarity, float gain, float perlinScale)
	{
		this.octaves = octaves;
		this.lacunarity = lacunarity;
		this.gain = gain;
		this.perlinScale = perlinScale;
	}

	public float GetValueNoise()
	{
		return Random.value;
	}

	public float GetPerlinNoise(float x, float z)
	{
		//Mathf.PerlinNoise gives a float between 0 and 1. For better fractal terrain, change this to values between -1 and +1
		return (2 * Mathf.PerlinNoise (x, z) - 1);
	}

	public float GetFractalNoise(float x, float z)
	{
		float fractalNoise = 0;

		float frequency = 1;
		float amplitude = 1;

		for (int i=0; i<octaves; i++)
		{
			float xVal = x * frequency * perlinScale;
			float zVal = z * frequency * perlinScale;

			fractalNoise += amplitude * GetPerlinNoise (xVal, zVal);

			frequency *= lacunarity;
			amplitude *= gain;
		}

		return fractalNoise;
	}
}
