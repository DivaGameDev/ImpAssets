using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class UIScript : MonoBehaviour
{
    [SerializeField]
    public GameObject ExitPanel;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.Escape))
        {
            ExitPanel.SetActive(true);
        }
    }
    public void ExitApplication()
    {
        Application.Quit();
    }
    public void PlayWithFriends()
    {
        SceneManager.LoadScene(1);
    }
    public void PlayWithComputer()
    {
        SceneManager.LoadScene(3);
    }
    public void StopPlaying()
    {
        SceneManager.LoadScene(0);
    }
    public void PlayAgainSame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void LoadAccordingScene(int sceneNo)
    {
        SceneManager.LoadScene(sceneNo);
    }
}
