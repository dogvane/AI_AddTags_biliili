using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using OpenCvSharp;

namespace AITag.Common
{
    class CascadeClassifierManager
    {
        private static string[] xmlFolder = new[]
        {
            "",
            "data/haarcascades/",
            "../data/haarcascades/",
            "../../data/haarcascades/",
            "../../../data/haarcascades/",
            "../../../../data/haarcascades/",
        };

        private static Dictionary<string, CascadeClassifier> map = new Dictionary<string, CascadeClassifier>();

        public static CascadeClassifier Load(string fileName)
        {
            if (map.TryGetValue(fileName, out CascadeClassifier ret))
                return ret;

            foreach (var p in xmlFolder)
            {
                var name = p + fileName;

                if (File.Exists(name))
                {
                    fileName = name;
                    break;
                }
            }

            var ret2 = new CascadeClassifier(fileName);
            map[fileName] = ret2;

            return ret2;
        }
    }
}
