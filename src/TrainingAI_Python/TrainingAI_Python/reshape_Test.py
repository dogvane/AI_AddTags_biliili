import numpy as np

arr1 = [1,1,1,2,2,2,3,3,3,4,4,4,5,5,5,6,6,6]

ar2 = np.array(arr1)
print(ar2)

ar3 = ar2.reshape(3,2,3)

print(ar3)
print(ar3[0,0,0])
print(ar3[1,0,1])

