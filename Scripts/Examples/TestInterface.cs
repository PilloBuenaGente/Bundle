using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bundle;

public interface ITestInterface
{
	void Foo();
}

[System.Serializable]
public class SerializedITestInterface : SerializableInterface<ITestInterface> { }

public class TestInterface : MonoBehaviour, ITestInterface
{
	public void Foo()
	{
		Debug.Log("Foo");
	}
}
