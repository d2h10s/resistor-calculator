using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace resistance_calculator
{
    /// <summary>
    /// 이 프로그램은 서울과학기술대학교 기계시스템디자인공학과 최호승에 의해 작성되었습니다.
    /// </summary>
    public partial class MainWindow : Window
    {
        int pre_idx, target_value;
        TextBox[] tb_input = new TextBox[10];
        TextBlock[] tblock_num = new TextBlock[10];
        TextBlock[] tblock_ohm = new TextBlock[10];
        RadioButton[] rb_num;
        List<List<int>> result = new List<List<int>>();
        

        public MainWindow()
        {
            InitializeComponent();
            rb_num = new RadioButton[9] { rb2, rb3, rb4, rb5, rb6, rb7, rb8, rb9, rb10 };
            rb2.IsChecked = true;
            for(int i = 0; i < 10; i++)
            {
                var row = new RowDefinition();
                gd_type.RowDefinitions.Add(row);
            }
            var col1 = new ColumnDefinition();
            var col2 = new ColumnDefinition();
            var col3 = new ColumnDefinition();
            col1.Width = new GridLength(2, GridUnitType.Star);
            col2.Width = new GridLength(6, GridUnitType.Star);
            col3.Width = new GridLength(1, GridUnitType.Star);
            gd_type.ColumnDefinitions.Add(col1);
            gd_type.ColumnDefinitions.Add(col2);
            gd_type.ColumnDefinitions.Add(col3);

            tb_output.Text = "Author: d2h10s\n" +
                "Github: https://github.com/d2h10s \n" +
                "Version: 1.0.2\n" +
                "--------------------사용법--------------------\n" +
                "1. 저항 수를 정합니다.\n" +
                "2. 저항값을 입력합니다.\n\n" +
                "3-1. [직렬 합성] 버튼을 누르면 직렬 연결시의 합성 저항 값을 보여줍니다.\n\n" +
                "3-2. [병렬 합성] 버튼을 누르면 병렬 연결시의 합성 저항 값을 보여줍니다.\n\n" +
                "3-3. [조합 찾기] 버튼을 누르면 현재 입력한 저항들로 목표값을 만들 수 있는 모든 조합을 보여줍니다.\n\n" +
                "------------------------------------------------\n\n";
        }

        private void rb_Checked(object sender, RoutedEventArgs e)
        {
            int idx;
            for (idx = 2; idx <= 10; idx++)
                if (rb_num[idx - 2].IsChecked == true) break;
            tb_output.Text += $"{idx} 개의 저항을 선택하셨습니다.\n\n";
            tb_output.ScrollToEnd();

            if (pre_idx < idx)
            {
                for (int i = pre_idx; i < idx; i++)
                {

                    tb_input[i] = new TextBox();
                    tb_input[i].TextAlignment = TextAlignment.Right;
                    tb_input[i].Margin = new Thickness(2, 2, 2, 2);
                    Grid.SetColumn(tb_input[i], 1);
                    Grid.SetRow(tb_input[i], i);
                    gd_type.Children.Add(tb_input[i]);

                    tblock_num[i] = new TextBlock();
                    tblock_num[i].Text = $"{i + 1}번";
                    tblock_num[i].TextAlignment = TextAlignment.Right;
                    tblock_num[i].Margin = new Thickness(2, 3, 0, 2);
                    Grid.SetColumn(tblock_num[i], 0);
                    Grid.SetRow(tblock_num[i], i);
                    gd_type.Children.Add(tblock_num[i]);

                    tblock_ohm[i] = new TextBlock();
                    tblock_ohm[i].Text = "Ω";
                    tblock_ohm[i].Margin = new Thickness(2, 3, 2, 2);
                    Grid.SetColumn(tblock_ohm[i], 2);
                    Grid.SetRow(tblock_ohm[i], i);
                    gd_type.Children.Add(tblock_ohm[i]);
                }
            }
            else if (pre_idx > idx)
            {
                for (int i = pre_idx - 1; i >= idx; i--)
                {
                    gd_type.Children.Remove(tb_input[i]);
                    gd_type.Children.Remove(tblock_num[i]);
                    gd_type.Children.Remove(tblock_ohm[i]);
                }
            }
            pre_idx = idx;
        }

        private void serial_combine_Click(object sender, RoutedEventArgs e)
        {
            int s = 0;
            for (int i = 0; i < pre_idx; i++)
            {
                if (int.TryParse(tb_input[i].Text, out int res))
                {
                    s += res;
                }
                else
                {
                    tb_output.Text += "저항값은 숫자만 가능합니다.\n\n";
                    tb_output.ScrollToEnd();
                    SystemSounds.Beep.Play();
                    return;
                }
            }
            tb_output.Text += $"직렬 합성 저항의 계산값은 {s} Ω 입니다." +
                $"\n------------------------------------------------\n\n";
            tb_output.ScrollToEnd();
        }
        private void parallel_combine_Click(object sender, RoutedEventArgs e)
        {
            double s = 0;
            for (int i = 0; i < pre_idx; i++)
            {
                if (int.TryParse(tb_input[i].Text, out int res))
                {
                    s += 1d/res;
                }
                else
                {
                    tb_output.Text += "저항값은 숫자만 가능합니다.\n\n";
                    tb_output.ScrollToEnd();
                    SystemSounds.Beep.Play();
                    return;
                }
            }
            tb_output.Text += $"병렬 합성 저항의 계산값은 {1/s:.000} Ω 입니다." +
                $"\n------------------------------------------------\n\n";
            tb_output.ScrollToEnd();
        }

        private void dfs(int start, int depth, double s, List<int> temp)
        {
            if (1 / s < target_value + 1 && 1 / s > target_value - 1)
            {
                result.Add(temp.ToList());
                return;
            }
            if (depth >= pre_idx) return;
            else
            {
                for(int i = start; i < pre_idx; i++)
                {
                    int res = int.Parse(tb_input[i].Text);
                    temp.Add(res);
                    dfs(i + 1, depth + 1, s + 1d / res, temp);
                    temp.RemoveAt(temp.Count - 1);
                }
            }
        }

        private void combination_Click(object sender, RoutedEventArgs e)
        {
            if (e.GetType() == typeof(KeyEventArgs) && (e as KeyEventArgs).Key != Key.Enter) return;
            result.Clear();
            for (int i = 0; i < pre_idx; i++)
            {
                if (!int.TryParse(tb_input[i].Text, out int res))
                {
                    tb_output.Text += "저항값은 숫자만 가능합니다.\n\n";
                    tb_output.ScrollToEnd();
                    SystemSounds.Beep.Play();
                    return;
                }
            }
            if (int.TryParse(target.Text, out int t)){
                target_value = t;
                dfs(0, 0, 0, new List<int>());
                if (result.Count == 0) tb_output.Text += "가능한 조합이 없습니다.";
                else
                {
                    tb_output.Text += $"###  {target_value}Ω 을 만들 수 있는 저항 조합  ###\n";
                    int i = 1;
                    foreach (var a in result)
                    {
                        int j = 1;
                        tb_output.Text += $"조합{i++} -> ";
                        foreach (var b in a)
                        {
                            tb_output.Text += $"{b}Ω" + (j++ < a.Count ? ", " : "");
                        }
                        tb_output.Text += "\n";
                        tb_output.ScrollToEnd();
                    }
                }
            }
            else
            {
                tb_output.Text += "저항값은 숫자만 가능합니다.\n\n";
                SystemSounds.Beep.Play();
            }
            tb_output.Text += "\n------------------------------------------------\n\n";
            tb_output.ScrollToEnd();
        }
    }
}
