using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace ImportedScenes.PurlySnowman_A04_20260427_Copy.Editor
{
    public static class PurlyAnimationBuilder
    {
        private const string RootPath = "Assets/ImportedScenes/PurlySnowman_A04_20260427_Copy/Animations";
        private const string IdleClipPath = RootPath + "/Idle.anim";
        private const string WalkRightClipPath = RootPath + "/Walk.anim";
        private const string WalkLeftClipPath = RootPath + "/WalkLeft.anim";
        private const string JumpClipPath = RootPath + "/Jump.anim";
        private const string ControllerPath = RootPath + "/PurlyAnimator.controller";

        [InitializeOnLoadMethod]
        private static void BuildOnLoad()
        {
            EditorApplication.delayCall += EnsureAnimationsExist;
        }

        [MenuItem("Tools/Purly/Rebuild Animations")]
        public static void EnsureAnimationsExist()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            DirectoryUtility.EnsureFolderExists("Assets/ImportedScenes/PurlySnowman_A04_20260427_Copy/Editor");

            AnimationClip idle = CreateIdleClip();
            AnimationClip walkRight = CreateWalkRightClip();
            AnimationClip walkLeft = CreateWalkLeftClip();
            AnimationClip jump = CreateJumpClip();

            SaveClip(idle, IdleClipPath);
            SaveClip(walkRight, WalkRightClipPath);
            SaveClip(walkLeft, WalkLeftClipPath);
            SaveClip(jump, JumpClipPath);

            CreateController(idle, walkRight, walkLeft, jump);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static AnimationClip CreateIdleClip()
        {
            AnimationClip clip = NewClip("Idle", true, 0.8f);

            SetLocalPositionY(clip, "Body_Middle", Keys((0f, -0.257f), (0.4f, -0.235f), (0.8f, -0.257f)));
            SetLocalPositionY(clip, "Head", Keys((0f, 0.473f), (0.4f, 0.505f), (0.8f, 0.473f)));
            SetLocalPositionY(clip, "Scarf", Keys((0f, 0.134f), (0.4f, 0.145f), (0.8f, 0.134f)));

            return clip;
        }

        private static AnimationClip CreateWalkRightClip()
        {
            AnimationClip clip = NewClip("Walk", true, 0.6f);

            SetPosition(clip, "LeftLeg",
                Keys((0f, -0.41045982f), (0.15f, -0.43f), (0.3f, -0.39f), (0.45f, -0.4f), (0.6f, -0.41045982f)),
                Keys((0f, -1.7782948f), (0.15f, -1.72f), (0.3f, -1.66f), (0.45f, -1.75f), (0.6f, -1.7782948f)),
                Keys((0f, -0.05195633f), (0.6f, -0.05195633f)));

            SetPosition(clip, "RightLeg",
                Keys((0f, 0.60548025f), (0.15f, 0.62f), (0.3f, 0.56f), (0.45f, 0.58f), (0.6f, 0.60548025f)),
                Keys((0f, -1.8753389f), (0.15f, -1.82f), (0.3f, -1.94f), (0.45f, -1.88f), (0.6f, -1.8753389f)),
                Keys((0f, 0.101387955f), (0.6f, 0.101387955f)));

            SetPosition(clip, "LeftHand",
                Keys((0f, -0.83060324f), (0.15f, -0.85f), (0.3f, -0.79f), (0.45f, -0.81f), (0.6f, -0.83060324f)),
                Keys((0f, -0.60908157f), (0.15f, -0.6f), (0.3f, -0.67f), (0.45f, -0.62f), (0.6f, -0.60908157f)),
                Keys((0f, -0.17130628f), (0.6f, -0.17130628f)));

            SetPosition(clip, "RightHand",
                Keys((0f, 0.9383498f), (0.15f, 0.97f), (0.3f, 0.91f), (0.45f, 0.94f), (0.6f, 0.9383498f)),
                Keys((0f, -0.62977576f), (0.15f, -0.65f), (0.3f, -0.57f), (0.45f, -0.61f), (0.6f, -0.62977576f)),
                Keys((0f, 0.08835077f), (0.6f, 0.08835077f)));

            SetLocalPositionY(clip, "Body_Middle", Keys((0f, -0.257f), (0.15f, -0.23f), (0.3f, -0.257f), (0.45f, -0.23f), (0.6f, -0.257f)));
            SetLocalPositionY(clip, "Head", Keys((0f, 0.473f), (0.15f, 0.515f), (0.3f, 0.473f), (0.45f, 0.515f), (0.6f, 0.473f)));

            return clip;
        }

        private static AnimationClip CreateWalkLeftClip()
        {
            AnimationClip clip = NewClip("WalkLeft", true, 0.6f);

            SetPosition(clip, "LeftLeg",
                Keys((0f, -0.39f), (0.15f, -0.4f), (0.3f, -0.41045982f), (0.45f, -0.43f), (0.6f, -0.39f)),
                Keys((0f, -1.66f), (0.15f, -1.75f), (0.3f, -1.7782948f), (0.45f, -1.72f), (0.6f, -1.66f)),
                Keys((0f, -0.05195633f), (0.6f, -0.05195633f)));

            SetPosition(clip, "RightLeg",
                Keys((0f, 0.56f), (0.15f, 0.58f), (0.3f, 0.60548025f), (0.45f, 0.62f), (0.6f, 0.56f)),
                Keys((0f, -1.94f), (0.15f, -1.88f), (0.3f, -1.8753389f), (0.45f, -1.82f), (0.6f, -1.94f)),
                Keys((0f, 0.101387955f), (0.6f, 0.101387955f)));

            SetPosition(clip, "LeftHand",
                Keys((0f, -0.79f), (0.15f, -0.81f), (0.3f, -0.83060324f), (0.45f, -0.85f), (0.6f, -0.79f)),
                Keys((0f, -0.67f), (0.15f, -0.62f), (0.3f, -0.60908157f), (0.45f, -0.6f), (0.6f, -0.67f)),
                Keys((0f, -0.17130628f), (0.6f, -0.17130628f)));

            SetPosition(clip, "RightHand",
                Keys((0f, 0.91f), (0.15f, 0.94f), (0.3f, 0.9383498f), (0.45f, 0.97f), (0.6f, 0.91f)),
                Keys((0f, -0.57f), (0.15f, -0.61f), (0.3f, -0.62977576f), (0.45f, -0.65f), (0.6f, -0.57f)),
                Keys((0f, 0.08835077f), (0.6f, 0.08835077f)));

            SetLocalPositionY(clip, "Body_Middle", Keys((0f, -0.257f), (0.15f, -0.23f), (0.3f, -0.257f), (0.45f, -0.23f), (0.6f, -0.257f)));
            SetLocalPositionY(clip, "Head", Keys((0f, 0.473f), (0.15f, 0.515f), (0.3f, 0.473f), (0.45f, 0.515f), (0.6f, 0.473f)));

            return clip;
        }

        private static AnimationClip CreateJumpClip()
        {
            AnimationClip clip = NewClip("Jump", false, 0.3f);

            SetPosition(clip, "RightLeg",
                Keys((0f, 0.60548025f), (0.08f, 0.69f), (0.18f, 0.56f), (0.3f, 0.60548025f)),
                Keys((0f, -1.8753389f), (0.08f, -2.03f), (0.18f, -1.76f), (0.3f, -1.8753389f)),
                Keys((0f, 0.101387955f), (0.3f, 0.101387955f)));

            SetPosition(clip, "LeftLeg",
                Keys((0f, -0.41045982f), (0.08f, -0.44f), (0.18f, -0.39f), (0.3f, -0.41045982f)),
                Keys((0f, -1.7782948f), (0.08f, -1.85f), (0.18f, -1.72f), (0.3f, -1.7782948f)),
                Keys((0f, -0.05195633f), (0.3f, -0.05195633f)));

            SetPosition(clip, "LeftHand",
                Keys((0f, -0.83060324f), (0.18f, -0.87f), (0.3f, -0.83060324f)),
                Keys((0f, -0.60908157f), (0.18f, -0.55f), (0.3f, -0.60908157f)),
                Keys((0f, -0.17130628f), (0.3f, -0.17130628f)));

            SetPosition(clip, "RightHand",
                Keys((0f, 0.9383498f), (0.18f, 0.98f), (0.3f, 0.9383498f)),
                Keys((0f, -0.62977576f), (0.18f, -0.56f), (0.3f, -0.62977576f)),
                Keys((0f, 0.08835077f), (0.3f, 0.08835077f)));

            SetLocalPositionY(clip, "Head", Keys((0f, 0.473f), (0.18f, 0.54f), (0.3f, 0.473f)));
            SetLocalPositionY(clip, "Body_Middle", Keys((0f, -0.257f), (0.18f, -0.22f), (0.3f, -0.257f)));

            return clip;
        }

        private static void CreateController(AnimationClip idle, AnimationClip walkRight, AnimationClip walkLeft, AnimationClip jump)
        {
            AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(ControllerPath);
            if (controller == null)
            {
                controller = AnimatorController.CreateAnimatorControllerAtPath(ControllerPath);
            }

            CleanupControllerSubAssets(controller);

            controller.parameters = new AnimatorControllerParameter[0];
            controller.layers = new AnimatorControllerLayer[0];

            AnimatorControllerLayer layer = new()
            {
                name = "Base Layer",
                defaultWeight = 1f,
                stateMachine = new AnimatorStateMachine()
            };

            AssetDatabase.AddObjectToAsset(layer.stateMachine, controller);

            AnimatorState idleState = layer.stateMachine.AddState("Idle", new Vector3(60f, 90f, 0f));
            idleState.motion = idle;

            AnimatorState walkRightState = layer.stateMachine.AddState("WalkRight", new Vector3(220f, 40f, 0f));
            walkRightState.motion = walkRight;

            AnimatorState walkLeftState = layer.stateMachine.AddState("WalkLeft", new Vector3(220f, 150f, 0f));
            walkLeftState.motion = walkLeft;

            AnimatorState jumpState = layer.stateMachine.AddState("Jump", new Vector3(360f, 90f, 0f));
            jumpState.motion = jump;

            layer.stateMachine.defaultState = idleState;
            controller.layers = new[] { layer };

            EditorUtility.SetDirty(layer.stateMachine);
            EditorUtility.SetDirty(controller);
        }

        private static void CleanupControllerSubAssets(AnimatorController controller)
        {
            Object[] assets = AssetDatabase.LoadAllAssetsAtPath(ControllerPath);

            foreach (Object asset in assets)
            {
                if (asset == null || asset == controller)
                {
                    continue;
                }

                Object.DestroyImmediate(asset, true);
            }
        }

        private static AnimationClip NewClip(string name, bool loop, float stopTime)
        {
            AnimationClip clip = new() { name = name, frameRate = 60f };
            AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(clip);
            settings.loopTime = loop;
            settings.stopTime = stopTime;
            AnimationUtility.SetAnimationClipSettings(clip, settings);
            return clip;
        }

        private static void SaveClip(AnimationClip clip, string path)
        {
            AnimationClip existing = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
            if (existing == null)
            {
                AssetDatabase.CreateAsset(clip, path);
                return;
            }

            EditorUtility.CopySerialized(clip, existing);
            EditorUtility.SetDirty(existing);
        }

        private static void SetLocalPositionY(AnimationClip clip, string path, Keyframe[] yKeys)
        {
            SetCurve(clip, path, "m_LocalPosition.y", yKeys);
        }

        private static void SetPosition(AnimationClip clip, string path, Keyframe[] xKeys, Keyframe[] yKeys, Keyframe[] zKeys)
        {
            SetCurve(clip, path, "m_LocalPosition.x", xKeys);
            SetCurve(clip, path, "m_LocalPosition.y", yKeys);
            SetCurve(clip, path, "m_LocalPosition.z", zKeys);
        }

        private static void SetCurve(AnimationClip clip, string path, string propertyName, Keyframe[] keys)
        {
            AnimationCurve curve = new(keys);
            EditorCurveBinding binding = EditorCurveBinding.FloatCurve(path, typeof(Transform), propertyName);
            AnimationUtility.SetEditorCurve(clip, binding, curve);
        }

        private static Keyframe[] Keys(params (float time, float value)[] points)
        {
            List<Keyframe> keys = new(points.Length);

            foreach ((float time, float value) in points)
            {
                keys.Add(new Keyframe(time, value));
            }

            return keys.ToArray();
        }

        private static class DirectoryUtility
        {
            public static void EnsureFolderExists(string assetFolderPath)
            {
                string[] parts = assetFolderPath.Split('/');
                string current = parts[0];

                for (int index = 1; index < parts.Length; index++)
                {
                    string next = $"{current}/{parts[index]}";
                    if (!AssetDatabase.IsValidFolder(next))
                    {
                        AssetDatabase.CreateFolder(current, parts[index]);
                    }

                    current = next;
                }
            }
        }
    }
}
