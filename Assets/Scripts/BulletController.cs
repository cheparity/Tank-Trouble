using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using Unity.VisualScripting;

public class BulletController : MonoBehaviour {

	private float timer = 0;
	private float perishTime = 25;
	private float speed = 0.6f;
	private bool away = true;

	void Update() {
		Move();
		//一段时间后销毁
		timer += Time.deltaTime;
		if (timer > perishTime) Destroy(this.gameObject); //后期把子弹放在缓存池中
	}

	private void OnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.name.ToLower().Contains("wall")) {
			changeDirCollideWall(other);
			//StartCoroutine("IEChangeDir", other);
		}
		away = false; //进去的时候将away改为false
	}
	private void OnCollisionStay2D(Collision2D collision) {
		away = false;//进去的时候将away改为false
	}
	private void OnCollisionExit2D(Collision2D other) {
		//出去时将away调用为true
		away = true;
	}

	public void InitDir(Vector3 initD) {
		transform.rotation = Quaternion.FromToRotation(Vector3.up, initD.normalized); //让自身y轴方向与(世界坐标)dir相同
	}

	void Move() {
		transform.Translate(Vector3.up * Time.deltaTime * speed); //朝前走   
	}

	/// <summary>
	/// 撞墙改变方向.难点:如何只改变一次???
	/// </summary>
	/// <param name="wall"></param> 墙壁
	private void changeDirCollideWall(Collision2D wall) {
		if (away == false) return;
		away = false;
		Vector2 curDir = transform.TransformDirection(Vector2.up); //前进方向转换为世界坐标方向
		var normalVec = ObtainNormalVectorByCollider(wall);
		//var normalVec = ObtainNormalVectorManually(curDir, wall.gameObject);
		Vector2 newDir = Vector2.zero;
		//Debug.Log("normal:" + normalVec);
		//Debug.Log("curDir:" + curDir);
		newDir = Vector2.Reflect(curDir, normalVec); //反射
		Quaternion rotation = Quaternion.FromToRotation(Vector2.up, newDir); //将前进方向变为新方向
																			 //Debug.Log("newDir:" + newDir);
		transform.rotation = rotation;
		away = true;
	}


	IEnumerator IEChangeDir(Collision2D wall) {
		Debug.Log("转向时away的状态:" + away);
		changeDirCollideWall(wall);
		// Func<bool> func = () =>
		//     {
		//         return away;
		//     };
		//直到物体离开才能再次调用
		yield return new WaitUntil(() => {
			return away;
		});
	}

	private Vector2 ObtainNormalVectorManually(Vector2 headDirection, GameObject other) {
		var rotAngle = GetInspectorRotationValue(other.transform).z;
		Vector2 v = Vector2.zero;
		if (rotAngle == 0) v = Vector2.up;
		else if (rotAngle == 90) v = Vector2.left;
		//如果夹角锐角则反向
		var angle = Vector2.Angle(headDirection, v);
		if (angle < 90) v = -v;
		return v;
	}


	/// <summary>
	/// 通过碰撞体的方式获取法向量.由于碰撞检测不连续,这个方法不行,但思路很好
	/// </summary>
	/// <param name="collision"></param>碰撞体
	/// <returns></returns>如果找到返回平面法向量,如果没找到返回0向量
	private Vector2 ObtainNormalVectorByCollider(Collision2D collision) {
		// ContactPoint2D contactPoint = wall.contacts[0];
		Vector2 normalVec;
		//应该遍历每一个碰撞点,找正交向量
		ContactPoint2D[] contactPoints = collision.contacts;
		foreach (var i in contactPoints) {
			//寻找单位向量
			normalVec = i.normal;
			if (normalVec.Equals(Vector2.up) || normalVec.Equals(Vector2.down) || normalVec.Equals(Vector2.left) || normalVec.Equals(Vector2.right))
				return normalVec;
		}
		//否则(多一道保障),手动查找
		normalVec = ObtainNormalVectorManually(transform.TransformDirection(Vector2.up), collision.gameObject);
		return normalVec;

	}

	/// <summary>
	/// 由于转化的原因,不能直接获取角度,需要用以下函数转化.
	/// </summary>
	/// <param name="transform"></param>需要转化角度的transform
	/// <returns></returns>转化之后的欧拉角
	public static Vector3 GetInspectorRotationValue(Transform transform) {
		System.Type transformType = transform.GetType();
		PropertyInfo m_propertyInfo_rotationOrder = transformType.GetProperty("rotationOrder", BindingFlags.Instance | BindingFlags.NonPublic);
		object m_OldRotationOrder = m_propertyInfo_rotationOrder.GetValue(transform, null);
		MethodInfo m_methodInfo_GetLocalEulerAngles = transformType.GetMethod("GetLocalEulerAngles", BindingFlags.Instance | BindingFlags.NonPublic);
		object value = m_methodInfo_GetLocalEulerAngles.Invoke(transform, new object[] { m_OldRotationOrder });
		string temp = value.ToString();
		temp = temp.Remove(0, 1);
		temp = temp.Remove(temp.Length - 1, 1);
		string[] tempVector3;
		tempVector3 = temp.Split(',');
		Vector3 vector3 = new Vector3(float.Parse(tempVector3[0]), float.Parse(tempVector3[1]), float.Parse(tempVector3[2]));
		return vector3;

	}
}


