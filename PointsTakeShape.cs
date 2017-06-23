using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using SoloProjects.Dudhit.Utilities.GeometryCalculators;
using System.Windows;

namespace SoloProjects.Dudhit.Utilities.Curves
{
 public class PointsTakeShape : IDisposable
  {
    private int radiusX;
    private int radiusY;
    private int radiusZ;
    private int shape;
    private int wallThickness;
    private double estimateTotalCalculations;
    private int estimateCalculated;
    private HashSet<Point3D> myThreadSafeData;
    public HashSet<Point3D> GlobalCurveSet;
    public PointsTakeShape(int myX, int myY, int myZ, int shape, int thick)
    {

      myThreadSafeData = new HashSet<Point3D>();
      this.radiusX=myX;
      this.radiusY=myY;
      this.radiusZ=myZ;
      this.shape=shape;
      this.wallThickness=thick;
      this.estimateCalculated=0;
      this.estimateTotalCalculations=0;
    }
    public void ProcessingShape()
    {
      switch(this.shape)
      {
        case 17:// quartercircle 17 x
          {
            estimateTotalCalculations = Geometries.CirclePerimeter(radiusX)/4;
            MakeQuarterCircle();
            break;
          }
        case 33:// semicircle 33 x
          {
            estimateTotalCalculations = Geometries.CirclePerimeter(radiusX)/2;
            LineAlongPlaneGenerator(-1*radiusX, radiusX, "x", 0, 0);
            MakeQuarterCircle();
            MirrorGlobalSetByAxis("x");
            break;
          }
        case 65:// fullcircle 65 x
          {
            estimateTotalCalculations = Geometries.CirclePerimeter(radiusX);
            LineAlongPlaneGenerator(-1*radiusX, radiusX, "y", 0, 0);
            MakeQuarterCircle();
            MirrorGlobalSetByAxis("x");
            MirrorGlobalSetByAxis("y");
            break;
          }
        case 18:// quarterellipse 18 xy
          {
            estimateTotalCalculations = Geometries.EllipsePerimeter(radiusX, radiusY)/4;
            MakeQuarterEllipse();
            break;
          }
        case 34:// semiellipse 34 xy
          {
            estimateTotalCalculations =  Geometries.EllipsePerimeter(radiusX, radiusY)/2;
            LineAlongPlaneGenerator(-1*radiusX, radiusX, "x", 0, 0);
            MakeQuarterEllipse();
            MirrorGlobalSetByAxis("x");
            break;
          }
        case 66:// fullellipse 66 xy
          {
            estimateTotalCalculations =  Geometries.EllipsePerimeter(radiusX, radiusY);
            LineAlongPlaneGenerator(-1*radiusY, radiusY, "y", 0, 0);
            MakeQuarterEllipse();
            MirrorGlobalSetByAxis("x");
            MirrorGlobalSetByAxis("y");
            break;
          }
        case 20:// quartersphere 20 x
          {
            estimateTotalCalculations =Geometries.SphereArea(radiusX)/4;
            MakeQuarterSphere();
            break;
          }

        case 36: // semisphere 36 x
          {
            estimateTotalCalculations =Geometries.SphereArea(radiusX)/2;
            LineAlongPlaneGenerator(-1*radiusX, radiusX, "x", 0, 0);
            LineAlongPlaneGenerator(-1*radiusX, radiusX, "y", 0, 0);
            MakeQuarterSphere();
            MirrorGlobalSetByAxis("x");
            MirrorGlobalSetByAxis("y");
            break;
          }

        case 68: // fullsphere 68 x
          {
            estimateTotalCalculations = Geometries.SphereArea(radiusX);
            LineAlongPlaneGenerator(-1*radiusX, radiusX, "z", 0, 0);
            MakeQuarterSphere();
            MirrorGlobalSetByAxis("x");
            MirrorGlobalSetByAxis("y");
            MirrorGlobalSetByAxis("z");
            break;
          }

        case 24: // quarterellipsiod 24 xyz
          {
            estimateTotalCalculations =Geometries.EllipseVolume(radiusX, radiusY, radiusZ)/4;
            MakeQuaterEllipsoid();
            break;
          }

        case 40: // semiellipsiod 40 xyz
          {
            estimateTotalCalculations = Geometries.EllipseVolume(radiusX, radiusY, radiusZ)/2;
            LineAlongPlaneGenerator(-1*radiusX, radiusX, "x", 0, 0);
            LineAlongPlaneGenerator(-1*radiusY, radiusY, "y", 0, 0);
            MakeQuaterEllipsoid();
            MirrorGlobalSetByAxis("x");
            MirrorGlobalSetByAxis("y");
            break;
          }

        case 72: // fullellipsiod 72 xyz
          {
            estimateTotalCalculations = Geometries.EllipseVolume(radiusX, radiusY, radiusZ);

            LineAlongPlaneGenerator(-1*radiusZ, radiusZ, "z", 0, 0);
            MakeQuaterEllipsoid();
            MirrorGlobalSetByAxis("x");
            MirrorGlobalSetByAxis("y");
            MirrorGlobalSetByAxis("z");
            break;
          }

      }
    }

