﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildCollider : MonoBehaviour
{
    private bool touchingEnemy;

    // When the Collider has stopped touching the trigger.
    void OnTriggerExit2D(Collider2D col)
    {
        if (touchingEnemy)
            return;
        else  if (col.gameObject.tag == "enemy")
        {
            touchingEnemy = true;
            this.GetComponent<Collider2D>().attachedRigidbody.SendMessage("KilledEnemy");
        }
           
    }
}
