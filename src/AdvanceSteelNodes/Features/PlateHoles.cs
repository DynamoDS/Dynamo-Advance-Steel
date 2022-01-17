using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.ConstructionTypes;
using Autodesk.AdvanceSteel.Geometry;
using Autodesk.DesignScript.Runtime;
using System.Collections.Generic;
using System.Linq;
using SteelServices = Dynamo.Applications.AdvanceSteel.Services;
using Autodesk.AdvanceSteel.Modelling;
using AdvanceSteel.Nodes.Plates;
using DynGeometry = Autodesk.DesignScript.Geometry;
using SteelGeometry = Autodesk.AdvanceSteel.Geometry;
using Autodesk.AdvanceSteel.Arrangement;
using Autodesk.AdvanceSteel.Contours;
using System;

namespace AdvanceSteel.Nodes.Features
{
  /// <summary>
  /// Advance Steel Rectangular Hole Patterns
  /// </summary>
  [DynamoServices.RegisterForTrace]
  public class PlateHoles : GraphicObject
  {

    internal PlateHoles()
    {
    }

    internal PlateHoles(AdvanceSteel.Nodes.SteelDbObject element,
                                    SteelGeometry.Matrix3d cs,
                                    int holeType, int arrangementType, int boundType,
                                    List<Property> holeData)
    {
      lock (access_obj)
      {
        using (var ctx = new SteelServices.DocContext())
        {
          List<Property> defaultData = holeData.Where(x => x.Level == ".").ToList<Property>();
          List<Property> defaultHoleData = holeData.Where(x => x.Level == "-").ToList<Property>();
          List<Property> postWriteDBData = holeData.Where(x => x.Level == "Z_PostWriteDB").ToList<Property>();

          double dX, dY, diameterHole, slotLength, sunkDepth, alphaE, headDepth, radius, wX, wY, width, length;
          int nX, nY, isTappingRight, slotDirection;

          dX = (double)defaultHoleData.FirstOrDefault<Property>(x => x.Name == "Dx").InternalValue;
          dY = (double)defaultHoleData.FirstOrDefault<Property>(x => x.Name == "Dy").InternalValue;
          wX = (double)defaultHoleData.FirstOrDefault<Property>(x => x.Name == "Wx").InternalValue;
          wY = (double)defaultHoleData.FirstOrDefault<Property>(x => x.Name == "Wy").InternalValue;
          nX = (int)defaultHoleData.FirstOrDefault<Property>(x => x.Name == "Nx").InternalValue;
          nY = (int)defaultHoleData.FirstOrDefault<Property>(x => x.Name == "Ny").InternalValue;
          diameterHole = (double)defaultHoleData.FirstOrDefault<Property>(x => x.Name == "Diameter").InternalValue;
          slotLength = (double)defaultHoleData.FirstOrDefault<Property>(x => x.Name == "SlotLength").InternalValue;
          slotDirection = (int)defaultHoleData.FirstOrDefault<Property>(x => x.Name == "SlotDirection").InternalValue; 
          sunkDepth = (double)defaultHoleData.FirstOrDefault<Property>(x => x.Name == "SunkDepth").InternalValue;
          alphaE = (double)defaultHoleData.FirstOrDefault<Property>(x => x.Name == "AlphaE").InternalValue;
          isTappingRight = (int)defaultHoleData.FirstOrDefault<Property>(x => x.Name == "IsTappingRight").InternalValue;
          headDepth = (double)defaultHoleData.FirstOrDefault<Property>(x => x.Name == "HeadDepth").InternalValue;
          radius = (double)defaultHoleData.FirstOrDefault<Property>(x => x.Name == "Radius").InternalValue;
          width = (double)defaultHoleData.FirstOrDefault<Property>(x => x.Name == "Width").InternalValue;
          length = (double)defaultHoleData.FirstOrDefault<Property>(x => x.Name == "Length").InternalValue;

          Autodesk.AdvanceSteel.Modelling.ConnectionHolePlate holes = null;

          string existingFeatureHandle = SteelServices.ElementBinder.GetHandleFromTrace();
          string elementHandle = element.Handle;
          FilerObject obj = Utils.GetObject(elementHandle);

          if (obj != null && obj.IsKindOf(FilerObject.eObjectType.kPlate))
          {
            if (string.IsNullOrEmpty(existingFeatureHandle) || Utils.GetObject(existingFeatureHandle) == null)
            {
              Autodesk.AdvanceSteel.Modelling.Plate atomic = obj as Autodesk.AdvanceSteel.Modelling.Plate;

              holes = new Autodesk.AdvanceSteel.Modelling.ConnectionHolePlate(atomic, cs);

              switch (arrangementType)
              {
                case 0:
                  holes.Arranger = new RectangularArranger(new Matrix2d(), dX, dY, nX, nY);
                  break;
                case 1:
                  holes.Arranger = new CircleArranger(new Matrix2d(), radius, nX);
                  break;
                case 2:
                  BoundedRectArranger bArr = new BoundedRectArranger((Arranger.eBoundedSide)boundType);
                  if (boundType == 1)
                  {
                    bArr.Length = width;
                    bArr.Width = length;
                  }
                  bArr.Wx = wX;
                  bArr.Wy = wY;
                  break;
              }

              holes.SetHoleType((Autodesk.AdvanceSteel.Contours.Hole.eHoleType)holeType);

              switch (holeType)
              {
                case 1:
                  //Hole
                  Hole hol = new Hole(diameterHole);
                  holes.Hole = hol;
                  break;
                case 2:
                  //Slotted Hole
                  SlottedHole slhol = new SlottedHole(diameterHole, slotLength, (SlottedHole.eDirection)slotDirection); 
                  holes.Hole = slhol;
                  break;
                case 3:
                  //Counter Sunk Hole
                  ExtendedHole cSunkHole = new ExtendedHole(Hole.eHoleType.kCounterSunk);
                  cSunkHole.Alpha_e = alphaE; //Angle of of Sunk Hole up
                  cSunkHole.Diameter = diameterHole; //Diamter of Hole at the top of the sink hole
                  cSunkHole.SunkDepth = sunkDepth; //overall Depth of sink
                  holes.Hole = cSunkHole;
                  break;
                case 4:
                  //Blind Hole
                  ExtendedHole blindHole = new ExtendedHole(Hole.eHoleType.kBlindHole);
                  blindHole.Diameter = diameterHole; //Diamter of Hole at the top of the blind hole
                  blindHole.SunkDepth = sunkDepth; //overall Depth of blind hole
                  holes.Hole = blindHole;
                  break;
                case 5:
                  //Threaded Hole
                  ExtendedHole thrdHole = new ExtendedHole(Hole.eHoleType.kThreadHole);
                  thrdHole.Diameter = diameterHole; //Diamter of holes to be threaded
                  thrdHole.IsTappingRight = Convert.ToBoolean(isTappingRight); //Tickbox
                  thrdHole.HeadDiameter = headDepth; //Tapping Holes size
                  thrdHole.SunkDepth = sunkDepth; //Overall Threaded Hole depth distance
                  thrdHole.Alpha_e = alphaE;//back taper thread
                  holes.Hole = thrdHole;
                  break;
                case 6:
                  //Sunk Hole
                  ExtendedHole sunkHole = new ExtendedHole(Hole.eHoleType.kSunkBolt);
                  sunkHole.Alpha_e = 45; //Angle of of Sunk Hole up
                  sunkHole.Diameter = 12; //Diamter of Hole at the bottom of the sunk hole
                  sunkHole.HeadDiameter = 15; //Sunk Diameter
                  sunkHole.SunkDepth = 6; //Depth of bottom of sunk
                  holes.Hole = sunkHole;
                  break;
                case 7:
                  //Punch Mark
			            ExtendedHole punchMark = new ExtendedHole(Hole.eHoleType.kPunchMark);
                  holes.Hole = punchMark;
                  break;
              }

              if (defaultData != null)
              {
                Utils.SetParameters(holes, defaultData);
              }

              atomic.AddFeature(holes);

              if (postWriteDBData != null)
              {
                Utils.SetParameters(holes, postWriteDBData);
              }

            }
            else
            {
              holes = Utils.GetObject(existingFeatureHandle) as Autodesk.AdvanceSteel.Modelling.ConnectionHolePlate;
              if (holes != null && holes.IsKindOf(FilerObject.eObjectType.kConnectionHolePlate))
              {
                holes.CS = cs;
                holes.SetHoleType((Autodesk.AdvanceSteel.Contours.Hole.eHoleType)holeType);
                holes.Arranger.Dx = dX;
                holes.Arranger.Dy = dY;
                holes.Arranger.Nx = nX;
                holes.Arranger.Ny = nY;

                switch (holeType)
                {
                  case 1:
                    //Hole
                    Hole hol = holes.Hole;
                    hol.Diameter = diameterHole;
                    holes.Hole = hol;
                    break;
                  case 2:
                    //Slotted Hole
                    SlottedHole slhol = (SlottedHole)holes.Hole;
                    slhol.Diameter = diameterHole;
                    slhol.SlotLength = slotLength;
                    slhol.SlotDirection = (SlottedHole.eDirection)slotDirection;
                    holes.Hole = slhol;
                    break;
                  case 3:
                    //Counter Sunk Hole
                    ExtendedHole cSunkHole = (ExtendedHole)holes.Hole;
                    cSunkHole.Alpha_e = alphaE; //Angle of of Sunk Hole up
                    cSunkHole.Diameter = diameterHole; //Diamter of Hole at the top of the sink hole
                    cSunkHole.SunkDepth = sunkDepth; //overall Depth of sink
                    holes.Hole = cSunkHole;
                    break;
                  case 4:
                    //Blind Hole
                    ExtendedHole blindHole = (ExtendedHole)holes.Hole;
                    blindHole.Diameter = diameterHole; //Diamter of Hole at the top of the blind hole
                    blindHole.SunkDepth = sunkDepth; //overall Depth of blind hole
                    holes.Hole = blindHole;
                    break;
                  case 5:
                    //Threaded Hole
                    ExtendedHole thrdHole = (ExtendedHole)holes.Hole;
                    thrdHole.Diameter = diameterHole; //Diamter of holes to be threaded
                    thrdHole.IsTappingRight = Convert.ToBoolean(isTappingRight); //Tickbox
                    thrdHole.HeadDiameter = headDepth; //Tapping Holes size
                    thrdHole.SunkDepth = sunkDepth; //Overall Threaded Hole depth distance
                    thrdHole.Alpha_e = alphaE;//back taper thread
                    holes.Hole = thrdHole;
                    break;
                  case 6:
                    //Sunk Hole
                    ExtendedHole sunkHole = (ExtendedHole)holes.Hole;
                    sunkHole.Alpha_e = 45; //Angle of of Sunk Hole up
                    sunkHole.Diameter = 12; //Diamter of Hole at the bottom of the sunk hole
                    sunkHole.HeadDiameter = 15; //Sunk Diameter
                    sunkHole.SunkDepth = 6; //Depth of bottom of sunk
                    holes.Hole = sunkHole;
                    break;
                  case 7:
                    //Punch Mark
                    ExtendedHole punchMark = (ExtendedHole)holes.Hole;
                    holes.Hole = punchMark;
                    break;
                }

                if (defaultData != null)
                {
                  Utils.SetParameters(holes, defaultData);
                }

                if (postWriteDBData != null)
                {
                  Utils.SetParameters(holes, postWriteDBData);
                }
              }
              else
                throw new System.Exception("Not a rectangular pattern for Plates");
            }
          }

          Handle = holes.Handle;
          SteelServices.ElementBinder.CleanupAndSetElementForTrace(holes);
        }
      }
    }

