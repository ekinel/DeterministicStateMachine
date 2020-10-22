using System;
using System.Collections.Generic;
using System.Text;

namespace DeterministicStateMachine
{
    class LanguageRules
    {
		static string _storageString = null, _resultString = null;

		private static Stack<char> _orderOfSigns = new Stack<char>();
		private static Stack<string> _resultCode = new Stack<string>();

		static Dictionary<char, int> _orderOfSignsPriority = new Dictionary<char, int>()
		{
			{')', 0},
			{'(', 1},
			{'=', 2},
			{'+', 3},
			{'*', 4},
		};


		public static bool ParsingByPriorities(int action, char text)
		{
			switch (action)
            {
				case 0:
					_storageString += text;
					break;

				case 1:
				case 5:
					if (_storageString.Length != 0)
					{
						_resultCode.Push(_storageString);
						_resultString += (_storageString + " ");
					}

					_storageString = "";

					while (_orderOfSigns.Count > 0 && (action == 5 || (text != '(' && _orderOfSignsPriority[_orderOfSigns.Peek()] >= _orderOfSignsPriority[text])))
					{
						char sign = _orderOfSigns.Pop();

						if (sign == '(')
						{
							if (action == 5) return false;
							break;
						}

						_resultString += sign;
						_resultString += " ";
						var right = _resultCode.Pop();
						var left = _resultCode.Pop();

						int num = 1;

						while (right.Contains($"${num}") || left.Contains($"${num}")) num++;

						if (sign == '=')
						{
							_resultCode.Push($"LOAD {right}\nSTORE {left}");
						}
						else
						{
							if (sign == '+')
							{
								_resultCode.Push($"{right}\nSTORE ${num}\nLOAD {left}\nADD ${num}");
							}
							else
							{
								_resultCode.Push($"{right}\nSTORE ${num}\nLOAD {left}\nMPY ${num}");
							}
						}
					}

					if (text != ')' && action != 5)
						_orderOfSigns.Push(text);

					if (action == 5)
					{
						var s = _resultCode.Peek();
						//Console.WriteLine(_resultCode.Pop());

						var actions_opt = new List<(string, string)>();
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
					}

				break;

				default:
					break;
            }

			return true;
		}

		~LanguageRules()
        {
        }
	}
}
