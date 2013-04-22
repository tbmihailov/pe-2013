using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectionsMandateCalculator
{
    /// <summary>
    /// Model from Candidates.txt
    /// ex:
    /// 1;1;“Кандидат 1 в МИР 1 – Партия 1“
    /// 1;2;“Кандидат 2 в МИР 1 – Партия 1“
    /// 1;1000;“Независим кандидат 1 в МИР 1“
    /// 2;1001;“Независим кандидат 1 в МИР 2“
    /// </summary>
    public class Candidate
    {
        public Candidate(int mirId, int partyId, int seqNum, string name)
        {
            MirId = mirId;
            PartyId = partyId;
            Name = name;
            SeqNum = seqNum;
            PartyType = partyId >= 1000 ? PartyType.InitCommittee : PartyType.Party;
        }

        public int MirId { get; set; }
        public int PartyId { get; set; }
        public int SeqNum { get; set; }
        public string Name { get; set; }

        public PartyType PartyType { get; set; }

    }

    /// <summary>
    /// MIR class from MIRs.txt
    ///1;“МИР 1“;10
    ///2;“МИР 2“;5
    ///3;“Чужбина“;0
    /// </summary>
    public class Mir
    {
        public Mir(int id, string name, int mandatesLimit)
        {
            Id = id;
            MandatesLimit = mandatesLimit;
            Name = name;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public int MandatesLimit { get; set; }
    }


    /// <summary>
    /// Party from Parties.txt
    /// 1;“Партия 1“
    /// 2;“Коалиция 1“
    /// 1000;“Инициативен комитет в МИР 1“
    /// 1001;“Инициативен комитет в МИР 2“
    /// </summary>
    public class Party
    {
        public Party(int id, string name)
        {
            Id = id;
            Name = name;
            Type = id >= 1000 ? PartyType.InitCommittee : PartyType.Party;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public PartyType Type { get; set; }
    }

    /// <summary>
    /// PartyType:
    /// Party and Initiative Committee
    /// 
    /// </summary>
    public enum PartyType
    {
        Party = 0,
        InitCommittee = 1
    }

    /// <summary>
    /// Vote class from Voes.txt
    /// </summary>
    public class Vote
    {
        public Vote(int mirId, int partyId, int count)
        {
            MirId = mirId;
            PartyId = partyId;
            Count = count;
        }

        public int MirId { get; set; }
        public int PartyId { get; set; }
        /// <summary>
        /// Valid votes
        /// </summary>
        public int Count { get; set; }
    }

}
