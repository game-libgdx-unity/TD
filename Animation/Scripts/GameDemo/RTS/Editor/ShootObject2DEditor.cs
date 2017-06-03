using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

namespace UnitedSolution
{

    [CustomEditor(typeof(ShootObject2D))]
    public class ShootObject2DInspector : BaseInspector
    {
        static private ShootObject2D shootObject;

        private static string[] typeLabel = new string[4];
        private static string[] typeTooltip = new string[4];
        private static bool init = false;


        private static void Init()
        {
            init = true;

            //public enum _ShootObjectType{Projectile, Missile, Beam, Effect, FPSRaycast, FPSDirect}
            int enumLength = Enum.GetValues(typeof(_ShootObjectType)).Length;
            typeLabel = new string[enumLength];
            typeTooltip = new string[enumLength];
            for (int i = 0; i < enumLength; i++)
            {
                typeLabel[i] = ((_ShootObjectType)i).ToString();
                if ((_ShootObjectType)i == _ShootObjectType.Projectile)
                    typeTooltip[i] = "A typical projectile, travels from turret shoot-point towards target in a 2D trajectory (no rotation in y-axis)";
                if ((_ShootObjectType)i == _ShootObjectType.Missile)
                    typeTooltip[i] = "Similar to projectile, however the trajectory are randomized and swerve around in multiple direction";
                if ((_ShootObjectType)i == _ShootObjectType.Beam)
                    typeTooltip[i] = "Used to render laser or any beam like effect. The shootObject doest move instead it requires a LineRenderer component to render the beam effect";
                if ((_ShootObjectType)i == _ShootObjectType.Effect)
                    typeTooltip[i] = "A shootObject primarily use to show various firing effect. There's no trajectory involved, the target is hit immediately. An Effect shootObject will remain at shootPoint so it can act as a shoot effect";
                if ((_ShootObjectType)i == _ShootObjectType.FPSProjectile)
                    typeTooltip[i] = "Projectile type shootObject used in First-Person-Shooter mode. Only travel in straight line. Require trigger collider and rigidbody to detect collision with in game object";
                if ((_ShootObjectType)i == _ShootObjectType.FPSProjectile)
                    typeTooltip[i] = "Beam type shootObject used in First-Person-Shooter mode. Uses a spherecast to detect if it hits target. The LineRenderer component must use local-space to work properly";
                if ((_ShootObjectType)i == _ShootObjectType.FPSProjectile)
                    typeTooltip[i] = "Effect type shootObject used in First-Person-Shooter mode. Uses a spherecast to detect if it hits target.";
            }
        }

        void Awake()
        {

            if (!init) Init();
        }




        private static bool showDefaultFlag = false;
        private static bool showLineRendererList = false;

        private GUIContent cont;
        private GUIContent[] contList;

        public override void OnInspectorGUI()
        {
            shootObject = (ShootObject2D)target;
            
            GUI.changed = false;

            int type = (int)shootObject.type;
            cont = new GUIContent("Type:", "Type of the shootObject, each shootObject type works different from another, covering various requirement");
            contList = new GUIContent[typeLabel.Length];
            for (int i = 0; i < contList.Length; i++) contList[i] = new GUIContent(typeLabel[i], typeTooltip[i]);
            type = EditorGUILayout.Popup(cont, type, contList);
            shootObject.type = (_ShootObjectType)type;

            if (shootObject.type == _ShootObjectType.Projectile || shootObject.type == _ShootObjectType.Missile || shootObject.type == _ShootObjectType.Bullet || shootObject.type == _ShootObjectType.FPSProjectile)
            {
                cont = new GUIContent("Speed:", "The travel speed of the shootObject");
                shootObject.speed = EditorGUILayout.FloatField(cont, shootObject.speed);

                cont = new GUIContent("Accel:", "The Accelaration of the shootObject");
                shootObject.accel = EditorGUILayout.FloatField(cont, shootObject.accel);
            }

            if (shootObject.type == _ShootObjectType.Projectile)
            {
                cont = new GUIContent("Max Shoot Elevation:", "The maximum elevation at which the shootObject will be fired. The firing elevation depends on the target distance. The further the target, the higher the elevation. ");
                shootObject.maxShootAngle = EditorGUILayout.FloatField(cont, shootObject.maxShootAngle);

                cont = new GUIContent("Max Shoot Range:", "The maximum range of the shootObject. This is used to govern the elevation, not the actual range limit. When a target exceed this distance, the shootObject will be fired at the maximum elevation");
                shootObject.maxShootRange = EditorGUILayout.FloatField(cont, shootObject.maxShootRange);
            }
            else if (shootObject.type == _ShootObjectType.Missile)
            {
                cont = new GUIContent("Max Shoot Angle X:", "The maximum elevation at which the shootObject will be fired. The shoot angle in x-axis will not exceed specified value.");
                shootObject.maxShootAngle = EditorGUILayout.FloatField(cont, shootObject.maxShootAngle);

                cont = new GUIContent("Max Shoot Angle Y:", "The maximum shoot angle in y-axis (horizontal).");
                shootObject.shootAngleY = EditorGUILayout.FloatField(cont, shootObject.shootAngleY);
            }
            else if (shootObject.type == _ShootObjectType.Beam || shootObject.type == _ShootObjectType.FPSBeam)
            {
                cont = new GUIContent("Beam Duration:", "The active duration of the beam");
                shootObject.beamDuration = EditorGUILayout.FloatField(cont, shootObject.beamDuration);
            }
            else if (shootObject.type == _ShootObjectType.Effect)
            {
                cont = new GUIContent("Effect delay:", "The Effect delay");
                shootObject.EffectDelay = EditorGUILayout.FloatField(cont, shootObject.EffectDelay);
            }


            cont = new GUIContent("Shoot Effect:", "The gameObject (as visual effect) to be spawn at shootPoint when the shootObject is fired");
            shootObject.shootEffect = (GameObject)EditorGUILayout.ObjectField(cont, shootObject.shootEffect, typeof(GameObject), true);

            cont = new GUIContent("Hit Effect:", "The gameObject (as visual effect) to be spawn at hit point when the shootObject hit it's target");
            shootObject.hitEffect = (GameObject)EditorGUILayout.ObjectField(cont, shootObject.hitEffect, typeof(GameObject), true);

            DrawFoldOut("Cast a spell", () =>
            {
                DrawTextfield("Spell Name", ref shootObject.spell.spellName);
                DrawMask("Custom Layer", ref shootObject.spell.customMask);
                DrawFloat("Effect range", ref shootObject.spell.effectRange);

                DrawList<SpellEffect>("Spell", "Effect", shootObject.spell.effects, se=>
                { 
                    DrawFloat("Damage", ref se.damage);
                    BeginHorizontal();
                    DrawFloat("Slow Mul", ref se.slow.slowMultiplier);
                    DrawFloat("Slow Dur", ref se.slow.duration);
                });
            });

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", GUILayout.MaxWidth(10));
            showDefaultFlag = EditorGUILayout.Foldout(showDefaultFlag, "Show default editor");
            EditorGUILayout.EndHorizontal();
            if (showDefaultFlag) DrawDefaultInspector();

            if (GUI.changed) EditorUtility.SetDirty(shootObject);

        }
    }

}