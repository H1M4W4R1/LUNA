using H1M4W4R1.LUNA.Entities;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace H1M4W4R1.LUNA.Weapons.Editor.Scripts
{
    [CustomEditor(typeof(Hitbox))]
    public class HitboxEditor : UnityEditor.Editor
    {
        [SerializeField]
        VisualTreeAsset editorAsset;

        public override VisualElement CreateInspectorGUI()
        {
            var tree = editorAsset.CloneTree();
            var eField = tree.Q<Label>("events");
            var index = tree.IndexOf(eField);
            
            var onHitEventField = new PropertyField(serializedObject.FindProperty(nameof(Hitbox.onHit)), "On Hit");
            if (index + 1 < tree.childCount - 1)
                tree.Insert(index + 1, onHitEventField);
            else
                tree.Add(onHitEventField);
            
            return tree;
        }


    }
}