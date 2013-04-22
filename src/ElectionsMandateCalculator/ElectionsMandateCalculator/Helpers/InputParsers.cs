using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectionsMandateCalculator.Helpers
{
    public class InputParsers
    {
        private const char SEPARATOR = ';';

        #region single row model parsing
        /// <summary>
        /// Parse Mir from Mirs.txt
        /// ex:
        /// 1;“МИР 1“;10
        /// 2;“МИР 2“;5
        /// 3;“Чужбина“;0
        /// </summary>
        /// <param name="recordLine"></param>
        /// <returns></returns>
        public static Mir ParseMirFromString(string recordLine)
        {
            if (string.IsNullOrEmpty(recordLine))
            {
                throw new ArgumentNullException();
            }

            var propValues = recordLine.Split(SEPARATOR);

            var item = new Mir(
                                id: int.Parse(propValues[0]),
                                name: propValues[1],//.Substring(1,propValues[1].Length-2),
                                mandatesLimit: int.Parse(propValues[2])
                            );

            return item;
        }

        /// <summary>
        /// Parse Parties from Parties.txt
        /// ex:
        /// 1;“Партия 1“
        /// 2;“Коалиция 1“
        /// 1000;“Инициативен комитет в МИР 1“
        /// 1001;“Инициативен комитет в МИР 2“
        /// </summary>
        /// <param name="recordLine"></param>
        /// <returns></returns>
        public static Party ParsePartyFromString(string recordLine)
        {
            if (string.IsNullOrEmpty(recordLine))
            {
                throw new ArgumentNullException();
            }

            var propValues = recordLine.Split(SEPARATOR);

            var item = new Party(
                                id: int.Parse(propValues[0]),
                                name: propValues[1]
                           );

            return item;
        }

        /// <summary>
        /// Parsing line from Candidates.txt
        /// ex:
        /// Mir/Party/Sequence num/name
        /// 1;1;1;“Кандидат 1 в МИР 1 – Партия 1“
        /// 1;1;2;“Кандидат 2 в МИР 1 – Партия 1“
        /// </summary>
        /// <param name="recordLine"></param>
        /// <returns></returns>
        public static Candidate ParseCandidateFromString(string recordLine)
        {
            if (string.IsNullOrEmpty(recordLine))
            {
                throw new ArgumentNullException();
            }

            var propValues = recordLine.Split(SEPARATOR);

            var item = new Candidate(
                                mirId: int.Parse(propValues[0]),
                                partyId: int.Parse(propValues[1]),
                                seqNum: int.Parse(propValues[2]),
                                name: propValues[3]
                            );

            return item;
        }

        /// <summary>
        /// Parse Vote from Votes.txt
        /// ex:
        /// 1;1;1000
        /// 1;2;500
        /// 1;1000;600
        /// </summary>
        /// <param name="recordLine"></param>
        /// <returns></returns>
        public static Vote ParseVoteFromString(string recordLine)
        {
            if (string.IsNullOrEmpty(recordLine))
            {
                throw new ArgumentNullException();
            }

            var propValues = recordLine.Split(SEPARATOR);

            var item = new Vote(
                                mirId: int.Parse(propValues[0]),
                                partyId: int.Parse(propValues[1]),
                                count: int.Parse(propValues[2])
                           );

            return item;
        }
        #endregion

        #region parsing files
        public static List<Mir> ParseMirsListFromFile(string fileName)
        {
            var itemsList = new List<Mir>();

            string line;
            // Read the file and display it line by line.
            System.IO.StreamReader file =
               new System.IO.StreamReader(fileName);
            while ((line = file.ReadLine()) != null)
            {
                var item = ParseMirFromString(line);
                itemsList.Add(item);
            }

            file.Close();
            
            return itemsList;
        }

        public static List<Party> ParsePartiesListFromFile(string fileName)
        {
            var itemsList = new List<Party>();

            string line;
            // Read the file and display it line by line.
            System.IO.StreamReader file =
               new System.IO.StreamReader(fileName);
            while ((line = file.ReadLine()) != null)
            {
                var item = ParsePartyFromString(line);
                itemsList.Add(item);
            }

            file.Close();

            return itemsList;
        }

        public static List<Candidate> ParseCandidatesListFromFile(string fileName)
        {
            var itemsList = new List<Candidate>();

            string line;
            // Read the file and display it line by line.
            System.IO.StreamReader file =
               new System.IO.StreamReader(fileName);
            while ((line = file.ReadLine()) != null)
            {
                var item = ParseCandidateFromString(line);
                itemsList.Add(item);
            }

            file.Close();

            return itemsList;
        }

        public static List<Vote> ParseVotesListFromFile(string fileName)
        {
            var itemsList = new List<Vote>();

            string line;
            // Read the file and display it line by line.
            System.IO.StreamReader file =
               new System.IO.StreamReader(fileName);
            while ((line = file.ReadLine()) != null)
            {
                var item = ParseVoteFromString(line);
                itemsList.Add(item);
            }

            file.Close();

            return itemsList;
        }

        public static List<int> ParseLotsListFromFile(string fileName)
        {
            var itemsList = new List<int>();

            string line;
            // Read the file and display it line by line.
            System.IO.StreamReader file =
               new System.IO.StreamReader(fileName);
            while ((line = file.ReadLine()) != null)
            {
                var item = int.Parse(line);
                itemsList.Add(item);
            }

            file.Close();

            return itemsList;
        }
        #endregion
    }
}
