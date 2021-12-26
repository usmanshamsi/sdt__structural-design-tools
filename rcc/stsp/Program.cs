using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ush_lib;

namespace stsp
{
    class Program
    {
        static void Main(string[] args)
        {
            string syntax = @"stsp Av/s At/s Ast As
    where
    Av/s = Required shear reinforcement (sq.inch /inch)
    At/s = Required torsion reinforcement (sq.inch /inch)
    Ast  = Area of single leg of outer closed hoop (sq.inch)
    As   = Area (Sum) of all remaining vertical shear reinforcement legs (sq.inch)";

            const int nArguments = 4;

            Print.Title("STSP: STIRRUP SPACING CALCULATOR");

            switch (args.Length)
            {
                case nArguments:
                    run_program(args);
                    break;

                default:
                    Console.WriteLine("Syntax Error, use following syntax:");
                    Console.WriteLine(syntax);
                    break;
            }

        }

        static void run_program(string[] args)
        {
            // define variables

            double av_over_s, at_over_s, area_hoop, area_extra_shear_reinf;
            av_over_s = Convert.ToDouble(args[0]);
            at_over_s = Convert.ToDouble(args[1]);
            area_hoop = Convert.ToDouble(args[2]);
            area_extra_shear_reinf = Convert.ToDouble(args[3]);


            // print inputs
            Console.WriteLine("I N P U T S");
            Console.WriteLine("-----------");
            Console.WriteLine("Required shear reinforcement, Av/s = {0} sq.inch /inch", av_over_s);
            Console.WriteLine("Required torsion reinforcement, At/s = {0} sq.inch /inch", at_over_s);
            Console.WriteLine("Area of single leg of outer closed hoop, Ast = {0} sq.inch", area_hoop);
            Console.WriteLine("Area (Sum) of all remaining vertical shear reinforcement legs, As = {0} sq.inch", area_extra_shear_reinf);
            Console.WriteLine();

            // print outputs
            Console.WriteLine("O U T P U T S");
            Console.WriteLine("-------------");



            double total_shear_reinf_area = 2 * area_hoop + area_extra_shear_reinf;
            double scaled_av_over_s = av_over_s * area_hoop / total_shear_reinf_area;
            Console.WriteLine("total shear reinf area = {0:0.######} sq.inch", total_shear_reinf_area);
            Console.WriteLine("scaled shear reinforcement = {0:0.######} sq.inch/inch", scaled_av_over_s);

            double total_a_over_s = at_over_s + scaled_av_over_s;
            Console.WriteLine("Total (Av + At)/s = {0:0.######} sq.inch/inch", total_a_over_s);

            Console.WriteLine();

            double spacing = area_hoop / total_a_over_s;

            Console.WriteLine("Required spacing, S = {0:0.00} inch", spacing);

        }
    }
}
