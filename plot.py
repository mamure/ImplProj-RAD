import matplotlib.pyplot as plt
import pandas as pd
import numpy as np
pd.options.display.float_format = '{:.2e}'.format


def plot_line_and_stdev(df, column, color, **kwargs):
    group = df.groupby("bits")
    mean = group[column].mean()
    stdev = group[column].std()
    bits = group['bits'].first()

    plt.xticks(range(1, bits.max() + 1, 2))
    plt.plot(bits, mean, color=color, **kwargs)
    plt.fill_between(bits, mean - stdev, mean + stdev, color=color, alpha=0.2)

def plot_line_and_mse(df, ref, est, color, **kwargs):
    df['square error'] = (df[est] - df[ref]) ** 2
    group = df.groupby("bits")
    mean = group[est].mean()
    mse = group['square error'].mean()
    bits = group['bits'].first()

    plt.xticks(range(1, bits.max() + 1, 2))
    plt.plot(bits, mean, color=color, **kwargs)
    plt.fill_between(bits, mean + mse, mean, color=color, alpha=0.2)


def plot_1c_or_3(file, title):
    plt.figure(figsize=(10, 7))
    df = pd.read_csv(file)
    plot_line_and_stdev(df, "MSH", "red", label="MSH: Multiply Shift Hash", linewidth=2)
    plot_line_and_stdev(df, "MMP", "green", label="MMP: Multiply Mod Prime", linewidth=2)
    plot_line_and_stdev(df, "PMP", "blue", label="PMP: Polynomial Mod Prime (4-univ)", linewidth=2)
    plt.xlabel('bit size')
    plt.ylabel('time [ms]') 
    plt.yscale('log', base=2)
    plt.title(f"{title}\n{df['i'].max() + 1} experiments per bit size. Stream size of {df['size'][0]:.2e}")
    plt.legend()
    plt.grid(True)
    plt.show()

def plot_7(file, title, bits, median_size):
    plt.figure(figsize=(10, 7))
    df = pd.read_csv(file)
    group = df[df['bits'] == bits]

    medtrick = group.groupby(np.arange(len(group)) // median_size).mean()
    medtrick['square error'] = (medtrick['X'] - medtrick['S']) ** 2
    medtrick_mse = medtrick['square error'].mean()
    medtrick = medtrick.sort_values(by='X', ascending=False)
    
    normal = group.sort_values(by='X', ascending=False)
    normal['square error'] = (normal['X'] - normal['S']) ** 2
    normal_mse = normal['square error'].mean()

    plt.plot(
        range(1, len(normal) + 1), 
        normal['S'], color="blue", 
        label="S: Hash Table"
    )
    plt.plot(
        range(1, len(normal) + 1), 
        normal['X'], color="red", 
        label="X: Count Sketch"
    )
    plt.plot(
        range(1, len(medtrick) * median_size + 1, median_size), 
        medtrick['X'], 
        color="green", 
        label=f"X: Count Sketch with Median Trick of size {median_size}"
    )
    plt.title(
        f"{title}\n" +
        f"MSE: {normal_mse:.2e}. MSE with median trick: {medtrick_mse:.2e}\n" +
        f"{df['i'].max() + 1} experiments with {bits} bits. Stream size of {df['size'][0]:.0e}"
    )
    plt.xticks([max(i, 1) for i in range(0, len(normal) + 1, 10)])
    plt.xlabel('Sorted Index')
    plt.ylabel('Square Sum')
    plt.legend()
    plt.grid(True)
    plt.show()

def plot_8a(file, title):
    plt.figure(figsize=(10, 7))
    df = pd.read_csv(file)
    plot_line_and_mse(df, "S", "X", "red", label="X: Count Sketch", linewidth=3)
    plot_line_and_stdev(df, "S", "blue", label="S: Hash Table", linewidth=2, linestyle="dashed")
    plt.xlabel('Bit Size')
    plt.ylabel('Square Sum')
    plt.yscale('log', base=2)
    plt.title(f"{title}\n{df['i'].max() + 1} experiments per bit size. Stream size of {df['size'][0]:.2e}")
    plt.legend()
    plt.grid(True)
    plt.show()


def plot_8b(file, title):
    df = pd.read_csv(file)
    plt.figure(figsize=(10, 6))
    plot_line_and_stdev(df, "Stime", "blue", label="S: Hash Table", linewidth=2)
    plot_line_and_stdev(df, "Xtime", "red", label="X: Count Sketch", linewidth=2)
    plt.xlabel('Bit Size')
    plt.ylabel('Time [ms]') 
    plt.yscale('log', base=2)
    plt.title(f"{title}\n{df['i'].max() + 1} experiments per bit size. Stream size of {df['size'][0]:.2e}")
    plt.legend()
    plt.grid(True)
    plt.show()


plot_1c_or_3("opg_1c.csv", "Time to execute MSH, MMP and PMP")
plot_1c_or_3("opg_3.csv", "Time to calculate Square Sum using Hash Table backed by MSH, MMP and PMP")
plot_7(
    'opg_7_8.csv', 
    "MSE between Square Sum calculated using a Hash Table versus a Count Sketch \nBoth backed by 4-universal Polynomial Mod Prime", 
    13,
    11
)
plot_8a("opg_7_8.csv", "MSE between Square Sum calculated using a Hash Table versus a Count Sketch \nBoth backed by 4-universal Polynomial Mod Prime")
plot_8b("opg_7_8.csv", "Time to calculate Square Sum for a Hash Table versus a Count Sketch \nBoth backed by 4-universal Polynomial Mod Prime")

