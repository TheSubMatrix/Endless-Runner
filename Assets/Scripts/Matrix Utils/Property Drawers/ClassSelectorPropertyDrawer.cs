#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MatrixUtils.Extensions;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using MatrixUtils.Attributes;
using MatrixUtils.PropertyDrawers.Helpers;
using UnityEngine;

namespace MatrixUtils.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(ClassSelectorAttribute))]
    public class ClassSelectorPropertyDrawer : PropertyDrawer
    {
        ClassSelectorAttribute m_attributeData;
        bool m_isManagedReference;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            m_attributeData = attribute as ClassSelectorAttribute;
            m_isManagedReference = property.propertyType == SerializedPropertyType.ManagedReference;

            // Infer base type from field if not provided
            Type baseType = m_attributeData?.Type;
            if (baseType == null)
            {
                property.GetFieldInfoAndStaticType(out Type staticType);
                baseType = staticType;
            }

            if (baseType == null)
            {
                return CreateErrorBox("Could not determine base type", $"property: {property.propertyPath}");
            }

            // Validate base type
            string validationError = ValidateBaseType(baseType, m_isManagedReference);
            if (validationError != null)
            {
                return CreateErrorBox(validationError, $"Type: {baseType.Name}, Property: {property.propertyPath}");
            }

            return !m_isManagedReference
                ? CreateConcreteTypeUI(property)
                : CreatePolymorphicTypeUI(property, baseType);
        }

        static string ValidateBaseType(Type baseType, bool isManagedReference)
        {
            if (typeof(UnityEngine.Object).IsAssignableFrom(baseType))
            {
                if (isManagedReference)
                {
                    return "[ClassSelector] cannot be used with UnityEngine.Object types on [SerializeReference] fields.\n" +
                           "Unity objects cannot be serialized as managed references.\n" +
                           "Remove [ClassSelector] or use a non-Unity object type.";
                }

                return "[ClassSelector] is not needed for UnityEngine.Object types.\n" +
                       "Unity already provides object pickers for these types.\n" +
                       "Remove [ClassSelector] attribute.";
            }

            if (baseType.IsInterface && isManagedReference)
            {
                return null; // Valid
            }

            if (baseType.IsValueType)
            {
                return "[ClassSelector] cannot be used with value types (structs).\n" +
                       "ClassSelector is designed for reference types only.\n" +
                       "Consider using a class instead of a struct.";
            }

            if (baseType.IsGenericTypeDefinition)
            {
                return "[ClassSelector] cannot be used with open generic types.\n" +
                       "Use a closed generic type (e.g., MyClass<int> instead of MyClass<T>).";
            }

            if (baseType.IsAbstract && baseType.IsSealed)
            {
                return "[ClassSelector] cannot be used with static classes.\n" +
                       "Static classes cannot be instantiated.";
            }

            if (isManagedReference && baseType.IsAbstract)
            {
                return null; // Valid
            }

            if (!baseType.IsAbstract && !baseType.IsInterface)
            {
                bool hasParameterlessConstructor = baseType.GetConstructor(
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                    null, Type.EmptyTypes, null) != null;

                if (!hasParameterlessConstructor)
                {
                    return "[ClassSelector] requires types to have a parameterless constructor.\n" +
                           $"Type '{baseType.Name}' does not have a parameterless constructor.\n" +
                           $"Add a constructor: public {baseType.Name}() {{ }}";
                }
            }

            if (baseType.Namespace == null || !baseType.Namespace.StartsWith("System") || baseType.IsInterface)
                return null;

            if (baseType != typeof(string) &&
                baseType != typeof(Uri) &&
                !baseType.IsGenericType)
            {
                return "[ClassSelector] should not be used with System types.\n" +
                       $"Type '{baseType.FullName}' is a framework type that may not serialize correctly.";
            }

            return null;
        }

        static VisualElement CreateErrorBox(string message, string details)
        {
            VisualElement container = new() { style = { marginTop = 2, marginBottom = 2 } };

            HelpBox errorBox = new(message, HelpBoxMessageType.Error);
            container.Add(errorBox);

            if (string.IsNullOrEmpty(details)) return container;

            Label detailsLabel = new(details)
            {
                style =
                {
                    fontSize = 10,
                    color = new(Color.gray),
                    marginLeft = 4,
                    marginTop = 2,
                    whiteSpace = WhiteSpace.Normal
                }
            };
            container.Add(detailsLabel);

            return container;
        }

        static VisualElement CreateConcreteTypeUI(SerializedProperty property)
        {
            VisualElement root = new() { style = { marginTop = 2, marginBottom = 2 } };

            Foldout foldout = new()
            {
                text = ObjectNames.NicifyVariableName(property.name),
                value = property.isExpanded
            };

            foldout.RegisterValueChangedCallback(evt =>
            {
                property.isExpanded = evt.newValue;
                property.serializedObject.ApplyModifiedProperties();
            });

            VisualElement propertiesContainer = new()
            {
                style = { paddingLeft = 15, marginTop = 4 }
            };

            foldout.Add(propertiesContainer);
            root.Add(foldout);
            DrawerOptions options = new()
            {
                ExcludeDrawerType = typeof(ClassSelectorPropertyDrawer)
            };
            PropertyDrawerVisualElementFactory.CreateUIInContainer(property, propertiesContainer, options);
            return root;
        }

        static VisualElement CreatePolymorphicTypeUI(SerializedProperty property, Type baseType)
        {
            VisualElement root = new() { style = { marginTop = 2, marginBottom = 2 } };

            Foldout foldout = new()
            {
                text = ObjectNames.NicifyVariableName(property.name),
                value = property.isExpanded
            };

            foldout.RegisterValueChangedCallback(evt =>
            {
                property.isExpanded = evt.newValue;
                property.serializedObject.ApplyModifiedProperties();
            });

            DropdownField dropdown = new()
            {
                name = "TypeSelectionDropdown",
                style = { marginBottom = 4, marginLeft = 0 }
            };

            VisualElement propertiesContainer = new()
            {
                name = "ObjectProperties",
                style = { paddingLeft = 15, marginTop = 4 }
            };

            foldout.Add(dropdown);
            foldout.Add(propertiesContainer);
            root.Add(foldout);

            // Get derived types (filter for instantiable types)
            List<Type> derivedTypes = PropertyDrawerRegistry.GetDerivedTypes(baseType, includeBaseType: !baseType.IsAbstract)
                .Where(IsTypeInstantiable)
                .ToList();

            if (derivedTypes.Count == 0)
            {
                VisualElement warningContainer = new();
                warningContainer.Add(new HelpBox(
                    $"No valid instantiable types found that derive from {baseType.Name}.\n" +
                    "Derived types must have parameterless constructors and cannot be Unity objects.",
                    HelpBoxMessageType.Warning
                ));
                foldout.Add(warningContainer);
                return root;
            }

            Dictionary<string, Type> typesByName = derivedTypes.ToDictionary(t => t.Name, t => t);

            List<string> choices = new() { "None" };
            choices.AddRange(typesByName.Keys.OrderBy(name => name));
            dropdown.choices = choices;
            dropdown.SetValueWithoutNotify("None");

            // Handle type selection
            dropdown.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue == "None")
                {
                    property.managedReferenceValue = null;
                    property.serializedObject.ApplyModifiedProperties();
                    propertiesContainer.Clear();
                    return;
                }

                if (!typesByName.TryGetValue(evt.newValue, out Type selectedType)) return;

                try
                {
                    property.managedReferenceValue = Activator.CreateInstance(selectedType);
                    property.serializedObject.ApplyModifiedProperties();

                    propertiesContainer.Clear();
                    DrawManagedReferenceFields(property, propertiesContainer);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Failed to create instance of {selectedType.Name}: {ex.Message}");
                    propertiesContainer.Clear();
                    propertiesContainer.Add(new HelpBox(
                        $"Failed to instantiate {selectedType.Name}. Check that it has a parameterless constructor.",
                        HelpBoxMessageType.Error
                    ));
                }
            });
            object currentValue = GetCurrentManagedReferenceValue(property);

            if (currentValue == null)
            {
                dropdown.SetValueWithoutNotify("None");
                return root;
            }

            Type selectedType = currentValue.GetType();
            string typeName = selectedType.Name;

            if (!dropdown.choices.Contains(typeName)) return root;

            dropdown.SetValueWithoutNotify(typeName);
            DrawManagedReferenceFields(property, propertiesContainer);

            return root;
        }

        static bool IsTypeInstantiable(Type type)
        {
            if (typeof(UnityEngine.Object).IsAssignableFrom(type)) return false;
            if (type.IsAbstract) return false;
            if (type.IsInterface) return false;
            if (type.IsValueType) return false;
            if (type.IsGenericTypeDefinition) return false;

            bool hasParameterlessConstructor = type.GetConstructor(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null, Type.EmptyTypes, null) != null;

            return hasParameterlessConstructor;
        }

        static void DrawManagedReferenceFields(SerializedProperty property, VisualElement container)
        {
            SerializedProperty iterator = property.Copy();
            SerializedProperty endProperty = property.GetEndProperty();

            if (!iterator.NextVisible(true)) return;
            do
            {
                if (SerializedProperty.EqualContents(iterator, endProperty))
                    break;

                SerializedProperty childProperty = iterator.Copy();
                PropertyDrawer childDrawer = PropertyDrawerFactory.CreateDrawerForProperty(
                    childProperty,
                    excludeDrawerType: typeof(ClassSelectorPropertyDrawer),
                    excludeAttributeDrawers: true
                );

                VisualElement customElement = childDrawer?.CreatePropertyGUI(childProperty);
                if (customElement != null)
                {
                    container.Add(customElement);
                    continue;
                }

                container.Add(new PropertyField(childProperty));

            } while (iterator.NextVisible(false));
        }

        static object GetCurrentManagedReferenceValue(SerializedProperty property)
        {
            object currentValue = property.managedReferenceValue;

            if (currentValue != null) return currentValue;

            // Check for the auto-property backing field
            SerializedProperty backingField = property.serializedObject.FindProperty(
                $"<{property.name}>k__BackingField");

            if (backingField == null) return currentValue;

            currentValue = backingField.managedReferenceValue;
            if (currentValue != null)
            {
                property.managedReferenceValue = currentValue;
            }

            return currentValue;
        }
    }
}
#endif