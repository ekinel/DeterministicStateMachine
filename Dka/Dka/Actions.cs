using System;
using System.Collections.Generic;
using System.Text;

namespace Dka
{
	static class Actions
	{
		static string stor = "", polstr = "";
		static Stack<char> signs = new Stack<char>();
		static Stack<string> code = new Stack<string>();

		static Dictionary<char, int> prior = new Dictionary<char, int>()
		{
			{')', 0},
			{'(', 1},
			{'=', 2},
			{'+', 3},
			{'*', 4},
		};

		/**
		* Function parses signs and prioritizes
		*/
		public static bool Run(int action, char text)
		{
			if (action == 0)
				stor += text;

			// if the sign
			if (action == 1 || action == 5)
			{
				if (stor.Length != 0)
				{
					code.Push(stor);
					polstr += stor;
					polstr += " ";
				}

				stor = "";

				while (signs.Count > 0 && (action == 5 || (text != '(' && prior[signs.Peek()] >= prior[text])))
				{
					char sign = signs.Pop();
					if (sign == '(')
					{
						if (action == 5) return false;
						break;
					}

					polstr += sign;
					polstr += " ";

					string right = code.Pop();
					string left = code.Pop();
					int num = 1;
					while (right.Contains($"${num}") || left.Contains($"${num}")) num++;
					if (sign == '=') code.Push($"LOAD {right}\nSTORE {left}");
					else if (sign == '+') code.Push($"{right}\nSTORE ${num}\nLOAD {left}\nADD ${num}");
					else code.Push($"{right}\nSTORE ${num}\nLOAD {left}\nMPY ${num}");
				}

				if (text != ')' && action != 5) signs.Push(text);
			}

			if (action == 5)
			{
				Console.WriteLine("Result = ");
				Console.WriteLine(polstr);
				Console.WriteLine("\nNon-optimized code:");

				string s = code.Peek();
				Console.WriteLine(code.Pop());

				List<(string, string)> actions_opt = new List<(string, string)>();
				string b = "", c = "";
				bool flag = true;

				for (int i = 0; i < s.Length; i++)
				{
					if (s[i] == '\n' || s[i] == ' ' || i == s.Length - 1)
					{
						if (flag)
						{
							c = b;
							b = "";
							flag = false;
						}
						else
						{
							flag = true;

							if (i == s.Length - 1)
								b += s[i];

							actions_opt.Add((c, b));
							b = "";
						}
					}
					else b += s[i];
				}


				// optimization
				flag = true;

				while (flag)
				{
					flag = false;

					for (int i = 1; i < actions_opt.Count; i++)
					{
						if (actions_opt[i - 1].Item1 == "LOAD" && actions_opt[i].Item1 == "ADD")
						{
							for (int j = i; j < actions_opt.Count; j++)
								if (actions_opt[j].Item2 == actions_opt[i].Item2)
									break;

							string po = actions_opt[i].Item2, po1 = actions_opt[i - 1].Item2;

							actions_opt.RemoveAt(i - 1);
							actions_opt.RemoveAt(i - 1);

							actions_opt.Insert(i - 1, ("ADD", po1));
							actions_opt.Insert(i - 1, ("LOAD", po));

						}
					}

					for (int i = 1; i < actions_opt.Count; i++)
					{
						if (actions_opt[i - 1].Item1 == "LOAD" && actions_opt[i].Item1 == "MPY")
						{
							for (int j = i; j < actions_opt.Count; j++)
								if (actions_opt[j].Item2 == actions_opt[i].Item2)
									break;

							string po = actions_opt[i].Item2, po1 = actions_opt[i - 1].Item2;

							actions_opt.RemoveAt(i - 1);
							actions_opt.RemoveAt(i - 1);

							actions_opt.Insert(i - 1, ("MPY", po1));
							actions_opt.Insert(i - 1, ("LOAD", po));

						}
					}

					for (int i = 1; i < actions_opt.Count; i++)
					{
						if (actions_opt[i - 1].Item1 == "STORE" && actions_opt[i].Item1 == "LOAD" && (actions_opt[i - 1].Item2 == actions_opt[i].Item2))
						{
							for (int j = i; j < actions_opt.Count; j++)
							{
								if (actions_opt[j].Item2 == actions_opt[i].Item2)
									if (actions_opt[j].Item1 == "STORE")
									{
										actions_opt.RemoveAt(i - 1);
										actions_opt.RemoveAt(i - 1);
									}
									else break;
							}

							actions_opt.RemoveAt(i - 1);
							actions_opt.RemoveAt(i - 1);

							flag = true;
						}
					}

					for (int i = 1; i < actions_opt.Count - 1; i++)
					{
						if (actions_opt[i - 1].Item1 == "LOAD" && actions_opt[i].Item1 == "STORE" && actions_opt[i + 1].Item1 == "LOAD")
						{
							for (int j = i + 1; j < actions_opt.Count - 1; j++)
								if (actions_opt[j].Item2 == actions_opt[i].Item2)
								{
									string p = actions_opt[j].Item1;
									actions_opt.RemoveAt(j);
									actions_opt.Insert(j, (p, actions_opt[i - 1].Item2));
									break;
								}

							actions_opt.RemoveAt(i - 1);
							actions_opt.RemoveAt(i - 1);

							flag = true;
						}
					}
				}

				Console.WriteLine("\nOptimized code:");

				foreach ((string, string) i in actions_opt)
					Console.WriteLine(i);
			}

			return true;
		}
	}

