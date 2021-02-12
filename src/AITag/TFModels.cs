using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using Newtonsoft.Json;
using NumSharp;
using Tensorflow;
using Tensorflow.Keras;
using Tensorflow.Keras.Layers;
using static Tensorflow.Binding;
using static Tensorflow.KerasApi;
using Tensorflow.Keras.Engine;

namespace AITag
{
    /// <summary>
    /// 用来放公共的TensorFlow的模型用
    /// </summary>
    public class TFModels
    {
        protected static LayersApi layers = new LayersApi();

        public static Functional GetBilibiliModelV1(int width, int height, int labelNameLenght)
        {
            tf.enable_eager_execution();

            // 宽*高*通道数
            var inputss = keras.Input((height, width, 3));

            var inputs = layers.Conv2D(32, (3, 3), activation: keras.activations.Relu).Apply(inputss);
            inputs = layers.MaxPooling2D((2, 2)).Apply(inputs);
            inputs = layers.Conv2D(64, (3, 3), activation: keras.activations.Relu).Apply(inputs);
            inputs = layers.MaxPooling2D((2, 2)).Apply(inputs);
            inputs = layers.Conv2D(64, (3, 3), activation: keras.activations.Relu).Apply(inputs);
            inputs = layers.MaxPooling2D((2, 2)).Apply(inputs);
            inputs = layers.Conv2D(64, (3, 3), activation: keras.activations.Relu).Apply(inputs);
            inputs = layers.MaxPooling2D((1, 1)).Apply(inputs);
            inputs = layers.Conv2D(64, (2, 2), activation: keras.activations.Relu).Apply(inputs);
            inputs = layers.MaxPooling2D((1, 1)).Apply(inputs);

            inputs = layers.Flatten().Apply(inputs);

            inputs = layers.Dense(64, keras.activations.Relu).Apply(inputs);
            var outputs = layers.Dense(labelNameLenght).Apply(inputs);

            var model = keras.Model(inputss, outputs, "bilibili");

            model.summary();

            model.compile(loss: keras.losses.SparseCategoricalCrossentropy(from_logits: true),
             optimizer: keras.optimizers.Adam(),
             metrics: new[] { "accuracy" });

            return model;
        }
    }
}
