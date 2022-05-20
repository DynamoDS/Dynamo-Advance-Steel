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
using ASConnector = Autodesk.AdvanceSteel.Modelling.Connector;
using Autodesk.AdvanceSteel.Arrangement;
using Autodesk.AdvanceSteel.Contours;

namespace AdvanceSteel.Nodes.ConnectionObjects.ShearStuds
{
  /// <summary>
  /// Advance Steel Circular Shear Stud Pattern
  /// </summary>
  [DynamoServices.RegisterForTrace]
  public class CircularShearStudsPattern : GraphicObject
  {
    private CircularShearStudsPattern(string handleToConnect,
                                        SteelGeometry.Matrix3d coordSyst,
                                        List<Property> shearStudData,
                                        int boltCon)
    {
      SafeInit(() => InitCircularShearStudsPattern(handleToConnect, coordSyst, shearStudData, boltCon));
    }

    private CircularShearStudsPattern(ASConnector shearStuds)
    {
      SafeInit(() => SetHandle(shearStuds));
    }

    internal static CircularShearStudsPattern FromExisting(ASConnector shearStuds)
    {
      return new CircularShearStudsPattern(shearStuds)
      {
        IsOwnedByDynamo = false
      };
    }

    private void InitCircularShearStudsPattern(string handleToConnect,
                                        SteelGeometry.Matrix3d coordSyst,
                                        List<Property> shearStudData,
                                        int boltCon)
    {
      List<Property> defaultShearStudData = shearStudData.Where(x => x.Level == LevelEnum.Default).ToList<Property>();
      List<Property> arrangerShearStudData = shearStudData.Where(x => x.Level == LevelEnum.Arranger).ToList<Property>();
      List<Property> postWriteDBData = shearStudData.Where(x => x.Level == LevelEnum.PostWriteDB).ToList<Property>();

      ASConnector shearStuds = SteelServices.ElementBinder.GetObjectASFromTrace<ASConnector>();
      if (shearStuds == null)
      {
        var temp_radius = (double)arrangerShearStudData.FirstOrDefault<Property>(x => x.Name == nameof(CircleArranger.Radius)).InternalValue;
        var temp_noss = (int)arrangerShearStudData.FirstOrDefault<Property>(x => x.Name == nameof(CircleArranger.NumberOfElements)).InternalValue;

        shearStuds = new ASConnector();
        shearStuds.Arranger = new CircleArranger(Matrix2d.kIdentity, temp_radius, temp_noss);

        if (defaultShearStudData != null)
        {
          Utils.SetParameters(shearStuds, defaultShearStudData);
        }

        Utils.SetParameters(shearStuds.Arranger, arrangerShearStudData);

        shearStuds.WriteToDb();
      }
      else
      {
        if (!shearStuds.IsKindOf(FilerObject.eObjectType.kConnector))
          throw new System.Exception("Not a shear stud pattern");

        if (defaultShearStudData != null)
        {
          Utils.SetParameters(shearStuds, defaultShearStudData);
        }

        Utils.SetParameters(shearStuds.Arranger, arrangerShearStudData);
      }
    
      SetHandle(shearStuds);

      if (postWriteDBData != null)
      {
        Utils.SetParameters(shearStuds, postWriteDBData);
      }

      FilerObject obj = Utils.GetObject(handleToConnect);
      Autodesk.AdvanceSteel.Modelling.WeldPoint weld = shearStuds.Connect(obj, coordSyst);
      weld.AssemblyLocation = (AtomicElement.eAssemblyLocation)boltCon;

      SteelServices.ElementBinder.CleanupAndSetElementForTrace(shearStuds);
    }

    /// <summary>
    /// Create an Advance Steel Shear Stud Pattern By Circle
    /// </summary>
    /// <param name="circle"> Input Dynamo Circle </param>
    /// <param name="referenceVector"> Input Dynamo Vector for alignment of circle</param>
    /// <param name="objectToConnect"> Object to attached ShearStud </param>
    /// <param name="studLength"> Input Shear Stud Length</param>
    /// <param name="studDiameter"> Input Shear Stud Diameter</param>
    /// <param name="noOfShearStudsInCircle"> Input Number of Shear Stud to be placed in the Circle Pattern</param>
    /// <param name="shearStudConnectionType"> Input Shear Bolt Connection type - Shop Bolt Default</param>
    /// <param name="additionalShearStudParameters"> Optional Input ShearStud Build Properties </param>
    /// <returns name="circularShearStudsPattern"> shear studs</returns>
    public static CircularShearStudsPattern ByCircle(Autodesk.DesignScript.Geometry.Circle circle,
                                                      Autodesk.DesignScript.Geometry.Vector referenceVector,
                                                      SteelDbObject objectToConnect,
                                                      double studLength,
                                                      double studDiameter,
                                                      [DefaultArgument("9;")] int noOfShearStudsInCircle,
                                                      [DefaultArgument("2;")] int shearStudConnectionType,
                                                      [DefaultArgument("null")] List<Property> additionalShearStudParameters)
    {
      var norm = Utils.ToAstVector3d(circle.Normal, true);
      var vx = Utils.ToAstVector3d(referenceVector, true);
      var vy = norm.CrossProduct(vx);
      var vz = norm;

      vx = vx.Normalize();
      vy = vy.Normalize();
      vz = vz.Normalize();

      List<SteelDbObject> tempList = new List<SteelDbObject>() { objectToConnect };
      List<string> handlesList = Utils.GetSteelDbObjectsToConnect(tempList);

      Matrix3d matrix3D = new Matrix3d();
      matrix3D.SetCoordSystem(Utils.ToAstPoint(circle.CenterPoint, true), vx, vy, vz);

      additionalShearStudParameters = PreSetValuesInListProps(additionalShearStudParameters, noOfShearStudsInCircle, Utils.ToInternalDistanceUnits(circle.Radius, true), Utils.ToInternalDistanceUnits(studLength, true), Utils.ToInternalDistanceUnits(studDiameter, true));

      return new CircularShearStudsPattern(handlesList[0], matrix3D, additionalShearStudParameters, shearStudConnectionType);
    }

