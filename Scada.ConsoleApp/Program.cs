using System;
using System.Reflection.PortableExecutable;
using System.Threading;
using Scada.Core.Binding;
using Scada.Core.Config;
using Scada.Core.Data;
using Scada.Core.Data.Repositories;
using Scada.Core.Data.Repository;
using Scada.Core.DeviceClass;
using Scada.Core.Domain;
using Scada.Core.Logging;
using Scada.Core.Modbus;
using Scada.Core.Modbus.Decode;
using Scada.Core.Runtime;
using Scada.Core.Services;


// ===== 這裡就是 Main() 內容 =====
// 1. 讀設定檔與初始化 Repository
var settings = new SettingsReader("Setting/Settings.xml");
string connStr = settings.GetSqlConnectionString();
var reader = new DbReader(connStr);
var coRepo = new CoordinatorRepository(reader);
var acRepo = new ACToolsRepository(reader);
var defLoader = new CoordinatorDefLoader();
var loader = new CoordinatorConfigLoader();
var repo = new EndDeviceRepository();
var logger = new Logger(new TextFileLogSink("Log/log.txt"));

// 2. 載入基本資料
ScadaRuntime.gcolCoordinator.Clear();
foreach (var mobjCoordinator in coRepo.GetCoordinator())
{
    ScadaRuntime.gcolCoordinator[mobjCoordinator.strMAC] = mobjCoordinator;
}

ScadaRuntime.gcolEndDeviceNode.Clear();
List<clsControlDevice> Controldevices = acRepo.LoadControlDevices();

string basePath = AppDomain.CurrentDomain.BaseDirectory;

// ⭐ 關鍵：建立一個對應表，存儲每個 Coordinator 對應的專屬指令
// 這樣 RuntimeLoop 才知道每一台要發什麼指令
var coordinatorCommands = new Dictionary<string, List<ModbusReadCommand>>();

// 3. 預處理所有 Coordinator (一次性完成初始化)
foreach (var co in ScadaRuntime.gcolCoordinator.Values)
{
    Console.WriteLine($"[Init] 處理 Coordinator: {co.strName} ({co.strMAC})");

    // 載入 .def 文字檔
    if (co.u8Type == 30) defLoader.LoadType30(co, basePath);

    // 解析組態檔案
    ParsedCoordinatorFile parsed = loader.Load(co, basePath);

    // 建立該 Coordinator 底下的所有 EndDevices 並加入全域快取
    tgEndDevice[] devices = repo.CreateFromCoordinator(co, parsed);
    foreach (var device in devices)
    {
        // 確保不會重複加入相同的 lngMac
        ScadaRuntime.gcolEndDeviceNode[device.lngMac] = device;
    }

    // ⭐ 建立這台 Coordinator 的專用指令集
    var myCommands = new List<ModbusReadCommand>();
    foreach (var group in parsed.Groups)
    {
        if (string.IsNullOrWhiteSpace(group.RawNodeDef)) continue;
        var cmds = ModbusReadPlanner.Build(group.RawNodeDef);
        myCommands.AddRange(cmds);
    }

    coordinatorCommands[co.strMAC] = myCommands;
}

var historyRepo = new HistoryRepository(connStr);

// 4. ⭐ 啟動 RuntimeLoop (只啟動一次)
// 注意：我們需要稍微修改 RuntimeLoop 的建構子，接收這個 Dictionary
var runtime = new RuntimeLoop(Controldevices, coordinatorCommands, historyRepo, logger);
runtime.Start();

Console.WriteLine("\n>>> 系統運行中，按任意鍵停止系統 <<<");

// 5. 這裡才是停下的地方
Console.ReadKey();

runtime.Stop();
Console.WriteLine("系統已停止。");




