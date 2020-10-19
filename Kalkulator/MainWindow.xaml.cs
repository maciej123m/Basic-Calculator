using System;
using System.Windows;
using System.Windows.Controls;

namespace Kalkulator {
    /// <summary>
    ///     Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public interface Icalc {
            double sqroot(double liczba);
            double exponentiation(double sc, int ile);
        }

        private enum operacja : int {
            multiplication = (int) '*',
            division = (int) '/',
            addition = (int) '+',
            subtraction = (int) '-',
            exponentiation = (int) '^',
            equal = (int) '=',
            none = 0
        }

        //interfejs
        private Icalc calc;

        //delegate do pola tekstowego
        readonly private delegateTextChanged del;

        //zmienna odpowiadająca za 1 wprowadzenie wartości
        private bool firstNumber = true;

        //pole tekstowe w kalkulatorze
        private string text = "";

        //wynik
        private double score = 0;

        //ostatni znak jak ma zostać wykonany
        private operacja lastAction = operacja.none;


        public MainWindow() {
            del = textChanged;
            InitializeComponent();
            Kalkulator.SizeChanged += window_SizeChanged;
            calc = new Calc();
        }

        private void textChanged(char t) {
            //jeżeli już jest przecinek
            if (t == ',' && text.Contains(t.ToString())) {
                return;
            }

            if (t == '0' && text.Length == 0) {
                return;
            }

            //jeżeli przecinek jest jako pierwszy dodaj przed nim 0 (0,)
            if (t == ',' && text.Length == 0) {
                text += "0";
            }

            text += t;
            textWin.Content = text;
        }

        private delegate void delegateTextChanged(char t);

        /// <summary>
        /// kliknięcie cyfry
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonClick(object sender, RoutedEventArgs e) {
            string content = (sender as Button).Content.ToString();

            //jeżeli ostatnia operacja to równa się czyli po sumowaniu i nie wybraniu następnej operacji, zresetuj aplikacje
            //ma na celu reset przy następnym wykonywaniu działania
            if (lastAction == operacja.equal) {
                restart(false);
            }

            del(char.Parse(content));
        }

        /// <summary>
        /// kliknięcie przycisku akcji typu + - itp
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void actionButton(object sender, RoutedEventArgs e) {
            string act = (sender as Button).Content.ToString();
            //jeżeli to C
            if (act[0] == 'C') {
                restart();
                return;
            }

            //żeby wynik nie można było dodawać,mnożyć etc.
            if (text.Length == 0 && lastAction != operacja.equal) {
                return;
            }

            if (firstNumber) {
                if (text.Length != 0) {
                    score = double.Parse(text);
                }
                else {
                    score = 0;
                }

                firstNumber = false;
            }
            else {
                var valid = sum();
                if (!valid)
                    return;
            }

            //ustawia ostatnią akcje
            lastAction = (operacja) act[0];

            //czyści zmienną zawierającą liczby kalkulatora
            text = "";

            //ustawia wynik jako widzialny
            textWin.Content = score.ToString();
        }

        /// <summary>
        /// restart aplikacji
        /// </summary>
        /// <param name="res">czy okno ma zostać zresetowane</param>
        private void restart(bool res = true) {
            firstNumber = true;

            text = "";

            score = 0;

            lastAction = operacja.none;

            if (res)
                refreshTextLabel();
        }

        /// <summary>
        /// odczytywanie ostatniej operacji
        /// wykonywanie jej
        /// czyszczenie operacji
        /// wywowyłane za każdym kliknięciem przycisku akcji
        /// </summary>
        /// <returns>zwraca czy operacja przebiegła pomyślnie</returns>
        private bool sum() {
            double value = double.Parse(textWin.Content.ToString());

            switch (lastAction) {
                case operacja.addition:
                    score += value;
                    break;
                case operacja.subtraction:
                    score += -value;
                    break;
                case operacja.multiplication:
                    score *= value;
                    break;
                case operacja.division:
                    //sprawdzanie czy nie jest dzielone przez 0
                    if (value == 0) {
                        textWin.Content = "Nie można dzielić przez 0!";
                        restart(false);
                        return false;
                    }

                    score /= value;
                    break;
                case operacja.exponentiation:
                    score = calc.exponentiation(score, (int) value);
                    break;
            }

            //ustawiamy lastAction na pustą akcje
            lastAction = operacja.none;
            return true;
        }

        /// <summary>
        /// kliknięcie przycisku pierwiastka
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pierButton(object sender, RoutedEventArgs e) {
            var value = double.Parse(textWin.Content.ToString());
            if (value < 0) {
                textWin.Content = "błąd!";
                restart(false);
                return;
            }

            var pierValue = calc.sqroot(value);
            text = pierValue.ToString();
            refreshTextLabel();
        }

        /// <summary>
        /// ma na celu ustawianie wielkości tekstu przy zmianie wielkości okna
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void window_SizeChanged(object sender, SizeChangedEventArgs e) {
            var a = Kalkulator.ActualHeight;
            textWin.FontSize = a > 700 ? 50 : 30;
        }

        /// <summary>
        /// kliknięcie przycisku cofania
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_cof_Click(object sender, RoutedEventArgs e) {
            if (text.Length == 0) {
                return;
            }

            text = text.Remove(text.Length - 1);
            refreshTextLabel();
        }

        /// <summary>
        /// odświeżanie okna tekstu kalkulatora
        /// </summary>
        private void refreshTextLabel() {
            if (text.Length == 0) {
                textWin.Content = "0";
                return;
            }

            textWin.Content = text;
        }
    }
}