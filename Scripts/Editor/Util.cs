using UnityEditor.Animations;
using UnityEditor;
using UnityEngine;
using System;

namespace Narazaka.VRCRenderQueueDebug.Editor
{
    public static class Util
    {
        //   paramater:
        public static void AddParameterEX(this AnimatorController animatorController, string name, UnityEngine.AnimatorControllerParameterType type)
        {
            var animatorControllerParameter = new AnimatorControllerParameter();
            animatorControllerParameter.name = name;
            animatorControllerParameter.type = type;
            animatorController.AddParameterEX(animatorControllerParameter);
        }

        public static void AddParameterEX(this AnimatorController animatorController, AnimatorControllerParameter paramater)
        {
            var array = animatorController.parameters;
            ArrayUtility.Add(ref array, paramater);
            animatorController.parameters = array;
        }


        public static void AddLayerEX(this AnimatorController animatorController, string name)
        {
            var animatorControllerLayer = new AnimatorControllerLayer();
            animatorControllerLayer.name = name;
            animatorControllerLayer.stateMachine = new AnimatorStateMachine();
            animatorControllerLayer.stateMachine.name = animatorControllerLayer.name;
            animatorControllerLayer.stateMachine.hideFlags = HideFlags.HideInHierarchy;
            if (AssetDatabase.GetAssetPath(animatorController) != "")
            {
                AssetDatabase.AddObjectToAsset(animatorControllerLayer.stateMachine, AssetDatabase.GetAssetPath(animatorController));
            }

            animatorController.AddLayerEX(animatorControllerLayer);
        }

        public static void AddLayerEX(this AnimatorController animatorController, AnimatorControllerLayer layer)
        {
            var array = animatorController.layers;
            ArrayUtility.Add(ref array, layer);
            animatorController.layers = array;
        }

        public static AnimatorStateMachine AddStateMachineEX(this AnimatorStateMachine animatorStateMachine, string name)
        {
            return animatorStateMachine.AddStateMachineEX(name, Vector3.zero);
        }

        public static AnimatorStateMachine AddStateMachineEX(this AnimatorStateMachine sourceAnimatorStateMachine, string name, Vector3 position)
        {
            AnimatorStateMachine animatorStateMachine = new AnimatorStateMachine();
            animatorStateMachine.hideFlags = HideFlags.HideInHierarchy;
            animatorStateMachine.name = name;
            sourceAnimatorStateMachine.AddStateMachineEX(animatorStateMachine, position);
            if (AssetDatabase.GetAssetPath(sourceAnimatorStateMachine) != "")
            {
                AssetDatabase.AddObjectToAsset(animatorStateMachine, AssetDatabase.GetAssetPath(sourceAnimatorStateMachine));
            }

            return animatorStateMachine;
        }

        public static void AddStateMachineEX(this AnimatorStateMachine animatorStateMachine, AnimatorStateMachine stateMachine, Vector3 position)
        {
            ChildAnimatorStateMachine[] array = animatorStateMachine.stateMachines;

            ChildAnimatorStateMachine item = default;
            item.stateMachine = stateMachine;
            item.position = position;
            ArrayUtility.Add(ref array, item);
            animatorStateMachine.stateMachines = array;
        }

        //     The AnimatorState that was created for this state.
        public static AnimatorState AddStateEX(this AnimatorStateMachine animatorStateMachine, string name)
        {
            return animatorStateMachine.AddStateEX(name, (animatorStateMachine.states.Length != 0) ? (animatorStateMachine.states[animatorStateMachine.states.Length - 1].position + new Vector3(35f, 65f)) : new Vector3(200f, 0f, 0f));
        }

        //
        // 概要:
        //     Utility function to add a state to the state machine.
        //
        // パラメーター:
        //   name:
        //     The name of the new state.
        //
        //   position:
        //     The position of the state node.
        //
        // 戻り値:
        //     The AnimatorState that was created for this state.
        public static AnimatorState AddStateEX(this AnimatorStateMachine animatorStateMachine, string name, Vector3 position)
        {
            AnimatorState animatorState = new AnimatorState();
            animatorState.hideFlags = HideFlags.HideInHierarchy;
            animatorState.name = name;
            if (AssetDatabase.GetAssetPath(animatorStateMachine) != "")
            {
                AssetDatabase.AddObjectToAsset(animatorState, AssetDatabase.GetAssetPath(animatorStateMachine));
            }

            animatorStateMachine.AddStateEX(animatorState, position);
            return animatorState;
        }

        //
        // 概要:
        //     Utility function to add a state to the state machine.
        //
        // パラメーター:
        //   state:
        //     The state to add.
        //
        //   position:
        //     The position of the state node.
        public static void AddStateEX(this AnimatorStateMachine animatorStateMachine, AnimatorState state, Vector3 position)
        {
            ChildAnimatorState[] array = animatorStateMachine.states;

            ChildAnimatorState item = default;
            item.state = state;
            item.position = position;
            ArrayUtility.Add(ref array, item);
            animatorStateMachine.states = array;
        }


