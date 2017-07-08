using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using SoloProjects.Dudhit.Utilities.GeometryCalculators;
using System.Windows;
using System.IO;

namespace SoloProjects.Dudhit.Utilities.Curves
{
  public class PointsTakeShape : IDisposable
  {
    private int radiusX;
    private int radiusY;
    private int radiusZ;
    private int shape;
    private int wallThickness;
    private string dataPath;
    private double estimateTotalCalculations;
    private int estimateCalculated;
    private List<string> OUTPUTNAMES =new List<string>() { "framePxPyPz.dat", "skinPxPyPz.dat", "frameNxPyPz.dat", "skinNxPyPz.dat", "framePNxNyPz.dat", "skinPNxNyPz.dat", "framePNxPNyNz.dat", "skinPNxPNyNz.dat" };

    private HashSet<Point3D> frameData;
    private HashSet<Point3D> skinData;
    public PointsTakeShape(int myX, int myY, int myZ, int shape, int thick)
    {

      frameData = new HashSet<Point3D>();
      skinData = new HashSet<Point3D>();
      this.radiusX=myX;
      this.radiusY=myY;
      this.radiusZ=myZ;
      this.shape=shape;
      this.wallThickness=thick;
      this.estimateCalculated=0;
      this.estimateTotalCalculations=0;
      FileSystemStuff();

    }

