using SoloProjects.Dudhit.Utilities.GeometryCalculators;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;

namespace SoloProjects.Dudhit.Utilities.Curves
{
  public delegate void DetermineProcessingPathsCompletedEventHandler(object sender, DetermineProcessingPathsCompletedEventArgs e);

  public class PointsToShape : IDisposable
  {
    private ConcurrentQueue<Point3D> myThreadSafeData;
    //  private HashSet<Point3D> myThreadSafeData;
    //  private ConcurrentBag<Point3D> myThreadSafeData;
    private object lockingOb;
    private int radiusX;
    private int radiusY;
    private int radiusZ;
    private int shape;
    private bool solid;
    private double estimateTotalCalculations;
    private int estimateCalculated;
    public HashSet<Point3D> GlobalCurveSet;
    private ParallelOptions setCrashDependancy;
    private IProgress<MyTaskProgressReporter> progress;
    /* 
    quartercircle 17 semicircle 33 fullcircle 65 x
    quarterellipse 18 semiellipse 34 fullellipse 66 xy
    quartersphere 20 semisphere 36 fullsphere 68 x
    quarterellipsiod 24 semiellipsiod 40 fullellipsiod 72 xyz
    */

    /// <summary>
    /// provide radius x 3 shape and mass - get a collection of Point3D
    /// </summary>
    /// <param name="myX"></param>
    /// <param name="myY"></param>
    /// <param name="myZ"></param>
    /// <param name="shape"></param>
    /// <param name="mass"></param>
    public PointsToShape(int myX, int myY, int myZ, int shape, bool mass, IProgress<MyTaskProgressReporter> progress)
    {
      myThreadSafeData = new ConcurrentQueue<Point3D>();
      //   myThreadSafeData = new HashSet<Point3D>();
      //  myThreadSafeData = new ConcurrentBag<Point3D>();
      this.radiusX=myX;
      this.radiusY=myY;
      this.radiusZ=myZ;
      this.shape=shape;
      this.solid=mass;
      this.estimateCalculated=0;
      this.progress=progress;
      this.lockingOb=new object();
      this.setCrashDependancy =   new ParallelOptions() { MaxDegreeOfParallelism = 4 };

    }

    /// <summary>
    /// offers async call to ProcessingShape
    /// </summary>
    public async void ProcessingShapeAsync()
    {
      /*  await Task.Run(() => {*/
      ProcessingShape();/* });*/
      await Task.Run(() => { ConvertAndRemoveDuplicates(); });
    }

    private void ConvertAndRemoveDuplicates()
    {
      GlobalCurveSet = new HashSet<Point3D>();
      foreach(Point3D p in myThreadSafeData)
      {
        if(!GlobalCurveSet.Contains(p))
          GlobalCurveSet.Add(p);
      }
      GiveFeedback(100, "\nData has shape.\n");
    }

    /// <summary>
    /// BlueprintModel owns the enum that calculates shape
    /// </summary>
    /// <returns></returns>
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
            if(LineAlongPlaneGenerator(-1*radiusX, radiusX, "x", 0, 0))
            {

              MakeQuarterCircle();
              GiveFeedback(-1, "Adding depth");
              MirrorGlobalSetByAxis("x");
            }
            break;
          }
        case 65:// fullcircle 65 x
          {
            estimateTotalCalculations = Geometries.CirclePerimeter(radiusX);
            if(LineAlongPlaneGenerator(-1*radiusX, radiusX, "y", 0, 0))
            {
              MakeQuarterCircle();
              GiveFeedback(-1, "Adding depth");
              MirrorGlobalSetByAxis("x");
              GiveFeedback(-1, "Adding more depth");
              MirrorGlobalSetByAxis("y");
            }
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
            if(LineAlongPlaneGenerator(-1*radiusX, radiusX, "x", 0, 0))
            {
              MakeQuarterEllipse();
              GiveFeedback(-1, "Adding depth");
              MirrorGlobalSetByAxis("x");
            }
            break;
          }
        case 66:// fullellipse 66 xy
          {
            estimateTotalCalculations =  Geometries.EllipsePerimeter(radiusX, radiusY);
            if(LineAlongPlaneGenerator(-1*radiusY, radiusY, "y", 0, 0))
            {
              MakeQuarterEllipse();
              GiveFeedback(-1, "Adding depth");
              MirrorGlobalSetByAxis("x");
              GiveFeedback(-1, "Adding more depth");
              MirrorGlobalSetByAxis("y");
            }
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
            if(LineAlongPlaneGenerator(-1*radiusX, radiusX, "x", 0, 0)&&LineAlongPlaneGenerator(-1*radiusX, radiusX, "y", 0, 0))
            {
              MakeQuarterSphere();
              GiveFeedback(-1, "Adding depth");
              MirrorGlobalSetByAxis("x");
              GiveFeedback(-1, "Adding more depth");
              MirrorGlobalSetByAxis("y");
            }
            break;
          }

