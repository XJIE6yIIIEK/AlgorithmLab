using System;
using AlgoBigLab;
using Matrices;
using IOModule;

class Program {
	public static void Main(string[] args) {
		int maxTime = 2 * 60 * 1000;
		IOModule.IOModule io = new IOModule.IOModule();

		Tester.TestSeries(50, 50, maxTime, 3, io);
	}
}