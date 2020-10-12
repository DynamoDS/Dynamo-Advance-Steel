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

    public static Dictionary<string, Property> GetBoltProperties(int listFilter)
    {
      return BuildBoltPropertyList(listFilter);
    }

    public static Dictionary<string, Property> GetAnchorBoltPropertyList(int listFilter)
    {
      return BuildAnchorBoltPropertyList(listFilter);
    }

    public static Dictionary<string, Property> GetShearStudPropertyList(int listFilter)
    {
      return BuildShearStudPropertyList(listFilter);
    }

    public static Dictionary<string, Property> GetStraighBeamPropertyList(int listFilter)
    {
      Dictionary<string, Property> combinedData = BuildStriaghtBeamPropertyList(listFilter).Union(
                                            BuildGenericBeamPropertyList(listFilter)).ToDictionary(s => s.Key, s => s.Value);
      return combinedData;
    }

    public static Dictionary<string, Property> GetBentBeamPropertyList(int listFilter)
    {
      Dictionary<string, Property> combinedData = BuildBentBeamPropertyList(listFilter).Union(
                                            BuildGenericBeamPropertyList(listFilter)).ToDictionary(s => s.Key, s => s.Value);
      return combinedData;
    }

    public static Dictionary<string, Property> GetTaperBeamPropertyList(int listFilter)
    {
      Dictionary<string, Property> combinedData = BuildTaperedBeamPropertyList(listFilter).Union(
                                                  BuildCompundBaseBeamPropertyList(listFilter)).Union(
                                                  BuildGenericBeamPropertyList(listFilter)).ToDictionary(s => s.Key, s => s.Value);
      return combinedData;
    }

    public static Dictionary<string, Property> GetCompoundStraightBeamPropertyList(int listFilter)
    {
      Dictionary<string, Property> combinedData = BuildCompoundStraightBeamPropertyList(listFilter).Union(
                                                  BuildCompundBaseBeamPropertyList(listFilter)).Union(
                                                  BuildGenericBeamPropertyList(listFilter)).ToDictionary(s => s.Key, s => s.Value);
      return combinedData;
    }

    public static Property GetProperty(string keyValue, int listFilter)
    {
      Dictionary<string, Property> searchData = CombineAllLists(listFilter);
      Property retValue = null;
      searchData.TryGetValue(keyValue, out retValue);
      return retValue;
    }

    private static Dictionary<string, Property> CombineAllLists(int listFilter)
    {
      Dictionary<string, Property> searchData = BuildGenericBeamPropertyList(listFilter).Union(
                                                BuildStriaghtBeamPropertyList(listFilter)).Union(
                                                BuildCompoundStraightBeamPropertyList(listFilter)).Union(
                                                BuildTaperedBeamPropertyList(listFilter)).Union(
                                                BuildBentBeamPropertyList(listFilter)).Union(
                                                BuildCompundBaseBeamPropertyList(listFilter)).Union(
                                                BuildBoltPropertyList(listFilter)).Union(
                                                BuildAnchorBoltPropertyList(listFilter)).Union(
                                                BuildShearStudPropertyList(listFilter)).ToDictionary(s => s.Key, s => s.Value);
      return searchData;
    }

    private static Dictionary<string, Property> BuildBoltPropertyList(int listFilter)
    {
      Dictionary<string, Property> dictProps = new Dictionary<string, Property>() { };
      dictProps.Add("Select Bolt Property...", new Property("none", typeof(string)));
      dictProps.Add("Bolt Standard", new Property("Standard", typeof(string)));
      dictProps.Add("Bolt Assembly", new Property("BoltAssembly", typeof(string)));
      dictProps.Add("Bolt Grade", new Property("Grade", typeof(string)));
      dictProps.Add("Bolt Diameter", new Property("ScrewDiameter", typeof(double)));
      dictProps.Add("Bolt Hole Tolerance", new Property("HoleTolerance", typeof(double)));
      dictProps.Add("No of Bolts Circle", new Property("NumberOfScrews", typeof(int)));
      dictProps.Add("X Bolt Count", new Property("Nx", typeof(int)));
      dictProps.Add("Y Bolt Count", new Property("Ny", typeof(int)));
      dictProps.Add("Bolt X Spacing", new Property("Dx", typeof(double)));
      dictProps.Add("Bolt Y Spacing", new Property("Dy", typeof(double)));
      dictProps.Add("Bolt Pattern Radius", new Property("Radius", typeof(double)));
      dictProps.Add("Bolt Fake Set", new Property("BBBB", typeof(double), ".", 2));
      dictProps.Add("Bolt Fake Get", new Property("AAA", typeof(double), ".", 3));
      dictProps.Add("Bolt Length Addition", new Property("BindingLengthAddition", typeof(double)));
      dictProps.Add("Bolt Inverted", new Property("IsInverted", typeof(bool)));

      return dictProps.Where(x => (x.Value.PropertyDataOp % listFilter) == 0).ToDictionary(x => x.Key, x => x.Value);
    }

    private static Dictionary<string, Property> BuildAnchorBoltPropertyList(int listFilter)
    {
      Dictionary<string, Property> dictProps = new Dictionary<string, Property>() { };
      dictProps.Add("Select Anchor Property...", new Property("none", typeof(string)));
      dictProps.Add("Anchor Standard", new Property("Standard", typeof(string)));
      dictProps.Add("Anchor Assembly", new Property("BoltAssembly", typeof(string)));
      dictProps.Add("Anchor Grade", new Property("Grade", typeof(string))); 
      dictProps.Add("Anchor Length", new Property("ScrewLength", typeof(double)));
      dictProps.Add("Anchor Diameter", new Property("ScrewDiameter", typeof(double)));
      dictProps.Add("Anchor Hole Tolerance", new Property("HoleTolerance", typeof(double)));
      dictProps.Add("No of Anchor Circle", new Property("NumberOfScrews", typeof(int)));
      dictProps.Add("X Anchor Count", new Property("Nx", typeof(int)));
      dictProps.Add("Y Anchor Count", new Property("Ny", typeof(int)));
      dictProps.Add("Anchor X Spacing", new Property("Dx", typeof(double)));
      dictProps.Add("Anchor Fake Set", new Property("as", typeof(double), ".", 2));
      dictProps.Add("Anchor Fake Get", new Property("as", typeof(double), ".", 3));
      dictProps.Add("Anchor Y Spacing", new Property("Dy", typeof(double)));
      dictProps.Add("Anchor Pattern Radius", new Property("Radius", typeof(double))); 
      //dictProps.Add("Anchor Orientation", new ASProperty("OrientationType", typeof(int))); 
      dictProps.Add("Anchor Inverted", new Property("IsInverted", typeof(bool)));

      return dictProps.Where(x => (x.Value.PropertyDataOp % listFilter) == 0).ToDictionary(x => x.Key, x => x.Value);
    }

    private static Dictionary<string, Property> BuildShearStudPropertyList(int listFilter)
    {
      Dictionary<string, Property> dictProps = new Dictionary<string, Property>() { };
      dictProps.Add("Select Stud Property...", new Property("none", typeof(string)));
      dictProps.Add("Stud Standard", new Property("Standard", typeof(string)));
      dictProps.Add("Stud Grade", new Property("Grade", typeof(string)));
      dictProps.Add("Stud Length", new Property("Length", typeof(double)));
      dictProps.Add("Stud Diameter", new Property("Diameter", typeof(double)));
      dictProps.Add("Stud Hole Tolerance", new Property("HoleTolerance", typeof(double)));
      dictProps.Add("No of Shear Studs Circle", new Property("NumberOfElements", typeof(int), "Arranger"));
      dictProps.Add("Shear Stud Radius", new Property("Radius", typeof(double), "Arranger")); 
      dictProps.Add("X Stud Count", new Property("Nx", typeof(int), "Arranger"));
      dictProps.Add("Stud Fake Set", new Property("BBBB", typeof(double), ".", 2));
      dictProps.Add("Stud Fake Get", new Property("BBBB", typeof(double), ".", 3));
      dictProps.Add("Y Stud Count", new Property("Ny", typeof(int), "Arranger"));
      dictProps.Add("Stud X Spacing", new Property("Dx", typeof(double), "Arranger"));
      dictProps.Add("Stud Y Spacing", new Property("Dy", typeof(double), "Arranger"));
      dictProps.Add("Display Stud As Solid", new Property("ReprMode", typeof(int), "Z_PostWriteDB"));

      return dictProps.Where(x => (x.Value.PropertyDataOp % listFilter) == 0).ToDictionary(x => x.Key, x => x.Value);
    }

    private static Dictionary<string, Property> BuildGenericBeamPropertyList(int listFilter)
    {
      Dictionary<string, Property> dictProps = new Dictionary<string, Property>() { };
      //dictProps.Add("Select Beam Property...", new Property("none", typeof(string)));
      dictProps.Add("Beam Angle", new Property("Angle", typeof(double)));
      dictProps.Add("Beam Approval Comment", new Property("ApprovalComment", typeof(string)));
      dictProps.Add("Beam Approval Status Code", new Property("ApprovalStatusCode", typeof(string)));
      dictProps.Add("Beam Assembly", new Property("Assembly", typeof(string)));
      dictProps.Add("Beam Assembly Used For Numbering", new Property("AssemblyUsedForNumbering", typeof(int)));
      dictProps.Add("Beam Center Point", new Property("CenterPoint", typeof(Point3d), ".", 3));
      dictProps.Add("Beam Carrier", new Property("Carrier", typeof(string)));
      dictProps.Add("Beam Coating", new Property("Coating", typeof(string))); 
      dictProps.Add("Beam Coating Description", new Property("CoatingDescription", typeof(string), ".", 3));
      dictProps.Add("Beam Coating Used For Numbering", new Property("CoatingUsedForNumbering", typeof(int))); 
      dictProps.Add("Beam Delivery Date", new Property("DeliveryDate", typeof(string)));
      dictProps.Add("Beam Denotation Used For Numbering", new Property("DennotationUsedForNumbering", typeof(int))); 
      dictProps.Add("Beam Denotation Role", new Property("Denotation", typeof(string)));
      dictProps.Add("Beam Deviation", new Property("Deviation", typeof(double)));
      dictProps.Add("Beam Explicit Quantity", new Property("ExplicitQuantity", typeof(int))); 
      dictProps.Add("Beam Fabrication Station", new Property("FabricationStation", typeof(string)));
      dictProps.Add("Beam Fabrication Station UsedF or Numbering", new Property("FabricationStationUsedForNumbering", typeof(bool)));
      dictProps.Add("Beam Handle", new Property("Handle", typeof(string),".", 3));
      dictProps.Add("Beam Heat Number", new Property("HeatNumber", typeof(string)));
      dictProps.Add("Beam Heat Number Used For Numbering", new Property("HeatNumberUsedForNumbering", typeof(bool)));
      dictProps.Add("Beam Holes Used For Numbering", new Property("HolesUsedForNumbering", typeof(int))); 
      dictProps.Add("Beam Set IsMainPart Flag", new Property("IsMainPart", typeof(bool), "Z_PostWriteDB"));
      dictProps.Add("Beam Get IsAttachedPart Flag", new Property("IsAttachedPart", typeof(bool), ".", 3));
      dictProps.Add("Beam Get IsCrossSectionMirrored Flag", new Property("IsCrossSectionMirrored", typeof(bool), ".", 3));
      dictProps.Add("Beam ItemNumber", new Property("ItemNumber", typeof(string)));
      dictProps.Add("Beam ItemNumber Used For Numbering", new Property("ItemNumberUsedForNumbering", typeof(int))); 
      dictProps.Add("Beam Layer", new Property("Layer", typeof(string)));
      dictProps.Add("Beam Load Number", new Property("LoadNumber", typeof(string)));
      dictProps.Add("Beam MainPart Number", new Property("MainPartNumber", typeof(string)));
      dictProps.Add("Beam MainPart Number Prefix", new Property("MainPartPrefix", typeof(string)));
      dictProps.Add("Beam MainPart Used For BOM", new Property("MainPartUsedForBOM", typeof(int))); 
      dictProps.Add("Beam MainPart Used For Collision Check", new Property("MainPartUsedForCollisionCheck", typeof(int))); 
      dictProps.Add("Beam MainPart Used For Numbering", new Property("MainPartUsedForNumbering", typeof(int))); 
      dictProps.Add("Beam Material", new Property("Material", typeof(string)));
      dictProps.Add("Beam Material Description", new Property("MaterialDescription", typeof(string), ".", 3));
      dictProps.Add("Beam Material Used For Numbering", new Property("MaterialUsedForNumbering", typeof(int))); 
      dictProps.Add("Beam Note", new Property("Note", typeof(string)));
      dictProps.Add("Beam Note Used For Numbering", new Property("NoteUsedForNumbering", typeof(int))); 
      dictProps.Add("Beam Number Of Holes", new Property("NumberOfHoles", typeof(int), ".", 3)); 
      dictProps.Add("Beam PONumber", new Property("PONumber", typeof(string)));
      dictProps.Add("Beam PONumber Used For Numbering", new Property("PONumberUsedForNumbering", typeof(bool)));
      dictProps.Add("Beam Preliminary Part Number", new Property("PreliminaryPartNumber", typeof(string))); 
      dictProps.Add("Beam Preliminary Part Position Number", new Property("PreliminaryPartPositionNumber", typeof(string), ".", 3)); 
      dictProps.Add("Beam Preliminary Part Prefix", new Property("PreliminaryPartPrefix", typeof(string)));
      dictProps.Add("Beam Profile Name", new Property("ProfName", typeof(string)));
      dictProps.Add("Beam Profile Section Type", new Property("ProfSectionType", typeof(string), ".", 3));
      dictProps.Add("Beam Profile Section name", new Property("ProfSectionName", typeof(string), ".", 3));
      dictProps.Add("Beam Requisition Number", new Property("RequisitionNumber", typeof(string)));
      dictProps.Add("Beam Requisition Number Used For Numbering", new Property("RequisitionNumberUsedForNumbering", typeof(bool)));
      dictProps.Add("Beam Model Role", new Property("Role", typeof(string)));
      dictProps.Add("Beam Model Role Description", new Property("RoleDescription", typeof(string)));
      dictProps.Add("Beam Role Used For Numbering", new Property("RoleUsedForNumbering", typeof(int)));
      dictProps.Add("Beam Runname", new Property("Runname", typeof(string), ".", 3));
      dictProps.Add("Beam Shipped Date", new Property("ShippedDate", typeof(string)));
      dictProps.Add("Beam ShrinkValue", new Property("ShrinkValue", typeof(double), ".", 3));
      dictProps.Add("Beam Single Part Number", new Property("SinglePartNumber", typeof(string)));
      dictProps.Add("Beam Single Part Prefix", new Property("SinglePartPrefix", typeof(string)));
      dictProps.Add("Beam Single Part Used For BOM", new Property("SinglePartUsedForBOM", typeof(int))); 
      dictProps.Add("Beam Single Part Used For CollisionCheck", new Property("SinglePartUsedForCollisionCheck", typeof(int))); 
      dictProps.Add("Beam Single Part Used For Numbering", new Property("SinglePartUsedForNumbering", typeof(int)));
      dictProps.Add("Beam Specific Gravity", new Property("SpecificGravity", typeof(double), ".", 3)); 
      dictProps.Add("Beam Structural Member", new Property("StructuralMember", typeof(int)));
      dictProps.Add("Beam System Line Length", new Property("SysLength", typeof(double), ".", 3));
      dictProps.Add("Beam Supplier", new Property("Supplier", typeof(string)));
      dictProps.Add("Beam SupplierUsedForNumbering", new Property("SupplierUsedForNumbering", typeof(bool)));
      dictProps.Add("Beam Unwind / Unfolder", new Property("Unwind", typeof(bool)));
      dictProps.Add("Beam UnwindStartFactor", new Property("UnwindStartFactor", typeof(double)));
      dictProps.Add("Beam Volume", new Property("Volume", typeof(double), ".", 3));
      dictProps.Add("Change Beam Display Mode", new Property("ReprMode", typeof(int), "Z_PostWriteDB"));

      return dictProps.Where(x => (x.Value.PropertyDataOp % listFilter) == 0).ToDictionary(x => x.Key, x => x.Value);
    }

    private static Dictionary<string, Property> BuildStriaghtBeamPropertyList(int listFilter)
    {
      Dictionary<string, Property> dictProps = new Dictionary<string, Property>() { };
      dictProps.Add("Select Straight Beam Property...", new Property("none", typeof(string)));

      return dictProps.Where(x => (x.Value.PropertyDataOp % listFilter) == 0).ToDictionary(x => x.Key, x => x.Value);
    }

    private static Dictionary<string, Property> BuildCompoundStraightBeamPropertyList(int listFilter)
    {
      Dictionary<string, Property> dictProps = new Dictionary<string, Property>() { };
      dictProps.Add("Select Compound Straiht Beam Property...", new Property("none", typeof(string)));

      return dictProps.Where(x => (x.Value.PropertyDataOp % listFilter) == 0).ToDictionary(x => x.Key, x => x.Value);
    }

    private static Dictionary<string, Property> BuildTaperedBeamPropertyList(int listFilter)
    {
      Dictionary<string, Property> dictProps = new Dictionary<string, Property>() { };
      dictProps.Add("Select Tapered Beam Property...", new Property("none", typeof(string)));

      return dictProps.Where(x => (x.Value.PropertyDataOp % listFilter) == 0).ToDictionary(x => x.Key, x => x.Value);
    }

    private static Dictionary<string, Property> BuildBentBeamPropertyList(int listFilter)
    {
      Dictionary<string, Property> dictProps = new Dictionary<string, Property>() { };
      dictProps.Add("Select Bend Beam Property...", new Property("none", typeof(string)));
      dictProps.Add("BendBeam Offset Curve Radius", new Property("OffsetCurveRadius", typeof(double)));
      dictProps.Add("BendBeam Curve Offset", new Property("CurveOffset", typeof(double), ".", 3));
      dictProps.Add("BendBeam Systemline Radius", new Property("SystemlineRadius", typeof(double), ".", 3));

      return dictProps.Where(x => (x.Value.PropertyDataOp % listFilter) == 0).ToDictionary(x => x.Key, x => x.Value);
    }

    private static Dictionary<string, Property> BuildCompundBaseBeamPropertyList(int listFilter)
    {
      Dictionary<string, Property> dictProps = new Dictionary<string, Property>() { };
      dictProps.Add("Use Compound Beam As One Beam", new Property("UseCompoundAsOneBeam", typeof(bool)));
      dictProps.Add("Compound Beam ClassName", new Property("CompoundClassName", typeof(string), ".", 3));
      dictProps.Add("Compound Beam TypeName", new Property("CompoundTypeName", typeof(string), ".", 3));

      return dictProps.Where(x => (x.Value.PropertyDataOp % listFilter) == 0).ToDictionary(x => x.Key, x => x.Value);
    }

    public static void CheckListUpdateOrAddValue(List<Property> listOfPropertyData, 
                                                  string propName, 
                                                  object propValue, 
                                                  string propLevel = "",
                                                  int propertyDataOp = 6)
    {
      var foundItem = listOfPropertyData.FirstOrDefault<Property>(props => props.PropName == propName);
      if (foundItem != null)
      {
        foundItem.PropValue = propValue;
      }
      else
      {
        listOfPropertyData.Add(new Property(propName, propValue, propValue.GetType(), propLevel, propertyDataOp));
      }
    }

    public static void SetParameters(Autodesk.AdvanceSteel.Modelling.CountableScrewBoltPattern objToMod, List<Property> properties)
    {
      if (properties != null)
      {
        foreach (var prop in properties)
        {
          prop.UpdateASObject(objToMod);
        }
      }
    }

    public static void SetParameters(Autodesk.AdvanceSteel.Modelling.CircleScrewBoltPattern objToMod, List<Property> properties)
    {
      if (properties != null)
      {
        foreach (var prop in properties)
        {
          prop.UpdateASObject(objToMod);
        }
      }
    }
    
    public static void SetParameters(Autodesk.AdvanceSteel.Arrangement.Arranger objToMod, List<Property> properties)
    {
      if (properties != null)
      {
        foreach (var prop in properties)
        {
          prop.UpdateASObject(objToMod);
        }
      }
    }

    public static void SetParameters(Autodesk.AdvanceSteel.ConstructionTypes.AtomicElement objToMod, List<Property> properties)
    {
      if (properties != null) 
      { 
        foreach (var prop in properties)
        {
          prop.UpdateASObject(objToMod);
        }
      }
    }

    public static void SetParameters(Autodesk.AdvanceSteel.Modelling.AnchorPattern objToMod, List<Property> properties)
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