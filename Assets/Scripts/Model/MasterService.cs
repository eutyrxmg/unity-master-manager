using System.IO;
using MessagePack;
using MessagePack.Resolvers;
using Model;
using UnityEngine;

/// <summary>
/// マスタデータへのアクセスを提供するクラス
/// </summary>
public static class MasterService {
    public static MemoryDatabase MemoryDatabase;
    public static void Init() {
        SetupMessagePackResolver();
        MemoryDatabase = new MemoryDatabase(File.ReadAllBytes(Const.MasterDataPath));
    }
    
    [RuntimeInitializeOnLoadMethod (RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void SetupMessagePackResolver() {
        StaticCompositeResolver.Instance.Register(new[] {
            MasterMemoryResolver.Instance, // set MasterMemory generated resolver
            GeneratedResolver.Instance, // set MessagePack generated resolver
            StandardResolver.Instance // set default MessagePack resolver
        });

        var options = MessagePackSerializerOptions.Standard.WithResolver(StaticCompositeResolver.Instance);
        MessagePackSerializer.DefaultOptions = options;
    }
}
