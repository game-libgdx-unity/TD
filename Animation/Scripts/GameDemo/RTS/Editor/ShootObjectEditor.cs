using UnityEngine;
using UnityEditor;

using System;

using System.Collections;

using UnitedSolution;

namespace UnitedSolution
{

    [CustomEditor(typeof(ShootObject))]
    public class ShootObjectInspector : Editor
    {

        static private ShootObject instance;

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

            instance = (ShootObject)target;

            GUI.changed = false;

            int type = (int)instance.type;
            cont = new GUIContent("Type:", "Type of the shootObject, each shootObject type works different from another, covering various requirement");
            contList = new GUIContent[typeLabel.Length];
            for (int i = 0; i < contList.Length; i++) contList[i] = new GUIContent(typeLabel[i], typeTooltip[i]);
            type = EditorGUILayout.Popup(cont, type, contList);
            instance.type = (_ShootObjectType)type;

            if (instance.type == _ShootObjectType.Projectile || instance.type == _ShootObjectType.Missile || instance.type == _ShootObjectType.FPSProjectile)
            {
                cont = new GUIContent("Speed:", "The travel speed of the shootObject");
                instance.speed = EditorGUILayout.FloatField(cont, instance.speed);

                cont = new GUIContent("Accel:", "The Accelaration of the shootObject");
                instance.accel = EditorGUILayout.FloatField(cont, instance.accel);
            }

            if (instance.type == _ShootObjectType.Projectile)
            {
                cont = new GUIContent("Max Shoot Elevation:", "The maximum elevation at which the shootObject will be fired. The firing elevation depends on the target distance. The further the target, the higher the elevation. ");
                instance.maxShootAngle = EditorGUILayout.FloatField(cont, instance.maxShootAngle);

                cont = new GUIContent("Max Shoot Range:", "The maximum range of the shootObject. This is used to govern the elevation, not the actual range limit. When a target exceed this distance, the shootObject will be fired at the maximum elevation");
                instance.maxShootRange = EditorGUILayout.FloatField(cont, instance.maxShootRange);
            }
            else if (instance.type == _ShootObjectType.Missile)
            {
                cont = new GUIContent("Max Shoot Angle X:", "The maximum elevation at which the shootObject will be fired. The shoot angle in x-axis will not exceed specified value.");
                instance.maxShootAngle = EditorGUILayout.FloatField(cont, instance.maxShootAngle);

                cont = new GUIContent("Max Shoot Angle Y:", "The maximum shoot angle in y-axis (horizontal).");
                instance.shootAngleY = EditorGUILayout.FloatField(cont, instance.shootAngleY);
            }
            else if (instance.type == _ShootObjectType.Beam || instance.type == _ShootObjectType.FPSBeam)
            {
                cont = new GUIContent("Beam Duration:", "The active duration of the beam");
                instance.beamDuration = EditorGUILayout.FloatField(cont, instance.beamDuration);

                cont = new GUIContent("AutoSearchForLineRenderer:", "Check to let the script automatically search for all the LineRenderer component on the prefab instead of assign it manually");
                instance.autoSearchLineRenderer = EditorGUILayout.Toggle(cont, instance.autoSearchLineRenderer);

                if (!instance.autoSearchLineRenderer)
                {
                    cont = new GUIContent("LineRenderers", "The LineRenderer component in this prefab\nOnly applicable when AutoSearchForLineRenderer is unchecked");

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("", GUILayout.MaxWidth(10));
                    showLineRendererList = EditorGUILayout.Foldout(showLineRendererList, cont);
                    EditorGUILayout.EndHorizontal();

                    if (showLineRendererList)
                    {
                        cont = new GUIContent("LineRenderers:", "The LineRenderer component on the prefab to be controlled by the script");
                        float listSize = instance.lineList.Count;
                        listSize = EditorGUILayout.FloatField("    Size:", listSize);

                        //~ if(!EditorGUIUtility.editingTextField && listSize!=instance.lineList.Count){
                        if (listSize != instance.lineList.Count)
                        {
                            while (instance.lineList.Count < listSize) instance.lineList.Add(null);
                            while (instance.lineList.Count > listSize) instance.lineList.RemoveAt(instance.lineList.Count - 1);
                        }

                        for (int i = 0; i < instance.lineList.Count; i++)
                        {
                            instance.lineList[i] = (LineRenderer)EditorGUILayout.ObjectField("    Element " + i, instance.lineList[i], typeof(LineRenderer), true);
                        }
                    }
                }
            }
            else if (instance.type == _ShootObjectType.Effect)
            {
                cont = new GUIContent("Effect delay:", "The Effect delay");
                instance.EffectDelay = EditorGUILayout.FloatField(cont, instance.EffectDelay);
            }

