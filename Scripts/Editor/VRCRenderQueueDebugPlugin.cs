using nadena.dev.ndmf;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;
using UnityEditor;
using VRC.SDKBase;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;
using nadena.dev.modular_avatar.core;
using System;

[assembly: ExportsPlugin(typeof(Narazaka.VRCRenderQueueDebug.Editor.VRCRenderQueueDebugPlugin))]

namespace Narazaka.VRCRenderQueueDebug.Editor
{
    public class VRCRenderQueueDebugPlugin : Plugin<VRCRenderQueueDebugPlugin>

    {
        public override string QualifiedName => "net.narazaka.vrchat.vrc_render_queue_debug";

        public override string DisplayName => "VRCRenderQueueDebug";

        protected override void Configure()
        {
            InPhase(BuildPhase.Generating).BeforePlugin("nadena.dev.modular-avatar").Run("VRCRenderQueueDebug", ctx =>
            {
                var rqd = ctx.AvatarRootTransform.GetComponentInChildren<VRCRenderQueueDebug>();
                if (rqd != null)
                {
                    var renderers = rqd.Renderers;
                    var animator = MakeAnimator(rqd.gameObject, renderers, rqd.MinRenderQueue, rqd.MaxRenderQueue);
                    MakeMergeAnimator(rqd.gameObject, animator);
                    MakeParameters(rqd.gameObject);
                    UnityEngine.Object.DestroyImmediate(rqd);
                }

                var rqdPresets = ctx.AvatarRootTransform.GetComponentInChildren<VRCRenderQueueDebugPresets>();
                if (rqdPresets != null)
                {
                    var setRenderQueues = rqdPresets.SetRenderQueues.Distinct().ToArray();
                    var setRenderQueuesAnimator = MakeSetRenderQueueAnimator(setRenderQueues);
                    MakeMergeAnimator(rqdPresets.gameObject, setRenderQueuesAnimator);
                    MakeSetRenderQueueParameters(rqdPresets.gameObject, setRenderQueues);
                    MakeSetRenderQueueMenu(rqdPresets.gameObject, setRenderQueues);
                    UnityEngine.Object.DestroyImmediate(rqdPresets);
                }
            });
        }

