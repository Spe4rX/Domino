using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public partial class TestLineSpawnerSystem : SystemBase
{
	protected override void OnCreate()
	{
		RequireForUpdate(GetEntityQuery(new EntityQueryDesc
		{
			All = new[]
			{
				ComponentType.ReadOnly<TestSpawner>(),
			}
		}));
	}
	protected override void OnUpdate()
	{
		Entities
			.WithStructuralChanges()
			.ForEach(
			(
				Entity entity,
				in int entityInQueryIndex,
				in TestSpawner spawner) =>
			{
				var spawnedEntities = new NativeArray<Entity>(spawner.Count, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
				EntityManager.Instantiate(spawner.Prefab, spawnedEntities);
				EntityManager.DestroyEntity(entity);

				var translationFromEntity = GetComponentDataFromEntity<Translation>();
				Dependency = new SetSpawnedTranslation
				{
					TranslationFromEntity = translationFromEntity,
					Entities = spawnedEntities,
					Offset = spawner.Offset,
					Count = spawner.Count,
				}.Schedule(spawner.Count, 16, Dependency);
				Dependency = spawnedEntities.Dispose(Dependency);

			}).Run();
	}
	[BurstCompile]
	struct SetSpawnedTranslation : IJobParallelFor
	{
		[NativeDisableParallelForRestriction]
		public ComponentDataFromEntity<Translation> TranslationFromEntity;

		public NativeArray<Entity> Entities;

		public float Offset;
		public int Count;
		public void Execute(int i)
		{
			var entity = Entities[i];

			TranslationFromEntity[entity] = new Translation() { Value = new float3((i % 64) * Offset - 64 * Offset / 2, 1f, (i / 64) * Offset - Count / 64 * Offset / 2) };
		}
	}
}