	class Automat
	{
		static public string[] alphabet = { "_abcdfghijklmnopqrstuvwxyzABCDFGHIJKLMNOPQRSTUVWXYZ", "0123456789", "+", "*", "-", "Ee", " ", "(", ")", "=", ".", "\0" };

		public const int ERROR = -1;
		public const int HALT = -2;
		public const int NA = -1;

		static public int[,,] delta =
		{
			/*q0*/ {{1, 0 } ,		 {ERROR, NA },		{ERROR, NA },	 {ERROR, NA },		{ERROR, NA },		{1, 0 },		{0, NA },		{ERROR, NA },	{ERROR, NA },	{ERROR, NA },	{ERROR, NA },	{ERROR, NA }},
			/*q1*/ {{1, 0 },		 {1, 0 },			{ERROR, NA },	 {ERROR, NA },		{ERROR, NA },		{1, 0 },		{2, NA },		{ERROR, NA },	{ERROR, NA },	{3, 1 },		{ERROR, NA },	{ERROR, NA }},
			/*q2*/ {{ERROR, NA },	 {ERROR, NA },		{ERROR, NA },	 {ERROR, NA },		{ERROR, NA },		{ERROR, NA },	{2, NA },		{ERROR, NA },	{ERROR, NA },	{3, 1 },		{ERROR, NA },	{ERROR, NA } },
			/*q3*/ {{4, 0 },		 {6, 0 },			{ERROR, NA },	 {ERROR, NA },		{ERROR, NA },		{4, 0 },		{3, NA },		{3, 1 },		{ERROR, NA },	{ERROR, NA },	{ERROR, NA },	{ERROR, NA }},
			/*q4*/ {{4, 0 },		 {4, 0 },			{3, 1 },		 {3, 1 },			{ERROR, NA },		{4, 0 },		{5, NA },		{ERROR, NA },	{12, 1 },		{ERROR, NA },	{ERROR, NA },	{HALT, 5 }},
			/*q5*/ {{ERROR, NA },	 {ERROR, NA },		{3, 1 },		 {3, 1 },			{ERROR, NA },		{ERROR, NA },	{5, NA },		{ERROR, NA },	{12, 1 },		{ERROR, NA },	{ERROR, NA },	{HALT, 5 } },
			/*q6*/ {{ERROR, NA },	 {6, 0 },			{3, 1 },		 {3, 1 },			{ERROR, NA },		{7, 0 },		{5, NA },		{ERROR, NA },	{12, 1 },		{ERROR, NA },	{10, 0 },		{HALT, 5 } },
			/*q7*/ {{ERROR, NA },	 {9, 0 },			{8, 0 },		 {ERROR, NA },		{8, 0 },			{ERROR, NA },	{ERROR, NA },	{ERROR, NA },	{ERROR, NA },	{ERROR, NA },	{ERROR, NA },	{ERROR, NA }},
			/*q8*/ {{ERROR, NA },	 {9, 0 },			{ERROR, NA },	 {ERROR, NA },		{ERROR, NA },		{ERROR, NA },	{ERROR, NA },	{ERROR, NA },	{ERROR, NA },	{ERROR, NA },	{ERROR, NA },	{ERROR, NA } },
			/*q9*/ {{ERROR, NA },	 {9, 0 },			{3, 1 },		 {3, 1 },			{ERROR, NA },		{ERROR, NA },	{5, NA },		{ERROR, NA },	{12, 1 },		{ERROR, NA },	{ERROR, NA },	{HALT, 5 } },
			/*q10*/ {{ERROR, NA },	 {11, 0 },			{ERROR, NA },	 {ERROR, NA },		{ERROR, NA },		{ERROR, NA },	{ERROR, NA },	{ERROR, NA },	{ERROR, NA },	{ERROR, NA },	{ERROR, NA },	{ERROR, NA } },
			/*q11*/ {{ERROR, NA },	 {11, 0 },			{3, 1 },		 {3, 1 },			{ERROR, NA },		{7, 0 },		{5, NA },		{5, 1 },		{ERROR, NA },	{ERROR, NA },	{ERROR, NA },	{HALT, 5 } },
			/*q12*/ {{ERROR, NA },	 {ERROR, NA },		{3, 1 },		 {3, 1 },			{ERROR, NA },		{ERROR, NA },	{5, NA },		{5, 1 },		{ERROR, NA },	{ERROR, NA },	{ERROR, NA },	{HALT, 5 } },
		};

		string text = "\0";
		Func<int, char, bool> action;

		/**
        * Constructor
        */
		public Automat(string text1, Func<int, char, bool> action1)
		{
			text = text1 + "\0";
			action = action1;
		}

		/**
        * Function checks for belonging to an element of the alphabet
        */
		public int Helper(char text)
		{
			for (int i = 0; i < alphabet.Length; i++)
				if (alphabet[i].Contains(text))
					return i;

			return -1;
		}

		/**
        * Function carries out the work of the automat and returns the validation, position number
        */
		public (bool status, int position) Calculations()
		{
			int index = 0;
			int state = 0;
			int i = 0;

			while (state != HALT && state != ERROR)
			{
				index = Helper(text[i]);

				// if the automat met a symbol not from the intervals
				if (index == -1)
					return (false, i);
				else
				{
					if (delta[state, index, 1] != NA && !action(delta[state, index, 1], text[i]))
						return (false, i);

					// automat receives a new state
					state = delta[state, index, 0];

					if (state == HALT)
						return (true, i);
					else if (state == ERROR)
						return (false, i);
				}
				i++;
			}
			return (false, i);
		}
	}
}
