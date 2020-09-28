using AdvanceSteel.Nodes.Plates;
using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.DesignScript.Runtime;
using DynGeometry = Autodesk.DesignScript.Geometry;
using SteelGeometry = Autodesk.AdvanceSteel.Geometry;
using Autodesk.AdvanceSteel.DotNetRoots.DatabaseAccess;
using SteelServices = Dynamo.Applications.AdvanceSteel.Services;
using Autodesk.AdvanceSteel.ConstructionTypes;
using System.Collections.Generic;
using Autodesk.AdvanceSteel.Geometry;
using System.Linq;
using System;


namespace AdvanceSteel.Nodes.Util
{
	/// <summary>
	/// Store Bolts properties in a Node to pass to Bolt Node
	/// </summary>
	public class BuildPropertyCollection
	{
		internal BuildPropertyCollection()
		{
		}

    /// <summary>
    /// Store Advance Steel Bolt Properties
    /// </summary>
    /// <param name="existingBoltProperties"></param>
    /// <param name="standard"></param>
    /// <param name="boltAssembly"></param>
    /// <param name="grade"> Input Hole Material Grade</param>
    /// <param name="diameter"> Input Hole Diameter</param>
    /// <param name="holeTolerance"> Input Hole Tolereance</param>
    /// <param name="xHoleCount"> Input Hole Count in the X direction or number of holes in a circular pattern</param>
    /// <param name="yHoleCount"> Input Hole Count in the Y direction </param>
    /// <param name="xHoleSpacing"> Input holes spacing in the X Direction </param>
    /// <param name="yHoleSpacing"> Input holes spacing in the Y Direction </param>
    /// <param name="radius"> Input Radius for Circular Bolt Pattern</param>
    /// <param name="lengthAddition"> Input grip length addition to increase the length of the bolt</param>
    /// <param name="boltType"> Input Bolt Connection type - Shop Bolt Default</param>
    /// <param name="boltInverted"> Input True or False to flip bolt orientation</param>
    /// <returns></returns>
    public static PropertiesBolts Bolts([DefaultArgument("null;")]PropertiesBolts existingBoltProperties, 
                                          [DefaultArgument("AS 1111;")]string standard, 
                                          [DefaultArgument("MuS;")]string boltAssembly,
                                          [DefaultArgument("4.6;")]string grade,
                                          [DefaultArgument("0;")]double diameter, [DefaultArgument("-1;")]double holeTolerance,
                                          [DefaultArgument("-1;")]int xHoleCount, [DefaultArgument("-1;")]int yHoleCount,
                                          [DefaultArgument("0;")]double xHoleSpacing, [DefaultArgument("0;")]double yHoleSpacing,
                                          [DefaultArgument("0;")]double radius, [DefaultArgument("0;")]double lengthAddition,
                                          [DefaultArgument("2;")]int boltType, [DefaultArgument("false;")]bool boltInverted)

		{
      PropertiesBolts retValue = null;
      if (existingBoltProperties == null)
      {
        retValue = new PropertiesBolts(standard, boltAssembly, grade, diameter, holeTolerance, xHoleCount, yHoleCount, xHoleSpacing, yHoleSpacing, radius, lengthAddition, boltType, boltInverted); 
      }
      else
      {
        retValue = new PropertiesBolts(existingBoltProperties); 
        if (!string.IsNullOrEmpty(standard)) { retValue.Standard = standard; }
        if (!string.IsNullOrEmpty(boltAssembly)) { retValue.BoltAssembly = boltAssembly; }
        if (!string.IsNullOrEmpty(grade)) { retValue.Grade = grade; }
        if (diameter > 0) { retValue.Diameter = diameter; }
        if (holeTolerance > -1) { retValue.HoleTolerance = holeTolerance; }
        if (xHoleCount > -1) { retValue.XCount = xHoleCount; }
        if (yHoleCount > -1) { retValue.YCount = yHoleCount; }
        if (xHoleSpacing > 0) { retValue.XSpacing = xHoleSpacing; }
        if (yHoleSpacing > 0) { retValue.YSpacing = yHoleSpacing; }
        if (radius > 0) { retValue.Radius = radius; }
        retValue.LengthAddition = lengthAddition;
        if (boltType > -1) { retValue.BoltConnectionType = boltType; }
        retValue.BoltInverted = boltInverted;
      }
      if (!retValue.HasProperties())
      {
        throw new System.Exception("No Properties in Property Block");
      }
      return retValue;
		}

