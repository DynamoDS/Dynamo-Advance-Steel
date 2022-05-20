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
using ASConnectionHolePlate = Autodesk.AdvanceSteel.Modelling.ConnectionHolePlate;

namespace AdvanceSteel.Nodes.Features
{
  /// <summary>
  /// Advance Steel Rectangular Hole Patterns
  /// </summary>
  [DynamoServices.RegisterForTrace]
  public class PlateHoles : GraphicObject
  {

    private PlateHoles(AdvanceSteel.Nodes.SteelDbObject element,
                                    SteelGeometry.Matrix3d cs,
                                    int holeType, int arrangementType, int boundType,
                                    List<Property> holeData)
    {
      SafeInit(() => InitPlateHoles(element, cs, holeType, arrangementType, boundType, holeData));
    }

    private PlateHoles(ASConnectionHolePlate holes)
    {
      SafeInit(() => SetHandle(holes));
    }

    internal static PlateHoles FromExisting(ASConnectionHolePlate holes)
    {
      return new PlateHoles(holes)
      {
        IsOwnedByDynamo = false
      };
    }

    private void InitPlateHoles(AdvanceSteel.Nodes.SteelDbObject element,
                                    SteelGeometry.Matrix3d cs,
                                    int holeType, int arrangementType, int boundType,
                                    List<Property> holeData)
    {
      List<Property> defaultData = holeData.Where(x => x.Level == LevelEnum.Default).ToList<Property>();
      List<Property> defaultHoleData = holeData.Where(x => x.Level == LevelEnum.HoleDefault).ToList<Property>();
      List<Property> postWriteDBData = holeData.Where(x => x.Level == LevelEnum.PostWriteDB).ToList<Property>();

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

      FilerObject obj = Utils.GetObject(element.Handle);
      if (obj == null || !(obj.IsKindOf(FilerObject.eObjectType.kPlate)))
        throw new System.Exception("No Input Element found");

      ASConnectionHolePlate holes = SteelServices.ElementBinder.GetObjectASFromTrace<ASConnectionHolePlate>();
      if (holes == null)
      {
        Autodesk.AdvanceSteel.Modelling.Plate atomic = obj as Autodesk.AdvanceSteel.Modelling.Plate;

        holes = new ASConnectionHolePlate(atomic, cs);

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
            //if (boundType == 1)
            //{
            bArr.Length = width;
            bArr.Width = length;
            //}
            bArr.Nx = nX;
            bArr.Ny = nY;
            bArr.Wx = wX;
            bArr.Wy = wY;
            holes.Arranger = bArr;
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
      }
      else
      {
        if (!holes.IsKindOf(FilerObject.eObjectType.kConnectionHolePlate))
          throw new System.Exception("Not a rectangular pattern for Plates");

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
      }

      SetHandle(holes);

      if (postWriteDBData != null)
      {
        Utils.SetParameters(holes, postWriteDBData);
      }

      SteelServices.ElementBinder.CleanupAndSetElementForTrace(holes);
    }

    #region "Hole = 1"
    /// <summary>
    /// Create Rectangular Hole Pattern by Coordinate System
    /// </summary>
    /// <param name="element"> Steel element to recieve Holes</param>
    /// <param name="coordinateSystem"> Dynamo Coordinate System</param>
    /// <param name="noOfHolesX"> Number of holes in the X Direction of the Pattern</param>
    /// <param name="noOfHolesY"> Number of holes in the Y Direction of the Pattern</param>
    /// <param name="spacingBoltsX"> Spacing between holes in the X Direction</param>
    /// <param name="spacingBoltsY"> Spacing between holes in the Y Direction</param>
    /// <param name="diameterHole"> Hole Diameter</param>
    /// <param name="additionalHoleParameters"> Optional Input Hole Build Properties </param>
    /// <returns></returns>
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

    /// <summary>
    /// Create Circular Hole Pattern by Coordinate System
    /// </summary>
    /// <param name="element"> Steel element to recieve Holes</param>
    /// <param name="coordinateSystem"> Dynamo Coordinate System</param>
    /// <param name="noOfHolesX"> Number of holes in the Circular Pattern</param>
    /// <param name="radius"> Circular Hole Patern Radius</param>
    /// <param name="diameterHole"> Hole Diameter</param>
    /// <param name="additionalHoleParameters"> Optional Input Hole Build Properties </param>
    /// <returns></returns>
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
    /// <summary>
    /// Create Rectangular Slotted Hole Pattern by Coordinate System
    /// </summary>
    /// <param name="element"> Steel element to recieve Holes</param>
    /// <param name="coordinateSystem"> Dynamo Coordinate System</param>
    /// <param name="noOfHolesX"> Number of holes in the X Direction of the Pattern</param>
    /// <param name="noOfHolesY"> Number of holes in the Y Direction of the Pattern</param>
    /// <param name="spacingBoltsX"> Spacing between holes in the X Direction</param>
    /// <param name="spacingBoltsY"> Spacing between holes in the Y Direction</param>
    /// <param name="diameterHole"> Hole Diameter</param>
    /// <param name="slotLength"> Slot Length of Hole</param>
    /// <param name="slotDirection"> Slot Direction of Hole 0 - Along Arc, 1 - X Axis, 2 - Y Axis</param>
    /// <param name="additionalHoleParameters"> Optional Input Hole Build Properties </param>
    /// <returns></returns>
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

