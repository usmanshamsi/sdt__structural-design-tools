using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ush_lib;


namespace drec2
{
    class Program
        //  program for design of rectangular section for shear
    {
        static void Main(string[] args)
        {
            string syntax = @"drec2 b d f'c fy Vu phi
    where:
    b   = width of rectangular section (inch)
    d   = effective depth of rectangular section (inch)
    f'c = cylinderical compressive strength of concrete (psi)
    fy  = yield strength of reinforcing steel (psi)
    Vu  = Factored shear force (kip)
    phi = Strength reduction factor for shear and torsion";

            const int nArguments = 6;

            Print.Title("DREC2: DESIGN OF RECTANGULAR RCC SECTIONS FOR SHEAR");

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
            // define variables
            // units: lb-inch
            double b, d, fc, fy, vu, phi;
            b = Convert.ToDouble(args[0]);
            d = Convert.ToDouble(args[1]);
            fc = Convert.ToDouble(args[2]);
            fy = Convert.ToDouble(args[3]);
            vu = Convert.ToDouble(args[4]) * 1000.0; // converted to lb from kips in input syntax
            phi = Convert.ToDouble(args[5]);
            // av = Convert.ToDouble(args[5]);

            // print inputs
            Console.WriteLine("I N P U T S");
            Console.WriteLine("-----------");
            Console.WriteLine("Width of rectangular section, b = {0} inch", b);
            Console.WriteLine("Effective depth of rectangular section, d = {0} inch", d);
            Console.WriteLine("Specified Strength of Concrete, f'c = {0} psi", fc);
            Console.WriteLine("Yield Strength of reinforcement, fy = {0} psi", fy);
            Console.WriteLine("Design Shear Force, Vu = {0} kip", vu /1000.0);
            Console.WriteLine("Strength reduction factor, phi_shear = {0}", phi);
            // Console.WriteLine("Area of shear reinforcement (sum of all legs), Av = {0} sq.inch ", av);
            Console.WriteLine();


            double av_over_s_1, av_over_s_2, av_over_s_min, av_over_s; 
            double rootfc = Math.Sqrt(fc);

            av_over_s_1 = 50 * b / fy;
            av_over_s_2 = 0.75 * rootfc * b / fy;
            av_over_s_min = Math.Max(av_over_s_1, av_over_s_2);


            // print outputs
            Console.WriteLine("O U T P U T S");
            Console.WriteLine("-------------");
            
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
                Console.WriteLine();
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


        }
    }
}
