using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TubeSpawner : MonoBehaviour {
    
    public int poolCount = 20;

    public float growingVelocity = 5f;
    public float rotationVelocity = 10f;

    public Arc arcPrefab;

	// Use this for initialization
	void Start () {
        PoolManager.instance.CreatePool(arcPrefab.gameObject, poolCount);
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space))
        {

            GameObject obj = PoolManager.instance.ReuseObject(arcPrefab.gameObject, Vector3.zero, Quaternion.identity);

            if (obj)
            {
                if (obj.GetComponent<PoolObject>())
                {
                    Arc arc = obj.GetComponent<Arc>();
                    //arc.startAngle = Random.Range(0f, 360.0f);
                    arc.arcAngle = 360;
                    arc.arcWidth = 2f;
                    arc.arcHeight = 100.0f;
                    arc.arcRadius = 45.0f;
                    obj.transform.rotation = new Quaternion(0, 90, 90, 0);


                    //arc.growingVelocity = growingVelocity;
                    //arc.rotationVelocity = (Random.value < 0.5) ? rotationVelocity : -rotationVelocity;

                }
            }
        }
	}
}
