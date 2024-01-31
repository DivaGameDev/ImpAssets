using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManagerForComputers : MonoBehaviour
{
    public static GameManagerForComputers Instance;
    public List<IndividualPlayerScriptForComputers> allPlayers = new List<IndividualPlayerScriptForComputers>();
    public List<GameObject> winnerImages = new List<GameObject>();
    public int i = 0, NoOfPlayers, winnerPosition = 0;
    public AudioSource movingClip, diceClip;
    public int maxPlayers;
    public GameObject GameOver;
    private void Start()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        foreach (IndividualPlayerScriptForComputers IPS in allPlayers)
            IPS.DisableAllButtons();
        allPlayers[0].EnableAllButtons();
        allPlayers[0].TurnHighligher.SetActive(true);
    }
    void Update()
    {
        i = i <= 0 ? 0 : i;
        if (allPlayers[i] != null)
            if ((allPlayers[i].MyTurnOverBool && !allPlayers[i].YesSix && !allPlayers[i].YesCaptured && allPlayers[i].ImDone) || allPlayers[i].ImWin)//true,no six,no capture
            {
                // if (allPlayers[i].ImWin) NoOfPlayers--;
                allPlayers[i].DisableAllButtons();
                if (i >= NoOfPlayers)
                    i = 0;
                else
                    i++;
                    allPlayers[i].Invoke("EnableAllButtons", .5f);
            }
        foreach (IndividualPlayerScriptForComputers ips in allPlayers)
        {
            if (ips.CountOfWins == 4)
            {
                foreach (GameObject go in winnerImages)
                    if (go.name == ips.name)
                    {
                        winnerPosition++;
                        go.SetActive(true);
                        go.GetComponentInChildren<TextMeshProUGUI>().text = winnerPosition.ToString();
                        NoOfPlayers--;
                        i--;
                    }
               // Debug.Log("one player is won");
                if (winnerPosition == maxPlayers)
                {
                    GameOver.SetActive(true);
                }/////here need to show the end screen of after all players completed

                allPlayers.Remove(ips);

            }
        }

    }
    public void MoveTurnToNextPlayer()
    {

        allPlayers[i].DisableAllButtons();
        if (i >= NoOfPlayers)
            i = 0;
        else
            i++;
        allPlayers[i].Invoke("EnableAllButtons", 0.25f);

    }
    public void PlayMovingClip()
    {
        movingClip.Play();
    }
    public void PlayDiceClip()
    {
        diceClip.Play();
    }
}
