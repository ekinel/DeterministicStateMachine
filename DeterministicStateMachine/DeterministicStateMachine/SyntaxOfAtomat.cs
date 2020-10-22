using System;
using System.Collections.Generic;
using System.Text;

namespace DeterministicStateMachine
{
    class SyntaxOfAtomat
    {
        static public string[] alphabet = { "_abcdfghijklmnopqrstuvwxyzABCDFGHIJKLMNOPQRSTUVWXYZ", "0123456789", "+", "*", "-", "Ee", " ", "(", ")", "=", ".", "\0" };

        private const int ERROR = -1;
		private const int HALT = -2;
		private const int NA = -1;

		static public int[,,] StateTable =
{					/*q0*/			/*q1*/			/*q2*/			/*q3*/			/*q4*/			/*q5*/			/*q6*/			/*q7*/			/*q8*/			/*q9*/			/*q10*/			/*q11/
			/*q0*/{{1, 0 },			{ERROR, NA },	{ERROR, NA },	{ERROR, NA },	{ERROR, NA },	{1, 0 },		{0, NA },		{ERROR, NA },	{ERROR, NA },	{ERROR, NA },	{ERROR, NA },	{ERROR, NA }},
			/*q1*/{{1, 0 },			{1, 0 },		{ERROR, NA },	{ERROR, NA },	{ERROR, NA },	{1, 0 },		{2, NA },		{ERROR, NA },	{ERROR, NA },	{3, 1 },		{ERROR, NA },	{ERROR, NA }},
			/*q2*/{{ERROR, NA },	{ERROR, NA },	{ERROR, NA },	{ERROR, NA },	{ERROR, NA },	{ERROR, NA },	{2, NA },		{ERROR, NA },	{ERROR, NA },	{3, 1 },		{ERROR, NA },	{ERROR, NA } },
			/*q3*/{{4, 0 },			{6, 0 },		{ERROR, NA },	{ERROR, NA },	{ERROR, NA },	{4, 0 },		{3, NA },		{3, 1 },		{ERROR, NA },	{ERROR, NA },	{ERROR, NA },	{ERROR, NA }},
			/*q4*/{{4, 0 },			{4, 0 },		{3, 1 },		{3, 1 },		{ERROR, NA },	{4, 0 },		{5, NA },		{ERROR, NA },	{12, 1 },		{ERROR, NA },	{ERROR, NA },	{HALT, 5 }},
			/*q5*/{{ERROR, NA },	{ERROR, NA },	{3, 1 },		{3, 1 },		{ERROR, NA },	{ERROR, NA },	{5, NA },		{ERROR, NA },	{12, 1 },		{ERROR, NA },	{ERROR, NA },	{HALT, 5 } },
			/*q6*/{{ERROR, NA },	{6, 0 },		{3, 1 },		{3, 1 },		{ERROR, NA },	{7, 0 },		{5, NA },		{ERROR, NA },	{12, 1 },		{ERROR, NA },	{10, 0 },		{HALT, 5 } },
			/*q7*/{{ERROR, NA },	{9, 0 },		{8, 0 },		{ERROR, NA },	{8, 0 },		{ERROR, NA },	{ERROR, NA },	{ERROR, NA },	{ERROR, NA },	{ERROR, NA },	{ERROR, NA },	{ERROR, NA }},
			/*q8*/{{ERROR, NA },	{9, 0 },		{ERROR, NA },	{ERROR, NA },	{ERROR, NA },	{ERROR, NA },	{ERROR, NA },	{ERROR, NA },	{ERROR, NA },	{ERROR, NA },	{ERROR, NA },	{ERROR, NA } },
			/*q9*/{{ERROR, NA },	{9, 0 },		{3, 1 },		{3, 1 },		{ERROR, NA },	{ERROR, NA },	{5, NA },		{ERROR, NA },	{12, 1 },		{ERROR, NA },	{ERROR, NA },	{HALT, 5 } },
			/*q10*/{{ERROR, NA },	{11, 0 },		{ERROR, NA },	{ERROR, NA },	{ERROR, NA },	{ERROR, NA },	{ERROR, NA },	{ERROR, NA },	{ERROR, NA },	{ERROR, NA },	{ERROR, NA },	{ERROR, NA } },
			/*q11*/{{ERROR, NA },	{11, 0 },		{3, 1 },		{3, 1 },		{ERROR, NA },	{7, 0 },		{5, NA },		{5, 1 },		{ERROR, NA },	{ERROR, NA },	{ERROR, NA },	{HALT, 5 } },
			/*q12*/{{ERROR, NA },	{ERROR, NA },	{3, 1 },		{3, 1 },		{ERROR, NA },	{ERROR, NA },	{5, NA },		{5, 1 },		{ERROR, NA },	{ERROR, NA },	{ERROR, NA },	{HALT, 5 } },
		};

		public string InputString;
		public Func<int, char, bool> action;

		public SyntaxOfAtomat(string _InputString, Func<int, char, bool> _action)
		{
			action = _action;
			AddingTheLastSymbol(_InputString);
		}

		public SyntaxOfAtomat(Func<int, char, bool> _action)
        { 
			action = _action; 
		}

		public void AddingTheLastSymbol(string _InputString) 
		{
			InputString = _InputString + "\0";
		}

		public int AlphabetVerification(char InputString)
		{
			for (int i = 0; i < alphabet.Length; i++)
            {
				if (alphabet[i].Contains(InputString))
					return i;
			}

			return -1;
		}

		public (bool status, int position) AlphabetSearch()
		{
			int index = 0;
			int state = 0;
			int number = 0;

			while (state != HALT && state != ERROR)
			{
				index = AlphabetVerification(InputString[number]);

				if (index == -1)
					return (false, number);

				if (StateTable[state, index, 1] != NA && !action(StateTable[state, index, 1], InputString[number]))
					return (false, number);

				state = StateTable[state, index, 0];

				if (state == HALT)
					return (true, number);

				if (state == ERROR)
					return (false, number);


				number++;
			}
			return (false, number);
		}

		public void ResultHandler()
		{
			var searchresult = AlphabetSearch();
			if (!searchresult.status)
			{
				Console.WriteLine("Error in position");
				Console.WriteLine(searchresult.position);
			}
            else
            {
				Console.WriteLine("String parsed successfully");
			}
		}

        ~SyntaxOfAtomat()
		{
		}
	}

}
