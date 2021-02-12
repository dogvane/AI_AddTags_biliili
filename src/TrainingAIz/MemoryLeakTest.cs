using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tensorflow.Keras.Layers;
using NumSharp;
using Tensorflow.Keras;
using static Tensorflow.Binding;
using static Tensorflow.KerasApi;

namespace TrainingAI
{
    class MemoryLeakTest
    {
        protected static LayersApi layers = new LayersApi();

        public static void Test()
        {
            int num = 50, width = 128, height = 128;

            var bytes = new byte[num * width * height * 3];
            var inputImages = np.array(bytes) / 255.0f;
            inputImages = inputImages.reshape(num, height, width, 3);

            bytes = new byte[num];
            var outLables = np.array(bytes);
            Console.WriteLine("Image.Shape={0}", inputImages.Shape);
            Console.WriteLine("Label.Shape={0}", outLables.Shape);

            tf.enable_eager_execution();

            // 宽*高*通道数
            var inputss = keras.Input((height, width, 3));

            var inputs = layers.Conv2D(32, (3, 3), activation: keras.activations.Relu).Apply(inputss);
            inputs = layers.MaxPooling2D((2, 2)).Apply(inputs);

            inputs = layers.Flatten().Apply(inputs);

            var outputs = layers.Dense(10).Apply(inputs);

            var model = keras.Model(inputss, outputs, "gpuleak");

            model.summary();

            model.compile(loss: keras.losses.SparseCategoricalCrossentropy(from_logits: true),
             optimizer: keras.optimizers.RMSprop(),
             metrics: new[] { "accuracy" });

            model.fit(inputImages, outLables, epochs: 200);

        }
    }
}
