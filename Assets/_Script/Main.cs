using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using GoogleMobileAds.Api;

public class Main : MonoBehaviour {


    public GameObject FloorObj;
    public bool CanJump = true;
    public bool LoseBool = true;
    public bool Pause;
    public bool Shop;
    private Vector3 MouseClick;
    private Action NowAction;

    [Header("BackGround")]
    public GameObject BackGroundPanel;
    public Text ScoreText;
    [Header("MainPanel")]
    public GameObject MainPanel;
    public Image LevelBar, QuestImage;
    public Text ExpText, LevelText, ScoreTextImage, QuestTextStart;
    [Header("LosePanel")]
    public GameObject LosePanel;
    public Text ScoreTextLast;
    public GameObject LosePanelNew;
    [Header("PausePanel")]
    public GameObject PausePanel;
    public Button SoundButtonPause, MusicButtonPause;
    public Text ScoreTextPause, PauseTimerText;
    private DateTime PauseStart;
    private TimeSpan PauseAll;
    public Text PauseText;
    [Header("ShopPanel")]
    public GameObject ShopPanel;
    public GameObject ShopObjects;
    public GameObject ShopAnimSphere;
    public Text CoinsText,
        PriceText,
        NeedLevelText;
    private int NowClick;
    public Button BuyButton, SetButton;
    [Header("SettingsPanel")]
    public GameObject SettingsPanel;
    public Button SoundButtonSettings, MusicButtonSettings;
    public Text BestScoreTextSettings,
                AverageScoreTextSettings,
                CountGamesTextSettings,
                CoinsTextSettings,
                CoinsCollected;
    public Dropdown QualityDropDown;
    [Header("SettingsGamePanel")]
    public GameObject SettingsGamePanel;
    public DateTime StartDT, EndDT;
    public Text StartDTText,
                EndDTText,
                PlayTimeText,
                AverageScoreBAText,
                ScoreGame,
                BestScoreGame,
                NumberGame,
                PauseGame,
                PlusCoins;
    [Header("RewardedPanel")]
    public GameObject RewardedPanel;
    public GameObject RewardedObjects;
    public GameObject GetPanel;
    public GameObject BlockedPanel;
    public Text RewardedText;

    public GameObject QuestionPanel;
    public GameObject PlayerObject;
    public AudioSource CameraMainAS;
    public Rigidbody PlayerRigidbody;
    public PlayerControl PlayerControlScript;
    public CreateFloors CreateFloorsScript;
    public bool Cheating;
    private float _experiance = 0;

    const string LowerBanner = "ca-app-pub-6132055699566134/4964069458";
    const string GetMoneyVideo = "ca-app-pub-6132055699566134/4033722045";
    const string AfterFiveLose = "ca-app-pub-6132055699566134/6878080068";
    const string appID = "ca-app-pub-6132055699566134~3104192875";
    private BannerView lowerBanner;
    private RewardBasedVideoAd rewardBasedVideo;
    private InterstitialAd afterFiveLoseBanner;
    private bool lowerIsActive;
    private int countOfLoseForBanner;

    void Start()
    {
        Lang.Starting();
        MobileAds.Initialize(appID);
            if (Save.Level < 10)
            {
                while (Save.Experiance >= GetExpForNextLevel(Save.Level))
                {
                    Save.Experiance -= GetExpForNextLevel(Save.Level);
                    Save.Level++;
                }
                LevelBar.fillAmount = (float)Save.Experiance / (float)GetExpForNextLevel(Save.Level);
                ExpText.text = Save.Experiance + "/" + GetExpForNextLevel(Save.Level);
            }
            else
            {
                LevelBar.fillAmount = 1f;
                ExpText.text = Save.Experiance + "/∞";
            }
            LevelText.text = Save.Level.ToString();
            CameraMainAS.volume = PlayerPrefs.GetInt("MusicVolume", 1);
            CameraMainAS.Play();
            StartRewarded();
            if (PlayerPrefs.GetInt("Replay", 0) == 1)
                ClickOnTapToPlay();
            PlayerPrefs.SetInt("Replay", 0);
            RewardedText.text = Lang.Phrase("Rewarded");
            PlayerPrefs.SetInt("CheckYear", DateTime.Now.Year);
            PlayerPrefs.SetInt("CheckMonth", DateTime.Now.Month);
            PlayerPrefs.SetInt("CheckDay", DateTime.Now.Day);
            PlayerPrefs.SetInt("CheckHour", DateTime.Now.Hour);
            PlayerPrefs.SetInt("CheckMinute", DateTime.Now.Minute);
            PlayerPrefs.SetInt("CheckSecond", DateTime.Now.Second);

        lowerBanner = new BannerView(LowerBanner, AdSize.Banner, AdPosition.Bottom);
        this.rewardBasedVideo = RewardBasedVideoAd.Instance;
        afterFiveLoseBanner = new InterstitialAd(AfterFiveLose);
        AdRequest _request = new AdRequest.Builder().Build();
        lowerBanner.OnAdFailedToLoad += OnAdFailToLoadedBanner;
        lowerBanner.OnAdLoaded += OnAdLoadedBanner;
        lowerBanner.OnAdClosed += OnAdClosedBanner;
        lowerBanner.LoadAd(_request);
        this.rewardBasedVideo.OnAdRewarded += OnAdRewardedVideo;
        this.rewardBasedVideo.OnAdLoaded += OnAdLoadedVideo;
        this.rewardBasedVideo.OnAdClosed += OnAdClosedVideo;
        this.rewardBasedVideo.OnAdFailedToLoad += OnAdFailedToLoadVideo;
        this.rewardBasedVideo.LoadAd(_request, GetMoneyVideo);
        afterFiveLoseBanner.OnAdClosed += OnAdClosedAfter;
        afterFiveLoseBanner.LoadAd(_request);
    }

