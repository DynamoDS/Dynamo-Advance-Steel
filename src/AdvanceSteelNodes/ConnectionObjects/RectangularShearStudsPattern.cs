using System;
using AdvanceSteel.Nodes.Plates;
using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.DesignScript.Runtime;
using DynGeometry = Autodesk.DesignScript.Geometry;
using SteelGeometry = Autodesk.AdvanceSteel.Geometry;
using SteelServices = Dynamo.Applications.AdvanceSteel.Services;
using Autodesk.AdvanceSteel.ConstructionTypes;
using System.Collections.Generic;
using Autodesk.AdvanceSteel.Geometry;
using System.Linq;

namespace AdvanceSteel.Nodes.ConnectionObjects.ShearStuds
{
  /// <summary>
  /// Advance Steel Rectangular Shear Stud Pattern
  /// </summary>
  [DynamoServices.RegisterForTrace]
  public class RectangularShearStudsPattern : GraphicObject
  {
    internal RectangularShearStudsPattern()
    {
    }

    internal RectangularShearStudsPattern(SteelGeometry.Point3d astPoint1, SteelGeometry.Point3d astPoint2, string handleToConnect,
                                          SteelGeometry.Vector3d vx, SteelGeometry.Vector3d vy,
                                          SteelGeometry.Matrix3d coordSyst,
                                          List<ASProperty> shearStudData, int boltCon)
    {
      lock (access_obj)
      {
        List<ASProperty> defaultShearStudData = shearStudData.Where(x => x.PropLevel == ".").ToList<ASProperty>();
        List<ASProperty> arrangerShearStudData = shearStudData.Where(x => x.PropLevel == "Arranger").ToList<ASProperty>();
        List<ASProperty> postWriteDBData = shearStudData.Where(x => x.PropLevel == "Z_PostWriteDB").ToList<ASProperty>();

        int temp_nx = (int)arrangerShearStudData.FirstOrDefault<ASProperty>(x => x.PropName == "Nx").PropValue;
        int temp_ny = (int)arrangerShearStudData.FirstOrDefault<ASProperty>(x => x.PropName == "Ny").PropValue;

        var dx = Utils.GetRectangleLength(astPoint1, astPoint2, vx) / (temp_nx - 1);
        Utils.CheckListUpdateOrAddValue(arrangerShearStudData, "Dx", dx, "Arranger");

        var dy = Utils.GetRectangleHeight(astPoint1, astPoint2, vx) / (temp_ny - 1);
        Utils.CheckListUpdateOrAddValue(arrangerShearStudData, "Dy", dy, "Arranger");

        using (var ctx = new SteelServices.DocContext())
        {
          Autodesk.AdvanceSteel.Modelling.Connector shearStuds = null;
          string handle = SteelServices.ElementBinder.GetHandleFromTrace();
          if (string.IsNullOrEmpty(handle) || Utils.GetObject(handle) == null)
          {

            shearStuds = new Autodesk.AdvanceSteel.Modelling.Connector();
            Autodesk.AdvanceSteel.Arrangement.Arranger arranger = new Autodesk.AdvanceSteel.Arrangement.RectangularArranger(Matrix2d.kIdentity, dx, dy, temp_nx, temp_ny);
            shearStuds.Arranger = arranger;

            if (defaultShearStudData != null)
            {
              Utils.SetParameters(shearStuds, defaultShearStudData);
            }

            Utils.SetParameters(shearStuds.Arranger, arrangerShearStudData);

            shearStuds.WriteToDb();

            if (postWriteDBData != null)
            {
              Utils.SetParameters(shearStuds, postWriteDBData);
            }

          }
          else
          {
            shearStuds = Utils.GetObject(handle) as Autodesk.AdvanceSteel.Modelling.Connector;
            if (shearStuds != null || shearStuds.IsKindOf(FilerObject.eObjectType.kConnector))
            {

              if (defaultShearStudData != null)
              {
                Utils.SetParameters(shearStuds, defaultShearStudData);
              }

              Utils.SetParameters(shearStuds.Arranger, arrangerShearStudData);

              if (postWriteDBData != null)
              {
                Utils.SetParameters(shearStuds, postWriteDBData);
              }
            }
            else
              throw new System.Exception("Not a shear stud pattern");
          }

          FilerObject obj = Utils.GetObject(handleToConnect);
          Autodesk.AdvanceSteel.Modelling.WeldPoint weld = shearStuds.Connect(obj, coordSyst);
          weld.AssemblyLocation = (AtomicElement.eAssemblyLocation)boltCon;

          Handle = shearStuds.Handle;
          SteelServices.ElementBinder.CleanupAndSetElementForTrace(shearStuds);
        }
      }
    }

