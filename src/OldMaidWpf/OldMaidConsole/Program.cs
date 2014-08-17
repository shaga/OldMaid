using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OldMaidModel;

namespace OldMaidConsole
{
    class Program
    {
        private const string ActionFormatMaster = "{0}(*)>{1}";
        private const string ActionFormatPlayer = "{0}>{1}";

        static void Main(string[] args)
        {
            GameMaster master = new GameMaster("Master");
            master.OutputActionEvent += OutputAction;

            var p1 = new Player("Kondo");
            p1.OutputActionEvent += OutputAction;
            master.AddPlayer(p1);

            var p2 = new Player("Ookura");
            p2.OutputActionEvent += OutputAction;
            master.AddPlayer(p2);

            var p3 = new Player("Yamazaki");
            p3.OutputActionEvent += OutputAction;
            master.AddPlayer(p3);

            var p4 = new Player("Nakazawa");
            p4.OutputActionEvent += OutputAction;
            master.AddPlayer(p4);

            var sim = new GameSimulator(master);

            sim.RunSimSync();

            Console.ReadLine();
        }

        private static void OutputAction(IParson parson, string action)
        {
            var fmt = parson is GameMaster ? ActionFormatMaster : ActionFormatPlayer;

            Console.WriteLine(fmt, parson.Name, action);
        }
    }
}
