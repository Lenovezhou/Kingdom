using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

/// <summary>
/// Panel_show10下的子物体将接受控制，bottompanel的子物体只确定前进多少格距离（注意命名），所以：
/// 最好不要通过名字去控制，方法的参数尽量简单，通用
/// bottompanel与Panel_show10子物体的数量应相等添加时，先在枚举togglechange中添加状态，在再UImanager的UPdate
/// 和itemschange中添加对应状态
/// 在界面调整时，要将Panel_show10子UI的中心点放在（0.5，0.5）；
/// LateUpdate中更新的距离是根据子物体的宽度前进，
/// 在这个Demo里应用了UI的适配，将Panel_show10的子物体垒在一起，初始化时根据自身宽度依次排开，锚点定在屏幕四周
/// </summary>



public enum togglechange
{
	toggle0=0,toggle1=1,toggle2=2,toggle3=3,toggle4=4,none
}

public class UImanager : MonoBehaviour,IDragHandler,IPointerDownHandler,IPointerUpHandler {
	
	//toggle与panel对应字典
//	public Dictionary<GameObject,GameObject> panel_toggle=new Dictionary<GameObject, GameObject>();
	//每个panel对应的状态
	[HideInInspector]
	public togglechange uimanagertogglechange;
	//调整间隔
	public float Panel_show10Xspeacing,Panel_show10cellYspeacing;
	//改变Panel_show10的cell宽度
	public float Panel_show10cellX;

	public GameObject bottompanel,Panel_show10;

	//每帧在X轴的偏移量
	public float deltaX;
	public bool isdrag=false;

	private Toggle[] toggles;
	private GameObject current;

	private Vector3 templerp;
	private float parentwidth;

	private GameObject BackgroundImage;
	private Vector2 normal;
	private float scaletitle=1.5f,equalsL;

	//底部菜单的高度
	private float BottomItemsHigh;
	//lerp时间
	private float lerptimer=0.15f;

	public Vector2 startpos;

	private Vector3[] destinations;

	public int nowpanle;



	 void Awake ()
		{
		Inite ();
		}
	public void Inite()
	{
		startpos =new Vector2(transform.localPosition.x,transform.position.y);

		Panel_show10 = transform.FindChild ("Panel_show0").gameObject;
		bottompanel = transform.FindChild ("Panel_bottom").gameObject;
		BackgroundImage = bottompanel.transform.FindChild ("BackImage").gameObject;

		BottomItemsHigh = bottompanel.GetComponent<RectTransform> ().rect.height;

		GridLayoutGroup bottomglg = bottompanel.GetComponent<GridLayoutGroup> ();

		//使用Gridlayout初始化cellsize的大小,不能控制锚点，在Inspectors以取消勾选
		GridLayoutGroup glg = Panel_show10.GetComponent<GridLayoutGroup> ();
		bottomglg.cellSize = new Vector2 ((Screen.width/4)+Panel_show10cellX,(Screen.height/8));
		glg.cellSize = new Vector2 (Screen.width+Panel_show10cellX,Screen.height-bottomglg.cellSize.y);
		glg.spacing = new Vector2 (Panel_show10Xspeacing,Panel_show10cellYspeacing);


		parentwidth = Panel_show10.transform.GetChild(0).GetComponent<RectTransform> ().rect.size.x;

		destinations = new Vector3[Panel_show10.transform.childCount];

		//不使用Gridlayout初始化Panel_show10子物体
		for (int i = 0; i < Panel_show10.transform.childCount; i++) 
		{
			Panel_show10.transform.GetChild (i).localPosition = new Vector3 ((i)* parentwidth,0,0);
			destinations [i] = new Vector3 ((i) * parentwidth, Panel_show10.transform.localPosition.y, 0);
		}


		//不使用Gridlayout初始化bottompanel子物体
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
		}
		equalsL = (bottompanel.GetComponent<RectTransform> ().rect.width) / (toggles.Length - 1 + scaletitle);
		//初始化字典：
		//		for (int i = 0; i <  toggles.Length; i++)
		//		{
		//			panel_toggle.Add (toggles[i].gameObject,Panel_show10.transform.GetChild(i).gameObject);
		//		}

