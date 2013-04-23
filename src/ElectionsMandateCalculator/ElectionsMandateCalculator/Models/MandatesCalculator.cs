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
        List<Party> _partiesAll;
        List<Mir> _mirsAll;
        List<Vote> _votesAll;
        List<Mandate> _mandatesGiven;

        public MandatesCalculator(IEnumerable<Mir> mirs, IEnumerable<Party> parties, IEnumerable<Vote> votes)
        {
            _mirsAll = mirs.OrderBy(m => m.Id).ToList();

            _partiesAll = parties.OrderBy(p => p.Id).ToList();

            _votesAll = new List<Vote>(votes);

            int mirsCount = _mirsAll.Count;
            int partiesCount = _partiesAll.Count;

            _givenMandatesTable1 = new int[mirsCount, partiesCount];
            _mandatesGiven = new List<Mandate>();
        }

        //table 1

        int[,] _givenMandatesTable1;//all votes per party per mir

        int[] _mirMandatesAvailable;//available mandates left


        //decimal _mirMandateQuote;//votes/mandates

        //int _allVotesCount;
        //decimal _fourPercentBarrier;


        int _allMandates;

        public void CalculateMandates()
        {
            Party[] _parties = _partiesAll.ToArray();
            Mir[] _mirs = _mirsAll.OrderBy(m => m.Id).ToArray();

            Logger.Info("******Calculating mandates******");
            int mirsCount = _mirs.Count();
            int partiesCountTable1 = _parties.Count();

            Logger.Info(string.Format("Mirs: {0};Parties:{1}", mirsCount, partiesCountTable1));

            bool[] workingPartyFlagsTable1 = new bool[partiesCountTable1];//parties that are in THE GAME
            for (int i = 0; i < partiesCountTable1; i++)
            {
                workingPartyFlagsTable1[i] = true;
            }

            //associate index from array to mit
            Dictionary<int, int> mirIndeces = new Dictionary<int, int>();
            for (int i = 0; i < mirsCount; i++)
            {
                mirIndeces.Add(_mirs[i].Id, i);
            }

            //associate index from array to mir
            Dictionary<int, int> partyIndecesTable1 = new Dictionary<int, int>();
            for (int i = 0; i < partiesCountTable1; i++)
            {
                partyIndecesTable1.Add(_parties[i].Id, i);
            }

            //fill votes table
            int[,] votesTable1 = new int[mirsCount, partiesCountTable1];//all votes per party per mir
            for (int i = 0; i < mirsCount; i++)
            {
                for (int j = 0; j < partiesCountTable1; j++)
                {
                    votesTable1[i, j] = 0;
                }
            }
            //fill votes table with votes
            foreach (var vote in _votesAll)
            {
                votesTable1[mirIndeces[vote.MirId], partyIndecesTable1[vote.PartyId]] = vote.Count;
            }

            //set initial mir mandates available
            _mirMandatesAvailable = _mirs.Select(mir => mir.MandatesLimit).ToArray();

            //calculate mir and partyVotes votes
            int[] _mirVotesCountTable1 = new int[mirsCount];
            int[] _partyVotesCountTable1 = new int[partiesCountTable1];
            for (int i = 0; i < mirsCount; i++)
            {
                for (int j = 0; j < partiesCountTable1; j++)
                {
                    _mirVotesCountTable1[i] += votesTable1[i, j];
                    _partyVotesCountTable1[j] += votesTable1[i, j];
                }
            }

            //calculate _mirMandateQuote
            decimal[] _mirMandateQuotesTable1 = new decimal[mirsCount];
            for (int i = 0; i < mirsCount; i++)
            {
                if (_mirMandatesAvailable[i] == 0)
                {
                    _mirMandateQuotesTable1[i] = 0;
                }
                else
                {
                    _mirMandateQuotesTable1[i] = (decimal)_mirVotesCountTable1[i] / _mirMandatesAvailable[i];
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
                for (int i = 0; i < partiesCountTable1; i++)
                {
                    if (_parties[i].Type != PartyType.InitCommittee)
                    {
                        continue;
                    }

                    for (int j = 0; j < mirsCount; j++)
                    {
                        int partyMirVotesCnt = votesTable1[j, i];
                        decimal mirMandateQuote = _mirMandateQuotesTable1[j];

                        if (partyMirVotesCnt > mirMandateQuote)
                        {
                            Logger.logger.InfoFormat("IC {0} has {1} votes in MIR {2} {3} {4}", _parties[i].DisplayName, partyMirVotesCnt, _mirs[j].DisplayName, partyMirVotesCnt >= mirMandateQuote ? ">=" : "<", mirMandateQuote);
                            _givenMandatesTable1[j, i] += 1;
                            _mandatesGiven.Add(new Mandate(_mirs[j].Id, _parties[i].Id));
                            _mirMandatesAvailable[j] -= 1;
                            //INITITIVE COMMITTEE can have only 1 mandate in only 1 MIR
                        }
                        workingPartyFlagsTable1[i] = false;
                        //Logger.logger.InfoFormat("{0} excluded from working parties", _parties[i].DisplayName);
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
            int allVotesCount = _mirVotesCountTable1.Sum();
            Logger.logger.Info("**Calculating 4% barrier**");
            //Calculate 4% barrier
            decimal fourPercentBarrier = 0.04M * allVotesCount;
            Logger.logger.InfoFormat("4% barrier = {0} votes", fourPercentBarrier);

            Logger.logger.Info("**Checking PARTIES that pass 4% barrier**");
            for (int i = 0; i < partiesCountTable1; i++)
            {
                if (!workingPartyFlagsTable1[i])
                {
                    continue;
                }
                int partyVotes = _partyVotesCountTable1[i];
                if (partyVotes < fourPercentBarrier)
                {
                    Logger.logger.InfoFormat("Party {0} has {1} votes {2} {3}", _parties[i].DisplayName, partyVotes, partyVotes < fourPercentBarrier ? "<" : ">=", fourPercentBarrier);
                    workingPartyFlagsTable1[i] = false;
                    Logger.logger.InfoFormat("{0} excluded from working parties", _parties[i].DisplayName);
                }
            }

            //GENERATE TABLE2
            int partiesCountInTable2 = workingPartyFlagsTable1.Count(f => f);

            //Votes table
            int[,] votesTable2 = new int[mirsCount, partiesCountInTable2];
            int[,] givenMandatesTable2 = new int[mirsCount, partiesCountInTable2];
            List<Party> partiesListTable2 = new List<Party>();
            int partyIndexTable2 = 0;
            for (int j = 0; j < partiesCountTable1; j++)//TO DO - optimize by j
            {
                if (workingPartyFlagsTable1[j])
                {
                    for (int i = 0; i < mirsCount; i++)
                    {
                        votesTable2[i, partyIndexTable2] = votesTable1[i, j];
                        givenMandatesTable2[i, partyIndexTable2] = _givenMandatesTable1[i, j];
                    }
                    partiesListTable2.Add(_parties[j]);
                    partyIndexTable2++;
                }
            }

            var partiesTable2 = partiesListTable2.ToArray();//indexed array with parties in Table2
            int partiesCountTable2 = partiesListTable2.Count;
            //calculatae new quote - Hare
            int allMandatesAfterStep0 = _mirMandatesAvailable.Sum();
            int allVotesTable2 = 0;//chl.14-chl16
            for (int i = 0; i < mirsCount; i++)
            {
                for (int j = 0; j < partiesCountTable2; j++)
                {
                    allVotesTable2 += votesTable2[i, j];
                }
            }

            decimal globalHaerQuote = (decimal)allVotesTable2 / allMandatesAfterStep0;
            Logger.logger.InfoFormat("GHQ = {0} = {1}/{2}", globalHaerQuote, allVotesTable2, allMandatesAfterStep0);

            //calculate mir and partyVotes votes
            int[] mirVotesCountTable2 = new int[mirsCount];
            int[] partyVotesCountTable2 = new int[partiesCountTable1];
            for (int i = 0; i < mirsCount; i++)
            {
                for (int j = 0; j < partiesCountTable2; j++)
                {
                    mirVotesCountTable2[i] += votesTable2[i, j];
                    partyVotesCountTable2[j] += votesTable2[i, j];
                }
            }

            //mandates that every party should have
            decimal[] partiesGlobalMandatesCoef = new decimal[partiesCountTable2];
            for (int i = 0; i < partiesCountTable2; i++)
            {
                decimal partyVotes = (decimal)partyVotesCountTable2[i];
                partiesGlobalMandatesCoef[i] = decimal.Divide(partyVotes, globalHaerQuote);
            }

            //TO DO calculate initial global mandates

            //TO DO calculate additional global mandates

            //TO DO calculate overall mandates

            // ..

        }

    }
}