    /// <summary>
    /// Create Circular Slotted Hole Pattern by Coordinate System
    /// </summary>
    /// <param name="element"> Steel element to recieve Holes</param>
    /// <param name="coordinateSystem"> Dynamo Coordinate System</param>
    /// <param name="noOfHolesX"> Number of holes in the Circular Pattern</param>
    /// <param name="radius"> Circular Hole Patern Radius</param>
    /// <param name="diameterHole"> Hole Diameter</param>
    /// <param name="slotLength"> Slot Length of Hole</param>
    /// <param name="slotDirection"> Slot Direction of Hole 0 - Along Arc, 1 - X Axis, 2 - Y Axis</param>
    /// <param name="additionalHoleParameters"> Optional Input Hole Build Properties </param>
    /// <returns></returns>
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
    /// <summary>
    /// Create Rectangular CSunk Hole Pattern by Coordinate System
    /// </summary>
    /// <param name="element"> Steel element to recieve Holes</param>
    /// <param name="coordinateSystem"> Dynamo Coordinate System</param>
    /// <param name="noOfHolesX"> Number of holes in the X Direction of the Pattern</param>
    /// <param name="noOfHolesY"> Number of holes in the Y Direction of the Pattern</param>
    /// <param name="spacingBoltsX"> Spacing between holes in the X Direction</param>
    /// <param name="spacingBoltsY"> Spacing between holes in the Y Direction</param>
    /// <param name="diameterHole"> Hole Diameter</param>
    /// <param name="sunkDepth"> Hole Sunk Depth</param>
    /// <param name="sunkAngle"> Hole Sunk Angle</param>
    /// <param name="additionalHoleParameters"> Optional Input Hole Build Properties </param>
    /// <returns></returns>
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

    /// <summary>
    /// Create Circular CSunk Hole Pattern by Coordinate System
    /// </summary>
    /// <param name="element"> Steel element to recieve Holes</param>
    /// <param name="coordinateSystem"> Dynamo Coordinate System</param>
    /// <param name="noOfHolesX"> Number of holes in the Circular Pattern</param>
    /// <param name="radius"> Circular Hole Patern Radius</param>
    /// <param name="diameterHole"> Hole Diameter</param>
    /// <param name="sunkDepth"> Hole Sunk Depth</param>
    /// <param name="sunkAngle"> Hole Sunk Angle</param>
    /// <param name="additionalHoleParameters"> Optional Input Hole Build Properties </param>
    /// <returns></returns>
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
    /// <summary>
    /// Create Rectangular Blind Hole Pattern by Coordinate System
    /// </summary>
    /// <param name="element"> Steel element to recieve Holes</param>
    /// <param name="coordinateSystem"> Dynamo Coordinate System</param>
    /// <param name="noOfHolesX"> Number of holes in the X Direction of the Pattern</param>
    /// <param name="noOfHolesY"> Number of holes in the Y Direction of the Pattern</param>
    /// <param name="spacingBoltsX"> Spacing between holes in the X Direction</param>
    /// <param name="spacingBoltsY"> Spacing between holes in the Y Direction</param>
    /// <param name="diameterHole"> Hole Diameter</param>
    /// <param name="blindHoleDepth"> Depth of Blind Hole</param>
    /// <param name="additionalHoleParameters"> Optional Input Hole Build Properties </param>
    /// <returns></returns>
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