            if (instance.type == _ShootObjectType.FPSBeam || instance.type == _ShootObjectType.FPSEffect)
            {
                cont = new GUIContent("Sphere Cast Radius:", "The radius of the SphereCast used to detect target hit. The bigger the value, the easier to hit a target");
                instance.hitRadius = EditorGUILayout.FloatField(cont, instance.hitRadius);
            }


            cont = new GUIContent("Shoot Effect:", "The gameObject (as visual effect) to be spawn at shootPoint when the shootObject is fired");
            instance.shootEffect = (GameObject)EditorGUILayout.ObjectField(cont, instance.shootEffect, typeof(GameObject), true);

            cont = new GUIContent("Hit Effect:", "The gameObject (as visual effect) to be spawn at hit point when the shootObject hit it's target");
            instance.hitEffect = (GameObject)EditorGUILayout.ObjectField(cont, instance.hitEffect, typeof(GameObject), true);

            //cont = new GUIContent("Start Ab ID:", "The Ability of the shootObject when start hit");
            instance.Ab_Start_Holder = GetAbility("Start Ability:", instance.Ab_Start_Holder);
            instance.Ab_End_Holder = GetAbility("End Ability:", instance.Ab_End_Holder);

            instance.penetratable = EditorGUILayout.Toggle("Penetratable:", instance.penetratable);
            if (instance.penetratable)
            {
                //instance.bouncing_max_targets = EditorGUILayout.LayerField("Layer", instance.bouncing_max_targets);
                instance.penetration_distance = EditorGUILayout.FloatField("Distance:", instance.penetration_distance);
                instance.penetration_max_targets = EditorGUILayout.IntField("Max Targets:", instance.penetration_max_targets);
            }

            instance.multiShoot = EditorGUILayout.Toggle("Multishoot:", instance.multiShoot);
            if (instance.multiShoot)
            {
                instance.multishot_delay = EditorGUILayout.FloatField("MultiShoot Delay:", instance.multishot_delay);
                instance.multishot_max_targets = EditorGUILayout.IntField("Max Targets:", instance.multishot_max_targets);
            }

            instance.bouncing = EditorGUILayout.Toggle("Bouncing:", instance.bouncing);
            if (instance.bouncing)
            {
                instance.bouncing_radius = EditorGUILayout.FloatField("Distance:", instance.bouncing_radius);
                instance.bouncingDelay = EditorGUILayout.FloatField("Bouncing Delay:", instance.bouncingDelay);
                instance.bouncing_max_targets = EditorGUILayout.IntField("Max Targets:", instance.bouncing_max_targets);
            }

            //instance.effectSetting = (EffectSettings) EditorGUILayout.ObjectField("Effect Setting:", instance.effectSetting, typeof(EffectSettings),true );

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            showDefaultFlag = EditorGUILayout.Foldout(showDefaultFlag, "Show default editor");
            EditorGUILayout.EndHorizontal();
            if (showDefaultFlag) DrawDefaultInspector();

            if (GUI.changed) EditorUtility.SetDirty(instance);

        }

        public static AbilityBehavior GetAbility(string label, AbilityBehavior Ab_holder)
        {
            GameObject go = (GameObject)EditorGUILayout.ObjectField(label, Ab_holder == null ? null : Ab_holder.gameObject, typeof(GameObject));
            if (go)
            {
                AbilityBehavior holder = go.GetComponent<AbilityBehavior>();
                if (holder)
                {
                    Ab_holder = holder;
                    return Ab_holder;
                }
            }

            return null;
        }
    }

}