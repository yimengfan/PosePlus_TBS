using System.Collections;
using System.Collections.Generic;
using BDFramework.Http;
using UnityEngine;

public class TestForGod : MonoBehaviour {

	struct MyStruct
	{
		public int a;
	}
	// Use this for initialization
	void Start ()
	{

	}


	void test1()
	{
		Debug.Log("逆波兰式计算---");
		string formula1 = "1+2*3+5/2";
		string formula2 = "1+2*3+5%2";
		string formula3 = "1+2*3-5%2";
		Debug.Log(formula1 + " = " + MyMathf.RPN.Calucate(formula1));
		Debug.Log(formula2 + " = " + MyMathf.RPN.Calucate(formula2));
		Debug.Log(formula3 + " = " + MyMathf.RPN.Calucate(formula3));
	}

	void test2()
	{
//		var layer = HttpMgr.I.GetLayer(0);
//		var task = new PostTask("http://172.168.1.37:9999/gm.GmLoginHandler.WebJumpLogin" , () => { });
	}
}
