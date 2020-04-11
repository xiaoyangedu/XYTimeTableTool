using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYKernel.OS.Common.Models.Mixed;
using XYKernel.OS.Common.Models.Mixed.AlgoRule;
using XYKernel.OS.Common.Models.Mixed.Rule;

namespace OSKernel.Presentation.Core.DataManager
{
    public class PatternDataManager : IPatternDataManager
    {
        public Dictionary<string, object> Patterns
        {
            get;set;
        }

        public Dictionary<string, CLCase> Cases
        {
            get;set;
        }

        public Dictionary<string, Rule> Rules
        {
            get;set;
        }

        public Dictionary<string, AlgoRule> Algoes
        {
            get;set;
        }

        public PatternDataManager()
        {
            this.Patterns = new Dictionary<string, object>();
            this.Cases = new Dictionary<string, CLCase>();
            this.Rules = new Dictionary<string, Rule>();
            this.Algoes = new Dictionary<string, AlgoRule>();
        }

        public void AddAlgoRule(string localID, AlgoRule model)
        {
            if (Algoes.ContainsKey(localID))
            {
                Algoes[localID] = model;
            }
            else
            {
                Algoes.Add(localID, model);
            }
        }

        public void AddRule(string localID, Rule model)
        {
            if (Rules.ContainsKey(localID))
            {
                Rules[localID] = model;
            }
            else
            {
                Rules.Add(localID, model);
            }
        }

        public void AddCase(string localID, CLCase model)
        {
            if (Cases.ContainsKey(localID))
            {
                Cases[localID] = model;
            }
            else
            {
                Cases.Add(localID, model);
            }
        }

        public void AddPatternParam(string localID, object model)
        {
            if (Patterns.ContainsKey(localID))
            {
                Patterns[localID] = model;
            }
            else
            {
                Patterns.Add(localID, model);
            }
        }

        public Rule GetRule(string localID)
        {
            if (Rules.ContainsKey(localID))
                return Rules[localID];
            else
            {
                var rule = new Rule();
                Rules.Add(localID, rule);

                return rule;
            }
        }

        public AlgoRule GetAlgoRule(string localID)
        {
            if (Algoes.ContainsKey(localID))
                return Algoes[localID];
            else
            {
                var algoRule = new AlgoRule();
                Algoes.Add(localID, algoRule);

                return algoRule;
            }
        }

        public CLCase GetCase(string localID)
        {
            if (Cases.ContainsKey(localID))
            {
                return Cases[localID];
            }
            else
            {
                CLCase newCase = new CLCase();
                Cases.Add(localID, newCase);

                return newCase;
            }
        }

        public object GetPatternParam(string localID)
        {
            if (Patterns.ContainsKey(localID))
            {
                return Patterns[localID];
            }
            else
            {
                return null;
            }
        }

        public void RemoveAll(string localID)
        {
            Cases.Remove(localID);
            Rules.Remove(localID);
            Algoes.Remove(localID);
            Patterns.Remove(localID);
        }
    }
}
