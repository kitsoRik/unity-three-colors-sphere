using UnityEngine;
using System.Collections;

public class FloorDo : MonoBehaviour {

	void OnTriggerEnter(Collider other)
	{
		switch (int.Parse (transform.parent.name)) 
		{
		//case 0: StartCoroutine(JustRotateX(transform.parent.gameObject)); break;
		default: break;
		}
	}

	IEnumerator JustRotateX(GameObject _go)
	{
		Debug.Log ("START");
		int timer = 0;
		while (timer < 10) 
		{
			timer++;
			_go.transform.Rotate(Vector3.back * 180 * Time.deltaTime);
			yield return new WaitForSeconds(0.1f);
		}
		_go.transform.rotation = new Quaternion (0, 0, 180, 1);
	}


}
