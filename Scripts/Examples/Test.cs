using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bundle;

public class Test : MonoBehaviour
{
	public SerializedITestInterface t1 = default;

	public SerializableInterface<ITestInterface> t2 = default; //UNITY 2020 Feature

	private void OnEnable()
	{
		t1.Interface.Foo();
	}
}
