
import tensorflow as tf

from tensorflow.keras import datasets, layers, models
import matplotlib.pyplot as plt

import numpy as np

import os

# os.environ["CUDA_VISIBLE_DEVICES"] = "-1"

class BilibiliLabData:
    def __init__(self):
        self.num = 0
        self.width = 56
        self.height = 128
        self.labelNames = []
        self.images = []
        self.lables = []

def loadConfigData(fileName = ''):
    if fileName == '':
        fileName = 'V:/Data/TrainData/config.txt'
    
    ret = BilibiliLabData()

    with open(fileName, encoding='utf-8') as f2:
        num = f2.readline();
        width = f2.readline();
        height = f2.readline();
        labNames = f2.readlines();

        ret.width = int(width)
        ret.height = int(height)
        ret.num = int(num)
        ret.labelNames = labNames

        fileName2 = 'V:/Data/TrainData/image.mat'

        with open(fileName2, 'rb') as fi:
            data = fi.read()
            train_images = np.frombuffer(data, np.uint8)

            train_images = train_images.reshape(ret.num, ret.height, ret.width, 3)            
            ret.images = train_images

        fileName3 = 'V:/Data/TrainData/lable.mat'

        with open(fileName3, 'rb') as f3:
            data = f3.read()
            train_lables = np.frombuffer(data, np.uint8)
            train_lables = train_lables.reshape(ret.num, 1)
            ret.lables = train_lables
        
        return ret

config = loadConfigData()

print(config.num)
print(config.labelNames)
print(config.images.shape)
print(config.lables.shape)
print(config.lables[1])

# 将像素的值标准化至0到1的区间内。
config.images = config.images / 255.0

model = models.Sequential()
model.add(layers.Conv2D(32, (3, 3), activation='relu', input_shape=(config.height, config.width, 3)))
model.add(layers.MaxPooling2D((2, 2)))
model.add(layers.Conv2D(64, (3, 3), activation='relu'))
model.add(layers.MaxPooling2D((2, 2)))
model.add(layers.Conv2D(64, (3, 3), activation='relu'))
model.add(layers.MaxPooling2D((2, 2)))
model.add(layers.Conv2D(64, (3, 3), activation='relu'))

model.summary()

model.add(layers.Flatten())
model.add(layers.Dense(64, activation='relu'))
model.add(layers.Dense(len(config.labelNames)))

model.summary()

model.compile(optimizer='adam',
              loss=tf.keras.losses.SparseCategoricalCrossentropy(from_logits=True),
              metrics=['accuracy'])

history = model.fit(config.images, config.lables, epochs=100, validation_data=(config.images, config.lables))