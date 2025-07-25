using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class GridPositionerFixedIndex : MonoBehaviour
{
    [Header("Columnas (X)")]
    public float[] columnPositions = new float[3] { 0f, 2f, 4f };

    [Header("Filas")]
    public int rowCount = 5;
    public float rowStartY = -2f;
    public float rowSpacing = 1.5f;

    [Header("Opciones")]
    public bool autoUpdate = true;
    public bool showTilesInPlayMode = false;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!Application.isPlaying && autoUpdate)
        {
            UpdateTilePositions();
        }
    }
#endif

    private void OnEnable()
    {
        HandleVisibility();
    }

    private void HandleVisibility()
    {
        for (int col = 0; col < columnPositions.Length; col++)
        {
            for (int row = 0; row < rowCount; row++)
            {
                string tileName = $"Tile_{col}_{row}";
                Transform tile = transform.Find(tileName);

                if (tile != null)
                {
                    bool shouldBeActive = !Application.isPlaying || showTilesInPlayMode;
                    tile.gameObject.SetActive(shouldBeActive);
                }
            }
        }
    }

    private void UpdateTilePositions()
    {
        for (int col = 0; col < columnPositions.Length; col++)
        {
            for (int row = 0; row < rowCount; row++)
            {
                string tileName = $"Tile_{col}_{row}";
                Transform tile = transform.Find(tileName);

                if (tile != null)
                {
                    float posX = columnPositions[col];
                    float posY = rowStartY + row * rowSpacing;
                    tile.position = transform.position + new Vector3(posX, posY, 0f);
                }
            }
        }
    }
}
