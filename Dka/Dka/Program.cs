using System;
using System.Collections.Generic;

using Dka;

namespace Lab1
{
	class Program
	{
		static void Main(string[] args)
		{
			Automat A = new Automat("cost = ((price + 2E-4 + 1)  *0.98 * x + 2) * qq * ww + ee + rr", Actions.Run);
			var result = A.Calculations();

			if (!result.status)
			{
				Console.WriteLine("Error in position");
				Console.WriteLine(result.position);
			}

			Console.ReadKey();
			return;
		}
	}
}