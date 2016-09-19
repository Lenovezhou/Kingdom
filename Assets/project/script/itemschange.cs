using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class itemschange : MonoBehaviour,IPointerDownHandler {

	public UImanager uimanager;
	public togglechange tchange=togglechange.none;

	public void OnPointerDown (PointerEventData eventData)
	{
		OnStart ();
	}





	void  OnStart () 
	{
		
		switch (this.gameObject.name) {
		case "toggle0":
			uimanager.uimanagertogglechange = togglechange.toggle0;
			break;
		case "toggle1":
			uimanager.uimanagertogglechange = togglechange.toggle1;
			break;
		case "toggle2":
			uimanager.uimanagertogglechange = togglechange.toggle2;
			break;
		case "toggle3":
			uimanager.uimanagertogglechange = togglechange.toggle3;
			break;
		case "toggle4":
			uimanager.uimanagertogglechange = togglechange.toggle4;
			break;
		default:
			break;
		}
	}
	

	void Update ()
	{
	
	}
}