    internal RectangularShearStudsPattern(string handleToConnect,
                                          SteelGeometry.Matrix3d coordSyst,
                                          List<ASProperty> shearStudData,
                                          int boltCon)
    {
      lock (access_obj)
      {
        using (var ctx = new SteelServices.DocContext())
        {
          List<ASProperty> defaultShearStudData = shearStudData.Where(x => x.PropLevel == ".").ToList<ASProperty>();
          List<ASProperty> arrangerShearStudData = shearStudData.Where(x => x.PropLevel == "Arranger").ToList<ASProperty>();
          List<ASProperty> postWriteDBData = shearStudData.Where(x => x.PropLevel == "Z_PostWriteDB").ToList<ASProperty>();

          Autodesk.AdvanceSteel.Modelling.Connector shearStuds = null;
          string handle = SteelServices.ElementBinder.GetHandleFromTrace();
          if (string.IsNullOrEmpty(handle) || Utils.GetObject(handle) == null)
          {

            double temp_Dx = (double)arrangerShearStudData.FirstOrDefault<ASProperty>(x => x.PropName == "Dx").PropValue;
            double temp_Dy = (double)arrangerShearStudData.FirstOrDefault<ASProperty>(x => x.PropName == "Dy").PropValue;
            int temp_nx = (int)arrangerShearStudData.FirstOrDefault<ASProperty>(x => x.PropName == "Nx").PropValue;
            int temp_ny = (int)arrangerShearStudData.FirstOrDefault<ASProperty>(x => x.PropName == "Ny").PropValue;

            shearStuds = new Autodesk.AdvanceSteel.Modelling.Connector();
            Autodesk.AdvanceSteel.Arrangement.Arranger arranger = new Autodesk.AdvanceSteel.Arrangement.RectangularArranger(Matrix2d.kIdentity, temp_Dx, temp_Dy, temp_nx, temp_ny);
            shearStuds.Arranger = arranger;

            if (defaultShearStudData != null)
            {
              Utils.SetParameters(shearStuds, defaultShearStudData);
            }

            Utils.SetParameters(shearStuds.Arranger, arrangerShearStudData);

            shearStuds.WriteToDb();

            if (postWriteDBData != null)
            {
              Utils.SetParameters(shearStuds, postWriteDBData);
            }
          }
          else
          {
            shearStuds = Utils.GetObject(handle) as Autodesk.AdvanceSteel.Modelling.Connector;
            if (shearStuds != null || shearStuds.IsKindOf(FilerObject.eObjectType.kConnector))
            {

              if (defaultShearStudData != null)
              {
                Utils.SetParameters(shearStuds, defaultShearStudData);
              }

              Utils.SetParameters(shearStuds.Arranger, arrangerShearStudData);

              if (postWriteDBData != null)
              {
                Utils.SetParameters(shearStuds, postWriteDBData);
              }
            }
            else
              throw new System.Exception("Not a shear stud pattern");
          }

          FilerObject obj = Utils.GetObject(handleToConnect);
          Autodesk.AdvanceSteel.Modelling.WeldPoint weld = shearStuds.Connect(obj, coordSyst);
          weld.AssemblyLocation = (AtomicElement.eAssemblyLocation)boltCon;

          Handle = shearStuds.Handle;
          SteelServices.ElementBinder.CleanupAndSetElementForTrace(shearStuds);
        }
      }
    }

