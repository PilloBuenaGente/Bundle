using UnityEngine;
//==============================================================================
namespace Bundle
{
	//==============================================================================
	[System.Serializable]
	/// <summary> Type Serialization. </summary>
	public class SerializableType
	{
		//==============================================================================
		/// <summary> Empty serializable Type. </summary>
		public static readonly SerializableType empty = new SerializableType(null);
		//------------------------------------------------------------------------------
		public const string NULL = "Null";
		//==============================================================================
#pragma warning disable 0414
		[SerializeField]
		private string	m_name;
		[SerializeField]
		private bool	m_isArray;
		[SerializeField]
		private bool	m_isDirty;
#pragma warning restore 0414

		[SerializeField]
		private string	m_assemblyQualifiedName;
		//------------------------------------------------------------------------------
		private System.Type m_type;
		//==============================================================================
		/// <summary> Init or Get SerializableType. </summary>
		public System.Type Type
		{
			get
			{
#if UNITY_EDITOR
				if(m_isDirty)
				{
					m_isDirty = false;
					m_type = System.Type.GetType(m_assemblyQualifiedName, false, false);
				}
				else
#endif
				if(m_type == null && ! string.IsNullOrEmpty(m_assemblyQualifiedName))
				{
					m_type = System.Type.GetType(m_assemblyQualifiedName, false, false);
				}
				return m_type;
			}
			set
			{
				m_type = value;
				m_assemblyQualifiedName = (value == null) ? string.Empty : value.AssemblyQualifiedName;
			}
		}
		//------------------------------------------------------------------------------
		/// <summary> Override ToString. </summary>
		//------------------------------------------------------------------------------
		/// <summary> Get type's name. </summary>
		public override string ToString()
		{
			return m_type.Name;
		}
		//------------------------------------------------------------------------------
		/// <summary> Get type's fullname. </summary>
		public string FullName
		{
			get { return m_type == null ? "" : m_type.FullName; }
		}
		//==============================================================================
		/// <summary> Return System.Type. </summary>
		public static implicit operator System.Type(SerializableType type)
		{
			return type.Type;
		}
		//==============================================================================
		/// <summary> Constructor init Type. </summary>
		public SerializableType(System.Type type, bool isArray = false)
		{
			if(type != null)
			{
				m_isArray = false;
				m_type = type;
				m_name = type.Name;
				m_assemblyQualifiedName = type.AssemblyQualifiedName;
			}
			else
			{
				m_isArray = false;
				m_type = null;
				m_name = NULL;
				m_assemblyQualifiedName = string.Empty;
			}
			m_isArray = isArray;
		}
		//------------------------------------------------------------------------------
		/// <summary> Check Types equality by Object. </summary>
		public override bool Equals(object obj)
		{
			SerializableType temp = (SerializableType)obj;
			return temp.Type != Type;
		}
		//------------------------------------------------------------------------------
		/// <summary> Return Hash of Type. </summary>
		public override int GetHashCode()
		{
			return Type.GetHashCode();
		}
		//------------------------------------------------------------------------------
		/// <summary> Check Types equality. </summary>
		public static bool operator ==(SerializableType first, SerializableType second)
		{
			if(System.Object.ReferenceEquals(first, second))
			{
				return true;
			}
			if(((object)first == null) || ((object)second == null))
			{
				return false;
			}
			return first.Equals(second);
		}
		//------------------------------------------------------------------------------
		/// <summary> Check Types difference. </summary>
		public static bool operator !=(SerializableType first, SerializableType second)
		{
			return !(first == second);
		}
		//==============================================================================
	}
	//==============================================================================
}