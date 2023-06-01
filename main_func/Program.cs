using System;

namespace ConsoleApp { 
    class Program { 
        static double EPS = 1.0 / 3.0;
        static double K(double x)
    {
        if (x >= 0 && x < EPS)
        {
            return x;
        }
        else if (x > EPS && x <= 1)
        {
            return 1;
        }
        return 0;
    }

    static double Q(double x)
    {
        if (x >= 0 && x < EPS)
        {
            return x * x;
        }
        else if (x > EPS && x <= 1)
        {
            return x;
        }
        return 0;
    }

    static double F(double x)
    {
        if (x >= 0 && x < EPS)
        {
            return 1;
        }
        else if (x > EPS && x <= 1)
        {
            return x * x - 1;
        }
        return 0;
    }

    static double FindIntegral(double a, double b, Func<double, double> f)
    {
        double h = (b - a) / 2;
        double res = (h / 3) * (f(a) + 4 * f(a + h) + f(b));
        return res;
    }

    static double[] Solution(int n, Func<double, double> k, Func<double, double> q, Func<double, double> f, double bord1, double bord2)
    {
        double h = 1.0 / n;

        double[] temp = new double[n];
        double x = h;

        for (int i = 0; i < n; i++)
        {
            if (x <= EPS || (x - h) >= EPS)
            {
                temp[i] = k(x - 0.5 * h);
            }
            else
            {
                double curr = (EPS - x + h) / k(0.5 * (x - h + EPS)) + (x - EPS) / k(0.5 * (x + EPS));
                temp[i] = h / curr;
            }
            x += h;
        }

        double[] temp1 = new double[n - 1];
        double[] temp2 = new double[n - 1];
        x = h;

        for (int i = 0; i < n - 1; i++)
        {
            if ((x + 0.5 * h) <= EPS || (x - 0.5 * h) >= EPS)
            {
                temp1[i] = n * FindIntegral(x - 0.5 * h, x + 0.5 * h, q);
                temp2[i] = n * FindIntegral(x - 0.5 * h, x + 0.5 * h, f);
            }
            else
            {
                temp1[i] = n * (FindIntegral(x - 0.5 * h, EPS, q) + FindIntegral(EPS, x + 0.5 * h, q));
                temp2[i] = n * (FindIntegral(x - 0.5 * h, EPS, f) + FindIntegral(EPS, x + 0.5 * h, f));
            }
            x += h;
        }

        double[] first = new double[n - 1];
        double[] second = new double[n - 1];
        double[] third = new double[n - 1];

        for (int i = 0; i < n - 1; i++)
        {
            first[i] = temp[i] / (h * h);
            second[i] = temp[i + 1] / (h * h);
            third[i] = temp[i] / (h * h) + temp[i + 1] / (h * h) + temp1[i];
        }

        double[] alpha = new double[n];
        double[] beta = new double[n];

        double start = 0;

        alpha[0] = start;
        beta[0] = bord1;

        for (int i = 0; i < n - 1; i++)
        {
            alpha[i + 1] = second[i] / (third[i] - first[i] * alpha[i]);
            beta[i + 1] = (temp2[i] + first[i] * beta[i]) / (third[i] - first[i] * alpha[i]);
        }

        double[] res = new double[n + 1];
        res[0] = bord1;
        res[n] = bord2;
        for (int i = n - 1; i >= 1; i--)
        {
            res[i] = alpha[i] * res[i + 1] + beta[i];
        }
        return res;
    }

    static Tuple<double[], double[], double[], double[]> Task(int n, Func<double, double> k, Func<double, double> q, Func<double, double> f, double bord1, double bord2)
    {
        double[] v = Solution(n, k, q, f, bord1, bord2);
        double[] v2 = Solution(2 * n, k, q, f, bord1, bord2);

        double[] e = new double[n + 1];

        double x_tmp = 0.0;
        double[] x = new double[n + 1];
        double h = 1.0 / n;

        for (int i = 0; i < n + 1; i++)
        {
            x[i] = x_tmp;
            e[i] = Math.Abs(v[i] - v2[2 * i]);
            x_tmp += h;
        }

        return Tuple.Create(x, v, v2, e);
    }

    static void Main(string[] args)
    {
        int n = 50;
        double border1 = 1.0;
        double border2 = 0.0;
        var result = Task(n, K, Q, F, border1, border2);

        using (System.IO.StreamWriter file =
            new System.IO.StreamWriter("main.txt"))
        {
            file.WriteLine("N, Xi, V(xi), V2(x2i), V(xi) - V2(x2i)");
            for (int i = 0; i < n + 1; i += 5)
            {
                file.WriteLine(string.Format("{0}, {1}, {2}, {3}, {4}", i, result.Item1[i], result.Item2[i], result.Item3[2 * i], result.Item4[i]));
            }
        }
    }
}
    
}
