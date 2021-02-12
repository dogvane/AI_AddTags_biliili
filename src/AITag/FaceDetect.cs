using AITag.Common;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITag
{
    /// <summary>
    /// 脸部检测
    /// </summary>
    public class FaceDetect
    {

        public static Rect[] OpenCvDetectMultiScale(Mat mat)
        {
            var cc = CascadeClassifierManager.Load("haarcascade_frontalface_alt2.xml");

            return cc.DetectMultiScale(mat);
        }



    }
}
