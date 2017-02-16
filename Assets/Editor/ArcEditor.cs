using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof (Arc))]
public class ArcEditor : Editor {

	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();

		Arc arc = target as Arc;
		if (GUILayout.Button("Randomize Mesh")) {

			arc.DrawArcMesh ();
		}
		arc.DrawArcMesh ();
	}

}
