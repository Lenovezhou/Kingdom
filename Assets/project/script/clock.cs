using UnityEngine;
using System.Collections;

public class clock : MonoBehaviour {



		public Transform shi;  
		public Transform fen;  
		public Transform miao;  


		// Use this for initialization  
		void Start () {  
		shi = transform.FindChild ("shi");
		fen = transform.FindChild ("fen");
		miao = transform.FindChild ("miao");
		}  

		// Update is called once per frame  
		void Update () {  
//			Debug.Log("时"+System.DateTime.Now.Hour);  
//			Debug.Log("分"+System.DateTime.Now.Minute);  
//			Debug.Log("秒"+System.DateTime.Now.Second);  

			//秒钟  
			float miaonum=System.DateTime.Now.Second*6f;  
			miao.eulerAngles = new Vector3(0,0, -miaonum);  

			//分钟  
			float fennum = System.DateTime.Now.Minute * 6f;  
			fen.eulerAngles = new Vector3(0, 0, -fennum);  

			//小时  
			float shinum = System.DateTime.Now.Hour * 30f;  
			shi.eulerAngles = new Vector3(0,0, -shinum);  
	}
}
