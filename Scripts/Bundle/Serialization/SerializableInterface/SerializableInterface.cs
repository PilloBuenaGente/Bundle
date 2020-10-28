using Bundle;
using UnityEngine;
//==============================================================================
namespace Bundle
{
	//==============================================================================
	/// <summary> Class to override with interfaces. </summary>
	[System.Serializable]
	public class SerializableInterface<INTERFACE> : SerializableInterfaceModel where INTERFACE : class
	{
		//==============================================================================
		protected SerializableInterface()
		{
			type = new SerializableType(typeof(INTERFACE));
		}
		//==============================================================================
		/// <summary> Get SerializableInterface Interface. </summary>
		public INTERFACE Interface
		{
			get { return Reference as INTERFACE; }
		}
		//==============================================================================
	}
	//==============================================================================
}