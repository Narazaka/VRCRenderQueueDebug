using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Narazaka.VRCRenderQueueDebug
{
    public class VRCRenderQueueDebug : MonoBehaviour, VRC.SDKBase.IEditorOnly
    {
        public Renderer[] Renderers;
        public int MinRenderQueue = 0;
        public int MaxRenderQueue = 5000;
    }
}
