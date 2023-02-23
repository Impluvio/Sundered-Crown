using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexTile : ScriptableObject
{
    
    public enum TileType
    {
        Grass,
        Water,
        Forest,
    }

    public GameObject Grass;
    public GameObject Water;
    public GameObject Forest;

    public GameObject GetTile(TileType tileType)
    {
        switch (tileType)
        {
            case TileType.Grass:
                return Grass;
            case TileType.Water:
                return Water;
            case TileType.Forest:
                return Forest; 
        }
        return null;
    }


}
