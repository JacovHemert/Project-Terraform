using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    public Tile[,] Grid;

    [SerializeField] private List<Tile> desertTiles = new List<Tile>();
    [SerializeField] private List<Tile> terraformedTiles = new List<Tile>();
    [SerializeField] private GameObject tilePlacer, contextPanel;

    private bool contextMenuOpen = false;

    //prefabs
    [SerializeField] private GameObject tilePrefab, tileButtonPrefab;

    private void Start()
    {
        initialiseTileGrid(5, 5);
        drawTiles();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (contextMenuOpen) { closeContextMenu(); }
            contextMenuOpen = true;
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                int column, row;
                column = hit.collider.gameObject.GetComponent<TileVariables>().column;
                row = hit.collider.gameObject.GetComponent<TileVariables>().row;
                List<Tile> adjacentTiles = getAdjacentTiles(column, row);
                openContextMenu(getContextTiles(Grid[column,row], adjacentTiles), hit.collider.gameObject);                
            }
        }else if (Input.GetButtonDown("Cancel") && contextMenuOpen)
        {
            closeContextMenu();
        }
    }

    private void initialiseTileGrid(int numberColumns, int numberRows) // creating the randomised grid from the scriptable object list of desert tiles
    {
        Grid = new Tile[numberColumns, numberRows];

        for (int column = 0; column < numberColumns; column++)
        {
            for (int row = 0; row < numberRows; row++)
            {
                Grid[column, row] = desertTiles[Random.Range(0, desertTiles.Count)];
            }
        }
    }

    private void drawTiles() // initial draw of all the tiles
    {        
        for (int column = 0; column < Grid.GetLength(0); column++)
        {
            for (int row = 0; row < Grid.GetLength(1); row++)
            {
                GameObject newTile = Instantiate(tilePrefab, tilePlacer.transform.position, Quaternion.identity);                
                newTile.GetComponent<SpriteRenderer>().sprite = Grid[column, row].tileSprite;
                tilePlacer.transform.position = new Vector2(tilePlacer.transform.position.x, tilePlacer.transform.position.y - 1);
                newTile.GetComponent<TileVariables>().column = column;
                newTile.GetComponent<TileVariables>().row = row;
            }
            tilePlacer.transform.position = new Vector2(tilePlacer.transform.position.x + 1, tilePlacer.transform.position.y + Grid.GetLength(1));
        }
    }

    public void redrawTile(GameObject redrawnTile) // method to destroy one tile and make a new one
    {
        Vector2 oldTilePos = redrawnTile.transform.position;
        int column = redrawnTile.GetComponent<TileVariables>().column;
        int row = redrawnTile.GetComponent<TileVariables>().row;        

        GameObject.Destroy(redrawnTile);
        GameObject newTile = Instantiate(tilePrefab, oldTilePos, Quaternion.identity);
        newTile.GetComponent<SpriteRenderer>().sprite = Grid[column, row].tileSprite;        
        newTile.GetComponent<TileVariables>().column = column;
        newTile.GetComponent<TileVariables>().row = row;

        closeContextMenu();
    }

    private void openContextMenu(List<Tile> contextTiles, GameObject currTileObject)
    {
        contextPanel.SetActive(true);
        contextPanel.transform.position = Input.mousePosition;
        for (int i = 0; i < contextTiles.Count; i++)
        {
            GameObject newTileButton = Instantiate(tileButtonPrefab, contextPanel.transform, false);            
            newTileButton.GetComponent<Image>().sprite = contextTiles[i].tileSprite;
            newTileButton.GetComponent<ChangeTile>().initVariables(contextTiles[i], currTileObject, currTileObject.GetComponent<TileVariables>().column, currTileObject.GetComponent<TileVariables>().row);
        }
        
    }
    private void closeContextMenu()
    {
        contextMenuOpen = false;
        foreach (Transform tileButton in contextPanel.transform)
        {
            GameObject.Destroy(tileButton.gameObject);            
        }
        contextPanel.SetActive(false);
    }

    private List<Tile> getAdjacentTiles(int column, int row)
    {
        List<Tile> adjacentTiles = new List<Tile>();

        for (int col = column - 1; col < column + 2; col++)
        {
            for (int ro = row - 1; ro < row + 2; ro++)
            {   // conditional so that the program doesn't count the current tile as an adjacent tile
                if (!(col == column && ro == row))
                { // conditional so that the program doesn't try to add tiles outside the grid
                    if (col >= 0 && col <= Grid.GetLength(0)-1 && ro >= 0 && ro <= Grid.GetLength(1)-1)
                    {
                        adjacentTiles.Add(Grid[col, ro]);
                    }                    
                }                
            }
        }
        return adjacentTiles;
    }

    private List<Tile> getContextTiles(Tile middleTile, List<Tile> adjacentTiles)
    {
        List<Tile> contextTiles = new List<Tile>();        
        //cycling through each potential tile that can be placed
        for (int terraformCount = 0; terraformCount < terraformedTiles.Count; terraformCount++)
        {
            bool validPlaceTile = false;
            //checking to see if the placement tile is valid
            for (int placeTileCount = 0; placeTileCount < terraformedTiles[terraformCount].requiredPlaceTiles.Length; placeTileCount++)
            {
                if (terraformedTiles[terraformCount].requiredPlaceTiles[placeTileCount] == middleTile.tileName)
                {
                    validPlaceTile = true;
                    break;
                }
            }
            int adjcount = 0;
            bool validAdjTiles = false;
            //checking to see if the adjacent tile criteria are met
            if(terraformedTiles[terraformCount].requiredAdjacentTile != "")
            {
                for (int adjTileCount = 0; adjTileCount < adjacentTiles.Count; adjTileCount++)
                {
                    if (adjacentTiles[adjTileCount].tileName == terraformedTiles[terraformCount].requiredAdjacentTile)
                    { adjcount += 1; }

                    if (adjcount >= terraformedTiles[terraformCount].requiredAdjacentTileAmount)
                    {
                        validAdjTiles = true;
                        break;
                    }
                }
            }
            else { validAdjTiles = true; }            

            if(validPlaceTile && validAdjTiles)
            {
                contextTiles.Add(terraformedTiles[terraformCount]);
            }
        }
        return contextTiles;
    }
}