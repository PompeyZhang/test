
//////////////////////////////////////////////////////////////////////////
//
//   FileName : GameWorld.cs
//     Author : Chiyer
// CreateTime : 2014-03-12
//       Desc :
//
//////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public enum GameStartStep
{
	none,
	init,
	start,
	run,
	exit,
}

public static class GameWorld
{
    static bool initOnce = false;
    public static GameConfig config;
    public static GameDefault Default;
    public static GameObject gameWorld;
    public static List<Schedule> scheduleList;
    public static AttributeFactorDeploy attributeFactorDeploy;
	public static bool startLogin = false;
    public static List<ScheduleDelegate> deleteList = new List<ScheduleDelegate>();

    public static bool isLogin = false;
    public static bool isTestLogin = false; //测试
    private static float memoryLogTime = Time.time + memoryLogInterval;
    private static float memoryLogInterval = 300f;
    private static double sumFps = 0;  //帧率和
    private static uint frameCount = 0; //帧数
    private static float timeFps;
    private static int tickFps;
    private static GameStartStep step = GameStartStep.none;
	private static bool initOver = false;
    public static LanguageType currentLanguage = LanguageType.SimpleCN;

    static GameWorld()
    {
        scheduleList = new List<Schedule>();
    }

    public static void Awake()
    {
        Sound.InitConfig();
        XUI_Manager.Init();
        InitLogTrack();
        timeFps = -1f;
    }

    private static void InitLogTrack()
    {
        List<string> IgnoreList = new List<string>();
        IEnumerator<LogStayTimeIgnoreDeploy> ie = TableMgr.GetTable<LogStayTimeIgnoreDeploy>().GetEnumerator();
        using (ie)
        {
            while (ie.MoveNext())
            {
                if (ie.Current != null) IgnoreList.Add(ie.Current.classView);
            }
        }
        LogTrackManager.GetInstance().Init(IgnoreList, (action, strData) =>
        {
            BufferWriter writer = new BufferWriter();
            writer.Write(action, strData);
            writer.Write();
            Net.Rpc("rpc_send_client_log", writer, result =>
            {
                DebugUtility.Log("Send client data to server");
            });

        });
    }

    public static void Start()
	{
		step = GameStartStep.init;
	}

	public static void OnDestroy()
	{
        Net.Close();
        TableDatabase.Destory();
        GameReport.Destory();
	}
	
