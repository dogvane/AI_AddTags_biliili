
import matplotlib.pyplot as plt
import numpy as np

fileName = 'V:/Data/TrainData/config.txt'
with open(fileName, encoding='utf-8') as f2:
    num = f2.readline();
    width = f2.readline();
    height = f2.readline();
    labNames = f2.readlines();

    print(num, int(width) * int(height))
    print(labNames);

    width2 = int(width)
    height2 = int(height)
    num2 = int(num)

    fileName2 = 'V:/Data/TrainData/image.mat'


    with open(fileName2, 'rb') as fi:
        data = fi.read()
        train_images = np.frombuffer(data, np.uint8)
        print('train_images')
        print(train_images)
        print(train_images.shape)
        print(num2* 3* width2* height2)
        train_images = train_images.reshape(num2, height2, width2, 3)
        # train_images2 = train_images[:,:, :, 0]
        train_images2 = train_images[12, :, :, :] # 切割第一张图片
        train_images2 = train_images2[...,::-1].copy() # bgr -> rgb
        print(train_images2.shape)
        plt.imshow(train_images2, cmap=plt.cm.binary)
        plt.show()
