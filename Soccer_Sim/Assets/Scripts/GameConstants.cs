using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConstants : MonoBehaviour {

	public static int BlueScore = 0;
    	public static int RedScore = 0;
	public static Vector3[] centers;

	public int numRows;
	public int numColums;

	private GameObject field;


	public void Start()
	{
		field = GameObject.Find ("Field");
		centers = new Vector3[numRows * numColums];

		computeGrid ();
	}

	public void Update()
	{
		drawGrid ();
	}


	public void computeGrid()
	{
		float fieldWidth = field.GetComponent<Renderer> ().bounds.size.x;
		float fieldHeight = field.GetComponent<Renderer> ().bounds.size.z;
		Vector3 min = field.GetComponent<Renderer> ().bounds.min;

		float realTileWidth = fieldWidth / numColums;
		float realTileHeight = fieldHeight / numRows;

		for (int i = 0; i < numRows; ++i)
		{
			for (int j = 0; j < numColums; ++j)
			{

				centers[i*numColums + j] = (min + new Vector3(realTileWidth, 0.0f, realTileHeight) + new Vector3(realTileWidth*j, 0.0f, realTileHeight*i)) - (new Vector3(realTileWidth/numColums, 0.0f, realTileHeight/numRows));


			}
		}

	}

	public void drawGrid()
	{
		for(int i = 0; i < centers.Length-1; ++i){

			if (i%numColums != numColums-1)
			{
				Debug.DrawLine (centers[i], centers[i+1], Color.black);	
			}

			if (i + numColums < centers.Length)
			{
				Debug.DrawLine (centers[i], centers[i+numColums], Color.black);
			}

		}
	}

}