    #region "Hole = 1"
    public static PlateHoles RectHoleByCS(AdvanceSteel.Nodes.SteelDbObject element,
                                                      Autodesk.DesignScript.Geometry.CoordinateSystem coordinateSystem,
                                                      [DefaultArgument("2;")] int noOfHolesX,
                                                      [DefaultArgument("2;")] int noOfHolesY,
                                                      double spacingBoltsX,
                                                      double spacingBoltsY,
                                                      double diameterHole,
                                                      [DefaultArgument("null")] List<Property> additionalHoleParameters)
    {
      additionalHoleParameters = PreSetValuesInListProps(additionalHoleParameters, noOfHolesX, noOfHolesY, Utils.ToInternalDistanceUnits(spacingBoltsX, true), Utils.ToInternalDistanceUnits(spacingBoltsY, true),
                                                          Utils.ToInternalDistanceUnits(diameterHole, true), 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0);
      return new PlateHoles(element, Utils.ToAstMatrix3d(coordinateSystem, true), 1, 0, -1, additionalHoleParameters);
    }

    public static PlateHoles CircularHoleByCS(AdvanceSteel.Nodes.SteelDbObject element,
                                                  Autodesk.DesignScript.Geometry.CoordinateSystem coordinateSystem,
                                                  [DefaultArgument("6;")] int noOfHolesX,
                                                  double radius,
                                                  double diameterHole,
                                                  [DefaultArgument("null")] List<Property> additionalHoleParameters)
    {
      additionalHoleParameters = PreSetValuesInListProps(additionalHoleParameters, noOfHolesX, 0, 0, 0,
                                                          Utils.ToInternalDistanceUnits(diameterHole, true), 0, 1, 0, 0, 0, 0,
                                                          Utils.ToInternalDistanceUnits(radius, true), 0, 0, 0, 0);
      return new PlateHoles(element, Utils.ToAstMatrix3d(coordinateSystem, true), 1, 1, -1, additionalHoleParameters);
    }
    #endregion

