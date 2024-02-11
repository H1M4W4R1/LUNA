using H1M4W4R1.LUNA.Entities;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace H1M4W4R1.LUNA.Weapons.Editor.Scripts
{
    [CustomEditor(typeof(DynamicWeapon))]
    public class DynamicWeaponEditor : WeaponEditor
    {
        [SerializeField]
        VisualTreeAsset editorAsset;
        
        public override VisualElement CreateInspectorGUI()
        {
            var weapon = (WeaponBase) target;
            
            var tree = editorAsset.CloneTree();
            var vList = tree.Q<ListView>("vectorsList");
            vList.selectionChanged += (a) =>
            {
                weapon.selectedIndex = vList.selectedIndex;
                SceneView.RepaintAll(); 
            };
            vList.itemsRemoved += (a) =>
            {
                if (weapon.selectedIndex > vList.itemsSource.Count)
                    weapon.selectedIndex = vList.itemsSource.Count - 1;
                SceneView.RepaintAll();
            };
            return tree;
        }
            
        
    }
}