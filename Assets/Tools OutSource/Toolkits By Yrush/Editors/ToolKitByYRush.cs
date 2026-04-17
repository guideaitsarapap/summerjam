using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ToolKitByYRush : EditorWindow
{
    // Ref: AJ. Banyapon Poolsawas

    private Texture2D banner;

    private int tabs = 3;
    string[] tabOptions = new string[] { "Create Object", "Create Tools", "etc" };

    [MenuItem("Tools/Toolkit By Yrush/Open Panel")]
    public static void ShowWindow()
    {
        GetWindow<ToolKitByYRush>("Toolkit By Yrush");
    }

    void OnEnable()
    {
        // If you want banner input "bannerForToolKit.jpg" in Resources files First
        //banner = Resources.Load<Texture2D>("banner");
    }

    void OnGUI()
    {
        GUILayout.Space(10);

        if (banner != null)
        {
            GUILayout.Label(banner, GUILayout.Height(100));
        }

        EditorGUILayout.HelpBox("Hello This Tool is Ref(or copy) from AJ. Banyapon Poolsawas at github", MessageType.Info);

        GUILayout.Space(10);

        tabs = GUILayout.Toolbar(tabs, tabOptions);

        switch (tabs)
        {
            case 0:
                FirstTab();
                break;
            case 1:
                SecondTab();
                break;
            case 2:
                ThirdTab();
                break;
        }


    }

    #region FunctionsHere

    public static void CreatePlayer2DPlatformer()
    {
        GameObject player2DPlatformer = new GameObject("Player2DPlatformerBasic");
        GameObject groundCheck = new GameObject("groundCheck");

        groundCheck.transform.position += new Vector3(0, -0.5f, 0);
        groundCheck.transform.SetParent(player2DPlatformer.transform);

        SpriteRenderer SR = player2DPlatformer.AddComponent<SpriteRenderer>();
        Texture2D texture = new Texture2D(1, 1);
        texture.name = "SquareFake";
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();

        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 1f);
        SR.sprite = sprite;
        player2DPlatformer.transform.localScale = new Vector3(1f, 1f, 1);

        Rigidbody2D rb = player2DPlatformer.AddComponent<Rigidbody2D>();
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.gravityScale = 4f;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        BoxCollider2D boxColldier2d = player2DPlatformer.AddComponent<BoxCollider2D>();

        PlayerMovement2DPlatformerBasic playermovement = player2DPlatformer.AddComponent<PlayerMovement2DPlatformerBasic>();
        playermovement.AddComponentToScriptsAtCreate(rb,groundCheck.transform);
    }

    public static void CreatePlayer2DTopDown()
    {
        GameObject player2DTopDown = new GameObject("Player2DTopDownBasic", typeof(PlayerMovement2DTopDownBasic));

        SpriteRenderer SR = player2DTopDown.AddComponent<SpriteRenderer>();
        Texture2D texture = new Texture2D(1, 1);
        texture.name = "SquareFake";
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();

        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 1f);
        SR.sprite = sprite;
        player2DTopDown.transform.localScale = new Vector3(1f, 1f, 1);

        Rigidbody2D rb = player2DTopDown.AddComponent<Rigidbody2D>();
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

    }

    public static void CreatePlayer3DFirstPerson()
    {
        GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        Object.DestroyImmediate(player.GetComponent<CapsuleCollider>(),true);
        player.name = "Player3DFirstPersonBasic";
        CharacterController characterControllerPlayer = player.AddComponent<CharacterController>();
        PlayerMovement3DFirstPersonBasic playermovement3DFPSsctipt = player.AddComponent<PlayerMovement3DFirstPersonBasic>();

        GameObject Eye = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Object.DestroyImmediate(Eye.GetComponent<BoxCollider>(),true);
        Eye.name = "Eye";
        Eye.transform.localScale = new Vector3(0.8f, 0.25f, 0.5f);
        Eye.transform.position = new Vector3(0, 0.5f, 0.3f);
        Eye.transform.SetParent(player.transform);

        GameObject cameraFirstPerson = new GameObject("CameraFirstPerson");
        cameraFirstPerson.transform.position = new Vector3(0, 0.5f, 0.2f);
        Camera cameraCam = cameraFirstPerson.AddComponent<Camera>();
        AudioListener audioListenerCamera = cameraFirstPerson.AddComponent<AudioListener>();
        UniversalAdditionalCameraData UniversalAddCameraData = cameraFirstPerson.AddComponent<UniversalAdditionalCameraData>();
        cameraFirstPerson.transform.SetParent(player.transform);

        GameObject GroundCheck = new GameObject("GroundCheck");
        GroundCheck.transform.position = new Vector3(0, -1.2f, 0);
        GroundCheck.transform.SetParent(player.transform);

        playermovement3DFPSsctipt.CreateSetOnlyUseWhenCreate(cameraFirstPerson.transform, GroundCheck.transform);

    }

    public static void CreatePlayer3DThirdPerson()
    {
        GameObject player = GameObject.CreatePrimitive(PrimitiveType.Cube);
        player.name = "Player";
        player.tag = "Player";
        player.transform.position = new Vector3(0f, 1.0f, 0f);

        Rigidbody rb = player.AddComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        rb.useGravity = false;

        //player.AddComponent<PlayerMovement2DPlatformer>();

        BoxCollider col1 = player.GetComponent<BoxCollider>();
        BoxCollider col2 = player.AddComponent<BoxCollider>();
        col2.isTrigger = true;

        Debug.LogWarning("This is not finish yet");
    }

    public static void CreateSoundManager()
    {

        GameObject soundManager = new GameObject("SoundManager", typeof(SoundManager));
        SoundManager _soundM = soundManager.GetComponent<SoundManager>();


        GameObject sfx = new GameObject("SFXAudioSource", typeof(AudioSource));
        AudioSource sfxAudio = sfx.GetComponent<AudioSource>();
        sfxAudio.playOnAwake = false;
        sfx.transform.SetParent(soundManager.transform, false);


        GameObject music = new GameObject("MusicAudioSource", typeof(AudioSource));
        AudioSource musicAudio = music.GetComponent<AudioSource>();
        musicAudio.playOnAwake = false;
        musicAudio.loop = true;
        music.transform.SetParent(soundManager.transform, false);


        GameObject ambient = new GameObject("AmbientAudioSource", typeof(AudioSource));
        AudioSource ambientAudio = ambient.GetComponent<AudioSource>();
        ambientAudio.playOnAwake = false;
        ambientAudio.loop = true;
        ambient.transform.SetParent(soundManager.transform, false);

        _soundM.audioSFXSource = sfxAudio;
        _soundM.audioMusicSource = musicAudio;
        _soundM.audioAmbientSource = ambientAudio;

    }

    #endregion



    #region Tab1

    public static void FirstTab()
    {
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.BeginVertical(GUILayout.Width(250)); // Adjust width for you

        EditorGUILayout.LabelField("Create Objects", EditorStyles.boldLabel);

        if (GUILayout.Button("▶ Create Player 2D Platformer Basic", GUILayout.ExpandWidth(true)))
            CreatePlayer2DPlatformer();
        if (GUILayout.Button("▶ Create Player 2D Top-Down Basic", GUILayout.ExpandWidth(true)))
            CreatePlayer2DTopDown();
        if (GUILayout.Button("▶ Create Player 3D First-Person Basic", GUILayout.ExpandWidth(true)))
            CreatePlayer3DFirstPerson();
        GUILayout.EndVertical();
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

    }


   


    #endregion

    #region Tab2

    public static void SecondTab()
    {
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.BeginVertical(GUILayout.Width(250)); // Adjust width for you

        EditorGUILayout.LabelField("Create Tools", EditorStyles.boldLabel);

        if (GUILayout.Button("▶ Create SoundManager", GUILayout.ExpandWidth(true)))
            CreateSoundManager();


        GUILayout.EndVertical();
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

    }


    


    #endregion

    #region Tab3

    public static void ThirdTab()
    {
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.BeginVertical(GUILayout.Width(250)); // Adjust width for you


        GUILayout.EndVertical();
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

    }

    #endregion





}
