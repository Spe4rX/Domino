using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

[MaterialProperty("_BaseColor", MaterialPropertyFormat.Float4)]
[GenerateAuthoringComponent]
public struct DominoMaterialPropertyBaseColor : IComponentData { public float4 Value; }