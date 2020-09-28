using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.DocumentManagement;
using Autodesk.DesignScript.Runtime;
using Dynamo.Applications.AdvanceSteel.Services;
using System;
using Autodesk.AdvanceSteel.Geometry;

namespace AdvanceSteel.Nodes
{
  [IsVisibleInDynamoLibrary(false)]
  public class PropertiesAnchorBolts
  {

    public PropertiesAnchorBolts()
    {
    }

    public PropertiesAnchorBolts(string standard,
                            string boltAssembly,
                            string grade,
                            double diameter, double holeTolerance,
                            int xCount, int yCount,
                            double xSpacing, double ySpacing,
                            double radius, double AnchorBoltlength,
                            int anchorBoltConnectionType, int anchorBoltOrientation,
                            bool anchorBoltInverted)
    {
      Standard = standard;
      AnchorBoltAssembly = boltAssembly;
      Grade = grade;
      Diameter = diameter;
      HoleTolerance = holeTolerance;
      XCount = xCount;
      YCount = yCount;
      XSpacing = xSpacing;
      YSpacing = ySpacing;
      Radius = radius;
      AnchorBoltLength = AnchorBoltlength;
      AnchorBoltConnectionType = anchorBoltConnectionType;
      AnchorBoltInverted = anchorBoltInverted;   
      AnchorBoltOrientationType = anchorBoltOrientation;
    }

    public PropertiesAnchorBolts(PropertiesAnchorBolts derivedAnchorBoltProperties)
    {
      Standard = derivedAnchorBoltProperties.Standard;
      AnchorBoltAssembly = derivedAnchorBoltProperties.AnchorBoltAssembly;
      Grade = derivedAnchorBoltProperties.Grade;
      Diameter = derivedAnchorBoltProperties.Diameter;
      HoleTolerance = derivedAnchorBoltProperties.HoleTolerance;
      XCount = derivedAnchorBoltProperties.XCount;
      YCount = derivedAnchorBoltProperties.YCount;
      XSpacing = derivedAnchorBoltProperties.XSpacing;
      YSpacing = derivedAnchorBoltProperties.YSpacing;
      Radius = derivedAnchorBoltProperties.Radius;
      AnchorBoltLength = derivedAnchorBoltProperties.AnchorBoltLength;
      AnchorBoltConnectionType = derivedAnchorBoltProperties.AnchorBoltConnectionType;
      AnchorBoltInverted = derivedAnchorBoltProperties.AnchorBoltInverted;
      AnchorBoltOrientationType = derivedAnchorBoltProperties.AnchorBoltOrientationType;
    }

    private string _standard = "";
    private string _anchorBoltAssembly = "";
    private string _grade = "";
    private double _diameter = 0;
    private double _holeTolerance = -1;

    private int _yCount = -1;
    private int _xCount = -1;
    private double _xSpacing = 0;
    private double _ySpacing = 0;
    private double _radius = 0;
    private double _anchorBoltLength = 0;
    private int _anchorBoltOrientation = 0;
    private int _anchorBoltConnType = -1;

    private bool _inverted = false;

    private bool foundData { get; set; }

    public string Standard 
    {
      get { return _standard; }
      set
      {
        _standard = value;
        if (!foundData)
        {
          foundData = !string.IsNullOrEmpty(value);
        }
      }
    }

    public string AnchorBoltAssembly
    {
      get { return _anchorBoltAssembly; }
      set
      {
        _anchorBoltAssembly = value;
        if (!foundData)
        {
          foundData = !string.IsNullOrEmpty(value);
        }
      }
    }

    public string Grade
    {
      get { return _grade; }
      set
      {
        _grade = value;
        if (!foundData)
        {
          foundData = !string.IsNullOrEmpty(value);
        }
      }
    }

    public double Diameter
    {
      get { return _diameter; }
      set
      {
        _diameter = value;
        if (!foundData)
        {
          foundData = (value > 0);
        }
      }
    }

    public double HoleTolerance
    {
      get { return _holeTolerance; }
      set
      {
        _holeTolerance = value;
        if (!foundData)
        {
          foundData = (value > 0);
        }
      }
    }

    public double XSpacing
    {
      get { return _xSpacing; }
      set
      {
        _xSpacing = value;
        if (!foundData)
        {
          foundData = (value > 0);
        }
      }
    }

    public double YSpacing
    {
      get { return _ySpacing; }
      set
      {
        _ySpacing = value;
        if (!foundData)
        {
          foundData = (value > 0);
        }
      }
    }

    public int XCount
    {
      get { return _xCount; }
      set
      {
        _xCount = value;
        if (!foundData)
        {
          foundData = (value > -1);
        }
      }
    }

    public int YCount
    {
      get { return _yCount; }
      set
      {
        _yCount = value;
        if (!foundData)
        {
          foundData = (value > -1);
        }
      }
    }

    public double Radius
    {
      get { return _radius; }
      set
      {
        _radius = value;
        if (!foundData)
        {
          foundData = (value > 0);
        }
      }
    }

    public double AnchorBoltLength
    {
      get { return _anchorBoltLength; }
      set
      {
        _anchorBoltLength = value;
      }
    }

    public int AnchorBoltConnectionType
    {
      get { return _anchorBoltConnType; }
      set
      {
        _anchorBoltConnType = value;
        if (!foundData)
        {
          foundData = (value > -1);
        }
      }
    }

    public int AnchorBoltOrientationType
    {
      get { return _anchorBoltOrientation; }
      set
      {
        _anchorBoltOrientation = value;
        if (foundData)
        {
          foundData = (value > -1);
        }
      }
    }

    public bool AnchorBoltInverted
    {
      get { return _inverted; }
      set
      {
        _inverted = value;
      }
    }

    public bool HasProperties()
    {
      return foundData;
    }

  }
}