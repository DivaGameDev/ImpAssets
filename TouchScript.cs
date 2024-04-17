using UnityEngine;

public class TouchScript : MonoBehaviour
{
    void Update()
    {
      
      if(Input.touchCount>0)
        {
            Touch tch = Input.GetTouch(0);
            Ray _ray = Camera.main.ScreenPointToRay(tch.position);
            RaycastHit _rayCast;
            if(Physics.Raycast(_ray, out _rayCast))
            {
                if (_rayCast.transform.gameObject.tag == "CornSeed")
                {
                    _rayCast.transform.GetComponent<Rigidbody>().useGravity = true;
                }
            }
        }
    }
}
//this script is used to select an objects with the help of raycast and touch input