        public static AnimatorController MakeAnimator(GameObject root, IEnumerable<Renderer> renderers, int MinRenderQueue, int MaxRenderQueue)
        {
            var animatorController = new AnimatorController();
            animatorController.AddParameterEX("RenderQueue_1", AnimatorControllerParameterType.Int);
            animatorController.AddParameterEX("RenderQueue_10", AnimatorControllerParameterType.Int);
            animatorController.AddParameterEX("RenderQueue_100", AnimatorControllerParameterType.Int);
            animatorController.AddParameterEX("RenderQueue_1000", AnimatorControllerParameterType.Int);

            animatorController.AddLayerEX("RenderQueue");
            var stateMachine = animatorController.layers[animatorController.layers.Length - 1].stateMachine;
            var defaultState = stateMachine.AddStateEX("Default");
            SetState(defaultState);
            stateMachine.defaultState = defaultState;
            for (int i = MinRenderQueue; i <= MaxRenderQueue; i += 1000)
            {
                var renderQueue = i;
                var subStateMachine1000 = stateMachine.AddStateMachineEX($"RenderQueue_1000_{renderQueue}_m", new Vector3(600, 100 * i / 1000));
                var state1000 = subStateMachine1000.AddStateEX($"RenderQueue_1000_{renderQueue}", new Vector3(300, 0));
                SetState(state1000);
                subStateMachine1000.defaultState = state1000;
                var transition1000 = defaultState.AddTransitionEX(subStateMachine1000);
                SetTransition(transition1000, "RenderQueue_1000", i / 1000);
                var exitTransition1000m = state1000.AddExitTransition();
                SetNotTransition(exitTransition1000m, "RenderQueue_1000", i / 1000);
                for (int j = 0; j < 1000; j += 100)
                {
                    renderQueue = i + j;
                    var subStateMachine100 = subStateMachine1000.AddStateMachineEX($"RenderQueue_100_{renderQueue}_m", new Vector3(600, 100 * j / 100));
                    var state100 = subStateMachine100.AddStateEX($"RenderQueue_100_{renderQueue}", new Vector3(300, 0));
                    SetState(state100);
                    subStateMachine100.defaultState = state100;
                    var transition100 = state1000.AddTransitionEX(subStateMachine100);
                    SetTransition(transition100, "RenderQueue_100", j / 100);
                    var exitTransition100m_100 = state100.AddExitTransition();
                    SetNotTransition(exitTransition100m_100, "RenderQueue_100", j / 100);
                    var exitTransition100m_1000 = state100.AddExitTransition();
                    SetNotTransition(exitTransition100m_1000, "RenderQueue_1000", i / 1000);
                    for (int k = 0; k < 100; k += 10)
                    {
                        renderQueue = i + j + k;
                        var subStateMachine10 = subStateMachine100.AddStateMachineEX($"RenderQueue_10_{renderQueue}_m", new Vector3(600, 100 * k / 10));
                        var state10 = subStateMachine10.AddStateEX($"RenderQueue_10_{renderQueue}", new Vector3(300, 0));
                        SetState(state10);
                        subStateMachine10.defaultState = state10;
                        var transition10 = state100.AddTransitionEX(subStateMachine10);
                        SetTransition(transition10, "RenderQueue_10", k / 10);
                        var exitTransition10m_10 = state10.AddExitTransition();
                        SetNotTransition(exitTransition10m_10, "RenderQueue_10", k / 10);
                        var exitTransition10m_100 = state10.AddExitTransition();
                        SetNotTransition(exitTransition10m_100, "RenderQueue_100", j / 100);
                        var exitTransition10m_1000 = state10.AddExitTransition();
                        SetNotTransition(exitTransition10m_1000, "RenderQueue_1000", i / 1000);
                        for (int l = 0; l < 10; l++)
                        {
                            renderQueue = i + j + k + l;
                            var state1 = subStateMachine10.AddStateEX($"RenderQueue_1_{renderQueue}", new Vector3(600, 100 * l));
                            SetState(state1);
                            var transition1 = state10.AddTransitionEX(state1);
                            SetTransition(transition1, "RenderQueue_1", l);
                            state1.motion = MakeMaterialAndClip(root, renderers, renderQueue);
                            var exitTransition1 = state1.AddExitTransition();
                            SetNotTransition(exitTransition1, "RenderQueue_1", l);
                            var exitTransition10 = state1.AddExitTransition();
                            SetNotTransition(exitTransition10, "RenderQueue_10", k / 10);
                            var exitTransition100 = state1.AddExitTransition();
                            SetNotTransition(exitTransition100, "RenderQueue_100", j / 100);
                            var exitTransition1000 = state1.AddExitTransition();
                            SetNotTransition(exitTransition1000, "RenderQueue_1000", i / 1000);
                        }
                    }
                }
            }
            return animatorController;
        }

