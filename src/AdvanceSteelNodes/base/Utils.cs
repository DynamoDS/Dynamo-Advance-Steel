using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.Geometry;
using Autodesk.DesignScript.Runtime;
using Dynamo.Applications.AdvanceSteel.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdvanceSteel.Nodes
{
  [IsVisibleInDynamoLibrary(false)]
  public static class Utils
  {
    private static readonly string separator = "#@§@#";

    public static double RadToDegree(double rad)
    {
      return 180.0 * rad / System.Math.PI;
    }

    static public Autodesk.AdvanceSteel.Geometry.Point3d GetMidPointBetween(this Point3d OriginPoint, Point3d SecondPoint)
    {
      Autodesk.AdvanceSteel.Geometry.Point3d returnPnt = new Autodesk.AdvanceSteel.Geometry.Point3d();
      Autodesk.AdvanceSteel.Geometry.Vector3d v = SecondPoint.Subtract(OriginPoint);
      v.Normalize();
      returnPnt = OriginPoint + ((OriginPoint.DistanceTo(SecondPoint) / 2) * v);
      return returnPnt;
    }

    static public Autodesk.AdvanceSteel.Geometry.Point3d GetMidPointOnArc(this Point3d startPointOnArc, Point3d endPointOnArc, Point3d arcCentrePoint)
    {
      Autodesk.AdvanceSteel.Geometry.Point3d returnPnt = new Autodesk.AdvanceSteel.Geometry.Point3d();
      double radius = arcCentrePoint.DistanceTo(startPointOnArc);
      Autodesk.AdvanceSteel.Geometry.Point3d biSectPoint = startPointOnArc.GetMidPointBetween(endPointOnArc);
      Autodesk.AdvanceSteel.Geometry.Vector3d v = biSectPoint.Subtract(arcCentrePoint);
      v.Normalize();
      returnPnt = startPointOnArc + (radius * v);
      return returnPnt;
    }

    static public Autodesk.AdvanceSteel.Geometry.Point3d ToAstPoint(Autodesk.DesignScript.Geometry.Point pt, bool bConvertToAstUnits)
    {
      double factor = 1.0;
      if (bConvertToAstUnits)
      {
        var units = AppResolver.Resolve<IAppInteraction>().DbUnits;
        factor = units.UnitOfDistance.Factor;
      }

      return new Autodesk.AdvanceSteel.Geometry.Point3d(pt.X, pt.Y, pt.Z) * factor;
    }

    static public Double ToInternalUnits(double value, bool bConvert)
    {
      double factor = 1.0;
      if (bConvert)
      {
        var units = AppResolver.Resolve<IAppInteraction>().DbUnits;
        factor = units.UnitOfDistance.Factor;
      }

      return (value * factor);
    }

    static public Double FromInternalUnits(double value, bool bConvertFromAstUnits)
    {
      double factor = 1.0;
      if (bConvertFromAstUnits)
      {
        var units = AppResolver.Resolve<IAppInteraction>().DbUnits;
        factor = units.UnitOfDistance.Factor;
      }
      return (value * (1 / factor));
    }

    static public Double ToInternalAngleUnits(double value, bool bConvert)
    {
      double factor = 1.0;
      if (bConvert)
      {
        var units = AppResolver.Resolve<IAppInteraction>().DbUnits;
        factor = units.UnitOfAngle.Factor;
      }

      return (value * factor);
    }

    static public Double FromInternalAngleUnits(double value, bool bConvertFromAstUnits)
    {
      double factor = 1.0;
      if (bConvertFromAstUnits)
      {
        var units = AppResolver.Resolve<IAppInteraction>().DbUnits;
        factor = units.UnitOfAngle.Factor;
      }
      return (value * (1 / factor));
    }

    static public Autodesk.DesignScript.Geometry.Point ToDynPoint(Autodesk.AdvanceSteel.Geometry.Point3d pt, bool bConvertFromAstUnits)
    {
      double factor = 1.0;
      if (bConvertFromAstUnits)
      {
        var units = AppResolver.Resolve<IAppInteraction>().DbUnits;
        factor = units.UnitOfDistance.Factor;
      }
      pt = pt * (1 / factor);
      return Autodesk.DesignScript.Geometry.Point.ByCoordinates(pt.x, pt.y, pt.z);
    }

    static public Autodesk.DesignScript.Geometry.Vector ToDynVector(Autodesk.AdvanceSteel.Geometry.Vector3d vect, bool bConvertFromAstUnits)
    {
      double factor = 1.0;
      if (bConvertFromAstUnits)
      {
        var units = AppResolver.Resolve<IAppInteraction>().DbUnits;
        factor = units.UnitOfDistance.Factor;
      }
      vect = vect * (1 / factor);
      return Autodesk.DesignScript.Geometry.Vector.ByCoordinates(vect.x, vect.y, vect.z);
    }

    static public Autodesk.DesignScript.Geometry.CoordinateSystem ToDynCoordinateSys(Autodesk.AdvanceSteel.Geometry.Matrix3d matrix, bool bConvertToAstUnits)
    {
      Autodesk.AdvanceSteel.Geometry.Point3d origin = new Point3d();
      Autodesk.AdvanceSteel.Geometry.Vector3d xAxis = new Autodesk.AdvanceSteel.Geometry.Vector3d();
      Autodesk.AdvanceSteel.Geometry.Vector3d yAxis = new Autodesk.AdvanceSteel.Geometry.Vector3d();
      Autodesk.AdvanceSteel.Geometry.Vector3d zAxis = new Autodesk.AdvanceSteel.Geometry.Vector3d();
      matrix.GetCoordSystem(out origin, out xAxis, out yAxis, out zAxis);

      return Autodesk.DesignScript.Geometry.CoordinateSystem.ByOriginVectors(ToDynPoint(origin, bConvertToAstUnits), ToDynVector(xAxis, bConvertToAstUnits), 
        ToDynVector(yAxis, bConvertToAstUnits), ToDynVector(zAxis, bConvertToAstUnits));
    }

    static public Autodesk.AdvanceSteel.Geometry.Vector3d ToAstVector3d(Autodesk.DesignScript.Geometry.Vector v, bool bConvertToAstUnits)
    {
      double factor = 1.0;
      if (bConvertToAstUnits)
      {
        var units = AppResolver.Resolve<IAppInteraction>().DbUnits;
        factor = units.UnitOfDistance.Factor;
      }
      return new Autodesk.AdvanceSteel.Geometry.Vector3d(v.X, v.Y, v.Z) * factor;
    }

    static public Autodesk.AdvanceSteel.Geometry.Matrix3d ToAstMatrix3d(Autodesk.DesignScript.Geometry.CoordinateSystem cs, bool bConvertToAstUnits)
    {
      Autodesk.AdvanceSteel.Geometry.Matrix3d matrix = new Autodesk.AdvanceSteel.Geometry.Matrix3d();
      matrix.SetCoordSystem(ToAstPoint(cs.Origin, bConvertToAstUnits), ToAstVector3d(cs.XAxis, bConvertToAstUnits), ToAstVector3d(cs.YAxis, bConvertToAstUnits), ToAstVector3d(cs.ZAxis, bConvertToAstUnits));
      return matrix;
    }

    static public Autodesk.AdvanceSteel.Geometry.Point3d[] ToAstPoints(Autodesk.DesignScript.Geometry.Point[] pts, bool bConvertToAstUnits)
    {
      if (pts == null)
        return new Autodesk.AdvanceSteel.Geometry.Point3d[0];

      Autodesk.AdvanceSteel.Geometry.Point3d[] astPts = new Autodesk.AdvanceSteel.Geometry.Point3d[pts.Length];
      for (int nIdx = 0; nIdx < pts.Length; nIdx++)
      {
        astPts[nIdx] = ToAstPoint(pts[nIdx], bConvertToAstUnits);
      }

      return astPts;
    }

    static public Autodesk.DesignScript.Geometry.Point[] ToDynPoints(Autodesk.AdvanceSteel.Geometry.Point3d[] astPts, bool bConvertToAstUnits)
    {
      if (astPts == null)
        return new Autodesk.DesignScript.Geometry.Point[0];

      Autodesk.DesignScript.Geometry.Point[] pts = new Autodesk.DesignScript.Geometry.Point[astPts.Length];
      for (int nIdx = 0; nIdx < pts.Length; nIdx++)
      {
        pts[nIdx] = ToDynPoint(astPts[nIdx], bConvertToAstUnits);
      }

      return pts;
    }

    static public FilerObject GetObject(string handle)
    {
      return FilerObject.GetFilerObjectByHandle(handle);
    }

    internal static void SetOrientation(Autodesk.AdvanceSteel.Modelling.Beam beam, Autodesk.AdvanceSteel.Geometry.Vector3d vOrientation)
    {
      beam.PhysCSStart.GetCoordSystem(out _, out Vector3d vXAxis, out Vector3d vYAxis, out Vector3d vZAxis);
      if (!vXAxis.IsParallelTo(vOrientation))
      {
        Vector3d vProj = vOrientation.OrthoProject(vXAxis);

        double dAngle = vZAxis.GetAngleTo(vProj, vXAxis);

        beam.SetXRotation(dAngle * 180 / Math.PI);
      }
    }

    internal static void AdjustBeamEnd(Autodesk.AdvanceSteel.Modelling.Beam beam, Autodesk.AdvanceSteel.Geometry.Point3d newPtStart)
    {
      Autodesk.AdvanceSteel.Geometry.Point3d beamPtStart = beam.GetPointAtStart();
      Autodesk.AdvanceSteel.Geometry.Point3d beamPtEnd = beam.GetPointAtEnd();

      if (beamPtEnd.IsEqualTo(newPtStart))
      {
        Autodesk.AdvanceSteel.Geometry.Point3d newBeamEnd = beamPtEnd + (beamPtEnd - beamPtStart) * 0.5;
        beam.SetSysEnd(newBeamEnd);
      }
    }

    internal static string Separator
    {
      get { return separator; }
    }

    internal static string[] SplitSectionName(string sectionName)
    {
      string[] result = sectionName.Split(new string[] { Separator }, System.StringSplitOptions.None);

      if (2 == result.Length)
      {
        return result;
      }
      else
      {
        throw new System.Exception("Invalid section name");
      }
    }

    internal static bool CompareCompoundSectionTypes(string first, string second)
    {
      if (first.Equals(second) || (first.Contains("Welded") && second.Contains("Welded")) || (first.Contains("Compound") && second.Contains("Compound")) || (first.Contains("Tapered") && second.Contains("Tapered")))
      {
        return true;
      }
      return false;
    }

    public static double GetDiagonalLength(Autodesk.AdvanceSteel.Geometry.Point3d point1, Autodesk.AdvanceSteel.Geometry.Point3d point2)
    {
      return (point2 - point1).GetLength();
    }
    public static double GetRectangleAngle(Autodesk.AdvanceSteel.Geometry.Point3d point1, Autodesk.AdvanceSteel.Geometry.Point3d point2, Autodesk.AdvanceSteel.Geometry.Vector3d vx)
    {
      return (point2 - point1).GetAngleTo(vx);
    }
    public static double GetRectangleLength(Autodesk.AdvanceSteel.Geometry.Point3d point1, Autodesk.AdvanceSteel.Geometry.Point3d point2, Autodesk.AdvanceSteel.Geometry.Vector3d vx)
    {
      var diagLen = GetDiagonalLength(point1, point2);
      var alpha = GetRectangleAngle(point1, point2, vx);

      return diagLen * Math.Cos(alpha);
    }
    public static double GetRectangleHeight(Autodesk.AdvanceSteel.Geometry.Point3d point1, Autodesk.AdvanceSteel.Geometry.Point3d point2, Autodesk.AdvanceSteel.Geometry.Vector3d vx)
    {
      var diagLen = GetDiagonalLength(point1, point2);
      var alpha = GetRectangleAngle(point1, point2, vx);

      return diagLen * Math.Sin(alpha);
    }

    public static FilerObject[] GetSteelObjectsToConnect(IEnumerable<string> handlesToConnect)
    {
      var ret = new List<FilerObject>();
      bool Objs = false;
      foreach (var objHandle in handlesToConnect)
      {
        FilerObject obj = Utils.GetObject(objHandle);
        if (obj != null && (obj.IsKindOf(FilerObject.eObjectType.kBeam) ||
                            obj.IsKindOf(FilerObject.eObjectType.kBentBeam) ||
                            obj.IsKindOf(FilerObject.eObjectType.kPlate)))
        {
          Objs = true;
          ret.Add(obj);
        }
        else
        {
          throw new System.Exception("Object is empty");
        }
      }
      return Objs ? ret.ToArray() : Array.Empty<FilerObject>();
    }

    public static FilerObject[] GetFilerObjects(IEnumerable<string> handles)
    {
      var ret = new List<FilerObject>();
      bool Objs = false;
      foreach (var objHandle in handles)
      {
        Objs = true;
        ret.Add(Utils.GetObject(objHandle));
      }

      return Objs ? ret.ToArray() : Array.Empty<FilerObject>();
    }

    public static List<string> GetSteelDbObjectsToConnect(IEnumerable<SteelDbObject> objectsToConnect)
    {
      List<string> handlesList = new List<string>();
      foreach (var obj in objectsToConnect)
      {
        if (obj is AdvanceSteel.Nodes.Beams.BentBeam ||
            obj is AdvanceSteel.Nodes.Beams.StraightBeam ||
            obj is AdvanceSteel.Nodes.Plates.Plate)
        {
          handlesList.Add(obj.Handle);
        }
        else
        {
          throw new Exception("Only beams and plates can be connected");
        }
      }
      return handlesList;
    }

    public static Dictionary<string, ASProperty> GetBoltProperties(int listFilter)
    {
      return BuildBoltPropertyList(listFilter);
    }

    public static Dictionary<string, ASProperty> GetAnchorBoltPropertyList(int listFilter)
    {
      return BuildAnchorBoltPropertyList(listFilter);
    }

    public static Dictionary<string, ASProperty> GetShearStudPropertyList(int listFilter)
    {
      return BuildShearStudPropertyList(listFilter);
    }
    
    public static Dictionary<string, ASProperty> GetPlatePropertyList(int listFilter)
    {
      return BuildGenericPlatePropertyList(listFilter);
    }

    public static Dictionary<string, ASProperty> GetStraighBeamPropertyList(int listFilter)
    {
      Dictionary<string, ASProperty> combinedData = BuildStriaghtBeamPropertyList(listFilter).Union(
                                            BuildGenericBeamPropertyList(listFilter)).ToDictionary(s => s.Key, s => s.Value);
      return combinedData;
    }

    public static Dictionary<string, ASProperty> GetBentBeamPropertyList(int listFilter)
    {
      Dictionary<string, ASProperty> combinedData = BuildBentBeamPropertyList(listFilter).Union(
                                            BuildGenericBeamPropertyList(listFilter)).ToDictionary(s => s.Key, s => s.Value);
      return combinedData;
    }

    public static Dictionary<string, ASProperty> GetTaperBeamPropertyList(int listFilter)
    {
      Dictionary<string, ASProperty> combinedData = BuildTaperedBeamPropertyList(listFilter).Union(
                                                  BuildCompundBaseBeamPropertyList(listFilter)).Union(
                                                  BuildGenericBeamPropertyList(listFilter)).ToDictionary(s => s.Key, s => s.Value);
      return combinedData;
    }

    public static Dictionary<string, ASProperty> GetCompoundStraightBeamPropertyList(int listFilter)
    {
      Dictionary<string, ASProperty> combinedData = BuildCompoundStraightBeamPropertyList(listFilter).Union(
                                                  BuildCompundBaseBeamPropertyList(listFilter)).Union(
                                                  BuildGenericBeamPropertyList(listFilter)).ToDictionary(s => s.Key, s => s.Value);
      return combinedData;
    }

    public static ASProperty GetProperty(string keyValue)
    {
      Dictionary<string, ASProperty> searchData = CombineAllLists(0);
      ASProperty retValue = null;
      searchData.TryGetValue(keyValue, out retValue);
      return retValue;
    }

    private static Dictionary<string, ASProperty> CombineAllLists(int listFilter)
    {
      Dictionary<string, ASProperty> searchData = BuildGenericBeamPropertyList(listFilter).Union(
                                                BuildStriaghtBeamPropertyList(listFilter)).Union(
                                                BuildCompoundStraightBeamPropertyList(listFilter)).Union(
                                                BuildTaperedBeamPropertyList(listFilter)).Union(
                                                BuildBentBeamPropertyList(listFilter)).Union(
                                                BuildCompundBaseBeamPropertyList(listFilter)).Union(
                                                BuildGenericPlatePropertyList(listFilter)).Union(
                                                BuildBoltPropertyList(listFilter)).Union(
                                                BuildAnchorBoltPropertyList(listFilter)).Union(
                                                BuildShearStudPropertyList(listFilter)).ToDictionary(s => s.Key, s => s.Value);
      return searchData;
    }

    private static Dictionary<string, ASProperty> filterDictionary(Dictionary<string, ASProperty> dictProps, int listFilter)
    {
      return (listFilter > 0 ? dictProps.Where(x => (x.Value.PropertyDataOp % listFilter) == 0).ToDictionary(x => x.Key, x => x.Value) : dictProps);
    }

    private static Dictionary<string, ASProperty> BuildBoltPropertyList(int listFilter)
    {
      Dictionary<string, ASProperty> dictProps = new Dictionary<string, ASProperty>() { };
      dictProps.Add("Select Bolt Property...", new ASProperty("none", typeof(string)));
      dictProps.Add("Bolt Standard", new ASProperty("Standard", typeof(string)));
      dictProps.Add("Bolt Assembly", new ASProperty("BoltAssembly", typeof(string)));
      dictProps.Add("Bolt Grade", new ASProperty("Grade", typeof(string)));
      dictProps.Add("Bolt Diameter", new ASProperty("ScrewDiameter", typeof(double)));
      dictProps.Add("Bolt Hole Tolerance", new ASProperty("HoleTolerance", typeof(double)));
      dictProps.Add("No of Bolts Circle", new ASProperty("NumberOfScrews", typeof(int)));
      dictProps.Add("X Bolt Count", new ASProperty("Nx", typeof(int)));
      dictProps.Add("Y Bolt Count", new ASProperty("Ny", typeof(int)));
      dictProps.Add("Bolt X Spacing", new ASProperty("Dx", typeof(double)));
      dictProps.Add("Bolt Y Spacing", new ASProperty("Dy", typeof(double)));
      dictProps.Add("Bolt Pattern Radius", new ASProperty("Radius", typeof(double)));
      dictProps.Add("Bolt Length Addition", new ASProperty("BindingLengthAddition", typeof(double)));
      dictProps.Add("Bolt Inverted", new ASProperty("IsInverted", typeof(bool)));

      return filterDictionary(dictProps, listFilter);
    }

    private static Dictionary<string, ASProperty> BuildAnchorBoltPropertyList(int listFilter)
    {
      Dictionary<string, ASProperty> dictProps = new Dictionary<string, ASProperty>() { };
      dictProps.Add("Select Anchor Property...", new ASProperty("none", typeof(string)));
      dictProps.Add("Anchor Standard", new ASProperty("Standard", typeof(string)));
      dictProps.Add("Anchor Assembly", new ASProperty("BoltAssembly", typeof(string)));
      dictProps.Add("Anchor Grade", new ASProperty("Grade", typeof(string))); 
      dictProps.Add("Anchor Length", new ASProperty("ScrewLength", typeof(double)));
      dictProps.Add("Anchor Diameter", new ASProperty("ScrewDiameter", typeof(double)));
      dictProps.Add("Anchor Hole Tolerance", new ASProperty("HoleTolerance", typeof(double)));
      dictProps.Add("No of Anchor Circle", new ASProperty("NumberOfScrews", typeof(int)));
      dictProps.Add("X Anchor Count", new ASProperty("Nx", typeof(int)));
      dictProps.Add("Y Anchor Count", new ASProperty("Ny", typeof(int)));
      dictProps.Add("Anchor X Spacing", new ASProperty("Dx", typeof(double)));
      dictProps.Add("Anchor Y Spacing", new ASProperty("Dy", typeof(double)));
      dictProps.Add("Anchor Pattern Radius", new ASProperty("Radius", typeof(double))); 
      dictProps.Add("Anchor Inverted", new ASProperty("IsInverted", typeof(bool)));

      return filterDictionary(dictProps, listFilter);
    }

    private static Dictionary<string, ASProperty> BuildShearStudPropertyList(int listFilter)
    {
      Dictionary<string, ASProperty> dictProps = new Dictionary<string, ASProperty>() { };
      dictProps.Add("Select Stud Property...", new ASProperty("none", typeof(string)));
      dictProps.Add("Stud Standard", new ASProperty("Standard", typeof(string)));
      dictProps.Add("Stud Grade", new ASProperty("Grade", typeof(string)));
      dictProps.Add("Stud Length", new ASProperty("Length", typeof(double)));
      dictProps.Add("Stud Diameter", new ASProperty("Diameter", typeof(double)));
      dictProps.Add("Stud Hole Tolerance", new ASProperty("HoleTolerance", typeof(double)));
      dictProps.Add("No of Shear Studs Circle", new ASProperty("NumberOfElements", typeof(int), "Arranger"));
      dictProps.Add("Shear Stud Radius", new ASProperty("Radius", typeof(double), "Arranger")); 
      dictProps.Add("X Stud Count", new ASProperty("Nx", typeof(int), "Arranger"));
      dictProps.Add("Y Stud Count", new ASProperty("Ny", typeof(int), "Arranger"));
      dictProps.Add("Stud X Spacing", new ASProperty("Dx", typeof(double), "Arranger"));
      dictProps.Add("Stud Y Spacing", new ASProperty("Dy", typeof(double), "Arranger"));
      dictProps.Add("Display Stud As Solid", new ASProperty("ReprMode", typeof(int), "Z_PostWriteDB"));

      return filterDictionary(dictProps, listFilter);
    }

    private static Dictionary<string, ASProperty> BuildGenericBeamPropertyList(int listFilter)
    {
      Dictionary<string, ASProperty> dictProps = new Dictionary<string, ASProperty>() { };
      //dictProps.Add("Select Beam Property...", new Property("none", typeof(string)));
      dictProps.Add("Beam Angle", new ASProperty("Angle", typeof(double)));
      dictProps.Add("Beam Approval Comment", new ASProperty("ApprovalComment", typeof(string)));
      dictProps.Add("Beam Approval Status Code", new ASProperty("ApprovalStatusCode", typeof(string)));
      dictProps.Add("Beam Assembly", new ASProperty("Assembly", typeof(string)));
      dictProps.Add("Beam Assembly Used For Numbering", new ASProperty("AssemblyUsedForNumbering", typeof(int)));
      dictProps.Add("Beam Center Point", new ASProperty("CenterPoint", typeof(Point3d), ".", ePropertyDataOperator.Get));
      dictProps.Add("Beam Carrier", new ASProperty("Carrier", typeof(string)));
      dictProps.Add("Beam Coating", new ASProperty("Coating", typeof(string))); 
      dictProps.Add("Beam Coating Description", new ASProperty("CoatingDescription", typeof(string), ".", ePropertyDataOperator.Get));
      dictProps.Add("Beam Coating Used For Numbering", new ASProperty("CoatingUsedForNumbering", typeof(int))); 
      dictProps.Add("Beam Delivery Date", new ASProperty("DeliveryDate", typeof(string)));
      dictProps.Add("Beam Denotation Used For Numbering", new ASProperty("DennotationUsedForNumbering", typeof(int))); 
      dictProps.Add("Beam Denotation Role", new ASProperty("Denotation", typeof(string)));
      dictProps.Add("Beam Deviation", new ASProperty("Deviation", typeof(double)));
      dictProps.Add("Beam Explicit Quantity", new ASProperty("ExplicitQuantity", typeof(int))); 
      dictProps.Add("Beam Fabrication Station", new ASProperty("FabricationStation", typeof(string)));
      dictProps.Add("Beam Fabrication Station UsedF or Numbering", new ASProperty("FabricationStationUsedForNumbering", typeof(bool)));
      dictProps.Add("Beam Handle", new ASProperty("Handle", typeof(string),".", ePropertyDataOperator.Get));
      dictProps.Add("Beam Heat Number", new ASProperty("HeatNumber", typeof(string)));
      dictProps.Add("Beam Heat Number Used For Numbering", new ASProperty("HeatNumberUsedForNumbering", typeof(bool)));
      dictProps.Add("Beam Holes Used For Numbering", new ASProperty("HolesUsedForNumbering", typeof(int))); 
      dictProps.Add("Beam Set IsMainPart Flag", new ASProperty("IsMainPart", typeof(bool), "Z_PostWriteDB"));
      dictProps.Add("Beam Get IsAttachedPart Flag", new ASProperty("IsAttachedPart", typeof(bool), ".", ePropertyDataOperator.Get));
      dictProps.Add("Beam Get IsCrossSectionMirrored Flag", new ASProperty("IsCrossSectionMirrored", typeof(bool), ".", ePropertyDataOperator.Get));
      dictProps.Add("Beam ItemNumber", new ASProperty("ItemNumber", typeof(string)));
      dictProps.Add("Beam ItemNumber Used For Numbering", new ASProperty("ItemNumberUsedForNumbering", typeof(int))); 
      dictProps.Add("Beam Layer", new ASProperty("Layer", typeof(string)));
      dictProps.Add("Beam Load Number", new ASProperty("LoadNumber", typeof(string)));
      dictProps.Add("Beam MainPart Number", new ASProperty("MainPartNumber", typeof(string)));
      dictProps.Add("Beam MainPart Number Prefix", new ASProperty("MainPartPrefix", typeof(string)));
      dictProps.Add("Beam MainPart Used For BOM", new ASProperty("MainPartUsedForBOM", typeof(int))); 
      dictProps.Add("Beam MainPart Used For Collision Check", new ASProperty("MainPartUsedForCollisionCheck", typeof(int))); 
      dictProps.Add("Beam MainPart Used For Numbering", new ASProperty("MainPartUsedForNumbering", typeof(int))); 
      dictProps.Add("Beam Material", new ASProperty("Material", typeof(string)));
      dictProps.Add("Beam Material Description", new ASProperty("MaterialDescription", typeof(string), ".", ePropertyDataOperator.Get));
      dictProps.Add("Beam Material Used For Numbering", new ASProperty("MaterialUsedForNumbering", typeof(int))); 
      dictProps.Add("Beam Note", new ASProperty("Note", typeof(string)));
      dictProps.Add("Beam Note Used For Numbering", new ASProperty("NoteUsedForNumbering", typeof(int))); 
      dictProps.Add("Beam Number Of Holes", new ASProperty("NumberOfHoles", typeof(int), ".", ePropertyDataOperator.Get)); 
      dictProps.Add("Beam PONumber", new ASProperty("PONumber", typeof(string)));
      dictProps.Add("Beam PONumber Used For Numbering", new ASProperty("PONumberUsedForNumbering", typeof(bool)));
      dictProps.Add("Beam Preliminary Part Number", new ASProperty("PreliminaryPartNumber", typeof(string))); 
      dictProps.Add("Beam Preliminary Part Position Number", new ASProperty("PreliminaryPartPositionNumber", typeof(string), ".", ePropertyDataOperator.Get)); 
      dictProps.Add("Beam Preliminary Part Prefix", new ASProperty("PreliminaryPartPrefix", typeof(string)));
      dictProps.Add("Beam Profile Name", new ASProperty("ProfName", typeof(string)));
      dictProps.Add("Beam Profile Section Type", new ASProperty("ProfSectionType", typeof(string), ".", ePropertyDataOperator.Get));
      dictProps.Add("Beam Profile Section name", new ASProperty("ProfSectionName", typeof(string), ".", ePropertyDataOperator.Get));
      dictProps.Add("Beam Requisition Number", new ASProperty("RequisitionNumber", typeof(string)));
      dictProps.Add("Beam Requisition Number Used For Numbering", new ASProperty("RequisitionNumberUsedForNumbering", typeof(bool)));
      dictProps.Add("Beam Model Role", new ASProperty("Role", typeof(string)));
      dictProps.Add("Beam Model Role Description", new ASProperty("RoleDescription", typeof(string)));
      dictProps.Add("Beam Role Used For Numbering", new ASProperty("RoleUsedForNumbering", typeof(int)));
      dictProps.Add("Beam Runname", new ASProperty("Runname", typeof(string), ".", ePropertyDataOperator.Get));
      dictProps.Add("Beam Shipped Date", new ASProperty("ShippedDate", typeof(string)));
      dictProps.Add("Beam ShrinkValue", new ASProperty("ShrinkValue", typeof(double), ".", ePropertyDataOperator.Get));
      dictProps.Add("Beam Single Part Number", new ASProperty("SinglePartNumber", typeof(string)));
      dictProps.Add("Beam Single Part Prefix", new ASProperty("SinglePartPrefix", typeof(string)));
      dictProps.Add("Beam Single Part Used For BOM", new ASProperty("SinglePartUsedForBOM", typeof(int))); 
      dictProps.Add("Beam Single Part Used For CollisionCheck", new ASProperty("SinglePartUsedForCollisionCheck", typeof(int))); 
      dictProps.Add("Beam Single Part Used For Numbering", new ASProperty("SinglePartUsedForNumbering", typeof(int)));
      dictProps.Add("Beam Specific Gravity", new ASProperty("SpecificGravity", typeof(double), ".", ePropertyDataOperator.Get)); 
      dictProps.Add("Beam Structural Member", new ASProperty("StructuralMember", typeof(int)));
      dictProps.Add("Beam System Line Length", new ASProperty("SysLength", typeof(double), ".", ePropertyDataOperator.Get));
      dictProps.Add("Beam Supplier", new ASProperty("Supplier", typeof(string)));
      dictProps.Add("Beam SupplierUsedForNumbering", new ASProperty("SupplierUsedForNumbering", typeof(bool)));
      dictProps.Add("Beam Unwind / Unfolder", new ASProperty("Unwind", typeof(bool)));
      dictProps.Add("Beam UnwindStartFactor", new ASProperty("UnwindStartFactor", typeof(double)));
      dictProps.Add("Beam Volume", new ASProperty("Volume", typeof(double), ".", ePropertyDataOperator.Get));
      dictProps.Add("Change Beam Display Mode", new ASProperty("ReprMode", typeof(int), "Z_PostWriteDB"));

      return filterDictionary(dictProps, listFilter);
    }

    private static Dictionary<string, ASProperty> BuildGenericPlatePropertyList(int listFilter)
    {
      Dictionary<string, ASProperty> dictProps = new Dictionary<string, ASProperty>() { };
      dictProps.Add("Select Plate Property...", new ASProperty("none", typeof(string)));
      dictProps.Add("Plate Approval Comment", new ASProperty("ApprovalComment", typeof(string)));
      dictProps.Add("Plate Approval Status Code", new ASProperty("ApprovalStatusCode", typeof(string)));
      dictProps.Add("Plate Assembly", new ASProperty("Assembly", typeof(string)));
      dictProps.Add("Plate Assembly Used For Numbering", new ASProperty("AssemblyUsedForNumbering", typeof(int)));
      dictProps.Add("Plate Center Point", new ASProperty("CenterPoint", typeof(Point3d), ".", ePropertyDataOperator.Get));
      dictProps.Add("Plate Carrier", new ASProperty("Carrier", typeof(string)));
      dictProps.Add("Plate Coating", new ASProperty("Coating", typeof(string)));
      dictProps.Add("Plate Coating Description", new ASProperty("CoatingDescription", typeof(string), ".", ePropertyDataOperator.Get));
      dictProps.Add("Plate Coating Used For Numbering", new ASProperty("CoatingUsedForNumbering", typeof(int)));
      dictProps.Add("Plate Delivery Date", new ASProperty("DeliveryDate", typeof(string)));
      dictProps.Add("Plate Denotation Used For Numbering", new ASProperty("DennotationUsedForNumbering", typeof(int)));
      dictProps.Add("Plate Denotation", new ASProperty("Denotation", typeof(string)));
      dictProps.Add("Plate Explicit Quantity", new ASProperty("ExplicitQuantity", typeof(int)));
      dictProps.Add("Plate Fabrication Station", new ASProperty("FabricationStation", typeof(string)));
      dictProps.Add("Plate Fabrication Station UsedF or Numbering", new ASProperty("FabricationStationUsedForNumbering", typeof(bool)));
      dictProps.Add("Plate Handle", new ASProperty("Handle", typeof(string), ".", ePropertyDataOperator.Get));
      dictProps.Add("Plate Heat Number", new ASProperty("HeatNumber", typeof(string)));
      dictProps.Add("Plate Heat Number Used For Numbering", new ASProperty("HeatNumberUsedForNumbering", typeof(bool)));
      dictProps.Add("Plate Holes Used For Numbering", new ASProperty("HolesUsedForNumbering", typeof(int)));
      dictProps.Add("Plate Set IsAttached Flag", new ASProperty("IsAttachedPart", typeof(bool), "Z_PostWriteDB"));
      dictProps.Add("Plate Set IsMainPart Flag", new ASProperty("IsMainPart", typeof(bool), "Z_PostWriteDB"));
      dictProps.Add("Plate ItemNumber", new ASProperty("ItemNumber", typeof(string)));
      dictProps.Add("Plate ItemNumber Used For Numbering", new ASProperty("ItemNumberUsedForNumbering", typeof(int)));
      dictProps.Add("Plate Layer", new ASProperty("Layer", typeof(string)));
      dictProps.Add("Plate Length", new ASProperty("Length", typeof(double)));
      dictProps.Add("Plate Load Number", new ASProperty("LoadNumber", typeof(string)));
      dictProps.Add("Plate MainPart Number", new ASProperty("MainPartNumber", typeof(string)));
      dictProps.Add("Plate MainPart Number Prefix", new ASProperty("MainPartPrefix", typeof(string)));
      dictProps.Add("Plate MainPart Used For BOM", new ASProperty("MainPartUsedForBOM", typeof(int)));
      dictProps.Add("Plate MainPart Used For Collision Check", new ASProperty("MainPartUsedForCollisionCheck", typeof(int)));
      dictProps.Add("Plate MainPart Used For Numbering", new ASProperty("MainPartUsedForNumbering", typeof(int)));
      dictProps.Add("Plate Material", new ASProperty("Material", typeof(string)));
      dictProps.Add("Plate Material Description", new ASProperty("MaterialDescription", typeof(string), ".", ePropertyDataOperator.Get));
      dictProps.Add("Plate Material Used For Numbering", new ASProperty("MaterialUsedForNumbering", typeof(int)));
      dictProps.Add("Plate Note", new ASProperty("Note", typeof(string)));
      dictProps.Add("Plate Note Used For Numbering", new ASProperty("NoteUsedForNumbering", typeof(int)));
      dictProps.Add("Plate Number Of Holes", new ASProperty("NumberOfHoles", typeof(int), ".", ePropertyDataOperator.Get));
      dictProps.Add("Plate PONumber", new ASProperty("PONumber", typeof(string)));
      dictProps.Add("Plate PONumber Used For Numbering", new ASProperty("PONumberUsedForNumbering", typeof(bool)));
      dictProps.Add("Plate Face Alignment", new ASProperty("Portioning", typeof(double)));
      dictProps.Add("Plate Preliminary Part Number", new ASProperty("PreliminaryPartNumber", typeof(string)));
      dictProps.Add("Plate Preliminary Part Position Number", new ASProperty("PreliminaryPartPositionNumber", typeof(string)));
      dictProps.Add("Plate Preliminary Part Prefix", new ASProperty("PreliminaryPartPrefix", typeof(string)));
      dictProps.Add("Plate Radius Increment", new ASProperty("RadIncrement", typeof(double)));
      dictProps.Add("Plate Radius", new ASProperty("Radius", typeof(double), ".", ePropertyDataOperator.Get));
      dictProps.Add("Plate Requisition Number", new ASProperty("RequisitionNumber", typeof(string)));
      dictProps.Add("Plate Requisition Number Used For Numbering", new ASProperty("RequisitionNumberUsedForNumbering", typeof(bool)));
      dictProps.Add("Plate Model Role", new ASProperty("Role", typeof(string)));
      dictProps.Add("Plate Model Role Description", new ASProperty("RoleDescription", typeof(string), ".", ePropertyDataOperator.Get));
      dictProps.Add("Plate Role Used For Numbering", new ASProperty("RoleUsedForNumbering", typeof(int)));
      dictProps.Add("Plate Shipped Date", new ASProperty("ShippedDate", typeof(string)));
      dictProps.Add("Plate Single Part Number", new ASProperty("SinglePartNumber", typeof(string)));
      dictProps.Add("Plate Single Part Prefix", new ASProperty("SinglePartPrefix", typeof(string)));
      dictProps.Add("Plate Single Part Used For BOM", new ASProperty("SinglePartUsedForBOM", typeof(int)));
      dictProps.Add("Plate Single Part Used For CollisionCheck", new ASProperty("SinglePartUsedForCollisionCheck", typeof(int)));
      dictProps.Add("Plate Single Part Used For Numbering", new ASProperty("SinglePartUsedForNumbering", typeof(int)));
      dictProps.Add("Plate SpecificGravity", new ASProperty("SpecificGravity", typeof(double), ".", ePropertyDataOperator.Get));
      dictProps.Add("Plate Supplier", new ASProperty("Supplier", typeof(string)));
      dictProps.Add("Plate SupplierUsedForNumbering", new ASProperty("SupplierUsedForNumbering", typeof(bool)));
      dictProps.Add("Plate Thickness", new ASProperty("Thickness", typeof(double)));
      dictProps.Add("Plate Volume", new ASProperty("Volume", typeof(double), ".", ePropertyDataOperator.Get));
      dictProps.Add("Plate Width", new ASProperty("Width", typeof(double)));
      dictProps.Add("Change Plate Display Mode", new ASProperty("ReprMode", typeof(int), "Z_PostWriteDB"));

      return filterDictionary(dictProps, listFilter);
    }

    private static Dictionary<string, ASProperty> BuildStriaghtBeamPropertyList(int listFilter)
    {
      Dictionary<string, ASProperty> dictProps = new Dictionary<string, ASProperty>() { };
      dictProps.Add("Select Straight Beam Property...", new ASProperty("none", typeof(string)));

      return filterDictionary(dictProps, listFilter);
    }

    private static Dictionary<string, ASProperty> BuildCompoundStraightBeamPropertyList(int listFilter)
    {
      Dictionary<string, ASProperty> dictProps = new Dictionary<string, ASProperty>() { };
      dictProps.Add("Select Compound Straiht Beam Property...", new ASProperty("none", typeof(string)));

      return filterDictionary(dictProps, listFilter);
    }

    private static Dictionary<string, ASProperty> BuildTaperedBeamPropertyList(int listFilter)
    {
      Dictionary<string, ASProperty> dictProps = new Dictionary<string, ASProperty>() { };
      dictProps.Add("Select Tapered Beam Property...", new ASProperty("none", typeof(string)));

      return filterDictionary(dictProps, listFilter);
    }

    private static Dictionary<string, ASProperty> BuildBentBeamPropertyList(int listFilter)
    {
      Dictionary<string, ASProperty> dictProps = new Dictionary<string, ASProperty>() { };
      dictProps.Add("Select Bend Beam Property...", new ASProperty("none", typeof(string)));
      dictProps.Add("BendBeam Offset Curve Radius", new ASProperty("OffsetCurveRadius", typeof(double)));
      dictProps.Add("BendBeam Curve Offset", new ASProperty("CurveOffset", typeof(double), ".", ePropertyDataOperator.Get));
      dictProps.Add("BendBeam Systemline Radius", new ASProperty("SystemlineRadius", typeof(double), ".", ePropertyDataOperator.Get));

      return filterDictionary(dictProps, listFilter);
    }

    private static Dictionary<string, ASProperty> BuildCompundBaseBeamPropertyList(int listFilter)
    {
      Dictionary<string, ASProperty> dictProps = new Dictionary<string, ASProperty>() { };
      dictProps.Add("Use Compound Beam As One Beam", new ASProperty("UseCompoundAsOneBeam", typeof(bool)));
      dictProps.Add("Compound Beam ClassName", new ASProperty("CompoundClassName", typeof(string), ".", ePropertyDataOperator.Get));
      dictProps.Add("Compound Beam TypeName", new ASProperty("CompoundTypeName", typeof(string), ".", ePropertyDataOperator.Get));

      return filterDictionary(dictProps, listFilter);
    }

    public static void CheckListUpdateOrAddValue(List<ASProperty> listOfPropertyData, 
                                                  string propName, 
                                                  object propValue, 
                                                  string propLevel = "",
                                                  int propertyDataOp = 6)
    {
      var foundItem = listOfPropertyData.FirstOrDefault<ASProperty>(props => props.PropName == propName);
      if (foundItem != null)
      {
        foundItem.PropValue = propValue;
      }
      else
      {
        listOfPropertyData.Add(new ASProperty(propName, propValue, propValue.GetType(), propLevel, propertyDataOp));
      }
    }

    public static void SetParameters(Autodesk.AdvanceSteel.Modelling.CountableScrewBoltPattern objToMod, List<ASProperty> properties)
    {
      if (properties != null)
      {
        foreach (var prop in properties)
        {
          prop.UpdateASObject(objToMod);
        }
      }
    }

    public static void SetParameters(Autodesk.AdvanceSteel.Modelling.CircleScrewBoltPattern objToMod, List<ASProperty> properties)
    {
      if (properties != null)
      {
        foreach (var prop in properties)
        {
          prop.UpdateASObject(objToMod);
        }
      }
    }
    
    public static void SetParameters(Autodesk.AdvanceSteel.Arrangement.Arranger objToMod, List<ASProperty> properties)
    {
      if (properties != null)
      {
        foreach (var prop in properties)
        {
          prop.UpdateASObject(objToMod);
        }
      }
    }

    public static void SetParameters(Autodesk.AdvanceSteel.ConstructionTypes.AtomicElement objToMod, List<ASProperty> properties)
    {
      if (properties != null) 
      { 
        foreach (var prop in properties)
        {
          prop.UpdateASObject(objToMod);
        }
      }
    }

    public static void SetParameters(Autodesk.AdvanceSteel.Modelling.AnchorPattern objToMod, List<ASProperty> properties)
    {
      if (properties != null)
      {
        foreach (var prop in properties)
        {
          prop.UpdateASObject(objToMod);
        }
      }
    }
  }
}