    private void MakeQuaterEllipsoid()
    {
      LineAlongPlaneGenerator(0, radiusX, "x", 0, 0);
      LineAlongPlaneGenerator(0, radiusY, "y", 0, 0);
      LineAlongPlaneGenerator(0, radiusZ, "z", 0, 0);
      BresenhamEllipticalCurve xz = new BresenhamEllipticalCurve(radiusX, radiusZ);
      xz.BeginCalculations();
      BresenhamEllipticalCurve yz = new BresenhamEllipticalCurve(radiusY, radiusZ);
      yz.BeginCalculations();
      BresenhamEllipticalCurve xy = new BresenhamEllipticalCurve(radiusX, radiusY);
      xy.BeginCalculations();
      AddSetToGlobalSet(TwoDIntoThreeDPoint(xz.GetCurve(), "xz", 0));
      AddSetToGlobalSet(TwoDIntoThreeDPoint(yz.GetCurve(), "yz", 0));
      AddSetToGlobalSet(TwoDIntoThreeDPoint(xy.GetCurve(), "xy", 0));
      //generate yz curve along X using xy and xz curve points 
      int xx=0;//      radiusX;
      int yy=0;//      radiusY;
      int zz=0;//      radiusZ;
      while(xx<radiusX)
      {
        if(xy.GetCurve().Contains(new Point(xx, radiusY-yy))&&xz.GetCurve().Contains(new Point(xx, radiusZ-zz)))
        {
          yz =new BresenhamEllipticalCurve(radiusY-yy, radiusZ-zz);
          yz.BeginCalculations();
          AddSetToGlobalSet(TwoDIntoThreeDPoint(yz.GetCurve(), "yz", xx));
          xx++;
          if(wallThickness!=0)
          {
            foreach(Point p3 in yz.GetCurve())
            {
              int thickness=zz-wallThickness;
              if(thickness>0)
              {
                LineAlongPlaneGenerator(thickness, zz, "z", (int)p3.X, (int)p3.Y);
              }
            }
          }
        }
        else//no match of y or z in respective curves
        {
          if(!xy.GetCurve().Contains(new Point(xx, radiusY-yy))) { yy++; }
          if(!xz.GetCurve().Contains(new Point(xx, radiusZ-zz))) { zz++; }
        }
      }
    }

    private void AddSetToGlobalSet(object sender)
    {
      HashSet<Point3D> set = (sender as HashSet<Point3D>);
      if(set != null)
      {
        foreach(Point3D p3 in set) { AddNewPointToGlobalSet(p3); }
      }
    }

    private HashSet<Point3D> TwoDIntoThreeDPoint(HashSet<Point> twoDPointCollection, string plane, int fixedPlaneValue)
    {
      HashSet<Point3D> temporaryCollection = new HashSet<Point3D>();
      switch(plane)
      {
        case "xy":
          foreach(Point twoDPoint in twoDPointCollection)
          {
            temporaryCollection.Add(new Point3D(twoDPoint.X, twoDPoint.Y, fixedPlaneValue));
          }
          break;

        case "xz":

          foreach(Point twoDPoint in twoDPointCollection)
          {
            temporaryCollection.Add(new Point3D(twoDPoint.X, fixedPlaneValue, twoDPoint.Y));
          }
          break;

        case "yz":

          foreach(Point twoDPoint in twoDPointCollection)
          {
            temporaryCollection.Add(new Point3D(fixedPlaneValue, twoDPoint.X, twoDPoint.Y));
          }
          break;
      }
      return temporaryCollection;
    }

    private void MakeQuarterSphere()
    {
      LineAlongPlaneGenerator(0, radiusX, "x", 0, 0);
      LineAlongPlaneGenerator(0, radiusX, "y", 0, 0);
      LineAlongPlaneGenerator(0, radiusX, "z", 0, 0);
      BresenhamCircularCurve xy= new BresenhamCircularCurve(radiusX);
      xy.BeginCalculations();
      AddSetToGlobalSet(TwoDIntoThreeDPoint(xy.GetCurve(), "xy", 0));
      AddSetToGlobalSet(TwoDIntoThreeDPoint(xy.GetCurve(), "xz", 0));
      AddSetToGlobalSet(TwoDIntoThreeDPoint(xy.GetCurve(), "yz", 0));
      if(wallThickness!=0)
      {
        foreach(Point p in xy.GetCurve())
        {
          int thickness=(int)p.Y-wallThickness;
          if(thickness>0)
          {
            LineAlongPlaneGenerator(thickness, (int)p.Y, "x", (int)p.X, 0);
            LineAlongPlaneGenerator(thickness, (int)p.Y, "z", (int)p.X, 0);
            LineAlongPlaneGenerator(thickness, (int)p.Y, "y", 0, (int)p.X);
          }
        }
      }
      foreach(Point p in xy.GetCurve())
      {
        if(p.X>0)
        {
          BresenhamCircularCurve xz= new BresenhamCircularCurve((int)p.X);
          xz.BeginCalculations();
          AddSetToGlobalSet(TwoDIntoThreeDPoint(xz.GetCurve(), "xz", (int)p.Y));
          estimateCalculated++;
          if(wallThickness!=0)
          {
            foreach(Point wallTthick in xz.GetCurve())
            {
              int thickness=(int)wallTthick.X-wallThickness;
              if(thickness>0)
              {
                LineAlongPlaneGenerator(thickness, (int)wallTthick.X, "x", (int)p.Y, (int)wallTthick.Y);
              }
            }
          }
        }
      }
    }

