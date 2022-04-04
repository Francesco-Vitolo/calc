using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Data;

namespace Calculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {      
        bool klammerAuf = true;       // "(" oder ")"
        List<string> ops = new List<string>() { "+", "-" ,"x", "÷", "mod" };
        bool punktmöglich = true;
        
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Button_Click_Zahl(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            char eingabe = Convert.ToChar(b.Content);
            if (Result.Text.EndsWith(")")) // Nach schließender Klammer darf keine Zahl folgen
                return;
            Result.Text += eingabe;
            Schriftkleiner();
        }

        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            string eingabe = Convert.ToString(b.Content); //string wegen mod

            string res = Result.Text;
            if (ops.Any(x => res.EndsWith(x)) && eingabe != ".")
            {
                Button_Click_Backspace(sender, e); //Verhindern von 2 Operatoren hintereinander
            }

            if (res.EndsWith("-") && res.Length == 1) //Verhindert, dass "-" ersetzt wird durch andere Operatoren, wenn an 1. Pos
                return;
            if (res.EndsWith("(")) //nach öffnender Klammer muss eine Zahl kommen
                return;
            if (!punktmöglich && eingabe == ".")
                return;
            if(punktmöglich && eingabe == ".")
            {
                Result.Text += eingabe;
                punktmöglich = false;
                return;
            }


            if (res != "" || eingabe == "-") //Keine Ahnung, aber wichtig
            {
                if (!punktmöglich)
                    punktmöglich = true;
                Result.Text += eingabe;

            }
            Schriftkleiner();
        }
        private void Schriftkleiner()
        {
            if (Result.Text.Length == 12)
                Result.FontSize = 34;

            if (Result.Text.Length == 16)
                Result.FontSize = 28;

            if (Result.Text.Length == 20)
                Result.FontSize = 22;

            if (Result.Text.Length == 24)
                Result.FontSize = 18;
        }

        private void Button_Click_Result(object sender, RoutedEventArgs e)
        {
            Alert.Visibility = Visibility.Hidden;
            string res = Result.Text;

            if (ops.Any(x => res.EndsWith(x))) // Lambda - Ausdruck geht nicht in Prüfen();
            {
                Ausgabe("Darf nicht auf Operator enden");
                return;
            }

            if(!Prüfen(ref res))
            return;

            ReplaceChars(ref res);

            Result.Text = new DataTable().Compute(res, null).ToString();

            res = Result.Text;
            if (res.Contains("NaN") || res.Contains("∞")) //bei x/0
            {
                Result.Text = "";
                Ausgabe("Es wurde versucht durch 0 zu teilen");
            }
            Result.FontSize = 40; //reset Fontsize
        }

        private bool Prüfen(ref string res)
        {            
            if (res.Contains("mod") && (res.Contains(".") || res.Contains("-"))) // Dezimal geht nicht mit mod
            {
                Ausgabe("mod nicht mit negativen");
                return false;
            }
            if (res.EndsWith("("))
            {
                klammerAuf = true;
                Ausgabe("Darf nicht auf Operator enden");
                return false;
            }
            if (res.StartsWith("(") && res.EndsWith(")"))
                return false;
            return true;
        }
        private void ReplaceChars(ref string res)
        {
            if (res.Contains('x'))
                res = res.Replace('x', '*');
            if (res.Contains('÷'))
                res = res.Replace('÷', '/');
            if (res.Contains("mod"))
                res = res.Replace("mod", "%");
            if (res.Count(x => x == '(') != res.Count(x => x == ')')) //Wenn Klammer nicht schließt, dann schließende hinzufügen
            {
                res += ")";
                klammerAuf = true;
            }
        }
        private void Button_Click_Delete(object sender, RoutedEventArgs e)
        {
            //string s = Result.Text;
            //ReplaceChars(ref s);
            Result.Text = "";
            Result.FontSize = 40;
            punktmöglich = true;
        }

        private void Button_Click_Backspace(object sender, RoutedEventArgs e)
        {
            string res = Result.Text;            
            if(res.EndsWith("mod"))
            {
                res = res.Substring(0, res.Length - 2);
            }
            if(res.EndsWith("("))
            {
                klammerAuf = true;
            }
            if(res.EndsWith(")"))
            {
                klammerAuf = false;
            }
            if (res.EndsWith("."))
            {
                punktmöglich = true;
            }
                if (res.Length != 0)
                Result.Text = res.Substring(0, res.Length - 1);
        }

        private void Button_Click_Klammern(object sender, RoutedEventArgs e)
        {
            if ((ops.Any(x => Result.Text.EndsWith(x)) && klammerAuf) || Result.Text == "")
            {
                Result.Text += '(';
                klammerAuf = false;
            }
            else if(!klammerAuf && !ops.Any(x => Result.Text.EndsWith(x)))
            {
                Result.Text += ')';
                klammerAuf = true;
            }
        }
        private void Ausgabe(string s)
        {
            Alert.Visibility = Visibility.Visible;
            Alert.Text = s;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
