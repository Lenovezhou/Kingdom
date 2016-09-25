using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Test : MonoBehaviour,IPointerDownHandler,IDragHandler,IPointerUpHandler {

	public UImanager uimanager;
	 void Awake () {
		GetComponent<Button> ().onClick.AddListener (OnButton);
	}



	public virtual void OnDrag (PointerEventData eventData)
	{
		GetComponent<CanvasGroup> ().interactable = false;
		uimanager.deltaX = eventData.delta.x;
		uimanager.Panel_show10.transform.localPosition +=new Vector3(eventData.delta.x,0f,0f);
	}
	/// <summary>
	/// 弹起时完成剩下动作
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public virtual void OnPointerUp (PointerEventData eventData)
	{
		GetComponent<CanvasGroup> ().interactable = true;
		uimanager.isdrag = false;
		Vector3 tempdestination = uimanager.startpos-eventData.position;
		if (uimanager.nowpanle>=0 && uimanager.nowpanle<=4) {
			if (uimanager.deltaX > 10f ||(uimanager.deltaX > 1f&& Vector3.Distance(uimanager.startpos,eventData.position)>300f)) {
				uimanager.nowpanle --;
			} 
			if (uimanager.deltaX < -10f||(uimanager.deltaX<-1f&& Vector3.Distance(uimanager.startpos,eventData.position)>300f)) {
				uimanager.nowpanle ++;
			} 
		}


		switch (uimanager.nowpanle) 
		{

		case 0:
		case -1:
			uimanager.uimanagertogglechange = togglechange.toggle0;
			break;
		case 1:
			uimanager.uimanagertogglechange = togglechange.toggle1;
			break;
		case 2:
			uimanager.uimanagertogglechange = togglechange.toggle2;
			break;
		case 3:
			uimanager.uimanagertogglechange = togglechange.toggle3;
			break;
		case 4:
		case 5:
			uimanager.uimanagertogglechange = togglechange.toggle4;
			break;
		default:
			break;
		}
	}


	/// <summary>
	/// 跟新每次按下时的初始位置
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public virtual void OnPointerDown (PointerEventData eventData)
	{
		uimanager.isdrag = true;
		uimanager.deltaX = 0f;
		uimanager.startpos = eventData.position;
	}



	void OnButton()
	{
		
		Debug.Log ("Button执行了");
	
	}

	// Update is called once per frame
	void Update () {
	
	}
}
