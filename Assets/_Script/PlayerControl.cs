using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour {
	
	public Rigidbody rg;
	public Renderer rr;
	public int Score;
	public float spe = 1.75f;
	public CreateFloors CreateFloorsScript;
	public Main MainScript;
	public GameObject FloorObj;
	public float nextcubeZ;
	public int PlusCoins;
	void Awake () 
	{
		SetAllAnother ();
	}

	void Update () 
	{

		if (!MainScript.LoseBool) {
            Camera.main.transform.position = new Vector3(transform.position.x, 3, transform.position.z - 8);
            Vector3 _v3 = transform.position;
			_v3.y = -4;
			if (transform.position.y < 0)
				MainScript.Lose ();
		}
	}

	void FixedUpdate()
	{
		if (transform.position.z < nextcubeZ && !MainScript.LoseBool) 
		{
			rg.MovePosition(Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, transform.position.y, nextcubeZ), CreateFloorsScript.BeetwenDistanceList[Score] * spe * Time.deltaTime));
            transform.rotation = new Quaternion(0, 0, FloorObj.transform.position.x * 90 * -Time.deltaTime, 1);
        }
	}

	public void SetAllAsBegin()
	{
		rg.velocity = Vector3.zero;
		Score = 0;
	}

	public void SetAllAnother()
	{
		if (transform.childCount > 0) {
			DestroyImmediate (transform.GetChild (0).gameObject);
		}
		GameObject _go = Instantiate (Resources.Load<GameObject> (PlayerPrefs.GetInt ("NowClick", 0).ToString ()));
		_go.transform.SetParent(transform);
		_go.transform.position = transform.position;
		rr = _go.GetComponent<Renderer> ();
		rr.material.color = CreateFloorsScript.StartPlayerColor;
		CreateFloorsScript.PlayerRenderer = _go.GetComponent<Renderer> ();
	}

	void OnCollisionEnter(Collision other)
	{
		if (!MainScript.LoseBool && MainScript.CanJump && other.gameObject.GetComponent<Renderer> ().material.color == rr.material.color) 
		{
			MainScript.CanJump = false;
			StartCoroutine(DestroyIfLoseBool (other.gameObject.transform.parent.gameObject, 3f));
			CreateFloorsScript.Create ();
			if(other.gameObject.name != "StartCubes")
				Score++;

			rg.AddForce (Vector3.up * 3f, ForceMode.Impulse);
			rg.velocity = Vector3.zero;
		} else if(other.gameObject.GetComponent<Renderer> ().material.color != rr.material.color)
		{
			MainScript.Lose();
		}
	}

	void OnTriggerEnter (Collider other)
	{
		if (other.tag == "BonusChangeColor") {
			StartCoroutine (DestroyIfLoseBool (other.transform.parent.gameObject, 3f));
			rr.material.color = CreateFloorsScript.BeenColor;
		} else if (other.name == "Coin") 
		{
			Destroy(other.gameObject);
            if(PlayerPrefs.GetInt("SoundVolume", 1) != 0)
            MainScript.GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("PickUp"));
			PlusCoins++;
		}
	}

	IEnumerator ToNextCube (float nextcubeZ)
	{
		while (transform.position.z < nextcubeZ && !MainScript.LoseBool) 
		{
			rg.MovePosition(Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, transform.position.y, nextcubeZ), CreateFloorsScript.BeetwenDistanceList[Score] * spe * Time.deltaTime));
			yield return new WaitForSeconds(0.01f);
		}
	}
	
	void OnCollisionExit (Collision other)
	{
        if (!MainScript.LoseBool)
        {
            MainScript.CanJump = true;
            nextcubeZ = other.transform.position.z + CreateFloorsScript.BeetwenDistanceList[Score];
            //StartCoroutine (ToNextCube(other.transform.position.z + CreateFloorsScript.BeetwenDistanceList[Score]));
        }
	}

	IEnumerator DestroyIfLoseBool(GameObject obj, float t = 3f)
	{
		yield return new WaitForSeconds(t);
		if (!MainScript.LoseBool) 
			Destroy (obj);
	}
	
}
