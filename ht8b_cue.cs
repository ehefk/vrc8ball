#if !UNITY_ANDROID
#define HT8B_DEBUGGER
#else
#define HT_QUEST
#endif

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ht8b_cue : UdonSharpBehaviour
{
   [SerializeField]
   public GameObject objTarget;
   public ht8b_otherhand objTargetController;

   [SerializeField]
   public GameObject objCue;

   [SerializeField]
   public ht8b gameController;

   [SerializeField]
   public GameObject objTip;

   // Pickuip components
   private VRC_Pickup pickup_this;
   private VRC_Pickup pickup_target;


#if !HT_QUEST

   // Make desktop mode a bit easier
   public bool useDesktop = false;
   float dkRotation = -1.5708f;      // start at 90 degrees
   float dkRotSpeed = 0.0f;
   public Vector3 dkTarget = Vector3.zero;
   bool dkSnapAim = false;
   bool dkShoot = false;
   float dkShotDist;
   float dkShotSpeed = 0.0f;
   public bool dkSimLock = true;
   bool dkPickupLock = false;            // Waiting for user to stop clicking

   const float k_dkShotMax = 1.5f;         // Maximum power
   const float k_dkShotReset = 0.91f;      // Normal pos
   const float k_dkShotTrigger = 0.85f;   // Reset point

#endif

   // ( Experimental ) Allow player ownership autoswitching routine 
   public bool bAllowAutoSwitch = true;
   public int playerID = 0;
   
   Vector3 lag_objTarget;
   Vector3 lag_objBase;

   Vector3 vBase;
   Vector3 vLineNorm;
   Vector3 targetOriginalDelta;

   Vector3 vSnOff;
   float vSnDet;

   bool bArmed = false;
   bool bHolding = false;

   Vector3 reset_pos_this;
   Vector3 reset_pos_target;

   private void OnPickupUseDown()
   {
#if !HT_QUEST
      if( !useDesktop )   // VR only controls
      {
#endif
         bArmed = true;

         // copy target position in
         vBase = this.transform.position;

         // Set up line normal
         vLineNorm = (objTarget.transform.position - vBase).normalized;

         // It should now be able to impulse ball
         gameController._tr_starthit();

#if !HT_QUEST
      }
#endif
   }
   private void OnPickupUseUp()   // VR
   {

#if !HT_QUEST
      if( !useDesktop )
      {

         bArmed = false;

         gameController._tr_endhit();
      }
#else
      bArmed = false;
      gameController._tr_endhit();
#endif

   }

   private void OnPickup()
   {
#if !HT_QUEST
      if( !useDesktop )   // We dont need other hand to be availible for desktop player
      {
         objTarget.transform.localScale = Vector3.one;
      }
      else
      {
         dkPickupLock = true;
      }
#else
      objTarget.transform.localScale = Vector3.one;
#endif

      // Register the cuetip with main game
      // gameController.cuetip = objTip; 

      // Not sure if this is necessary to do both since we pickup this one,
      // but just to be safe
      Networking.SetOwner( Networking.LocalPlayer, this.gameObject );
      Networking.SetOwner( Networking.LocalPlayer, objTarget );
      bHolding = true;
      objTargetController.bOtherHold = true;
      objTarget.GetComponent<SphereCollider>().enabled = true;
   }

#if !HT_QUEST
   // Shot registered
   public void _dk_endhit()
   {
      if( useDesktop )
      {
         dkShoot = false;
         dkShotDist = k_dkShotReset;
         dkShotSpeed = 0.0f;
         dkSimLock = true;
      }
   }

   // Localize target coordinate memory
   public void _dk_cpytarget()
   {
      if( useDesktop )
      {
         dkTarget = gameController.dkTargetPos;
      }
   }

   public void _dk_unlock()
   {
      if( useDesktop )
      {
         dkSimLock = false;
      }
   }

#endif

   private void OnDrop()
   {
      objTarget.transform.localScale = Vector3.zero;
      bHolding = false;
      objTargetController.bOtherHold = false;
      objTarget.GetComponent<SphereCollider>().enabled = false;
   }

   private void Start()
   {
      // Match lerped positions at start
      lag_objBase = this.transform.position;
      lag_objTarget = objTarget.transform.position;

      #if !HT_QUEST

      dkShotDist = k_dkShotReset;

      #endif

      targetOriginalDelta = this.transform.InverseTransformPoint( objTarget.transform.position );
      OnDrop();

      pickup_this = (VRC_Pickup)this.gameObject.GetComponent(typeof(VRC_Pickup));
      pickup_target = (VRC_Pickup)objTarget.GetComponent(typeof(VRC_Pickup));

      reset_pos_target = objTarget.transform.position;
      reset_pos_this = this.transform.position;
   }

   // Set if local player can hold onto cue grips or not
   public void _access_allow()
   {
      this.GetComponent<SphereCollider>().enabled = true;
      objTarget.GetComponent<SphereCollider>().enabled = true;
   }

   public void _access_deny()
   {
      // Put back on the table
      objTarget.transform.position = reset_pos_target;
      this.transform.position = reset_pos_this;

      this.GetComponent<SphereCollider>().enabled = false;
      objTarget.GetComponent<SphereCollider>().enabled = false;

      // Force user to drop it
      pickup_this.Drop();
      pickup_target.Drop();
   }

   void Update()
   {
      // Clamp controllers to play boundaries while we have hold of them
      if( bHolding )
      {
         Vector3 temp = this.transform.localPosition;
         temp.x = Mathf.Clamp( temp.x, -4.0f, 4.0f );
         temp.y = Mathf.Clamp( temp.y, -0.8f, 1.5f );
         temp.z = Mathf.Clamp( temp.z, -3.25f, 3.25f );
         this.transform.localPosition = temp;
         temp = objTarget.transform.localPosition;
         temp.x = Mathf.Clamp( temp.x, -4.0f, 4.0f );
         temp.y = Mathf.Clamp( temp.y, -0.8f, 1.5f );
         temp.z = Mathf.Clamp( temp.z, -3.25f, 3.25f );
         objTarget.transform.localPosition = temp;
      }

      lag_objBase = Vector3.Lerp( lag_objBase, this.transform.position, Time.deltaTime * 16.0f );
      lag_objTarget = Vector3.Lerp( lag_objTarget, objTarget.transform.position, Time.deltaTime * 16.0f );

      if( bArmed )
      {
         vSnOff = lag_objBase - vBase;
         vSnDet = Vector3.Dot( vSnOff, vLineNorm );
         objCue.transform.position = vBase + vLineNorm * vSnDet;
      }
      else
      {
         // put cue at base position
         objCue.transform.position = lag_objBase;
         objCue.transform.LookAt( lag_objTarget );

         #if !HT_QUEST

         // Special desktop alignment
         if( bHolding && useDesktop )
         {
            // Stop input registering if we've just picked the thing up
            if( dkPickupLock )
            {
               if(!Input.GetButton("Fire1"))
                  dkPickupLock = false;
            }
            else
            {
               // Place cue, (only within 1 meters)
               if( (this.transform.position-dkTarget).sqrMagnitude < 1.0f && !dkSimLock )
               {
                  bool in_left = Input.GetKey( KeyCode.Q );
                  bool in_right = Input.GetKey( KeyCode.E );

                  float accel = 0.8f * Time.deltaTime;

                  // Use accelleration when rotating 
                  if( !dkShoot )
                  {
                     if( in_left )
                     {
                        dkRotSpeed -= accel;
                     }

                     if( in_right )
                     {
                        dkRotSpeed += accel;
                     }

                     if( !( in_left || in_right ) )
                     {
                        dkRotSpeed = 0.0f;
                     }

                     // Position/rotation update pre-delayed by 1 frame to allow ht8b.cs to pick up the shot.
                     dkRotation += dkRotSpeed * Time.deltaTime * 2.0f;
                  }

                  objCue.transform.position = new Vector3( dkTarget.x + Mathf.Sin( dkRotation ) * dkShotDist, 0.0f, dkTarget.z + Mathf.Cos( dkRotation ) * dkShotDist );
                  objCue.transform.LookAt( dkTarget );

                  if( Input.GetButton("Fire1") )
                  {
                     // Rising trigger start hitting
                     if( !dkShoot )
                     {
                        gameController._tr_starthit();
                     }

                     dkShoot = true;
                     dkShotDist += 0.2f * Time.deltaTime;

                     dkShotDist = Mathf.Clamp( dkShotDist, 0, k_dkShotMax );
                  }
                  else
                  {
                     if( dkShoot )   // Shooting sequence ( accelerate towords reset point )
                     {
                        dkShotSpeed += Time.deltaTime * 0.12f;
                        dkShotDist -= dkShotSpeed;

                        // Should not be hit, but it might
                        if( dkShotDist < 0.0f )
                        {
                           _dk_endhit();
                           gameController._tr_endhit();
                        }
                     }
                  }
               }
            }
         }

         #endif
      }
   }
}
