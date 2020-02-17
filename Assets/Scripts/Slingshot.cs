
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slingshot : MonoBehaviour
{
    static public Slingshot S;
//class level variable
[Header("Set in Inspector")]
public GameObject prefabProjectile;
public float velocityMult=8f;

[Header("Set dynamically")]
public GameObject launchPoint;

public Vector3 launchPos;
public GameObject projectile;
public bool aimingMode;
private Rigidbody projectileRigidbody;

void Awake()
{
    // Set the Slingshot singleton S
    S = this;
     Transform launchPointTrans = transform.Find("LaunchPoint") ;
    launchPoint=launchPointTrans.gameObject;
    launchPoint.SetActive(false);
    launchPos=launchPointTrans.position;
}//end of awake
    
    void Update(){
        // if sling shot is not in aimingMode,dont run the code
        if(!aimingMode)return; //bad code,bad
        //Get the current mouse position in 2D screen coordinates
        Vector3 mousePos2D=Input.mousePosition;
        //Convert the mouse position to 3D world coordinates
        mousePos2D.z=-Camera.main.transform.position.z;
        Vector3 mousePos3D=Camera.main.ScreenToWorldPoint(mousePos2D);
        //Find the delta from the launchPos to the mousePos3D
        Vector3 mouseDelta=mousePos3D-launchPos;
        //Limit mouseDelta to the radius of the SLingshot SphereCollider
        float maxMagnitude=this.GetComponent<SphereCollider>().radius;
        if (mouseDelta.magnitude>maxMagnitude){
            mouseDelta.Normalize();
            mouseDelta*=maxMagnitude;
        }
        //Move the projectile to this new position
        Vector3 projPos=launchPos+mouseDelta;
        projectile.transform.position=projPos;

        if (Input.GetMouseButtonUp(0)){
            //the mouse has been released
            aimingMode=false;
            projectile.GetComponent<Rigidbody>().isKinematic=false;
            projectile.GetComponent<Rigidbody>().velocity=-mouseDelta*velocityMult;
            FollowCam.S.poi = projectile;
            projectile=null;
            MissionDemolition.ShotFired();
        }
    }
    
    void OnMouseDown() {
        //the player has pressed the mouse button while over slingshot
        aimingMode=true;
        //instantiating a projectile
        projectile=Instantiate(prefabProjectile) as GameObject;
        //start it at the launchPoint
        projectile.transform.position=launchPos;
        //set it to isKinematic for now
        //projectile.GetComponent<Rigidbody>().isKinematic = true ;
        projectileRigidbody=projectile.GetComponent<Rigidbody>();
        projectileRigidbody.isKinematic=true;
    }
    // Start is called before the first frame update
    void OnMouseEnter()
    {
       // print("Slinshot:OnMouseEnter()");     
        launchPoint.SetActive(true);   
    }

    // Update is called once per frame
    void OnMouseExit()
    {
        //print("Slingshot:OnMouseExit()");
        launchPoint.SetActive(false);
    }
}