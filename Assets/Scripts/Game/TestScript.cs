using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TestScript : NetworkBehaviour {
    
	void Start () {
		
	}
	
	void Update () {
		
	}

    public void Test(string input) {

        RpcTestLookup(input);


    }

    [ClientRpc]
    public void RpcTestLookup(string input) {

        //test
        print(Assets.Weapon(input));
    }
}
