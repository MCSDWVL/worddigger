using UnityEngine;

public class SceneDataPasser : MonoBehaviour
{
	public static int LastScore = 0;
	public static string PlayerName = "";
	private void Awake()
	{
		DontDestroyOnLoad(this.gameObject);
	}
}
