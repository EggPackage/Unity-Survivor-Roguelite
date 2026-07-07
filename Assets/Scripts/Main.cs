using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestData : IPoolObject
{
    public int i;
    public string str;
    public object o;

    public void ResetInfo()
    {
        i = 0;
        str = "";
        o = null;
    }
}

public class Main : MonoBehaviour
{
    public RawImage img;

    // Start is called before the first frame update
    void Start()
    {
        TestMgr.Instance.TestLog();

        Test2Mgr2.Instance.TestLog();

        //破坏单例模式唯一性 我们能够自己去new对应的单例模式类对象 这样是不安全
        //TestMgr t = new TestMgr();
        //BaseManager<TestMgr> t2 = new BaseManager<TestMgr>();

        ResMgr.Instance.LoadAsync<GameObject>("Test", TestFun);
        Debug.Log(ResMgr.Instance.GetRefCount<GameObject>("Test"));

        ResMgr.Instance.LoadAsync<GameObject>("Test", TestFun);
        Debug.Log(ResMgr.Instance.GetRefCount<GameObject>("Test"));

        ResMgr.Instance.UnloadAsset<GameObject>("Test", false, TestFun);
        Debug.Log(ResMgr.Instance.GetRefCount<GameObject>("Test"));

        ResMgr.Instance.UnloadAsset<GameObject>("Test", false, TestFun);
        Debug.Log(ResMgr.Instance.GetRefCount<GameObject>("Test"));

        //Instantiate(ResMgr.Instance.Load<GameObject>("Test"));
        //Debug.Log(ResMgr.Instance.GetRefCount<GameObject>("Test"));

        //Instantiate(EditorResMgr.Instance.LoadEditorRes<GameObject>("Cube"));

        //Coroutine c = StartCoroutine(TestFun());
        //StopCoroutine(c);
        //AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/PC/test");

        //ABMgr.Instance.LoadResAsync<GameObject>("test", "Cube", (obj) =>
        //{
        //    GameObject gameObj = Instantiate(obj);
        //    gameObj.name = "ABmgrCreateObj";
        //});

        //ABMgr.Instance.LoadResAsync<GameObject>("test", "Cube", (obj) =>
        //{
        //    GameObject gameObj = Instantiate(obj);
        //    gameObj.name = "ABmgrCreateObj";
        //}, true);

        //ABMgr.Instance.LoadResAsync<GameObject>("test", "Cube", (obj) =>
        //{
        //    GameObject gameObj = Instantiate(obj);
        //    gameObj.name = "ABmgrCreateObj";
        //});

        //UWQResMgr.Instance.LoadRes<string>("file://" + Application.streamingAssetsPath + "/test.txt", (str) =>
        //{
        //    print("加载的文本信息" + str);
        //}, () =>
        //{
        //    print("加载是失败");
        //});

        //UWQResMgr.Instance.LoadRes<byte[]>("file://" + Application.streamingAssetsPath + "/test.txt", (bytes) =>
        //{
        //    print("加载的2进制字节数组信息" + bytes.Length);
        //}, () =>
        //{
        //    print("加载是失败");
        //});

        //UWQResMgr.Instance.LoadRes<Texture>("file://" + Application.streamingAssetsPath + "/基础小框架封面480.png", (tex) =>
        //{
        //    img.texture = tex;
        //}, () =>
        //{
        //    print("加载是失败");
        //});

        //UWQResMgr.Instance.LoadRes<AssetBundle>("file://" + Application.streamingAssetsPath + "/test", (ab) =>
        //{
        //    print("ab包加载成功，名字是：" + ab.name);
        //}, () =>
        //{
        //    print("加载是失败");
        //});

        ABResMgr.Instance.LoadResAsync<GameObject>("test", "Cube", (res) =>
        {
            GameObject obj = Instantiate(res);
            obj.name = "ABResMgr创建的对象";
        });

        //同一帧调用显示某一个面板
        UIMgr.Instance.ShowPanel<BeginPanel>(E_UILayer.Middle, (panel) =>
        {
            print("第一次显示");
        }, false);

        UIMgr.Instance.HidePanel<BeginPanel>();

        UIMgr.Instance.ShowPanel<BeginPanel>(E_UILayer.Middle, (panel) =>
        {
            print("第二次显示");
        }, true);

        UIMgr.Instance.GetPanel<BeginPanel>((panel) =>
        {
            print("获取面板要处理的逻辑");
        });

        InputMgr.Instance.StartOrCloseInputMgr(true);

        InputMgr.Instance.ChangeKeyboardInfo(E_EventType.E_Input_Skill1, KeyCode.J, InputInfo.E_InputType.Down);
        InputMgr.Instance.ChangeKeyboardInfo(E_EventType.E_Input_Skill2, KeyCode.K, InputInfo.E_InputType.Up);
        InputMgr.Instance.ChangeMouseInfo(E_EventType.E_Input_Skill3, 0, InputInfo.E_InputType.Down);


        EventCenter.Instance.AddEventListener(E_EventType.E_Input_Skill1, Skill1);
        EventCenter.Instance.AddEventListener(E_EventType.E_Input_Skill2, Skill2);
        EventCenter.Instance.AddEventListener(E_EventType.E_Input_Skill3, Skill3);

        EventCenter.Instance.AddEventListener<float>(E_EventType.E_Input_Horizontal, CheckHorizontal);
    }