    #region "Slotted Holes = 2"
    public static PlateHoles RectSlottedHoleByCS(AdvanceSteel.Nodes.SteelDbObject element,
                                                      Autodesk.DesignScript.Geometry.CoordinateSystem coordinateSystem,
                                                      [DefaultArgument("2;")] int noOfHolesX,
                                                      [DefaultArgument("2;")] int noOfHolesY,
                                                      double spacingBoltsX,
                                                      double spacingBoltsY,
                                                      double diameterHole, 
                                                      double slotLength,
                                                      [DefaultArgument("1;")] int slotDirection,
                                                      [DefaultArgument("null")] List<Property> additionalHoleParameters)
    {
      additionalHoleParameters = PreSetValuesInListProps(additionalHoleParameters, noOfHolesX, noOfHolesY, Utils.ToInternalDistanceUnits(spacingBoltsX, true), Utils.ToInternalDistanceUnits(spacingBoltsY, true),
                                                          Utils.ToInternalDistanceUnits(diameterHole, true), Utils.ToInternalDistanceUnits(slotLength, true), slotDirection, 0, 0, 0, 0, 0, 0, 0, 0, 0);
      return new PlateHoles(element, Utils.ToAstMatrix3d(coordinateSystem, true), 2, 0, -1, additionalHoleParameters);
    }