    private void FileSystemStuff()
    {
      dataPath= Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SEPB_data");
      if(!Directory.Exists(dataPath)) { Directory.CreateDirectory(dataPath); }
    }
    public void ProcessingShape()
    {

      switch(this.shape)
      {
        #region quartercircle
        case 17:// quartercircle 17 x
          {
            estimateTotalCalculations = Geometries.CirclePerimeter(radiusX)/4;
            MakeQuarterCircle();
            PurgeDateToFile(Path.Combine(dataPath, OUTPUTNAMES[0]), frameData);
            PurgeDateToFile(Path.Combine(dataPath, OUTPUTNAMES[1]), skinData);
            break;
          }
        #endregion
        #region semicircle
        case 33:// semicircle 33 x
          {
            estimateTotalCalculations = Geometries.CirclePerimeter(radiusX)/2;
            // LineAlongPlaneGenerator(-1*radiusX, radiusX, "x", 0, 0);
            MakeQuarterCircle();
            PurgeDateToFile(Path.Combine(dataPath, OUTPUTNAMES[0]), frameData);
            PurgeDateToFile(Path.Combine(dataPath, OUTPUTNAMES[1]), skinData);
            MirrorFileByAxis("x", Path.Combine(dataPath, OUTPUTNAMES[0]), Path.Combine(dataPath, OUTPUTNAMES[2]));
            MirrorFileByAxis("x", Path.Combine(dataPath, OUTPUTNAMES[1]), Path.Combine(dataPath, OUTPUTNAMES[3]));
            break;
          }
        #endregion
        #region fullcircle
        case 65:// fullcircle 65 x
          {
            estimateTotalCalculations = Geometries.CirclePerimeter(radiusX);
            //   LineAlongPlaneGenerator(-1*radiusX, radiusX, "y", 0, 0);
            MakeQuarterCircle();
            PurgeDateToFile(Path.Combine(dataPath, OUTPUTNAMES[0]), frameData);
            PurgeDateToFile(Path.Combine(dataPath, OUTPUTNAMES[1]), skinData);
            MirrorFileByAxis("x", Path.Combine(dataPath, OUTPUTNAMES[0]), Path.Combine(dataPath, OUTPUTNAMES[2]));
            MirrorFileByAxis("x", Path.Combine(dataPath, OUTPUTNAMES[1]), Path.Combine(dataPath, OUTPUTNAMES[3]));
            MirrorFileByAxis("y", Path.Combine(dataPath, OUTPUTNAMES[0]), Path.Combine(dataPath, OUTPUTNAMES[4]));
            MirrorFileByAxis("y", Path.Combine(dataPath, OUTPUTNAMES[1]), Path.Combine(dataPath, OUTPUTNAMES[5]));
            MirrorFileByAxis("y", Path.Combine(dataPath, OUTPUTNAMES[2]), Path.Combine(dataPath, OUTPUTNAMES[4]));
            MirrorFileByAxis("y", Path.Combine(dataPath, OUTPUTNAMES[3]), Path.Combine(dataPath, OUTPUTNAMES[5]));
            break;
          }
        #endregion
        #region quarterellipse
        case 18:// quarterellipse 18 xy
          {
            estimateTotalCalculations = Geometries.EllipsePerimeter(radiusX, radiusY)/4;
            MakeQuarterEllipse();
            PurgeDateToFile(Path.Combine(dataPath, OUTPUTNAMES[0]), frameData);
            PurgeDateToFile(Path.Combine(dataPath, OUTPUTNAMES[1]), skinData);
            break;
          }
        #endregion
        #region semiellipse
        case 34:// semiellipse 34 xy
          {
            estimateTotalCalculations =  Geometries.EllipsePerimeter(radiusX, radiusY)/2;
            //    LineAlongPlaneGenerator(-1*radiusX, radiusX, "x", 0, 0);
            MakeQuarterEllipse();
            PurgeDateToFile(Path.Combine(dataPath, OUTPUTNAMES[0]), frameData);
            PurgeDateToFile(Path.Combine(dataPath, OUTPUTNAMES[1]), skinData);
            MirrorFileByAxis("x", Path.Combine(dataPath, OUTPUTNAMES[0]), Path.Combine(dataPath, OUTPUTNAMES[2]));
            MirrorFileByAxis("x", Path.Combine(dataPath, OUTPUTNAMES[1]), Path.Combine(dataPath, OUTPUTNAMES[3]));

            break;
          }
        #endregion
        #region fullellipse
        case 66:// fullellipse 66 xy
          {
            estimateTotalCalculations =  Geometries.EllipsePerimeter(radiusX, radiusY);
            //    LineAlongPlaneGenerator(-1*radiusY, radiusY, "y", 0, 0);
            MakeQuarterEllipse();
            PurgeDateToFile(Path.Combine(dataPath, OUTPUTNAMES[0]), frameData);
            PurgeDateToFile(Path.Combine(dataPath, OUTPUTNAMES[1]), skinData);
            MirrorFileByAxis("x", Path.Combine(dataPath, OUTPUTNAMES[0]), Path.Combine(dataPath, OUTPUTNAMES[2]));
            MirrorFileByAxis("x", Path.Combine(dataPath, OUTPUTNAMES[1]), Path.Combine(dataPath, OUTPUTNAMES[3]));
            MirrorFileByAxis("y", Path.Combine(dataPath, OUTPUTNAMES[0]), Path.Combine(dataPath, OUTPUTNAMES[4]));
            MirrorFileByAxis("y", Path.Combine(dataPath, OUTPUTNAMES[1]), Path.Combine(dataPath, OUTPUTNAMES[5]));
            MirrorFileByAxis("y", Path.Combine(dataPath, OUTPUTNAMES[2]), Path.Combine(dataPath, OUTPUTNAMES[4]));
            MirrorFileByAxis("y", Path.Combine(dataPath, OUTPUTNAMES[3]), Path.Combine(dataPath, OUTPUTNAMES[5]));
            break;
          }
        #endregion
        #region eigthsphere
        case 20:// eigthsphere 20 x
          {
            estimateTotalCalculations =Geometries.SphereArea(radiusX)/4;
            MakeEigthSphere();
            PurgeDateToFile(Path.Combine(dataPath, OUTPUTNAMES[0]), frameData);
            PurgeDateToFile(Path.Combine(dataPath, OUTPUTNAMES[1]), skinData);
            break;
          }
        #endregion
        #region semisphere
        case 36: // semisphere 36 x
          {
            estimateTotalCalculations =Geometries.SphereArea(radiusX)/2;
            //   LineAlongPlaneGenerator(-1*radiusX, radiusX, "x", 0, 0);
            //    LineAlongPlaneGenerator(-1*radiusX, radiusX, "y", 0, 0);
            MakeEigthSphere();
            PurgeDateToFile(Path.Combine(dataPath, OUTPUTNAMES[0]), frameData);
            PurgeDateToFile(Path.Combine(dataPath, OUTPUTNAMES[1]), skinData);
            MirrorFileByAxis("x", Path.Combine(dataPath, OUTPUTNAMES[0]), Path.Combine(dataPath, OUTPUTNAMES[2]));
            MirrorFileByAxis("x", Path.Combine(dataPath, OUTPUTNAMES[1]), Path.Combine(dataPath, OUTPUTNAMES[3]));
            MirrorFileByAxis("y", Path.Combine(dataPath, OUTPUTNAMES[0]), Path.Combine(dataPath, OUTPUTNAMES[4]));
            MirrorFileByAxis("y", Path.Combine(dataPath, OUTPUTNAMES[1]), Path.Combine(dataPath, OUTPUTNAMES[5]));
            MirrorFileByAxis("y", Path.Combine(dataPath, OUTPUTNAMES[2]), Path.Combine(dataPath, OUTPUTNAMES[4]));
            MirrorFileByAxis("y", Path.Combine(dataPath, OUTPUTNAMES[3]), Path.Combine(dataPath, OUTPUTNAMES[5]));
            break;
          }
        #endregion
        #region fullsphere
        case 68: // fullsphere 68 x
          {
            estimateTotalCalculations = Geometries.SphereArea(radiusX);
            //     LineAlongPlaneGenerator(-1*radiusX, radiusX, "z", 0, 0);
            MakeEigthSphere();
            PurgeDateToFile(Path.Combine(dataPath, OUTPUTNAMES[0]), frameData);
            PurgeDateToFile(Path.Combine(dataPath, OUTPUTNAMES[1]), skinData);
            MirrorFileByAxis("x", Path.Combine(dataPath, OUTPUTNAMES[0]), Path.Combine(dataPath, OUTPUTNAMES[2]));
            MirrorFileByAxis("x", Path.Combine(dataPath, OUTPUTNAMES[1]), Path.Combine(dataPath, OUTPUTNAMES[3]));
            MirrorFileByAxis("y", Path.Combine(dataPath, OUTPUTNAMES[0]), Path.Combine(dataPath, OUTPUTNAMES[4]));
            MirrorFileByAxis("y", Path.Combine(dataPath, OUTPUTNAMES[1]), Path.Combine(dataPath, OUTPUTNAMES[5]));
            MirrorFileByAxis("y", Path.Combine(dataPath, OUTPUTNAMES[2]), Path.Combine(dataPath, OUTPUTNAMES[4]));
            MirrorFileByAxis("y", Path.Combine(dataPath, OUTPUTNAMES[3]), Path.Combine(dataPath, OUTPUTNAMES[5]));
            MirrorFileByAxis("z", Path.Combine(dataPath, OUTPUTNAMES[0]), Path.Combine(dataPath, OUTPUTNAMES[6]));
            MirrorFileByAxis("z", Path.Combine(dataPath, OUTPUTNAMES[2]), Path.Combine(dataPath, OUTPUTNAMES[6]));
            MirrorFileByAxis("z", Path.Combine(dataPath, OUTPUTNAMES[4]), Path.Combine(dataPath, OUTPUTNAMES[6]));
            MirrorFileByAxis("z", Path.Combine(dataPath, OUTPUTNAMES[1]), Path.Combine(dataPath, OUTPUTNAMES[7]));
            MirrorFileByAxis("z", Path.Combine(dataPath, OUTPUTNAMES[3]), Path.Combine(dataPath, OUTPUTNAMES[7]));
            MirrorFileByAxis("z", Path.Combine(dataPath, OUTPUTNAMES[5]), Path.Combine(dataPath, OUTPUTNAMES[7]));
            break;
          }
        #endregion
        #region eigthellipsiod
        case 24: // eigthellipsiod 24 xyz
          {
            estimateTotalCalculations =Geometries.EllipseVolume(radiusX, radiusY, radiusZ)/4;
            MakeEigthOfEllipsoid();
            PurgeDateToFile(Path.Combine(dataPath, OUTPUTNAMES[0]), frameData);
            PurgeDateToFile(Path.Combine(dataPath, OUTPUTNAMES[1]), skinData);
            break;
          }
        #endregion
        #region semiellipsiod
        case 40: // semiellipsiod 40 xyz
          {
            estimateTotalCalculations = Geometries.EllipseVolume(radiusX, radiusY, radiusZ)/2;
            //     LineAlongPlaneGenerator(-1*radiusX, radiusX, "x", 0, 0);
            //     LineAlongPlaneGenerator(-1*radiusY, radiusY, "y", 0, 0);
            MakeEigthOfEllipsoid();
            PurgeDateToFile(Path.Combine(dataPath, OUTPUTNAMES[0]), frameData);
            PurgeDateToFile(Path.Combine(dataPath, OUTPUTNAMES[1]), skinData);
            MirrorFileByAxis("x", Path.Combine(dataPath, OUTPUTNAMES[0]), Path.Combine(dataPath, OUTPUTNAMES[2]));
            MirrorFileByAxis("x", Path.Combine(dataPath, OUTPUTNAMES[1]), Path.Combine(dataPath, OUTPUTNAMES[3]));
            MirrorFileByAxis("y", Path.Combine(dataPath, OUTPUTNAMES[0]), Path.Combine(dataPath, OUTPUTNAMES[4]));
            MirrorFileByAxis("y", Path.Combine(dataPath, OUTPUTNAMES[1]), Path.Combine(dataPath, OUTPUTNAMES[5]));
            MirrorFileByAxis("y", Path.Combine(dataPath, OUTPUTNAMES[2]), Path.Combine(dataPath, OUTPUTNAMES[4]));
            MirrorFileByAxis("y", Path.Combine(dataPath, OUTPUTNAMES[3]), Path.Combine(dataPath, OUTPUTNAMES[5]));
            break;
          }
        #endregion
        #region fullellipsiod
        case 72: // fullellipsiod 72 xyz
          {
            estimateTotalCalculations = Geometries.EllipseVolume(radiusX, radiusY, radiusZ);

            //     LineAlongPlaneGenerator(-1*radiusZ, radiusZ, "z", 0, 0);
            MakeEigthOfEllipsoid();
            PurgeDateToFile(Path.Combine(dataPath, OUTPUTNAMES[0]), frameData);
            PurgeDateToFile(Path.Combine(dataPath, OUTPUTNAMES[1]), skinData);
            MirrorFileByAxis("x", Path.Combine(dataPath, OUTPUTNAMES[0]), Path.Combine(dataPath, OUTPUTNAMES[2]));
            MirrorFileByAxis("x", Path.Combine(dataPath, OUTPUTNAMES[1]), Path.Combine(dataPath, OUTPUTNAMES[3]));
            MirrorFileByAxis("y", Path.Combine(dataPath, OUTPUTNAMES[0]), Path.Combine(dataPath, OUTPUTNAMES[4]));
            MirrorFileByAxis("y", Path.Combine(dataPath, OUTPUTNAMES[1]), Path.Combine(dataPath, OUTPUTNAMES[5]));
            MirrorFileByAxis("y", Path.Combine(dataPath, OUTPUTNAMES[2]), Path.Combine(dataPath, OUTPUTNAMES[4]));
            MirrorFileByAxis("y", Path.Combine(dataPath, OUTPUTNAMES[3]), Path.Combine(dataPath, OUTPUTNAMES[5]));
            MirrorFileByAxis("z", Path.Combine(dataPath, OUTPUTNAMES[0]), Path.Combine(dataPath, OUTPUTNAMES[6]));
            MirrorFileByAxis("z", Path.Combine(dataPath, OUTPUTNAMES[2]), Path.Combine(dataPath, OUTPUTNAMES[6]));
            MirrorFileByAxis("z", Path.Combine(dataPath, OUTPUTNAMES[4]), Path.Combine(dataPath, OUTPUTNAMES[6]));
            MirrorFileByAxis("z", Path.Combine(dataPath, OUTPUTNAMES[1]), Path.Combine(dataPath, OUTPUTNAMES[7]));
            MirrorFileByAxis("z", Path.Combine(dataPath, OUTPUTNAMES[3]), Path.Combine(dataPath, OUTPUTNAMES[7]));
            MirrorFileByAxis("z", Path.Combine(dataPath, OUTPUTNAMES[5]), Path.Combine(dataPath, OUTPUTNAMES[7]));
            break;
          }
        #endregion

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

    private void AddSetToGlobalSet(object sender)
    {

      HashSet<Point3D> set = (sender as HashSet<Point3D>);
      if(set != null)
      {
        foreach(Point3D p3 in set)
        {
          if(frameData.Contains(p3)||skinData.Contains(p3)) { continue; }
          else
          {
            if(p3.X==0||p3.Y==0||p3.Z==0)
            { frameData.Add(p3); }
            else
            { skinData.Add(p3); }
          }

          //if(frameData!=null&&frameData.Count>1000)
          //{
          //  PurgeDateToFile(Path.Combine(dataPath, OUTPUTNAMES[0]), frameData);
          //  frameData.Clear();
          //}
          //if(skinData!=null&&skinData.Count>1000)
          //{
          //  PurgeDateToFile(Path.Combine(dataPath, OUTPUTNAMES[1]), skinData);
          //  skinData.Clear();
          //}
        }
      }
    }

    private void MakeEigthOfEllipsoid()
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
              int thickness=(int)p3.X-wallThickness;
              if(thickness>0)
              {
                LineAlongPlaneGenerator(thickness, (int)p3.X, "y", xx, (int)p3.Y);
              }
            }
          }
        }
        else//no match of y or z in respective curves
        {
          while(!xy.GetCurve().Contains(new Point(xx, radiusY-yy)))
          { yy++; }
          while(!xz.GetCurve().Contains(new Point(xx, radiusZ-zz)))
          { zz++; }
        }
      }
    }

    private void MakeEigthSphere()
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

    private void MirrorFileByAxis(string axis, string fileInName, string fileOut)
    {
      HashSet<Point3D> tempSet= new HashSet<Point3D>();
      //string line = File.ReadLines(FileName).Skip(14).Take(1).First();
      switch(axis)
      {
        /*
           // Source must be array or IList.
        var source = Enumerable.Range(0, 100000).ToArray();

        // Partition the entire source array.
        var rangePartitioner = Partitioner.Create(0, source.Length);

        double[] results = new double[source.Length];

        // Loop over the partitions in parallel.
        Parallel.ForEach(rangePartitioner, (range, loopState) =>
        {
            // Loop over each range element without a delegate invocation.
            for (int i = range.Item1; i < range.Item2; i++)
            {
                results[i] = source[i] * Math.PI;
            }
        });
         */
        case "x":
          using(StreamReader fileIn = new StreamReader(fileInName))
          {
            while(fileIn.Peek()!=-1)
            {
              string[] textInt=   fileIn.ReadLine().Split(',');
              if(textInt[0]=="0")
                continue;
              tempSet.Add(new Point3D(-1*int.Parse(textInt[0]), int.Parse(textInt[1]), int.Parse(textInt[2])));
            }
          }
          PurgeDateToFile(fileOut, tempSet);
          tempSet.Clear();
          break;
        case "y":
          using(StreamReader fileIn = new StreamReader(fileInName))
          {
            while(fileIn.Peek()!=-1)
            {
              string[] textInt=   fileIn.ReadLine().Split(',');
              if(textInt[1]=="0")
                continue;
              tempSet.Add(new Point3D(int.Parse(textInt[0]), -1* int.Parse(textInt[1]), int.Parse(textInt[2])));
            }
          }
          PurgeDateToFile(fileOut, tempSet);
          tempSet.Clear();
          break;
        case "z":
          using(StreamReader fileIn = new StreamReader(fileInName))
          {
            while(fileIn.Peek()!=-1)
            {
              string[] textInt=   fileIn.ReadLine().Split(',');
              if(textInt[2]=="0")
                continue;
              tempSet.Add(new Point3D(int.Parse(textInt[0]), int.Parse(textInt[1]), -1* int.Parse(textInt[2])));
            }
          }
          PurgeDateToFile(fileOut, tempSet);
          tempSet.Clear();
          break;
      }

      tempSet=null;
    }

    private void LineAlongPlaneGenerator(int start, int end, string axis, int yx, int zy)
    {
      if(start<=end&&end>0&&axis!=string.Empty)
      {
        HashSet<Point3D> tempSet= new HashSet<Point3D>();
        switch(axis)
        {
          case "x":
            for(int i=start;i<end;i++)
            {
              tempSet.Add(new Point3D(i, yx, zy));
            }
            break;
          case "y":
            for(int i=start;i<end;i++)
            {
              tempSet.Add(new Point3D(yx, i, zy));
            }
            break;
          case "z":
            for(int i=start;i<end;i++)
            {
              tempSet.Add(new Point3D(yx, zy, i));
            }
            break;
        }
        if(tempSet.Count>0)
        {
          AddSetToGlobalSet(tempSet);
        }
        tempSet=null;

      }
    }

    private void PurgeDateToFile(string file, object data)
    {
      HashSet<Point3D> set = (data as HashSet<Point3D>);
      if(set != null)
      {
        using(StreamWriter sw =new StreamWriter(file, true))
        {
          foreach(Point3D p in set)
          {
            sw.WriteLine(p);
          }
        }
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
