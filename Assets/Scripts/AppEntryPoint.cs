namespace Game
{
    /*
    public class AppEntryPoint : MonoBehaviour
    {
        [SerializeField, IsntNull] GameFlow gameFlow;
        [SerializeField, IsntNull] TutorialFlow tutorialFlow;
        [SerializeField, IsntNull] Canvas mainMetaCanvas;
        [SerializeField, IsntNull] GameFactory gameFactory;
        [SerializeField, IsntNull] UpdateGamePresenter updateGamePresenter;
        [SerializeField, IsntNull] WelcomeScreen welcomeScreenPrefab;
        [SerializeField, IsntNull] RaceModesCollection raceModesCollection;
        [SerializeField, IsntNull] DebugSettings debugSettings;


        [Space]
        //[SerializeField, IsntNull] LoadingScreenView loadingScreen;
        [SerializeField, IsntNull] Sprite loadingSprites;


        static bool isInit;

        AccountData data;
        DateTime startTime;
        UnityRemoteConfigIntegration<RemoteConfigValues> config;


        void Start()
        {
            if (isInit)
            {
                DestroyImmediate(gameObject);
            }
            else
            {
                Application.targetFrameRate = 60;
                startTime = DateTime.Now;

                mainMetaCanvas.gameObject.SetActive(false);
                //loadingScreen.ShowImmediateWithSmallImage(loadingSprites);
                isInit = true;
                LoadServices(LoadRemoteConfig);
            }
        }

        async void LoadServices(UnityAction complete)
        {
            try
            {
                var options = new InitializationOptions();
#if UNITY_EDITOR
                options.SetEnvironmentName("dev");
#else
				if(Debug.isDebugBuild)
					options.SetEnvironmentName("dev");
				else
					options.SetEnvironmentName("production");
#endif
                await UnityServices.InitializeAsync(options);
                //await AnalyticsService.Instance.CheckForRequiredConsents();

                // remote config requires authentication for managing environment information
                if (!AuthenticationService.Instance.IsSignedIn)
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();// вот тут однажды из юнити 10 секунд грузилось. Походу из-за плохого интернета.
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            complete?.Invoke();
        }

        void LoadRemoteConfig()
        {
            config = new UnityRemoteConfigIntegration<RemoteConfigValues>();
            config.FetchConfigsAsync(() =>
            {
#if UNITY_EDITOR
                if (!Application.isPlaying) // Бывает, что конфиг загружается уже не в плей моде. Тогда менюшки спавнятся прямо в редакторе
                    return;
#endif
                ShowUpdateScreen();
            });
        }

        void ShowUpdateScreen()
        {
            Debug.Log($"Время инициализации сервисов = {Math.Round((DateTime.Now - startTime).TotalSeconds, 1)}");

            Version appVersion = new Version(Application.version);
            updateGamePresenter.skip += InitGame;

            string url = config.UpdateWindowsUrl;

            if (appVersion < config.MinWindowsVersion)
            {
                updateGamePresenter.Show(false, config.MaxWindowsVersion, config.NewVersionInfo, url);
            }
            else if (appVersion < config.MaxWindowsVersion)
            {
                updateGamePresenter.Show(true, config.MaxWindowsVersion, config.NewVersionInfo, url);
            }
            else
            {
                InitGame();
            }
        }

        void InitGame()
        {
            DontDestroyOnLoad(gameFlow.gameObject);

            Exception dataCorruptedException = null;
            try
            {
                data = GameFactory.Storage.GetDataSingleton();
            }
            catch (Exception e)
            {
                dataCorruptedException = e;
                Debug.LogError("DataCorruptedException");

                GameFactory.Storage.Clear();
                data = GameFactory.Storage.GetDataSingleton();
            }

            // Sounds and Music
            {
                gameFactory.Init(data.appAudioAccountData);
                AppSounds.SetAsSceneSound(gameFactory.CreateAppSounds()); // Этот AppSounds для первого запуска меты.

                AppSounds guiAppSounds = gameFactory.CreateAppSounds();
                guiAppSounds.transform.SetParent(transform); // Этот AppSounds для некоторых GUI звуков, он DestroyOnLoad
                AppSounds.SetAsDontDestroyOnLoadAppSounds(guiAppSounds);

                AppMusic appMusic = gameFactory.CreateAppMusic();
                appMusic.transform.SetParent(transform);
            }

            foreach (var raceModeInfo in raceModesCollection.collection)
                raceModeInfo.SetData(data.mainMetaData);


            GameAnalytics analytics = new GameAnalytics();
            analytics.Init(data.appAnalyticsData);
            GameAnalyticsToGameFlowAdapter.Apply(analytics, gameFlow, tutorialFlow);
            if (dataCorruptedException != null)
                analytics.AccountDataCorrupted(dataCorruptedException.ToString());

            analytics.StartApp();
            mainMetaCanvas.gameObject.SetActive(true);

            tutorialFlow.Init(data, gameFlow);
            gameFlow.Init(data, gameFactory);
            gameFlow.IsAutoPlay = debugSettings.autoPlay;

            StartGame();
            Destroy(this);
        }

        void StartGame()
        {
            bool carStartTutorial = !data.mainMetaData.isPlayerSkipOrCompleteTutorial && data.appAnalyticsData.sessionNumber < 3;
            if (carStartTutorial && !debugSettings.skipTutorial)
            {
                tutorialFlow.StartTutorial();
            }
            else
            {
                ShowWelcomeScreen();
            }
        }

        void ShowWelcomeScreen()
        {
            WelcomeScreen welcomeScreen = Instantiate(welcomeScreenPrefab);

            welcomeScreen.close += ShowMainMenu;
            welcomeScreen.Show(config.Config.welcomeScreenConfig.socialUrls);
        }

        void ShowMainMenu()
        {
            //mainMetaPresenter.gameObject.SetActive(true);
            gameFlow.StarGame();
            //loadingScreen.Hide();

            //sceneMusic.StartPlay().Forget();// Музыка в меню запускается только при запуске игры.
        }
    }
    */
}