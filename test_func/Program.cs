using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class Program
{
    const double EPS = 1.0 / 3.0;

    static double k_test(double x)
    {
        if (x < 0 || x > 1)
            return 0;

        if (x >= 0 && x < EPS)
            return 1.0 / 3.0;

        return 1;
    }

    static double q_test(double x)
    {
        if (x < 0 || x > 1)
            return 0;

        if (x >= 0 && x < EPS)
            return 1.0 / 9.0;

        return 1.0 / 3.0;
    }

    static double f_test(double x)
    {
        if (x < 0 || x > 1)
            return 0;

        if (x >= 0 && x < EPS)
            return 1;

        return -(8.0 / 9.0);
    }

    static double analytic_solution(double x)
    {
        if (x < 0 || x > 1)
            return 0;

        var c1 = 0.365910611;
        var c2 = -0.427124746;
        var c3 = 0.75205762;
        var c4 = 0.309157747;

        if (x >= 0 && x <= EPS)
            return c1 * Math.Exp(x / 3) + c2 * Math.Exp(-x / 3);

        return c3 * Math.Exp(x / Math.Sqrt(3)) + c4 * Math.Exp(-x / Math.Sqrt(3));
    }

    static List<double> GetTestTaskSolution(int n)
    {
        var h = 1.0 / n;
        var res = new List<double>();

        for (var i = 0; i < n + 1; i++)
        {
            var x = i * h;
            res.Add(analytic_solution(x));
        }

        return res;
    }

    static double GetIntegral(double a, double b, Func<double, double> func)
    {
    var h = (b - a) / 2;
    var resPrev = 0.0;
    var resCurr = h / 3 * (func(a) + 4 * func(a + h) + func(b));
    var n = 1;
    while (Math.Abs(resCurr - resPrev) > 0.5e-6)
{
    resPrev = resCurr;
    n *= 2;
    h /= 2;
    var sum = 0.0;

    for (var i = 0; i < n; i++)
    {
        var x = a + (2 * i + 1) * h;
        sum += func(x);
    }

    resCurr = h / 3 * (func(a) + 4 * sum + 2 * resPrev - func(b));
    }

    return resCurr;
    }

    static List<double> GetSolution(int n, Func<double, double> k, Func<double, double> q, Func<double, double> f,
        double border1, double border2)
    {
        var h = 1.0 / n;

        var a_j = new List<double>();
        var x = h;

        for (var i = 1; i < n + 1; i++)
        {
            if ((x <= EPS) || ((x - h) >= EPS))
                a_j.Add(k(x - 0.5 * h));
            else
            {
                var tmp = (EPS - x + h) / k(0.5 * (x - h + EPS)) + (x - EPS) / k(0.5 * (x + EPS));
                a_j.Add(h / tmp);
            }

            x += h;
        }

        var d_j = new List<double>();
        var fi_j = new List<double>();
        x = h;

        for (var i = 1; i < n; i++)
        {
            if ((x + 0.5 * h) <= EPS || (x - 0.5 * h) >= EPS)
            {
                d_j.Add(n * GetIntegral(x - 0.5 * h, x + 0.5 * h, q));
                fi_j.Add(n*GetIntegral(x - 0.5 * h, x + 0.5 * h, f));
            }
            else
            {
            d_j.Add(n * (GetIntegral(x - 0.5 * h, EPS, q) + GetIntegral(EPS, x + 0.5 * h, q)));
            fi_j.Add(n * (GetIntegral(x - 0.5 * h, EPS, f) + GetIntegral(EPS, x + 0.5 * h, f)));
            }
            x += h;
    }

    var A = new List<double>();
    var B = new List<double>();
    var C = new List<double>();

    for (var i = 0; i < n - 1; i++)
    {
        A.Add(a_j[i] / (h * h));
        B.Add(a_j[i + 1] / (h * h));
        C.Add(a_j[i] / (h * h) + a_j[i + 1] / (h * h) + d_j[i]);
    }

    var xi1 = 0.0;
    var alpha = new List<double> { xi1 };
    var beta = new List<double> { border1 };

    for (var i = 0; i < n - 1; i++)
    {
        alpha.Add(B[i] / (C[i] - A[i] * alpha[i]));
        beta.Add((fi_j[i] + A[i] * beta[i]) / (C[i] - A[i] * alpha[i]));
    }

    var v = Enumerable.Repeat<double>(0, n + 1).ToList();
    v[0] = border1;
    v[n] = border2;

    for (var i = n - 2; i >= 0; i--)
    {
        v[i + 1] = alpha[i] * v[i + 2] + beta[i];
    }

    return v;
}

static Tuple<List<double>, List<double>, List<double>, List<double>> Task(int n, Func<double, double> k, Func<double, double> q,
    Func<double, double> f, double border1, double border2)
{
    var v = GetSolution(n, k, q, f, border1, border2);
    var v2 = GetSolution(2 * n, k, q, f, border1, border2);
    var e = new List<double>();

    var xTmp = 0.0;
    var x = new List<double>();
    var h = 1.0 / n;

    v2 = Enumerable.Range(0, n + 1).Select(i => v2[2 * i]).ToList();

    for (var i = 0; i < n + 1; i++)
    {
        x.Add(xTmp);
        e.Add(Math.Abs(v[i] - v2[i]));
        xTmp += h;
    }

    return Tuple.Create(x, v, v2, e);
}

static void Test(int n)
{
    const double border1 = 0;
    const double border2 = 1;

    var res = Task(n, k_test, q_test, f_test, border1, border2);
    var u = GetTestTaskSolution(n);
    var e = Enumerable.Range(0, res.Item2.Count).Select(i => Math.Abs(res.Item2[i] - u[i])).ToList();
    var maxError = e.Max();

    using (var writer = new StreamWriter("test.txt"))
    {
        writer.WriteLine("N,Xi,U(xi),V(xi),U(xi) - V(xi)");
        for (var i = 0; i < n + 1; i++)
        {
            writer.WriteLine($"{i},{res.Item1[i]},{u[i]},{res.Item2[i]},{e[i]}");
        }
    }
    Console.WriteLine($"Погрешность = {maxError}");
}

static void Main()
{
    Test(5);
}
}