    private void Skill1()
    {
        print("技能1触发");
    }
    private void Skill2()
    {
        print("技能2触发");
    }
    private void Skill3()
    {
        print("技能3触发");
    }

    private void CheckHorizontal(float v)
    {
        print(v);
    }


    //IEnumerator TestFun()
    //{
    //    AssetBundleCreateRequest req = AssetBundle.LoadFromFileAsync(Application.streamingAssetsPath + "/PC/test");
    //    yield return req;
    //    print(req.assetBundle.name);
    //}

    private void TestFun(GameObject obj)
    {
        Instantiate(obj);
    }

    private float v;
    private float oldV;


    private AudioSource source;
    private void OnGUI()
    {
        if(GUILayout.Button("播放背景音乐"))
        {
            MusicMgr.Instance.PlayBKMusic("Begin");
        }

        if (GUILayout.Button("播放背景音乐2"))
        {
            MusicMgr.Instance.PlayBKMusic("Fight");
        }

        if (GUILayout.Button("停止音乐"))
        {
            MusicMgr.Instance.StopBKMusic();
        }

        if (GUILayout.Button("暂停音乐"))
        {
            MusicMgr.Instance.PauseBKMusic();
        }

        v = GUILayout.HorizontalSlider(v, 0, 1);

        if(oldV != v)
        {
            oldV = v;
            MusicMgr.Instance.ChangeBKMusicValue(v);
            MusicMgr.Instance.ChangeSoundValue(v);
        }

        if (GUILayout.Button("播放音效"))
        {
            MusicMgr.Instance.PlaySound("hit_5");
            MusicMgr.Instance.PlaySound("hit_4", true, false, (source) =>
            {
                this.source = source;
            });
        }

        if (GUILayout.Button("停止音效"))
        {
            MusicMgr.Instance.StopSound(this.source);
            this.source = null;
        }

        if (GUILayout.Button("暂停音效"))
        {
            MusicMgr.Instance.PlayOrPauseSound(false);
        }

        if (GUILayout.Button("回复播放音效"))
        {
            MusicMgr.Instance.PlayOrPauseSound(true);
        }
    }

    private TestData data;

    private int timerID;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Test6Mgr.Instance.ICanUpdateAndCoroutine();

        if (Input.GetKeyUp(KeyCode.Space))
            Test6Mgr.Instance.ICanStopUpdateAndCoroutine();

        if (Input.GetKeyDown(KeyCode.X))
        {
            GameObject.Find("Monster").GetComponent<Monster>().Dead();
            EventCenter.Instance.EventTrigger(E_EventType.E_Test);
        }


        if (Input.GetKeyDown(KeyCode.S))
        {
            UIMgr.Instance.ShowPanel<BeginPanel>(E_UILayer.System, (panel) => {
                panel.TestFun();
            });
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            UIMgr.Instance.HidePanel<BeginPanel>(false);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            print("开始改建，获取输入信息了");
            InputMgr.Instance.GetInputInfo((inputInfo) =>
            {
                if(inputInfo.keyOrMouse == InputInfo.E_KeyOrMouse.Key)
                {
                    print("键盘输入：" + inputInfo.key);
                    InputMgr.Instance.ChangeKeyboardInfo(E_EventType.E_Input_Skill1, inputInfo.key, InputInfo.E_InputType.Down);
                }
                else
                {
                    print("鼠标输入：" + inputInfo.mouseID);
                    InputMgr.Instance.ChangeMouseInfo(E_EventType.E_Input_Skill1, inputInfo.mouseID, InputInfo.E_InputType.Down);
                }
            });
        }

        if(Input.GetKeyDown(KeyCode.O))
        {
            data = PoolMgr.Instance.GetObj<TestData>();
            data.i = 11;
            data.o = new object();
            data.str = "123";
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            PoolMgr.Instance.PushObj<TestData>(data);
            data = null;
        }

        if(Input.GetKeyDown(KeyCode.V))
        {
            //timerID = 
            TimerMgr.Instance.CreateTimer(false, 5000, () =>
            {
                print("5s计时结束");
            }, 1000, ()=> {
                print("每间隔1s会打印信息");
            });

            TimerMgr.Instance.CreateTimer(true, 5000, () =>
            {
                print("不受timeScele影响的：5s计时结束");
            }, 1000, () => {
                print("不受timeScele影响的：每间隔1s会打印信息");
            });
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            //TimerMgr.Instance.RemoveTimer(timerID);
            Time.timeScale = 0;
        }

        #region 缓存池测试
        if (Input.GetMouseButtonDown(0))
        {
            GameObject obj = PoolMgr.Instance.GetObj("Test/Cube");
            obj.transform.position = Vector3.zero;
        }

        if(Input.GetMouseButtonDown(1))
        {
            PoolMgr.Instance.GetObj("Test/Sphere");
        }
        #endregion
    }
}
