using UnityEngine;

namespace Narazaka.VRCRenderQueueDebug
{
    public class VRCRenderQueueDebugPresets : MonoBehaviour, VRC.SDKBase.IEditorOnly
    {
        public int[] SetRenderQueues = new int[] { 2000, 2450, 2460, 3000 };
    }
}
