using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class PyramidSpawnerAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
	[SerializeField] private GameObject _prefab;
	[SerializeField] private int _layers;
	public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
	{
		dstManager.AddComponentData(entity, new PyramidSpawner
		{
			Prefab = conversionSystem.GetPrimaryEntity(_prefab),
			Layers = _layers
		});
	}
	public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
	{
		referencedPrefabs.Add(_prefab);
	}
}

struct PyramidSpawner : IComponentData
{
	public Entity Prefab;
	public int Layers;
}
