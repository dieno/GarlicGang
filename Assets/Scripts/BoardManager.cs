using UnityEngine;
using System;
using System.Collections.Generic; 		//Allows us to use Lists.
using Random = UnityEngine.Random; 		//Tells Random to use the Unity Engine random number generator.
	
public class BoardManager : MonoBehaviour
{
	// Using Serializable allows us to embed a class with sub properties in the inspector.
	[Serializable]
	public class Count
	{
		public int minimum; 			//Minimum value for our Count class.
		public int maximum; 			//Maximum value for our Count class.
			
			
		//Assignment constructor.
		public Count (int min, int max)
		{
			minimum = min;
			maximum = max;
		}
	}
		
	public int columns = 8; 										//Number of columns in our game board.
	public int rows = 8;											//Number of rows in our game board.
	public Count wallCount = new Count (5, 9);                      //Lower and upper limit for our random number of walls per level.
    public Count foodCount = new Count (1, 5);						//Lower and upper limit for our random number of food items per level.
	public GameObject exit;											//Prefab to spawn for exit.
	public GameObject[] floorTiles;									//Array of floor prefabs.
	public GameObject[] wallTiles;									//Array of wall prefabs.
	public GameObject[] foodTiles;									//Array of food prefabs.
	public GameObject[] enemyTiles;									//Array of enemy prefabs.
	public GameObject[] outerWallTiles;								//Array of outer tile prefabs.

    public GameObject floorTile;
    public GameObject topLeftWallTile;
    public GameObject topRightWallTile;
    public GameObject bottomLeftWallTile;
    public GameObject bottomRightWallTile;
    public GameObject leftWallTile;
    public GameObject rightWallTile;
    public GameObject topWallTile;
    public GameObject bottomWallTile;

    public float tileScaleX = 4.0f;
    public float tileScaleY = 4.0f;

    private Transform boardHolder;									//A variable to store a reference to the transform of our Board object.
	private List <Vector3> gridPositions = new List <Vector3> ();   //A list of possible locations to place tiles.

    void InitializedList()
    {
        gridPositions.Clear();
        for (int x = 1; x < columns - 1; x++)
        {
            for (int y = 1; y < rows - 1; y++)
            {
                gridPositions.Add(new Vector3(x, y, 0f));
            }
        }
    }

    void BoardSetup()
    {
        boardHolder = new GameObject("Board").transform;
        
        boardHolder.parent = transform;

        for (int x = -1; x < rows + 1; x++)
        {
            for (int y = -1; y < columns + 1; y++)
            {
                GameObject toInstantiate = floorTile;
                if (x == -1)
                {
                    if(y == -1) // bottom left
                    {
                        toInstantiate = bottomLeftWallTile;
                    }
                    else if(y == columns) // top left
                    {
                        toInstantiate = topLeftWallTile;
                    }
                    else // left
                    {
                        toInstantiate = leftWallTile;
                    }
                }
                else if(x == rows)
                {
                    if (y == -1) // bottom right
                    {
                        toInstantiate = bottomRightWallTile;
                    }
                    else if (y == columns) // top right
                    {
                        toInstantiate = topRightWallTile;
                    }
                    else // right
                    {
                        toInstantiate = rightWallTile;
                    }
                }
                else
                {
                    if(y == -1) // bottom
                    {
                        toInstantiate = bottomWallTile;
                    }
                    else if(y == columns) //top
                    {
                        toInstantiate = topWallTile;
                    }
                }

                GameObject instance = Instantiate(toInstantiate, new Vector3(x * tileScaleX, y * tileScaleY, 0f), Quaternion.identity) as GameObject;
                instance.transform.SetParent(boardHolder);
            }
        }

        boardHolder.transform.Translate(-(rows * tileScaleX) / 2.0f, -1.0f * ((columns * tileScaleY) / 2.5f), 0f);
    }

    Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector3 randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex);
        return randomPosition;
    }

    void Start()
    {

        BoardSetup();
    }

}

