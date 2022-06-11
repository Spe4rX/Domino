using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class DominoColorSystem : SystemBase
{
	protected override void OnCreate()
	{
		RequireForUpdate(GetEntityQuery(new EntityQueryDesc
		{
			All = new[]
			{
				ComponentType.ReadOnly<DominoMaterialPropertyBaseColor>(),
			}
		}));
	}
	protected override void OnUpdate()
	{
		float t = Time.DeltaTime;
		Entities
			.ForEach(
			(
				ref DominoMaterialPropertyBaseColor property,
				ref DominoColorRef colorRef,
				in Translation translation) =>
			{
				//property.Value = new float4(translation.Value / 32f, 0f);
				//translation.Value.x += t * 1f;
				//float2 p = translation.Value.xz;// 50f;
				//p = math.clamp(p, float2.zero, new float2(1f, 1f));
				int2 size = colorRef.ColorRef.Value.Size;
				//int a = math.clamp((int)(p.x + p.y * s.y) * s.x, 0, s.x * s.y - 1);
				int2 pos = math.int2(translation.Value.xz) + size / 2;
				int n = math.clamp(pos.x + pos.y * size.x, 0, size.x * size.y - 1);
				//property.Value = colorRef.ColorRef.Value.ColorTable[n] + translation.Value.y / 32f;
				property.Value = TextureAssetConversionSystem.TextureAssetRef.Value.ColorTable[n] + translation.Value.y / 32f;
			}).ScheduleParallel();
	}
}
