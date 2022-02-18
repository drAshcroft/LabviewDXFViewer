using System;
using System.Collections.Generic;
using System.Linq;

namespace NamingControl
{
    public class MathClass
    {


        public class FitReturn
        {
            public double[][] Volts { get; set; }

            public double[][] dVdT { get; set; }

            public double[][] TrimmedData { get; set; }
            public double[,] InitalParamters { get; set; }

            public Tuple<double[],double[],double[]> GetItem(int index)
            {
                return new Tuple<double[], double[], double[]>(Volts[index], dVdT[index], TrimmedData[index]);
            }
        }
        public FitReturn CleanIVCurves(double[,] IVcurves, double voltageMinThresh, double voltageMaxThresh, double sampleRate, double dvdtThresh)
        {

            int cc = 0;
            int cc2 = 0;
            var volts = new double[(int)(.4 * IVcurves.GetLength(1))];
            var dvdt = new double[(int)(.4 * IVcurves.GetLength(1))];
            var minV = 0d;
            var maxV = 0d;
            for (int i = 0; i < IVcurves.GetLength(1) - 1; i++)
            {
                var dvdt_ = (IVcurves[0, i + 1] - IVcurves[0, i]) * sampleRate;
                if (dvdt_ > dvdtThresh)
                {
                    if (cc < volts.Length)
                    {
                        volts[cc] = IVcurves[0, i];
                        dvdt[cc] = dvdt_;
                        cc2++;
                    }
                    cc++;
                }
                if (IVcurves[0, i] > maxV)
                    maxV = IVcurves[0, i];
                if (IVcurves[0, i] < minV)
                    minV = IVcurves[0, i];
            }
            if (cc2 < volts.Length)
            {
                var volts2 = new double[cc2];
                var dvdt2 = new double[cc2];
                Array.Copy(volts, volts2, cc2);
                Array.Copy(dvdt, dvdt2, cc2);
                volts = volts2;
                dvdt = dvdt2;
            }

            double[] p = MathNet.Numerics.Fit.Polynomial(volts, dvdt, 3);


            var indexs = new int[cc];


            double minThresh = Math.Max(Math.Abs(maxV), Math.Abs(minV)) * voltageMinThresh;
            double maxPosThresh = maxV * voltageMaxThresh;
            double maxNegThresh = minV * voltageMaxThresh;
            cc = 0;

            for (int i = 0; i < IVcurves.GetLength(1) - 1; i++)
            {
                var dvdt_ = (IVcurves[0, i + 1] - IVcurves[0, i]) * sampleRate;
                if (dvdt_ > dvdtThresh)
                {
                    var vr = IVcurves[0, i];
                    var v = Math.Abs(vr);
                    if (v > minThresh && vr > maxNegThresh && vr < maxPosThresh)
                    {
                        indexs[cc] = i;
                        cc++;
                    }
                }
            }

            var voltsOut = new List<double>[IVcurves.GetLength(0)];
            var dVdtOut = new List<double>[IVcurves.GetLength(0)];
            var currentOut = new List<double>[IVcurves.GetLength(0)];

            for (int i = 0; i < cc; i++)
            {
                double v = IVcurves[0, indexs[i]];

                for (int j = 0; j < IVcurves.GetLength(0); j++)
                {
                    if (voltsOut[j] == null)
                    {
                        voltsOut[j] = new List<double>();
                        dVdtOut[j] = new List<double>();
                        currentOut[j] = new List<double>();
                    }
                    var curr = IVcurves[j, indexs[i]];
                    if (Math.Abs(curr) < 14)
                    {
                        voltsOut[j].Add(v);
                        dVdtOut[j].Add(PolyValue(v, p));
                        currentOut[j].Add(curr);
                    }
                }
            }

            for (int j = 0; j < IVcurves.GetLength(0); j++)
            {
                if (voltsOut[j] == null || voltsOut[j].Count == 0)
                {
                    for (int i = 0; i < IVcurves.GetLength(1); i++)
                    {
                        var curr = IVcurves[j, i];
                        if (Math.Abs(curr) < 14)
                        {
                            voltsOut[j].Add(IVcurves[0,i]);
                            dVdtOut[j].Add(1);
                            currentOut[j].Add(IVcurves[j, i]);
                        }
                    }
                }
            }

            var voltsH = new List<double>[IVcurves.GetLength(0)];
            var currentH = new List<double>[IVcurves.GetLength(0)];

            var voltsL = new List<double>[IVcurves.GetLength(0)];
            var currentL = new List<double>[IVcurves.GetLength(0)];

            for (int j = 0; j < IVcurves.GetLength(0); j++)
            {
                for (int i = 0; i < voltsOut[j].Count; i++)
                {
                    if (voltsOut[j][i] > 0)
                    {
                        if (currentH[j] == null)
                        {
                            currentH[j] = new List<double>();
                            voltsH[j] = new List<double>();
                        }
                            currentH[j].Add(currentOut[j][i]);
                        voltsH[j].Add(voltsOut[j][i]);
                    }

                    if (voltsOut[j][i] < 0)
                    {

                            if (currentL[j] == null)
                            {
                                currentL[j] = new List<double>();
                                voltsL[j] = new List<double>();
                            }

                            currentL[j].Add(currentOut[j][i]);
                            voltsL[j].Add(voltsOut[j][i] - minV);
                    }
                }
            }



            var fitParms = new double[IVcurves.GetLength(0), 5];
            for (int j = 1; j < IVcurves.GetLength(0); j++)
            {
                if (currentH[j] != null && currentH[j].Count>0)
                {
                    var p2 = MathNet.Numerics.Fit.Line(voltsH[j].ToArray(), currentH[j].ToArray());

                    fitParms[j, 0] = p2.Item2; //slope
                    fitParms[j, 1] = p2.Item1; //intercept

                    fitParms[j, 3] = minV;

                    if (voltsL[j]!=null &&voltsL[j].Count > 100)
                    {
                        for (int k = 0; k < currentL[j].Count; k++)
                            currentL[j][k] = currentL[j][k] - p2.Item2;

                        double[] p3 = MathNet.Numerics.Fit.Polynomial(voltsL[j].ToArray(), currentL[j].ToArray(), 6);
                        var A = PolyValue(0, p3);
                        var B = -3 * Math.Pow(Math.Abs(p3[6] * 720 / A), 1d / 6);
                        fitParms[j, 2] = A;
                        fitParms[j, 4] = B;
                    }
                }
                else  
                {
                    if (voltsOut[j] != null && voltsOut[j].Count > 0)
                    {
                        var p2 = MathNet.Numerics.Fit.Line(voltsOut[j].ToArray(), currentOut[j].ToArray());

                        fitParms[j, 0] = p2.Item2; //slope
                        fitParms[j, 1] = p2.Item1; //intercept

                        fitParms[j, 3] = minV;
                    }
                }
            }// double y = x * a[0] + a[1] + a[2] * System.Math.Exp((x-a[3]) * a[4]);

            double[][] vO = voltsOut.Select(x => x.ToArray()).ToArray();

            return new FitReturn { Volts=voltsOut.Select(x => x.ToArray()).ToArray(),
                dVdT=dVdtOut.Select(x => x.ToArray()).ToArray(),
                TrimmedData = currentOut.Select(x => x.ToArray()).ToArray(),
                InitalParamters = fitParms };
        }



