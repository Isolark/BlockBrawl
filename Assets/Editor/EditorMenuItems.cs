using UnityEngine;
using UnityEditor;

public class MenuItems : MonoBehaviour {
	[MenuItem("Tools/Data/Clear PlayerPrefs", false, 2)]
	private static void ClearPlayerPrefs(){
		PlayerPrefs.DeleteAll();
	}
}