using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
		}

		//table 1
		int[,] _votesTable;
		int[] _mirMandates;
		int[] _mirVotes;
		decimal[] _mirMandateQuotes;
		int[] _partyVotes;

		//decimal _mirMandateQuote;//votes/mandates

		int _allVotesCount;
		decimal _fourPercentBarrier;
		bool[] _workingPartyFlags;

		public void CalculateMandates()
		{
			int mirsCount = _mirs.Count();
			int partiesCount = _parties.Count();

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
			_votesTable = new int[_mirs.Count(), _parties.Count()];
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

			//set mir mandates available
			_mirMandates = _mirs.Select(mir => mir.MandatesLimit).ToArray();
			
			//calculate mir votes
			_mirVotes = new int[mirsCount];
			for (int i = 0; i < mirsCount; i++)
			{
				int cnt = 0;
				for (int j = 0; j < partiesCount; j++)
				{
					cnt += _votesTable[i, j];
				}
				_mirVotes[i] = cnt;
			}

			//calculate _mirMandateQuote
			_mirMandateQuotes = new decimal[mirsCount];
			for (int i = 0; i < mirsCount; i++)
			{
				_mirMandateQuotes[i] = (decimal)_mirVotes[i] / _mirMandates[i];
			}

            //check which INITIATIVE candidates pass the mir quote and give them a MANDATE
		}
	}
}
