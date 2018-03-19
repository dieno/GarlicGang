using UnityEngine;
using System.Collections;

public class WaitAndDestroyScript : MonoBehaviour
{
    public float TimeToStay;
    public GameObject ObjectToDestroy;

    private float elapsed;

    // Update is called once per frame
    void Update()
    {
        elapsed += Time.deltaTime;

        if (elapsed >= TimeToStay)
        {
            if (ObjectToDestroy == null)
                Destroy(this.gameObject);
            else
            {
                Destroy(ObjectToDestroy);
                Destroy(this);
            }

        }

    }
}
