using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PointersScript : MonoBehaviour, IPointerDownHandler
{
    // Start is called before the first frame update

    public Vector2 ReturnPoint,HouseStartPoint,winPoint;
    public List<Transform> completePath = new List<Transform>();
    public List<RectTransform> ListOfSafePoints = new List<RectTransform>();
     int StepsCompleted, StepsPending;
    public bool AbleToGo = false,unlocked=false,stopCollisions=false,returningHome,OurTurnCompleted=false,ImWin=false,YesCaptured;
    public int GetDiceValue;
    public IndividualPlayer_Script thisParent;
    public int temp=0,IndexOfPath=0;
    RectTransform thisRectTransform;
    Animator animationClip;
    float timer = .5f,timer2=.05f;
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
            Debug.Log("Win" + completePath.Count);
        if (!AbleToGo)
        {  
            /*if(GetDiceValue==6)
            {
                //highlight this point;;
            }*/


        }
        if(AbleToGo)
        {
            timer -= Time.deltaTime;
            if(timer<=0 && temp<=(GetDiceValue+1) && (IndexOfPath+1)<completePath.Count)
            {
                thisRectTransform.localPosition = completePath[IndexOfPath+1].gameObject.GetComponent<RectTransform>().anchoredPosition;
                animationClip.Play(0);
                IndexOfPath++;
                temp++;
                timer = .25f;
                thisCollider.enabled = false;
                GameManager.Instance.PlayMovingClip();
            }
            if (temp == GetDiceValue && GetDiceValue!=6 )
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
                }
*/
                Invoke("stopCollisionsFalse", 1.5f);
            }
            if (GetDiceValue == 6 && (IndexOfPath + GetDiceValue) > completePath.Count)
            {
                thisParent.Invoke("EnableAllButtons", .5f);
            }
            if (temp == GetDiceValue && GetDiceValue == 6)
            {
                thisParent.Invoke("EnableAllButtons", .5f);
                temp = 0;
                GetDiceValue = 0;
            }

           
        } 
        if (IndexOfPath + 1 == completePath.Count) 
            {
                ImWin = true;
                IndexOfPath = 0;
                unlocked = false;
            thisParent.RemoveWinnerPoint(this.GetComponent<PointersScript>());
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
            if(IndexOfPath<=0)
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
           // Debug.Log(YesCaptured);
            OurTurnCompleted = true;
            thisParent.MyTurnOverBool = OurTurnCompleted;
            thisParent.YesCaptured = YesCaptured;
        }
        else
        {
            OurTurnCompleted = false;
            thisParent.MyTurnOverBool = OurTurnCompleted;
            thisParent.YesCaptured = YesCaptured;
        }
      
    }
    void stopCollisionsFalse()
    {
        stopCollisions = false;
    }
    public void OnPointerDown(PointerEventData eventData)//this method can be replaced as the move(int diceValue)
    {
        YesCaptured = false;
        if (!unlocked && GetDiceValue==6)
        {
            ScaleUpHighlight();
            thisParent.SetDiceValueInAllPoints(this.GetComponent<PointersScript>());
            thisRectTransform.localPosition = completePath[0].gameObject.GetComponent<RectTransform>().anchoredPosition;
            GetDiceValue = 0;
            thisCollider.enabled = true;
            unlocked = true;
            returningHome = false;
            OurTurnCompleted = false;
            thisParent.Invoke("EnableAllButtons", .5f);
            
        }
        else if((IndexOfPath + GetDiceValue) < completePath.Count && unlocked && GetDiceValue!=0)
        {
            thisParent.SetDiceValueInAllPoints(this.GetComponent<PointersScript>());
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
            collision.gameObject.GetComponent<PointersScript>().Invoke("GoReturnHome", .5f);

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
