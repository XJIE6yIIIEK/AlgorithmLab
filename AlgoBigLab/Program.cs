using System;
using AlgoBigLab;
using Matrices;

class Program {
	public static void Main(string[] args) {
        int vmax = Tester.FindVMax(Tester.Methods.Strassen, 50, 500);
        Console.WriteLine("Vmax = " + vmax.ToString());



        //Trivial - 1500
    }
}