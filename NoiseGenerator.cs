using UnityEngine;
using System.Collections;

//true fractal noise settings: lacunarity = 2, gain = 0.5, perlinScale = 1 
//these variables gives flexibility when generating landscapes

public class NoiseGenerator
{
	private int octaves; //can also be called layers. Increasing this increases the level of detail in the terrain.
	//beyond 8 octaves the differences becomes unnoticeable
	private  float lacunarity; //determines how fast the frequency changes for each octave.
	private float gain; //determines how fast the amplitude changes for each octave. Also called persistence.
	private float perlinScale; //overall noise scale


	public NoiseGenerator(){} 
	//if don't want fractal style noise with specified octave/gain, add an empty constructor
	//create an instance of the class when using it
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
		//Mathf.PerlinNoise gives a float between 0 and 1. 
		//For better fractal terrain, change this to values between -1 and +1
		return (2 * Mathf.PerlinNoise (x, z) - 1); //2 * to get values between 0 and 2 and -1 to get -1 and 1
	}

	public float GetFractalNoise(float x, float z)
	{
		float fractalNoise = 0;

		float frequency = 1;
		float amplitude = 1;

		//sum up the different octaves
		for (int i=0; i<octaves; i++)
		{
			float xVal = x * frequency * perlinScale; //the xVal to pass in to the GetPerlinNoise method
			float zVal = z * frequency * perlinScale;

			//add the current octave's perlin noise to the fractal noise
			fractalNoise += amplitude * GetPerlinNoise (xVal, zVal);

			//set the frequency and amplitude of the next loop
			//lacunarity determines how quickly the frequency changes
			//gain determines how quickly the amplitude changes over each octave
			frequency *= lacunarity;
			amplitude *= gain;
		}

		return fractalNoise;
	}
}
