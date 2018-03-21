using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaneCrashSceneController : MonoBehaviour
{
    public string NextScene = "FirstGameScene";

    public float ChanceOfDyingHorriblyInAPlaneCrash = 0.5f;

    public float PlaneMoveRate = 1.0f;
    public float PlaneDecisionDistance = 5.0f;
    public float PlaneEndDistance = 10.0f;
    public float PlaneCrashedHeight = -20.0f;

    public Transform PlaneTransform;
    public GameObject ContinueDialog;

    public GameObject PlaneCrashEffectPrefab;

    private float DistanceMoved;
    private bool DecisionMade;
    private bool Crashing;
    private bool DidEffect;

	void Start ()
    {
		
	}
	

	void Update ()
    {
		if(PlaneTransform != null)
        {
            if(!Crashing)
            {
                float moveDist = PlaneMoveRate * Time.deltaTime;
                PlaneTransform.Translate(moveDist * Vector3.right);
                DistanceMoved += moveDist;

                if (DistanceMoved >= PlaneDecisionDistance && !DecisionMade)
                {
                    Crashing = Random.Range(0, 1.0f) < ChanceOfDyingHorriblyInAPlaneCrash;
                    if(Crashing)
                    {
                        PlaneTransform.GetComponent<Rigidbody>().isKinematic = false;
                        PlaneTransform.GetComponent<Rigidbody>().velocity = new Vector3(PlaneMoveRate, -2.0f, 0);
                        PlaneTransform.Find("Effect").gameObject.SetActive(true);
                        
                    }

                    DecisionMade = true;
                }

                if(DistanceMoved >= PlaneEndDistance)
                {
                    GameState.Instance.CurrentScene = NextScene;
                    SceneManager.LoadScene(NextScene);
                }
            }
            else
            {
                //TODO check crashy plane
                if(PlaneTransform.position.y <= PlaneCrashedHeight && !DidEffect)
                {
                    if (PlaneCrashEffectPrefab != null)
                        Instantiate<GameObject>(PlaneCrashEffectPrefab, PlaneTransform.position, Quaternion.identity, transform);

                    ContinueDialog.SetActive(true);

                    DidEffect = true;
                }
            }

            
            
            
        }
	}

    public void ContinueDead()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

}
