using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IndividualPlayerScriptForComputers : MonoBehaviour
{
    public List<PointersScriptForComputers> IndividualScript = new List<PointersScriptForComputers>();
    public TextMeshProUGUI diceValue;
    public bool DiceRolled = false, MyTurnOverBool, YesSix, YesCaptured, ImWin,ImDone;
    public Button DiceButton;
    public int DiceValue, unLockedCount, CountOfWins, CountOfUnlockedAndReached;
    public Image buttonImage;
    public Sprite _1, _2, _3, _4, _5, _6;
    public Animator DiceAnimator;
    public GameObject TurnHighligher;
    public void RollDice()
    {
        ImDone = false;
        GameManagerForComputers.Instance.PlayDiceClip();
        DiceButton.interactable = false;
        TurnHighligher.SetActive(false);
        DiceAnimator.enabled = true;
        DiceAnimator.Play(0);
        ///    YesCaptured = YesCaptured?false:false;
        Invoke("RollDiceWithAnimation", .5f);
    }
    public void RollDiceByComputer()
    {
        ImDone = false;
        GameManagerForComputers.Instance.PlayDiceClip();
        DiceButton.interactable = false;
        TurnHighligher.SetActive(false);
        DiceAnimator.enabled = true;
        DiceAnimator.Play(0);
        ///    YesCaptured = YesCaptured?false:false;
        Invoke("RollDiceWithAnimationByComputer", .5f);
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
       // YesSix = DiceValue == 6 ? true : false;

        foreach (PointersScriptForComputers ps in IndividualScript)
        {
                ps.GetDiceValue = DiceValue;
            if (ps.unlocked)
                unLockedCount++;
            if (ps.completePath.Count >= (ps.IndexOfPath + 1 + DiceValue) && (ps.unlocked || DiceValue == 6))
                ps.ScaleUpHighlight();    //here call a random player to move with computer

        }
        ////*.Log(unLockedCount);
        foreach (PointersScriptForComputers ps in IndividualScript)
        {
            if (unLockedCount != 0 && ps.unlocked && (ps.IndexOfPath + 1 + DiceValue > ps.completePath.Count))
            {
                CountOfUnlockedAndReached++;

            }
        }
        if (CountOfUnlockedAndReached == unLockedCount && unLockedCount != 0 && unLockedCount == IndividualScript.Count)
        {
            YesSix = false;
            MyTurnOverBool = true;
            ImDone = true;
            CountOfUnlockedAndReached = 0;
        }
        if (unLockedCount == 0 && DiceValue != 6)
        {
            ImDone = true;
            MyTurnOverBool = true;
        }
        if (unLockedCount == CountOfUnlockedAndReached && DiceValue != 6)
        {
            ImDone = true;
            MyTurnOverBool = true;
        }
        unLockedCount = 0;
        CountOfUnlockedAndReached = 0;
    }
    void RollDiceWithAnimationByComputer()
    {
        unLockedCount = 0;
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
      //  YesSix = DiceValue == 6 ? true : false;

        foreach (PointersScriptForComputers ps in IndividualScript)
        {
            // ps.GetDiceValue = DiceValue;
            
            if (ps.completePath.Count >= (ps.IndexOfPath + 1 + DiceValue) && (ps.unlocked || DiceValue == 6))
            {
                if (!ps.unlocked && !ps.ImWin)
                {
                    ps.move(DiceValue);    //here call a random player to move with computer
                   return;
                }
                else
                {
                    ps.move(DiceValue);
                  return;
                }
            }
            if (ps.unlocked && (ps.IndexOfPath + DiceValue + 1) <= ps.completePath.Count)
                unLockedCount++;
           // Debug.Log(unLockedCount);
           
        } 
        if(unLockedCount==0 && DiceValue ==6)
            {
            //Debug.Log("Yes triggered");
            MyTurnOverBool = true;
            YesSix = false;
            ImDone = true;
            }
        ////*.Log(unLockedCount);
        foreach (PointersScriptForComputers ps in IndividualScript)
        {
            if (unLockedCount != 0 && ps.unlocked && (ps.IndexOfPath + 1 + DiceValue > ps.completePath.Count))
            {
                CountOfUnlockedAndReached++;

            }
        }
        if (CountOfUnlockedAndReached == unLockedCount && unLockedCount != 0 && unLockedCount == IndividualScript.Count)
        {
            YesSix = false;
            MyTurnOverBool = true;
            ImDone = true;
            CountOfUnlockedAndReached = 0;
        }
        if (unLockedCount == 0 && DiceValue != 6)
        {
            MyTurnOverBool = true;
            ImDone = true;
        }
        if (unLockedCount == CountOfUnlockedAndReached && DiceValue != 6)
        {
            MyTurnOverBool = true;
            ImDone = true;
        }
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
    public void SetDiceValueInAllPoints(PointersScriptForComputers psRef)
    {
        foreach (PointersScriptForComputers ps in IndividualScript)
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
        if (this.gameObject.name != "RedPlayer")
        {
            Invoke("RollDiceByComputer", .5f);
          //  new WaitForSeconds(2);
        }
    }
    public void RemoveWinnerPoint(PointersScriptForComputers ps)
    {
        unLockedCount = 0;
        CountOfWins++;
        if (CountOfWins != 4)
            EnableAllButtons();
        if (CountOfWins == 4)
            ImWin = true;
        IndividualScript.Remove(ps);
    }
    void DisableRayCast()
    {
        foreach (PointersScriptForComputers pt in IndividualScript)
        {
            pt.GetComponent<Image>().raycastTarget = false;
        }
    }
    void EnableRayCast()
    {
        foreach (PointersScriptForComputers pt in IndividualScript)
        {
            pt.GetComponent<Image>().raycastTarget = true;
        }
    }
}