	public static IEnumerator Init()
	{
        ResourcesDatabase.singleLoad = true;
        gameWorld.isStatic = true;
        Object.DontDestroyOnLoad(gameWorld);
        gameWorld.transform.hideFlags = HideFlags.HideInInspector;

        if (Default.UguiCanvas)
            Object.DontDestroyOnLoad(Default.UguiCanvas);
        TableDatabase.Load<HttpDeploy>("system/http");

        Random.InitState(System.DateTime.Now.Second);
        config = ConfigDatabase.Load<GameConfig>("game");
        PlanarShadow.enablePlanarShadow = config.usePlanarShadow;
        LoadAttributeFactor();
        GameQualityConfig.AppStart();
		yield return 0;

        MissionPreLoadTab.Load();

        Sound.globalMusicVolume = PlayerPrefs.GetInt("Music", 1);
        Sound.globalAudioVolume = PlayerPrefs.GetInt("Sound", 1);
        ResourcesDatabase.breakLoad = Default.resourceBreakLoad;
        
        #if !UNITY_EDITOR && UNITY_ANDROID
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
        #endif

        #if !UNITY_EDITOR
		DebugUtility.enableLog = false;
		DebugUtility.enableLogWarning = false;
		DebugUtility.enableLogError = true;
		DebugUtility.enableLogRes = false;
		DebugUtility.enableLogOverhead = false;
        #else
        DebugUtility.enableLog = Default.enableLog;
        DebugUtility.enableLogWarning = Default.enableLogWarning;
        DebugUtility.enableLogError = Default.enableLogError;
        DebugUtility.enableLogRes = Default.enableLogRes;
        DebugUtility.enableLogOverhead = Default.enableLogOverhead;
        DCManager.Init();
        #endif
        DebugUtility.enableLogOverhead = true;


        #if !UNITY_EDITOR && UNITY_STANDALONE
		
// 		DisplayConfig configDisplay = ConfigDatabase.Load<DisplayConfig>("display");
// 		if (configDisplay != null)
// 		{
// 			int designWidth = configDisplay.designWidth;
// 			int designHeigh = configDisplay.designHeigh;
//             
// 			float aspect = Mathf.Min(
// 				Screen.currentResolution.width  / designWidth,
// 				Screen.currentResolution.height  / designHeigh);
// 			if (aspect < 1f)
// 			{
// 				designWidth = (int)(designWidth * aspect);
// 				designHeigh = (int)(designHeigh * aspect);
// 			}
// 			Screen.SetResolution(designWidth, designHeigh, false);
// 			//DebugUtility.Log(string.Format("set resolution {0}x{1}", designWidth, designHeigh));
// 			
// 		}
        #endif

        #if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR

        int[] resolutions = XGameSetting.GetXResolution;

        int designWidth = resolutions[0];
        int designHeigh = resolutions[1];

        float aspect = Mathf.Min(
            Screen.currentResolution.width / designWidth,
            Screen.currentResolution.height / designHeigh);
        if (aspect < 1f)
        {
            designWidth = (int)(designWidth * aspect);
            designHeigh = (int)(designHeigh * aspect);
        }

        Screen.SetResolution(designWidth, designHeigh, false);

        #endif

        GameAssis.Start();
        SoundListenter.Init();

        yield return 0;
        GamePause.Init();
        DebugUtility.Log("GameWorld awake end!!!");
        initOver = true;
        Net.SetRecordGateway(Default.enableRecordGateway);
        Net.SetRecordBattle(Default.enableRecordBattle);
	}

	public static void OnGameStart()
	{
		//UILoginNotice.PreLoad ();

#if (XGSDK && !UNITY_EDITOR) && (UNITY_ANDROID || UNITY_IOS)
		XGSDKCallbackWrapper.CreateSDKManager(); 
        GunAdmob.Init();
        //Demo.Init();
#else
#endif
        //FBHelper.Init();
        //GoogleMobileAdsDemoScript.Init();
        if (GameWorld.isCanFastStart())
        {
            GlobalCoroutine.Start(GameWorld.IeStart());
            GlobalCoroutine.Start(ServerDownData.Ins().DownLoadServer());

            GamePlayer.InitConfig();
            Net.LoadConfig();
            GameNetData.InitRegRpc();

            ResourcesDatabase.singleLoad = false;
            Startwidget.ShortShow(() =>
            {
                isTestLogin = true;
                GameLogin.TryEnter();
                GameWorld.isLogin = true;
            });
        }
        else
        {
            LoginAnimation.LoadLogoAnimation(() =>
            {
                LoginAnimation.LoadLoginAnimation(OnStartUserLogin);
            });
        }
	}
	
	public static void OnStartUserLogin()
    {
        if (!initOnce)
        {
            GamePlayer.InitConfig();
            Net.LoadConfig();
            GameNetData.InitRegRpc();
            initOnce = true;
        }
        if (!GameWorld.isCanFastStart())
        {
            UILoginNotice.Show(() => {  },()=> { });
        }

        DebugUtility.Log("doPlatformLogin end!!!");
	}
 
    public static bool isCanFastStart()
    {
        //if (!isTestLogin)
        //if (LocalStorage.Contains("curaccount"))
        //    if (!string.IsNullOrEmpty(LocalStorage.Read<string>("curaccount")))
        //    // if(PlayerPrefs.GetInt("server") != 0)
        //    {
        //        return true;
        //    }
        if (GamePlayer.isSwitchRole)
            return true;
        return false;
    }
   public static IEnumerator IeStart()
    {
        yield return 0;
        Net.LoadProtocol();
        Platform.inst.PlatformType = Platform.inst.GetChannelId();
        DCManager.StartReport();
    }

