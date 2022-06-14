using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.Geometry;
using System.Collections.Generic;
using System.Linq;
using SteelServices = Dynamo.Applications.AdvanceSteel.Services;
using Autodesk.DesignScript.Runtime;
using ASGrid = Autodesk.AdvanceSteel.Modelling.Grid;
using ASGrid1D = Autodesk.AdvanceSteel.Modelling.Grid1D;

namespace AdvanceSteel.Nodes.Miscellaneous
{
  /// <summary>
  /// Advance Steel Grids
  /// </summary>
  [DynamoServices.RegisterForTrace]
  public class Grid : GraphicObject
  {
    private Grid(List<Property> gridProperties, double length, double width = 0, int noOfAxis = 0)
    {
      SafeInit(() => InitGrid(gridProperties, length, width, noOfAxis));
    }

    private Grid(ASGrid grid)
    {
      SafeInit(() => SetHandle(grid));
    }

    internal static Grid FromExisting(ASGrid grid)
    {
      return new Grid(grid)
      {
        IsOwnedByDynamo = false
      };
    }

    private void InitGrid(List<Property> gridProperties, double length, double width = 0, int noOfAxis = 0)
    {
      List<Property> defaultData = gridProperties.Where(x => x.Level == LevelEnum.Default).ToList<Property>();
      List<Property> postWriteDBData = gridProperties.Where(x => x.Level == LevelEnum.PostWriteDB).ToList<Property>();

      Matrix3d gridMat = (Matrix3d)defaultData.FirstOrDefault<Property>(x => x.MemberName == nameof(ASGrid.CS)).InternalValue;

      ASGrid myGrid = SteelServices.ElementBinder.GetObjectASFromTrace<ASGrid>();
      if (myGrid == null)
      {
        switch (noOfAxis)
        {
          case 0:
            myGrid = new ASGrid1D(gridMat, length);
            break;
          default:
            myGrid = new ASGrid1D(gridMat, length, width, noOfAxis);
            break;
        }
        if (defaultData != null)
        {
          UtilsProperties.SetParameters(myGrid, defaultData);
        }

        myGrid.WriteToDb();
      }
      else
      {
        if (!myGrid.IsKindOf(FilerObject.eObjectType.kGrid))
          throw new System.Exception("Not a Grid");

        myGrid.updateWidth(length);
        if (width > 0)
        {
          myGrid.updateLength(width);
          myGrid.setNumElementPerSequence(0, noOfAxis);
        }

        if (defaultData != null)
        {
          UtilsProperties.SetParameters(myGrid, defaultData);
        }
      }

      SetHandle(myGrid);

      if (postWriteDBData != null)
      {
        UtilsProperties.SetParameters(myGrid, postWriteDBData);
      }

      SteelServices.ElementBinder.CleanupAndSetElementForTrace(myGrid);
    }

    /// <summary>
    /// Create an Advance Steel Single Grid (Y Axis of CS)
    /// </summary>
    /// <param name="coordinateSystem">Input Dynamo Coordinate System - Bottom Left of grid system</param>
    /// <param name="length">Input Grid Length</param>
    /// <param name="additionalGridParameters"> Optional Input Grid Build Properties </param>
    /// <returns name="grid"> grid</returns>
    public static Grid ByCSByLength(Autodesk.DesignScript.Geometry.CoordinateSystem coordinateSystem,
                                  double length,
                                  [DefaultArgument("null")] List<Property> additionalGridParameters)
    {
      Matrix3d gridMat = Utils.ToAstMatrix3d(coordinateSystem, true);
      additionalGridParameters = PreSetDefaults(additionalGridParameters, gridMat);

      return new Grid(additionalGridParameters, length);
    }

