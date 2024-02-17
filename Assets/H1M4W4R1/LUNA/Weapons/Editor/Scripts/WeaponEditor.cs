using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace H1M4W4R1.LUNA.Weapons.Editor.Scripts
{
    public class WeaponEditor : UnityEditor.Editor
    {
        public void OnSceneGUI()
        {
            var weapon = (WeaponBase) target;

            // Efficiency is king
            var wTransform = weapon.transform;
            var wPosition = (float3) wTransform.position;
            var wRotation = (quaternion) wTransform.rotation;
            var wScale = (float3) wTransform.lossyScale;
            
            // Draw vector gizmos
            var vIndex = 0;
            var vectors = weapon.GetVectors();
            for (var index = 0; index < vectors.Count; index++)
            {
                var vector = vectors[index];
                
                // Prepare dynamic variables
                var cPoint = wPosition + vector.GetStartPoint(wRotation, wScale);
                var cQuat = vector.vectorRotation;
                if(cQuat.value is {x:0, y:0, z:0, w:0})
                    cQuat = quaternion.identity;

                // Compute vector handles
                if (vIndex == weapon.selectedIndex && UnityEditor.Tools.current == Tool.Custom)
                {
                    Handles.color = Color.green;
                    
                    // Compute handles and transform object if necessary
                    EditorGUI.BeginChangeCheck();
                    cPoint = Handles.PositionHandle(cPoint, cQuat);
                    cQuat = Handles.RotationHandle(cQuat, cPoint);
                   
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(weapon, "Modify weapon vector data");
                        
                        // Update vector data
                        var vClone = vectors[vIndex];
                        vClone.SetStartPoint(wRotation, wScale, cPoint - wPosition);
                        vClone.vectorRotation = cQuat;

                        // Update vectors information
                        vectors[vIndex] = vClone;
                        vector = vClone;

                        DrawArrow(cPoint, vector.GetVectorForRotation(wRotation));
                    }
                    else
                    {
                        DrawArrow(cPoint, vector.GetVectorForRotation(wRotation));
                    }
                }
                else
                {
                    Handles.color = Color.white;
                    DrawArrow(cPoint, vector.GetVectorForRotation(wRotation));
                }
                
                vIndex++;
            }
        }
        
        // NOTE: Do not use Unity.Mathematics, it will fu**-up the arrow
        private static void DrawArrow(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
        {
            var ePos = pos + direction;
            Handles.DrawLine(pos, ePos);
       
            var right = (Quaternion.LookRotation(direction) * Quaternion.Euler(0,180+arrowHeadAngle,0) * new Vector3(0,0,1));
            var left = (Quaternion.LookRotation(direction) * Quaternion.Euler(0,180-arrowHeadAngle,0) * new Vector3(0,0,1));
            Handles.DrawLine(ePos, ePos + right * arrowHeadLength);
            Handles.DrawLine(ePos, ePos + left * arrowHeadLength);
        }
        
    }
}