    private void MakeQuarterEllipse()
    {
      LineAlongPlaneGenerator(0, radiusX, "x", 0, 0);
      LineAlongPlaneGenerator(0, radiusY, "y", 0, 0);

      BresenhamEllipticalCurve xyCurve = new BresenhamEllipticalCurve(radiusX, radiusY);
      xyCurve.BeginCalculations();
      AddSetToGlobalSet(TwoDIntoThreeDPoint(xyCurve.GetCurve(), "xy", 0));
      estimateCalculated++;

      if(wallThickness!=0)
      {
        foreach(Point p in xyCurve.GetCurve())
        {
          int thickness=(int)p.X-wallThickness;
          if(thickness>0)
          {
            LineAlongPlaneGenerator(thickness, (int)p.X, "x", (int)p.Y, 0);
          }
        }
      }
      xyCurve=null;
    }



    private void MakeQuarterCircle()
    {
      LineAlongPlaneGenerator(0, radiusX, "x", 0, 0);
      LineAlongPlaneGenerator(0, radiusX, "y", 0, 0);

      BresenhamCircularCurve xyCurve= new BresenhamCircularCurve(radiusX);
      xyCurve.BeginCalculations();
      AddSetToGlobalSet(TwoDIntoThreeDPoint(xyCurve.GetCurve(), "xy", 0));
      estimateCalculated++;
      if(wallThickness!=0)
      {
        foreach(Point p in xyCurve.GetCurve())
        {
          int thickness=(int)p.X-wallThickness;
          if(thickness>0)
          {
            LineAlongPlaneGenerator(thickness, (int)p.X, "x", (int)p.Y, 0);
          }
        }
      }
      xyCurve=null;

    }

    private void MirrorGlobalSetByAxis(string axis)
    {
      estimateCalculated=0;
      estimateTotalCalculations =myThreadSafeData.Count;
      HashSet<Point3D> tempSet= new HashSet<Point3D>();
      switch(axis)
      {
        case "x":
          foreach(Point3D ppp in myThreadSafeData)
          {
            tempSet.Add(new Point3D(-1*ppp.X, ppp.Y, ppp.Z));
          }
          break;
        case "y":
          foreach(Point3D ppp in myThreadSafeData)
          {
            tempSet.Add(new Point3D(ppp.X, -1*ppp.Y, ppp.Z));
          }
          break;
        case "z":
          foreach(Point3D ppp in myThreadSafeData)
          {
            tempSet.Add(new Point3D(ppp.X, ppp.Y, -1*ppp.Z));
          }
          break;
      }
      if(tempSet.Count>0)
      {
        AddSetToGlobalSet(tempSet);
      }
      tempSet=null;
    }

    private void LineAlongPlaneGenerator(int start, int end, string axis, int yx, int zy)
    {
      if(start<=end&&end>0&&axis!=string.Empty)
      {
        switch(axis)
        {
          case "x":
            for(int i=start;i<end;i++)
            {
              AddNewPointToGlobalSet(new Point3D(i, yx, zy));
            }
            break;
          case "y":
            for(int i=start;i<end;i++)
            {
              AddNewPointToGlobalSet(new Point3D(yx, i, zy));
            }
            break;
          case "z":
            for(int i=start;i<end;i++)
            {
              AddNewPointToGlobalSet(new Point3D(yx, zy, i));
            }
            break;
        }
        GlobalCurveSet=myThreadSafeData;
      }
    }
 
    private void AddNewPointToGlobalSet(Point3D pointToAdd)
    {
      if(!myThreadSafeData.Contains(pointToAdd))
      {
        myThreadSafeData.Add(pointToAdd);
      }
    }
 
    #region disposal
    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    ~PointsTakeShape()
    {
      Dispose(false);
    }

    protected virtual void Dispose(bool disposing)
    {
      if(disposing)
      {
        // free managed resources 
        //if (Encoding != null)
        //{
        // Encoding.Dispose();
        // Encoding = null;
        //}
      }

    }
    #endregion
  }
}
