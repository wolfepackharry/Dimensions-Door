using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class DimensionTilemapFiller : MonoBehaviour
{
    [SerializeField] Tilemap _tilemapToCopy;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [Button]
    private void UpdateTiles()
    {
        GetComponent<Tilemap>().ClearAllTiles();
        foreach (Vector3Int pos in _tilemapToCopy.cellBounds.allPositionsWithin)
        {
            TileBase tile = _tilemapToCopy.GetTile(pos);
            if (tile != null)
            {
                GetComponent<Tilemap>().SetTile(pos, tile);
            }
        }
        gameObject.SetActive(false);
    }
    // Update is called once per frame
    
}