        case 68: // fullsphere 68 x
          {
            estimateTotalCalculations = Geometries.SphereArea(radiusX);
            if(LineAlongPlaneGenerator(-1*radiusX, radiusX, "z", 0, 0))
            {
              MakeQuarterSphere();
              GiveFeedback(-1, "Adding depth");
              MirrorGlobalSetByAxis("x");
              GiveFeedback(-1, "Adding more depth");
              MirrorGlobalSetByAxis("y");
              GiveFeedback(-1, "Adding even more depth");
              MirrorGlobalSetByAxis("z");
            }
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
            if(LineAlongPlaneGenerator(-1*radiusX, radiusX, "x", 0, 0)&&LineAlongPlaneGenerator(-1*radiusY, radiusY, "y", 0, 0))
            {
              MakeQuaterEllipsoid();
              GiveFeedback(-1, "Adding depth");
              MirrorGlobalSetByAxis("x");
              GiveFeedback(-1, "Adding more depth");
              MirrorGlobalSetByAxis("y");
            }
            break;
          }

        case 72: // fullellipsiod 72 xyz
          {
            estimateTotalCalculations = Geometries.EllipseVolume(radiusX, radiusY, radiusZ);
            if(LineAlongPlaneGenerator(-1*radiusZ, radiusZ, "z", 0, 0))
            {
              MakeQuaterEllipsoid();
              GiveFeedback(-1, "Adding depth");
              MirrorGlobalSetByAxis("x");
              GiveFeedback(-1, "Adding more depth");
              MirrorGlobalSetByAxis("y");
              GiveFeedback(-1, "Adding even moredepth");
              MirrorGlobalSetByAxis("z");
            }
            break;
          }

      }
    }

    private void MakeQuaterEllipsoid()
    {
      if(LineAlongPlaneGenerator(0, radiusX, "x", 0, 0)&&LineAlongPlaneGenerator(0, radiusY, "y", 0, 0)&&LineAlongPlaneGenerator(0, radiusZ, "z", 0, 0))
      {
        BresenhamEllipticalCurve xz = new BresenhamEllipticalCurve(radiusX, radiusZ);
        xz.BeginCalculations();
        BresenhamEllipticalCurve yz = new BresenhamEllipticalCurve(radiusY, radiusZ);
        yz.BeginCalculations();
        BresenhamEllipticalCurve xy = new BresenhamEllipticalCurve(radiusX, radiusY);
        xy.BeginCalculations();
        AddSetToGlobalSet(TwoDIntoThreeDPoint(xz.GetCurve(), "xz", 0));
        AddSetToGlobalSet(TwoDIntoThreeDPoint(yz.GetCurve(), "yz", 0));
        AddSetToGlobalSet(TwoDIntoThreeDPoint(xy.GetCurve(), "xy", 0));
        GiveFeedback(PercentComplete(), " base Ellipsoid calculated");
        //generate yz curve along X using xy and xz curve points the lazy way
        int xx= radiusX;
        int yy=radiusY;
        int zz=radiusZ;
        while(xx>0)
        {
          while(yy>0)
          {
            while(zz>0)
            {
              if(xy.GetCurve().Contains(new Point(xx, yy))&&xz.GetCurve().Contains(new Point(xx, zz)))
              {
                yz =new BresenhamEllipticalCurve(yy, zz);
                yz.BeginCalculations();
                AddSetToGlobalSet(TwoDIntoThreeDPoint(yz.GetCurve(), "yz", xx));
                estimateCalculated++;
                // GiveFeedback(PercentComplete(), null);
                if(solid)
                {
                  Parallel.ForEach(yz.GetCurve(), solidP =>
                    {
                      lock(lockingOb)
                      {
                        LineAlongPlaneGenerator(0, xx, "x", (int)solidP.X, (int)solidP.Y);
                      }
                    });
                }
                else
                {
                  Parallel.ForEach(yz.GetCurve(), solidP =>
                  {
                    lock(lockingOb)
                    {
                      int thickness=xx-2;
                      if(thickness>0)
                      {
                        LineAlongPlaneGenerator(thickness, xx, "x", (int)solidP.X, (int)solidP.Y);
                      }
                    }
                  });
                }
              }
              zz--;
            }
            yy--;
            zz=radiusZ;
          }
          xx--;
          yy=radiusY;
        }
        xy=null;
        yz=null;
        xz=null;
      }
    }

    private void MakeQuarterSphere()
    {
      if(LineAlongPlaneGenerator(0, radiusX, "x", 0, 0)&&LineAlongPlaneGenerator(0, radiusX, "y", 0, 0)&&LineAlongPlaneGenerator(0, radiusX, "z", 0, 0))
      {
        BresenhamCircularCurve xy= new BresenhamCircularCurve(radiusX);
        xy.BeginCalculations();
        AddSetToGlobalSet(TwoDIntoThreeDPoint(xy.GetCurve(), "xy", 0));
        AddSetToGlobalSet(TwoDIntoThreeDPoint(xy.GetCurve(), "xz", 0));
        AddSetToGlobalSet(TwoDIntoThreeDPoint(xy.GetCurve(), "yz", 0));
        GiveFeedback(PercentComplete(), " base sphere calculated");
        if(solid)
        {
          Parallel.ForEach(xy.GetCurve(), setCrashDependancy, p =>
          {
            lock(lockingOb)
            {
              LineAlongPlaneGenerator(0, (int)p.Y, "x", (int)p.X, 0);
              LineAlongPlaneGenerator(0, (int)p.Y, "z", (int)p.X, 0);
              LineAlongPlaneGenerator(0, (int)p.Y, "y", 0, (int)p.X);
            }
          });
        }//RE-ENFORCEMENT by adding thickness
        else
        {
          Parallel.ForEach(xy.GetCurve(), setCrashDependancy, p =>
          {
            lock(lockingOb)
            {
              int thickness=(int)p.Y-2;
              if(thickness>0)
              {
                LineAlongPlaneGenerator(thickness, (int)p.Y, "x", (int)p.X, 0);
                LineAlongPlaneGenerator(thickness, (int)p.Y, "z", (int)p.X, 0);
                LineAlongPlaneGenerator(thickness, (int)p.Y, "y", 0, (int)p.X);
              }
            }
          });
        }
        Parallel.ForEach(xy.GetCurve(), setCrashDependancy, p =>
        {
          lock(lockingOb)
          {
            if(p.X>0)
            {
              BresenhamCircularCurve xz= new BresenhamCircularCurve((int)p.X);
              xz.BeginCalculations();
              AddSetToGlobalSet(TwoDIntoThreeDPoint(xz.GetCurve(), "xz", (int)p.Y));
              Interlocked.Increment(ref estimateCalculated);
              GiveFeedback(PercentComplete(), "sphere building");
              if(solid)
              {
                Parallel.ForEach(xz.GetCurve(), setCrashDependancy, solidP =>
                {
                  lock(lockingOb)
                  {
                    LineAlongPlaneGenerator(0, (int)solidP.X, "x", (int)p.Y, (int)solidP.Y);
                  }
                });
              }//RE-ENFORCEMENT by adding thickness
              else
              {
                Parallel.ForEach(xz.GetCurve(), setCrashDependancy, solidP =>
                {
                  lock(lockingOb)
                  {
                    LineAlongPlaneGenerator((int)solidP.X-1, (int)solidP.X, "x", (int)p.Y, (int)solidP.Y);
                  }
                });
              }
            }
          }
        }
        );
        xy=null;
      }
    }

    private void MakeQuarterEllipse()
    {
      if(LineAlongPlaneGenerator(0, radiusX, "x", 0, 0)&& LineAlongPlaneGenerator(0, radiusY, "y", 0, 0))
      {
        BresenhamEllipticalCurve xyCurve = new BresenhamEllipticalCurve(radiusX, radiusY);
        xyCurve.BeginCalculations();
        AddSetToGlobalSet(TwoDIntoThreeDPoint(xyCurve.GetCurve(), "xy", 0));
        Interlocked.Increment(ref estimateCalculated);
        GiveFeedback(PercentComplete(), "base ellipse calculated");
        if(solid)
        {
          Parallel.ForEach(xyCurve.GetCurve(), setCrashDependancy, p =>
          {
            lock(lockingOb)
            {
              LineAlongPlaneGenerator(0, (int)p.X, "x", (int)p.Y, 0);
            }
          });
        }
        else//RE-ENFORCEMENT by adding thickness
        {
          Parallel.ForEach(xyCurve.GetCurve(), setCrashDependancy, p =>
          {
            lock(lockingOb)
            {
              int thickness=(int)p.X-2;
              if(thickness>0)
              {
                LineAlongPlaneGenerator(thickness, (int)p.X, "x", (int)p.Y, 0);
              }
            }
          });
        }
        xyCurve=null;
      }
    }

    private void MakeQuarterCircle()
    {
      if(LineAlongPlaneGenerator(0, radiusX, "x", 0, 0)&&LineAlongPlaneGenerator(0, radiusX, "y", 0, 0))
      {
        BresenhamCircularCurve xyCurve= new BresenhamCircularCurve(radiusX);
        xyCurve.BeginCalculations();
        AddSetToGlobalSet(TwoDIntoThreeDPoint(xyCurve.GetCurve(), "xy", 0));
        Interlocked.Increment(ref estimateCalculated);
        GiveFeedback(PercentComplete(), "\nBase Circle calculated\n");
        if(solid)
        {
          Parallel.ForEach(xyCurve.GetCurve(), setCrashDependancy, p =>
          {
            lock(lockingOb)
            {
              LineAlongPlaneGenerator(0, (int)p.X, "x", (int)p.Y, 0);
            }
          });
        }
        else//RE-ENFORCEMENT by adding thickness
        {
          Parallel.ForEach(xyCurve.GetCurve(), setCrashDependancy, p =>
          {
            lock(lockingOb)
            {
              int thickness=(int)p.X-2;
              if(thickness>0)
              {
                LineAlongPlaneGenerator(thickness, (int)p.X, "x", (int)p.Y, 0);
              }
            }
          });
        }
        xyCurve=null;
      }
    }

    private void GiveFeedback(double number, string message)
    {
      if(progress!=null)
        progress.Report(new MyTaskProgressReporter() { ProgressCounter= number, ProgressMessage=message });
    }

    private double PercentComplete()
    {
      return ((estimateCalculated/estimateTotalCalculations)*100);
    }

    /// <summary>
    /// the only way to generate negative co-ordinates
    /// </summary>
    /// <param name="axis"></param>
    private void MirrorGlobalSetByAxis(string axis)
    {
      //   Object sillyLock = new Object();

      estimateCalculated=0;
      estimateTotalCalculations =myThreadSafeData.Count;
      ConcurrentBag<Point3D> tempSet= new ConcurrentBag<Point3D>();
      switch(axis)
      {
        case "x":
          {
            Parallel.ForEach(myThreadSafeData, setCrashDependancy, ppp =>
            {
              lock(lockingOb)
              {
                Interlocked.Increment(ref estimateCalculated);
                GiveFeedback(PercentComplete(), null);
                tempSet.Add(new Point3D(-1*ppp.X, ppp.Y, ppp.Z));
              }
            }
            );

            break;
          }
        case "y":
          {
            Parallel.ForEach(myThreadSafeData, setCrashDependancy, ppp =>
            {
              lock(lockingOb)
              {
                estimateCalculated++;
                GiveFeedback(PercentComplete(), null);
                tempSet.Add(new Point3D(ppp.X, -1*ppp.Y, ppp.Z));
              }
            }
            );
            break;
          }
        case "z":
          {
            Parallel.ForEach(myThreadSafeData, setCrashDependancy, ppp =>
            {
              lock(lockingOb)
              {
                estimateCalculated++;
                GiveFeedback(PercentComplete(), null);
                tempSet.Add(new Point3D(ppp.X, ppp.Y, -1*ppp.Z));
              }
            }
            );
            break;
          }
      }
      AddSetToGlobalSet(tempSet);
      tempSet=null;
    }

    /// <summary>
    /// takes a Hashset<Point3D> and adds to global collection
    /// </summary>
    /// <param name="anotherTempSet"></param>
    private void AddSetToGlobalSet(ConcurrentBag<Point3D> anotherTempSet)
    {

      //   Object sillyLock =new Object();
      Parallel.ForEach(anotherTempSet, setCrashDependancy, ptd =>
      //foreach(Point3D ptd in anotherTempSet)
      {
        lock(lockingOb)
        {
          AddNewPointToGlobalSet(ptd);
        }
      });
    }

    /// <summary>
    /// takes a collection of 2d points and converts them to 3d points by specifing a new coordinate plane, a level within that plane and 
    /// </summary>
    /// <param name="twoDPointCollection"></param>
    /// <param name="plane"></param>
    /// <param name="fixedPlaneValue"></param>
    /// <returns></returns>
    private ConcurrentBag<Point3D> TwoDIntoThreeDPoint(HashSet<Point> twoDPointCollection, string plane, int fixedPlaneValue)
    {

      //  Object sillyLock = new Object();
      ConcurrentBag<Point3D> temporaryCollection = new ConcurrentBag<Point3D>();
      switch(plane)
      {
        case "xy":
          {
            Parallel.ForEach(twoDPointCollection, setCrashDependancy, twoDPoint =>
            {
              lock(lockingOb)
              {
                temporaryCollection.Add(new Point3D(twoDPoint.X, twoDPoint.Y, fixedPlaneValue));
              }
            });
            break;
          }
        case "xz":
          {
            Parallel.ForEach(twoDPointCollection, twoDPoint =>
            {
              lock(lockingOb)
              {
                temporaryCollection.Add(new Point3D(twoDPoint.X, fixedPlaneValue, twoDPoint.Y));
              }
            });
            break;
          }
        case "yz":
          {
            Parallel.ForEach(twoDPointCollection, twoDPoint =>
            {
              lock(lockingOb)
              {
                temporaryCollection.Add(new Point3D(fixedPlaneValue, twoDPoint.X, twoDPoint.Y));
              }
            });
            break;
          }
      }
      return temporaryCollection;
    }


    private void AddNewPointToGlobalSet(Point3D pointToAdd)
    {
      //  if(!myThreadSafeData..Contains(summonThirdDimension))
      //   {
      myThreadSafeData.Enqueue(pointToAdd);
      //}

    }

    /// <summary>
    /// Plots a line given a start point smaller than a positive end point, along a single plane and Point3D connection
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="axis"></param>
    /// <param name="yx"></param>
    /// <param name="zy"></param>
    /// <returns></returns>
    private bool LineAlongPlaneGenerator(int start, int end, string axis, int yx, int zy)
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
        return true;
      }
      return false;
    }

    #region disposal

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    ~PointsToShape()
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
