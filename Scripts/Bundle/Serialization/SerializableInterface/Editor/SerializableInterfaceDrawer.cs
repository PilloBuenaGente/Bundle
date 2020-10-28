using UnityEngine;
using UnityEditor;
using Object	= UnityEngine.Object;
using Convert	= System.Convert;
using System.Reflection;
//==============================================================================
namespace Bundle
{
	//==============================================================================
	/// <summary> SerializableInterfaceModel Drawer. </summary>
	[CustomPropertyDrawer(typeof(SerializableInterfaceModel), true)]
	public class SerializableInterfaceDrawer : PropertyDrawer
	{
		private Object newReference = null;
		//==============================================================================
		/// <summary> Override OnGUI. </summary>
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			object obj = property.serializedObject.targetObject;

			FieldInfo field = null;
			foreach (var path in property.propertyPath.Split('.'))
			{
				var type = obj.GetType();
				field = type.GetField(path, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Default | BindingFlags.Instance);
				if(field == null) { continue; }
				obj = field.GetValue(obj);
			}

			SerializableInterfaceModel	interf	= null;
			if (obj.GetType().IsArray)
			{
				SerializableInterfaceModel[] array = (SerializableInterfaceModel[])obj;
				int index = Convert.ToInt32(property.propertyPath.Split('[')[1].Split(']')[0]);
				if(array.Length <= index) { return; }
				interf = ((SerializableInterfaceModel[])obj)[index];
			}
			else
			{
				interf = obj as SerializableInterfaceModel;
			}

			Draw(position, property, label, interf);

			if(interf.Reference != newReference)
			{
				Undo.RecordObject(property.serializedObject.targetObject, "Serialized interface reference changed");
				interf.Reference = newReference;
				EditorUtility.SetDirty(property.serializedObject.targetObject);
			}
		}
		//------------------------------------------------------------------------------
		/// <summary> Override GetPropertyHeight. </summary>
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUIUtility.singleLineHeight;
		}
		//==============================================================================
		private void Draw(Rect position, SerializedProperty property, GUIContent label, SerializableInterfaceModel interf)
		{
			EditorGUI.BeginProperty(position, label, property);
			position = EditorGUI.PrefixLabel(position, label);
			EditorGUI.indentLevel = 0;
			DrawInterfaceField(position, interf);
			DropAreaGUI(position, property, interf);
			EditorGUI.EndProperty();
		}
		//------------------------------------------------------------------------------
		private void DrawInterfaceField(Rect position, SerializableInterfaceModel interf)
		{
			newReference = EditorGUI.ObjectField(position, interf.Reference, interf.SerializedType.Type, true);
		}
		//------------------------------------------------------------------------------
		private void DropAreaGUI(Rect position, SerializedProperty property, SerializableInterfaceModel interf)
		{
			if (DragAndDrop.objectReferences.Length != 1) { return; }

			Object	obj				= DragAndDrop.objectReferences[0];
			Event	currentEvent	= Event.current;

			if (obj is GameObject)
			{
				GameObject gameObject = obj as GameObject;
				if(gameObject.GetComponent(interf.SerializedType.Type) == null) { return; }
				if (!IsDropable(interf, currentEvent, position)) { return; }

				DragAndDrop.AcceptDrag();
				newReference = gameObject.GetComponent(interf.SerializedType.Type);
			}
			else
			{
				if (!interf.SerializedType.Type.IsSubclassOf(obj.GetType())) { return; }
				if (!IsDropable(interf, currentEvent, position)) { return; }

				DragAndDrop.AcceptDrag();
			}
		}
		//------------------------------------------------------------------------------
		private bool IsDropable(SerializableInterfaceModel interf, Event currentEvent, Rect position)
		{
			if (!(((currentEvent.type == EventType.DragUpdated) || (currentEvent.type == EventType.DragPerform))
			&& (position.Contains(currentEvent.mousePosition)))) { return false; }

			DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
			if (currentEvent.type != EventType.DragPerform) { return false; }

			return true;
		}
		//==============================================================================
	}
	//==============================================================================
}