        static AnimatorController MakeSetRenderQueueAnimator(IEnumerable<int> setRenderQueues)
        {
            var animatorController = new AnimatorController();

            animatorController.AddLayerEX("SetRenderQueue");
            var setRenderQueueStateMachine = animatorController.layers[animatorController.layers.Length - 1].stateMachine;
            var setDefaultState = setRenderQueueStateMachine.AddStateEX("Default");
            SetState(setDefaultState);
            foreach (var setRenderQueue in setRenderQueues)
            {
                animatorController.AddParameterEX($"SetRenderQueue_{setRenderQueue}", AnimatorControllerParameterType.Bool);
                var state = setRenderQueueStateMachine.AddStateEX($"SetRenderQueue_{setRenderQueue}");
                SetState(state);
                var transition = setDefaultState.AddTransitionEX(state);
                SetTransition(transition, $"SetRenderQueue_{setRenderQueue}", true);
                var exitTransition = state.AddExitTransitionEX();
                SetTransition(exitTransition, $"SetRenderQueue_{setRenderQueue}", false);
                state.behaviours = new StateMachineBehaviour[] {
                        new VRCAvatarParameterDriver
                        {
                            localOnly = true,
                            parameters = new List<VRC_AvatarParameterDriver.Parameter>
                            {
                                new() {
                                    name = $"RenderQueue_1",
                                    type = VRC_AvatarParameterDriver.ChangeType.Set,
                                    value = setRenderQueue % 10,
                                },
                                new() {
                                    name = $"RenderQueue_10",
                                    type = VRC_AvatarParameterDriver.ChangeType.Set,
                                    value = (setRenderQueue / 10) % 10,
                                },
                                new() {
                                    name = $"RenderQueue_100",
                                    type = VRC_AvatarParameterDriver.ChangeType.Set,
                                    value = (setRenderQueue / 100) % 10,
                                },
                                new() {
                                    name = $"RenderQueue_1000",
                                    type = VRC_AvatarParameterDriver.ChangeType.Set,
                                    value = (setRenderQueue / 1000) % 10,
                                },
                            }
                        },
                    };
            }
            return animatorController;
        }

        public static ModularAvatarMergeAnimator MakeMergeAnimator(GameObject root, AnimatorController animatorController)
        {
            var mergeAnimator = root.AddComponent<ModularAvatarMergeAnimator>();
            mergeAnimator.animator = animatorController;
            mergeAnimator.layerType = VRCAvatarDescriptor.AnimLayerType.FX;
            mergeAnimator.matchAvatarWriteDefaults = true;
            mergeAnimator.pathMode = MergeAnimatorPathMode.Relative;
            return mergeAnimator;
        }

        public static ModularAvatarParameters MakeParameters(GameObject root)
        {
            var parameters = root.GetComponent<ModularAvatarParameters>();
            if (parameters == null) root.AddComponent<ModularAvatarParameters>();
            parameters.parameters.AddRange(new List<ParameterConfig>
            {
                new() { nameOrPrefix = "RenderQueue_1", syncType = ParameterSyncType.Int, defaultValue = 0, saved = false },
                new() { nameOrPrefix = "RenderQueue_10", syncType = ParameterSyncType.Int, defaultValue = 0, saved = false },
                new() { nameOrPrefix = "RenderQueue_100", syncType = ParameterSyncType.Int, defaultValue = 0, saved = false },
                new() { nameOrPrefix = "RenderQueue_1000", syncType = ParameterSyncType.Int, defaultValue = 3, saved = false }
            });
            return parameters;
        }

        static void MakeSetRenderQueueParameters(GameObject root, IEnumerable<int> setRenderQueues)
        {
            var parameters = root.GetComponent<ModularAvatarParameters>();
            if (parameters == null) root.AddComponent<ModularAvatarParameters>();
            foreach (var setRenderQueue in setRenderQueues)
            {
                parameters.parameters.Add(new ParameterConfig { nameOrPrefix = $"SetRenderQueue_{setRenderQueue}", syncType = ParameterSyncType.Bool, defaultValue = 0 });
            }
        }

        static void MakeSetRenderQueueMenu(GameObject menu, IEnumerable<int> setRenderQueues)
        {
            var sets = menu.transform.Find("Set").gameObject;
            foreach (var setRenderQueue in setRenderQueues)
            {
                var set = new GameObject($"={setRenderQueue}");
                set.transform.SetParent(sets.transform, false);
                var button = set.AddComponent<ModularAvatarMenuItem>();
                button.Control = new VRCExpressionsMenu.Control
                {
                    type = VRCExpressionsMenu.Control.ControlType.Button,
                    parameter = new VRCExpressionsMenu.Control.Parameter
                    {
                        name = $"SetRenderQueue_{setRenderQueue}",
                    },
                    value = 1,
                };
            }
        }

