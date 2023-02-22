using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TileGridTypes;
using TileTypes;

public class TileManager : MonoBehaviour
{
    [SerializeField] private int gridX = 15, gridY = 10;
    [SerializeField] private Vector2Int offsetFarm, offsetProcessing;

    private static TileManager tileManager; 
    public static TileManager instance 
    { 
        get 
        { 
            if (!tileManager)
            {
                tileManager = FindObjectOfType<TileManager>();
                if (!tileManager)
                {
                    Debug.LogError("There needs to be one tileManager in the scene.");
                }
                else
                {
                    tileManager.Initialize();
                }
            }
            return tileManager;
        } 
    }

    private TileGrid farmTileGrid;
    private TileGrid processingTileGrid;

    private void Awake()
    {
        if (!tileManager) { Initialize(); }
    }

    private void Initialize()
    {
        farmTileGrid = new TileGrid(TileGridType.farmTileGrid, new Vector2Int(gridX, gridY), offsetFarm);
        processingTileGrid = new TileGrid(TileGridType.processingTileGrid, new Vector2Int(gridX, gridY), offsetProcessing);
    }

    public static void AddToTile(TileGridType targetTileGrid, GameObject gameObject, Vector2Int targetTile, Vector3 offset = new Vector3(), Vector3 scale = new Vector3())
    {
        TileGrid targetGrid = (targetTileGrid == TileGridType.farmTileGrid) ? instance.farmTileGrid : instance.processingTileGrid;
        bool success = targetGrid.AddToTile(gameObject, targetTile, offset, scale);
        if (success) { instance.RecalculateMachines(); }
    }

    // remove from tile
    public static void RemoveFromTile(TileGridType targetTileGrid, Vector2Int targetTile)
    {
        TileGrid targetGrid = (targetTileGrid == TileGridType.farmTileGrid) ? instance.farmTileGrid : instance.processingTileGrid;
        bool isCropGrown = targetGrid.GetTile(targetTile).IsCropFullyGrown();
        bool success = targetGrid.RemoveFromTile(targetTile);
        if (success) 
        {
            if (isCropGrown) { GameSystem.Instance.AddCurrency(1); }
            instance.RecalculateMachines(); 
        }
    }

    private void RecalculateMachines()
    {
        ResetValues();
        // go through our grid and recalculate modifiers 
        foreach (Tile tile in farmTileGrid.tileGrid) { 
            //crop booster logic
            if (tile.HasObject() && tile.tileScript.tileType == TileType.Booster)
            {
                //get all crop tiles in radius
                List<Tile> cropTiles = farmTileGrid.GetTilesInRadius(tile.position, TileType.Crop, 2);
                foreach (Tile cropTile in cropTiles)
                { 
                    Crop crop = (Crop)cropTile.tileScript;
                    crop.addGrowSpeed(1f);
                }
            }
        }
    }

    // resets the modifiers for things that can be changed, called before machine calculation
    private void ResetValues()
    {
        foreach (Tile tile in farmTileGrid.tileGrid)
        {
            if (tile.tileScript?.tileType == TileType.Crop)
            {
                tile.tileScript.Reset();
            }
        }
    }

    public TileGrid GetTileGrid(TileGridType tileGridType)
    {
        if (tileGridType == TileGridType.farmTileGrid) { return farmTileGrid; }
        else { return processingTileGrid; }
    }
}
