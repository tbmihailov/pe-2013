using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using ElectionsMandateCalculator.Helpers;

namespace ElectionsMandateCalculator.Models
{
    public class MandatesCalculator
    {
        Party[] _parties;
        Mir[] _mirs;
        List<Vote> _votes;

        public MandatesCalculator(IEnumerable<Mir> mirs, IEnumerable<Party> parties, IEnumerable<Vote> votes)
        {
            _mirs = mirs.OrderBy(m => m.Id).ToArray();
            _parties = parties.OrderBy(p => p.Id).ToArray();
            _votes = new List<Vote>(votes);

            int mirsCount = _mirs.Count();
            int partiesCount = _parties.Count();

            _partyGivenMandates = new int[partiesCount];
            _givenMandatesTable = new int[mirsCount, partiesCount];
            _workingPartyFlags = new bool[partiesCount];
            for (int i = 0; i < partiesCount; i++)
            {
                _workingPartyFlags[i] = true;
            }
        }

        //table 1
        int[,] _votesTable;//all votes per party per mir
        int[,] _givenMandatesTable;//all votes per party per mir

        int[] _mirMandatesAvailable;//available mandates left
        int[] _mirVotesAll;//all votes per mir
        decimal[] _mirMandateQuotes;

        int[] _partyVotesAll;//all votes per party
        int[] _partyGivenMandates;//currently given mandates
        bool[] _workingPartyFlags;//parties that are in THE GAME

        //decimal _mirMandateQuote;//votes/mandates

        int _allVotesCount;
        decimal _fourPercentBarrier;


        public void CalculateMandates()
        {
            Logger.Info("******Calculating mandates******");
            int mirsCount = _mirs.Count();
            int partiesCount = _parties.Count();

            Logger.Info(string.Format("Mirs: {0};Parties:{1}", mirsCount, partiesCount));

            //associate index from array to mit
            Dictionary<int, int> mirIndeces = new Dictionary<int, int>();
            for (int i = 0; i < mirsCount; i++)
            {
                mirIndeces.Add(_mirs[i].Id, i);
            }

            //associate index from array to mir
            Dictionary<int, int> partyIndeces = new Dictionary<int, int>();
            for (int i = 0; i < partiesCount; i++)
            {
                partyIndeces.Add(_parties[i].Id, i);
            }

            //fill votes table
            _votesTable = new int[mirsCount, partiesCount];
            for (int i = 0; i < mirsCount; i++)
            {
                for (int j = 0; j < partiesCount; j++)
                {
                    _votesTable[i, j] = 0;
                }
            }
            //fill votes table with votes
            foreach (var vote in _votes)
            {
                _votesTable[mirIndeces[vote.MirId], partyIndeces[vote.PartyId]] = vote.Count;
            }

            //set initial mir mandates available
            _mirMandatesAvailable = _mirs.Select(mir => mir.MandatesLimit).ToArray();

            //calculate mir votes
            _mirVotesAll = new int[mirsCount];
            _partyVotesAll = new int[partiesCount];
            for (int i = 0; i < mirsCount; i++)
            {
                for (int j = 0; j < partiesCount; j++)
                {
                    _mirVotesAll[i] += _votesTable[i, j];
                    _partyVotesAll[j] += _votesTable[i, j];
                }
            }

            //calculate _mirMandateQuote
            _mirMandateQuotes = new decimal[mirsCount];
            for (int i = 0; i < mirsCount; i++)
            {
                if (_mirMandatesAvailable[i] == 0)
                {
                    _mirMandateQuotes[i] = 0;
                }
                else
                {
                    _mirMandateQuotes[i] = (decimal)_mirVotesAll[i] / _mirMandatesAvailable[i];
                }
            }

            //***********************/
            //*********STEP 0********/
            //***********************/
            Logger.logger.Info("***Give INITIATIVE PARTIES Mandates***");
            //check which INITIATIVE candidates pass the mir quote and give them a MANDATE
            int initPartiesCount = _parties.Count(p => p.Type == PartyType.InitCommittee);

            Logger.logger.InfoFormat("Initiative committees:{0}", initPartiesCount);
            if (initPartiesCount > 0)
            {
                for (int i = 0; i < partiesCount; i++)
                {
                    if (_parties[i].Type != PartyType.InitCommittee)
                    {
                        continue;
                    }

                    for (int j = 0; j < mirsCount; j++)
                    {
                        int partyMirVotesCnt = _votesTable[j, i];
                        decimal mirMandateQuote = _mirMandateQuotes[j];
                        Logger.logger.InfoFormat("IC {0} has {1} votes in MIR {2} {3} {4}", _parties[i].DisplayName, partyMirVotesCnt, _mirs[j].DisplayName, partyMirVotesCnt >= mirMandateQuote ? ">=" : "<", mirMandateQuote);
                        if (partyMirVotesCnt > mirMandateQuote)
                        {
                            GiveMandate(j, i, 1);
                            //INITITIVE COMMITTEE can have only 1 mandate in only 1 MIR
                        }
                        else
                        {
                            Logger.logger.InfoFormat("- NO");
                        }
                        _workingPartyFlags[i] = false;
                        Logger.logger.InfoFormat("{0} excluded from working parties", _parties[i].DisplayName);
                    }
                }
            }
            else
            {
                Logger.Info("NO INITIATIVE COMITEEES");
            }

            //***********************/
            //*********STEP 1********/
            //***********************/

            Logger.logger.Info("**Calculating 4% barrier**");
            //Calculate 4% barrier
            _fourPercentBarrier = 0.04M * _allVotesCount;
            Logger.logger.InfoFormat("4% barrier = {0} votes", _fourPercentBarrier);

            Logger.logger.Info("**Checking PARTIES that pass 4% barrier**");
            for (int i = 0; i < partiesCount; i++)
            {
                int partyVotes = _partyVotesAll[i];
                if (partyVotes < _fourPercentBarrier)
                {
                    Logger.logger.InfoFormat("Party {0} has {1} votes {2} {3}", _parties[i].DisplayName, partyVotes, partyVotes < _fourPercentBarrier ? "<" : ">=", _fourPercentBarrier);
                    _workingPartyFlags[i] = false;
                    Logger.logger.InfoFormat("{0} excluded from working parties", _parties[i].DisplayName);
                }
            }

        }

        private void GiveMandate(int mirIndex, int partyIndex, int mandatesCnt = 1)
        {
            _givenMandatesTable[mirIndex, partyIndex] += mandatesCnt;
            _mirMandatesAvailable[mirIndex] -= mandatesCnt;
            Logger.logger.InfoFormat("{0} mandates given to {1} in MIR {2}", mandatesCnt, _parties[partyIndex].DisplayName, _mirs[mirIndex].DisplayName);
        }

        private void TakeMandate(int mirIndex, int partyIndex, int mandatesCnt = 1)
        {
            _givenMandatesTable[mirIndex, partyIndex] -= mandatesCnt;
            _mirMandatesAvailable[mirIndex] += mandatesCnt;
            Logger.logger.InfoFormat("{0} mandates taken from {1} in MIR {2}", mandatesCnt, _parties[partyIndex].DisplayName, _mirs[mirIndex].DisplayName);
        }

    }
}
