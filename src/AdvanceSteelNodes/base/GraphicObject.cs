using Autodesk.DesignScript.Interfaces;
using Autodesk.DesignScript.Runtime;
using System.Collections.Generic;
using SteelServices = Dynamo.Applications.AdvanceSteel.Services;

namespace AdvanceSteel.Nodes
{
  /// <summary>
  /// This represents the Advance Steel Objects as graphical objects
  /// </summary>
  [IsVisibleInDynamoLibrary(false)]
  public abstract class GraphicObject : SteelDbObject, IGraphicItem
  {
    private const byte DefR = 101;
    private const byte DefG = 86;
    private const byte DefB = 130;
    private const byte DefA = 255;

    private static readonly object access_obj_graphic = new object();

    public virtual Autodesk.DesignScript.Geometry.Curve GetDynCurve() { return null; }
    public virtual IEnumerable<IGraphicItem> GetDynGeometry()
    {
      lock (access_obj_graphic)
      {
        using (var ctx = new SteelServices.DocContext())
        {
          return new List<IGraphicItem>() { GetDynCurve() };
        }
      }
    }

    public new void Tessellate(IRenderPackage package, TessellationParameters parameters)
    {
      var previousMeshVertexCount = package.MeshVertexCount;
      var previousLineVertexCount = package.LineVertexCount;

      foreach (var geometry in GetDynGeometry())
      {
        if (geometry == null)
          continue;

        geometry.Tessellate(package, parameters);
      }

      if (package is IRenderPackageSupplement packageSupplement)
      {
        int size = (package.MeshVertexCount - previousMeshVertexCount) * 4;
        if (size == 0) return;

        packageSupplement.AddTextureMapForMeshVerticesRange(previousMeshVertexCount, package.MeshVertexCount - 1, CreateColorByteArrayOfSize(size, DefR, DefG, DefB, DefA), size);

        if (package.LineVertexCount > previousLineVertexCount)
        {
          packageSupplement.UpdateLineVertexColorForRange(previousLineVertexCount, package.LineVertexCount - 1, DefR, DefG, DefB, DefA);
        }
      }
    }

    private static byte[] CreateColorByteArrayOfSize(int size, byte red, byte green, byte blue, byte alpha)
    {
      var arr = new byte[size * 4];
      for (var i = 0; i < arr.Length; i += 4)
      {
        arr[i] = red;
        arr[i + 1] = green;
        arr[i + 2] = blue;
        arr[i + 3] = alpha;
      }
      return arr;
    }
  }
}