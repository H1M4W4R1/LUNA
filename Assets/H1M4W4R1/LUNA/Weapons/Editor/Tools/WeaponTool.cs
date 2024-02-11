using Codice.Client.Common;
using Unity.Mathematics;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

namespace H1M4W4R1.LUNA.Weapons.Editor.Tools
{
    [EditorTool("Weapon Editor", typeof(WeaponBase))]
    public class WeaponTool : EditorTool
    {
        public override void OnToolGUI(EditorWindow window)
        {
            var weapon = (WeaponBase) target;
            var vRotation = weapon.transform.rotation;
            
            Handles.BeginGUI();
            using (new GUILayout.HorizontalScope())
            {
                using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    var vectors = weapon.GetVectors();
                    var vIndex = weapon.selectedIndex;

                    // Check if index is correct 
                    if (vIndex < vectors.Count && vIndex >= 0)
                    {
                        GUILayout.Label($"Current Vector Index: {vIndex}");
                        GUILayout.Label($"Current Vector Direction: {vectors[vIndex].GetVectorForRotation(vRotation)}");
                    }
                }

                GUILayout.FlexibleSpace();
            }
            Handles.EndGUI();
        }
    }
}