    public static PlateHoles CircularSlottedHoleByCS(AdvanceSteel.Nodes.SteelDbObject element,
                                                  Autodesk.DesignScript.Geometry.CoordinateSystem coordinateSystem,
                                                  [DefaultArgument("6;")] int noOfHolesX,
                                                  double radius,
                                                  double diameterHole, double slotLength,
                                                  [DefaultArgument("1;")] int slotDirection,
                                                  [DefaultArgument("null")] List<Property> additionalHoleParameters)
    {
      additionalHoleParameters = PreSetValuesInListProps(additionalHoleParameters, noOfHolesX, 0, 0, 0,
                                                          Utils.ToInternalDistanceUnits(diameterHole, true), Utils.ToInternalDistanceUnits(slotLength, true), 
                                                          slotDirection, 0, 0, 0, 0, 
                                                          Utils.ToInternalDistanceUnits(radius, true), 0, 0, 0, 0);
      return new PlateHoles(element, Utils.ToAstMatrix3d(coordinateSystem, true), 2, 1, -1, additionalHoleParameters);
    }
    #endregion

    #region "Counter Sink Hole = 3"
    public static PlateHoles RectCSunkHoleByCS(AdvanceSteel.Nodes.SteelDbObject element,
                                                      Autodesk.DesignScript.Geometry.CoordinateSystem coordinateSystem,
                                                      [DefaultArgument("2;")] int noOfHolesX,
                                                      [DefaultArgument("2;")] int noOfHolesY,
                                                      double spacingBoltsX,
                                                      double spacingBoltsY,
                                                      double diameterHole,
                                                      double sunkDepth,
                                                      double sunkAngle,
                                                      [DefaultArgument("null")] List<Property> additionalHoleParameters)
    {
      additionalHoleParameters = PreSetValuesInListProps(additionalHoleParameters, noOfHolesX, noOfHolesY, Utils.ToInternalDistanceUnits(spacingBoltsX, true), Utils.ToInternalDistanceUnits(spacingBoltsY, true),
                                                          Utils.ToInternalDistanceUnits(diameterHole, true), 0, 0, 
                                                          Utils.ToInternalDistanceUnits(sunkDepth, true), 
                                                          Utils.ToInternalAngleUnits(sunkAngle, true), 0, 0, 0, 0, 0, 0, 0);
      return new PlateHoles(element, Utils.ToAstMatrix3d(coordinateSystem, true), 3, 0, -1, additionalHoleParameters);
    }

