using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Hash128 = Unity.Entities.Hash128;

[DisallowMultipleComponent]
public class TextureAssetAuthoring : MonoBehaviour//,IConvertGameObjectToEntity
{
	[SerializeField] private Texture2D _texture;

	public Texture2D Texture { get => _texture; }

	//public static BlobAssetReference<TextureAssetData> TextureAssetRef;

	//public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
	//{
	//	using (BlobBuilder blobBuilder = new BlobBuilder(Allocator.Temp))
	//	{
	//		ref TextureAssetData blobAsset = ref blobBuilder.ConstructRoot<TextureAssetData>();
	//		blobAsset.Size = new int2(_texture.width, _texture.height);

	//		BlobBuilderArray<float4> data = blobBuilder.Allocate(ref blobAsset.ColorTable, _texture.width * _texture.height);

	//		var cs = _texture.GetRawTextureData<Color32>();
	//		for (int i = 0; i < data.Length; i++)
	//		{
	//			var p = cs[i];
	//			data[i] = math.float4(p.r, p.g, p.b, p.a) / 255f;
	//		}
	//		cs.Dispose();

	//		TextureAssetRef = blobBuilder.CreateBlobAssetReference<TextureAssetData>(Allocator.Persistent);

	//		conversionSystem.BlobAssetStore.TryAdd(new Hash128("TextureAsset"), TextureAssetRef);
	//	}
	//}
}

public struct TextureAssetData
{
	public BlobArray<float4> ColorTable;
	public int2 Size;
}
public class TextureAssetConversionSystem : GameObjectConversionSystem
{
	public static BlobAssetReference<TextureAssetData> TextureAssetRef;
	protected override void OnUpdate()
	{
		Entities
			.ForEach(
			(
				TextureAssetAuthoring texAuth) =>
			{
				using (BlobBuilder blobBuilder = new BlobBuilder(Allocator.Temp))
				{
					ref TextureAssetData blobAsset = ref blobBuilder.ConstructRoot<TextureAssetData>();
					blobAsset.Size = new int2(texAuth.Texture.width, texAuth.Texture.height);

					BlobBuilderArray<float4> data = blobBuilder.Allocate(ref blobAsset.ColorTable, texAuth.Texture.width * texAuth.Texture.height);
					
					var cs = texAuth.Texture.GetRawTextureData<Color32>();
					//var cs2 = texAuth.Texture.GetPixelData<Color>(0);
					for (int i = 0; i < cs.Length; i++)
					{
						var p = cs[i];
						data[i] = math.float4(p.r, p.g, p.b, p.a)/256f;
					}
					cs.Dispose();

					TextureAssetRef = blobBuilder.CreateBlobAssetReference<TextureAssetData>(Allocator.Persistent);

					BlobAssetStore.TryAdd(new Hash128("TextureAsset"), TextureAssetRef);
				}
				DstEntityManager.DestroyEntity(GetPrimaryEntity(texAuth));
			});
	}
}
