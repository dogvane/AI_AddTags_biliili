using System;
using System.Threading;
using OpenCvSharp;

namespace OpenCVDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var img = Cv2.ImRead(@"V:\Data\Images\i1_hdslb_com\bfs\archive\0a2bb74cd07054c79151f3a4bf0322f42b8d3899.jpg");
            Cv2.ImShow("first", img);
            
            Cv2.WaitKey();
        }
    }

    /**
     * 需要增加的测试用例
     * 图片的切割，拉伸，旋转
     * 
     * 图片的特效处理
     * 
     * 内置的AI的处理方法
     * 
     * 
     * 
     */
}
