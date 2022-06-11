using Unity.Entities;
using UnityEngine;

public class DominoColorRefAuthoring : MonoBehaviour//, IConvertGameObjectToEntity
{
	//public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
	//{
	//	dstManager.AddComponentData<DominoColorRef>(entity, new DominoColorRef { ColorRef = TextureAssetAuthoring.TextureAssetRef });
	//}
}
struct DominoColorRef : IComponentData
{
	public BlobAssetReference<TextureAssetData> ColorRef;
}

[UpdateAfter(typeof(TextureAssetConversionSystem))]
public class DominoColorRefConversionSystem : GameObjectConversionSystem
{
	protected override void OnUpdate()
	{
		Entities
			.ForEach(
			(
				DominoColorRefAuthoring colorRefAuth) =>
			{
				var entity = GetPrimaryEntity(colorRefAuth);
				DstEntityManager.AddComponentData(entity, new DominoColorRef { ColorRef = TextureAssetConversionSystem.TextureAssetRef });
			});
	}
}