using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class FootIK : MonoBehaviour
{
    public Transform body;
    public TwoBoneIKConstraint leftLegRig;
    public TwoBoneIKConstraint rightLegRig;
    public Transform leftFootTarget;
    public Transform rightFootTarget;
    public Transform leftFootOrigin;
    public Transform rightFootOrigin;
    public LayerMask groundLayer;
    public AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);
    public float raycastDistance = 1.0f;
 
    public float offset;
    public float offset2;
    public float offset3;
    public Vector3 startPos;
    public Vector3 startVector;
   
    public float weight;
    public float smoothTime = 0.1f;
    public float smoothTime2 = 0.1f;
    private float currentBodyY;

    private float velocityY = 0f;

    // Добавим поля для сглаживания движения ног
    private float leftFootVelocityY = 0f;
    private float rightFootVelocityY = 0f;
    private float currentLeftFootY;
    private float currentRightFootY;
    private Vector3 leftFootVelocity = Vector3.zero;
    private Vector3 rightFootVelocity = Vector3.zero;
    private Vector3 currentLeftFootPos;
    private Vector3 currentRightFootPos;
    private void Start()
    {
        startPos = transform.parent.transform.position - startVector;
        currentBodyY = startPos.y;
        currentLeftFootY = leftFootTarget.position.y;
        currentRightFootY = rightFootTarget.position.y;
        currentLeftFootPos = leftFootTarget.position;
        currentRightFootPos = rightFootTarget.position;
    }

    void LateUpdate()
    {
        float l = AdjustFoot(false,leftFootOrigin, leftFootTarget, Color.green, leftLegRig, ref currentLeftFootY, ref leftFootVelocityY);
        float r = AdjustFoot(true ,rightFootOrigin, rightFootTarget, Color.blue, rightLegRig, ref currentRightFootY, ref rightFootVelocityY);
       
        float targetY;
        if (l != -1 && r != -1)
            targetY = ((l + r) / 2f) - offset;
        else
        {
            if ((l == -1 && r != -1) || (r == -1 && l != -1))
            {
                startPos = transform.parent.transform.position;
                targetY = startPos.y - offset3;
            }
            else
            {
                startPos = transform.parent.transform.position;
                targetY = startPos.y - offset2;
            }
        }

        currentBodyY = Mathf.SmoothDamp(currentBodyY, targetY, ref velocityY, smoothTime);
        body.transform.position = new Vector3(body.transform.position.x, currentBodyY, body.transform.position.z);
        
    }

    float AdjustFoot(bool side, Transform origin, Transform target, Color rayColor, TwoBoneIKConstraint rig, ref float currentFootY, ref float footVelocityY)
    {
        Vector3 start = origin.position + Vector3.up * 1.5f;
        Vector3 dir = Vector3.down;

        Debug.DrawRay(start, dir * raycastDistance, rayColor);
        
       
        
        
        
        if (Physics.Raycast(start, dir, out RaycastHit hit, raycastDistance, groundLayer))
        {
            rig.weight = weight;
          
            // float targetY = hit.point.y;
            // currentFootY = Mathf.SmoothDamp(currentFootY, targetY, ref footVelocityY, smoothTime);
            //
            // target.position = new Vector3(hit.point.x, currentFootY, hit.point.z);
            
            float distance =hit.point.y- transform.parent.transform.position.y;



            currentLeftFootPos = leftFootTarget.position;
            currentRightFootPos = rightFootTarget.position;



            Vector3 targetPos = hit.point + Vector3.up * curve.Evaluate(distance);

            if (side)
            {
                currentRightFootPos = Vector3.SmoothDamp(currentRightFootPos, targetPos, ref rightFootVelocity, smoothTime2);
                target.position = currentRightFootPos;
            }
            else
            {
                currentLeftFootPos = Vector3.SmoothDamp(currentLeftFootPos, targetPos, ref leftFootVelocity, smoothTime2);
                target.position = currentLeftFootPos;
            }




            
  
            return  target.position.y; // startVector.y + ;
        }
        else
        {
            rig.weight = 0;
            return -1;
        }
    }
}
