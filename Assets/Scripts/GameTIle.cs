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
    public GameTile[] tileNeighbours;


    public GameTile(int xCoordinate, int yCoordinate)
    {
        tilePosition = new Vector3Int(xCoordinate, yCoordinate, 0); //we were declaring a new varible here by putting vector3Int in front of tilePosition.

        spriteList = new List<string> { "water", "sand", "grass" };

        string currentSprite = "water";
    }


    public void SetName(string newName)
    {
        name = newName;

    }

    public void setNeighbours(GameTile[] neighbours)
    {
        tileNeighbours = neighbours;
    }

    public void setLandOrSea(bool isItLand)
    {
        isLand = isItLand;
        if(isLand == true)
        {
            currentSprite = spriteList[2];
        }
        else
        {
            //set sprite to water
        }
    }



}
