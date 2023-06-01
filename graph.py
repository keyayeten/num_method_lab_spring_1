import seaborn as sns
import matplotlib.pyplot as plt
import pandas as pd


def main_task():
    results = []
    with open('main.txt', 'r') as f:
        while True:
            results.append(f.readline())
            if not f.readline():
                break
    results = [i.replace('\n', '').replace(' ', '').split(',')
               for i in results][1:-1:]
    results = [list(map(float, i)) for i in results]

    df = pd.DataFrame(results, columns=['N', 'Xi', 'V(xi)',
                                        'V2(x2i)', 'V(xi) - V2(x2i)'])
    print(df)

    sns.regplot(data=df, x='Xi', y='V(xi)')
    sns.regplot(data=df, x='Xi', y='V2(x2i)')
    plt.show()
