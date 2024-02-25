using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LandGenerator : MonoBehaviour
{
    [Tooltip("ensure this is a round number above 20")] [SerializeField] int mapSizeX;
    [Tooltip("ensure this is a round number above 20")] [SerializeField] int mapSizeY;

    [Tooltip("this is approx no. of large continents")] [SerializeField] int numberOfLargeWalkers;
    [Tooltip("this is approx no. of small continents")] [SerializeField] int numberOfSmallWalkers;
    [Tooltip("this will 'smooth' the map the higher it is set")] [SerializeField] int cellAutIterations;

    int numberOfMountainRanges; 

    public Tilemap tileMap;
    public TileBase waterTile; // NOTE: look at pulling tile type from the tilePallette which you populate in the editor. EXAMPLE:    TileBase tile = tilePalette.GetTile(0);
    public TileBase landTile;
    public TileBase MountainTile; 


    public GameTile[,] gameTiles;
    GameTile[,] mapGrid;
    GameTile[,] newStateGrid;

    void Start()
    {
        //Vector3Int positionTest = new Vector3Int(1, 1, 0);
        //tileMap.SetTile(positionTest, newTilebase);

        SetGridData();
        SetTileNeighbours();
        GenerateLandmasses();

    }

   

    private void SetGridData()
    {
        mapGrid = new GameTile[mapSizeX, mapSizeY]; // initialises the array of game tiles.

        for (int i = 0; i < mapSizeX; i++)
        {
            for (int j = 0; j < mapSizeY; j++)
            {
                GameTile newTileInstance = new GameTile(i, j);

                string xCoord = i.ToString();
                string yCoord = j.ToString();

                string tileName = $" [{i},{j}]";

                newTileInstance.SetName(tileName);
                mapGrid[i, j] = newTileInstance; // populates array.

                //Debug.Log(mapGrid.Length);
                //Debug.Log($"game tile at [{i},{j}] is set to {mapGrid[i, j].currentSprite}");

                Vector3Int currentPosition = new Vector3Int(i, j, 0);
                tileMap.SetTile(currentPosition, waterTile);
                // we want to flip all of the tiles to water, then iterate over them creating classes and setting their coordinates. 
            }


        }

    }

    private void SetTileNeighbours()
    {
        

        int row = mapGrid.GetLength(0);
        int col = mapGrid.GetLength(1);

        for (int i = 0; i < row; i++) // i is row or Y axis going up or down 
        {
            for (int j = 0; j < col; j++) //j is column or X axis going left or right
            {
                GameTile[] neighbours = new GameTile[4];
                //Debug.Log("position" + i + "," + j);

                if (i + 1 >= 0 && i + 1 < row && j >= 0 && j < col) //iterates to next row up
                {
                    neighbours[0] = mapGrid[i + 1, j];
                }
                else { }
                if (i >= 0 && i < row && j + 1 >= 0 && j + 1 < col) //iterates to next column right 
                {
                    neighbours[1] = mapGrid[i, j + 1];
                }
                else { }
                if (i - 1 >= 0 && i - 1 < row && j >= 0 && j < col) //iterates to next row down
                {
                    neighbours[2] = mapGrid[i - 1, j];
                }
                else { }
                if (i >= 0 && i < row && j - 1 >= 0 && j - 1 < col) //iterates to next column left 
                {
                    neighbours[3] = mapGrid[i, j - 1];
                }
                else { }


                //Debug.Log("Position" + i + "," + j + " has neighbours(Up, right, down, left)" + neighbours[0].name + neighbours[1].name);


                GameTile currentTile = mapGrid[i, j];
                currentTile.setNeighbours(neighbours);


                //foreach (GameTile gameTile in neighbours)
                //{
                //    if (gameTile != null)
                //    {
                //        Debug.Log("neighbours" + gameTile.tilePosition);
                //    }
                //    else { };

                //}


            }


        }


    }

    private void GenerateLandmasses()
    {
        int largeWalker = mapSizeX * mapSizeY;
        int smallWalker = UnityEngine.Random.Range(mapSizeX * mapSizeY / 30, mapSizeY * mapSizeX / 20);

        for (int i = 0; i < numberOfLargeWalkers; i++)
        {
            RandomWalker(largeWalker);
        }

        for (int i = 0; i < numberOfSmallWalkers; i++)
        {
            RandomWalker(smallWalker);
        }

        RunCellularAutomata(cellAutIterations); //great way to tell if this is working is the map will look more gritty/granular in the land masses if not actioned.

        CreateMountains(numberOfMountainRanges); 


        // fault line progression algorithm to set the mountain ranges
        // maybe for this, set a straighter walker and along the line, set a number of branches so that the range has more character. 
        // add a height aspect to the gameTile for this. 

        // Algorithm to set north south cold areas and cold 

        // temporate and arboreal forest seeding / growth 

        // Desertification

        // move to kingdom setup method 


        updateMap();
    }

    private void RandomWalker(int maxSteps)
    {
        int randomCoordsX = UnityEngine.Random.Range(0, mapSizeX);
        int randomCoordsY = UnityEngine.Random.Range(0, mapSizeY);

        GameTile startTile = mapGrid[randomCoordsX, randomCoordsY];


        int currentSteps = 0;

        while (currentSteps < maxSteps)
        {
            GameTile currentTile = startTile;



            if (currentTile != null)
            {
                currentTile.setLandOrSea(true);
                int randomDirection = UnityEngine.Random.Range(0, 4);
                startTile = MoveToNeighbour(currentTile, randomDirection);
                currentSteps++;

            }
            else
            {
                Debug.LogError("currentTile is null. Aborting GenerateLandMasses.");
                break; // Exit the loop to prevent an infinite loop
            }


        }



        //while loop while current steps are less than numberofsteps choose random next tile from array and switch to grass/land


    }

    private GameTile MoveToNeighbour(GameTile currentTile, int Direction)
    {
        GameTile tileToMoveFrom = currentTile;


        if (tileToMoveFrom != null && tileToMoveFrom.tileNeighbours != null && Direction >= 0 && Direction < tileToMoveFrom.tileNeighbours.Length)
        {
            GameTile nextTile = tileToMoveFrom.tileNeighbours[Direction];

            if (nextTile != null)
            {

                return (nextTile);

            }

        }

        return (tileToMoveFrom);
    }

    private void RunCellularAutomata(int cellAutIterations)
    {
        //GameTile[,] newStateGrid;

        newStateGrid = new GameTile[mapSizeX, mapSizeY]; // this part feeds

        for (int j = 0; j < mapSizeX; j++)
        {
            for (int k = 0; k < mapSizeY; k++)
            {
                newStateGrid[j, k] = mapGrid[j, k]; 
            }
        }

        for (int i = 0; i < cellAutIterations; i++)
        {
            foreach(GameTile gameTile in mapGrid)
            {
                GameTile currentTile = gameTile;
                int landCount = countNoLandNeighbours(currentTile);

                if(landCount >= 3)
                {
                    Debug.Log("land is flipping");
                    GameTile newStateTileToChange = newStateGrid[currentTile.tilePosition.x, currentTile.tilePosition.y];
                    newStateTileToChange.setLandOrSea(true);
                    Debug.Log("tile number" + newStateTileToChange.tilePosition + "has flipped to land");
                }
                if(landCount < 2)
                {
                    GameTile newStateTileToChange = newStateGrid[currentTile.tilePosition.x, currentTile.tilePosition.y];
                    newStateTileToChange.setLandOrSea(false);
                }

            }
            Debug.Log("flips state");
            mapGrid = newStateGrid;

        }
    }

    private int countNoLandNeighbours(GameTile currentTile)
    {
        int landCount = 0;

        foreach (GameTile neighbour in currentTile.tileNeighbours)
        {
            if (neighbour != null)
            {
                GameTile neighbourTile = neighbour;

                if(neighbourTile != null && neighbourTile.isLand)
                {
                    ++landCount;
                }

            }
        }

        return (landCount);
    }

    private void CreateMountains(int numberOfRanges)
    {
        int randomCoordsX = UnityEngine.Random.Range(0, mapSizeX);
        int randomCoordsY = UnityEngine.Random.Range(0, mapSizeY);

        int lowerRangeLength = mapSizeX + mapSizeY / 10;
        int upperRangeLength = mapSizeX + mapSizeY / 2;

        for (int i = 0; i < numberOfMountainRanges; i++)
        {
            GameTile startTile = mapGrid[randomCoordsX, randomCoordsY];

            int randomLength = UnityEngine.Random.Range(lowerRangeLength, upperRangeLength);
            int currentSteps = 0;

            while (currentSteps < randomLength)
            {


                ++currentSteps;
            }

        }









        // create random start point - will need x/y for this. 

        // set random walker in a direction - we could do a random 


    }

    private void updateMap()
    {
        //iterate over the 2d array, and return the vector3 coordinate & land bool // we will change this later, so that we access tilemap pallet from the gameTile object, and return this, but for now just the bool.

        foreach (GameTile gameTile in mapGrid)

        {
            GameTile currentTile = gameTile;

            if (currentTile.isLand)
            {
                tileMap.SetTile(currentTile.tilePosition, landTile);

            }

        }

    }


    // Update is called once per frame
    void Update()
    {

    }
}
