using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kalkulator
{

    public class Calc : MainWindow.Icalc
    {
        public double sqroot(double liczba) {

            if (double.IsInfinity(liczba)) {
                return double.PositiveInfinity;
            }

            double x = liczba / 2;
            while (Math.Abs(x - liczba / x) > 0.0000000000001) {
                x = (x + liczba / x) / 2;
                if (x * x == liczba)
                    break;
            }

            return x;
        }

        public double exponentiation(double sc, int ile) {
            var pre = sc;
            for (int i = 0; i < ile - 1; i++) {
                if (double.IsInfinity(sc)) break;
                sc *= pre;
            }

            return sc;
        }

    }
}
