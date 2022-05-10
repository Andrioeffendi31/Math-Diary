using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwoDimentionalFigure
{
    class CalculateAreaParimeter
    {
        public float num1 { get; set; } = 0;
        public float num2 { get; set; } = 0;
        public float num3 { get; set; } = 0;

        public CalculateAreaParimeter()
        {
            num1 = 0;
            num2 = 0;
            num3 = 0;
        }

        public CalculateAreaParimeter(float num1)
        {
            this.num1 = num1;
        }

        public CalculateAreaParimeter(float num1, float num2)
        {
            this.num1 = num1;
            this.num2 = num2;
        }

        public CalculateAreaParimeter(float num1, float num2, float num3)
        {
            this.num1 = num1;
            this.num2 = num2;
            this.num3 = num3;
        }

        public float RectangleArea()
        {
            return num1 * num2;
        }

        public float RectanglePerimeter()
        {
            return (num1 + num2) * 2;
        }

        public float TriangleArea()
        {
            return (num1 * num2) / 2;
        }

        public float TrianglePerimeter()
        {
            return num1 + num2 + num3;
        }

        public float CircleArea()
        {
            return (float)Math.Round((Math.PI * Math.Pow(num1, 2)), 2);
        }

        public float CirclePerimeter()
        {
            return (float)Math.Round((2 * Math.PI * num1), 2);
        }

        public float SquareArea()
        {
            return num1 * num1;
        }

        public float SquarePerimeter()
        {
            return num1 * 4;
        }

        public float RhombusArea()
        {
            return num1 * num2 / 2;
        }

        public float RhombusPerimeter()
        {
            return num1 * 4;
        }

        public float TrapezoidArea()
        {
            return (num1 + num2) / 2 * num3;
        }

        public float TrapezoidPerimeter()
        {
            return num1 + num2 + num3 + num3;
        }

        public float ParallelogramArea()
        {
            return num1 * num2;
        }

        public float ParallelogramPerimeter()
        {
            return (num1 + num2) * 2;
        }

        public float KiteArea()
        {
            return num1 * num2 / 2;
        }

        public float KitePerimeter()
        {
            return num1 + num1 + num2 + num2;
        }
    }
}