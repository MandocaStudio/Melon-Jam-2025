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
        UpdateTilePositions(); // Actualiza las posiciones de los tiles al iniciar.
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
        // Obtener la rotación actual de la cámara
        float cameraAngle = Camera.main.transform.rotation.eulerAngles.y;

        for (int col = 0; col < columnPositions.Length; col++)
        {
            for (int row = 0; row < rowCount; row++)
            {
                string tileName = $"Tile_{col}_{row}";
                Transform tile = transform.Find(tileName);

                if (tile != null)
                {
                    // Calculamos las posiciones X y Y de acuerdo con la rotación de la cámara
                    float posX = columnPositions[col];
                    float posY = rowStartY + row * rowSpacing;

                    // Mantén la posición Z constante si no necesitas un desplazamiento en Z
                    float posZ = 0f;

                    // Aplica la rotación al tile sin afectar la escala
                    Vector3 offset = new Vector3(posX, posY, posZ);
                    tile.position = transform.position + offset;

                    // Aplique la rotación solo al eje Y sin alterar la escala del tile
                    tile.rotation = Quaternion.Euler(0, cameraAngle, 0);

                    // Si el tile es un cubo, asegúrate de que su escala sea constante
                    tile.localScale = Vector3.one; // Asegura que la escala del tile sea 1 en todos los ejes
                }
            }
        }
    }
}
