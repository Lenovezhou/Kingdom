﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Panel_show10下的子物体将接受控制，bottompanel的子物体只确定前进多少格距离（注意命名），所以：
/// bottompanel与Panel_show10子物体的数量应相等添加时，先在枚举togglechange中添加状态，在再UImanager的UPdate
/// 和itemschange中添加对应状态
/// 在界面调整时，要将Panel_show10子UI的中心点放在（0.5，0.5）；
/// LateUpdate中更新的距离是根据子物体的宽度前进，
/// 在这个Demo里应用了UI的适配，将Panel_show10的子物体垒在一起，初始化时根据自身宽度依次排开，锚点定在屏幕四周
/// </summary>



public enum togglechange
{
	toggle0,toggle1,toggle2,toggle3,toggle4,none
}

public class UImanager : MonoBehaviour {
	//toggle与panel对应字典
	public Dictionary<GameObject,GameObject> panel_toggle=new Dictionary<GameObject, GameObject>();
	//每个panel对应的状态
	public togglechange uimanagertogglechange=togglechange.none;
	public string togglename;
	//调整间隔
	public float Panel_show10Xspeacing,Panel_show10cellYspeacing;
	//改变Panel_show10的cell宽度
	public float Panel_show10cellX;

	private GameObject bottompanel,Panel_show10;



	private Toggle[] toggles;
	private GameObject current;

	private Vector3 templerp;
	private float parentwidth;

	private GameObject BackgroundImage;
	private Vector2 morethan,normal;
	private float scaletitle=1.5f,equalsL;


	void Start ()
	{
		
		Panel_show10 = transform.FindChild ("Panel_show10").gameObject;
		bottompanel = transform.FindChild ("Panel_bottom").gameObject;
		BackgroundImage = bottompanel.transform.FindChild ("BackImage").gameObject;

		GridLayoutGroup bottomglg = bottompanel.GetComponent<GridLayoutGroup> ();

		//使用Gridlayout初始化cellsize的大小,不能控制锚点，在Inspectors以取消勾选
		GridLayoutGroup glg = Panel_show10.GetComponent<GridLayoutGroup> ();
		bottomglg.cellSize = new Vector2 ((Screen.width/4)+Panel_show10cellX,(Screen.height/8));
		glg.cellSize = new Vector2 (Screen.width+Panel_show10cellX,Screen.height-bottomglg.cellSize.y);
		glg.spacing = new Vector2 (Panel_show10Xspeacing,Panel_show10cellYspeacing);


		parentwidth = Panel_show10.transform.GetChild(0).GetComponent<RectTransform> ().rect.size.x;



//		morethan =new Vector2( bottompanel.transform.GetChild (0).GetComponent<RectTransform> ().rect.size.x+50f,
//			bottompanel.transform.GetChild (0).GetComponent<RectTransform> ().rect.size.y);
//		normal =new Vector2((bottompanel.GetComponent<RectTransform>().rect.width - morethan.x) / 4,
//			bottompanel.GetComponent<RectTransform>().rect.height);
	
		//不使用Gridlayout初始化Panel_show10子物体
		for (int i = 0; i < Panel_show10.transform.childCount; i++) 
		{
			//Panel_show10.的rect的宽度与GetChild (i)的宽度不同时，只能使用自己的宽度初始化
			//i*Panel_show10.transform.GetChild (i).GetComponent<RectTransform>().rect.width；也可以

			Panel_show10.transform.GetChild (i).localPosition = new Vector3 ((i)* parentwidth,0,0);
				
		}


		//不使用Gridlayout初始化bottompanel子物体
		equalsL = (bottompanel.GetComponent<RectTransform> ().rect.width) / (4 + scaletitle);
		toggles = new Toggle[bottompanel.transform.childCount-1];
		for (int i = 0; i < bottompanel.transform.childCount; i++)
		{
			//要添加最大按钮下的背景移动效果，所以改变bottompanel的结构，限制Image添加进去，切其ISblackCast要勾选掉
			if ( bottompanel.transform.GetChild (i).gameObject.GetComponentInChildren<Toggle> ())
			{
				toggles [i] = bottompanel.transform.GetChild (i).gameObject.GetComponentInChildren<Toggle> ();
				itemschange change=toggles[i].gameObject.AddComponent<itemschange> ();
				change.uimanager = this;
				toggles[i].onValueChanged.AddListener (OnToggle);
			}





//			toggles [i].transform.localPosition += new Vector3 (i*equalsL,0f,0f);
//			toggles [i].GetComponent<RectTransform> ().sizeDelta = new Vector2 (equalsL,bottompanel.GetComponent<RectTransform>().rect.height);

		}

		//初始化状态为选中第三个按钮
		uimanagertogglechange = togglechange.toggle2;
		current = toggles [0].gameObject;
		Debug.Log ("          "+current.name);
		//Panel_show10.transform.localPosition = Vector3.zero;
		templerp = Panel_show10.transform.localPosition;
	}

