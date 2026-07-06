using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson10 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #region 思考题
        //现在我们确定最大容量是通过在获取时传入参数
        //若传入参数出错可能会导致“超上限”
        //能否优化下，以其它思路去制作，让我们可以更加方便的处理上限逻辑
        #endregion

        #region 解题思路
        //1.让使用者不用每次设置上限值
        //2.初始化抽屉时，第一次就直接定好上限为多少，之后直接在内部判断即可

        //让缓存池对象挂载一个用于配置上限值的脚本
        //只需要在制作预设体时，将脚本挂好，设置好上限即可
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