    public static PlateHoles CircularCSunkHoleByCS(AdvanceSteel.Nodes.SteelDbObject element,
                                                  Autodesk.DesignScript.Geometry.CoordinateSystem coordinateSystem,
                                                  [DefaultArgument("6;")] int noOfHolesX,
                                                  double radius,
                                                  double diameterHole,
                                                  double sunkDepth,
                                                  double sunkAngle,
                                                  [DefaultArgument("null")] List<Property> additionalHoleParameters)
    {
      additionalHoleParameters = PreSetValuesInListProps(additionalHoleParameters, noOfHolesX, 0, 0, 0,
                                                          Utils.ToInternalDistanceUnits(diameterHole, true), 0, 0,
                                                          Utils.ToInternalDistanceUnits(sunkDepth, true), 
                                                          Utils.ToInternalAngleUnits(sunkAngle, true), 0, 0,
                                                          Utils.ToInternalDistanceUnits(radius, true), 0, 0, 0, 0);
      return new PlateHoles(element, Utils.ToAstMatrix3d(coordinateSystem, true), 3, 1, -1, additionalHoleParameters);
    }
    #endregion

    #region "Blind Hole = 4"
    public static PlateHoles RectBlindHoleByCS(AdvanceSteel.Nodes.SteelDbObject element,
                                                      Autodesk.DesignScript.Geometry.CoordinateSystem coordinateSystem,
                                                      [DefaultArgument("2;")] int noOfHolesX,
                                                      [DefaultArgument("2;")] int noOfHolesY,
                                                      double spacingBoltsX,
                                                      double spacingBoltsY,
                                                      double diameterHole,
                                                      double blindHoleDepth,
                                                      [DefaultArgument("null")] List<Property> additionalHoleParameters)
    {
      additionalHoleParameters = PreSetValuesInListProps(additionalHoleParameters, noOfHolesX, noOfHolesY, Utils.ToInternalDistanceUnits(spacingBoltsX, true), Utils.ToInternalDistanceUnits(spacingBoltsY, true),
                                                          Utils.ToInternalDistanceUnits(diameterHole, true), 0, 0,
                                                          Utils.ToInternalDistanceUnits(blindHoleDepth, true),
                                                          0, 0, 0, 0, 0, 0, 0, 0);
      return new PlateHoles(element, Utils.ToAstMatrix3d(coordinateSystem, true), 4, 0, -1, additionalHoleParameters);
    }