    /// <summary>
    /// Create Circular Blind Hole Pattern by Coordinate System
    /// </summary>
    /// <param name="element"> Steel element to recieve Holes</param>
    /// <param name="coordinateSystem"> Dynamo Coordinate System</param>
    /// <param name="noOfHolesX"> Number of holes in the Circular Pattern</param>
    /// <param name="radius"> Circular Hole Patern Radius</param>
    /// <param name="diameterHole"> Hole Diameter</param>
    /// <param name="blindHoleDepth"> Depth of Blind Hole</param>
    /// <param name="additionalHoleParameters"> Optional Input Hole Build Properties </param>
    /// <returns></returns>
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
    /// <summary>
    /// Create Rectangular Threaded Hole by Coordinate System
    /// </summary>
    /// <param name="element"> Steel element to recieve Holes</param>
    /// <param name="coordinateSystem"> Dynamo Coordinate System</param>
    /// <param name="noOfHolesX"> Number of holes in the X Direction of the Pattern</param>
    /// <param name="noOfHolesY"> Number of holes in the Y Direction of the Pattern</param>
    /// <param name="spacingBoltsX"> Spacing between holes in the X Direction</param>
    /// <param name="spacingBoltsY"> Spacing between holes in the Y Direction</param>
    /// <param name="diameterHole"> Hole Diameter</param>
    /// <param name="threadTappingRight"> Thread Holes to the right 0 - Left, 1 - Right</param>
    /// <param name="tappingHoleSize"> Thread Tapping Hole Size</param>
    /// <param name="threadHoleDepth"> Thread Hole Diameter</param>
    /// <param name="threadTaper"> Thread Hole Taper</param>
    /// <param name="additionalHoleParameters"> Optional Input Hole Build Properties </param>
    /// <returns></returns>
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

    /// <summary>
    /// Create Circular Threaded Hole by Coordinate System
    /// </summary>
    /// <param name="element"> Steel element to recieve Holes</param>
    /// <param name="coordinateSystem"> Dynamo Coordinate System</param>
    /// <param name="noOfHolesX"> Number of holes in the Circular Pattern</param>
    /// <param name="radius"> Circular Hole Patern Radius</param>
    /// <param name="diameterHole"> Hole Diameter</param>
    /// <param name="threadTappingRight"> Thread Holes to the right 0 - Left, 1 - Right</param>
    /// <param name="tappingHoleSize"> Thread Tapping Hole Size</param>
    /// <param name="threadHoleDepth"> Thread Hole Diameter</param>
    /// <param name="threadTaper"> Thread Hole Taper</param>
    /// <param name="additionalHoleParameters"> Optional Input Hole Build Properties </param>
    /// <returns></returns>
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
    /// <summary>
    /// Create Rectangular Sunk Hole by Coordinate System
    /// </summary>
    /// <param name="element"> Steel element to recieve Holes</param>
    /// <param name="coordinateSystem"> Dynamo Coordinate System</param>
    /// <param name="noOfHolesX"> Number of holes in the X Direction of the Pattern</param>
    /// <param name="noOfHolesY"> Number of holes in the Y Direction of the Pattern</param>
    /// <param name="spacingBoltsX"> Spacing between holes in the X Direction</param>
    /// <param name="spacingBoltsY"> Spacing between holes in the Y Direction</param>
    /// <param name="diameterHole"> Hole Diameter</param>
    /// <param name="sunkDiameter"> Sunk Hole Diameter</param>
    /// <param name="holeDepth"> Hole Depth</param>
    /// <param name="holeTaper"> Hole Depth Taper</param>
    /// <param name="additionalHoleParameters"> Optional Input Hole Build Properties </param>
    /// <returns></returns>
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

    /// <summary>
    /// Create Circular Sunk Hole by Coordinate System
    /// </summary>
    /// <param name="element"> Steel element to recieve Holes</param>
    /// <param name="coordinateSystem"> Dynamo Coordinate System</param>
    /// <param name="noOfHolesX"> Number of holes in the Circular Pattern</param>
    /// <param name="radius"> Circular Hole Patern Radius</param>
    /// <param name="diameterHole"> Hole Diameter</param>
    /// <param name="sunkDiameter"> Sunk Hole Diameter</param>
    /// <param name="holeDepth"> Hole Depth</param>
    /// <param name="holeTaper"> Hole Depth Taper</param>
    /// <param name="additionalHoleParameters"> Optional Input Hole Build Properties </param>
    /// <returns></returns>
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
    /// <summary>
    /// Create Rectangular Punch Mark Pattern by Coordinate System
    /// </summary>
    /// <param name="element"> Steel element to recieve Holes</param>
    /// <param name="coordinateSystem"> Dynamo Coordinate System</param>
    /// <param name="noOfHolesX"> Number of holes in the X Direction of the Pattern</param>
    /// <param name="noOfHolesY"> Number of holes in the Y Direction of the Pattern</param>
    /// <param name="spacingBoltsX"> Spacing between holes in the X Direction</param>
    /// <param name="spacingBoltsY"> Spacing between holes in the Y Direction</param>
    /// <param name="additionalHoleParameters"> Optional Input Hole Build Properties </param>
    /// <returns></returns>
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

    /// <summary>
    /// Create Circular Punch Mark Pattern by Coordinate System
    /// </summary>
    /// <param name="element"> Steel element to recieve Holes</param>
    /// <param name="coordinateSystem"> Dynamo Coordinate System</param>
    /// <param name="noOfHolesX"> Number of holes in the Circular Pattern</param>
    /// <param name="radius"> Circular Hole Patern Radius</param>
    /// <param name="additionalHoleParameters"> Optional Input Hole Build Properties </param>
    /// <returns></returns>
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

