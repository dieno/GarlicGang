﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public float Damage = 1.0f;
    public GameObject HitEffect;

    public void Die()
    {
        if (HitEffect != null)
            Instantiate(HitEffect, transform.position, Quaternion.identity, transform.root);

        Destroy(gameObject);
    }
}
