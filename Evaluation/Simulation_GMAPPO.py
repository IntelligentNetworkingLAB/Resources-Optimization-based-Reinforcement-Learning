import onnx
import torch
import math
import onnxruntime as ort

def clamp(num, min_value, max_value):
   return max(min(num, max_value), min_value)

def Euclidean(p, q):
    return math.sqrt(math.pow(p[0]-q[0],2) + math.pow(p[1]-q[1],2) + math.pow(50.0,2))

p_onnx_path = "Models/PAgent.onnx"
l_onnx_path = "Models/My Behavior2.onnx"
w_onnx_path = "Models/WAgent.onnx"

p_ort_sess = ort.InferenceSession(p_onnx_path)
l_ort_sess = ort.InferenceSession(l_onnx_path)
w_ort_sess = ort.InferenceSession(w_onnx_path)

##############변수확인###################
g_0 = 10.0
sigma = 174
alpha = 0.7
p_0 = 4.0
w_0 = 30e3

USER_NUM = 45
UAV_NUM = 3

#########################################
UAV = [[0.0, 0.0] for i in range(UAV_NUM)]
USERS = [-157.8, -177.7, -117.5, -271.1, -178.6, -101.0, -140.1, -136.2, -250.1, -246.7, -106.0, -145.3, -141.3, -156.4, -228.8, -261.4, -197.4, -281.9, -210.5, -264.8]
POWER = [ p_0/10.0 for i in range(10)]
BAND = [ w_0/10.0 for i in range(10)]

def GetDataRate():
   sum = 0
   for i in range(10):
      dist = Euclidean(UAV, [USERS[2*i], USERS[2*i + 1]])
      gain = g_0 / pow(dist, alpha)
      sinr = ( (POWER[i]) * gain) / sigma
      sum += (BAND[i])* math.log2(1 + sinr)
   return sum

def ConstGetDataRate():
   sum = 0
   for i in range(10):
      dist = Euclidean(UAV, [USERS[2*i], USERS[2*i + 1]])
      gain = g_0 / pow(dist, alpha)
      sinr = ( (p_0/10.0) * gain) / sigma
      sum += (w_0/10.0)* math.log2(1 + sinr)
   return sum   

l_inputs =[[]]
l_inputs[0].append(UAV[0])
l_inputs[0].append(UAV[1])
for i in range(20):
   l_inputs[0].append(USERS[i])

DATERATE = []
DATERATE.append(GetDataRate())
print(GetDataRate())
for i in range(100):
   # L Agent
   l_ort_sess = ort.InferenceSession(l_onnx_path)
   l_outputs = l_ort_sess.run(None, {'obs_0': l_inputs})
   l_out = [clamp(l_outputs[2][0][1], -1.0, 1.0), clamp(l_outputs[2][0][0], -1.0, 1.0)]
   l_inputs[0][0] += l_out[0] * 6.0
   l_inputs[0][1] += l_out[1] * 6.0
   
   UAV[0] = l_inputs[0][0]
   UAV[1] = l_inputs[0][1]

   w_p_inputs =[[]]
   for i in range(10):
      w_p_inputs[0].append(Euclidean(UAV, [USERS[2*i], USERS[2*i + 1]]))

   # W Agent
   w_ort_sess = ort.InferenceSession(w_onnx_path)
   w_outputs = w_ort_sess.run(None, {'obs_0': w_p_inputs})
   for i in range(10):
      BAND[i] = 0.05 * w_0 * clamp(w_outputs[2][0][i], -1.0, 1.0) + 0.1 * w_0

   # P Agent
   p_ort_sess = ort.InferenceSession(p_onnx_path)
   p_outputs = p_ort_sess.run(None, {'obs_0': w_p_inputs})
   for i in range(10):
      POWER[i] = 0.05 * p_0 * clamp(p_outputs[2][0][i], -1.0, 1.0) + 0.1 * p_0
   
   DATERATE.append(GetDataRate())
   print(GetDataRate())



