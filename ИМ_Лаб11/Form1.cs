using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ИМ_Лаб11
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        const int range = 4;
        int[] ni = new int[range];

        private void button1_Click(object sender, EventArgs e)
        {
            chart1.Series[0].Points.Clear();

            double mean = (double)numericUpDown1.Value;
            double variance = (double)numericUpDown2.Value;
            int N = (int)numericUpDown3.Value;

            double[] sample = new double[N];

            Random random = new Random();

            double maxx = 0, minn = 10000;
            double a1, a2;
            double value;
            double step;

            //Находим границы выборки
            //датчик на основе преобразования бокса-Мюллера
            for (int i = 0; i < N; i++)
            {
                a1 = random.NextDouble(); 
                a2 = random.NextDouble();
                value = (Math.Sqrt(-2 * Math.Log(a1)) * Math.Cos(2 * Math.PI * a2)) * Math.Sqrt(variance) + mean;
                if (value > maxx) maxx = value;
                if (value < minn) minn = value;
                sample[i] = value;
            }
            step = (maxx - minn) / range;

            //Вычисляем частоты
            for (int i = 0; i < range; i++)
            {
                ni[i] = 0;
                for (int j = 0; j < N; j++)
                {
                    if (sample[j] > minn + step * i && sample[j] <= minn + step * (i + 1)) 
                        ni[i]++;
                }
            }

            //Выводим относительные частоты
            for (int i = 0; i < range; i++)
            {
                chart1.Series[0].Points.Add((double)ni[i] / N);
            }

            double dispersion = 0;
            double empirical_average = 0;
            for (int i = 0; i < N; i++)
            {
                empirical_average += sample[i];
                dispersion += sample[i] * sample[i];
            }
            empirical_average /= N;
            dispersion = dispersion / N - empirical_average * empirical_average;

            //Вычисление абсолютной погрешноси
            double absE = Math.Abs(empirical_average - mean);
            double absD = Math.Abs(dispersion - variance);

            //Вычисление относительной погрешности RelativeE RelativeD
            double relE = absE / Math.Abs(mean);
            double relD = absD / Math.Abs(variance);

            label1.Text = "Эмпирическое среднее: " + empirical_average + "\nотносительная погрешность эмпирического среднего: " + relE;
            label2.Text = "Дисперсия: " + dispersion + "\nотносительная погрешность дисперсии: " + relD;

            double[] probabilities = new double[range];
            double x;
            for (int i = 0; i < range; i++)
            {
                x = ((minn + step * (i + 1)) + (minn + step * i)) / 2;
                probabilities[i] = ((minn + step * (i + 1)) - (minn + step * i)) * 
                    Math.Exp(Math.Pow((x - mean)/dispersion, 2) / -2) / (Math.Sqrt(2 * Math.PI) * dispersion);
            }

            double chi_squared = 0;
            double chiA = 9.488;
            for (int i = 0; i < range; i++)
            {
                chi_squared += (ni[i] * ni[i]) / (N * probabilities[i]);
            }
            chi_squared -= N;

            label3.Text = chi_squared + " ";
            if (chi_squared > chiA)
                label3.Text += " > " + chiA + " is True";
            else label3.Text += " <= " + chiA + " is False";
        }
    }
    
}
