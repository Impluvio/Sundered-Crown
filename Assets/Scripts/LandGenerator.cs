using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class LandGenerator : MonoBehaviour
{
    // Sets Map Canvas 
    [Tooltip("ensure this is a round number above 20")] [SerializeField] int mapSizeX;
    [Tooltip("ensure this is a round number above 20")] [SerializeField] int mapSizeY;
    // Sets Walker Size to generate landmasses
    [Tooltip("this is approx no. of large continents")] [SerializeField] int numberOfLargeWalkers;
    [Tooltip("this is approx no. of small continents")] [SerializeField] int numberOfSmallWalkers;
    // Removes noise from Map
    [Tooltip("this will 'smooth' the map the higher it is set")] [SerializeField] int cellAutIterations;
    // Sets the middle row of the map    
    int equator;

    // Sets number of mountain ranges
    [SerializeField] int numberOfMountainRanges;
    
    // Manages the sprites for the project.
    public Tilemap tileMap;
    public TileBase waterTile; // NOTE: look at pulling tile type from the tilePallette which you populate in the editor. EXAMPLE:    TileBase tile = tilePalette.GetTile(0);
    public TileBase landTile;  // maybe these should be stored on the tile itself. 
    public TileBase MountainTile;
    public TileBase SnowTile;
    public TileBase SnowMountainTile;

    // Manages the array data structure for the 'Game Tile classes' 

    public GameTile[,] gameTiles;
    GameTile[,] mapGrid;
    GameTile[,] newStateGrid;
    List<GameTile> landTiles = new List<GameTile>();
    List<GameTile> mountainTiles = new List<GameTile>();
    private List<HashSet<GameTile>> mountainClusters = new List<HashSet<GameTile>>(); //contains mountain ranges as hashsets.

 

    private Dictionary<Vector3Int, int> depths = new Dictionary<Vector3Int, int>();

    public WeatherManager weatherManager;



    void Start()
    {
        //Vector3Int positionTest = new Vector3Int(1, 1, 0);
        //tileMap.SetTile(positionTest, newTilebase);

        SetGridData();
        SetTileNeighbours();
        GenerateLandmasses();
        testRun();
    }

  
    private void SetGridData() // Sets The grid, initialises array of gametiles, Names them, sets their position in the Tile Map
    {
        equator = mapSizeY / 2; // sets the equator as an int which represents the middle row of the map.
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

                int distanceToMid = Mathf.Abs(j - equator);
                Debug.Log("dist to mid: " + distanceToMid);
                newTileInstance.SetEquatorDistance(distanceToMid); 

                Vector3Int currentPosition = new Vector3Int(i, j, 0);
                tileMap.SetTile(currentPosition, waterTile);
                // we want to flip all of the tiles to water, then iterate over them creating classes and setting their coordinates. 
            }


        }

    }                     

    private void SetTileNeighbours()  // Calculates the neighbours of each Game Tile in the array, feeds these to the Game Tile
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
                currentTile.SetNeighbours(neighbours);


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

    private void GenerateLandmasses() // Manages the generation of continents, Calls on RandomWalker,  Cellular Automata, Create Mountains and Set Climate Zones 
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

        //CreateMountainRanges(numberOfMountainRanges);

        SetClimateZones();

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
                currentTile.SetLandOrSea(true);
                landTiles.Add(currentTile);
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


    } // This is utilised by different methods to create random land masses. 

    private GameTile MoveToNeighbour(GameTile currentTile, int Direction)
    {
        GameTile tileToMoveFrom = currentTile;

        if (tileToMoveFrom != null && tileToMoveFrom.tileNeighbours != null && Direction >= 0 && Direction < tileToMoveFrom.tileNeighbours.Length)
        {
            GameTile nextTile = tileToMoveFrom.tileNeighbours[Direction]; //0 to 3 is up | right | down | left.

            if (nextTile != null)
            {

                return (nextTile);

            }

        }

        return (tileToMoveFrom);
    } // utilised by RandomWalker / Create Mountains 

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
                int landCount = CountNoLandNeighbours(currentTile);

                if(landCount >= 3)
                {
                    
                    GameTile newStateTileToChange = newStateGrid[currentTile.tilePosition.x, currentTile.tilePosition.y];
                    newStateTileToChange.SetLandOrSea(true);
                   
                }
                if(landCount < 2)
                {
                    GameTile newStateTileToChange = newStateGrid[currentTile.tilePosition.x, currentTile.tilePosition.y];
                    newStateTileToChange.SetLandOrSea(false);
                }

            }
            mapGrid = newStateGrid;

        }
    } //iterates over land (only land) and smooths its edges.

    private int CountNoLandNeighbours(GameTile currentTile)
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
    } // records the number of land tiles for the game of life / cellular automata algorithm

    private void CreateMountainRanges(int numberOfRanges) 
    {
        int lowerRangeLength = mapSizeX + mapSizeY / 10;
        int upperRangeLength = mapSizeX + mapSizeY / 2;

        int upperSpineLength = Mathf.FloorToInt(mapSizeX / 3);
        int lowerSpineLength = Mathf.FloorToInt(mapSizeX / 9);

        for (int i = 0; i < numberOfMountainRanges; i++)
        {
            int landListLength = landTiles.Count;
            int randomTileEntry = UnityEngine.Random.Range(0, landListLength);
            GameTile selectedTile = landTiles[randomTileEntry];                                 // this is list of land tiles.
           // int randomLength = UnityEngine.Random.Range(lowerRangeLength, upperRangeLength);
            int spineLength = UnityEngine.Random.Range(lowerSpineLength, upperSpineLength);
           // int currentSteps = 0; 
            HashSet<GameTile> mountainCluster = setSpine(selectedTile, spineLength);
            
            
            
            
            //set the start point to a land tile and get it only to move to land tiles

            //while (currentSteps < randomLength)
            //{

            //    if (selectedTile != null)
            //    {

            //        selectedTile.SetMountains(true);
            //        mountainTiles.Add(selectedTile); 

            //        int randomDirectionOne = UnityEngine.Random.Range(0, 4); //0 to 3 is up | right | down | left.
            //        int randomDirectionTwo = UnityEngine.Random.Range(0, 4);
            //        int randomDirection = randomDirectionOne;

            //        GameTile tileToCheck = MoveToNeighbour(selectedTile, randomDirection);

            //        if (tileToCheck.isLand)
            //        {
            //            selectedTile = tileToCheck;
            //            currentSteps++;
            //        }
            //        else
            //        {
            //            randomDirection = randomDirectionTwo;
            //        }

                    

            //    }


            //} 

        }


    }

    private HashSet<GameTile> setSpine(GameTile selectedTile, int spineLength)
    {
        bool axialBiasX = UnityEngine.Random.value > 0.5f; //this is a condition
        int currentStep = 0;
        HashSet<GameTile> spine = new HashSet<GameTile>(); // would be good to auto name these mountain ranges at some point. 
        //add first tile here

        GameTile tileToProcess = selectedTile;
        int randomDirectionOne = 0;
        int randomDirectionTwo = 0;
        int directionToCheck;

        while (currentStep < spineLength)
        {
            if (tileToProcess != null)
            {
                tileToProcess.SetMountainSpine(true);        

                if (axialBiasX == true)
                {
                    float[] biasedWeightsX = { 2f, 1f, 2f, 1f };                 // Biases the Up & Down movements.

                    float totalWeight = 0;
                    foreach (float weight in biasedWeightsX)
                    {
                        totalWeight += weight;
                    }

                    float randomValue = UnityEngine.Random.value * totalWeight; // Sets the total weight as a number BETWEEN 0.0 & 1.0 * the added floats in the biased array 

                    float cumulativeWeight = 0;                                 // Cumulative weight will gradually add the floats in the array together
                    for (int i = 0; i < biasedWeightsX.Length; i++)
                    {
                        cumulativeWeight += biasedWeightsX[i];                  // Add the weights, with each round this goes higher so the chance of being under increases
                        if (randomValue < cumulativeWeight)
                        {
                            randomDirectionOne = i;
                            randomDirectionTwo = UnityEngine.Random.Range(0, 4);
                        }
                    }


                } // stupidly big if statement, would poss be better as separate method. 
                else // bias Y
                {
                    float[] biasedWeightsY = { 1f, 2f, 1f, 2f };

                    float totalWeight = 0;
                    foreach (float weight in biasedWeightsY)
                    {
                        totalWeight += weight;
                    }

                    float randomValue = UnityEngine.Random.value * totalWeight; // Sets the total weight as a number BETWEEN 0.0 & 1.0 * the added floats in the biased array 

                    float cumulativeWeight = 0;                                 // Cumulative weight will gradually add the floats in the array together
                    for (int i = 0; i < biasedWeightsY.Length; i++)
                    {
                        cumulativeWeight += biasedWeightsY[i];                  // Add the weights, with each round this goes higher so the chance of being under increases
                        if (randomValue < cumulativeWeight)
                        {
                            randomDirectionOne = i;
                            randomDirectionTwo = UnityEngine.Random.Range(0, 4);

                        }

                    }

                    
                }
                
                directionToCheck = randomDirectionOne;

                GameTile tileToCheck = MoveToNeighbour(tileToProcess, directionToCheck);

                if (tileToCheck.isLand)
                {
                    tileToProcess = tileToCheck;
                    currentStep++;
                    spine.Add(tileToProcess);
                }
                else
                {
                    directionToCheck = randomDirectionTwo;
                }

            }
        }

        return spine;

    }

    private void SetClimateZones() 
    {
        // what we will do here is, read the distance from the equator variable in each tile using a foreach. 
        // make the top 10% for example change the snow likelihood. 
        // we will set a parameter that indicates how far from the equator is ice/snow. 
        // then we will create a buffer zone - the further into that buffer zone the higher the likelihood of snow
        // lastly we will use a cellular automata algorithm to ensure any lone grass squares are converted.

        int calcPolarCapSize(int mapSizeY) =>           //set how deep the ice should be using terniary condition
            mapSizeY < 50 ? 6 :                         //based upon mapsize - take this from the .abs max distance from equator
            mapSizeY < 100 ? 10 :                       //maybe change this to a slider.
            mapSizeY < 200 ? 15 : 20;

        int polarCapSize = calcPolarCapSize(mapSizeY);       //passes this to a variable
        int transitionalZone = (int)Math.Ceiling(polarCapSize / 3.0); //this sets the buffer that can still contain ice
        int maxDistanceToPoles = Mathf.Abs(mapSizeY - equator);  // Sets the distance from equator to pole as an absolute number. 
        int polarZoneStart = maxDistanceToPoles - polarCapSize;  // notes the beginning of the polar zone
        int bufferZoneStart = polarZoneStart - transitionalZone; // notes the beginning area where snow MAY be present

        List<int> bufferZoneRows = getBufferZoneRows(polarZoneStart, bufferZoneStart);
        int rowListLength = bufferZoneRows.Count;
        float percentageDistribution = (100 / rowListLength);

        foreach (GameTile gameTile in mapGrid)
        {
            GameTile currentTile = gameTile;
            int currentTileToMid = currentTile.distanceFromEquator;

            if (currentTileToMid >= polarZoneStart && currentTileToMid <= maxDistanceToPoles + 1) //small strip still green owing to zero index.
            {
                currentTile.SetSnow(true);
            }
            else if (currentTileToMid > bufferZoneStart && currentTileToMid < polarZoneStart)
            {
                int Index = bufferZoneRows.IndexOf(currentTileToMid);
                int probabilityOfSnow = Mathf.FloorToInt(Index * percentageDistribution);
                int randomValue = UnityEngine.Random.Range(1, 101);

                // Check if the random value falls within the probability range
                if (randomValue <= probabilityOfSnow)
                {
                    currentTile.SetSnow(true); // Set tile as snow
                }
            }


        }

    }

    private List<int> getBufferZoneRows(int startOfPolarZone, int startOfBufferZone)
    {
        // int bufferZoneDepth = startOfPolarZone - startOfBufferZone;
        List<int> polarRows = new List<int>();

        for (int row = startOfBufferZone; row < startOfPolarZone; row++)
        {
            polarRows.Add(row);
        }

        return polarRows;

        

    }


    private void updateMap() // this actions all the changes iterated over the tiles
    {
        // iterate over the 2d array, and return the vector3 coordinate & land bool 
        // we will change this later, so that we access tilemap pallet from the gameTile object, and return this, but for now just the bool.
        // this can be much simpler - it could for each over the tiles in the array and update the grid based upon their gameTiles references.
        foreach (GameTile gameTile in mapGrid)

        {
            GameTile currentTile = gameTile;

            if (currentTile.isLand)
            {
                tileMap.SetTile(currentTile.tilePosition, landTile);

            }
            if (currentTile.isMountain)
            {
                tileMap.SetTile(currentTile.tilePosition, MountainTile);
            }
           if(currentTile.isLand && currentTile.isSnowy)
            {
                tileMap.SetTile(currentTile.tilePosition, SnowTile);
            }
            if (currentTile.isMountain && currentTile.isSnowy)
            {
                tileMap.SetTile(currentTile.tilePosition, SnowMountainTile);
            }


        }

    }

    private void testRun() // use this to run more advanced checks/debuging.
    {
        foreach (GameTile gameTile in mapGrid)
        {
            GameTile currentTile = gameTile;

            int distanceToMid = currentTile.distanceFromEquator;
            string tileName = currentTile.name;

            Debug.Log("Tile name: " + tileName + " Distance to equator: " + distanceToMid);

        }

        weatherManager.GenerateClouds(mapSizeX, mapSizeY);

    }



    // Update is called once per frame
    void Update()
    {

    }
}
