using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IndividualPlayer_Script : MonoBehaviour
{
    public List<PointersScript> IndividualScript = new List<PointersScript>();
    public TextMeshProUGUI diceValue;
    public bool DiceRolled = false,MyTurnOverBool,YesSix,YesCaptured,ImWin;
    public Button DiceButton;
    public int DiceValue,unLockedCount,CountOfWins,CountOfUnlockedAndReached;
    public Image buttonImage;
    public Sprite _1, _2, _3, _4, _5, _6;
    public Animator DiceAnimator;
    public GameObject TurnHighligher;
    public void RollDice()
    {   GameManager.Instance.PlayDiceClip();
        DiceButton.interactable = false;
        TurnHighligher.SetActive(false);
        DiceAnimator.enabled = true;
        DiceAnimator.Play(0);
    ///    YesCaptured = YesCaptured?false:false;
        Invoke("RollDiceWithAnimation", .5f);
    }
    void RollDiceWithAnimation()
    {
        DiceAnimator.enabled = false;
     
        DiceRolled = true;
        DiceValue = Random.Range(1, 7);
        switch (DiceValue)
        {
            case 1:
                buttonImage.sprite = _1;
                break;
            case 2:
                buttonImage.sprite = _2;
                break;
            case 3:
                buttonImage.sprite = _3;
                break;
            case 4:
                buttonImage.sprite = _4;
                break;
            case 5:
                buttonImage.sprite = _5;
                break;
            case 6:
                buttonImage.sprite = _6;
                break;
        }
        YesSix = DiceValue == 6 ? true : false;
     
        foreach (PointersScript ps in IndividualScript) 
        {
            ps.GetDiceValue = DiceValue;
            if (ps.unlocked)
                unLockedCount++;
            if(ps.completePath.Count >= (ps.IndexOfPath + 1 + DiceValue) && (ps.unlocked || DiceValue==6))
                ps.ScaleUpHighlight();    //here call a random player to move with computer
            
        }
        ////*.Log(unLockedCount);
        foreach (PointersScript ps in IndividualScript)
        {
            if (unLockedCount != 0 && ps.unlocked && (ps.IndexOfPath + 1 + DiceValue > ps.completePath.Count))
            {
                CountOfUnlockedAndReached++;
               
            }
        }
        if(CountOfUnlockedAndReached==unLockedCount && unLockedCount!=0 && unLockedCount==IndividualScript.Count)
        {
                YesSix = false;
                MyTurnOverBool = true;
            CountOfUnlockedAndReached = 0;
        }
            if (unLockedCount == 0 && DiceValue!=6)
            MyTurnOverBool = true;
        if (unLockedCount == CountOfUnlockedAndReached && DiceValue !=6) MyTurnOverBool = true;
        unLockedCount = 0;
        CountOfUnlockedAndReached = 0;
    }
    void Start()
    {
        //*.Log(IndividualScript.Count);
    }
    private void Update()
    {
        diceValue.text = DiceValue.ToString("0");
    }
    public void SetDiceValueInAllPoints(PointersScript psRef)
    {
        foreach (PointersScript ps in IndividualScript)
        {
            if (ps != psRef)
                ps.GetDiceValue = 0;
            ps.ScaleDownToNormal();
        }
    }
    public void DisableAllButtons()
    {

        DisableRayCast();
        MyTurnOverBool = false;
        DiceButton.interactable = false;
    }
    public void EnableAllButtons()
    {
        TurnHighligher.SetActive(true);
        DiceAnimator.enabled = true;
        EnableRayCast();
        MyTurnOverBool = false;
        DiceButton.interactable = true;
    }
    public void RemoveWinnerPoint(PointersScript ps)
    {
        CountOfWins++;
        if (CountOfWins != 4)
            EnableAllButtons();
        if (CountOfWins == 4)
            ImWin = true;
        IndividualScript.Remove(ps);
    }
    void DisableRayCast()
    {
        foreach(PointersScript pt in IndividualScript)
        {
            pt.GetComponent<Image>().raycastTarget = false;
        }
    }
    void EnableRayCast()
    {
        foreach(PointersScript pt in IndividualScript)
        {
            pt.GetComponent<Image>().raycastTarget = true;
        }
    }
}
