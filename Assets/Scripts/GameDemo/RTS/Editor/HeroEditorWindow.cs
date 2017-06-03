using UnityEngine;
using UnityEditor;

using System;
using System.Collections;
using System.Collections.Generic;

namespace UnitedSolution
{
    public class HeroEditorWindow : BaseEditorWindow<HeroEditorWindow, UnitHero>
    {
        protected override void Initialize()
        {
            TowerDB towerDBPrefab = TowerDB.LoadDB();
            List<UnitTower> towers = towerDBPrefab.towerList;

            foreach (UnitTower tower in towers)
            {
                if (tower.IsHero())
                {
                    if (tower.stats.Count == 0)
                    {
                        tower.stats.Add(new UnitStat());
                    }
                    units.Add(tower as UnitHero);
                }
            }
        }

        protected override void OnGUI()
        {
            base.OnGUI();
            Space();
            BeginHorizontal();
            DrawObject("Icon:", SelectedUnit.iconSprite);
            DrawTextfield("Name:", ref SelectedUnit.unitName);
            EndHorizontal();


        }
    }
}