using Autodesk.DesignScript.Interfaces;
using Autodesk.DesignScript.Runtime;

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

    public abstract Autodesk.DesignScript.Geometry.Curve GetDynCurve();

    public new void Tessellate(IRenderPackage package, TessellationParameters parameters)
    {
      using (var crv = GetDynCurve())
      {
        crv.Tessellate(package, parameters);
        package.ApplyLineVertexColors(CreateColorByteArrayOfSize(package.LineVertexCount, DefR, DefG, DefB, DefA));
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