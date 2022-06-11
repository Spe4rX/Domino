using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class PyramidSpawnerSystem : SystemBase
{
	protected override void OnCreate()
	{
		RequireForUpdate(GetEntityQuery(new EntityQueryDesc { All = new[] { ComponentType.ReadOnly<PyramidSpawner>() } }));
	}
	protected override void OnUpdate()
	{
		Entities
			.WithStructuralChanges()
			.ForEach(
			(
				Entity entity,
				in int entityInQueryIndex,
				in PyramidSpawner spawner) =>
			{

				//int count = (spawner.Layers - 1) * spawner.Layers + 1;
				int count = 0;
				int2 shape = new int2(1, 1);
				int2 add = new int2(1, 0);
				for (int i = 0; i < spawner.Layers; i++)
				{
					count += shape.x * shape.y;
					shape += add;
					add = add.yx;
				}

				var spawnedEntities = new NativeArray<Entity>(count, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
				EntityManager.Instantiate(spawner.Prefab, spawnedEntities);
				EntityManager.DestroyEntity(entity);

				Dependency = new SetSpawnedTranslation
				{
					TranslationFromEntity = GetComponentDataFromEntity<Translation>(),
					RotationFromEntity = GetComponentDataFromEntity<Rotation>(),
					Entities = spawnedEntities,
					LayerCountAddOne = spawner.Layers + 1,
					RotationOdd = new Rotation { Value = quaternion.Euler(0f, -45f * Mathf.Deg2Rad, 90f * Mathf.Deg2Rad) },
					RotationEven = new Rotation { Value = quaternion.Euler(0f, 45f * Mathf.Deg2Rad, 90f * Mathf.Deg2Rad) }

				}.Schedule(count, 16, Dependency);
				Dependency = spawnedEntities.Dispose(Dependency);

			}).Run();
	}
	[BurstCompile]
	struct SetSpawnedTranslation : IJobParallelFor
	{
		[NativeDisableParallelForRestriction]
		public ComponentDataFromEntity<Translation> TranslationFromEntity;

		[NativeDisableParallelForRestriction]
		public ComponentDataFromEntity<Rotation> RotationFromEntity;

		public NativeArray<Entity> Entities;

		public int LayerCountAddOne;

		public Rotation RotationOdd;
		public Rotation RotationEven;
		public void Execute(int i)
		{
			int layer = 1;
			int2 shape = new int2(1, 1);
			int2 add = new int2(1, 0);
			int acc = 0;
			while (acc + shape.x * shape.y <= i)
			{
				acc += shape.x * shape.y;
				shape += add;
				add = add.yx;
				layer++;
			}
			if (layer == 1)
			{
				TranslationFromEntity[Entities[i]] = new Translation()
				{
					Value = new float3(0f, (LayerCountAddOne - layer) * 1f, 0f)
				};
				RotationFromEntity[Entities[i]] = RotationOdd;
			}
			else
			{
				//int2 shape = new int2((layer >> 1) + 1, layer + 1 >> 1);
				int idxInLayer = i - acc;
				int2 pos = new int2(idxInLayer / shape.y, idxInLayer % shape.y);
				pos *= 2;
				float2 realPos = new float2(1f, 1f);
				realPos = pos - shape + realPos;
				realPos *= math.SQRT2;
				realPos *= 0.75f;
				TranslationFromEntity[Entities[i]] = new Translation()
				{
					Value = new float3(realPos.x, (LayerCountAddOne - layer) * 1f, realPos.y)
				};
				RotationFromEntity[Entities[i]] = layer % 2 == 0 ? RotationEven : RotationOdd;
			}
		}
	}
}