	public void LerpPanel(GameObject currentpanel)
	{
		
		if ((int.Parse(currentpanel.name.Substring(6,1))-
			(int.Parse(current.name.Substring(6,1))) == 0)) {
			return;
		}
		int a = int.Parse (currentpanel.name.Substring (6, 1));
		int b = int.Parse(current.name.Substring(6,1));
		templerp= Panel_show10.transform.localPosition - 
			(new Vector3((a-b)*(parentwidth), 
			0,0));
		current=currentpanel;
	}

	void LateUpdate()
	{
		Panel_show10.transform.localPosition = Vector3.Lerp (
			Panel_show10.transform.localPosition,
			templerp, 0.25f
		);
		//		Debug.Log (Panel_show10.transform.localPosition +"        "+templerp);
			

	}

	public void Bottomitems(GameObject presed,Vector2 morethan)
	{

		//修改位置及宽度
		for (int i = 0; i < toggles.Length; i++) 
		{
			//先修改大小
			if (presed.name.Equals (toggles [i].name)) {
				toggles [i].GetComponent<RectTransform> ().sizeDelta = new Vector2 (scaletitle * equalsL,
					bottompanel.GetComponent<RectTransform> ().rect.height);
			} else {
				toggles [i].GetComponent<RectTransform> ().sizeDelta = new Vector2 ( equalsL,
					bottompanel.GetComponent<RectTransform> ().rect.height);
			}
			//根据大小修改位置
			if (i != 0) {
				toggles [i].transform.localPosition = new Vector3 (toggles [i-1].transform.localPosition.x +
					(toggles [i-1].GetComponent<RectTransform> ().sizeDelta.x), 0f, 0f);
			} else {
				toggles [i].transform.localPosition = new Vector3 (0f, 0f, 0f);
			}
		}
		//修改bottompanel的items背景位置
		BackgroundImage.transform.localPosition=Vector3.Lerp(BackgroundImage.transform.localPosition,presed.transform.localPosition,0.25f);

	}


	public void OnToggle(bool ison)
	{
//		Debug.Log (this.gameObject.name);
	}



	// Update is called once per frame
	void Update () 
	{
		
	//	Debug.Log ("first" + uimanagertogglechange.ToString ());
		switch (uimanagertogglechange) {
		case togglechange.none:
			for (int i = 0; i < toggles.Length; i++) {
				toggles [i].transform.localScale = new Vector3 (1f,1f,1f);
			}
			break;
		case togglechange.toggle0:
//			toggles [0].gameObject.transform.localScale = new Vector3 (1.1f, 1.1f, 1.1f);
//			for (int i = 1; i < toggles.Length; i++) {
//				toggles [i].gameObject.transform.localScale = new Vector3 (1f, 1f, 1f);
//			}
			Bottomitems (toggles[0].gameObject,morethan);
			LerpPanel (toggles[0].gameObject);
			break;
		case togglechange.toggle1:
//			toggles [1].gameObject.transform.localScale = new Vector3 (1.1f,1.1f,1.1f);
//			for (int i = 0; i < toggles.Length; i++) 
//			{
//				if (i != 1) {
//					toggles [i].gameObject.transform.localScale = new Vector3 (1f,1f,1f);
//				}
//			}
			Bottomitems (toggles[1].gameObject,morethan);
			LerpPanel (toggles[1].gameObject);
			break;
		
		case togglechange.toggle2:
//			toggles [2].gameObject.transform.localScale = new Vector3 (1.1f, 1.1f, 1.1f);
//			for (int i = 0; i < toggles.Length; i++) {
//				if (i != 2) 
//				{
//					toggles [i].gameObject.GetComponent<RectTransform> ().rect.size = new Vector2 (
//					
//					);
//					//toggles [i].gameObject.transform.localScale = new Vector3 (1f, 1f, 1f);
//			
//				}
//			}
			Bottomitems (toggles[2].gameObject,morethan);
			LerpPanel (toggles[2].gameObject);
			break;
		case togglechange.toggle3:
//			toggles [3].gameObject.transform.localScale = new Vector3 (1.1f,1.1f,1.1f);
//			for (int i = 0; i < toggles.Length; i++) 
//			{
//				if (i != 3)
//				{
//					toggles [i].gameObject.transform.localScale = new Vector3 (1f,1f,1f);
//				}
//
//			}
			Bottomitems (toggles[3].gameObject,morethan);
			LerpPanel (toggles[3].gameObject);
			break;
		case togglechange.toggle4:
//			toggles [4].gameObject.transform.localScale = new Vector3 (1.1f,1.1f,1.1f);
//			for (int i = 0; i < toggles.Length; i++) 
//			{
//				if (i != 4)
//				{
//					toggles [i].gameObject.transform.localScale = new Vector3 (1f,1f,1f);
//				}
//
//			}
			Bottomitems (toggles[4].gameObject,morethan);
			LerpPanel (toggles[4].gameObject);
			break;
		}
	}
}