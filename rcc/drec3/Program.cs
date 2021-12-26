using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ush_lib;

namespace drec3
{
    class Program
    {
        static void Main(string[] args)
        {
            string syntax = @"drec3 b h f'c fy Vu Tu phi
    where:
    b   = width of web of beam (inch)
    h   = overall depth of beam (inch)
    f'c = cylinderical compressive strength of concrete (psi)
    fy  = yield strength of reinforcing steel (psi)
    Vu  = factored shear force (kip)
    Tu  = factored torsion (kip-inch)
    phi = strength reduction factor for shear and torsion";

            const int nArguments = 7;

            Print.Title("DREC3: DESIGN OF RECTANGULAR RCC SECTION FOR SHEAR AND TORSION");

            switch (args.Length)
            {
                case nArguments:
                    run_program(args);
                    break;

                default:
                    Console.WriteLine("Syntax Error, Use following syntax:");
                    Console.WriteLine(syntax);
                    break;
            }


        }

        static void run_program(string[] args)
        {
            #region define variables (units lb-inch)
            double b, h, fc, fy, vu, tu, phi;
            b = Convert.ToDouble(args[0]);
            h = Convert.ToDouble(args[1]);
            fc = Convert.ToDouble(args[2]);
            fy = Convert.ToDouble(args[3]);
            vu = Convert.ToDouble(args[4]) * 1000.0; // converted to lb from kip
            tu = Convert.ToDouble(args[5]) * 1000.0;  // converted to lb-inch from kip-inch
            phi = Convert.ToDouble(args[6]);
            #endregion

            #region Print User Inputs and Program Assumptions


            // print inputs
            Console.WriteLine("I N P U T S");
            Console.WriteLine("-----------");
            Console.WriteLine("Width of (web of) beam, b = {0} inch", b);
            Console.WriteLine("Overall depth of beam, h = {0} inch", h);
            Console.WriteLine("Specified Strength of Concrete, f'c = {0} psi", fc);
            Console.WriteLine("Yield Strength of reinforcement, fy = {0} psi", fy);
            Console.WriteLine("Design Shear Force, Vu = {0} kip", vu / 1000);
            Console.WriteLine("Design Torsion Moment, Tu = {0} kip-inch", tu / 1000);
            Console.WriteLine("Strength reduction factor, phi_shear = {0}", phi);
            Console.WriteLine();

            // print assumptions
            double clear_cover = 1.5; //inch
            double dia_stirrup = 0.5; // inch #4 bar
            double dia_main_bar = 1.0; // inch #8 bar
            Console.WriteLine("A S S U M P T I O N S");
            Console.WriteLine("---------------------");
            Console.WriteLine("Clear cover = {0} inch", clear_cover);
            Console.WriteLine("Stirrup bar = #4 (0.5 inch diameter)");
            Console.WriteLine("Main bar = #8 (1.0 inch diameter)");

            double d; // effective depth
            d = h - clear_cover - dia_stirrup - dia_main_bar / 2.0;
            Console.WriteLine("Effective Depth, d = {0} inch", d);
            Console.WriteLine();
            #endregion

            #region Design for Shear

            Console.WriteLine("S H E A R  D E S I G N");
            Console.WriteLine("----------------------");

            double av_over_s_1, av_over_s_2, av_over_s_min, av_over_s;
            double rootfc = Math.Sqrt(fc);

            av_over_s_1 = 50 * b / fy;
            av_over_s_2 = 0.75 * rootfc * b / fy;
            av_over_s_min = Math.Max(av_over_s_1, av_over_s_2);

            double vc = 2 * rootfc * b * d;
            Console.Write("Shear capacity of provided section, Vc = {0:0.##} kip", vc / 1000);
            Console.WriteLine(" (phi*Vc = {0:0.##} kip)", phi * vc / 1000);
            Console.WriteLine();


            if (vu < phi * vc / 2)
            {
                Print.Info("Vu < (phi * Vc) / 2, No shear reinforcement is required.");

            }
            else if (vu <= phi * vc)
            {
                // vs = 0 and minimum shear reinforcement is required

                av_over_s = av_over_s_min;

                Console.WriteLine("Minimum Shear Reinforcement required, Av/S = {0:0.######} sq.inch / inch", av_over_s);
            }
            else // Vu > phi-Vc
            {
                double vs = (vu - phi * vc) / phi;

                Console.WriteLine("Vs = (Vu - phi*Vc)/phi = {0:0.##} kip", vs / 1000);
                Console.WriteLine();


                if (vs > 4 * vc)
                {
                    Print.Error("Vs > 4 * Vc, Section need to be revised");
                }
                else
                {
                    av_over_s = vs / (fy * d);
                    av_over_s = Math.Max(av_over_s, av_over_s_min);
                    Console.WriteLine("Shear Reinforcement required, Av/S = {0:0.######} sq.inch / inch", av_over_s);
                }
            }

            #endregion

            #region Design for Torsion

            Console.WriteLine();
            Console.WriteLine("T O R S I O N  D E S I G N");
            Console.WriteLine("--------------------------");

            // section parameters
            double x0, y0, x1, y1;
            x0 = b; y0 = h;
            x1 = x0 - 2 * (clear_cover + dia_stirrup / 2);
            y1 = y0 - 2 * (clear_cover + dia_stirrup / 2);
            double Acp = x0 * y0;
            double Pcp = 2 * (x0 + y0);
            double A0h = x1 * y1;
            double A0 = 0.85 * A0h;
            double Ph = 2 * (x1 + y1);
            Console.WriteLine("Section Parameters:");
            Console.Write("x1 = {0} inch", x1);
            Console.WriteLine(", y1 = {0} inch", y1);
            Console.Write("Acp = {0} sq.inch", Acp);
            Console.WriteLine(", Pcp = {0} inch", Pcp);
            Console.Write("A0h = {0} sq.inch", A0h);
            Console.Write(", A0 = {0} sq.inch", A0);
            Console.WriteLine(", Ph = {0} inch", Ph);
            Console.WriteLine();

            double tcr = RCC_Functions.Tcr(b, h, fc);
            Console.Write("Tcr = {0:0.##} kip-inch", tcr / 1000.0);
            Console.WriteLine(" (phi * Tcr = {0:0.##} kip-inch)", phi*tcr/1000.0);
            Console.WriteLine();

            if (tu < phi*tcr / 4.0)
            {
                // torsion is negligible
                Print.Info("No need for torsion reinforcement.");
            }
            else
            {
                // check for combined vu and tu
                // left hand side of comparison equation
                double lhs;
                lhs = Math.Pow(vu / (b * d), 2.0);
                lhs += Math.Pow(tu * Ph / (1.7 * A0h * A0h), 2.0);
                lhs = Math.Sqrt(lhs);
                Console.WriteLine("Left hand side of shear + torsion check = {0:0.00} psi", lhs);

                // right hand side of torsion check
                double rhs;
                rhs = vc / (b * d) + 8 * rootfc;
                rhs *= phi;
                Console.WriteLine("Right hand side of shear + torsion check = {0:0.00} psi", rhs);

                bool section_is_adequate = (lhs <= rhs);

                if (section_is_adequate)
                {
                    Console.WriteLine("Section is adequate for design Vu and Tu");
                    Console.WriteLine();
                    double at_over_s = (tu / phi) / (2 * A0 * fy);
                    double Al_min = Math.Max(5 * rootfc * Acp / fy - at_over_s * Ph, 25 * b / fy);
                    double Al = Math.Max(at_over_s * Ph, Al_min);
                    Console.WriteLine("At/s = {0:0.######} sq.inch / inch", at_over_s);
                    Console.WriteLine("Al = {0:0.##} sq.inch", Al);
                    Console.WriteLine();
                }
                else
                {
                    Print.Error("Section is inadequate for shear + torsion. Revise Section");
                }
            }

            #endregion


        }
    }
}
