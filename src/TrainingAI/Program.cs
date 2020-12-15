using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Text;
using BilibiliSpider.Common;
using Newtonsoft.Json;
using NumSharp;
using OpenCvSharp;
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
            Console.WriteLine("Hello World!");
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
             optimizer: keras.optimizers.RMSprop(),
             metrics: new[] { "accuracy" });

            model.fit(config.Images, config.Lables, epochs: 2);
            
        }
    }
}
