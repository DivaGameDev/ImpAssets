using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PointersScriptForComputers : MonoBehaviour, IPointerDownHandler
{
    // Start is called before the first frame update

    public Vector2 ReturnPoint, HouseStartPoint, winPoint;
    public List<Transform> completePath = new List<Transform>();
    public List<RectTransform> ListOfSafePoints = new List<RectTransform>();
    int StepsCompleted, StepsPending;
    public bool AbleToGo = false, unlocked = false, stopCollisions = false, returningHome, OurTurnCompleted = false, ImWin = false, YesCaptured,YesSix;
    public int GetDiceValue;
    public IndividualPlayerScriptForComputers thisParent;
    public int temp = 0, IndexOfPath = 0;
    RectTransform thisRectTransform;
    Animator animationClip;
    float timer = .5f, timer2 = .05f;
    string thisTag;
    CircleCollider2D thisCollider;
    void Start()
    {

        ReturnPoint = GetComponent<RectTransform>().anchoredPosition;
        thisRectTransform = GetComponent<RectTransform>();
        animationClip = GetComponent<Animator>();
        thisTag = gameObject.tag;
        thisCollider = GetComponent<CircleCollider2D>();
        //   Debug.Log(completePath.Count);
    }

    // Update is called once per frame
    void Update()
    {

        //return if this player is win
        //   if (ImWin) re;


        if (IndexOfPath + 1 == completePath.Count)
        {
            //Debug.Log("Win" + completePath.Count);
            ImWin = true;
            GetComponent<PointersScriptForComputers>().enabled = false;
        }
        if (!AbleToGo)
        {
            /*if(GetDiceValue==6)
            {
                //highlight this point;;
            }*/


        }
        if (AbleToGo)
        {
            timer -= Time.deltaTime;
            if (timer <= 0 && temp <= (GetDiceValue + 1) && (IndexOfPath + 1) < completePath.Count && temp<6)
            {
                thisRectTransform.localPosition = completePath[IndexOfPath + 1].gameObject.GetComponent<RectTransform>().anchoredPosition;
                animationClip.Play(0);
                IndexOfPath++;
                temp++;
                timer = .25f;
                thisCollider.enabled = false;
                GameManagerForComputers.Instance.PlayMovingClip();
            }
            if (temp == GetDiceValue && GetDiceValue != 6)
            {
                for (int i = 0; i < ListOfSafePoints.Count; i++)
                {
                    if (thisRectTransform.localPosition == ListOfSafePoints[i].localPosition)
                    {
                        thisCollider.enabled = false;
                        break;
                    }
                    else
                        thisCollider.enabled = true;
                }
                GetDiceValue = 0;
                temp = 0;
                AbleToGo = false;
                Invoke("CheckCollisionAndConfirm", .25f);
                /* if (YesCaptured)
                 {
                     thisParent.Invoke("EnableAllButtons", .5f);
                 }*/
              
                Invoke("stopCollisionsFalse", 1.5f);
            }


            if (temp == GetDiceValue && GetDiceValue == 6)
            {
                for (int i = 0; i < ListOfSafePoints.Count; i++)
                {
                    if (thisRectTransform.localPosition == ListOfSafePoints[i].localPosition)
                    {
                        thisCollider.enabled = false;
                        break;
                    }
                    else
                        thisCollider.enabled = true;
                }
                temp = 0;
                GetDiceValue = 0;
                AbleToGo = false;
                ImWin = IndexOfPath + 1 == completePath.Count ? true : false;
                Invoke("CheckCollisionAndConfirm", .25f);//newly added
                Invoke("stopCollisionsFalse", 1.5f);
                if(!ImWin) thisParent.Invoke("EnableAllButtons", .5f);
                
            }


        }
        if (IndexOfPath + 1 == completePath.Count)
        {
            //Debug.Log(ImWin);
            ImWin = true;
            IndexOfPath = 0;
            unlocked = false;
            AbleToGo = false;
            OurTurnCompleted = false;
            thisParent.RemoveWinnerPoint(this.GetComponent<PointersScriptForComputers>());
            GetComponent<PointersScriptForComputers>().enabled = false;
        }

        //   if((GetDiceValue+IndexOfPath+1)>completePath.Count)

        //sending back to  home
        if (returningHome)
        {
            timer2 -= Time.deltaTime;

            if (timer2 <= 0 && IndexOfPath >= 0)
            {
                thisRectTransform.localPosition = completePath[IndexOfPath].gameObject.GetComponent<RectTransform>().anchoredPosition;
                IndexOfPath--;
                timer2 = .05f;
            }
            if (IndexOfPath <= 0)
            {
                thisRectTransform.localPosition = ReturnPoint;
                returningHome = false;
            }

        }
        if (Input.GetKey(KeyCode.Space))
            GoReturnHome();

    }

    void CheckCollisionAndConfirm()
    {
        if (!ImWin)
        {
            OurTurnCompleted = true;
            thisParent.MyTurnOverBool = OurTurnCompleted;
            thisParent.YesCaptured = YesCaptured;
            thisParent.ImDone = true;
            thisParent.YesSix = YesSix;
        }
        else
        {
            OurTurnCompleted = false;
            thisParent.MyTurnOverBool = OurTurnCompleted;
            thisParent.YesCaptured = YesCaptured;
            thisParent.ImDone = true;
            unlocked = false;
            thisParent.YesSix = false;
        }

    }
    void stopCollisionsFalse()
    {
        stopCollisions = false;
    }
    public void OnPointerDown(PointerEventData eventData)//this method can be replaced as the move(int diceValue)
    {
        YesSix = GetDiceValue == 6 ? true : false;
        YesCaptured = false;
        if (!unlocked && GetDiceValue == 6)
        {
            ScaleUpHighlight();
            thisParent.SetDiceValueInAllPoints(this.GetComponent<PointersScriptForComputers>());
            thisRectTransform.localPosition = completePath[0].gameObject.GetComponent<RectTransform>().anchoredPosition;
            GetDiceValue = 0;
            thisCollider.enabled = true;
            unlocked = true;
            returningHome = false;
            OurTurnCompleted = false;
            thisParent.Invoke("EnableAllButtons", .5f);

        }
        else if ((IndexOfPath + GetDiceValue) < completePath.Count && unlocked && GetDiceValue != 0)
        {
            thisParent.SetDiceValueInAllPoints(this.GetComponent<PointersScriptForComputers>());
            AbleToGo = true;
            stopCollisions = true;
            OurTurnCompleted = false;
            // if (GetDiceValue == 6 && (IndexOfPath + GetDiceValue) < completePath.Count) thisParent.Invoke("EnableAllButtons", .5f);
        }
        if (GetDiceValue == 6 && (IndexOfPath + GetDiceValue) > completePath.Count)
        {
            thisParent.Invoke("EnableAllButtons", .5f);
        }
        /* 
               if (GetDiceValue == 6)
                   if (!AbleToGo)
                   {
                       AbleToGo = true;
                       transform.position = HouseStartPoint;
                       return;
                   }*/

        thisParent.diceValue.text = "Roll";
    }
    //the above and below methods are works same but invoked by two different players
    public void move(int GetData)
    {
        GetDiceValue = GetData;
        YesSix = GetDiceValue == 6 ? true : false;
        YesCaptured = false;
        if (!unlocked && GetDiceValue == 6)
        {
            ScaleUpHighlight();
            thisParent.SetDiceValueInAllPoints(this.GetComponent<PointersScriptForComputers>());
            thisRectTransform.localPosition = completePath[0].gameObject.GetComponent<RectTransform>().anchoredPosition;
            GetDiceValue = 0;
            thisCollider.enabled = true;
            unlocked = true;
            returningHome = false;
            OurTurnCompleted = false;
            thisParent.Invoke("EnableAllButtons", .5f);

        }
        else if ((IndexOfPath + GetDiceValue) < completePath.Count && unlocked && GetDiceValue != 0)
        {
            thisParent.SetDiceValueInAllPoints(this.GetComponent<PointersScriptForComputers>());
            AbleToGo = true;
            stopCollisions = true;
            OurTurnCompleted = false;
            // if (GetDiceValue == 6 && (IndexOfPath + GetDiceValue) < completePath.Count) thisParent.Invoke("EnableAllButtons", .5f);
        }

      /*  if (GetDiceValue == 6 && (IndexOfPath + GetDiceValue) > completePath.Count)
        {
            thisParent.Invoke("EnableAllButtons", .5f);
        }
         
               if (GetDiceValue == 6)
                   if (!AbleToGo)
                   {
                       AbleToGo = true;
                       transform.position = HouseStartPoint;
                       return;
                   }*/

        thisParent.diceValue.text = "Roll";
    }

    public void ScaleUpHighlight()
    {
        animationClip.enabled = false;
        thisRectTransform.localScale = new Vector3(30, 30, 1);
    }
    public void ScaleDownToNormal()
    {
        animationClip.enabled = true;
        thisRectTransform.localScale = new Vector3(20, 20, 1);
    }
    public void GoReturnHome()
    {
        thisParent.unLockedCount = 0;
        //  thisRectTransform.anchoredPosition=ReturnPoint;
        unlocked = false;
        //   IndexOfPath = 0;
        AbleToGo = false;
        thisCollider.enabled = false;
        returningHome = true;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        RectTransform currentPosition = collision.gameObject.GetComponent<RectTransform>();

        if (!collision.gameObject.CompareTag(thisTag) && stopCollisions)
        {
            collision.gameObject.GetComponent<PointersScriptForComputers>().Invoke("GoReturnHome", .5f);
            /*YesCaptured = true;
            thisParent.MyTurnOverBool = false;
            thisParent.YesCaptured = true;
            thisParent.Invoke("EnableAllButtons", .5f);*/
            switch (thisParent.gameObject.name)
            {
                case "RedPlayer":
                    YesCaptured = true;
                    thisParent.MyTurnOverBool = false;
                    thisParent.YesCaptured = true;
                    thisParent.EnableAllButtons();
                    break;
                case "GreenPlayer":
                    YesCaptured = true;
                    thisParent.MyTurnOverBool = false;
                    thisParent.YesCaptured = true;
                    thisParent.EnableAllButtons();
                    break;
                case "YellowPlayer":
                    YesCaptured = true;
                    thisParent.MyTurnOverBool = false;
                    thisParent.YesCaptured = true;
                    thisParent.EnableAllButtons();
                    break;
                case "BluePlayer":
                    YesCaptured = true;
                    thisParent.MyTurnOverBool = false;
                    thisParent.YesCaptured = true;
                    thisParent.EnableAllButtons();
                    break;
            }
        }
        /*else
            thisParent.YesCaptured = false;*/
    }

}