    /// <summary>
    /// Create an Advance Steel Circular Shear Stud Pattern at a Point
    /// </summary>
    /// <param name="connectionPoint"> Input Insertion point of Bolt Pattern </param>
    /// <param name="shearStudCS"> Input Coordinate System </param>
    /// <param name="patternRadius"> Input Shear Stud pattern Radius</param>
    /// <param name="objectToConnect"> Object to attached ShearStud </param>
    /// <param name="studLength"> Input Shear Stud Length</param>
    /// <param name="studDiameter"> Input Shear Stud Diameter</param>
    /// <param name="noOfShearStudsInCircle"> Input Number of Shear Stud to be placed in the Circle Pattern</param>
    /// <param name="shearStudConnectionType"> Input Shear Bolt Connection type - Shop Bolt Default</param>
    /// <param name="additionalShearStudParameters"> Optional Input ShearStud Build Properties </param>
    /// <returns name="circularShearStudsPattern"> shear studs</returns>
    public static CircularShearStudsPattern AtCentrePoint(Autodesk.DesignScript.Geometry.Point connectionPoint,
                                                        Autodesk.DesignScript.Geometry.CoordinateSystem shearStudCS,
                                                        double patternRadius,
                                                        SteelDbObject objectToConnect,
                                                        double studLength,
                                                        double studDiameter,
                                                        [DefaultArgument("9;")] int noOfShearStudsInCircle,
                                                        [DefaultArgument("2;")] int shearStudConnectionType,
                                                        [DefaultArgument("null")] List<Property> additionalShearStudParameters)
    {
      List<SteelDbObject> tempList = new List<SteelDbObject>() { objectToConnect };
      List<string> handlesList = Utils.GetSteelDbObjectsToConnect(tempList);

      var vx = Utils.ToAstVector3d(shearStudCS.XAxis, true);
      var vy = Utils.ToAstVector3d(shearStudCS.YAxis, true);
      var vz = Utils.ToAstVector3d(shearStudCS.ZAxis, true);

      Matrix3d matrix3D = new Matrix3d();
      matrix3D.SetCoordSystem(Utils.ToAstPoint(connectionPoint, true), vx, vy, vz);

      additionalShearStudParameters = PreSetValuesInListProps(additionalShearStudParameters, noOfShearStudsInCircle, Utils.ToInternalDistanceUnits(patternRadius, true), Utils.ToInternalDistanceUnits(studLength, true), Utils.ToInternalDistanceUnits(studDiameter, true));

      return new CircularShearStudsPattern(handlesList[0], matrix3D, additionalShearStudParameters, shearStudConnectionType);
    }

    private static List<Property> PreSetValuesInListProps(List<Property> listOfBoltParameters, int noss, double radius, double studLength, double studDiameter)
    {
      if (listOfBoltParameters == null)
      {
        listOfBoltParameters = new List<Property>() { };
      }

      Utils.CheckListUpdateOrAddValue(listOfBoltParameters, nameof(Arranger.NumberOfElements), noss);
      Utils.CheckListUpdateOrAddValue(listOfBoltParameters, nameof(Arranger.Radius), radius);
      Utils.CheckListUpdateOrAddValue(listOfBoltParameters, nameof(Arranger.Length), studLength);
      Utils.CheckListUpdateOrAddValue(listOfBoltParameters, nameof(ASConnector.Diameter), studDiameter);

      return listOfBoltParameters;
    }

    [IsVisibleInDynamoLibrary(false)]
    public override Autodesk.DesignScript.Geometry.Curve GetDynCurve()
    {
      var shearStud = Utils.GetObject(Handle) as ASConnector;
      if (shearStud == null)
      {
        throw new Exception("Null shear stud pattern");
      }
      using (var point = Utils.ToDynPoint(shearStud.CenterPoint, true))
      using (var norm = Utils.ToDynVector(shearStud.Normal, true))
      {
        return Autodesk.DesignScript.Geometry.Circle.ByCenterPointRadiusNormal(point, Utils.FromInternalDistanceUnits(shearStud.Arranger.Radius, true), norm);
      }
    }

  }
}