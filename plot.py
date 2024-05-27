import matplotlib.pyplot as plt
import pandas as pd

def plot(file, title):
    df = pd.read_csv(file)
    plt.figure(figsize=(10, 6))
    plt.plot(df['l'], df['msh'], label='MSH: Multiply Shift Hash', marker='o')
    plt.plot(df['l'], df['mmp'], label='MMP: Multiply Mod Prime', marker='o')
    plt.plot(df['l'], df['pmp'], label='PMP: Polynomial Mod Prime (4-universal)', marker='o')
    plt.xlabel('l')
    plt.ylabel('time [s]')
    plt.yscale('log', base=2)
    plt.title(f"{title} (n={df['n'][0]})")
    plt.legend()
    plt.grid(True)
    plt.show()

plot("opg_1c.csv", "Time to execute MSH, MMP, PMP")
plot("opg_3.csv", "Time to calculate square sum using table backed by MSH, MMP, PMP")
