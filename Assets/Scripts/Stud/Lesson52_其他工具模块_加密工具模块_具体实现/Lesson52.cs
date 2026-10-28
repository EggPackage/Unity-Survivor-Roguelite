using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer
{
    private int hp;
    private int atk;

    private int key;

    public TestPlayer()
    {
        key = EncryptionUtil.GetRandomKey();
    }

    public int HP
    {
        get
        {
            return EncryptionUtil.UnLoackValue(hp, key);
        }
        set
        {
            hp = EncryptionUtil.LockValue(value, key);
        }
    }
}

public class Lesson52 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #region 知识点一 了解我们将如何使用加密工具模块
        //举例说明
        #endregion

        #region 知识点二 实现加密工具模块
        //主要提供以下方法：
        //1.获取随机密钥 方法
        //2.加密数据 方法
        //3.解密数据 方法
        #endregion

        #region 知识点三 测试加密工具模块
        TestPlayer player = new TestPlayer();
        player.HP = 100;
        print(player.HP);
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
