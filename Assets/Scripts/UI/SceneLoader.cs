using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    // --- ACCIÓN REQUERIDA ---
    // Escribe aquí los nombres EXACTOS de tus 4 escenas principales.
    private const string MAIN_MENU_SCENE_NAME = "main menu";
    private const string GAME_SCENE_NAME = "Game";         // Reemplaza esto
    private const string VICTORY_SCENE_NAME = "Victory";   // Reemplaza esto
    private const string DEFEAT_SCENE_NAME = "Defeat";     // Reemplaza esto

    // --- Métodos para Victoria/Derrota (Ya los tienes) ---
    public static void LoadVictoryScene()
    {
        Debug.Log($"Cargando escena: {VICTORY_SCENE_NAME}");
        SceneManager.LoadScene(VICTORY_SCENE_NAME);
    }

    public static void LoadDefeatScene()
    {
        Debug.Log($"Cargando escena: {DEFEAT_SCENE_NAME}");
        SceneManager.LoadScene(DEFEAT_SCENE_NAME);
    }
    
    // --- [NUEVOS] Métodos para los botones del menú ---
    
    /// <summary>
    /// Carga la escena principal del Menú.
    /// </summary>
    public static void LoadMainMenuScene()
    {
        Debug.Log($"Cargando escena: {MAIN_MENU_SCENE_NAME}");
        SceneManager.LoadScene(MAIN_MENU_SCENE_NAME);
    }

    /// <summary>
    /// Carga la escena principal del Juego (para "Jugar de Nuevo").
    /// </summary>
    public static void LoadGameScene()
    {
        Debug.Log($"Cargando escena: {GAME_SCENE_NAME}");
        SceneManager.LoadScene(GAME_SCENE_NAME);
    }
}