    /// <summary>
    /// Create an Advance Steel Rectangular Shear Stud Pattern By Rectangle
    /// </summary>
    /// <param name="rectangle"> Input Dynamo Rectangle </param>
    /// <param name="objectToConnect"> Object to attached ShearStud </param>
    /// <param name="studLength"> Input Shear Stud Length</param>
    /// <param name="studDiameter"> Input Shear Stud Diameter</param>
    /// <param name="noOfShearStudsX"> Input Number of Shear Stud X Direction</param>
    /// <param name="noOfShearStudsY"> Input Number of Shear Stud Y Direction</param>
    /// <param name="shearStudConnectionType"> Input Shear Bolt Connection type - Shop Bolt Default</param>
    /// <param name="additionalShearStudParameters"> Optional Input ShearStud Build Properties </param>
    public static RectangularShearStudsPattern ByRectangle(Autodesk.DesignScript.Geometry.Rectangle rectangle,
                                                            SteelDbObject objectToConnect,
                                                            double studLength,
                                                            double studDiameter,
                                                            [DefaultArgument("2;")] int noOfShearStudsX,
                                                            [DefaultArgument("2;")] int noOfShearStudsY,
                                                            [DefaultArgument("2;")] int shearStudConnectionType,
                                                            [DefaultArgument("null")] List<ASProperty> additionalShearStudParameters)
    {
      var dynCorners = rectangle.Corners();
      var astCorners = Utils.ToAstPoints(dynCorners, true);
      var vx = astCorners[1] - astCorners[0];
      var vy = astCorners[3] - astCorners[0];
      var vz = Utils.ToAstVector3d(rectangle.Normal, true);

      vx = vx.Normalize();
      vy = vy.Normalize();
      vz = vz.Normalize();

      List<SteelDbObject> tempList = new List<SteelDbObject>() { objectToConnect };
      List<string> handlesList = Utils.GetSteelDbObjectsToConnect(tempList);

      var rectangleCenter = astCorners[0] + (astCorners[2] - astCorners[0]) * 0.5;
      Matrix3d matrix3D = new Matrix3d();
      matrix3D.SetCoordSystem(rectangleCenter, vx, vy, vz);

      PreSetValuesInListProps(additionalShearStudParameters, noOfShearStudsX, noOfShearStudsY, Utils.ToInternalUnits(studLength, true), Utils.ToInternalUnits(studDiameter, true));

      return new RectangularShearStudsPattern(astCorners[0], astCorners[2], handlesList[0], vx, vy, matrix3D,
                                              additionalShearStudParameters, shearStudConnectionType);
    }

    /// <summary>
    /// Create an Advance Steel Rectangular Shear Stud Pattern at a Point
    /// </summary>
    /// <param name="connectionPoint"> Input Insertion point of Bolt Pattern </param>
    /// <param name="shearStudCS"> Input Bolt Coordinate System </param>
    /// <param name="objectToConnect"> Object to attached ShearStud </param>
    /// <param name="studLength"> Input Shear Stud Length</param>
    /// <param name="studDiameter"> Input Shear Stud Diameter</param>
    /// <param name="noOfShearStudsX"> Input Number of Shear Stud X Direction</param>
    /// <param name="noOfShearStudsY"> Input Number of Shear Stud Y Direction</param>
    /// <param name="studSpacingX"> Input Spacing of Shear Stud X Direction</param>
    /// <param name="studSpacingY"> Input Spacing of Shear Stud Y Direction</param>
    /// <param name="shearStudConnectionType"> Input Shear Bolt Connection type - Shop Bolt Default</param>
    /// <param name="additionalShearStudParameters"> Optional Input ShearStud Build Properties </param>
    /// <returns></returns>
    public static RectangularShearStudsPattern AtCentrePoint(Autodesk.DesignScript.Geometry.Point connectionPoint,
                                                            Autodesk.DesignScript.Geometry.CoordinateSystem shearStudCS,
                                                            SteelDbObject objectToConnect,
                                                            double studLength,
                                                            double studDiameter,
                                                            [DefaultArgument("2;")] int noOfShearStudsX,
                                                            [DefaultArgument("2;")] int noOfShearStudsY,
                                                            double studSpacingX,
                                                            double studSpacingY,
                                                            [DefaultArgument("2;")] int shearStudConnectionType,
                                                            [DefaultArgument("null")] List<ASProperty> additionalShearStudParameters)
    {
      List<SteelDbObject> tempList = new List<SteelDbObject>() { objectToConnect };
      List<string> handlesList = Utils.GetSteelDbObjectsToConnect(tempList);

      var vx = Utils.ToAstVector3d(shearStudCS.XAxis, true);
      var vy = Utils.ToAstVector3d(shearStudCS.YAxis, true);
      var vz = Utils.ToAstVector3d(shearStudCS.ZAxis, true);

      Matrix3d matrix3D = new Matrix3d();
      matrix3D.SetCoordSystem(Utils.ToAstPoint(connectionPoint, true), vx, vy, vz);

      PreSetValuesInListProps(additionalShearStudParameters,
                              noOfShearStudsX, noOfShearStudsY,
                              Utils.ToInternalUnits(studLength, true), Utils.ToInternalUnits(studDiameter, true));

      Utils.CheckListUpdateOrAddValue(additionalShearStudParameters, "Dx", Utils.ToInternalUnits(studSpacingX, true), "Arranger");
      Utils.CheckListUpdateOrAddValue(additionalShearStudParameters, "Dy", Utils.ToInternalUnits(studSpacingY, true), "Arranger");

      return new RectangularShearStudsPattern(handlesList[0], matrix3D, additionalShearStudParameters, shearStudConnectionType);
    }

