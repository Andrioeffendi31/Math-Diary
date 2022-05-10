using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeDimentionalFigure
{
    class CalculateSurfaceVolume
    {
        public float num1 { get; set; } = 0;
        public float num2 { get; set; } = 0;
        public float num3 { get; set; } = 0;

        public CalculateSurfaceVolume()
        {
            num1 = 0;
            num2 = 0;
            num3 = 0;
        }

        public CalculateSurfaceVolume(float num1)
        {
            this.num1 = num1;
        }

        public CalculateSurfaceVolume(float num1, float num2)
        {
            this.num1 = num1;
            this.num2 = num2;
        }

        public CalculateSurfaceVolume(float num1, float num2, float num3)
        {
            this.num1 = num1;
            this.num2 = num2;
            this.num3 = num3;
        }

        public float CubeSurface()
        {
            return 6 * num1 * num1;
        }

        public float CubeVolume()
        {
            return num1 * num1 * num1;
        }

        public float BoxSurface()
        {
            return 2 * ((num1 * num2) + (num1 * num3) + (num2 * num3));
        }

        public float BoxVolume()
        {
            return num1 * num2 * num3;
        }

        public float PrismSurface()
        {
            return (2 * num1) + (num2 + num3);
        }

        public float PrismVolume()
        {
            return num1 * num2;
        }

        public float PyramidSurface()
        {
            return num1 + (num2 * 3);
        }

        public float PyramidVolume()
        {
            return (num1 * num2) / 3;
        }

        public float TubeSurface()
        {
            return (float)Math.Round((2 * Math.PI * num1 * (num2 * num1)), 2);
        }

        public float TubeVolume()
        {
            return (float)Math.Round((Math.PI * Math.Pow(num1, 2) * num2), 2);
        }

        public float ConeSurface()
        {
            return (float)Math.Round((Math.PI * num1 * (num1 + (Math.Sqrt(Math.Pow(num1, 2) + Math.Pow(num2, 2))))), 2);
        }

        public float ConeVolume()
        {
            return (float)Math.Round((Math.PI * Math.Pow(num1, 2) * num2) / 3, 2);
        }

        public float SphereSurface()
        {
            return (float)Math.Round((4 * Math.PI * Math.Pow(num1, 2)), 2);
        }

        public float SphereVolume()
        {
            return (float)Math.Round(((4 / 3) * Math.PI * Math.Pow(num1, 3)), 2);
        }
    }
}