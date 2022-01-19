using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lab2.Automata
{
    public class Config
    {
        private string ConfigTxt { get; set; }
        private const string BaseError = "Error while parsing config file: ";

        public Config(string config)
        {
            ConfigTxt = config;
        }

        public List<State> ParseTxtConfig()
        {
            var states = new List<State>();

            if (string.IsNullOrEmpty(ConfigTxt)) return states;

            var parsedValue = new StringBuilder();
            string stateFrom = null;
            string routeSymbol = null;
            string stateTo = null;

            for (int i = 0; i < ConfigTxt.Length; i++)
            {
                if (ConfigTxt[i] == ',')
                {
                    if (string.IsNullOrEmpty(stateFrom))
                    {
                        stateFrom = parsedValue.ToString();
                        parsedValue.Clear();
                        continue;
                    }

                    throw new ArgumentException(BaseError + "empty or invalid state found");
                }

                if (ConfigTxt[i] == '=' && i < ConfigTxt.Length - 1 && ConfigTxt[i + 1] != '=')
                {
                    if (!string.IsNullOrEmpty(stateFrom))
                    {
                        routeSymbol = parsedValue.ToString();
                        parsedValue.Clear();
                        continue;
                    }

                    throw new ArgumentException(BaseError + "empty or invalid state route symbol found");
                }

                if (parsedValue.ToString().Contains(Environment.NewLine))
                {
                    if (!string.IsNullOrEmpty(stateFrom) && !string.IsNullOrEmpty(routeSymbol))
                    {
                        stateTo = parsedValue.ToString().Replace(Environment.NewLine, "");
                        parsedValue.Clear();

                        if (states.All(x => x.Name != stateTo))
                        {
                            states.Add(new State(stateTo));
                        }

                        if (states.All(x => x.Name != stateFrom))
                        {
                            states.Add(new State(stateFrom));
                        }

                        states.Find(x => x.Name == stateFrom)
                            ?.AddNextState(routeSymbol, states.Find(x => x.Name == stateTo));

                        stateTo = "";
                        stateFrom = "";
                        routeSymbol = "";
                    }
                }

                parsedValue.Append(ConfigTxt[i]);
            }

            if (!string.IsNullOrEmpty(stateFrom) && !string.IsNullOrEmpty(routeSymbol))
            {
                stateTo = parsedValue.ToString().Replace(Environment.NewLine, "");
                parsedValue.Clear();

                if (states.All(x => x.Name != stateTo))
                {
                    states.Add(new State(stateTo));
                }

                if (states.All(x => x.Name != stateFrom))
                {
                    states.Add(new State(stateFrom));
                }

                states.Find(x => x.Name == stateFrom)?.AddNextState(routeSymbol, states.Find(x => x.Name == stateTo));

                stateTo = "";
                stateFrom = "";
                routeSymbol = "";
            }


            return states;
        }
    }
}