		//初始化状态为选中第三个按钮
		uimanagertogglechange = togglechange.toggle2;
		current = toggles [0].gameObject;
		templerp = Panel_show10.transform.localPosition;
	}
	public void LerpPanel(int currentpanel)
	{
		templerp =new Vector3(-destinations [currentpanel].x,destinations[currentpanel].y,0f);
		current=toggles[currentpanel].gameObject;
		nowpanle = currentpanel;

		//修改位置及宽度
		for (int i = 0; i < toggles.Length; i++) 
		{
			//先修改大小
			if (i==currentpanel) {
				toggles [i].GetComponent<RectTransform> ().sizeDelta = Vector2.Lerp(toggles [i].GetComponent<RectTransform> ().sizeDelta
					, new Vector2 (scaletitle * equalsL,BottomItemsHigh),lerptimer);
			} else {
				toggles [i].GetComponent<RectTransform> ().sizeDelta = Vector2.Lerp(toggles [i].GetComponent<RectTransform> ().sizeDelta,
					new Vector2 ( equalsL,BottomItemsHigh),lerptimer);
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
		//BackgroundImage.transform.SetParent(bottompanel.transform);
		BackgroundImage.transform.localPosition=Vector3.Lerp(BackgroundImage.transform.localPosition,toggles[currentpanel].transform.localPosition,lerptimer);
	}

	void LateUpdate()
	{
		if (!isdrag && Panel_show10) {
			Panel_show10.transform.localPosition = Vector3.Lerp (
				Panel_show10.transform.localPosition,
				templerp, lerptimer);
		}
	}

	public void OnToggle(bool ison)
	{
//		Debug.Log (this.gameObject.name);
	}

	/// <summary>
	/// 拖动实现,eventData.delta.x每帧在X轴上的偏移量
	/// </summary>
	/// <param name="eventData">Event data.</param>

	public void OnDrag (PointerEventData eventData)
	{
		deltaX = eventData.delta.x;
		Panel_show10.transform.localPosition +=new Vector3(eventData.delta.x,0f,0f);
	}
	/// <summary>
	/// 弹起时完成剩下动作
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public void OnPointerUp (PointerEventData eventData)
	{
		isdrag = false;
		Vector3 tempdestination = startpos-eventData.position;
		if (nowpanle>=0 && nowpanle<=4) {
			if (deltaX > 10f ||(deltaX > 1f&& Vector3.Distance(startpos,eventData.position)>300f)) {
				nowpanle --;
			} 
			if (deltaX < -10f||(deltaX<-1f&& Vector3.Distance(startpos,eventData.position)>300f)) {
				nowpanle ++;
			} 
		}


		switch (nowpanle) 
		{

			case 0:
			case -1:
				uimanagertogglechange = togglechange.toggle0;
				break;
			case 1:
				uimanagertogglechange = togglechange.toggle1;
				break;
			case 2:
				uimanagertogglechange = togglechange.toggle2;
				break;
			case 3:
				uimanagertogglechange = togglechange.toggle3;
				break;
			case 4:
			case 5:
				uimanagertogglechange = togglechange.toggle4;
				break;
			default:
				break;
		}
	}


	/// <summary>
	/// 跟新每次按下时的初始位置
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public void OnPointerDown (PointerEventData eventData)
	{
		isdrag = true;
		deltaX = 0f;
		startpos = eventData.position;
	}


	void Update () 
	{
		
		switch (uimanagertogglechange) {
		case togglechange.none:
			for (int i = 0; i < toggles.Length; i++) {
				toggles [i].transform.localScale = new Vector3 (1f,1f,1f);
			}
			break;
		case togglechange.toggle0:
			LerpPanel (0);
			break;
		case togglechange.toggle1:
			LerpPanel (1);
			break;
		
		case togglechange.toggle2:
			LerpPanel (2);
			break;
		case togglechange.toggle3:
			LerpPanel (3);
			break;
		case togglechange.toggle4:
			LerpPanel (4);
			break;
		}

		//Touch控制拖动
#if UNITY_ANDRIOD
		if (Input.touchCount>=1)
		{
			if (Input.GetTouch (0).phase == TouchPhase.Began) {
				Debug.Log ("BeginTouch");
				isdrag = true;
				deltaX = 0f;
				startpos = Input.GetTouch (0).position;
			}
			if (Input.GetTouch (0).phase == TouchPhase.Moved) {
				deltaX = Input.GetTouch (0).position.x;
				Panel_show10.transform.localPosition += new Vector3 (Input.GetTouch (0).position.x, 0f, 0f);
			}
			if (Input.GetTouch (0).phase == TouchPhase.Canceled) {
				isdrag = false;
				//	Vector3 tempdestination = startpos - Input.GetTouch (0).position;
				if (nowpanle >= 0 && nowpanle <= 4) {
					if (deltaX > 10f || (deltaX > 1f && Vector3.Distance (startpos, Input.GetTouch (0).position) > 300f)) {
						nowpanle--;
					} 
					if (deltaX < -10f || (deltaX < -1f && Vector3.Distance (startpos, Input.GetTouch (0).position) > 300f)) {
						nowpanle++;
					} 
				}


				switch (nowpanle) {

				case 0:
				case -1:
					uimanagertogglechange = togglechange.toggle0;
					break;
				case 1:
					uimanagertogglechange = togglechange.toggle1;
					break;
				case 2:
					uimanagertogglechange = togglechange.toggle2;
					break;
				case 3:
					uimanagertogglechange = togglechange.toggle3;
					break;
				case 4:
				case 5:
					uimanagertogglechange = togglechange.toggle4;
					break;
				default:
					break;
				}
			}
		}
#endif
	}
}
