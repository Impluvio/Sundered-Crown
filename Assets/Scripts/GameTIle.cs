using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTile
{
    public string name;
    public Vector3Int tilePosition;
    //CHECK: - see if it is possible to access the list of tiles stored in Assets/Graphics/Tiles part of the project, and pass these into an array or list. 
    // For now we will just use a short list

    //CHECK if we need parameterized constructor for this.

    public List<string> spriteList;
    public string currentSprite;
    public bool isLand;
    public bool isMountain;
    public bool isLargeMountain;
    public bool isSnowy;
    public bool isForest;
    public GameTile[] tileNeighbours;
    [Range(-10, 10)] public int elevation;  // 0 is sea level.
    public int distanceFromEquator;         // this is a abs. row from the equator the equator being 0 and 1 being one row above or below equator etc.


    public GameTile(int xCoordinate, int yCoordinate)
    {
        tilePosition = new Vector3Int(xCoordinate, yCoordinate, 0); //we were declaring a new varible here by putting vector3Int in front of tilePosition.
        spriteList = new List<string> { "water", "sand", "grass", "mountain","Large Mountain","forest" };
        string currentSprite = "water";


    }

    public void SetName(string newName)
    {
        name = newName;

    }

    public void SetEquatorDistance(int distanceToMid)
    {

        distanceFromEquator = distanceToMid;

    }


    public void SetNeighbours(GameTile[] neighbours)
    {
        tileNeighbours = neighbours;
    }

    //the below bools are kinda lazy, we could set these bools and then check them at the end.
    //switch these to quick if statements. 

    public void SetLandOrSea(bool isItLand)
    {
        isLand = isItLand;
        if(isLand == true)
        {
            currentSprite = spriteList[2];
        }
        else
        {
            //set sprite to water (remains water);
        }
    }

    public void SetMountains(bool isItMountain)
    {
        isMountain = isItMountain;
        if(isMountain == true)
        {
            currentSprite = spriteList[3];
        }
        else
        {

        }

    }

    public void SetMountainSpine(bool isItLargemountain) // be aware that there is a weakness here in the code as it is highly dependant on execution order.
    {
        isLargeMountain = isItLargemountain;
        if (isLargeMountain == true);
        {
            currentSprite = spriteList[4]; //this is arbitrary and needs a better solution.
        }

    }

    public void SetForest(bool isItForest)
    {
        isForest = isItForest;
        if (isForest == true)
        {
            currentSprite = spriteList[5];
        }
        else
        {

        }
    }

    public void SetSnow(bool isItArtic)
    {
        isSnowy = isItArtic;
        if(isSnowy == true)
        {
         
        }
    }
        
    


}
