using System;

namespace DeterministicStateMachine
{
    class Program
    {
        static void Main(string[] args)
        {
			var ex = new SyntaxOfAtomat("cost = ((price + 2E-4 + 1) * 0.98 * x + 2) * qq * ww + ee + rr)", LanguageRules.ParsingByPriorities);

            ex.ResultHandler();

			Console.ReadKey();
			return;
		}
    }
}
