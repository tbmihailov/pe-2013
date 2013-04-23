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
            string dir = "tests";

            //MIRS
            string mirsFilePath = Path.Combine(dir, "MIRs.txt");
            var mirs = InputParsers.ParseMirsListFromFile(mirsFilePath);
            Logger.Info(string.Format("Mirs:{0}", mirs.Count));

            //parties
            string partiesFilePath = Path.Combine(dir, "Parties.txt");
            var parties = InputParsers.ParsePartiesListFromFile(partiesFilePath);
            Logger.Info(string.Format("Parties:{0}", parties.Count));

            //candidates
            string candidatesFilePath = Path.Combine(dir, "Candidates.txt");
            var candidates = InputParsers.ParseCandidatesListFromFile(candidatesFilePath);
            Logger.Info(string.Format("Candidates:{0}", candidates.Count));

            //votes
            string votesFilePath = Path.Combine(dir, "Votes.txt");
            var votes = InputParsers.ParseVotesListFromFile(votesFilePath);
           Logger.Info(string.Format("Votes:{0}", votes.Count));

            //lots
            string lotsFilePath = Path.Combine(dir, "Lot.txt");
            var lots = InputParsers.ParseLotsListFromFile(lotsFilePath);
            Logger.Info(string.Format("Lots:{0}", lots.Count));

            var calc = new MandatesCalculator(mirs, parties, votes);
            calc.CalculateMandates();
        }
    }
}
