using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SmartAttributes.MultiDraft.Attributes;
using UnityEditor;
using UnityEngine;

namespace SmartAttributes.MultiDraft.Editor
{
    [CustomPropertyDrawer(typeof(MultiPropertyAttribute), true)]
    public class MultiPropertyDrawer : PropertyDrawer
    {
        private List<AttributeDrawWorker> _drawWorkers;
        private FieldGUI _fieldGUI;
        private int _currentDrawer;

        public virtual AttributeDrawWorker GetDrawWorker(MultiPropertyAttribute propertyAttributeInstance)
        {
            throw new NotImplementedException("Need to override");
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            _fieldGUI = new FieldGUI(property, label, true);
            CheckAttributes();
            return -2; // Need to remove gap because using editorGUILayout and not editorGU
        }



        private void NextDrawer()
        {
            _currentDrawer++;
            if (_drawWorkers.Count <= _currentDrawer)
            {
                DrawAttribute(_fieldGUI);
                return;
            }

            _drawWorkers[_currentDrawer].DrawGUI(_fieldGUI, NextDrawer);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!_fieldGUI.NeedDraw) return;
            _currentDrawer = -1;

            NextDrawer();
        }


        private void UpdateDrawers()
        {
            var attributesDrawers = GetAttributesDrawers();

            _drawWorkers = new List<AttributeDrawWorker>();
            var customAttributes = fieldInfo.GetCustomAttributes();

            foreach (var customAttribute in customAttributes)
            {
                if (customAttribute is MultiPropertyAttribute multiAttribute)
                {
                    var drawerType = attributesDrawers[multiAttribute.GetType()];
                    var drawer = (MultiPropertyDrawer) Activator.CreateInstance(drawerType);
                    _drawWorkers.Add(drawer.GetDrawWorker(multiAttribute));
                }
            }

            _drawWorkers.Sort((a, b) => a.GetOrder().CompareTo(b.GetOrder()));
        }

        private Dictionary<Type, Type> GetAttributesDrawers()
        {
            var attributeDrawers = new Dictionary<Type, Type>();
            var drawersTypes = TypeCache.GetTypesDerivedFrom<MultiPropertyDrawer>();
            foreach (var drawerType in drawersTypes)
            {
                var attributeType = GetAttributeDrawerType(drawerType);
                attributeDrawers.Add(attributeType, drawerType);
            }

            return attributeDrawers;

            Type GetAttributeDrawerType(Type drawerType)
            {
                var customPropertyDrawerAttribute = drawerType.GetCustomAttribute<CustomPropertyDrawer>();
                var attributeTypeField = customPropertyDrawerAttribute.GetType().GetField("m_Type",
                    BindingFlags.NonPublic |
                    BindingFlags.Instance);

                var attributeType = (Type) attributeTypeField.GetValue(customPropertyDrawerAttribute);
                return attributeType;
            }
        }

        private void CheckAttributes()
        {
            if (_drawWorkers != null && _drawWorkers.Any()) return;
            UpdateDrawers();
        }

        private void DrawAttribute(FieldGUI fieldGUI)
        {
            EditorGUILayout.PropertyField(fieldGUI.Property, fieldGUI.Label, true);
        }
    }
}