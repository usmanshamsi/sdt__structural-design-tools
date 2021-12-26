using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ush_lib;

namespace Tcr
{
    class Program
    {
        static void Main(string[] args)
        {
            string syntax = @"Tcr b h f'c
    where:
    b = width of beam (inch)
    h = overall depth of beam (inch)
    f'c = cylinderical compressive strength of concrete (psi)";

            const int nArguments = 3;

            Print.Title("TCR: CRACKING TORSION FOR RECTANGULAR SOLID RCC SECTION");

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
            double b, h, fc;
            b = Convert.ToDouble(args[0]);
            h = Convert.ToDouble(args[1]);
            fc = Convert.ToDouble(args[2]);

            // inputs
            Console.WriteLine("I N P U T S");
            Console.WriteLine("-----------");
            Console.WriteLine("Width of beam, b = {0} inch", b);
            Console.WriteLine("Total depth of beam, h = {0} inch", h);
            Console.WriteLine("Specified Strength of Concrete, f'c = {0} psi", fc);
            Console.WriteLine();

            // outputs
            Console.WriteLine("O U T P U T S");
            Console.WriteLine("-------------");

            double tcr = RCC_Functions.Tcr(b, h, fc);

            Console.WriteLine("Tcr = {0:0.##} kip-inch", tcr/1000.0);
            Console.WriteLine("0.85*Tcr = {0:0.##} kip-inch", tcr/1000.0 * 0.85);
            Console.WriteLine("0.75*Tcr = {0:0.##} kip-inch", tcr/1000.0 * 0.75);

        }
    }
}