    public static PlateHoles CircularBlindHoleByCS(AdvanceSteel.Nodes.SteelDbObject element,
                                                  Autodesk.DesignScript.Geometry.CoordinateSystem coordinateSystem,
                                                  [DefaultArgument("6;")] int noOfHolesX,
                                                  double radius,
                                                  double diameterHole,
                                                  double blindHoleDepth,
                                                  [DefaultArgument("null")] List<Property> additionalHoleParameters)
    {
      additionalHoleParameters = PreSetValuesInListProps(additionalHoleParameters, noOfHolesX, 0, 0, 0,
                                                          Utils.ToInternalDistanceUnits(diameterHole, true), 0, 0,
                                                          Utils.ToInternalDistanceUnits(blindHoleDepth, true),
                                                          0, 0, 0,
                                                          Utils.ToInternalDistanceUnits(radius, true), 0, 0, 0, 0);
      return new PlateHoles(element, Utils.ToAstMatrix3d(coordinateSystem, true), 4, 1, -1, additionalHoleParameters);
    }
    #endregion

    #region "Threaded Hole = 5"
    public static PlateHoles RectThreadedHoleByCS(AdvanceSteel.Nodes.SteelDbObject element,
                                                      Autodesk.DesignScript.Geometry.CoordinateSystem coordinateSystem,
                                                      [DefaultArgument("2;")] int noOfHolesX,
                                                      [DefaultArgument("2;")] int noOfHolesY,
                                                      double spacingBoltsX,
                                                      double spacingBoltsY,
                                                      double diameterHole,
                                                      [DefaultArgument("0;")] int threadTappingRight,
                                                      double tappingHoleSize,
                                                      double threadHoleDepth,
                                                      double threadTaper,
                                                      [DefaultArgument("null")] List<Property> additionalHoleParameters)
    {
      additionalHoleParameters = PreSetValuesInListProps(additionalHoleParameters, noOfHolesX, noOfHolesY, Utils.ToInternalDistanceUnits(spacingBoltsX, true), 
                                                          Utils.ToInternalDistanceUnits(spacingBoltsY, true),
                                                          Utils.ToInternalDistanceUnits(diameterHole, true), 0, 0,
                                                          Utils.ToInternalDistanceUnits(threadHoleDepth, true), 
                                                          Utils.ToInternalAngleUnits(threadTaper, true), threadTappingRight,
                                                          Utils.ToInternalDistanceUnits(tappingHoleSize, true), 0, 0, 0, 0, 0);
      return new PlateHoles(element, Utils.ToAstMatrix3d(coordinateSystem, true), 5, 0, -1, additionalHoleParameters);
    }

    public static PlateHoles CircularThreadedHoleByCS(AdvanceSteel.Nodes.SteelDbObject element,
                                                  Autodesk.DesignScript.Geometry.CoordinateSystem coordinateSystem,
                                                  [DefaultArgument("6;")] int noOfHolesX,
                                                  double radius,
                                                  double diameterHole,
                                                  [DefaultArgument("0;")] int threadTappingRight,
                                                  double tappingHoleSize,
                                                  double threadHoleDepth,
                                                  double threadTaper,
                                                  [DefaultArgument("null")] List<Property> additionalHoleParameters)
    {
      additionalHoleParameters = PreSetValuesInListProps(additionalHoleParameters, noOfHolesX, 0, 0, 0,
                                                          Utils.ToInternalDistanceUnits(diameterHole, true), 0, 0,
                                                          Utils.ToInternalDistanceUnits(threadHoleDepth, true),
                                                          Utils.ToInternalAngleUnits(threadTaper, true), threadTappingRight,
                                                          Utils.ToInternalDistanceUnits(tappingHoleSize, true), 
                                                          Utils.ToInternalDistanceUnits(radius, true), 0, 0, 0, 0);
      return new PlateHoles(element, Utils.ToAstMatrix3d(coordinateSystem, true), 5, 1, -1, additionalHoleParameters);
    }
    #endregion

