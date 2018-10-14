using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SplashController : MonoBehaviour {

	SpriteRenderer image;

	[SerializeField]
	float waitTime = 2;

	// Use this for initialization
	IEnumerator Start () {
		image = GetComponent<SpriteRenderer>();
		yield return new WaitForSeconds(waitTime);
		var time = 0f;
		var baseColor = image.color;
		while (time < 1)
		{
			image.color = Color.Lerp(baseColor, new Color(0, 0, 0, 0), time); 
			time += Time.deltaTime;
			yield return null;
		}
		SceneManager.LoadScene("Main");
	}
}
