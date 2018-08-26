using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

namespace CustomSRP
{
    [ExecuteInEditMode]
    public class BasicAssetPipe : RenderPipelineAsset
    {
        public Color clearColor = Color.green;

#if UNITY_EDITOR
        [UnityEditor.MenuItem("CustomSRP/Create Basic Asset Pipeline")]
        static void CreateBasicAssetPipeline()
        {
            var instance = ScriptableObject.CreateInstance<BasicAssetPipe>();
            UnityEditor.AssetDatabase.CreateAsset(instance, "Assets/CustomSRP/BasicAssetPipe.asset");
        }
#endif

        protected override IRenderPipeline InternalCreatePipeline()
        {
            return new BasicPipeInstance(clearColor);
        }
    }

    public class BasicPipeInstance : RenderPipeline
    {
        private Color m_ClearColor = Color.black;

        public BasicPipeInstance(Color clearColor)
        {
            m_ClearColor = clearColor;
        }

        public override void Render(ScriptableRenderContext context, Camera[] cameras)
        {
            // does not so much yet :()
            base.Render(context, cameras);

            // clear buffers to the configured color
            var cmd = new CommandBuffer();
            cmd.ClearRenderTarget(true, true, m_ClearColor);
            context.ExecuteCommandBuffer(cmd);
            cmd.Release();
            context.Submit();
        }
    }
}