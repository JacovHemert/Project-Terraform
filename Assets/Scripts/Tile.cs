using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObjects/Tile")]
public class Tile: ScriptableObject
{    
    public string tileName;
    public Sprite tileSprite;
    public string[] requiredPlaceTiles; //the names of tiles on which this tile can be placed
    public string requiredAdjacentTile; //the name of the tile that needs to be adjacent to the spot this tile is to be placed
    public int requiredAdjacentTileAmount; // the number of the required adjacent tiles there needs to be for this tile to be placed
}
