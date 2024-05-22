import matplotlib.pyplot as plt
import pandas as pd
from io import StringIO


data = """
l,n,mmp,msh
1,5000000,1.085,0.172
2,5000000,0.973,0.171
3,5000000,0.953,0.183
4,5000000,1.021,0.19
5,5000000,1.019,0.214
6,5000000,1.007,0.19
7,5000000,1.021,0.191
8,5000000,1.028,0.18
9,5000000,1.056,0.189
10,5000000,1.127,0.21
11,5000000,1.144,0.207
12,5000000,1.175,0.216
13,5000000,1.162,0.236
14,5000000,1.141,0.238
15,5000000,1.183,0.288
16,5000000,1.25,0.303
17,5000000,1.34,0.435
18,5000000,1.704,0.811
19,5000000,2.221,1.154
20,5000000,3.097,1.713
21,5000000,5.113,2.161
22,5000000,10.271,4.226
23,5000000,14.207,5.466
24,5000000,17.591,9.32
"""


df = pd.read_csv(StringIO(data))
print(df)
plt.figure(figsize=(10, 6))
plt.plot(df['l'], df['mmp'], label='Multiply Mod Prime', marker='o')
plt.plot(df['l'], df['msh'], label='Multiply Shift Hash', marker='s')
plt.xlabel('l')
plt.ylabel('time [s]')
plt.yscale('log', base=2)
plt.title(f'Time to execute MMP and MSH with n={df['n'][0]}')
plt.legend()
plt.grid(True)
plt.show()
