using System;
using System.Threading;
using Scada.Core.Config;
using Scada.Core.Data;
using Scada.Core.Data.Repositories;
using Scada.Core.Logging;
using Scada.Core.Runtime;
using Scada.Core.Services;

namespace Scada.ConsoleApp
{
    public class Class1
    {
        static void Main()
        {
            // ===== 1. 讀設定檔 =====
            var settings = new SettingsReader("Setting/Settings.xml");
            string connStr = settings.GetSqlConnectionString();

            // ===== 2. 建立 DB Reader =====
            var reader = new DbReader(connStr);

            // ===== 3. 讀取 Coordinator =====
            var coRepo = new CoordinatorRepository(reader);

            ScadaRuntime.gcolCoordinator.Clear();

            foreach (var mobjCoordinator in coRepo.GetCoordinator())
            {
                // VB: gcolCoordinator.Add(mobjCoordinator, mstrMAC)
                ScadaRuntime.gcolCoordinator[mobjCoordinator.strMAC] = mobjCoordinator;
                // ===== 只顯示你要看的欄位 =====
                Console.WriteLine(
                    $"MAC={mobjCoordinator.strMAC}, " +
                    $"ConnSettings={mobjCoordinator.strConnSettings}, " +
                    $"ConnPort={mobjCoordinator.intConnPort}"
                );
            }

            Console.WriteLine($"Coordinator count = {ScadaRuntime.gcolCoordinator.Count}");




            Console.WriteLine("Polling stopped.");
            Console.ReadKey();
        }
    }
}