    public static int FIGHT_FRAME = 60;
    public static int UI_FRAME = 60;
    public static int PAUSE_FRAME = 60;
    
    public static void SetFrameRate(int value)
    {
#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
        Application.targetFrameRate = value;
#endif
    }

    public static void ChangeFramePreSetValue(int valueFight, int valueUI, int valuePause)
    {
        FIGHT_FRAME = valueFight;
        UI_FRAME = valueUI;
        PAUSE_FRAME = valuePause;
        Prestart.NormalFPS = valueFight;
    }

    public static void Exit()
    {
        OnDestroy ();

    }

	public static void Run()
	{
        if (startLogin && Prestart.IsReady) 
		{
			startLogin = false;
#if XGSDK
			Platform.doPlatformLogin("");
            DebugUtility.Log("GameWorld Run ThirdpartSDK login called");
#endif
        }
		for (int i = 0; i < deleteList.Count; ++i)
		{
			List<Schedule> unscheduleList =
				scheduleList.FindAll(schedule => schedule.update == deleteList[i]);
			CollectionUtility.Remove(scheduleList, unscheduleList);
		}
		deleteList.Clear();
		
		float delta = Time.deltaTime;
		
		Schedule tempSchedule;
		int len = scheduleList.Count;
		for (int i = 0; i < len; i++)
		{
			tempSchedule = scheduleList[i];
			if (tempSchedule.interval <= 0f)
				tempSchedule.update(delta);
			else if ((tempSchedule.delta += delta) >= tempSchedule.interval)
			{
				tempSchedule.update(tempSchedule.interval);
				tempSchedule.delta = tempSchedule.delta % tempSchedule.interval;
			}
		}

        TaskRefreshAndResetEveryday(delta);

		LoginAnimation.Update ();
		Net.Update();
		InputUtiltiy.Update();
        GamePlayer.Update(delta);
		ResourcesDatabase.Update ();
        Sound.Update();
		DevHelper.Update ();
	    XUI_Manager.Update();
		LogMemory();
        EveryReset();
	}

    static float updateTaskTime = 0;
    static int taskTimeCount = 20;
    static void TaskRefreshAndResetEveryday(float delta)
    {
        if(isLogin == false) return;

      //if(NextDayData.isGet == false) NextDayData.Update();

        // BodyPower.Power.Update(delta);
        updateTaskTime += delta;
        if(updateTaskTime < taskTimeCount) return;

        System.DateTime serverTime = TimeManager.serverTime;
        int h = serverTime.Hour;
        int m = serverTime.Minute;


        updateTaskTime = 0;
    }

    static bool focusStatus = false;


    static bool pauseStatus = false;
    static float m_lastCheckRestTime = 0;
    static void EveryReset()
    {
        if (Time.time - m_lastCheckRestTime < 60)
            return;
        m_lastCheckRestTime = Time.time;

        if (pauseStatus == false && focusStatus)
        {
            GamePlayer.TryResetEveryday();
        }
    }
	

    public static void Update()
    {
		switch (step) 
		{
		    case GameStartStep.init:
			    GlobalCoroutine.Start(Init());
			    step = GameStartStep.start;
			    break;
		    case GameStartStep.start:
			    if (initOver == true)
			    {
				    OnGameStart();
				    step = GameStartStep.run;
			    }
			    break;
		    case GameStartStep.run:
			    Run();
			    break;
		}
        //计算平均帧率
        AverageFrameRate();
        ServerPing.Update();
    }

    private static void AverageFrameRate()
    {
        ++tickFps;
        var time = Time.realtimeSinceStartup;
        if (timeFps < 0f)
        {
            timeFps = time;
        }
        else if (time - timeFps > 1f)
        {
            sumFps += (double)(tickFps / (time - timeFps));
            ++frameCount;
            timeFps = time;
            tickFps = 0;
        }
    }

