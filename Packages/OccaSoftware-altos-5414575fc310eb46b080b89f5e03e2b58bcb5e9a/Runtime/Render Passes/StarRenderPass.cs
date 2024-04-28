using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace OccaSoftware.Altos.Runtime
{
  internal class StarRenderPass
  {
    private AltosSkyDirector skyDirector;
    private Material starMaterial;

    private Texture2D starTexture;
    private ComputeBuffer meshPropertiesBuffer;
    private ComputeBuffer argsBuffer;
    private bool initialized;

    private Material GetStarMaterial(AltosSkyDirector skyDirector)
    {
      if (starMaterial == null)
      {
        starMaterial = CoreUtils.CreateEngineMaterial(skyDirector.data.shaders.starShader);
      }

      return starMaterial;
    }

    private Mesh mesh = null;
    private Mesh Mesh
    {
      get
      {
        if (mesh == null)
          mesh = StaticHelpers.CreateQuad();

        return mesh;
      }
    }

    private Texture2D white = null;
    private Texture2D White
    {
      get
      {
        if (white == null)
        {
          white = new Texture2D(1, 1, TextureFormat.RGBA32, false);
          white.SetPixel(0, 0, Color.white);
          white.Apply();
        }

        return white;
      }
    }

    public StarRenderPass() { }

    StarDefinition data;

    public void Setup(AltosSkyDirector skyDirector, StarDefinition data)
    {
      this.skyDirector = skyDirector;
      this.data = data;
    }

    private void Init()
    {
      InitializeBuffers();
    }

    private void Cleanup()
    {
      argsBuffer?.Release();
      argsBuffer = null;
      meshPropertiesBuffer?.Release();
      meshPropertiesBuffer = null;

      if (white != null)
      {
        CoreUtils.Destroy(white);
        white = null;
      }

      if (mesh != null)
      {
        CoreUtils.Destroy(mesh);
        mesh = null;
      }

      if (starMaterial != null)
      {
        CoreUtils.Destroy(starMaterial);
        starMaterial = null;
      }
    }

    private void InitializeBuffers()
    {
      if (data == null)
        return;

      uint[] args = new uint[5] { 0, 0, 0, 0, 0 };
      args[0] = Mesh.GetIndexCount(0);
      args[1] = (uint)data.count;

      if (argsBuffer == null)
      {
        argsBuffer?.Release();
        argsBuffer = new ComputeBuffer(
          1,
          args.Length * sizeof(uint),
          ComputeBufferType.IndirectArguments
        );
      }

      argsBuffer.SetData(args);

      // Initialize buffer with the given population.
      MeshProperties[] meshPropertiesArray = new MeshProperties[data.count];
      UnityEngine.Random.InitState(data.seed);
      for (int i = 0; i < data.count; i++)
      {
        MeshProperties meshProperties = new MeshProperties();
        Vector3 position = UnityEngine.Random.onUnitSphere * 100f;
        Quaternion rotation = Quaternion.LookRotation(
          Vector3.zero - position,
          UnityEngine.Random.onUnitSphere
        );
        Vector3 scale = Vector3.one * UnityEngine.Random.Range(1f, 2f) * 0.1f * data.size;

        meshProperties.mat = Matrix4x4.TRS(position, rotation, scale);

        if (data.automaticColor)
        {
          float temperature = StaticHelpers.GetStarTemperature(UnityEngine.Random.Range(0f, 1f));
          meshProperties.color = StaticHelpers.GetBlackbodyColor(temperature);
        }
        else
        {
          meshProperties.color = new Vector3(1, 1, 1);
        }

        if (data.automaticBrightness)
        {
          meshProperties.brightness = StaticHelpers.GetStarBrightness(
            UnityEngine.Random.Range(0f, 1f)
          );
        }
        else
        {
          meshProperties.brightness = 1f;
        }

        meshProperties.id = UnityEngine.Random.Range(0f, 1f);
        meshPropertiesArray[i] = meshProperties;
      }

      if (meshPropertiesBuffer == null || meshPropertiesBuffer.count != data.count)
      {
        meshPropertiesBuffer?.Release();
        meshPropertiesBuffer = new ComputeBuffer(data.count, MeshProperties.Size());
      }

      meshPropertiesBuffer.SetData(meshPropertiesArray);
    }

    public void Draw(CommandBuffer cmd, SkyDefinition skyboxDefinition)
    {
      if (!initialized || data.IsDirty())
      {
        Init();
        initialized = true;
      }
      Material m = GetStarMaterial(skyDirector);
      SetProperties(skyboxDefinition, m);
      cmd.DrawMeshInstancedIndirect(Mesh, 0, GetStarMaterial(skyDirector), -1, argsBuffer);
    }

    public void Dispose()
    {
      Cleanup();
    }

    float GetTime(bool IsStatic, SkyDefinition skyboxDefinition)
    {
      return IsStatic || skyboxDefinition == null ? 0 : skyboxDefinition.CurrentTime;
    }

    private void SetProperties(SkyDefinition skyboxDefinition, Material m)
    {
      m.SetFloat(ShaderParams._EarthTime, GetTime(data.positionStatic, skyboxDefinition));
      m.SetFloat(ShaderParams._Brightness, data.GetBrightness());

      m.SetFloat(ShaderParams._FlickerFrequency, data.flickerFrequency);
      m.SetFloat(ShaderParams._FlickerStrength, data.flickerStrength);
      m.SetFloat(ShaderParams._Inclination, -data.inclination);
      m.SetColor(ShaderParams._StarColor, data.color);
      m.SetFloat(ShaderParams._EarthTime, GetTime(data.positionStatic, skyboxDefinition));

      starTexture = data.texture == null ? White : data.texture;
      m.SetTexture(ShaderParams._MainTex, starTexture);
      m.SetBuffer("altos_StarBuffer", meshPropertiesBuffer);
    }

    private struct MeshProperties
    {
      public Matrix4x4 mat;
      public Vector3 color;
      public float brightness;
      public float id;

      public static int Size()
      {
        return sizeof(float) * 4 * 4
          + // matrix
          sizeof(float) * 3
          + // color
          sizeof(float)
          + // brightness
          sizeof(float); // id
      }
    }

    private static class ShaderParams
    {
      public static int _EarthTime = Shader.PropertyToID("_EarthTime");
      public static int _Brightness = Shader.PropertyToID("_Brightness");
      public static int _FlickerFrequency = Shader.PropertyToID("_FlickerFrequency");
      public static int _FlickerStrength = Shader.PropertyToID("_FlickerStrength");
      public static int _MainTex = Shader.PropertyToID("_MainTex");
      public static int _Properties = Shader.PropertyToID("_Properties");
      public static int _Inclination = Shader.PropertyToID("_Inclination");
      public static int _StarColor = Shader.PropertyToID("_StarColor");
    }
  }
}
