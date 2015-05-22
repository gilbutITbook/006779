using UnityEngine;
using System.Collections;

public enum FIREBULLET
{
	ANGLE,
	HOMING,
	HOMING_Z,
	HOMING_3D,
}

public class FireBullet : MonoBehaviour {

	// === 外部パラメータ（インスペクタ表示） =====================
	public FIREBULLET 	fireType 		= FIREBULLET.HOMING;

	public float 		attackDamage 	= 1;
	public Vector2		attackNockBackVector;

	public bool			penetration		= false;

	public float 		lifeTime 		= 3.0f;
	public float 		speedV 			= 10.0f;
	public float 		speedA 			= 0.0f;
	public float 		angle			= 0.0f;

	public float		homingTime		= 0.0f;
	public float 		homingAngleV	= 180.0f;
	public float 		homingAngleA	= 20.0f;

	public Sprite 		hiteSprite;
	public Vector3		hitEffectScale  = Vector3.one;
	public float		rotateVt		= 360.0f;

	// === 外部パラメータ ======================================
	[System.NonSerialized] public Transform 	ownwer;
	[System.NonSerialized] public GameObject 	targetObject;
	[System.NonSerialized] public bool 			attackEnabled;	

	// === 内部パラメータ ======================================
	float				fireTime;
	float 				homingAngle;
	Quaternion			homingRotation;
	float 				speed;

	// === コード（Monobehaviour基本機能の実装） ================
	void Start() {
		targetObject 	= PlayerController.GetGameObject();
		
		switch (fireType) {
		case FIREBULLET.ANGLE		:
			speed = (ownwer.localScale.x < 0.0f) ? -speedV : +speedV;
			rigidbody2D.velocity = Quaternion.Euler (0.0f,0.0f,angle) * new Vector3 (speed, 0.0f, 0.0f);
			break;
		case FIREBULLET.HOMING:
		case FIREBULLET.HOMING_Z	:
		case FIREBULLET.HOMING_3D	:
			speed = speedV;
			Homing(1.0f);
			break;
		}
		
		fireTime 	 	= Time.fixedTime;
		homingAngle  	= angle;
		homingRotation 	= Quaternion.Euler (0.0f, 0.0f, angle);
		
		attackEnabled	= true;
		Destroy (this.gameObject, lifeTime);
	}

	void OnTriggerEnter2D(Collider2D other) {
		// 自分自身にヒットしないようにチェック
		if ((other.isTrigger ||
		     (ownwer.tag == "Player" && other.tag == "PlayerBody") 		 ||
		     (ownwer.tag == "Player" && other.tag == "PlayerArm")  		 ||
		     (ownwer.tag == "Player" && other.tag == "PlayerArmBullet")  ||
		     (ownwer.tag == "Enemy"  && other.tag == "EnemyBody")  		 ||
		     (ownwer.tag == "Enemy"  && other.tag == "EnemyArm")   		 ||
		     (ownwer.tag == "Enemy"  && other.tag == "EnemyArmBullet" ) )) {
			return;
		}

		// 壁アタリをチェック
		GetComponent<SpriteRenderer>().sprite = hiteSprite;
		GetComponent<SpriteRenderer>().color  = new Color(1.0f,1.0f,1.0f,0.5f);
		transform.localScale = hitEffectScale;
		Destroy (this.gameObject,0.1f);
	}

	void Update() {
		transform.Rotate (0.0f, 0.0f, Time.deltaTime * rotateVt);
	}

	void FixedUpdate() {
		if (fireType != FIREBULLET.ANGLE && (Time.fixedTime - fireTime) < homingTime) {
			Homing(Time.fixedDeltaTime);
		}
	}

	// === コード（ホーミング処理） =============================
	void Homing(float t) {
		Vector3 posTarget 	= targetObject.transform.position + new Vector3 (0.0f, 1.0f, 0.0f);

		switch(fireType) {
		case FIREBULLET.HOMING :
			{
				// 常に完璧にホーミング
				Vector3 vecTarget 		= posTarget - transform.position;
				Vector3 vecMove			= (Quaternion.LookRotation (vecTarget) * Vector3.forward) * speed;
				rigidbody2D.velocity 	= Quaternion.Euler (0.0f,0.0f,angle) * vecMove;
			}
			break;

		case FIREBULLET.HOMING_Z :
			{
				// 指定された角度でホーミング
				float 	targetAngle = Mathf.Atan2 (	posTarget.y - transform.position.y, 
				                                    posTarget.x - transform.position.x) * Mathf.Rad2Deg;
				float	deltaAngle  = Mathf.DeltaAngle(targetAngle,homingAngle);
				float 	deltaHomingAngle = homingAngleV * t;
				if (Mathf.Abs(deltaAngle) >= deltaHomingAngle) {
					homingAngle += (deltaAngle < 0.0f) ? +deltaHomingAngle : -deltaHomingAngle;
				}
				Quaternion 	rt = Quaternion.Euler (0.0f, 0.0f, homingAngle);
				rigidbody2D.velocity = (rt * Vector3.right) * speed;

				homingAngleV += (homingAngleA * t);
			}
			break;

		case FIREBULLET.HOMING_3D :
			{
				// Hexia Drive
				Vector3 	vecTag     	= posTarget - transform.position;
				Vector3 	vecFoward  	= homingRotation * Vector3.forward; 
				float 		angleDiff 	= Vector3.Angle (vecFoward,vecTag);
				float		angleAdd  	= homingAngle * t;
				Quaternion 	rotTarget 	= Quaternion.LookRotation (vecTag);
				if (angleDiff <= angleAdd) {
					homingRotation = rotTarget;
				} else {
					homingRotation = Quaternion.Slerp (homingRotation, rotTarget, angleAdd/angleDiff);
				}
				rigidbody2D.velocity = (homingRotation * Vector3.forward) * speed;
			}
			break;
		}

		speed += speedA * Time.fixedDeltaTime;
	}
}
