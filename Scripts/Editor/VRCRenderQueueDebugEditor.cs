using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Collections.Generic;
using nadena.dev.modular_avatar.core;

namespace Narazaka.VRCRenderQueueDebug.Editor
{
    [CustomEditor(typeof(VRCRenderQueueDebug))]
    public class VRCRenderQueueDebugEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Generate (Danger: takes long time!!!!!)"))
            {
                VRCRenderQueueDebug vrcRenderQueueDebug = (VRCRenderQueueDebug)target;
                SaveAnimator(vrcRenderQueueDebug);
            }
        }

        void SaveAnimator(VRCRenderQueueDebug vrcRenderQueueDebug)
        {
            var path = EditorUtility.SaveFilePanelInProject("Save Animator Controller", "New Animator Controller", "controller", "Please enter a file name to save the Animator Controller to");
            if (string.IsNullOrEmpty(path)) return;

            var animator = VRCRenderQueueDebugPlugin.MakeAnimator(vrcRenderQueueDebug.gameObject, vrcRenderQueueDebug.Renderers, vrcRenderQueueDebug.MinRenderQueue, vrcRenderQueueDebug.MaxRenderQueue);

            AssetDatabase.CreateAsset(animator, path);
            foreach (var layer in animator.layers)
            {
                SaveStateMachineAssets(layer.stateMachine, animator);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            var ma = VRCRenderQueueDebugPlugin.MakeMergeAnimator(vrcRenderQueueDebug.gameObject, animator);
            Undo.RegisterCreatedObjectUndo(ma, "Merge Animator");
            var map = vrcRenderQueueDebug.GetComponent<ModularAvatarParameters>();
            if (map == null)
            {
                map = VRCRenderQueueDebugPlugin.MakeParameters(vrcRenderQueueDebug.gameObject);
                Undo.RegisterCreatedObjectUndo(map, "Parameters");
            }
            else
            {
                Undo.RecordObject(map, "Parameters");
                VRCRenderQueueDebugPlugin.MakeParameters(vrcRenderQueueDebug.gameObject);
            }
        }

        void SaveStateMachineAssets(AnimatorStateMachine stateMachine, AnimatorController animatorController)
        {
            SaveStateMachineAssets(stateMachine, animatorController, new HashSet<Object>());
        }

        void SaveStateMachineAssets(AnimatorStateMachine stateMachine, AnimatorController animatorController, HashSet<Object> added)
        {
            AssetDatabase.AddObjectToAsset(stateMachine, animatorController);
            foreach (var state in stateMachine.states)
            {
                AssetDatabase.AddObjectToAsset(state.state, animatorController);
                foreach (var tr in state.state.transitions)
                {
                    AssetDatabase.AddObjectToAsset(tr, animatorController);
                }

                var clip = state.state.motion as AnimationClip;
                if (clip != null)
                {
                    clip.hideFlags = HideFlags.None;
                    if (!added.Contains(clip))
                    {
                        AssetDatabase.AddObjectToAsset(clip, animatorController);
                        added.Add(clip);
                    }
                    var bindings = AnimationUtility.GetObjectReferenceCurveBindings(clip);
                    foreach (var binding in bindings)
                    {
                        var curve = AnimationUtility.GetObjectReferenceCurve(clip, binding);
                        if (curve != null && curve.Length > 0)
                        {
                            var obj = curve[0].value;
                            if (!added.Contains(obj))
                            {
                                obj.hideFlags = HideFlags.None;
                                AssetDatabase.AddObjectToAsset(obj, animatorController);
                                added.Add(obj);
                            }
                        }
                    }
                }
            }
            foreach (var tr in stateMachine.anyStateTransitions)
            {
                AssetDatabase.AddObjectToAsset(tr, animatorController);
            }
            foreach (var tr in stateMachine.entryTransitions)
            {
                AssetDatabase.AddObjectToAsset(tr, animatorController);
            }
            foreach (var behaviour in stateMachine.behaviours)
            {
                AssetDatabase.AddObjectToAsset(behaviour, animatorController);
            }

            foreach (var child in stateMachine.stateMachines)
            {
                foreach (var tr in stateMachine.GetStateMachineTransitions(child.stateMachine))
                {
                    AssetDatabase.AddObjectToAsset(tr, animatorController);
                }
                SaveStateMachineAssets(child.stateMachine, animatorController, added);
            }
        }
    }
}
