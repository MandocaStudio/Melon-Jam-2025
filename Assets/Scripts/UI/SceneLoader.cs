using UnityEngine;
using UnityEngine.SceneManagement; 

/// <summary>
/// Clase estática de ayuda para cargar escenas.
/// (Versión sin FadeManager - Carga Instantánea)
/// </summary>
public static class SceneLoader
{
    // --- Nombres de escena actualizados según tus imágenes ---
    // (Asegúrate de que las mayúsculas y minúsculas sean exactas)
    private const string MAIN_MENU_SCENE_NAME = "main menu"; 
    private const string GAME_SCENE_NAME = "Game";         
    private const string VICTORY_SCENE_NAME = "Victory";   
    private const string DEFEAT_SCENE_NAME = "Defeat";     
    // (Añadí tu escena de logos también, por si la necesitas)
    private const string LOGOS_SCENE_NAME = "logos";
    
    
    /// <summary>
    /// Carga la escena de Victoria (instantáneamente).
    /// </summary>
    public static void LoadVictoryScene()
    {
        Debug.Log($"Cargando escena: {VICTORY_SCENE_NAME}");
        SceneManager.LoadScene(VICTORY_SCENE_NAME);
    }

    /// <summary>
    /// Carga la escena de Derrota (instantáneamente).
    /// </summary>
    public static void LoadDefeatScene()
    {
        Debug.Log($"Cargando escena: {DEFEAT_SCENE_NAME}");
        SceneManager.LoadScene(DEFEAT_SCENE_NAME);
    }
    
    /// <summary>
    /// Carga la escena principal del Menú (instantáneamente).
    /// </summary>
    public static void LoadMainMenuScene()
    {
        Debug.Log($"Cargando escena: {MAIN_MENU_SCENE_NAME}");
        SceneManager.LoadScene(MAIN_MENU_SCENE_NAME);
    }

    /// <summary>
    /// Carga la escena principal del Juego (instantáneamente).
    /// </summary>
    public static void LoadGameScene()
    {
        Debug.Log($"Cargando escena: {GAME_SCENE_NAME}");
        SceneManager.LoadScene(GAME_SCENE_NAME);
    }
}