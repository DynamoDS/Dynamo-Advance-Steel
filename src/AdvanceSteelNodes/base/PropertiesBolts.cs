using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.DocumentManagement;
using Autodesk.DesignScript.Runtime;
using Dynamo.Applications.AdvanceSteel.Services;
using System;
using Autodesk.AdvanceSteel.Geometry;

namespace AdvanceSteel.Nodes
{
  [IsVisibleInDynamoLibrary(false)]
  public class PropertiesBolts
  {

    public PropertiesBolts()
    {
    }

    public PropertiesBolts(string standard,
                            string boltAssembly,
                            string grade,
                            double diameter, double holeTolerance,
                            int xCount, int yCount,
                            double xSpacing, double ySpacing,
                            double radius, double lengthAddition,
                            int boltConnectionType, bool boltInverted)
    {
      Standard = standard;
      BoltAssembly = boltAssembly;
      Grade = grade;
      Diameter = diameter;
      HoleTolerance = holeTolerance;
      XCount = xCount;
      YCount = yCount;
      XSpacing = xSpacing;
      YSpacing = ySpacing;
      Radius = radius;
      LengthAddition = lengthAddition;
      BoltConnectionType = boltConnectionType;
      BoltInverted = boltInverted;
    }

    public PropertiesBolts(PropertiesBolts derivedBoltProperties)
    {
      Standard = derivedBoltProperties.Standard;
      BoltAssembly = derivedBoltProperties.BoltAssembly;
      Grade = derivedBoltProperties.Grade;
      Diameter = derivedBoltProperties.Diameter;
      HoleTolerance = derivedBoltProperties.HoleTolerance;
      XCount = derivedBoltProperties.XCount;
      YCount = derivedBoltProperties.YCount;
      XSpacing = derivedBoltProperties.XSpacing;
      YSpacing = derivedBoltProperties.YSpacing;
      Radius = derivedBoltProperties.Radius;
      LengthAddition = derivedBoltProperties.LengthAddition;
      BoltConnectionType = derivedBoltProperties.BoltConnectionType;
      BoltInverted = derivedBoltProperties.BoltInverted;
    }

    private string _standard = "";
    private string _boltAssembly = "";
    private string _grade = "";
    private double _diameter = 0;
    private double _holeTolerance = -1;

    private int _yCount = -1;
    private int _xCount = -1;
    private double _xSpacing = 0;
    private double _ySpacing = 0;
    private double _radius = 0;
    private double _lengthAddition = 0;
    private int _boltConnType = -1;

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

    public string BoltAssembly
    {
      get { return _boltAssembly; }
      set
      {
        _boltAssembly = value;
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

    public double LengthAddition
    {
      get { return _lengthAddition; }
      set
      {
        _lengthAddition = value;
      }
    }

    public int BoltConnectionType
    {
      get { return _boltConnType; }
      set
      {
        _boltConnType = value;
        if (foundData)
        {
          foundData = (value > -1);
        }
      }
    }

    public bool BoltInverted
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