    private static void PreSetValuesInListProps(List<ASProperty> listOfBoltParameters, int nx, int ny,
                                                double studLength, double studDiameter)
    {
      if (listOfBoltParameters == null)
      {
        listOfBoltParameters = new List<ASProperty>() { };
      }

      Utils.CheckListUpdateOrAddValue(listOfBoltParameters, "Nx", nx, "Arranger");
      Utils.CheckListUpdateOrAddValue(listOfBoltParameters, "Ny", ny, "Arranger");
      Utils.CheckListUpdateOrAddValue(listOfBoltParameters, "Length", studLength, ".");
      Utils.CheckListUpdateOrAddValue(listOfBoltParameters, "Diameter", studDiameter, ".");
    }

    [IsVisibleInDynamoLibrary(false)]
    public override Autodesk.DesignScript.Geometry.Curve GetDynCurve()
    {
      lock (access_obj)
      {
        using (var ctx = new SteelServices.DocContext())
        {
          var shearStud = Utils.GetObject(Handle) as Autodesk.AdvanceSteel.Modelling.Connector;
          if (shearStud == null)
          {
            throw new Exception("Null shear stud pattern");
          }

          var coordSystem = shearStud.CS;
          // Vx and Vy direction
          var tempVx = new Vector3d(coordSystem.Values[0][0], coordSystem.Values[1][0], coordSystem.Values[2][0]);
          var tempVy = new Vector3d(coordSystem.Values[0][1], coordSystem.Values[1][1], coordSystem.Values[2][1]);

          var tempXlen = shearStud.Arranger.Dx * (shearStud.Arranger.Nx - 1) / 2.0;
          var tempYlen = shearStud.Arranger.Dy * (shearStud.Arranger.Ny - 1) / 2.0;

          var temp1 = tempVx * tempXlen;
          var temp2 = tempVy * tempYlen;

          var pt1 = new SteelGeometry.Point3d(shearStud.CenterPoint);
          pt1.Add(temp1 + temp2);

          var pt2 = new SteelGeometry.Point3d(shearStud.CenterPoint);
          pt2.Add(temp1 - temp2);

          var pt3 = new SteelGeometry.Point3d(shearStud.CenterPoint);
          pt3.Add(-temp1 - temp2);

          var pt4 = new SteelGeometry.Point3d(shearStud.CenterPoint);
          pt4.Add(-temp1 + temp2);

          {
            List<DynGeometry.Point> polyPoints = new List<DynGeometry.Point>
            {
              Utils.ToDynPoint(pt1, true),
              Utils.ToDynPoint(pt2, true),
              Utils.ToDynPoint(pt3, true),
              Utils.ToDynPoint(pt4, true)
            };

            return Autodesk.DesignScript.Geometry.Polygon.ByPoints(polyPoints);
          }
        }
      }
    }

  }
}
