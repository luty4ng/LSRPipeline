using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "Rendering/LSRPipeline")]
public class LSRPipelineAsset : RenderPipelineAsset
{    
    protected override RenderPipeline CreatePipeline()
    {
        return new LSRPipeline(true, true, true);
    }
}
