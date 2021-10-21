using System;
using TechnOllieG;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static bool isPaused = false;
    
    public Slider renderDistanceSlider;
    public Text renderDistanceCounter;
    public GameObject pauseMenu;
    public Button continueButton;
    public Button quitButton;
    
    private CursorLockMode _originalLockMode;

    private void Start()
    {
        _originalLockMode = Cursor.lockState;
        renderDistanceSlider.onValueChanged.AddListener(OnSliderDrag);
        continueButton.onClick.AddListener(OnContinue);
        quitButton.onClick.AddListener(OnQuit);
    }

    private void OnSliderDrag(float renderDistance)
    {
        renderDistanceCounter.text = $"{(int) renderDistance}";
        TerrainGenerationSystem.Instance.SetRenderDistance((int) renderDistance);
    }

    private void OnQuit()
    {
        Application.Quit();
    }

    private void OnContinue()
    {
        SetPauseState(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetPauseState(!isPaused);
        }
    }

    private void SetPauseState(bool newState)
    {
        isPaused = newState;
        pauseMenu.SetActive(isPaused);
        Cursor.lockState = isPaused ? CursorLockMode.None : _originalLockMode;
    } 
}
