import onnx
import torch
import math
import onnxruntime as ort

def clamp(num, min_value, max_value):
   return max(min(num, max_value), min_value)

def Euclidean(p, q):
    return math.sqrt(math.pow(p[0]-q[0],2) + math.pow(p[1]-q[1],2) + math.pow(50.0,2))

p_onnx_path = "../Models/Power.onnx"
w_onnx_path = "../Models/Bandwidth.onnx"
l_onnx_path = "../Models/Trajectory.onnx"
s_onnx_path = "../Models/Single.onnx"

p_ort_sess = ort.InferenceSession(p_onnx_path)
l_ort_sess = ort.InferenceSession(l_onnx_path)
w_ort_sess = ort.InferenceSession(w_onnx_path)
s_ort_sess = ort.InferenceSession(s_onnx_path)

##############변수확인###################
g_0 = 10.0
sigma = 174
alpha = 0.7
p_0 = 4.0
w_0 = 30e5

USERS_NUM = 15
#########################################
UAV = [0.0, 0.0]
USERS = [
-37.42891475,-106.4047109,
-133.798474,-57.01415808,
-10.68834623,-186.2735145,
-61.8521337,-36.40829525,
-14.31677323,-0.42933087,
-122.3750467,-102.8869922,
-202.5040928,-184.0890737,
-76.9388034,-15.49461321,
-171.2865981,49.9982908,
-125.9028676,-61.64475816,
-99.20218329,7.925370377,
-187.8524823,-143.6474974,
-136.6790179,48.36043824,
-119.1335981,100.8984269,
-222.245201,-68.31973042
]
POWER = [ 1/USERS_NUM for i in range(USERS_NUM)]
BAND = [ 1/USERS_NUM for i in range(USERS_NUM)]
cent = [-114.8136355, -50.36200989]

def GetDataRate():
   sum = 0
   for i in range(USERS_NUM):
      dist = Euclidean(UAV, [USERS[2*i], USERS[2*i + 1]])
      gain = g_0 / pow(dist, alpha)
      sinr = ( POWER[i] * p_0 * gain) / sigma
      sum += BAND[i] * w_0 * math.log(1 + sinr)
   return sum/USERS_NUM

DATERATE = []
DATERATE.append(GetDataRate())
print(GetDataRate())
for e in range(100):

    l_inputs =[[]]
    for i in range(USERS_NUM):
        l_inputs[0].append(UAV[0] - USERS[2*i])
        l_inputs[0].append(UAV[1] - USERS[2*i + 1])

    wp_inputs = [[]]
    for i in range(USERS_NUM):
        wp_inputs[0].append(Euclidean(UAV, [USERS[2*i], USERS[2*i + 1]]))
    

    '''
    # Single Agent
    s_ort_sess = ort.InferenceSession(s_onnx_path)
    s_outputs = s_ort_sess.run(None, {'obs_0': l_inputs})
    s_out = [clamp(s_outputs[2][0][0], -1.0, 1.0), clamp(s_outputs[2][0][1], -1.0, 1.0)]
    
    for i in range(USERS_NUM):
        BAND[i] = (1 / (2 * USERS_NUM)) * clamp(s_outputs[2][0][2*i + 2], -1.0, 1.0) + (1 / USERS_NUM)
        POWER[i] = (1 / (2 * USERS_NUM)) * clamp(s_outputs[2][0][2*i + 3], -1.0, 1.0) + (1 / USERS_NUM)
        
    UAV[0] += s_out[0] * 6.0
    UAV[1] += s_out[1] * 6.0
    '''
    
    '''
    # L Agent
    l_ort_sess = ort.InferenceSession(l_onnx_path)
    l_outputs = l_ort_sess.run(None, {'obs_0': l_inputs})
    l_out = [clamp(l_outputs[2][0][0], -1.0, 1.0), clamp(l_outputs[2][0][1], -1.0, 1.0)]
    UAV[0] += l_out[0] * 6.0
    UAV[1] += l_out[1] * 6.0
    '''

    # for onlyRSC
    UAV[0] = e * (cent[0]/100)
    UAV[1] = e * (cent[1]/100)
    # print(UAV)

    # W Agent
    w_ort_sess = ort.InferenceSession(w_onnx_path)
    w_outputs = w_ort_sess.run(None, {'obs_0': wp_inputs})
    for i in range(10):
        BAND[i] = (1 / (2 * USERS_NUM)) * clamp(w_outputs[2][0][i], -1.0, 1.0) + (1 / USERS_NUM)
    # P Agent
    p_ort_sess = ort.InferenceSession(p_onnx_path)
    p_outputs = p_ort_sess.run(None, {'obs_0': wp_inputs})
    for i in range(10):
        POWER[i] = (1 / (2 * USERS_NUM)) * clamp(w_outputs[2][0][i], -1.0, 1.0) + (1 / USERS_NUM)
    DATERATE.append(GetDataRate())
    print(GetDataRate())



