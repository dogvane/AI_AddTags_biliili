using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Text;
using BilibiliSpider.Common;
using Newtonsoft.Json;
using NumSharp;
using OpenCvSharp;
using Tensorflow;
using Tensorflow.Keras;
using Tensorflow.Keras.Layers;
using static Tensorflow.Binding;
using static Tensorflow.KerasApi;


namespace TrainingAI
{
    class Program
    {
        protected static LayersApi layers = new LayersApi();

        static void Main(string[] args)
        {
            //MemoryLeakTest.Test();
            //return;
            var x = new string[] { "", "", "", "", "", "" };
            var y = x[1..];

            Console.WriteLine("Hello World!");
            LoadAndReStart();
        }
        private static void LoadAndReStart()
        {
            var b = new bilibili_cnn();
            var config = b.ReadDataSets();
            Console.WriteLine(JsonConvert.SerializeObject(config, Formatting.Indented));
            Console.WriteLine("Image.Shape={0}", config.Images.Shape);
            Tensor one = config.Images["2:3"];

            Console.WriteLine("Label.Shape={0}", config.Lables.Shape);
            
            tf.enable_eager_execution();

            // 宽*高*通道数
            var inputss = keras.Input((config.Height, config.Width, 3));

            var inputs = layers.Conv2D(32, (3, 3), activation: keras.activations.Relu).Apply(inputss);
            inputs = layers.MaxPooling2D((2, 2)).Apply(inputs);

            inputs = layers.Conv2D(64, (1, 1), activation: keras.activations.Relu).Apply(inputs);
            inputs = layers.MaxPooling2D((1, 1)).Apply(inputs);
            inputs = layers.Conv2D(64, (3, 3), activation: keras.activations.Relu).Apply(inputs);
            inputs = layers.MaxPooling2D((2, 2)).Apply(inputs);
            inputs = layers.Conv2D(64, (3, 3), activation: keras.activations.Relu).Apply(inputs);
            inputs = layers.MaxPooling2D((2, 2)).Apply(inputs);
            inputs = layers.Conv2D(64, (3, 3), activation: keras.activations.Relu).Apply(inputs);

            inputs = layers.MaxPooling2D((1, 1)).Apply(inputs);
            inputs = layers.Conv2D(64, (2, 2), activation: keras.activations.Relu).Apply(inputs);
            inputs = layers.MaxPooling2D((1, 1)).Apply(inputs);
            inputs = layers.Conv2D(64, (1, 1), activation: keras.activations.Relu).Apply(inputs);
            inputs = layers.MaxPooling2D((1, 1)).Apply(inputs);

            inputs = layers.Flatten().Apply(inputs);

            inputs = layers.Dense(64, keras.activations.Relu).Apply(inputs);
            var outputs = layers.Dense(config.LabNames.Length).Apply(inputs);

            var model = keras.Model(inputss, outputs, "bilibili");

            model.summary();

            model.compile(loss: keras.losses.SparseCategoricalCrossentropy(from_logits: true),
             optimizer: keras.optimizers.Adam(),
             metrics: new[] { "accuracy" });

            var modelFile = @"V:\Data\TrainData\bilibili.h5";
            if (File.Exists(modelFile))
            {
                try
                {
                    model.load_weights(modelFile);

                    var resultTest = model.predict(one);
                    var maxTest = tf.argmax(resultTest[0], 1);

                    var numpy = resultTest[0].numpy();
                    var argMax = numpy.argmax();

                    Console.WriteLine(maxTest);
                    var labIndexTest = maxTest.numpy()[0];
                    Console.WriteLine(config.LabNames[labIndexTest]);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            var metrics = model.metrics.ToArray();

            Console.WriteLine(JsonConvert.SerializeObject(metrics));

            while (true)
            {
                model.fit(config.Images, config.Lables, epochs: 5);
                var metrices = model.metrics.ToArray();
                var loss = (float)metrices[0].result();
                var accuracy = (float)metrices[1].result();

                Console.WriteLine($"lost:{loss} accuracy:{accuracy}");

                model.save_weights(modelFile);

                if (accuracy > 0.95 && loss < 0.1)
                {
                    break;
                }
            }

            var result = model.predict(one);
            var max = tf.argmax(result[0], 1);

            Console.WriteLine(max);
            var labIndex = max.numpy()[0];
            Console.WriteLine(config.LabNames[labIndex]);
        }


        private static void StudyAndSave()
        {
            var b = new bilibili_cnn();
            var config = b.ReadDataSets();
            Console.WriteLine(JsonConvert.SerializeObject(config, Formatting.Indented));
            Console.WriteLine("Image.Shape={0}", config.Images.Shape);
            Console.WriteLine("Label.Shape={0}", config.Lables.Shape);

            tf.enable_eager_execution();

            // 宽*高*通道数
            var inputss = keras.Input((config.Height, config.Width, 3));

            var inputs = layers.Conv2D(32, (3, 3), activation: keras.activations.Relu).Apply(inputss);
            inputs = layers.MaxPooling2D((2, 2)).Apply(inputs);
            inputs = layers.Conv2D(64, (3, 3), activation: keras.activations.Relu).Apply(inputs);
            inputs = layers.MaxPooling2D((2, 2)).Apply(inputs);
            inputs = layers.Conv2D(64, (3, 3), activation: keras.activations.Relu).Apply(inputs);
            inputs = layers.MaxPooling2D((2, 2)).Apply(inputs);
            inputs = layers.Conv2D(64, (3, 3), activation: keras.activations.Relu).Apply(inputs);

            inputs = layers.Flatten().Apply(inputs);
            Console.WriteLine(inputs.shape);

            inputs = layers.Dense(64, keras.activations.Relu).Apply(inputs);
            var outputs = layers.Dense(config.LabNames.Length).Apply(inputs);

            var model = keras.Model(inputss, outputs, "bilibili");

            model.summary();

            model.compile(loss: keras.losses.SparseCategoricalCrossentropy(from_logits: true),
             optimizer: keras.optimizers.Adam(),
             metrics: new[] { "accuracy" });

            model.fit(config.Images, config.Lables, epochs: 150);
            model.save("11.h5");
        }
    }
}
