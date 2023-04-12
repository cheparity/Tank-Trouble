using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankController : MonoBehaviour {
	public GameObject tank;
	public GameObject bullet;
	private Transform firePoint;
	private Transform centerPoint;

	private int bulletCount = 0;
	// private float timer = 0;
	// private int maxBulletNum = 8;
	// private float shootGap = 15f;
	public float movingSpeed;
	public float rotSpeed;



	// Start is called before the first frame update
	void Start() {
		firePoint = transform.Find("FirePoint");
		centerPoint = transform.Find("CenterPoint");
	}

	// Update is called once per frame
	void Update() {
		Fire();
		Move();
	}

	void Fire() {
		if (Input.GetButtonDown("Fire1")) {
			var blt = Instantiate(bullet, firePoint.position, Quaternion.identity);
			//把初始方向传给子弹
			blt.GetComponent<BulletController>().InitDir(firePoint.position - centerPoint.position);
			bulletCount++;
			// timer += Time.deltaTime;
		}
		// if (timer > shootGap)//如果距离第一次发射子弹时间超过15s
		// {
		//     timer = 0;
		//     bulletCount = 0;//清零,意味着可以重新发射子弹了
		// }

	}

	void Move() {
		transform.Translate(Vector3.up * Time.deltaTime * Input.GetAxis("Vertical"), Space.Self);
		var rotAngle = -Input.GetAxis("Horizontal") * rotSpeed * Time.deltaTime;
		transform.RotateAround(centerPoint.position, new Vector3(0, 0, 1), rotAngle);
	}

	public void HitByBullet() {

	}
}
