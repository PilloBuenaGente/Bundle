using UnityEngine;
//==============================================================================
namespace Bundle
{
	//==============================================================================
	/// <summary> Model to use SerializableTypePropertyDrawer. </summary>
	[System.Serializable]
	public class SerializableInterfaceModel
	{
		//==============================================================================
		[SerializeField] private Object					reference	= null;
		[SerializeField] protected SerializableType		type		= null;
		//==============================================================================
		/// <summary> SerializableInterface Reference Accessor. </summary>
		public Object Reference { get { return reference; } set { reference = value; } }
		//------------------------------------------------------------------------------
		//==============================================================================
		/// <summary> SerializableInterface Type. </summary>
		public SerializableType SerializedType { get { return type; } }
		//==============================================================================
	}
	//==============================================================================
}