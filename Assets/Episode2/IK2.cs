using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IK2 : MonoBehaviour
{
float ikWeight = 1f; //ik will completly override animation
private Animator _animator;
public float footPlacementOffset;
public LayerMask groundMask;
private void Awake() {
  _animator = GetComponent < Animator > ();
}

private void OnAnimatorIK(int layerIndex) {
  Debug.Log("Trying to adjust pos");
  if (_animator) {
    /* this is used to control the blend between animation and foot Ik placement , 0 = no ik and 1 means ik completely takes over, 
    value between 0 and 1 is blend between 2 */
    _animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, ikWeight);
    _animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, ikWeight);

    _animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, ikWeight);
    _animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, ikWeight);

    Vector3 rayDir = Vector3.down;
    // for left foot
    AdjustFootIK(AvatarIKGoal.LeftFoot, rayDir);
    //for the right foot
    AdjustFootIK(AvatarIKGoal.RightFoot, rayDir);
  }
}

private void AdjustFootIK(AvatarIKGoal ikGoal, Vector3 rayDir) {
  Vector3 rayStartPos = _animator.GetIKPosition(ikGoal) + Vector3.up;
  // raycast origin starts from the foot location + offset of 1 unit in up dir   
  bool isGround = Physics.Raycast(rayStartPos, rayDir, out RaycastHit hitInfo, 2f, groundMask);
  // check for ground detection   
  if (isGround) // touching ground   
  {
    // point where the raycast hit
    Vector3 hitPos = hitInfo.point;
    // offset foot by certain value along normal direction  
    hitPos += hitInfo.normal * footPlacementOffset;
    // set the ik position for foot
    _animator.SetIKPosition(ikGoal, hitPos);
    // calculate new rotation for foot according to change in normal   
    Quaternion lookDir = Quaternion.LookRotation(transform.forward, hitInfo.normal);
    // set new foot pos 
    _animator.SetIKRotation(ikGoal, lookDir);
  }
}
}
