﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLINT_LIB.Config;
using TSQLLINT_LIB.Parser.Interfaces;
using TSQLLINT_LIB.Rules;
using TSQLLINT_LIB.Rules.Interface;
using TSQLLINT_LIB.Rules.RuleViolations;

namespace TSQLLINT_LIB.Parser
{
    public class SqlRuleVisitor : IRuleVisitor
    {
        public List<RuleViolation> Violations { get; set; }
        private readonly TSql120Parser Parser;
        private RuleVisitorBuilder RuleVisitorBuilder;

        public SqlRuleVisitor(ILintConfigReader configReader)
        {
            Parser = new TSql120Parser(true);
            Violations = new List<RuleViolation>();
            RuleVisitorBuilder = new RuleVisitorBuilder(configReader);
        }

        public void VisitRules(string sqlPath, TextReader sqlTextReader)
        {
            IList<ParseError> errors;
            var sqlFragment = GetFragment(sqlTextReader, out errors);

            if (errors.Count > 0)
            {
                Violations.Add(new RuleViolation(sqlPath, "TSQL not syntactically correct"));
                return;
            }

            foreach (var visitor in RuleVisitorBuilder.BuildVisitors(sqlPath, Violations))
            {
                sqlFragment.Accept(visitor);
            }
        }

        public void VisistRule(TextReader txtRdr, TSqlFragmentVisitor visitor)
        {
            IList<ParseError> errors;
            var sqlFragment = GetFragment(txtRdr, out errors);
            sqlFragment.Accept(visitor);
        }

        private TSqlFragment GetFragment(TextReader txtRdr, out IList<ParseError> errors)
        {
            return Parser.Parse(txtRdr, out errors);
        }
    }
}

