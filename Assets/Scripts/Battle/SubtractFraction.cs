using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fractional
{
    public class SubtractFraction : MonoBehaviour
    {
        private int den3, num3;
        protected int[] answer = { 1, 1 };
        private int num1, den1, num2, den2;
        public SubtractFraction()
        {
            num1 = 0;
            den1 = 0;
            num2 = 0;
            den2 = 0;
        }

        public int[] doSubtractFraction(int _num1, int _den1, int _num2, int _den2)
        {
            num1 = _num1;
            den1 = _den1;
            num2 = _num2;
            den2 = _den2;

            // Finding gcd of den1 and den2
            den3 = gcd(den1, den2);

            // Denominator of final fraction obtained
            // finding LCM of den1 and den2
            // LCM * GCD = a * b
            den3 = (den1 * den2) / den3;

            // Changing the fractions to have
            // same denominator.
            // Numerator of the final fraction obtained
            num3 = (num1) * (den3 / den1) -
                   (num2) * (den3 / den2);

            // Calling function to convert final fraction
            // into it's simplest form
            lowest();

            answer[0] = num3;
            answer[1] = den3;

            return answer;
        }

        private int gcd(int a, int b)
        {
            if (a == 0)
                return b;
            return gcd(b % a, a);
        }

        private void lowest()
        {
            // Finding gcd of both terms
            int common_factor = gcd(num3, den3);

            // Converting both terms into simpler
            // terms by dividing them by common factor
            den3 = den3 / common_factor;
            num3 = num3 / common_factor;
            if (den3 < 0)
            {
                num3 = num3 * -1;
                den3 = den3 * -1;
            }
        }
    }
}