    #region "Set and Get Functions"

    /// <summary>
    /// List all holes found in a plate
    /// </summary>
    /// <param name="plateObject">  Selected Advance Steel Plate Object</param>
    /// <returns> List of plate hole objects in Plate object</returns>
    public static List<SteelDbObject> GetPlateHoles(SteelDbObject plateObject)
    {
      List<SteelDbObject> foundHandlesOfHolesInPlate = new List<SteelDbObject>();
      using (var ctx = new SteelServices.DocContext())
      {
        string plateHandle = plateObject.Handle;
        FilerObject plateObj = Utils.GetObject(plateHandle);

        if (plateObj != null && plateObj.IsKindOf(FilerObject.eObjectType.kPlateBase))
        {
          //Platebase to make sure we can support folded plates in the future
          Autodesk.AdvanceSteel.Modelling.PlateBase plate = Utils.GetObject(plateHandle) as Autodesk.AdvanceSteel.Modelling.PlateBase;
          List<Autodesk.AdvanceSteel.CADLink.Database.ObjectId> idsOfFeatures = plate.GetFeatures(false).ToList();
          for (int i = 0; i < idsOfFeatures.Count; i++)
          {
            try
            {
              FilerObject fea = DatabaseManager.Open(idsOfFeatures[i]);
              if (fea.IsKindOf(FilerObject.eObjectType.kConnectionHolePlate))
              {
                SteelDbObject foundSteelObj = fea.ToDSType();
                foundHandlesOfHolesInPlate.Add(foundSteelObj);
              }
            }
            catch (Exception)
            {
              throw;
            }
          }
        }
        else
          throw new System.Exception("Failed to Get Plate Object");
      }
      return foundHandlesOfHolesInPlate;
    }

    /// <summary>
    /// Get Plate Hole Arranger Spacing Values
    /// </summary>
    /// <param name="plateObject"> Selected Advance Steel Plate Holes Object</param>
    /// <param name="holeObject">  Selected Advance Steel Plate Hole Object</param>
    /// <returns name="Hole_Spacing_X"> Hole spacing and edge Distance values from Arranger</returns>
    [MultiReturn(new[] { "Hole_Spacing_X", "Hole_Spacing_Y", "Hole_Spacing_Radius", "Hole_EdgeDistance_X", "Hole_EdgeDistance_Y" })]
    public static Dictionary<string, double> GetPlateHoleArrangerSpacingValues(SteelDbObject plateObject, SteelDbObject holeObject)
    {
      Dictionary<string, double> ret = new Dictionary<string, double>();

      double dX = 0, dY = 0, radius = 0, wX = 0, wY = 0;

      ret.Add("Hole_Spacing_X", dX);
      ret.Add("Hole_Spacing_Y", dY);
      ret.Add("Hole_Spacing_Radius", radius);
      ret.Add("Hole_EdgeDistance_X", wX);
      ret.Add("Hole_EdgeDistance_Y", wY);

      using (var ctx = new SteelServices.DocContext())
      {
        string plateHandle = plateObject.Handle;
        string plateHoleHandle = holeObject.Handle;
        FilerObject plateObj = Utils.GetObject(plateHandle);

        if (plateObj != null && plateObj.IsKindOf(FilerObject.eObjectType.kPlateBase))
        {
          Autodesk.AdvanceSteel.Modelling.ConnectionHolePlate holes = Utils.GetObject(plateHoleHandle) as Autodesk.AdvanceSteel.Modelling.ConnectionHolePlate;
          if (holes != null && holes.IsKindOf(FilerObject.eObjectType.kConnectionHolePlate))
          {
            ret["Hole_Spacing_X"] = Utils.FromInternalDistanceUnits(holes.Arranger.Dx, true);
            ret["Hole_Spacing_Y"] = Utils.FromInternalDistanceUnits(holes.Arranger.Dy, true);
            ret["Hole_Spacing_Radius"] = Utils.FromInternalDistanceUnits(holes.Arranger.Radius, true);
            ret["Hole_EdgeDistance_X"] = Utils.FromInternalDistanceUnits(holes.Arranger.Wx, true);
            ret["Hole_EdgeDistance_Y"] = Utils.FromInternalDistanceUnits(holes.Arranger.Wy, true);
          }
          else
            throw new System.Exception("Failed to Get Plate Hole Object");
        }
        else
          throw new System.Exception("Failed to Get Plate Object");
      }
      return ret;
    }

