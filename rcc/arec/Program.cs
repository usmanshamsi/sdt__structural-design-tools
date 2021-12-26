using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ush_lib;

namespace arec
{
    class Program
    {
        static void Main(string[] args)
        {
            string syntax = @"arec b d f'c fy As
    where:
    b   = width of beam (inch)
    d   = effective depth of beam (inch)
    f'c = cylinderical compressive strength of concrete (psi)
    fy  = yield strength of reinforcing steel (psi)
    As  = area of reinforcement (sq.inch)";

            const int nArguments = 5;

            Print.Title("AREC: ANALYSIS OF SINGLY REINFORCED RECTANGULAR CONCRETE SECTIONS");

            switch (args.Length)
            {
                case nArguments:
                    run_program(args);
                    break;

                default:
                    Console.WriteLine("Syntax Error, use following syntax");
                    Console.WriteLine(syntax);
                    break;
            }

        }


        static void run_program(string[] args)
        {
            // define variables
            double b, d, fc, fy, _as;
            b = Convert.ToDouble(args[0]);
            d = Convert.ToDouble(args[1]);
            fc = Convert.ToDouble(args[2]);
            fy = Convert.ToDouble(args[3]);
            _as = Convert.ToDouble(args[4]);

            
            // print inputs
            Console.WriteLine("I N P U T S");
            Console.WriteLine("-----------");
            Console.WriteLine("Width of beam, b = {0} inch", b);
            Console.WriteLine("Effective depth of beam, d = {0} inch", d);
            Console.WriteLine("Specified Strength of Concrete, f'c = {0} psi", fc);
            Console.WriteLine("Yield Strength of reinforcement, fy = {0} psi", fy);
            Console.WriteLine("Area of reinforcement, As = {0} sq.inch", _as);
            Console.WriteLine();

            // print outputs
            Console.WriteLine("O U T P U T S");
            Console.WriteLine("-------------");


            double beta1 = RCC_Functions.Beta1(fc);
            Console.WriteLine("beta1 = {0}", beta1);
            Console.WriteLine();

            double rho = _as / (b * d);
            Console.WriteLine("Provided reinforcement percentage, rho = {0:0.00} %", rho * 100.0);
            //Console.WriteLine();

            double rho_min = Math.Max(3 * Math.Sqrt(fc) / fy, 200.0 / fy);
            Console.Write("Minimum reinforcement, rho-minimum = {0:0.00} % ", rho_min * 100.0);
            Console.WriteLine("({0:0.00} sq.inch)", rho_min * b * d);
            //Console.WriteLine();

            double rho_bal = 0.85 * beta1 * (fc / fy) * (87000 / (87000 + fy));
            Console.Write("Balanced reinforcement, rho-balance = {0:0.00} % ", rho_bal * 100.0);
            Console.WriteLine("({0:0.00} sq.inch)", rho_bal * b * d);
            //Console.WriteLine();

            const double modulus_of_Steel = 29.0e6;

            double rho_max = ((0.003 + fy / modulus_of_Steel) / 0.008) * rho_bal;
            Console.Write("Maximum reinforcement, rho-max = {0:0.00} % ", rho_max * 100.0);
            Console.WriteLine("({0:0.00} sq.inch)", rho_max * b * d);
            Console.WriteLine();

            // CHECK RHO AGAINST RHO MAX AND RHO MIN
            if (rho > rho_max)
            {
                if (rho <= rho_bal)
                {
                    Print.Warning("rho is greater than rho_max, consider reducing.");
                }
                else
                {
                    Print.Error("rho is greater than rho_balance, reduce reinforcement percentage.");
                    return;
                }

            }

            if (rho < rho_min)
            {
                Print.Warning("rho is less than rho_min, consider increasing.");
            }

            // calculate compression block
            double a = (_as * fy) / (0.85 * fc * b);
            Console.WriteLine("Depth of Whitney block, a = {0:0.00} inch", a);

            double c = a / beta1;
            Console.WriteLine("Depth of neutral axis, c = {0:0.00} inch", c);
            Console.WriteLine();

            // calculate strain, phi and moment capacity
            double epsilon_t = ((d - c) / c) * 0.003;
            Console.WriteLine("Net Tensile Strain, epsilon_t = {0:0.00000}", epsilon_t);

            double phi = RCC_Functions.Phi_flexure(epsilon_t);
            Console.WriteLine("Strengh reduction factor, phi_flexure = {0:0.00}", phi);
            Console.WriteLine();

            double moment_capacity = _as * fy * (d - a / 2);
            Console.WriteLine("Nominal Moment Capacity, Mn = {0:0.0} kip-ft", moment_capacity / 12000.0);
            Console.WriteLine("Design Moment Capacity, Mu = phi * Mn = {0:0.0} kip-ft", phi * moment_capacity / 12000.0);

        }
    }
}
