using H1M4W4R1.LUNA.Entities;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace H1M4W4R1.LUNA.Weapons.Editor.Scripts
{
    [CustomEditor(typeof(DynamicWeapon))]
    public class DynamicWeaponEditor : UnityEditor.Editor
    {
        [SerializeField]
        VisualTreeAsset editorAsset;
        
        public override VisualElement CreateInspectorGUI() =>
            editorAsset.CloneTree();
            
        
    }
}