    public static float GetAverageFrameRate()
    {
        var aFps = sumFps / frameCount;
        //Debug.Log("sumFps-->" + sumFps);
        //Debug.Log("frameCount-->" + frameCount);
        sumFps = 0;
        frameCount = 0;
        return (float)aFps;
    }


    private static void LogMemory()
    {
        if (Time.time > memoryLogTime)
        {
            memoryLogTime = Time.time + memoryLogInterval;
        }
    }

    public static void LateUpdate()
    {
    }

    public static void FixedUpdate()
    {

#if UNITY_EDITOR
        if (Default != null)
        {
            DebugUtility.enableLog = Default.enableLog;
            DebugUtility.enableLogWarning = Default.enableLogWarning;
            DebugUtility.enableLogError = Default.enableLogError;
            DebugUtility.enableLogRes = Default.enableLogRes;
        }
#endif

    }

    public static void Schedule(ScheduleDelegate update, float interval = 0f)
    {
        Schedule schedule = new Schedule();
        schedule.delta = 0f;
        schedule.update = update;
        schedule.interval = interval;
        scheduleList.Add(schedule);
    }

    public static void UnSchedule(ScheduleDelegate update)
    {
        deleteList.Add(update);
    }

    public static void OnApplicationPause(bool pauseStatus)
    {
        GameWorld.pauseStatus = pauseStatus;
        GamePause.OnApplicationPause(pauseStatus);
        //BodyPower.SyncServer();
        if (pauseStatus && !pauseUnity)
        {
            JavaSetPauseUnity(pauseUnity);
        }
    }

    
    public static void OnApplicationFocus(bool focusStatus)
    {
        GameWorld.focusStatus = focusStatus;
        GamePause.OnApplicationFocus(focusStatus);

            
#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
        // GameEventCenter.Send(GameEvent.applicationFocus, focusStatus);
		if (focusStatus)
		{
            Application.targetFrameRate = Prestart.NormalFPS;
		}
		else if (Application.targetFrameRate > 1)
		{
			Application.targetFrameRate = 30;
		}

		Net.OnApplicationFocus(focusStatus);
        //ThirdpartSDK.OnApplicationFocus(focusStatus);
#endif
    }

    public static void LoadAttributeFactor()
    {
        TableT<AttributeFactorDeploy> afdTable = TableDatabase.Load<AttributeFactorDeploy>("battle/battle_common/attributefactor");
        if (afdTable)
        {
            attributeFactorDeploy = afdTable.GetSection("attributeFactor");
        }
    }

    public static void SetSleepTimeoutSchema(bool neverSleep = true)
    {
        if (neverSleep)
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        else
            Screen.sleepTimeout = SleepTimeout.SystemSetting;
        DebugUtility.Log(string.Format("screen sleep time : {0}", Screen.sleepTimeout));
    }

	public static int GetUsedMemoryPSS()
	{
        return 0;

#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject activity = jc.GetStatic<AndroidJavaObject>("currentActivity");
		return activity.Call<int>("GetUsedMemoryPSS");
#endif
        return (int)UnityEngine.Profiling.Profiler.GetTotalReservedMemory() / 1024;
	}

    static bool pauseUnity = true;
    public static void SetPauseUnity(bool b)
    {
        pauseUnity = b;
    }
    public static void JavaSetPauseUnity(bool b)
    {
        return;
#if UNITY_ANDROID && !UNITY_EDITOR
        //AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayerLaunchActivityFunova");
       // jc.CallStatic("SetPauseUnity", b);
#endif
    }
	
	public static long GetAvailSystemMemory()
	{
        return 0;
#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		//AndroidJavaObject activity = jc.GetStatic<AndroidJavaObject>("currentActivity");
		//return activity.Call<long>("GetAvailSystemMemory") / (1024 * 1024);
#endif
		return 0;
	}
	
	public static long LogMemoryInfo(string info = "null")
	{
        DebugUtility.Log(string.Format("Memory current used: {0} MB. Avail memory: {1} MB. (info: {2})", GetUsedMemoryPSS() / 1024, GetAvailSystemMemory(), info));
		return 0;
	}
}
