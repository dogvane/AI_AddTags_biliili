using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Text;
using BilibiliSpider.Common;
using NumSharp;
using OpenCvSharp;
using Tensorflow.Keras.Layers;
using static Tensorflow.Binding;
using static Tensorflow.KerasApi;


namespace TrainingAI
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            // ReadTrainData();
            // startTF();
            fashion_mnistTest();

        }

        static void startTF()
        {
            var graph = tf.get_default_graph();
            tf.reset_default_graph();
            Console.WriteLine(tf.VERSION);
            
            
            var sess = tf.Session();

            var a = tf.Variable(1, name: "a");
            var z = tf.Variable(2, name: "z");
            var z2 = tf.add(a, z);

            var b = tf.add(a, 1, name: "b");
            var c = tf.multiply(b, 4, name: "c");
            var d = tf.subtract(c, b, name: "d");
            sess.run(d);

        }

        protected static LayersApi layers = new LayersApi();


        static void fashion_mnistTest()
        {
            tf.enable_eager_execution();

            var fileNames = new string[]
            {
                "train-labels-idx1-ubyte.gz", "train-images-idx3-ubyte.gz",
                "t10k-labels-idx1-ubyte.gz", "t10k-images-idx3-ubyte.gz"
            };

            var train_labels = np.array(ReadBytes(fileNames[0], 8));
            var train_images = np.array(ReadBytes(fileNames[1], 16));
            var test_labels = np.array(ReadBytes(fileNames[2], 8));
            var test_images = np.array(ReadBytes(fileNames[3], 16));

            // 转为单通道 28*28 的图片数据
            train_images = train_images.reshape(train_labels.shape[0], 28 * 28);
            test_images = test_images.reshape(test_labels.shape[0], 28 * 28);

            Console.WriteLine(train_labels.Shape);
            Console.WriteLine(train_images.Shape); ;

            train_images = train_images / 255.0f;
            test_images = test_images / 255.0f;

            var inputs = keras.Input(shape: 28 * 28);

            var outputs = layers.Dense(128, keras.activations.Relu).Apply(inputs);

            outputs = layers.Dense(10).Apply(outputs);

            var model = keras.Model(inputs, outputs, name: "fmms");

            model.summary();

            model.compile(loss: keras.losses.SparseCategoricalCrossentropy(from_logits: true),
                optimizer: keras.optimizers.RMSprop(),
                metrics: new[] { "accuracy" });

            model.fit(train_images, train_labels, epochs:2);

            Console.WriteLine("finish");

            // model.evaluate(test_images, test_labels);

            //model.save("fmm_model");
            //var p1 = model.predict(test_images[0]);
            //Console.WriteLine(p1.shape);
        }

        static void fashion_mnistTest2()
        {
            var fileNames = new string[]
            {
                "train-labels-idx1-ubyte.gz", "train-images-idx3-ubyte.gz",
                "t10k-labels-idx1-ubyte.gz", "t10k-images-idx3-ubyte.gz"
            };

            var train_labels = np.array(ReadBytes(fileNames[0], 8));
            var train_images = np.array(ReadBytes(fileNames[1], 16));
            var test_labels = np.array(ReadBytes(fileNames[2], 8));
            var test_images = np.array(ReadBytes(fileNames[3], 16));

            // 转为单通道 28*28 的图片数据
            train_images = train_images.reshape(train_labels.shape[0], 28, 28);
            test_images = test_images.reshape(test_labels.shape[0], 28, 28);

            Console.WriteLine(train_labels.Shape);
            Console.WriteLine(train_images.Shape); ;

            train_images = train_images / 255.0;
            test_images = test_images / 255.0;
            //var model = keras.Sequential(
            //    new List<Tensorflow.Keras.ILayer>
            //    {
            //        keras.layers.Flatten("28,28"),
            //        keras.layers.Dense(128, keras.activations.Relu),
            //        keras.layers.Dense(10)
            //    }
            //);

            // Sequential 方法尚未完善 先使用 Functional 方法来实现

            //model.compile(keras.losses.SparseCategoricalCrossentropy(true), keras.optimizers.Adam(), new []{ "accuracy" });
            //model.fit(train_images, train_labels, 1);
        }

        private static byte[] ReadBytes(string fileName, int offSet = 0)
        {

            var path = Path.Combine(Utils.DefaultDataFolder, "fashion-mnist/" + fileName);
            using var stream1 = File.OpenRead(path);
            var gzip = new GZipStream(stream1, CompressionMode.Decompress);
            var ms = new MemoryStream();
            gzip.CopyTo(ms);

            // y_train = np.frombuffer(lbpath.read(), np.uint8, offset=8)
            // py的代码在这里读取数据是有一个 offset，在c#的初始化代码里没有这个偏移的参数
            // 所以这里读取的时候，在字节流读取阶段就放弃了这前8个byte字节
            var bytes = new byte[ms.Length - offSet];
            ms.Position = offSet;
            ms.Read(bytes);

            Console.WriteLine($"{fileName} streamLen:{ms.Length} byteLen:{bytes.Length}");

            return bytes;
        }

        /// <summary>
        /// 读取训练用的数据
        /// </summary>
        unsafe static void ReadTrainData()
        {
            var filePath = Path.Combine(Utils.DefaultDataFolder, "TrainData/bilibili.tags");

            var stream = File.OpenRead(filePath);
            byte[] intBuffer = new byte[4];

            stream.Read(intBuffer);
            var picNum = BitConverter.ToInt32(intBuffer);

            stream.Read(intBuffer);
            var width = BitConverter.ToInt32(intBuffer);

            stream.Read(intBuffer);
            var height = BitConverter.ToInt32(intBuffer);

            stream.Read(intBuffer);
            var channels = BitConverter.ToInt32(intBuffer);

            var buffer = new byte[width * height * channels];
            var v3 = MemoryMarshal.AsRef<Vec3b>(buffer);

            for (var i = 0; i < picNum; i++)
            {
                stream.Read(buffer);
                var mat = new Mat(height, width, MatType.CV_8UC3, buffer);

                // mat.SaveImage($"r:/t/{i}.png"); 输出图片demo
            }

            // 图片标签数量：int
            // 图片标签数组：图片数量*图片标签数量(多标签的处理,1表示带有对应的tag)
            // 图片标签名称：utf8字符串剩下的直接，出字符串后，用逗号分隔

            stream.Read(intBuffer);
            var tagNum = BitConverter.ToInt32(intBuffer);

            List<byte[]> imageTags = new List<byte[]>();

            for (var i = 0; i < picNum; i++)
            {
                var tagbuffer = new byte[tagNum];
                stream.Read(tagbuffer);
                imageTags.Add(tagbuffer);
            }

            var lessLen = stream.Length - stream.Position;
            var strBuffer = new byte[lessLen];
            stream.Read(strBuffer);
            string names = Encoding.UTF8.GetString(strBuffer);
            Console.WriteLine(names);
        }
    }
}
