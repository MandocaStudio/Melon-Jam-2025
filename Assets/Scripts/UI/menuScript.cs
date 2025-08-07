using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEngine.Audio;
using UnityEngine.UI;
public class menuScript : MonoBehaviour
{

    public GameObject optionsMenu;
    public GameObject firstButtonOptionMenu;
    public GameObject mainMenu;
    public GameObject firstButtonMainMenu;

    public GameObject creditSection;
    public GameObject firstButtoncreditSection;

    public GameObject imageTitle;

    private string currentControlScheme;

    [Header("variables de audio")]
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;
    //public Slider ambientSlider;

    [SerializeField] private AudioMixer audioMixer;

    private const float MIN_DB = -80f;
    private const float MAX_DB = 0f;
    private const float MIN_VOLUME = 0.0001f; // evita log(0)

    string scheme;
    void OnEnable()
    {
        InputSystem.onActionChange += HandleDeviceChange;
    }

    void OnDisable()
    {
        InputSystem.onActionChange -= HandleDeviceChange;
    }

    void HandleDeviceChange(object action, InputActionChange change)
    {
        if (change == InputActionChange.ActionStarted)
        {
            scheme = PlayerInput.all[0].currentControlScheme;

            //Debug.Log(scheme);
            if (scheme != currentControlScheme)
            {

                currentControlScheme = scheme;

            }
        }
    }

    public void openOptionsMenu()
    {
        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);

        imageTitle.SetActive(false);

        StartCoroutine(FocusNextButton(firstButtonOptionMenu));


    }



    public void openMainMenu()
    {
        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);
        if (creditSection.activeSelf)
        {
            creditSection.SetActive(false);

        }

        imageTitle.SetActive(true);


        StartCoroutine(FocusNextButton(firstButtonMainMenu));

    }

    public void openCreditsSection()
    {
        mainMenu.SetActive(false);
        creditSection.SetActive(true);

        imageTitle.SetActive(false);


        StartCoroutine(FocusNextButton(firstButtoncreditSection));

    }

    public void quitGame()
    {
        Application.Quit();
    }

    public void playGame()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        SceneManager.LoadScene(currentSceneIndex + 1);
    }



    public void goToTutorial()
    {
        SceneManager.LoadScene("tutorial");
    }

    public void fullScream(bool fullScream)
    {
        Screen.fullScreen = fullScream;
    }

    public void changeGeneralVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
        PlayerPrefs.SetFloat("volume", volume);
    }

    public void changeSFX(float volume)
    {
        audioMixer.SetFloat("sfx", volume);
        PlayerPrefs.SetFloat("sfx", volume);
    }

    public void changeMusic(float volume)
    {
        audioMixer.SetFloat("music", volume);
        PlayerPrefs.SetFloat("music", volume);
    }

    public void changeAmbient(float volume)
    {
        audioMixer.SetFloat("ambient", volume);
        PlayerPrefs.SetFloat("ambient", volume);
    }


    IEnumerator FocusNextButton(GameObject button)
    {
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(button);
    }

    // private float VolumeToDecibels(float volume)
    // {
    //     return volume > MIN_VOLUME ? Mathf.Log10(volume) * 20f : MIN_DB;
    // }


    private void Update()
    {
        // Solo si no hay ningún objeto seleccionado
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            // Detectar entrada de teclado o gamepad
            //Debug.Log("entro");

            if (optionsMenu.activeSelf && (scheme == "Gamepad" || scheme == "DualShockGamepad" || scheme == "KeyBoard"))
            {
                EventSystem.current.SetSelectedGameObject(firstButtonOptionMenu);

            }
            else if (mainMenu.activeSelf && (scheme == "Gamepad" || scheme == "DualShockGamepad" || scheme == "KeyBoard"))
            {
                EventSystem.current.SetSelectedGameObject(firstButtonMainMenu);
            }
            else if (creditSection.activeSelf && (scheme == "Gamepad" || scheme == "DualShockGamepad" || scheme == "KeyBoard"))
            {
                EventSystem.current.SetSelectedGameObject(firstButtoncreditSection);
            }

        }
    }



    void Start()
    {
        SetSliderAndVolume(masterSlider, "volume");
        SetSliderAndVolume(musicSlider, "music");
        SetSliderAndVolume(sfxSlider, "sfx");
        //SetSliderAndVolume(ambientSlider, "ambient");
    }

    void SetSliderAndVolume(Slider slider, string key)
    {
        float savedVolume = PlayerPrefs.HasKey(key) ? PlayerPrefs.GetFloat(key) : 0.5f;
        slider.value = savedVolume;
        audioMixer.SetFloat(key, savedVolume);
    }

    public void ChangeVolume(Slider slider, string key)
    {
        float volume = slider.value;
        PlayerPrefs.SetFloat(key, volume);
        audioMixer.SetFloat(key, volume);
    }



}
