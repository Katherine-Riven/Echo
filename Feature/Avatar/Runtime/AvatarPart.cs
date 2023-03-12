using UnityEngine;

namespace Echo.Avatar
{
    public enum AvatarPartType
    {
        Body,
        Cloak,
    }

    public sealed class AvatarPart
    {
        public AvatarPart(SkinnedMeshRenderer renderer)
        {
            Renderer = renderer;
            Default  = renderer.sharedMesh;
        }

        public SkinnedMeshRenderer Renderer { get; }
        public Mesh                Default  { get; }
        public Mesh                Active   { get; set; }
    }
}