    /// <summary>
    /// Store Advance Steel Anchor Bolt Properties
    /// </summary>
    /// <param name="existingAnchorBoltProperties"></param>
    /// <param name="standard"></param>
    /// <param name="boltAssembly"></param>
    /// <param name="grade"> Input Hole Material Grade</param>
    /// <param name="anchorLength"> Input Anchor Bolt Length</param>
    /// <param name="diameter"> Input Hole Diameter</param>
    /// <param name="holeTolerance"> Input Hole Tolereance</param>
    /// <param name="xHoleCount"> Input Hole Count in the X direction or number of holes in a circular pattern</param>
    /// <param name="yHoleCount"> Input Hole Count in the Y direction </param>
    /// <param name="xHoleSpacing"> Input holes spacing in the X Direction </param>
    /// <param name="yHoleSpacing"> Input holes spacing in the Y Direction </param>
    /// <param name="radius"> Input Radius for Circular Bolt Pattern</param>
    /// <param name="anchorBoltConType"> Input Anchor Bolt Connection type - Shop Bolt Default</param>
    /// <param name="anchorBoltOrientation"> Input Bolt Connection type - Shop Bolt Default</param>
    /// <param name="anchorBoltInverted"> Input True or False to flip anchor bolt orientation</param>
    /// <returns></returns>
    public static PropertiesAnchorBolts AnchorBolts([DefaultArgument("null;")]PropertiesAnchorBolts existingAnchorBoltProperties,
                                      [DefaultArgument("HOLDING DOWN BOLTS;")]string standard,
                                      [DefaultArgument("MuS;")]string boltAssembly,
                                      [DefaultArgument("4.6;")]string grade, [DefaultArgument("0;")]double anchorLength,
                                      [DefaultArgument("0;")]double diameter, [DefaultArgument("- 1;")]double holeTolerance,
                                      [DefaultArgument("-1;")]int xHoleCount, [DefaultArgument("-1;")]int yHoleCount,
                                      [DefaultArgument("0;")]double xHoleSpacing, [DefaultArgument("0;")]double yHoleSpacing,
                                      [DefaultArgument("0;")]double radius, 
                                      [DefaultArgument("2;")]int anchorBoltConType, [DefaultArgument("0;")]int anchorBoltOrientation,
                                      [DefaultArgument("false;")]bool anchorBoltInverted)

    {
      PropertiesAnchorBolts retValue = null;
      if (existingAnchorBoltProperties == null)
      {
        retValue = new PropertiesAnchorBolts(standard, boltAssembly, grade, diameter, holeTolerance, xHoleCount, yHoleCount, xHoleSpacing, yHoleSpacing, radius, anchorLength, anchorBoltConType, anchorBoltOrientation, anchorBoltInverted);
      }
      else
      {
        retValue = new PropertiesAnchorBolts(existingAnchorBoltProperties);
        if (!string.IsNullOrEmpty(standard)) { retValue.Standard = standard; }
        if (!string.IsNullOrEmpty(boltAssembly)) { retValue.AnchorBoltAssembly = boltAssembly; }
        if (!string.IsNullOrEmpty(grade)) { retValue.Grade = grade; }
        if (diameter > 0) { retValue.Diameter = diameter; }
        if (holeTolerance > -1) { retValue.HoleTolerance = holeTolerance; }
        if (xHoleCount > -1) { retValue.XCount = xHoleCount; }
        if (yHoleCount > -1) { retValue.YCount = yHoleCount; }
        if (xHoleSpacing > 0) { retValue.XSpacing = xHoleSpacing; }
        if (yHoleSpacing > 0) { retValue.YSpacing = yHoleSpacing; }
        if (radius > 0) { retValue.Radius = radius; }
        retValue.AnchorBoltLength = anchorLength;
        if (anchorBoltConType > -1) { retValue.AnchorBoltConnectionType = anchorBoltConType; }
        if (anchorBoltOrientation > -1) { retValue.AnchorBoltOrientationType = anchorBoltOrientation; }
        retValue.AnchorBoltInverted = anchorBoltInverted;
      }
      if (!retValue.HasProperties())
      {
        throw new System.Exception("No Properties in Property Block");
      }
      return retValue;
    }

    public static PropertiesShearStuds ShearStuds([DefaultArgument("null;")]PropertiesShearStuds existingShearStudProperties,
                                      [DefaultArgument("Nelson S3L;")]string standard,
                                      [DefaultArgument("Mild Steel;")]string grade, 
                                      [DefaultArgument("22;")]double diameter, [DefaultArgument("100;")]double length,
                                      [DefaultArgument("-1;")]int xHoleCount, [DefaultArgument("-1;")]int yHoleCount,
                                      [DefaultArgument("0;")]double xHoleSpacing, [DefaultArgument("0;")]double yHoleSpacing,
                                      [DefaultArgument("0;")]double radius,
                                      [DefaultArgument("2;")]int shearStudConType, [DefaultArgument("false;")]bool displayAsSolid)

    {
      PropertiesShearStuds retValue = null;
      if (existingShearStudProperties == null)
      {
        retValue = new PropertiesShearStuds(standard, grade, diameter, length, xHoleCount, yHoleCount, xHoleSpacing, yHoleSpacing, radius, shearStudConType, displayAsSolid);
      }
      else
      {
        retValue = new PropertiesShearStuds(existingShearStudProperties);
        if (!string.IsNullOrEmpty(standard)) { retValue.Standard = standard; }
        if (!string.IsNullOrEmpty(grade)) { retValue.Grade = grade; }
        if (diameter > 0) { retValue.Diameter = diameter; }
        if (length > -1) { retValue.Length = length; }
        if (xHoleCount > -1) { retValue.XCount = xHoleCount; }
        if (yHoleCount > -1) { retValue.YCount = yHoleCount; }
        if (xHoleSpacing > 0) { retValue.XSpacing = xHoleSpacing; }
        if (yHoleSpacing > 0) { retValue.YSpacing = yHoleSpacing; }
        if (radius > 0) { retValue.Radius = radius; }
        if (shearStudConType > -1) { retValue.ShearStudConnectionType = shearStudConType; }
        retValue.DisplayAsSolid = displayAsSolid;
      }
      if (!retValue.HasProperties())
      {
        throw new System.Exception("No Properties in Property Block");
      }
      return retValue;
    }
  }
}
