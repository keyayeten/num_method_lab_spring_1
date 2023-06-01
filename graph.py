import seaborn as sns
import matplotlib.pyplot as plt
import pandas as pd
import os


RESULTS_PATH = {
    'test': 'test_func\\test.txt',
    'main': 'main_func\\main.txt',
}

ALGORITHMS_PATH = {
    'test': 'test_func\\bin\\Debug\\net7.0\\test_func.exe',
    'main': 'main_func\\bin\\Debug\\net7.0\\main_func.exe',
}


def graphic_task(mode: str = 'test'):
    results = []
    with open(RESULTS_PATH[mode], 'r') as f:
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


def create_results(mode: str = 'test'):
    command = f"{ALGORITHMS_PATH[mode]}"
    os.system(command)


if __name__ == "__main__":
    print('1 - тестовое задание\n2 - основная задача')
    choice = input('Выберите режим работы: ')
    match choice:
        case '1':
            create_results()
            graphic_task()
        case '2':
            create_results(mode='main')
            graphic_task(mode='main')
        case _:
            print('error')
