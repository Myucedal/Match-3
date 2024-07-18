using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public Button startButton;
    public Button settingsButton;
    public Button closeButton;
    public GameObject settingsPanel;
    public GameObject closePanel;
    public Button settingsCloseButton;
    public Button closePanelCloseButton;
    public Button closePanelExitButton;

    void Start()
    {
        startButton.onClick.AddListener(StartGame);
        settingsButton.onClick.AddListener(OpenSettings);
        closeButton.onClick.AddListener(OpenClosePanel);

        settingsPanel.SetActive(false);
        closePanel.SetActive(false);
        settingsCloseButton.onClick.AddListener(CloseSettings);
        closePanelCloseButton.onClick.AddListener(CloseClosePanel);
        closePanelExitButton.onClick.AddListener(closePanelExit);
    }

    void StartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }

    void OpenClosePanel()
    {
        closePanel.SetActive(true);
    }
    void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }

    void CloseClosePanel()
    {
        closePanel.SetActive(false);
    }
    void closePanelExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Uygulamayý kapat
        Application.Quit();
#endif    
    }
}
