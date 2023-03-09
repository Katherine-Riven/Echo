namespace Echo.Abilities
{
    /// <summary>
    /// 能力资源引用
    /// </summary>
    public interface IAbilityReference : IAssetReference<AbilityAsset>
    {
        string GUID { get; }
    }
}