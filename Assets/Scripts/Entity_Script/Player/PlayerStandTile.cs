using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerStandTile : MonoBehaviour
{
    public Tilemap targetTilemap;

    public int standTileNum;            //Ǯ
    

    public void RewnewStandTile()
    {
        Vector3Int cellPosition = targetTilemap.WorldToCell(transform.position - Vector3Int.up);

        TileBase standTile = targetTilemap.GetTile(cellPosition);

        if (standTile != null)
        {            
            string tileName = standTile.name.ToLower();

            if (tileName.Contains("grass"))
            {
                standTileNum = 0;
            }
            else if (tileName.Contains("plank"))
            {
                standTileNum = 1;
            }
            else if (tileName.Contains("civilization"))
            {
                standTileNum = 1;
            }
        }
    }
}
