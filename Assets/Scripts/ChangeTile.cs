using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeTile : MonoBehaviour
{
    private Main main;
    private Tile tilePlaced; //the tile that this button will place
    private GameObject oldTile; // the tile this will replace
    public int tileColumn, tileRow;

    private void Start()
    {
        main = FindObjectOfType<Main>();
    }
    public void initVariables(Tile placedTile, GameObject replacedTile, int column, int row)
    {
        tilePlaced = placedTile;
        oldTile = replacedTile;
        tileColumn = column;
        tileRow = row;
    }

    public void changeTile()
    {
        main.Grid[tileColumn, tileRow] = tilePlaced;
        main.redrawTile(oldTile);        
    }
}