    /// <summary>
    /// Get Plate Hole Arranger Number Off Values
    /// </summary>
    /// <param name="plateObject"> Selected Advance Steel Plate Holes Object</param>
    /// <param name="holeObject">  Selected Advance Steel Plate Hole Object</param>
    /// <returns name="Hole_Noff_X"> Number of holes in the X and Y direction of Plate Hole</returns>
    [MultiReturn(new[] { "Hole_Noff_X", "Hole_Noff_Y" })]
    public static Dictionary<string, int> GetPlateHoleArrangerNumberOffValues(SteelDbObject plateObject, SteelDbObject holeObject)
    {
      Dictionary<string, int> ret = new Dictionary<string, int>();

      int nX = 0, nY = 0;

      ret.Add("Hole_Noff_X", nX);
      ret.Add("Hole_Noff_Y", nY);

      using (var ctx = new SteelServices.DocContext())
      {
        string plateHandle = plateObject.Handle;
        string plateHoleHandle = holeObject.Handle;
        FilerObject plateObj = Utils.GetObject(plateHandle);

        if (plateObj != null && plateObj.IsKindOf(FilerObject.eObjectType.kPlateBase))
        {
          Autodesk.AdvanceSteel.Modelling.ConnectionHolePlate holes = Utils.GetObject(plateHoleHandle) as Autodesk.AdvanceSteel.Modelling.ConnectionHolePlate;
          if (holes != null && holes.IsKindOf(FilerObject.eObjectType.kConnectionHolePlate))
          {
            ret["Hole_Noff_X"] = holes.Arranger.Radius > 0 ? holes.Arranger.NumberOfElements : holes.Arranger.Nx;
            ret["Hole_Noff_Y"] = holes.Arranger.Ny;
          }
          else
            throw new System.Exception("Failed to Get Plate Hole Object");
        }
        else
          throw new System.Exception("Failed to Get Plate Object");
      }
      return ret;
    }

    /// <summary>
    /// Get Plate Hole Type
    /// </summary>
    /// <param name="plateObject"> Selected Advance Steel Plate Holes Object</param>
    /// <param name="holeObject">  Selected Advance Steel Plate Hole Object</param>
    /// <returns> Return Hole Type as integer for hole type</returns>
    public static int GetPlateHoleType(SteelDbObject plateObject, SteelDbObject holeObject)
    {
      int ret = 0;

      using (var ctx = new SteelServices.DocContext())
      {
        string plateHandle = plateObject.Handle;
        string plateHoleHandle = holeObject.Handle;
        FilerObject plateObj = Utils.GetObject(plateHandle);

        if (plateObj != null && plateObj.IsKindOf(FilerObject.eObjectType.kPlateBase))
        {
          Autodesk.AdvanceSteel.Modelling.ConnectionHolePlate holes = Utils.GetObject(plateHoleHandle) as Autodesk.AdvanceSteel.Modelling.ConnectionHolePlate;
          if (holes != null && holes.IsKindOf(FilerObject.eObjectType.kConnectionHolePlate))
          {
            ret = (int)holes.Hole.Type;
          }
          else
            throw new System.Exception("Failed to Get Plate Hole Object");
        }
        else
          throw new System.Exception("Failed to Get Plate Object");
      }
      return ret;
    }

    /// <summary>
    /// Get Plate Slotted Hole Direction
    /// </summary>
    /// <param name="plateObject"> Selected Advance Steel Plate Holes Object</param>
    /// <param name="holeObject">  Selected Advance Steel Plate Hole Object</param>
    /// <returns> Returns hole Slot Direction</returns>
    public static int GetPlateHoleSlotDirection(SteelDbObject plateObject, SteelDbObject holeObject)
    {
      int ret = 0;

      using (var ctx = new SteelServices.DocContext())
      {
        string plateHandle = plateObject.Handle;
        string plateHoleHandle = holeObject.Handle;
        FilerObject plateObj = Utils.GetObject(plateHandle);

        if (plateObj != null && plateObj.IsKindOf(FilerObject.eObjectType.kPlateBase))
        {
          Autodesk.AdvanceSteel.Modelling.ConnectionHolePlate holes = Utils.GetObject(plateHoleHandle) as Autodesk.AdvanceSteel.Modelling.ConnectionHolePlate;
          if (holes != null && holes.IsKindOf(FilerObject.eObjectType.kConnectionHolePlate))
          {
            if (holes.Hole.Type == Hole.eHoleType.kSlottedHole)
            {
              SlottedHole sh = (SlottedHole)holes.Hole;
              ret = (int)sh.SlotDirection;
            }
            else
              throw new System.Exception("Failed - not a slotted hole");
          }
          else
            throw new System.Exception("Failed to Get Plate Hole Object");
        }
        else
          throw new System.Exception("Failed to Get Plate Object");
      }
      return ret;
    }

