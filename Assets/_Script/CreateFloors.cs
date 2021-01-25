using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CreateFloors : MonoBehaviour {

	public Renderer PlayerRenderer;
	public GameObject Cube, BonusCube;
	public GameObject FloorObj;
	private float distance;
	public List<Color> ColorCubeList = new List<Color>();
	public List<Color> NowColorCubeList = new List<Color>();
	public List<float> BeetwenDistanceList = new List<float> ();
	public Color StartPlayerColor;
	public PlayerControl PlayerControlScript;
	public GameObject PlayerObject;
	public float BeetwenDistance = 1.4f;
	public int CountCreated;
	private GameObject deltaXforCreate;
	private int LastNumberPlayerColor;
	public Color BeenColor;
    private int SetCoin, CountSetCoin, NextChangeColor;
	
	void Start () {
		GetSetCoin ();
		SetColorCubeList ();
        NextChangeColor = Random.Range(40, 60);
		StartPlayerColor = ColorCubeList [UnityEngine.Random.Range (0, 3)];
		PlayerRenderer.material.color = StartPlayerColor;
		BeenColor = PlayerRenderer.material.color;
	}

	public void SetAllAsBegin()
	{
		distance = 0;
		BeetwenDistance = 1.4f;
		BeetwenDistanceList.Clear ();
		CountCreated = 0;
		ColorCubeList.Clear ();
		SetColorCubeList ();
		StartPlayerColor = ColorCubeList [UnityEngine.Random.Range (0, 3)];
		PlayerRenderer.material.color = StartPlayerColor;
		BeenColor = PlayerRenderer.material.color;
        int _childcount = FloorObj.transform.childCount;
        for (int i = _childcount-1; i >= 0; i--)
            Destroy(FloorObj.transform.GetChild(i).gameObject);
        FloorObj.transform.position = Vector3.zero;
        PlayerControlScript.nextcubeZ = 0;
	}

	void SetColorCubeList()
	{
		ColorCubeList.Add(new Color(UnityEngine.Random.Range(0,1f), UnityEngine.Random.Range(0,1f), UnityEngine.Random.Range(0,1f)));
		Color cl = Color.clear;
			do cl = new Color (UnityEngine.Random.Range (0, 1f), UnityEngine.Random.Range (0, 1f), UnityEngine.Random.Range (0, 1f));
			while(Mathf.Sqrt (Mathf.Pow ((cl.r - ColorCubeList[0].r), 2) + Mathf.Pow ((cl.g - ColorCubeList[0].g), 2) + Mathf.Pow ((cl.b - ColorCubeList[0].b), 2)) < 0.5f);
		ColorCubeList.Add(cl);
		do cl = new Color (UnityEngine.Random.Range (0, 1f), UnityEngine.Random.Range (0, 1f), UnityEngine.Random.Range (0, 1f));
		while(Mathf.Sqrt (Mathf.Pow ((cl.r - ColorCubeList[0].r), 2) + Mathf.Pow ((cl.g - ColorCubeList[0].g), 2) + Mathf.Pow ((cl.b - ColorCubeList[0].b), 2)) < 0.5f && Mathf.Sqrt (Mathf.Pow ((cl.r - ColorCubeList[1].r), 2) + Mathf.Pow ((cl.g - ColorCubeList[1].g), 2) + Mathf.Pow ((cl.b - ColorCubeList[1].b), 2)) < 0.7f);

		ColorCubeList.Add(cl);
	}

	void SetNowColorCubeList()
	{
		List<int> tmpintList = new List<int> ();
		int rand;
		tmpintList.Add (0);
		tmpintList.Add (1);
		tmpintList.Add (2);
		NowColorCubeList.Clear ();
		for (int i = 0; i < 3; i++) 
		{
			rand = UnityEngine.Random.Range(0,tmpintList.Count);
			NowColorCubeList.Add (ColorCubeList [tmpintList[rand]]);
			tmpintList.Remove(tmpintList[rand]);
		}
		if (NowColorCubeList [LastNumberPlayerColor] == BeenColor) 
		{
			Color cl1 = NowColorCubeList[LastNumberPlayerColor];
			Color cl2 = NowColorCubeList[LastNumberPlayerColor == 1 ? 0 : 1];
			NowColorCubeList[LastNumberPlayerColor] = cl2;
			NowColorCubeList[LastNumberPlayerColor == 1 ? 0 : 1] = cl1;
		}

		for (int i = 0; i < 3; i++) 
		{
			if(NowColorCubeList[i] == BeenColor)
			{
				LastNumberPlayerColor = i;
			}
		}

	}

	public void StartCreate()
	{
			float tmpd = -2.8f;
			GameObject _gopar = new GameObject ();
			
			for (int i = 0; i < 3; i++) {
				GameObject _go = Instantiate (Cube);
				_go.transform.position = new Vector3 (tmpd += 1.4f,  0, distance);
				_go.transform.SetParent (_gopar.transform);
				_go.transform.GetComponent<Renderer>().material.color = StartPlayerColor;
				_go.name = "StartCubes";
			}
			_gopar.transform.SetParent (FloorObj.transform);
			_gopar.transform.position = FloorObj.transform.position;
			distance += BeetwenDistance;
			deltaXforCreate = _gopar;
			CountCreated++;
	}

	public void ResetColor()
	{
		Color[] cl = ColorCubeList.ToArray (); 
		Component[] rr = FloorObj.GetComponentsInChildren(typeof(Renderer)); 
		ColorCubeList.Clear();
		SetColorCubeList(); 
		foreach (Renderer renderer in rr) 
		{
			for(int j = 0; j < 3; j++)
			{
				if(renderer.material.color == cl[j]) 
				{
					renderer.material.color = ColorCubeList[j]; 
					break; 
				}
			}
		}

		for(int j = 0; j < 3; j++)
		{
			if(PlayerRenderer.material.color == cl[j]) 
			{
				PlayerRenderer.material.color = ColorCubeList[j]; 
				break; 
			}
		}
		BeenColor = PlayerRenderer.material.color;
	}

	public void Create()
	{
		SetNowColorCubeList ();
		float tmpd = -2.8f;
		GameObject _gopar = new GameObject();
		_gopar.name = CountCreated.ToString ();
		Color ChangeColor = Color.clear;
		if (CountCreated % NextChangeColor == 0) 
		{
			int randi = 0;
			do randi = UnityEngine.Random.Range(0, NowColorCubeList.Count);
			while(NowColorCubeList[randi] == PlayerRenderer.material.color);
			ChangeColor = NowColorCubeList[randi];
		}
		for (int i = 0; i < 3; i++) {
			GameObject _go = Instantiate (Cube);
			_go.transform.position = new Vector3 (tmpd += 1.4f,  0, distance);
			_go.transform.SetParent (_gopar.transform);
			if (CountCreated % NextChangeColor != 0)
			{
				_go.transform.GetComponent<Renderer>().material.color = NowColorCubeList[i];
				if(CountCreated >= SetCoin && CountSetCoin > 0 && NowColorCubeList[i] == BeenColor)
				{
					GameObject _coin = Instantiate(Resources.Load<GameObject>("Coin"));
					_coin.transform.parent = _go.transform;
					Vector3 _v3_go = _go.transform.position;
					_v3_go.y = 0.6f;
					_coin.transform.position = _v3_go;
					_coin.name = "Coin";
					CountSetCoin--;
				}else if(CountCreated >= SetCoin && CountSetCoin == 0)
				{
					GetSetCoin();
				}
			}else
			{
				_go.GetComponent<Renderer>().material.color = ChangeColor;
			}
		}
		_gopar.transform.position = new Vector3(FloorObj.transform.position.x + UnityEngine.Random.Range(-0.3f, 0.3f), FloorObj.transform.position.y, FloorObj.transform.position.z);
		_gopar.transform.SetParent (FloorObj.transform);
		deltaXforCreate = _gopar;

        if (CountCreated % NextChangeColor == 0)
        {
            CreateChangerColor(ChangeColor);
            NextChangeColor += Random.Range(40, 70);
        }
		distance += BeetwenDistance;
		BeetwenDistanceList.Add (BeetwenDistance);
		if (BeetwenDistance < 75)
		BeetwenDistance += UnityEngine.Random.Range(0.1f, 0.2f);
		CountCreated++;
	}

	private void CreateChangerColor(Color cl)
	{
		GameObject _gopar = new GameObject ();
		GameObject _go = Instantiate (BonusCube);
		_go.transform.GetChild (0).GetComponent<ParticleSystem> ().startColor = cl;
		_go.transform.position = new Vector3 (FloorObj.transform.position.x, 1, distance);
		_go.transform.SetParent (_gopar.transform);
		_go.GetComponent<Renderer>().material.color = cl;
		_go.tag = "BonusChangeColor";
		BeenColor = cl;
		_gopar.transform.SetParent (FloorObj.transform);
	}

	private void GetSetCoin()
	{
		SetCoin += UnityEngine.Random.Range (15, 40);
		CountSetCoin = UnityEngine.Random.Range (4, 11);
	}
}
