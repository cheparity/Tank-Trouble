using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRebound : MonoBehaviour
{


    // private void OnTriggerEnter2D(Collider2D other)
    // {
    //     if (other.name.ToLower().Contains("bullet"))
    //     {
    //         Transform bullet = other.GetComponent<Transform>();
    //         //更改bullet的弹道
    //         Vector2 oldDir = bullet.gameObject.transform.TransformDirection(Vector2.up);
    //         var normalVec = ObtainNormalVectorManually(oldDir);
    //         var newDir = Vector2.Reflect(oldDir, normalVec);
    //         bullet.rotation = Quaternion.FromToRotation(Vector2.up, newDir);
    //     }
    // }


    /// <summary>
    /// 获得碰撞法向量,如果失败则返回零向量
    /// </summary>
    /// <param name="headDirection"></param>被碰物体前进方向
    /// <returns></returns>碰撞法向量,如果失败则返回零向量
    // private Vector2 ObtainNormalVectorManually(Vector2 headDirection)
    // {
    //     var rotAngle = transform.rotation.z;
    //     Vector2 v = Vector2.zero;
    //     switch (rotAngle)
    //     {
    //         //水平
    //         case 0:
    //             v = Vector2.up;
    //             break;
    //         case 90: //竖直
    //             v = Vector2.left;
    //             break;
    //     }
    //     //如果夹角锐角则反向
    //     var angle = Vector2.Angle(headDirection, v);
    //     if (angle < 90) v = -v;
    //     return v;
    // }
}
