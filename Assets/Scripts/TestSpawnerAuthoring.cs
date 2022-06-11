using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class TestSpawnerAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
	[SerializeField] private GameObject _prefab;
	[SerializeField] private int _spawnCount = 10;
	[SerializeField] private float _offset = 1f;
	public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
	{
		dstManager.AddComponentData(entity, new TestSpawner { Prefab = conversionSystem.GetPrimaryEntity(_prefab), Count = _spawnCount, Offset = _offset });
		//dstManager.AddBuffer<TestLineSpawnerBufferElement>(entity).Add(new TestLineSpawnerBufferElement { Prefab = new EntityPrefabReference(_prefabs) });
	}

	public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
	{
		referencedPrefabs.Add(_prefab);
	}
}

struct TestSpawner : IComponentData
{
	public Entity Prefab;
	public int Count;
	public float Offset;
}
