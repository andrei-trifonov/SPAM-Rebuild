using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteOnActivate : RaycastObject
{
public GameObject toDelete;
public override void Activate()
    {
    
    Destroy (toDelete);
    }
}
