using UnityEngine;
using System.Collections;

public class ArcController : MonoBehaviour {

	public int poolCount = 20;

	public float growingVelocity = 5f;
	public float rotationVelocity = 10f;

	public Arc arcPrefab;

	void Start () {
	
		PoolManager.instance.CreatePool(arcPrefab.gameObject, poolCount);

	}
	
	// Update is called once per frame
	void Update () {
	
		if (Input.GetKeyDown(KeyCode.Space)) {

			GameObject obj = PoolManager.instance.ReuseObject(arcPrefab.gameObject, Vector3.zero, Quaternion.identity);

			if (obj) {
				if (obj.GetComponent<PoolObject>()) {
					Arc arc = obj.GetComponent<Arc>();
					//arc.startAngle = Random.Range(0f, 360.0f);
					arc.arcAngle = Random.Range(0, 360);
					arc.arcWidth = 2f;

					arc.growingVelocity = growingVelocity;
					arc.rotationVelocity = (Random.value < 0.5) ? rotationVelocity : -rotationVelocity;

				}
			}
		}
	}
}
