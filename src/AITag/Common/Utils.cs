using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITag.Common
{
    /// <summary>
    /// 辅助工具类
    /// </summary>
    public static class Utils
    {

        /// <summary>
        /// 根据脸部的尺寸，推到获得身体需要的尺寸
        /// </summary>
        /// <param name="faceRect">opencv或者其它工具截取出来的人脸的范围框</param>
        /// <param name="rate">需要截取身体部分的宽高比 w/h </param>
        /// <param name="picWidth">输入图片的宽度</param>
        /// <param name="picHeight">输入图片高度</param>
        /// <returns></returns>
        public static Rect GetBodyRect(Rect faceRect, double rate, int picWidth, int picHeight)
        {
            // 中心点
            var middle = new { x = faceRect.Left + faceRect.Width / 2, y = faceRect.Top + faceRect.Height / 2 };

            // 脸的大小（中点的半径)
            var r = Math.Min(faceRect.Height, faceRect.Width) / 2;

            var height = r * 17; // 八头身
            var width = (int)(height * rate); // 截取长宽比是 1:2

            var left = middle.x - (int)width / 2;
            var top = middle.y - r * 2;

            var ret = new Rect(left, top, width, height);

            // 判断body是否能满足截取需求

            if (ret.Top > 0 &&
                ret.Left > 0 &&
                ret.Top + height < picHeight &&
                ret.Left + width < picWidth)
            {
                // 矩形刚好再图片范围内
                return ret;
            }

            // 如果不在范围内，则尝试缩小一下范围，看看能不能放下
            var wr = 0.0;
            var hr = 0.0;

            if (ret.Top < 0)
            {
                hr = (double)-(ret.Top * 2) / height;
            }

            if (ret.Bottom > picHeight)
            {
                hr = (double)((ret.Bottom - picHeight) * 2) / height;
            }

            if (ret.Left < 0)
            {
                wr = (double)-(ret.Left * 2) / height;
            }

            if (ret.Right > picWidth)
            {
                wr = (double)((ret.Right - picWidth) * 2) / width;
            }

            var maxR = Math.Max(hr, wr);

            if (maxR < 0.3)
            {
                // 变动不超过2层，可以进行截取操作，否则先无视
                var nHeight = (int)(height * (1 - maxR));
                var nWidth = (int)(height * rate);
                var ret2 = new Rect(ret.X + ((width - nWidth) / 2), ret.Y + Math.Min(((height - nHeight) / 2), r), nWidth, nHeight);
                Console.WriteLine($"{ret} -> {ret2}");

                if (ret2.Top > 0 &&
                    ret2.Left > 0 &&
                    ret2.Top + height < picHeight &&
                    ret2.Left + width < picWidth)
                {
                    // 矩形刚好再图片范围内
                    return ret2;
                }
            }

            return Rect.Empty;
        }
    }
}
