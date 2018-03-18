using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DamageUtil
{
    public static float CalculateDamage(float Damage, float Pierce, float Threshold, float Resistance)
    {
        float d1 = Damage * ((100f - Mathf.Min(Resistance, 99f)) / 100f);
        float dt = Mathf.Max(0, Threshold - Pierce);
        float d2 = Mathf.Max(d1 - dt, Damage * 0.1f);
        return d2;
    }

}
