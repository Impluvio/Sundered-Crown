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
    int bufferZoneStart; 

    // Sets number of mountain ranges
    [SerializeField] int numberOfMountainRanges;
    // sets number of forests
    [Tooltip("sets number of forests")] [SerializeField] int numberOfForests;
    // Sets number of desert regions
    [Range(1, 10)] public int desertRegions;

    // Manages the sprites for the project.
    public Tilemap tileMap;
    public TileBase waterTile; // NOTE: look at pulling tile type from the tilePallette which you populate in the editor. EXAMPLE:    TileBase tile = tilePalette.GetTile(0);
    public TileBase landTile;  // maybe these should be stored on the tile itself. 
    public TileBase mountainTile;
    public TileBase snowTile;
    public TileBase snowMountainTile;
    public TileBase forestTile;
    public TileBase forestTileSnow;

    // Manages the array data structure for the 'Game Tile classes' 

    public GameTile[,] gameTiles;
    GameTile[,] mapGrid;
    GameTile[,] newStateGrid;
    List<GameTile> landTiles = new List<GameTile>();
    List<GameTile> mountainTiles = new List<GameTile>();
    private List<HashSet<GameTile>> mountainClusters = new List<HashSet<GameTile>>(); //contains mountain ranges as hashsets.
    public WeatherManager weatherManager;

    private 


    void Start()
    {
        //Vector3Int positionTest = new Vector3Int(1, 1, 0);
        //tileMap.SetTile(positionTest, newTilebase);

        SetGridData();
        SetTileNeighbours();
        GenerateLandmasses();
        //testRun();
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

        Debug.Log("Grid Data Set");
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


                GameTile currentTile = mapGrid[i, j];
                currentTile.SetNeighbours(neighbours);

            }

        }
        Debug.Log("Tile Neighbours Set");

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

        CreateMountainRanges(numberOfMountainRanges);

        SetClimateZones();

        CreateForests(numberOfForests);

        //Desertification(desertRegions);

        // move to kingdom setup method 

        updateMap();
        //testRun();
    }

    private void Desertification(int desertRegions) // move this method to above the mountains but before forests.
    {
        int desertZonelimit = UnityEngine.Mathf.FloorToInt(equator / 3);

        int desertZoneUpper = equator + desertZonelimit;
        int desertZoneLower = equator - desertZonelimit; 

        int randomCoordsX = UnityEngine.Random.Range(0, mapSizeX); 
        int randomCoordsY = UnityEngine.Random.Range(desertZoneLower, desertZoneUpper);

        int desertSizeLow = UnityEngine.Mathf.FloorToInt(mapSizeX / 6);
        int desertSizeHigh = UnityEngine.Mathf.FloorToInt(mapSizeX / 2);

        for(int i = 0; i < desertRegions; i++)
        {
            GameTile randomTile = mapGrid[randomCoordsX, randomCoordsY];
            int desertSize = UnityEngine.Random.Range(desertSizeLow, desertSizeHigh);
            int stepCount = 0;
            while (stepCount < desertSize)
            {
                GameTile tileToProcess = randomTile;
                if (tileToProcess.isLand)
                {

                }
                else
                {

                }
            }

        }

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
    } // utilised by RandomWalker / Create Mountains / everything 

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
    }                                  //iterates over land (only land) and smooths its edges.

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
    }                                  // records the number of land tiles for the game of life / cellular automata algorithm

    private void CreateMountainRanges(int numberOfRanges) 
    {

        int upperSpineLength = Mathf.FloorToInt(mapSizeX * 3);                                  // ensure this remains an Int 
        int lowerSpineLength = Mathf.FloorToInt(mapSizeX / 9);                                  // ensures this remains an Int 

        for (int i = 0; i < numberOfMountainRanges; i++)                                        // iterates over mountain range number.
        {
            int landListLength = landTiles.Count;                                               // Accesses all land tiles
            int randomTileEntry = UnityEngine.Random.Range(0, landListLength);                  // Selects random tile from all land tiles
            GameTile selectedTile = landTiles[randomTileEntry];                                 // Selects and assigns random tile entry.
                                                                                                // int randomLength = UnityEngine.Random.Range(lowerRangeLength, upperRangeLength);
            int spineLength = UnityEngine.Random.Range(lowerSpineLength, upperSpineLength);     // Sets the length of the Mountain range spine. 
            //Debug.Log("spine length: " + spineLength);
            //Debug.Log("selected tile is: " + selectedTile.name);
            int numberOfSpurs = Mathf.FloorToInt(spineLength / 10); 
            HashSet<GameTile> mountainCluster = setSpine(selectedTile, spineLength, numberOfSpurs);   //passes this to a set spine method which outputs hash for that mountain range

            //if statement for current axial bias - if true it biases x if false biases y

            // Next we need to decide how many spurs that we are going to have per spine 
            // We need to generate those spurs off the spine. This should probably be a percentage of the overall length.
            // before or after the spurs we should add a buffer to thicken out the mountains. 
            // add these to a hash set and use union with to join them.
            // iterate over the tiles, set tiles in hashset to mountains.

        }


    }

    private void CreateForests(int numberOfForests)
    {
        int largeForests = numberOfForests * 2;
        int smallForests = numberOfForests * 10;

        int forestSizeLargeUpper = mapSizeX * 4;
        int forestSizeLargeLower = mapSizeX * 2;
        int forestSizeSmallUpper = mapSizeX / 2;
        int forestSizeSmallLower = mapSizeX / 5;

        //mechanism to ensure forests spawn outside of polar zone.

        int temperateBounds = Mathf.FloorToInt(mapSizeX / 100 * 70);
        int boundsHalved = Mathf.FloorToInt(temperateBounds / 2);
        int mapMidline = equator;
        int arborealRangeNorth = equator + boundsHalved;
        int arborealRangeSouth = equator - boundsHalved;
        int randomRowInRange;
        int randomcolumn;
        // set the equator from global variable
        // add a set amount from that equator as a percentage of the overall number of rows
        // randomise the above,feed into statement [random-clamped, random] to grab tiles in specific region

        
      

        for (int i = 0; i < largeForests; i++)
        {
            randomRowInRange = UnityEngine.Random.Range(arborealRangeNorth, arborealRangeSouth);
            randomcolumn = UnityEngine.Random.Range(0, mapSizeY);
            GameTile tileToProcess = mapGrid[randomRowInRange, randomcolumn];
            
            GameTile currentTile = tileToProcess;
            int forestSize = 0;
            int targetForestSize = UnityEngine.Random.Range(forestSizeLargeLower, forestSizeLargeUpper);



            while (forestSize < targetForestSize)
            {
                if (currentTile.isLand && !currentTile.isMountain)
                {
                    currentTile.SetForest(true);
                    forestSize++;
                    int randomDirection = UnityEngine.Random.Range(0, 4);
                    currentTile = MoveToNeighbour(currentTile, randomDirection);
                }
                else
                {
                    int randomDirection = UnityEngine.Random.Range(0, 4);
                    currentTile = MoveToNeighbour(currentTile, randomDirection);
                }

            }

            i++;
        }

        for (int i = 0; i < smallForests; i++)
        {
            randomRowInRange = UnityEngine.Random.Range(arborealRangeNorth, arborealRangeSouth);
            randomcolumn = UnityEngine.Random.Range(0, mapSizeY);
            GameTile tileToProcess = mapGrid[randomRowInRange, randomcolumn];
            GameTile currentTile = tileToProcess;
            int forestSize = 0;
            int targetForestSize = UnityEngine.Random.Range(forestSizeSmallLower, forestSizeSmallUpper);

            while (forestSize < targetForestSize)
            {
                if (currentTile.isLand && !currentTile.isMountain)
                {
                    currentTile.SetForest(true);
                    forestSize++;
                    int randomDirection = UnityEngine.Random.Range(0, 4);
                    currentTile = MoveToNeighbour(currentTile, randomDirection);
                }
                else
                {
                    int randomDirection = UnityEngine.Random.Range(0, 4);
                    currentTile = MoveToNeighbour(currentTile, randomDirection);
                }

            }

            i++;
        }


    }

    private HashSet<GameTile> setSpine(GameTile selectedTile, int spineLength, int numberOfSpurs)
    {                                                
        int currentStep = 0;                                                                // resets step value
        HashSet<GameTile> spine = new HashSet<GameTile>();                                  // $$$ would be good to auto name these mountain ranges at some point $$$
                                                                                            
        int directionToCheck;                                                               // sets the direction to check based upon axial bias (see below)  
        int spurCounter = numberOfSpurs;
        GameTile tileToProcess = selectedTile;

        while (currentStep < spineLength)                                                   // iterates through as long as the current step in process is less than spine length
        {
            int upperThreshold = Mathf.FloorToInt(spineLength / numberOfSpurs);
            int lowerthreshould = 5;
            int chanceOfSpur = UnityEngine.Random.Range(0, 10);
            int spurLength = UnityEngine.Random.Range(lowerthreshould, upperThreshold);

            if (tileToProcess != null)                                                      // Null check 
            {
                tileToProcess.SetMountainSpine(true);                                       // Setts instance of GameTile to mountain spine (i.e. big mountain) 
                // Debug.Log("spine length:  " + spineLength);
            }
                                                                             
            directionToCheck = UnityEngine.Random.Range(0, 4);
            GameTile tileToCheck = MoveToNeighbour(tileToProcess, directionToCheck);    // we check the neighbours using ol'reliable which feeds the current tile
                //Debug.Log("Main tile to check after move: " + tileToCheck.name);

             if (tileToCheck.isLand && tileToCheck != null)                                                     // if the tile is land then add it to the spine.
             {
                    tileToProcess = tileToCheck;                                            // spine can write over itself (check this).
                    currentStep++;
                    spine.Add(tileToProcess);
             }
             else
             {
                tileToProcess = MoveToNeighbour(tileToProcess, UnityEngine.Random.Range(0, 4));
             }

            if (chanceOfSpur > 7 && spurCounter > 0)
            {
                // Debug.Log("spur created");

                GameTile spurTile = tileToProcess;
                for (int i = 0; i < spurLength; i++)
                {
                    int randomSpurDirection = UnityEngine.Random.Range(0, 4);
                    GameTile spurTileToCheck = MoveToNeighbour(spurTile, randomSpurDirection);

                    if (spurTileToCheck != null && spurTileToCheck.isLand)
                    {
                        spurTile = spurTileToCheck;
                        spurTile.SetMountains(true);
                        spine.Add(spurTileToCheck);
                    }
                    else
                    {
                        randomSpurDirection = UnityEngine.Random.Range(0, 4);
                    }


                }

                // needs to take in current tile run a process on the tile and return to

                spurCounter--;
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
            GameTile currentTile = gameTile;    // use switch statement here with 'when' 

            //switch (currentTile)
            //{
            //    case { isLand: true, isSnowy: false, isForest: false }:
            //        tileMap.SetTile(currentTile.tilePosition, landTile);
            //        break;

            //    case { isMountain: true, isSnowy: false }:
            //        tileMap.SetTile(currentTile.tilePosition, mountainTile);
            //        break;

            //    case { isLand: true, isSnowy: true }:
            //        tileMap.SetTile(currentTile.tilePosition, snowTile);
            //        break;

            //    case { isMountain: true, isSnowy: true }:
            //        tileMap.SetTile(currentTile.tilePosition, snowMountainTile);
            //        break;

            //    case { isLand: true, isForest: true }:
            //        tileMap.SetTile(currentTile.tilePosition, forestTile);
            //        break;
            //}


            if (currentTile.isLand)
            {
                tileMap.SetTile(currentTile.tilePosition, landTile);
            }
            if (currentTile.isMountain)
            {
                tileMap.SetTile(currentTile.tilePosition, mountainTile);
            }
            if (currentTile.isLand && currentTile.isSnowy)
            {
                tileMap.SetTile(currentTile.tilePosition, snowTile);
            }
            if (currentTile.isMountain && currentTile.isSnowy)
            {
                tileMap.SetTile(currentTile.tilePosition, snowMountainTile);
            }
            if (currentTile.isLand && currentTile.isForest)
            {
                tileMap.SetTile(currentTile.tilePosition, forestTile);
            }


        }

    }

    private void testRun() // use this to run more advanced checks/debuging.
    {
        foreach (GameTile gameTile in mapGrid)
        {
            GameTile currentTile = gameTile;

            if (currentTile.isMountain)
            {
                Debug.Log("GameTile no: " + currentTile.name + "is set to mountains");

            }
 



        }


    }


}
