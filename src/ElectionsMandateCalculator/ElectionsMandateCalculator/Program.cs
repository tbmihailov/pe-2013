using ElectionsMandateCalculator.Helpers;
using ElectionsMandateCalculator.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectionsMandateCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            string dir = "";

            //MIRS
            string mirsFilePath = Path.Combine(dir, "MIRs.txt");
            var mirs = InputParsers.ParseMirsListFromFile(mirsFilePath);
            Logger.Info(string.Format("Брой МИР:{0}", mirs.Count));

            //parties
            string partiesFilePath = Path.Combine(dir, "Parties.txt");
            var parties = InputParsers.ParsePartiesListFromFile(partiesFilePath);
            Logger.Info(string.Format("Брой партии:{0}", parties.Count));

            //candidates
            string candidatesFilePath = Path.Combine(dir, "Candidates.txt");
            var candidates = InputParsers.ParseCandidatesListFromFile(candidatesFilePath);
            Logger.Info(string.Format("Брой кандидати:{0}", candidates.Count));

            //votes
            string votesFilePath = Path.Combine(dir, "Votes.txt");
            var votes = InputParsers.ParseVotesListFromFile(votesFilePath);
            Logger.Info(string.Format("Брой записи за гласове:{0}", votes.Count));

            //lots
            string lotsFilePath = Path.Combine(dir, "Lot.txt");
            var lots = new List<Lot>();
            if (File.Exists(lotsFilePath))
            {
                lots = InputParsers.ParseLotsListFromFile(lotsFilePath);
                Logger.Info(string.Format("Брой записи за жребии:{0}", lots.Count));
            }
            else
            {
                Logger.Info("Брой записи за жребии: 0");
            }

            var calc = new MandatesCalculator(mirs, parties, votes, lots);
            calc.CalculateMandates();
        }
    }
}
