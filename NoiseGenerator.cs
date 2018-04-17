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

	//can call from another script to get a value
	public float GetValueNoise()
	{
	//use Unity's own random class to return a value between 0 - 1
		return Random.value; 
	}

	public float GetPerlinNoise(float x, float z) //coordinates for the landscapes(one of the value would be 0 for 2D terrains)
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