    void Update()
    {
        if (!LoseBool && !Pause)
        {
            if (!Cheating)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    MouseClick = Camera.main.ScreenPointToRay(Input.mousePosition).GetPoint(10);
                    MouseClick.z = 0;
                    MouseClick.y = 0;
                }
                else if (Input.GetMouseButton(0))
                {
                    Vector3 _v3 = Camera.main.ScreenPointToRay(Input.mousePosition).GetPoint(10);
                    _v3.z = 0;
                    _v3.y = 0;
                    Vector3 v3 = FloorObj.transform.position;
                    v3 += _v3 - MouseClick;
                    FloorObj.transform.position = v3;
                    MouseClick = _v3;
                }
            }
            ScoreText.text = Lang.Phrase("Score") + ": " + PlayerControlScript.Score.ToString();
        }
        else if (Shop)
        {
            if (Input.GetMouseButtonUp(0))
            {
                RaycastHit _hit;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out _hit) && _hit.transform.tag == "Shop")
                {
                    Destroy(ShopAnimSphere.transform.GetChild(0).gameObject);
                    GameObject _go = Instantiate(Resources.Load<GameObject>(_hit.transform.name));
                    _go.transform.SetParent(ShopAnimSphere.transform);
                    _go.transform.rotation = ShopAnimSphere.transform.rotation;
                    _go.transform.position = ShopAnimSphere.transform.position;
                    _go.transform.localScale = new Vector3(1, 1, 1);
                    CheckShop(int.Parse(_hit.transform.name));
                }
            }
        }
        else if (Pause)
        {
            PauseTimerText.text = GetFirstZero((DateTime.Now - PauseStart).Hours) + ":" + GetFirstZero((DateTime.Now - PauseStart).Minutes) + ":" + GetFirstZero((DateTime.Now - PauseStart).Seconds);
        }
    }


    #region Main

    public int GetExpForNextLevel(int level)
    {
        switch (level)
        {
            case 0: return 100;
            case 1: return 250;
            case 2: return 500;
        }
        return 1500;
    }

    public void ClickOnTapToPlay()
    {
        Camera.main.GetComponent<Animator>().Play("CameraToPlay");
        MainPanel.SetActive(false);
        StartCoroutine(CreateFloorsWhenStart());
    }

    IEnumerator CreateFloorsWhenStart()
    {
        for (int i = 0; i < 5; i++)
        {
            CreateFloorsScript.StartCreate();
            yield return new WaitForSeconds(0.2f);
        }
    }

    public void ClickOnTapToPlayAnim()
    {
        Camera.main.GetComponent<Animator>().enabled = false;
        PlayerObject.GetComponent<Animator>().enabled = false;
        StartDT = DateTime.Now;
        BackGroundPanel.SetActive(true);
        PlayerRigidbody.useGravity = true;
        LoseBool = false;
    }

    public void ClickOnSettings()
    {
        if (!SettingsPanel.activeInHierarchy)
        {
            MainPanel.SetActive(false);
            SettingsPanel.SetActive(true);
            PlayerObject.SetActive(false);
            SettingsPanel.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = Lang.Phrase("Settings");
            BestScoreTextSettings.text = Lang.Phrase("Best score") + ": " + Save.BestScore.ToString();
            AverageScoreTextSettings.text = Lang.Phrase("Average score") + ": " + Save.AverageScore.ToString("0.00");
            CountGamesTextSettings.text = Lang.Phrase("Count games") + ": " + Save.CountGames.ToString();
            CoinsTextSettings.text = Lang.Phrase("Coins") + ": " + Save.Coins.ToString();
            CoinsCollected.text = Lang.Phrase("Coins collected") + ": " + Save.CoinsCollected.ToString();
            MusicButtonSettings.image.sprite = PlayerPrefs.GetInt("MusicVolume", 1) == 1 ? Resources.LoadAll<Sprite>("InterFace")[10] : Resources.LoadAll<Sprite>("InterFace")[11];
            SoundButtonSettings.image.sprite = PlayerPrefs.GetInt("SoundVolume",1) == 1 ? Resources.LoadAll<Sprite>("InterFace")[1] : Resources.LoadAll<Sprite>("InterFace")[2];
        } else
        {
            MainPanel.SetActive(true);
            SettingsPanel.SetActive(false);
            PlayerObject.SetActive(true);
        }
    }

    void CheckShop(int _nowclick)
    {
            NowClick = _nowclick;
            if (PlayerPrefs.GetString("ShopObjects", "TFFFFFFFFFFFFFFF")[NowClick] == 'T')
            {
                BuyButton.gameObject.SetActive(false);
                if (PlayerPrefs.GetInt("NowClick", 0) != NowClick)
                    SetButton.interactable = true;
                else SetButton.interactable = false;
                SetButton.gameObject.SetActive(true);
                PriceText.text = Lang.Phrase("Bought");
                NeedLevelText.text = "";
            }
            else
            {
                PriceText.text = Lang.Phrase("Price") + ": " + GetPrice(NowClick);
                if (Save.Coins >= GetPrice(NowClick))
                    BuyButton.interactable = true;
                else
                    BuyButton.interactable = false;
                if (GetLevelForThisSphere(NowClick) <= Save.Level)
                {
                    NeedLevelText.text = "";
                }
                else
                {
                    NeedLevelText.text = Lang.Phrase("Need level") + ": " + GetLevelForThisSphere(NowClick);
                    BuyButton.interactable = false;
                }
                BuyButton.gameObject.SetActive(true);
                SetButton.gameObject.SetActive(false);
            }
        
    }
   
    public void ClickCheating() // RostikName
    {
        Cheating = true;
        SettingsPanel.SetActive(false);
        ClickOnTapToPlay();
    }

    public void ClickOnShop(bool isShop)
    {
        Shop = isShop;
        if (isShop)
        {
            ShopPanel.transform.GetChild(0).GetChild(4).GetChild(0).GetComponent<Text>().text = Lang.Phrase("Buy");
            ShopPanel.transform.GetChild(0).GetChild(5).GetChild(0).GetComponent<Text>().text = Lang.Phrase("Set");
            PlayerObject.SetActive(false);
            MainPanel.SetActive(false);
            ShopPanel.SetActive(true);
            CoinsText.text = Save.Coins.ToString();
            if (!rewardBasedVideo.IsLoaded())
            {
                ShopPanel.transform.GetChild(0).GetChild(0).GetComponent<Button>().interactable = false;
                AdRequest request = new AdRequest.Builder().Build();
                rewardBasedVideo.LoadAd(request, GetMoneyVideo);
            }
            else
            {
                ShopPanel.transform.GetChild(0).GetChild(0).GetComponent<Button>().interactable = true;
            }
        } else
        {
            ShopPanel.SetActive(false);
            MainPanel.SetActive(true);
            PlayerObject.SetActive(true);
        }
    }

    public void ClickOnBuyShop()
    {
        if (Save.Coins >= GetPrice(NowClick)) {
            Save.Coins -= GetPrice(NowClick);
            CoinsText.text = Save.Coins.ToString();
            string ShopString = PlayerPrefs.GetString("ShopObjects", "TFFFFFFFFFFFFFFF");
            ShopString = ShopString.Remove(NowClick, 1);
            ShopString = ShopString.Insert(NowClick, "T");
            PlayerPrefs.SetString("ShopObjects", ShopString);
            SetButton.interactable = true;
            SetButton.gameObject.SetActive(true);
            BuyButton.gameObject.SetActive(false);
        } else
        {
            BuyButton.interactable = false;
        }
    }

    public void ClickOnSet()
    {
        PlayerPrefs.SetInt("NowClick", NowClick);
        SetButton.interactable = false;
        PlayerControlScript.SetAllAnother();
    }

    public void ClickOnVideoButton()
    {
        rewardBasedVideo.Show();
        //QuestionPanelSett(Lang.Phrase("Watch video to get coins?"), Lang.Phrase("Yes"), Lang.Phrase("No"), ClickOnVideoButtonNext);
    }

    

    void ClickOnVideoButtonNext()
    {
        /*if (Advertisement.IsReady("rewardedVideo"))
        {
            Advertisement.Show("rewardedVideo", new ShowOptions
            {
                resultCallback = result =>
                {
                    if (result == ShowResult.Finished)
                    {
                        Save.Coins += 50;
                        CoinsText.text = Save.Coins.ToString();
                        CheckShop(NowClick);
                    }
                }
            });
        } else
        {
            StartCoroutine(VideoPanelIE());
        }
        */
    }

    public void ChangeQualityDropDown()
    {
        QualitySettings.SetQualityLevel(QualityDropDown.value);
        PlayerPrefs.SetInt("QualityDropDown", QualityDropDown.value);
    }

    int GetPrice(int num)
    {
        switch (num)
        {
            case 1: return 150;
            case 2: return 300;
            case 3: return 500;
            case 4: return 750;
            case 5: return 1000;
            case 6: return 1500;
            case 7: return 2500;
            case 8: return 5000;
            case 9: return 10000;
            case 10: return 1;
        }
        return 0;
    }

    int GetLevelForThisSphere(int num)
    {
        switch (num)
        {
            case 0: return 0;
            case 1: return 1;
            case 2: return 2;
            case 3: return 3;
            case 4: return 4;
            case 5: return 5;
            case 6: return 6;
            case 7: return 7;
            case 8: return 8;
            case 9: return 9;
        }
        return 0;
    }

    public void ClickSoundButtonSettings()
    {
        if (PlayerPrefs.GetInt("SoundVolume", 1) == 1)
        {
            PlayerPrefs.SetInt("SoundVolume", 0);
            SoundButtonSettings.image.sprite = Resources.LoadAll<Sprite>("InterFace")[2];
        }
        else
        {
            PlayerPrefs.SetInt("SoundVolume", 1);
            SoundButtonSettings.image.sprite = Resources.LoadAll<Sprite>("InterFace")[1];
        }
    }

    public void ClickMusicButtonSettings()
    {
        if (PlayerPrefs.GetInt("MusicVolume", 1) == 1)
        {
            PlayerPrefs.SetInt("MusicVolume", 0);
            MusicButtonSettings.image.sprite = Resources.LoadAll<Sprite>("InterFace")[11];
        }
        else
        {
            PlayerPrefs.SetInt("MusicVolume", 1);
            MusicButtonSettings.image.sprite = Resources.LoadAll<Sprite>("InterFace")[10];
        }

        CameraMainAS.volume = PlayerPrefs.GetInt("MusicVolume", 1);
    }

    #endregion

    #region BackGround

    public void ClickOnPause()
    {
        if (!Pause) {
            PauseText.text = Lang.Phrase("Pause");
            Time.timeScale = 0;
            Cheating = false;
            ScoreTextPause.text = Lang.Phrase("Score") + ": " + PlayerControlScript.Score;
            PauseTimerText.text = "00:00:00";
            PauseStart = DateTime.Now;
            SoundButtonPause.image.sprite = PlayerPrefs.GetInt("SoundVolume", 1) == 1 ? Resources.LoadAll<Sprite>("InterFace")[1] : Resources.LoadAll<Sprite>("InterFace")[2];
            MusicButtonPause.image.sprite = PlayerPrefs.GetInt("MusicVolume", 1) == 1 ? Resources.LoadAll<Sprite>("InterFace")[10] : Resources.LoadAll<Sprite>("InterFace")[11];
            PausePanel.SetActive(true);
        } else
        {
            PausePanel.SetActive(false);
            Time.timeScale = 1;
            StopCoroutine("PauseTimer");
            PauseAll += (DateTime.Now - PauseStart);
            Debug.Log(PauseAll);
        }
        Pause = !Pause;
    }



    public void ClickOnSoundPause()
    {
        if (PlayerPrefs.GetInt("SoundVolume", 1) == 1) {
            PlayerPrefs.SetInt("SoundVolume", 0);
            SoundButtonPause.image.sprite = Resources.LoadAll<Sprite>("InterFace")[2];
        } else
        {
            PlayerPrefs.SetInt("SoundVolume", 1);
            SoundButtonPause.image.sprite = Resources.LoadAll<Sprite>("InterFace")[1];
        }
    }

    public void ClickOnMusicPause()
    {
        if (PlayerPrefs.GetInt("MusicVolume", 1) == 1)
        {
            PlayerPrefs.SetInt("MusicVolume", 0);
            MusicButtonPause.image.sprite = Resources.LoadAll<Sprite>("InterFace")[11];
        }
        else
        {
            PlayerPrefs.SetInt("MusicVolume", 1);
            MusicButtonPause.image.sprite = Resources.LoadAll<Sprite>("InterFace")[10];
        }
        CameraMainAS.volume = PlayerPrefs.GetInt("MusicVolume", 1);
    }

    void OnApplicationPause(bool isPause)
    {
        if (!Pause && !LoseBool)
        {
            ClickOnPause();
        }
    }

    string DateToUkrainianString(DateTime dt)
    {
        return GetFirstZero(dt.Hour) + ":" + GetFirstZero(dt.Minute) + ":" + GetFirstZero(dt.Second);
    }

    string DateToUkrainianString(TimeSpan dt)
    {
        return GetFirstZero(dt.Hours) + ":" + GetFirstZero(dt.Minutes) + ":" + GetFirstZero(dt.Seconds);
    }

    string GetFirstZero(int i)
    {
        if (i < 10)
            return "0" + i.ToString();
        else
            return i.ToString();
    }

    #endregion

    #region Rewarded

    public void ClickOnRewardedButtonMain()
    {
        Transform _tr = RewardedPanel.transform.GetChild(0).GetChild(0);
        for (int i = 0; i < 4; i++)
            _tr.GetChild(i).GetChild(0).GetComponent<Text>().text = Lang.Phrase("Day") + " " + (i + 1).ToString();
        _tr.GetChild(4).GetChild(0).GetComponent<Text>().text = Lang.Phrase("Day") + " 5+";
        MainPanel.SetActive(false);
        PlayerObject.SetActive(false);
        if (GetPanel.activeSelf)
            GetPanel.SetActive(false);
        for (int i = 0; i < PlayerPrefs.GetInt("RewardedDayNumber", 0); i++) {
            if (PlayerPrefs.GetInt("GetDay" + i.ToString(), 0) != 0)
                RewardedObjects.transform.GetChild(i).GetChild(1).GetChild(0).GetComponent<Text>().text = PlayerPrefs.GetInt("GetDay" + i.ToString()).ToString();
            RewardedObjects.transform.GetChild(i).GetChild(2).gameObject.SetActive(true);
            RewardedObjects.transform.GetChild(i).GetChild(3).gameObject.SetActive(false);
            RewardedObjects.transform.GetChild(i).GetChild(1).GetComponent<Button>().interactable = false;
        }
        if (PlayerPrefs.GetInt("RewardedDayNumber", 0) == 4)
            RewardedObjects.transform.GetChild(4).GetChild(1).GetChild(0).GetComponent<Text>().text = PlayerPrefs.GetInt("GetDay4").ToString();

        RewardedPanel.SetActive(true);
        StartCoroutine(RewardedTimer(RewardedObjects.transform.GetChild(PlayerPrefs.GetInt("RewardedDayNumber", 0)).GetChild(4).GetComponent<Text>()));
    }

    void CheckDayGet()
    {
        GameObject _tmpGetObj = RewardedPanel.transform.GetChild(0).GetChild(0).gameObject;
        for (int i = 0; i < 5; i++)
        {
            if (PlayerPrefs.GetInt("GetDay" + i.ToString(), 0) != 0)
                _tmpGetObj.transform.GetChild(i).GetChild(1).GetChild(0).GetComponent<Text>().text = PlayerPrefs.GetInt("GetDay" + i.ToString()).ToString();
        }
    }

    public void StartRewarded()
    {
        DateTime RewardedDT = new DateTime(PlayerPrefs.GetInt("RewardedYear", DateTime.Now.Year), PlayerPrefs.GetInt("RewardedMonth", DateTime.Now.Month), PlayerPrefs.GetInt("RewardedDay", DateTime.Now.Day),
                                           PlayerPrefs.GetInt("RewardedHour", DateTime.Now.Hour), PlayerPrefs.GetInt("RewardedMinute", DateTime.Now.Minute), PlayerPrefs.GetInt("RewardedSecond", DateTime.Now.Second));
        if (RewardedDT <= DateTime.Now && (DateTime.Now - RewardedDT).Days == 0) {
            Transform _tr = RewardedPanel.transform.GetChild(0).GetChild(0);
            for (int i = 0; i < 4; i++)
                _tr.GetChild(i).GetChild(0).GetComponent<Text>().text = Lang.Phrase("Day") + " " + (i + 1).ToString();
            _tr.GetChild(4).GetChild(0).GetComponent<Text>().text = Lang.Phrase("Day") + " 5+";
            RewardedPanel.SetActive(true);
            MainPanel.SetActive(false);
            PlayerObject.SetActive(false);
            for (int i = 0; i < PlayerPrefs.GetInt("RewardedDayNumber", 0); i++) {
                if (PlayerPrefs.GetInt("GetDay" + i.ToString(), 0) != 0)
                    RewardedObjects.transform.GetChild(i).GetChild(1).GetChild(0).GetComponent<Text>().text = PlayerPrefs.GetInt("GetDay" + i.ToString()).ToString();
                RewardedObjects.transform.GetChild(i).GetChild(2).gameObject.SetActive(true);
                RewardedObjects.transform.GetChild(i).GetChild(3).gameObject.SetActive(false);
                RewardedObjects.transform.GetChild(i).GetChild(1).GetComponent<Button>().interactable = false;
            }
            RewardedObjects.transform.GetChild(PlayerPrefs.GetInt("RewardedDayNumber", 0)).GetChild(3).gameObject.SetActive(false);
        } else if ((DateTime.Now - RewardedDT).Days >= 1) {
            for (int i = 0; i < 5; i++)
                PlayerPrefs.SetInt("GetDay" + i.ToString(), 0);
            PlayerPrefs.SetInt("RewardedDayNumber", 0);
            PlayerPrefs.SetInt("RewardedYear", DateTime.Now.Year);
            PlayerPrefs.SetInt("RewardedMonth", DateTime.Now.Month);
            PlayerPrefs.SetInt("RewardedDay", DateTime.Now.Day);
            PlayerPrefs.SetInt("RewardedHour", DateTime.Now.Hour);
            PlayerPrefs.SetInt("RewardedMinute", DateTime.Now.Minute);
            PlayerPrefs.SetInt("RewardedSecond", DateTime.Now.Second);
            StartRewarded();
        }
    }

    IEnumerator RewardedTimer(Text _text)
    {
        DateTime RewardedDT = new DateTime(PlayerPrefs.GetInt("RewardedYear"), PlayerPrefs.GetInt("RewardedMonth"), PlayerPrefs.GetInt("RewardedDay"),
                                            PlayerPrefs.GetInt("RewardedHour"), PlayerPrefs.GetInt("RewardedMinute"), PlayerPrefs.GetInt("RewardedSecond"));
        _text.gameObject.SetActive(true);
        while (RewardedDT > DateTime.Now)
        {
            _text.text = GetFirstZero((RewardedDT - DateTime.Now).Hours) + ":" + GetFirstZero((RewardedDT - DateTime.Now).Minutes) + ":" + GetFirstZero((RewardedDT - DateTime.Now).Seconds);
            yield return new WaitForSeconds(1f);
        }
        _text.gameObject.SetActive(false);
        StartRewarded();
    }

    public void ClickRewardedButton(GameObject _go)
    {
        DateTime RewardedDT = DateTime.Now.AddDays(1);
        PlayerPrefs.SetInt("RewardedYear", RewardedDT.Year);
        PlayerPrefs.SetInt("RewardedMonth", RewardedDT.Month);
        PlayerPrefs.SetInt("RewardedDay", RewardedDT.Day);
        PlayerPrefs.SetInt("RewardedHour", RewardedDT.Hour);
        PlayerPrefs.SetInt("RewardedMinute", RewardedDT.Minute);
        PlayerPrefs.SetInt("RewardedSecond", RewardedDT.Second);

        int _tmpint = 0;
        switch (int.Parse(_go.name))
        {
            case 0: _tmpint = UnityEngine.Random.Range(50, 101); break;
            case 1: _tmpint = UnityEngine.Random.Range(100, 201); break;
            case 2: _tmpint = UnityEngine.Random.Range(200, 351); break;
            case 3: _tmpint = UnityEngine.Random.Range(350, 501); break;
            case 4: _tmpint = UnityEngine.Random.Range(500, 1001); break;
        }

        Save.Coins += _tmpint;
        GetPanel.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = _tmpint.ToString();
        GetPanel.SetActive(true);

        RewardedObjects.transform.GetChild(PlayerPrefs.GetInt("RewardedDayNumber", 0)).GetChild(1).GetChild(0).GetComponent<Text>().text = _tmpint.ToString();
        PlayerPrefs.SetInt("GetDay" + PlayerPrefs.GetInt("RewardedDayNumber", 0).ToString(), _tmpint);
        PlayerPrefs.SetInt("RewardedDayNumber", Mathf.Clamp(PlayerPrefs.GetInt("RewardedDayNumber", 0) + 1, 0, 4));
        RewardedObjects.transform.GetChild(PlayerPrefs.GetInt("RewardedDayNumber", 0) - 1).GetChild(2).gameObject.SetActive(true);
        RewardedObjects.transform.GetChild(PlayerPrefs.GetInt("RewardedDayNumber", 0) - 1).GetChild(3).gameObject.SetActive(false);
        RewardedObjects.transform.GetChild(PlayerPrefs.GetInt("RewardedDayNumber", 0) - 1).GetChild(1).GetComponent<Button>().interactable = false;
        if (PlayerPrefs.GetInt("RewardedDayNumber", 0) == 4)
            RewardedObjects.transform.GetChild(PlayerPrefs.GetInt("RewardedDayNumber", 0)).GetChild(3).gameObject.SetActive(true);
        StartCoroutine(RewardedTimer(RewardedObjects.transform.GetChild(PlayerPrefs.GetInt("RewardedDayNumber", 0)).GetChild(4).GetComponent<Text>()));
    }

    #endregion

    #region Lose

    public void Lose()
    {
        if (!LoseBool) {
            if (PlayerPrefs.GetInt("SoundVolume", 0) != 0)
            {
                GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Lose"));
                CameraMainAS.Stop();
            }
            Save.Coins += PlayerControlScript.PlusCoins;
            if (PlayerObject.transform.GetChild(0).GetComponent<Animator>() != null)
                PlayerObject.transform.GetChild(0).GetComponent<Animator>().enabled = false;
            EndDT = DateTime.Now;
            LoseBool = true;
            CanJump = false;
            BackGroundPanel.SetActive(false);
            float tmpavg = Save.AverageScore;
            _experiance = Save.Experiance;
            Save.CoinsCollected += PlayerControlScript.PlusCoins;
            Save.Experiance += PlayerControlScript.Score;
            Save.CountGames++;
            Save.AverageScore = (Save.AverageScore * (Save.CountGames - 1) + PlayerControlScript.Score) / Save.CountGames;
            LosePanelNew.SetActive(true);
            LosePanelNew.transform.GetChild(0).GetComponent<Animator>().Play("LoseAnim");
            countOfLoseForBanner++;
        }
    }

    public void AfterBlackScreen()
    {
        if (PlayerObject.transform.GetChild(0).GetComponent<Animator>() != null)
            PlayerObject.transform.GetChild(0).GetComponent<Animator>().enabled = true;
        PlayerObject.GetComponent<Animator>().enabled = true;
        PlayerObject.GetComponent<Animator>().Play("Start");
        PlayerObject.GetComponent<Rigidbody>().useGravity = false;
        PlayerObject.GetComponent<Rigidbody>().Sleep();
        PlayerObject.transform.rotation = new Quaternion(0, 0, 0, 1);

        StartCoroutine(UpExp());
        CanJump = true;

        PlayerObject.transform.position = new Vector3(0, 2, 0);
        Camera.main.GetComponent<Animator>().enabled = true;
        Camera.main.transform.position = new Vector3(0, 1.87f, -2.44f);
        CreateFloorsScript.SetAllAsBegin();
        if (PlayerControlScript.Score > Save.BestScore)
        {
            Save.BestScore = PlayerControlScript.Score;
            ScoreTextImage.text = Lang.Phrase("Best score") + ": " + PlayerControlScript.Score;
        }
        else
        {
            ScoreTextImage.text = Lang.Phrase("Score") + ": " + PlayerControlScript.Score;
        }
        PlayerControlScript.Score = 0;
        ScoreTextImage.transform.parent.gameObject.SetActive(true);
        CameraMainAS.Play();
        if (!afterFiveLoseBanner.IsLoaded())
        {
            afterFiveLoseBanner = new InterstitialAd(AfterFiveLose);
            AdRequest _requestAFLB = new AdRequest.Builder().Build();
            afterFiveLoseBanner.LoadAd(_requestAFLB);
        }

        if (countOfLoseForBanner >= 4 && afterFiveLoseBanner.IsLoaded())
        {
            afterFiveLoseBanner.Show();
            countOfLoseForBanner = 0;
        }
        else
        {
            LosePanelNew.SetActive(false);
            MainPanel.SetActive(true);
        }
    }

    IEnumerator UpExp()
    {
        if (Save.Level < 10)
        {
            ExpText.text = (int)_experiance + "+" + (int)(Save.Experiance - _experiance) + "/" + GetExpForNextLevel(Save.Level);
            yield return new WaitForSeconds(3f);
            while (_experiance < (float)Save.Experiance)
           {
                _experiance = Mathf.Lerp(_experiance, Save.Experiance + 1, Time.deltaTime / 0.5f);
                if (_experiance >= GetExpForNextLevel(Save.Level))
                {
                    Save.Experiance -= GetExpForNextLevel(Save.Level);
                    _experiance -= GetExpForNextLevel(Save.Level);
                    Save.Level++;
                }
                LevelBar.fillAmount = _experiance / (float)GetExpForNextLevel(Save.Level);
                ExpText.text = (int)_experiance + "+" + (int)(Save.Experiance - _experiance) + "/" + GetExpForNextLevel(Save.Level);
                LevelText.text = Save.Level.ToString();
                yield return new WaitForFixedUpdate();
            }
            ExpText.text = (int)_experiance + "/" + GetExpForNextLevel(Save.Level);
        }
        else
        {
            ExpText.text = (int)_experiance + "+" + (int)(Save.Experiance - _experiance) + "/∞";
            yield return new WaitForSeconds(3f);
            while (_experiance < (float)Save.Experiance)
            {
                _experiance = Mathf.Lerp(_experiance, Save.Experiance + 1, Time.deltaTime / 0.5f);
                LevelBar.fillAmount = 1f;
                ExpText.text = (int)_experiance + "/∞";
                LevelText.text = "10";
                yield return new WaitForFixedUpdate();
            }
        }

    }

	public void ClickOnMainMenu()
	{
		Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

	public void ClickOnReplay()
	{
		Time.timeScale = 1;
		Pause = false;
		Cheating = false; // CHAET
		LoseBool = false;
		CanJump = true;
		PausePanel.SetActive (false);
		LosePanel.SetActive (false);
		BackGroundPanel.SetActive (true);
		PlayerObject.transform.position = new Vector3 (0, 2, 0);
		Destroy (FloorObj);
		PauseAll = TimeSpan.Zero;
		FloorObj = new GameObject ();
		FloorObj.transform.position = Vector3.zero;
		CreateFloorsScript.FloorObj = FloorObj;
		PlayerControlScript.FloorObj = FloorObj;
		CreateFloorsScript.SetAllAsBegin ();	
		PlayerControlScript.SetAllAsBegin ();
		if(PlayerObject.transform.GetChild(0).GetComponent<Animator>() != null)
		PlayerObject.transform.GetChild(0).GetComponent<Animator>().enabled = true;

		StartDT = DateTime.Now;
		/*
		PlayerPrefs.SetInt ("Replay", 1);
		
		RetryPlay.allowSceneActivation = true;
		*/
	}

	#endregion

	void QuestionPanelSett(string _title, string _buttonYesText, string _buttonNoText, Action tmpact)
	{
		Transform _QP = QuestionPanel.transform.GetChild (0).GetChild (0);
		_QP.GetChild (0).GetComponent<Text> ().text = _title;
		_QP.GetChild (1).GetChild (0).GetComponent<Text> ().text = _buttonYesText;
		_QP.GetChild (2).GetChild (0).GetComponent<Text> ().text = _buttonNoText;
		NowAction = tmpact;
		QuestionPanel.SetActive (true);
	} 

	public void ClickYes()
	{
		NowAction ();
		QuestionPanel.SetActive (false);
	}

    #region LowerBanner

    void OnAdLoadedBanner(object sender, System.EventArgs args)
    {
    }

    void OnAdFailToLoadedBanner(object sender, System.EventArgs args)
    {
        AdRequest _request = new AdRequest.Builder().Build();
        lowerBanner.LoadAd(_request);
    }

    void OnAdClosedBanner(object sender, System.EventArgs args)
    {
        AdRequest _request = new AdRequest.Builder().Build();
        lowerBanner.LoadAd(_request);
    }

    #endregion

    #region VideoAd

    void OnAdFailedToLoadVideo(object sender, System.EventArgs args)
    {
        ShopPanel.transform.GetChild(0).GetChild(0).GetComponent<Button>().interactable = false;
        AdRequest request = new AdRequest.Builder().Build();
        rewardBasedVideo.LoadAd(request, GetMoneyVideo);
    }

    void OnAdLoadedVideo(object sender, System.EventArgs args)
    {
        ShopPanel.transform.GetChild(0).GetChild(0).GetComponent<Button>().interactable = true;
    }

    void OnAdClosedVideo(object sender, System.EventArgs args)
    {
        ShopPanel.transform.GetChild(0).GetChild(0).GetComponent<Button>().interactable = false;
        AdRequest request = new AdRequest.Builder().Build();
        rewardBasedVideo.LoadAd(request, GetMoneyVideo);
    }

    void OnAdRewardedVideo(object sender, System.EventArgs args)
    {
        Save.Coins += 50;
        CoinsText.text = Save.Coins.ToString();
    }

    #endregion

    #region AfterFiveLose

    void OnAdClosedAfter(object sender, System.EventArgs args)
    {
        LosePanelNew.SetActive(false);
        MainPanel.SetActive(true);
    }

    #endregion
}
