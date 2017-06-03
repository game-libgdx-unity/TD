using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace UnitedSolution
{

    [CustomEditor(typeof(Unit))]
    public class UnitInspector : BaseInspector
    {
        protected virtual void DrawStat(UnitStat stat)
        {
            DrawMask("Target Mask:", ref stat.customMask);
            DrawFloat("Damage min:", ref stat.damageMin);
            DrawFloat("Damage max:", ref stat.damageMax);
            DrawFloat("Attack range:", ref stat.attackRange);
            DrawFloat("AOE radius:", ref stat.aoeRadius);
            //DrawGameObject<ShootObject>("Shoot Object:", ref 
            DrawListObject<ShootObject>("ShootObjects:", "SO", stat.shootObjects);
        }
        protected void DrawUnitGUI()
        {
            Unit unit = (Unit)target;
            DrawFoldOut("Unit Essential:", () =>
            {
                if (DrawToggle("Set Custom Layer: ", ref unit.useCustomLayer))
                {
                    DrawLayer("Custom Layer", ref unit.customLayer);
                }
                DrawTextfield("Unit name:", ref unit.unitName);
                DrawTransform("Target point:", unit.targetPoint);
                unit.hitedEffect = DrawGameObject("Hitted Effect:", unit.hitedEffect);
                DrawFloat("Hitpoint:", ref unit.HP);
                DrawFloat("Max HP:", ref unit.fullHP);
                DrawFloat("Shield:", ref unit.shield);
                DrawSlider("Hit Threshold:", ref unit.hitThreshold, .2f, .5f);
                unit.behaviour = (Behaviour)EditorGUILayout.EnumPopup("Behaviour: ", unit.behaviour);
            });

            DrawFoldOut("Attack Behavior:", () =>
            {
                DrawFloat("Detect range: ", ref unit.detectRange);
                bool showFoldOut = unit.stats.Count > 1;
                if (showFoldOut)
                {
                    for (int i = 0; i < unit.stats.Count; i++)
                    {
                        UnitStat stat = unit.stats[i];
                        DrawFoldOut("Stat " + i, () =>
                        {
                            DrawStat(stat);
                        });
                    }
                }
                else
                {
                    DrawStat(unit.stats[0]);
                }

                showFoldOut = unit.shootPoints.Count > 0;
                if (showFoldOut)
                {
                    for (int i = 0; i < unit.shootPoints.Count; i++)
                    {
                        Transform sp = unit.shootPoints[i];
                        unit.shootPoints[i] = DrawTransform("ShootPoint " + i, sp);
                    }
                }
                else
                {
                    DrawStat(unit.stats[0]);
                }
            });
        }
    }
}
