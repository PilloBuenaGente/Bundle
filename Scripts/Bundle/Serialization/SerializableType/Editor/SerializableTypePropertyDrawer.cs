using UnityEngine;
using UnityEditor;
//==============================================================================
namespace Bundle
{
	//==============================================================================
	using Type = System.Type;
	//==============================================================================
	/// <summary> Display serializableType name and allow dropping Objects. </summary>
	[CustomPropertyDrawer(typeof(SerializableType))]
	public class SerializableTypePropertyDrawer : PropertyDrawer
	{
		//==============================================================================
		private static readonly Type[] types =
		{
			null,
			typeof(void),
			typeof(bool),
			typeof(int),
			typeof(float),
			typeof(string),
			typeof(MonoBehaviour),
			typeof(Transform),
			typeof(Vector2),
			typeof(Vector3),
		};
		//==============================================================================
		private static string[] typesName = null;
		//==============================================================================
		private DefaultAsset	asset;
		//------------------------------------------------------------------------------
		private const float		spacing						= 3f;
		private const float		basicTypesPopupWidth		= 20f;
		private const float		isArrayToggleWidth			= 20f;
		private const float		selectFromProjectWidth		= 35f;
		//------------------------------------------------------------------------------
		private const string	isArrayToggleLabel			= "A";
		//------------------------------------------------------------------------------
		private const string	assemblyQualifiedNameKey	= "m_assemblyQualifiedName";
		private const string	isArrayKey					= "m_isArray"; 
		private const string	nameKey						= "m_name";
		private const string	isDirty						= "m_isDirty";
		//==============================================================================
		static SerializableTypePropertyDrawer()
		{
			typesName = new string[types.Length];
			for(int index = types.Length - 1; index >= 0; --index)
			{
				typesName[index] = (types[index] == null) ? SerializableType.NULL : types[index].Name;
			}
		}
		//==============================================================================
		/// <summary> Overrided OnGUI method from Editor. </summary>
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);

			SerializedProperty isArrayProperty = property.FindPropertyRelative(isArrayKey);
			bool isArray = isArrayProperty.boolValue;

			position = EditorGUI.PrefixLabel(position, label);
			EditorGUI.indentLevel = 0;

			float rightPartWith = spacing + isArrayToggleWidth + spacing + basicTypesPopupWidth;
			Rect typeFieldRect	= new Rect(position.x, position.y, position.width - rightPartWith, position.height);

			DrawTypeField(typeFieldRect, property, isArray);
			DrawSelectFromProjectField(typeFieldRect, property, isArray);

			Rect tempRect	= new Rect(position.x + position.width - rightPartWith + spacing, position.y, rightPartWith, position.height);
			tempRect.width	= basicTypesPopupWidth;

			DrawBasicTypesPopupList(tempRect, property, isArray);

			UpdateElementRect(isArrayToggleWidth, ref tempRect);

			DrawIsArrayToggleField(tempRect, property, isArrayProperty,  isArray);
			
			EditorGUI.EndProperty();
		}
		//==============================================================================
		private void UpdateElementRect(float elementWidth, ref Rect rect)
		{
			rect.xMin += rect.width + spacing;
			rect.width = elementWidth;
		}
		//------------------------------------------------------------------------------
		private void DrawSelectFromProjectField(Rect fieldRect, SerializedProperty property, bool isArray)
		{
			fieldRect.xMin += fieldRect.width - selectFromProjectWidth;
			MonoScript newObj = EditorGUI.ObjectField(fieldRect, null, typeof(MonoScript), false) as MonoScript;
			if(newObj != null)
			{
				ChangeProperties(property, newObj.GetClass(), isArray);
			}
		}
		//------------------------------------------------------------------------------
		private void DrawIsArrayToggleField(Rect fieldRect, SerializedProperty property, SerializedProperty isArrayProperty, bool isArray)
		{
			bool newIsArray = GUI.Toggle(fieldRect, isArray, isArrayToggleLabel, "Button");
			if(newIsArray != isArray)
			{
				isArrayProperty.boolValue = newIsArray;
				System.Type type = System.Type.GetType(property.FindPropertyRelative(assemblyQualifiedNameKey).stringValue, false, false);
				ChangeProperties(property, type, newIsArray);
			}
		}
		//------------------------------------------------------------------------------
		private void DrawTypeField(Rect fieldRect, SerializedProperty property, bool isArray)
		{
			GUI.Box(fieldRect, GUIContent.none, EditorStyles.textField);
			DropAreaGUI(fieldRect, property, isArray);

			SerializedProperty typeName = property.FindPropertyRelative(nameKey);
			if(typeName == null)
			{
				return;
			}
			EditorGUI.LabelField(fieldRect, typeName.stringValue);
		}
		//------------------------------------------------------------------------------
		private void DrawBasicTypesPopupList(Rect position, SerializedProperty property, bool isArray)
		{
			int selection = EditorGUI.Popup(position, -1, typesName);
			if(selection != -1)
			{
				ChangeProperties(property, types[selection], isArray);
			}
		}
		//------------------------------------------------------------------------------
		private void DropAreaGUI(Rect position, SerializedProperty property, bool isArray)
		{
			Event evt = Event.current;
			if(! ((evt.type == EventType.DragPerform || evt.type == EventType.DragUpdated) && position.Contains(evt.mousePosition)))
			{
				return;
			}

			DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
			if(evt.type == EventType.DragPerform && DragAndDrop.objectReferences.Length == 1)
			{
				DragAndDrop.AcceptDrag();
				Object obj = DragAndDrop.objectReferences[0];

				Type type = obj.GetType();

				if(type == typeof(MonoScript))
				{
					MonoScript script = obj as MonoScript;
					type = script.GetClass();
				}

				ChangeProperties(property, type, isArray);
			}
		}
		//------------------------------------------------------------------------------
		private void ChangeProperties(SerializedProperty property, Type type, bool isArray)
		{
			if(type != null)
			{
				if(isArray && !type.IsArray)
				{
					type = type.MakeArrayType();
				}
				else if(!isArray && type.IsArray)
				{
					type = type.GetElementType();
				}
			}
			var name = property.FindPropertyRelative(nameKey);
			var assemblyQualifiedNameProp = property.FindPropertyRelative(assemblyQualifiedNameKey);
			var isDirtyProp = property.FindPropertyRelative(isDirty);

			name.stringValue = (type == null) ? SerializableType.NULL : type.Name;
			assemblyQualifiedNameProp.stringValue = (type == null) ? string.Empty : type.AssemblyQualifiedName;
			isDirtyProp.boolValue = true;
		}
		//==============================================================================
	}
	//==============================================================================
}