    /// <summary>
    /// Get Plate Slotted Hole Length Value
    /// </summary>
    /// <param name="plateObject"> Selected Advance Steel Plate Holes Object</param>
    /// <param name="holeObject">  Selected Advance Steel Plate Hole Object</param>
    /// <returns> Returns slotted hole length</returns>
    public static double GetPlateHoleSlotLength(SteelDbObject plateObject, SteelDbObject holeObject)
    {
      double ret = 0;

      using (var ctx = new SteelServices.DocContext())
      {
        string plateHandle = plateObject.Handle;
        string plateHoleHandle = holeObject.Handle;
        FilerObject plateObj = Utils.GetObject(plateHandle);

        if (plateObj != null && plateObj.IsKindOf(FilerObject.eObjectType.kPlateBase))
        {
          Autodesk.AdvanceSteel.Modelling.ConnectionHolePlate holes = Utils.GetObject(plateHoleHandle) as Autodesk.AdvanceSteel.Modelling.ConnectionHolePlate;
          if (holes != null && holes.IsKindOf(FilerObject.eObjectType.kConnectionHolePlate))
          {
            if (holes.Hole.Type == Hole.eHoleType.kSlottedHole)
            {
              SlottedHole sh = (SlottedHole)holes.Hole;
              ret = sh.SlotLength;
            }
            else
              throw new System.Exception("Failed - not a slotted hole");
          }
          else
            throw new System.Exception("Failed to Get Plate Hole Object");
        }
        else
          throw new System.Exception("Failed to Get Plate Object");
      }
      return ret;
    }

    /// <summary>
    /// Get Plate Hole Center Point
    /// </summary>
    /// <param name="plateObject"> Selected Advance Steel Plate Holes Object</param>
    /// <param name="holeObject">  Selected Advance Steel Plate Hole Object</param>
    /// <returns> Returns Dynamo Point</returns>
    public static Autodesk.DesignScript.Geometry.Point GetPlateHoleCenterPoint(SteelDbObject plateObject, SteelDbObject holeObject)
    {
      Autodesk.DesignScript.Geometry.Point ret = null;

      using (var ctx = new SteelServices.DocContext())
      {
        string plateHandle = plateObject.Handle;
        string plateHoleHandle = holeObject.Handle;
        FilerObject plateObj = Utils.GetObject(plateHandle);

        if (plateObj != null && plateObj.IsKindOf(FilerObject.eObjectType.kPlateBase))
        {
          Autodesk.AdvanceSteel.Modelling.ConnectionHolePlate holes = Utils.GetObject(plateHoleHandle) as Autodesk.AdvanceSteel.Modelling.ConnectionHolePlate;
          if (holes != null && holes.IsKindOf(FilerObject.eObjectType.kConnectionHolePlate))
          {
            ret = Utils.ToDynPoint(holes.GetCenterPoint(), true); //GetPatternCenter
          }
          else
            throw new System.Exception("Failed to Get Plate Hole Object");
        }
        else
          throw new System.Exception("Failed to Get Plate Object");
      }
      return ret;
    }

    /// <summary>
    /// Get Plate Hole Pattern Center Point
    /// </summary>
    /// <param name="plateObject"> Selected Advance Steel Plate Holes Object</param>
    /// <param name="holeObject">  Selected Advance Steel Plate Hole Object</param>
    /// <returns> Returns Dynamo Point</returns>
    public static Autodesk.DesignScript.Geometry.Point GetPlateHolePatternCenterPoint(SteelDbObject plateObject, SteelDbObject holeObject)
    {
      Autodesk.DesignScript.Geometry.Point ret = null;

      using (var ctx = new SteelServices.DocContext())
      {
        string plateHandle = plateObject.Handle;
        string plateHoleHandle = holeObject.Handle;
        FilerObject plateObj = Utils.GetObject(plateHandle);

        if (plateObj != null && plateObj.IsKindOf(FilerObject.eObjectType.kPlateBase))
        {
          Autodesk.AdvanceSteel.Modelling.ConnectionHolePlate holes = Utils.GetObject(plateHoleHandle) as Autodesk.AdvanceSteel.Modelling.ConnectionHolePlate;
          if (holes != null && holes.IsKindOf(FilerObject.eObjectType.kConnectionHolePlate))
          {
            ret = Utils.ToDynPoint(holes.GetPatternCenter(true), true);
          }
          else
            throw new System.Exception("Failed to Get Plate Hole Object");
        }
        else
          throw new System.Exception("Failed to Get Plate Object");
      }
      return ret;
    }