    /// <summary>
    /// Create an Advance Steel Grid
    /// </summary>
    /// <param name="coordinateSystem">Input Dynamo Coordinate System - Bottom Left of grid system</param>
    /// <param name="length">Input Grid Length</param>
    /// <param name="width">Input Grid Width</param>
    /// <param name="noOfGrids">Input the number of Grids</param>
    /// <param name="additionalGridParameters"> Optional Input Grid Build Properties </param>
    /// <returns name="camera"> camera</returns>
    public static Grid ByCSLengthWidthNoOfAxis(Autodesk.DesignScript.Geometry.CoordinateSystem coordinateSystem,
                                  double length,
                                  double width,
                                  int noOfGrids,
                                  [DefaultArgument("null")] List<Property> additionalGridParameters)
    {
      Matrix3d cameraMat = Utils.ToAstMatrix3d(coordinateSystem, true);
      additionalGridParameters = PreSetDefaults(additionalGridParameters, cameraMat);

      return new Grid(additionalGridParameters, length, width, noOfGrids);
    }

    /// <summary>
    /// Set Advance Steel Grid Length
    /// </summary>
    /// <param name="steelObject"> Selected Advance Steel Grid Object</param>
    /// <param name="length"> Set Grid Length in X Direction of CS</param>
    public static void SetGridLength(SteelDbObject steelObject,
                                    [DefaultArgument("0")] double length)
    {
      using (var ctx = new SteelServices.DocContext())
      {
        string handle = steelObject.Handle;
        FilerObject obj = Utils.GetObject(handle);

        if (obj != null && obj.IsKindOf(FilerObject.eObjectType.kGrid))
        {
          Autodesk.AdvanceSteel.Modelling.Grid myGrid = obj as Autodesk.AdvanceSteel.Modelling.Grid;
          myGrid.updateWidth(length);
        }
        else
          throw new System.Exception("Failed to Get Grid Object");
      }
    }

    /// <summary>
    /// Set Advance Steel Grid Width
    /// </summary>
    /// <param name="steelObject"> Selected Advance Steel Grid Object</param>
    /// <param name="width"> Set Grid Length in X Direction of CS</param>
    public static void SetGridWidth(SteelDbObject steelObject,
                                    [DefaultArgument("0")] double width)
    {
      using (var ctx = new SteelServices.DocContext())
      {
        string handle = steelObject.Handle;
        FilerObject obj = Utils.GetObject(handle);

        if (obj != null && obj.IsKindOf(FilerObject.eObjectType.kGrid))
        {
          Autodesk.AdvanceSteel.Modelling.Grid myGrid = obj as Autodesk.AdvanceSteel.Modelling.Grid;
          myGrid.updateLength(width);
        }
        else
          throw new System.Exception("Failed to Get Grid Object");
      }
    }

    private static List<Property> PreSetDefaults(List<Property> listGridData, Matrix3d gridCS)
    {
      if (listGridData == null)
      {
        listGridData = new List<Property>() { };
      }
      UtilsProperties.CheckListUpdateOrAddValue(typeof(ASGrid), listGridData, nameof(ASGrid.CS), gridCS);
      return listGridData;
    }

    [IsVisibleInDynamoLibrary(false)]
    public override IEnumerable<Autodesk.DesignScript.Interfaces.IGraphicItem> GetDynGeometry()
    {
      IList<Autodesk.DesignScript.Interfaces.IGraphicItem> ret = new List<Autodesk.DesignScript.Interfaces.IGraphicItem>();

      var myGrid = Utils.GetObject(Handle) as ASGrid;

      Matrix3d gridCS = myGrid.CS;
      Vector3d xVect = null;
      Vector3d yVect = null;
      Vector3d ZVect = null;
      Point3d origin = null;
      gridCS.GetCoordSystem(out origin, out xVect, out yVect, out ZVect);

      Autodesk.AdvanceSteel.Modelling.GridElement[] gridEles = null;
      myGrid.GetAllElements(out gridEles);


      foreach (var item in gridEles)
      {
        Curve3d curve = null;
        item.GetCurve(ref curve, gridCS);

        if (curve != null)
        {
          Point3d sp = null;
          Point3d ep = null;
          if (curve.HasStartPoint(out sp))
          {
            if (curve.HasEndPoint(out ep))
            {
              var line1 = Autodesk.DesignScript.Geometry.Line.ByStartPointEndPoint(Utils.ToDynPoint(sp, true), Utils.ToDynPoint(ep, true));
              ret.Add(line1);
            }
          }
        }
      }

      return ret;
    }

  }
}