    #region "Sunk Hole = 6"
    public static PlateHoles RectSunkHoleByCS(AdvanceSteel.Nodes.SteelDbObject element,
                                                      Autodesk.DesignScript.Geometry.CoordinateSystem coordinateSystem,
                                                      [DefaultArgument("2;")] int noOfHolesX,
                                                      [DefaultArgument("2;")] int noOfHolesY,
                                                      double spacingBoltsX,
                                                      double spacingBoltsY,
                                                      double diameterHole,
                                                      double sunkDiameter,
                                                      double holeDepth,
                                                      double holeTaper,
                                                      [DefaultArgument("null")] List<Property> additionalHoleParameters)
    {
      additionalHoleParameters = PreSetValuesInListProps(additionalHoleParameters, noOfHolesX, noOfHolesY, Utils.ToInternalDistanceUnits(spacingBoltsX, true),
                                                          Utils.ToInternalDistanceUnits(spacingBoltsY, true),
                                                          Utils.ToInternalDistanceUnits(diameterHole, true), 0, 0,
                                                          Utils.ToInternalDistanceUnits(holeDepth, true),
                                                          Utils.ToInternalAngleUnits(holeTaper, true), 0,
                                                          Utils.ToInternalDistanceUnits(sunkDiameter, true), 0, 0, 0, 0, 0);
      return new PlateHoles(element, Utils.ToAstMatrix3d(coordinateSystem, true), 6, 0, -1, additionalHoleParameters);
    }

    public static PlateHoles CircularSunkHoleByCS(AdvanceSteel.Nodes.SteelDbObject element,
                                                  Autodesk.DesignScript.Geometry.CoordinateSystem coordinateSystem,
                                                  [DefaultArgument("6;")] int noOfHolesX,
                                                  double radius,
                                                  double diameterHole,
                                                  double sunkDiameter,
                                                  double holeDepth,
                                                  double holeTaper,
                                                  [DefaultArgument("null")] List<Property> additionalHoleParameters)
    {
      additionalHoleParameters = PreSetValuesInListProps(additionalHoleParameters, noOfHolesX, 0, 0, 0,
                                                          Utils.ToInternalDistanceUnits(diameterHole, true), 0, 0,
                                                          Utils.ToInternalDistanceUnits(holeDepth, true),
                                                          Utils.ToInternalAngleUnits(holeTaper, true), 0,
                                                          Utils.ToInternalDistanceUnits(sunkDiameter, true),
                                                          Utils.ToInternalDistanceUnits(radius, true), 0, 0, 0, 0);
      return new PlateHoles(element, Utils.ToAstMatrix3d(coordinateSystem, true), 6, 1, -1, additionalHoleParameters);
    }
    #endregion

    #region "Punch Mark = 7"
    public static PlateHoles RectPunchMarkByCS(AdvanceSteel.Nodes.SteelDbObject element,
                                                      Autodesk.DesignScript.Geometry.CoordinateSystem coordinateSystem,
                                                      [DefaultArgument("2;")] int noOfHolesX,
                                                      [DefaultArgument("2;")] int noOfHolesY,
                                                      double spacingBoltsX,
                                                      double spacingBoltsY,
                                                      [DefaultArgument("null")] List<Property> additionalHoleParameters)
    {
      additionalHoleParameters = PreSetValuesInListProps(additionalHoleParameters, noOfHolesX, noOfHolesY, Utils.ToInternalDistanceUnits(spacingBoltsX, true), Utils.ToInternalDistanceUnits(spacingBoltsY, true),
                                                          0, 0, 0, 0,
                                                          0, 0, 0, 0, 0, 0, 0, 0);
      return new PlateHoles(element, Utils.ToAstMatrix3d(coordinateSystem, true), 7, 0, -1, additionalHoleParameters);
    }

    public static PlateHoles CircularPunchMarkByCS(AdvanceSteel.Nodes.SteelDbObject element,
                                                  Autodesk.DesignScript.Geometry.CoordinateSystem coordinateSystem,
                                                  [DefaultArgument("6;")] int noOfHolesX,
                                                  double radius,
                                                  [DefaultArgument("null")] List<Property> additionalHoleParameters)
    {
      additionalHoleParameters = PreSetValuesInListProps(additionalHoleParameters, noOfHolesX, 0, 0, 0,
                                                          0, 0, 0, 0,
                                                          0, 0, 0,
                                                          Utils.ToInternalDistanceUnits(radius, true), 0, 0, 0, 0);
      return new PlateHoles(element, Utils.ToAstMatrix3d(coordinateSystem, true), 7, 1, -1, additionalHoleParameters);
    }
    #endregion