    /// <summary>
    /// Get Plate Hole Points or each hole
    /// </summary>
    /// <param name="plateObject"> Selected Advance Steel Plate Holes Object</param>
    /// <param name="holeObject">  Selected Advance Steel Plate Hole Object</param>
    /// <returns> Returns List of Dynamo Point</returns>
    public static List<Autodesk.DesignScript.Geometry.Point> GetPlateHoleCenterPoints(SteelDbObject plateObject, SteelDbObject holeObject)
    {
      List<Autodesk.DesignScript.Geometry.Point> ret = new List<DynGeometry.Point>();

      using (var ctx = new SteelServices.DocContext())
      {
        string plateHandle = plateObject.Handle;
        string plateHoleHandle = holeObject.Handle;
        FilerObject plateObj = Utils.GetObject(plateHandle);

        if (plateObj != null && plateObj.IsKindOf(FilerObject.eObjectType.kPlateBase))
        {
          Autodesk.AdvanceSteel.Modelling.ConnectionHolePlate holes = Utils.GetObject(plateHoleHandle) as Autodesk.AdvanceSteel.Modelling.ConnectionHolePlate;
          if (holes != null && holes.IsKindOf(FilerObject.eObjectType.kConnectionHolePlate))
          {
            SteelGeometry.Point3d[] defPoints = null;
            holes.GetElementOrigins(out defPoints);
            for (int i = 0; i < defPoints.Count(); i++)
            {
              ret.Add(Utils.ToDynPoint(defPoints[i], true));
            }
          }
          else
            throw new System.Exception("Failed to Get Plate Hole Object");
        }
        else
          throw new System.Exception("Failed to Get Plate Object");
      }
      return ret;
    }

    /// <summary>
    /// Set the X and Y spacing of holes on Plate Hole Object
    /// </summary>
    /// <param name="plateObject"> Select Advance Steel Plate Hole Object</param>
    /// <param name="holeObject">  Selected Advance Steel Plate Hole Object</param>
    /// <param name="spacingHoleX"> X spacing of Hole Arranger</param>
    /// <param name="spacingHoleY"> Y spacing of Hole Arranger</param>
    public static void SetHoleSpacing(SteelDbObject plateObject, SteelDbObject holeObject,
                                  double spacingHoleX,
                                  double spacingHoleY)
    {
      using (var ctx = new SteelServices.DocContext())
      {
        string plateHandle = plateObject.Handle;
        string plateHoleHandle = holeObject.Handle;
        FilerObject plateObj = Utils.GetObject(plateHandle);

        if (plateObj != null && plateObj.IsKindOf(FilerObject.eObjectType.kPlateBase))
        {
          Autodesk.AdvanceSteel.Modelling.ConnectionHolePlate holes = Utils.GetObject(plateHoleHandle) as Autodesk.AdvanceSteel.Modelling.ConnectionHolePlate;
          if (holes != null && holes.IsKindOf(FilerObject.eObjectType.kConnectionHolePlate))
          {
            holes.Arranger.Dx = spacingHoleX;
            holes.Arranger.Dy = spacingHoleY;
          }
          else
            throw new System.Exception("Failed to Get Plate Hole Object");
        }
        else
          throw new System.Exception("Failed to Get Plate Object");
      }
    }

    /// <summary>
    /// Sets the number of holes in the X and Y values of the Arranger
    /// </summary>
    /// <param name="plateObject"> Select Advance Steel Plate Hole Object</param>
    /// <param name="holeObject">  Selected Advance Steel Plate Hole Object</param>
    /// <param name="noOfHolesX"></param>
    /// <param name="noOfHolesY"></param>
    public static void SetHoleArrangementCount(SteelDbObject plateObject, SteelDbObject holeObject,
                                                [DefaultArgument("2;")] int noOfHolesX,
                                                [DefaultArgument("2;")] int noOfHolesY)
    {
      using (var ctx = new SteelServices.DocContext())
      {
        string plateHandle = plateObject.Handle;
        string plateHoleHandle = holeObject.Handle;
        FilerObject plateObj = Utils.GetObject(plateHandle);

        if (plateObj != null && plateObj.IsKindOf(FilerObject.eObjectType.kPlateBase))
        {
          Autodesk.AdvanceSteel.Modelling.ConnectionHolePlate holes = Utils.GetObject(plateHoleHandle) as Autodesk.AdvanceSteel.Modelling.ConnectionHolePlate;
          if (holes != null && holes.IsKindOf(FilerObject.eObjectType.kConnectionHolePlate))
          {
            holes.Arranger.Nx = noOfHolesX;
            holes.Arranger.Ny = noOfHolesY;
          }
          else
            throw new System.Exception("Failed to Get Plate Hole Object");
        }
        else
          throw new System.Exception("Failed to Get Plate Object");
      }
    }