        static void SetState(AnimatorState animatorState)
        {
            animatorState.writeDefaultValues = false;
            animatorState.motion = EmptyAnimationClip;
        }

        static void SetTransition(AnimatorTransition animatorTransition, string param, int value)
        {
            animatorTransition.AddCondition(AnimatorConditionMode.Equals, value, param);
        }

        static void SetTransition(AnimatorStateTransition animatorTransition, string param, bool value)
        {
            animatorTransition.AddCondition(value ? AnimatorConditionMode.If : AnimatorConditionMode.IfNot, 1, param);
            animatorTransition.hasExitTime = false;
            animatorTransition.hasFixedDuration = false;
            animatorTransition.duration = 0;
        }

        static void SetTransition(AnimatorStateTransition animatorTransition, string param, int value)
        {
            animatorTransition.AddCondition(AnimatorConditionMode.Equals, value, param);
            animatorTransition.hasExitTime = false;
            animatorTransition.hasFixedDuration = false;
            animatorTransition.duration = 0;
        }

        static void SetNotTransition(AnimatorStateTransition animatorTransition, string param, int value)
        {
            animatorTransition.AddCondition(AnimatorConditionMode.NotEqual, value, param);
            animatorTransition.hasExitTime = false;
            animatorTransition.hasFixedDuration = false;
            animatorTransition.duration = 0;
        }

        static AnimationClip EmptyAnimationClip { get; } = MakeEmptyAnimationClip();

        static AnimationClip MakeEmptyAnimationClip()
        {
            var clip = new AnimationClip();
            clip.name = "__VRCRenderQueueDebug_EMPTY__";
            clip.SetCurve("__VRCRenderQueueDebug_EMPTY__", typeof(GameObject), "localPosition.x", new AnimationCurve { keys = new Keyframe[] { new Keyframe { time = 0, value = 0 }, new Keyframe { time = 1f / 60f, value = 0 } } });
            return clip;
        }

        static AnimationClip MakeMaterialAndClip(GameObject root, IEnumerable<Renderer> renderers, int renderQueue)
        {
            var clip = new AnimationClip();
            clip.name = $"RenderQueue_{renderQueue}";
            foreach (var renderer in renderers)
            {
                var curvePath = ChildPath(root, renderer.gameObject);
                var materials = renderer.sharedMaterials;
                for (int index = 0; index < materials.Length; index++)
                {
                    var material = new Material(materials[index]);
                    material.name = $"{material.name} ({renderQueue})";
                    material.parent = materials[index];
                    material.renderQueue = renderQueue;
                    var binding = EditorCurveBinding.PPtrCurve(curvePath, typeof(Renderer), $"m_Materials.Array.data[{index}]");
                    AnimationUtility.SetObjectReferenceCurve(
                        clip,
                        binding,
                        new ObjectReferenceKeyframe[] {
                            new ObjectReferenceKeyframe { time = 0, value = material },
                            new ObjectReferenceKeyframe { time = 1f / 60f, value = material },
                        });
                }
            }
            return clip;
        }

        /// <summary>
        /// 子GameObjectの相対パスを返す (親がnullなら絶対パス)
        /// </summary>
        /// <param name="baseObject">親 (nullなら絶対パス)</param>
        /// <param name="targetObject">子</param>
        /// <returns>パス</returns>
        static string ChildPath(GameObject baseObject, GameObject targetObject)
        {
            var paths = new List<string>();
            var transform = targetObject.transform;
            var baseObjectTransform = baseObject == null ? null : baseObject.transform;
            while (baseObjectTransform != transform && transform != null)
            {
                paths.Add(transform.gameObject.name);
                transform = transform.parent;
            }
            paths.Reverse();
            return string.Join("/", paths.ToArray());
        }
    }
}