        //     The destination state.
        public static AnimatorStateTransition AddTransitionEX(this AnimatorState sourceState, AnimatorState destinationState)
        {
            AnimatorStateTransition animatorStateTransition = sourceState.CreateTransitionEX(setDefaultExitTime: false);
            animatorStateTransition.destinationState = destinationState;
            sourceState.AddTransitionEX(animatorStateTransition);
            return animatorStateTransition;
        }

        //
        // 概要:
        //     Utility function to add an outgoing transition to the destination state machine.
        //
        //
        // パラメーター:
        //   defaultExitTime:
        //     If true, the exit time will be the equivalent of 0.25 second.
        //
        //   destinationStateMachine:
        //     The destination state machine.
        public static AnimatorStateTransition AddTransitionEX(this AnimatorState sourceState, AnimatorStateMachine destinationStateMachine)
        {
            AnimatorStateTransition animatorStateTransition = sourceState.CreateTransitionEX(setDefaultExitTime: false);
            animatorStateTransition.destinationStateMachine = destinationStateMachine;
            sourceState.AddTransitionEX(animatorStateTransition);
            return animatorStateTransition;
        }

        //
        // 概要:
        //     Utility function to add an outgoing transition to the destination state.
        //
        // パラメーター:
        //   defaultExitTime:
        //     If true, the exit time will be the equivalent of 0.25 second.
        //
        //   destinationState:
        //     The destination state.
        public static AnimatorStateTransition AddTransitionEX(this AnimatorState sourceState, AnimatorState destinationState, bool defaultExitTime)
        {
            AnimatorStateTransition animatorStateTransition = sourceState.CreateTransitionEX(defaultExitTime);
            animatorStateTransition.destinationState = destinationState;
            sourceState.AddTransitionEX(animatorStateTransition);
            return animatorStateTransition;
        }

        //
        // 概要:
        //     Utility function to add an outgoing transition to the destination state machine.
        //
        //
        // パラメーター:
        //   defaultExitTime:
        //     If true, the exit time will be the equivalent of 0.25 second.
        //
        //   destinationStateMachine:
        //     The destination state machine.
        public static AnimatorStateTransition AddTransitionEX(this AnimatorState sourceState, AnimatorStateMachine destinationStateMachine, bool defaultExitTime)
        {
            AnimatorStateTransition animatorStateTransition = sourceState.CreateTransitionEX(defaultExitTime);
            animatorStateTransition.destinationStateMachine = destinationStateMachine;
            sourceState.AddTransitionEX(animatorStateTransition);
            return animatorStateTransition;
        }

        //
        // 概要:
        //     Utility function to add an outgoing transition to the exit of the state's parent
        //     state machine.
        //
        // パラメーター:
        //   defaultExitTime:
        //     If true, the exit time will be the equivalent of 0.25 second.
        //
        // 戻り値:
        //     The Animations.AnimatorStateTransition that was added.
        public static AnimatorStateTransition AddExitTransitionEX(this AnimatorState sourceState)
        {
            return sourceState.AddExitTransitionEX(defaultExitTime: false);
        }

        //
        // 概要:
        //     Utility function to add an outgoing transition to the exit of the state's parent
        //     state machine.
        //
        // パラメーター:
        //   defaultExitTime:
        //     If true, the exit time will be the equivalent of 0.25 second.
        //
        // 戻り値:
        //     The Animations.AnimatorStateTransition that was added.
        public static AnimatorStateTransition AddExitTransitionEX(this AnimatorState sourceState, bool defaultExitTime)
        {
            AnimatorStateTransition animatorStateTransition = sourceState.CreateTransitionEX(defaultExitTime);
            animatorStateTransition.isExit = true;
            sourceState.AddTransitionEX(animatorStateTransition);
            return animatorStateTransition;
        }

        private static AnimatorStateTransition CreateTransitionEX(this AnimatorState sourceState, bool setDefaultExitTime)
        {
            AnimatorStateTransition newTransition = new AnimatorStateTransition();
            newTransition.hasExitTime = false;
            newTransition.hasFixedDuration = true;
            if (AssetDatabase.GetAssetPath(sourceState) != "")
            {
                AssetDatabase.AddObjectToAsset(newTransition, AssetDatabase.GetAssetPath(sourceState));
            }

            newTransition.hideFlags = HideFlags.HideInHierarchy;

            return newTransition;
        }


        public static void AddTransitionEX(this AnimatorState source, AnimatorStateTransition transition)
        {
            AnimatorStateTransition[] array = source.transitions;
            ArrayUtility.Add(ref array, transition);
            source.transitions = array;
        }
    }
}