    /// <summary>
    /// Sets the radius value in the arranger of the Plate Hole Object
    /// </summary>
    /// <param name="plateObject"> Select Advance Steel Plate Hole Object</param>
    /// <param name="holeObject">  Selected Advance Steel Plate Hole Object</param>
    /// <param name="radius"> New Radius Value</param>
    public static void SetHoleArrangementRadius(SteelDbObject plateObject,
                                                SteelDbObject holeObject,
                                                double radius)
    {
      using (var ctx = new SteelServices.DocContext())
      {
        string plateHandle = plateObject.Handle;
        string plateHoleHandle = holeObject.Handle;
        FilerObject plateObj = Utils.GetObject(plateHandle);

        if (plateObj != null && plateObj.IsKindOf(FilerObject.eObjectType.kPlateBase))
        {
          Autodesk.AdvanceSteel.Modelling.ConnectionHolePlate holes = Utils.GetObject(plateHoleHandle) as Autodesk.AdvanceSteel.Modelling.ConnectionHolePlate;
          if (holes != null && holes.IsKindOf(FilerObject.eObjectType.kConnectionHolePlate))
          {
            holes.Arranger.Radius = radius;
          }
          else
            throw new System.Exception("Failed to Get Plate Hole Object");
        }
        else
          throw new System.Exception("Failed to Get Plate Object");
      }
    }

    /// <summary>
    /// Sets the hole diameter of the Plate Hole Object
    /// </summary>
    /// <param name="plateObject"> Select Advance Steel Plate Hole Object</param>
    /// <param name="holeObject">  Selected Advance Steel Plate Hole Object</param>
    /// <param name="holeDiameter"> New Hole Diameter</param>
    public static void SetHoleArrangementDiameter(SteelDbObject plateObject,
                                                SteelDbObject holeObject,
                                                double holeDiameter)
    {
      using (var ctx = new SteelServices.DocContext())
      {
        string plateHandle = plateObject.Handle;
        string plateHoleHandle = holeObject.Handle;
        FilerObject plateObj = Utils.GetObject(plateHandle);

        if (plateObj != null && plateObj.IsKindOf(FilerObject.eObjectType.kPlateBase))
        {
          Autodesk.AdvanceSteel.Modelling.ConnectionHolePlate holes = Utils.GetObject(plateHoleHandle) as Autodesk.AdvanceSteel.Modelling.ConnectionHolePlate;
          if (holes != null && holes.IsKindOf(FilerObject.eObjectType.kConnectionHolePlate))
          {
            holes.Hole.Diameter = Utils.ToInternalDistanceUnits(holeDiameter, true);
          }
          else
            throw new System.Exception("Failed to Get Plate Hole Object");
        }
        else
          throw new System.Exception("Failed to Get Plate Object");
      }
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

      Utils.CheckListUpdateOrAddValue(listOfHoleParameters, "Nx", nx);
      Utils.CheckListUpdateOrAddValue(listOfHoleParameters, "Ny", ny);
      Utils.CheckListUpdateOrAddValue(listOfHoleParameters, "Dx", dx);
      Utils.CheckListUpdateOrAddValue(listOfHoleParameters, "Dy", dy);
      Utils.CheckListUpdateOrAddValue(listOfHoleParameters, "Diameter", diameterHole);
      Utils.CheckListUpdateOrAddValue(listOfHoleParameters, "SlotLength", slotLength);
      Utils.CheckListUpdateOrAddValue(listOfHoleParameters, "SlotDirection", slotDirection);
      Utils.CheckListUpdateOrAddValue(listOfHoleParameters, "SunkDepth", sunkDepth);
      Utils.CheckListUpdateOrAddValue(listOfHoleParameters, "AlphaE", alphaE);
      Utils.CheckListUpdateOrAddValue(listOfHoleParameters, "IsTappingRight", isTappingRight);
      Utils.CheckListUpdateOrAddValue(listOfHoleParameters, "HeadDepth", headDepth);

      Utils.CheckListUpdateOrAddValue(listOfHoleParameters, "Radius", radius);
      Utils.CheckListUpdateOrAddValue(listOfHoleParameters, "Length", length);
      Utils.CheckListUpdateOrAddValue(listOfHoleParameters, "Width", width);
      Utils.CheckListUpdateOrAddValue(listOfHoleParameters, "Wx", wx);
      Utils.CheckListUpdateOrAddValue(listOfHoleParameters, "Wy", wy);


      return listOfHoleParameters;
    }

    [IsVisibleInDynamoLibrary(false)]
    public override IEnumerable<Autodesk.DesignScript.Interfaces.IGraphicItem> GetDynGeometry()
    {
      IList<Autodesk.DesignScript.Interfaces.IGraphicItem> ret = new List<Autodesk.DesignScript.Interfaces.IGraphicItem>();

      var boltPattern = Utils.GetObject(Handle) as ASConnectionHolePlate;

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
      return ret;
    }

  }
}