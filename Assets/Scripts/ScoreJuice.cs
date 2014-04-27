using UnityEngine;
using System.Collections;

public class ScoreJuice : MonoBehaviour 
{

	public GUIStyle Style;
	public int Score;
	public float DriftUpSpeed = 1f;
	public float FadeSpeed = 1f;

	


	// Update is called once per frame
	public Vector3 Pos { get; set; }
	void Update () 
	{
		_yMove -= DriftUpSpeed;
	}

	private float _yMove = 0;
	void OnGUI()
	{
		Style.normal.textColor = new Color(Style.normal.textColor.r, Style.normal.textColor.g, Style.normal.textColor.b, Style.normal.textColor.a - Time.deltaTime * FadeSpeed);
		if (Style.normal.textColor.a <= 0)
			GameObject.Destroy(this.gameObject);


		GUI.Label(new Rect(Pos.x - Screen.width/2, Pos.y + _yMove - Screen.height/2, Screen.width, Screen.height), "+" + Score, Style);
	}
}