    private static List<Property> PreSetValuesInListProps(List<Property> listOfHoleParameters, int nx, int ny, double dx, double dy, double diameterHole,
                                                          double slotLength = 0, int slotDirection = 1, double sunkDepth = 0, double alphaE = 0, 
                                                          int isTappingRight = 0, double headDepth = 0, double radius = 0, double length = 0,
                                                          double width = 0, double wx = 0, double wy = 0)
    {
      if (listOfHoleParameters == null)
      {
        listOfHoleParameters = new List<Property>() { };
      }

      Utils.CheckListUpdateOrAddValue(listOfHoleParameters, "Nx", nx, "-");
      Utils.CheckListUpdateOrAddValue(listOfHoleParameters, "Ny", ny, "-");
      Utils.CheckListUpdateOrAddValue(listOfHoleParameters, "Dx", dx, "-");
      Utils.CheckListUpdateOrAddValue(listOfHoleParameters, "Dy", dy, "-"); 
      Utils.CheckListUpdateOrAddValue(listOfHoleParameters, "Diameter", diameterHole, "-");
      Utils.CheckListUpdateOrAddValue(listOfHoleParameters, "SlotLength", slotLength, "-");
      Utils.CheckListUpdateOrAddValue(listOfHoleParameters, "SlotDirection", slotDirection, "-");
      Utils.CheckListUpdateOrAddValue(listOfHoleParameters, "SunkDepth", sunkDepth, "-");
      Utils.CheckListUpdateOrAddValue(listOfHoleParameters, "AlphaE", alphaE, "-");
      Utils.CheckListUpdateOrAddValue(listOfHoleParameters, "IsTappingRight", isTappingRight, "-");
      Utils.CheckListUpdateOrAddValue(listOfHoleParameters, "HeadDepth", headDepth, "-");

      Utils.CheckListUpdateOrAddValue(listOfHoleParameters, "Radius", radius, "-");
      Utils.CheckListUpdateOrAddValue(listOfHoleParameters, "Length", length, "-");
      Utils.CheckListUpdateOrAddValue(listOfHoleParameters, "Width", width, "-");
      Utils.CheckListUpdateOrAddValue(listOfHoleParameters, "Wx", wx, "-");
      Utils.CheckListUpdateOrAddValue(listOfHoleParameters, "Wy", wy, "-");


      return listOfHoleParameters;
    }

    [IsVisibleInDynamoLibrary(false)]
    public override IEnumerable<Autodesk.DesignScript.Interfaces.IGraphicItem> GetDynGeometry()
    {
      IList<Autodesk.DesignScript.Interfaces.IGraphicItem> ret = new List<Autodesk.DesignScript.Interfaces.IGraphicItem>();

      lock (access_obj)
      {
        using (var ctx = new SteelServices.DocContext())
        {
          var boltPattern = Utils.GetObject(Handle) as Autodesk.AdvanceSteel.Modelling.ConnectionHolePlate;

          if (boltPattern == null)
          {
            throw new Exception("Null Plate Holes pattern");
          }

          SteelGeometry.Point3d[] defPoints = null;
          boltPattern.GetElementOrigins(out defPoints);
          Hole hl = boltPattern.Hole;
          double rad = 0;
          if (hl.Diameter != 0)
          {
            rad = hl.Diameter / 2;
          }
          else
          {
            rad = 2;
          }

          foreach (var item in defPoints)
          {

            var line1 = Autodesk.DesignScript.Geometry.Circle.ByCenterPointRadius(Utils.ToDynPoint(item, true), rad);
            ret.Add(line1);

          }
        }
      }

      return ret;
    }

  }
}
