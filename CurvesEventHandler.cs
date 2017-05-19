using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace SoloProjects.Dudhit.Utilities.Curves
{
  public class DetermineProcessingPathsCompletedEventArgs :System.ComponentModel.AsyncCompletedEventArgs
  {
    public HashSet<Point3D> Result { get; set; }

    public DetermineProcessingPathsCompletedEventArgs(Exception error, bool cancelled, object userState)
            : base(error, cancelled, userState)
        {

        }

  }
  public class ProcessingProgressEventArgs : System.ComponentModel.AsyncCompletedEventArgs
  {
    public double Result { get; set; }
    public ProcessingProgressEventArgs(Exception error, bool cancelled, object userState)
      : base(error, cancelled, userState)
    { }
  }
}
