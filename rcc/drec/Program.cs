using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ush_lib;

namespace drec
{
    class Program
    {
        static void Main(string[] args)
        {
            string syntax = @"drec b d f'c fy Mu
    where:
    b   = width of rectangular section (inch)
    d   = effective depth of rectangular section (inch)
    f'c = cylinderical compressive strength of concrete (psi)
    fy  = yield strength of reinforcing steel (psi)
    Mu  = design bending moment (kip-ft)";

            const int nArguments = 5;

            Print.Title("DREC: FLEXURAL DESIGN OF SINGLY REINFORCED RECTANGULAR RCC SECTIONS");

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
            double b, d, fc, fy, mu;
            b = Convert.ToDouble(args[0]);
            d = Convert.ToDouble(args[1]);
            fc = Convert.ToDouble(args[2]);
            fy = Convert.ToDouble(args[3]);
            mu = Convert.ToDouble(args[4]);
            mu = mu * 12000.0;      // convert from kip-ft to lb-in

            
            // print inputs
            Console.WriteLine("I N P U T S");
            Console.WriteLine("-----------");
            Console.WriteLine("Width of beam, b = {0} inch", b);
            Console.WriteLine("Effective depth of beam, d = {0} inch", d);
            Console.WriteLine("Specified Strength of Concrete, f'c = {0} psi", fc);
            Console.WriteLine("Yield Strength of reinforcement, fy = {0} psi", fy);
            Console.WriteLine("Design Bending Moment, Mu = {0} kip-ft", mu / 12000.0);
            Console.WriteLine();

            // print outputs
            Console.WriteLine("O U T P U T S");
            Console.WriteLine("-------------");


            double beta1 = RCC_Functions.Beta1(fc);
            Console.WriteLine("beta1 = {0}", beta1);
            Console.WriteLine();

            const double phi = 0.9;
            Console.Write("Strength reduction factor, phi_flexure = {0}", phi);
            Console.WriteLine(" (Reinforcement percentage rho will be limited to rho_max)");
            Console.WriteLine();
            
            double ru = mu / (b * Math.Pow(d, 2));
            double f1 = (2 * ru) / (0.85 * fc * phi);		// phi is assumed to be 0.9 as we will not allow exceeding the limit of rho_max for design
            double f2 = 1 - Math.Sqrt(1 - f1);
            double rho_calc = (0.85 * fc * f2) / fy;

            Console.Write("Calculated reinforcement, As-calc = {0:0.00} sq.inch", rho_calc * b * d);
            Console.WriteLine(" (rho-calc = {0:0.00}%)", rho_calc * 100);

            double rho_min = Math.Max(3 * Math.Sqrt(fc) / fy, 200.0 / fy);

            Console.Write("Minimum reinforcement, As-min = {0:0.00} sq.inch", rho_min * b * d);
            Console.WriteLine(" (rho-min = {0:0.00}%)", rho_min * 100);

            double rho_bal = 0.85 * beta1 * (fc / fy) * (87000 / (87000 + fy));
            Console.Write("Balanced reinforcement, As-bal = {0:0.00} sq.inch", rho_bal * b * d);
            Console.WriteLine(" (rho-balance = {0:0.00}%)", rho_bal * 100);

            const double modulus_of_Steel = 29.0e6;

            double rho_max = ((0.003 + fy / modulus_of_Steel) / 0.008) * rho_bal;

            Console.Write("Maximum reinforcement, As-max = {0:0.00} sq.inch", rho_max * b * d);
            Console.WriteLine(" (rho-max = {0:0.00}%)", rho_max * 100);
            Console.WriteLine();

            // CHECK RHO AGAINST RHO MAX
            if (rho_calc > rho_max)
            {
                Print.Error("Calculated reinforcement is more than maximum allowed (rho-calc > rho-max). Revise section.");

            }
            else
            {

                double rho = Math.Max(rho_calc, Math.Min(rho_min, 4 * rho_calc / 3));
                Console.Write("Reinforcement to be provided, As = {0:0.00} sq.inch", rho * b * d);
                Console.WriteLine(" (rho = {0:0.00}%)", rho * 100);
            }
        }
    }
}
