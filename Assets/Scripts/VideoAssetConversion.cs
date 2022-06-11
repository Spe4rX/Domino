using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Video;

public class VideoAssetConversion : MonoBehaviour
{
	[SerializeField] private VideoPlayer _player;
	[SerializeField] private VideoClip _clip;

	private void OnEnable()
	{
		_player.sendFrameReadyEvents = true;
		_player.frameReady += OnFrameReady;
	}
	private void OnFrameReady(VideoPlayer source, long frameIdx)
	{
		var rendertex = source.texture as RenderTexture;
		RenderTexture.active = rendertex;
		Texture2D texture = new Texture2D(rendertex.width, rendertex.height, TextureFormat.RGBA32, false);
		//Texture2D texture = new Texture2D(64, 64, TextureFormat.RGBA32, false);
		texture.ReadPixels(new Rect(0f, 0f, rendertex.width, rendertex.height), 0, 0);
		//texture.ReadPixels(new Rect(0f, 0f, 64, 64), 0, 0);
		int fx = rendertex.width / 64;
		int fy = rendertex.height / 64;

		//Texture2D texture = new Texture2D(64, 64);
		//Debug.Log($"{fx},{fy}");
		//Debug.Log($"{rendertex.width},{rendertex.height}");

		using (BlobBuilder blobBuilder = new BlobBuilder(Allocator.Temp))
		{
			ref TextureAssetData blobAsset = ref blobBuilder.ConstructRoot<TextureAssetData>();
			//blobAsset.Size = new int2(texture.width, texture.height);
			blobAsset.Size = new int2(64, 64);

			BlobBuilderArray<float4> data = blobBuilder.Allocate(ref blobAsset.ColorTable, 64 * 64);

			var cs = texture.GetRawTextureData<Color32>();
			//var cs2 = texAuth.Texture.GetPixelData<Color>(0);
			//for (int i = 0; i < cs.Length; i++)
			//{
			//	var p = cs[i];
			//	data[i] = math.float4(p.r, p.g, p.b, p.a) / 256f;
			//}
			for (int y = 0; y < 64; y++)
			{
				for (int x = 0; x < 64; x++)
				{
					Debug.Assert(x * fx + y * fy * rendertex.width < cs.Length, $"{y},{x},{fx},{fy},{rendertex.width},{rendertex.height}");
					var p = cs[x * fx + y * fy * rendertex.width];
					data[y * 64 + x] = math.float4(p.r, p.g, p.b, p.a) / 256f;
				}
			}

			cs.Dispose();

			TextureAssetConversionSystem.TextureAssetRef = blobBuilder.CreateBlobAssetReference<TextureAssetData>(Allocator.Persistent);
		}
	}
	[ContextMenu("Play")]
	private void Play()
	{
		Application.targetFrameRate = 30;
		_player.Play();
	}
	[ContextMenu("Stop")]
	private void Stop()
	{
		_player.Stop();
	}
}