        private double PolyValue(double x, double[] a)
        {
            double result = a[0];
            double xr = x;
            for (int i = 1; i < a.Length; i++)
            {
                result += xr * a[i];
                xr *= x;
            }
            return result;
        }


        /// <summary>
        /// Implements Gauss Bell Shape function
        /// </summary>
        //public class IVFunctionFunction : LMAFunction
        //{

        //    /// <summary>
        //    /// Returns Gaussian values
        //    /// </summary>
        //    /// <param name="x">x value</param>
        //    /// <param name="a">parameters</param>
        //    /// <returns></returns>
        //    public override double GetY(double x, double[] a)
        //    {
        //        double y = x * a[0] + a[1] + a[2] * System.Math.Exp((x - a[3]) * a[4]);
        //        return y;
        //    }

        //    /// <summary>
        //    /// Derivative value
        //    /// </summary>
        //    /// <param name="x">x value</param>
        //    /// <param name="a">vector of parameters</param>
        //    /// <param name="parameterIndex"></param>
        //    /// <returns></returns>
        //    public override double GetPartialDerivative(double x, double[] a, int parameterIndex)
        //    {
        //        double result = 0;


        //        switch (parameterIndex)
        //        {
        //            case 0:
        //                {
        //                    result = x / a[0];
        //                    break;
        //                }
        //            case 1:
        //                {
        //                    result = 1;
        //                    break;
        //                }
        //            case 2:
        //                {
        //                    result = System.Math.Exp((x - a[3]) * a[4]);
        //                    break;
        //                }
        //            case 3:
        //                {
        //                    result = -1 * a[2] * a[4] * System.Math.Exp((x - a[3]) * a[4]);
        //                    break;
        //                }
        //            case 4:
        //                {
        //                    result = a[2] * (x - a[3]) * System.Math.Exp((x - a[3]) * a[4]);
        //                    break;
        //                }
        //        }


        //        return result;
        //    }


        //}
    }
}
