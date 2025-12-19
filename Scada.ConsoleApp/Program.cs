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

// 1. 讀設定檔
var settings = new SettingsReader("Setting/Settings.xml");
string connStr = settings.GetSqlConnectionString();

// 2. 建立 DB Reader
var reader = new DbReader(connStr);

// 3. 讀取 Coordinator
var coRepo = new CoordinatorRepository(reader);

ScadaRuntime.gcolCoordinator.Clear();

foreach (var mobjCoordinator in coRepo.GetCoordinator())
{
    ScadaRuntime.gcolCoordinator[mobjCoordinator.strMAC] = mobjCoordinator;

    Console.WriteLine(
        $"MAC={mobjCoordinator.strMAC}, " +
        $"ConnSettings={mobjCoordinator.strConnSettings}, " +
        $"ConnPort={mobjCoordinator.intConnPort}"
    );
}

ScadaRuntime.gcolEndDeviceNode.Clear();

var defLoader = new CoordinatorDefLoader();
string basePath = AppDomain.CurrentDomain.BaseDirectory;

foreach (var co in ScadaRuntime.gcolCoordinator.Values)
{
    // 讀取文字檔
    switch (co.u8Type)
    {
        case 30:
            defLoader.LoadType30(co, basePath);
            break;
    }
    // 計算Modbus指令
    var loader = new CoordinatorConfigLoader();

    ParsedCoordinatorFile parsed =
        loader.Load(co, basePath);

    // 看結果
    Console.WriteLine(parsed.TypeID);
    Console.WriteLine(parsed.Groups.Count);
    // 4. 測試 ModbusReadPlanner
    var allCommands = new List<ModbusReadCommand>();

    var repo = new EndDeviceRepository();
    tgEndDevice[] devices = repo.CreateFromCoordinator(co, parsed);
    foreach (var device in devices)
    {
        ScadaRuntime.gcolEndDeviceNode.Add(device.lngMac, device);
    }

    foreach (var group in parsed.Groups)
    {
        if (string.IsNullOrWhiteSpace(group.RawNodeDef))
            continue;

        // ⭐ Build 吃 string，這裡就餵 string
        var cmds = ModbusReadPlanner.Build(group.RawNodeDef);
        foreach (var c in cmds)
        {
            Console.WriteLine(c);
        }
        allCommands.AddRange(cmds);
    }

    var executor = new ModbusTcpExecutor();

    foreach (var cmd in allCommands)
    {
        var result = executor.Execute(co, cmd);

        if (result.IsException)
            Console.WriteLine("Read failed");
        else
        {
            Console.WriteLine($"Read OK: {cmd.StartAddress} len={cmd.Length}");
            if (!result.IsException && result.Registers != null)
            {
                for (int i = 0; i < result.Registers.Length; i++)
                {
                    int addr = cmd.StartAddress + i;
                    Console.WriteLine($"Addr {addr} = {result.Registers[i]}");
                }
            }
        }

        if (!result.IsException && result.Registers != null)
        {
            ushort[] regs = result.Registers;
            int start = cmd.StartAddress;

            foreach (var device in devices)
            {
                foreach (var kv in device.AddressProfiles)
                {
                    int address = kv.Key;
                    AddressDecodeProfile profile = kv.Value;

                    // 只處理這次 read 範圍內的 address
                    if (address < start || address >= start + regs.Length)
                        continue;

                    try
                    {
                        double value =
                            ModbusValueDecoder.Decode(regs, start, profile);

                        if (!device.Sensors.TryGetValue(address, out var sensor))
                        {
                            sensor = new SensorPoint { Address = address };
                            device.Sensors[address] = sensor;

                        }

                        sensor.Value = value;
                        sensor.IsValid = true;
                        sensor.Quality = SensorQuality.Good;
                        sensor.LastUpdate = DateTime.Now;

                        Console.WriteLine($"[OK] Addr {address} = {value}");
                    }
                    catch (Exception ex)
                    {
                        if (device.Sensors.TryGetValue(address, out var sensor))
                        {
                            sensor.IsValid = false;
                            sensor.Quality = SensorQuality.Bad;
                            sensor.ErrorMessage = ex.Message;
                        }

                        Console.WriteLine($"[ERR] Addr {address}: {ex.Message}");
                    }
                }
                device.BuildOrderedAddresses();
            }
        }
    }
}

// Enddevice已經取得值
var acRepo = new ACToolsRepository(reader);

List<clsControlDevice> Controldevices =
    acRepo.LoadControlDevices();

var resolver = new SensorResolver();

foreach (var dev in Controldevices)
{
    Console.WriteLine($"Type={dev.TypeID}, MAC={dev.MacId}, Name={dev.Name}");

    // ================= AI =================
    if (dev is clsAI ai)
    {
        for (int i = 0; i < 20; i++)
        {
            if (string.IsNullOrWhiteSpace(ai.InputSIDs[i]))
                continue;

            string sid = ai.InputSIDs[i];

            var sp = resolver.ResolveBySID(sid);

            if (sp != null)
            {
                Console.WriteLine(
                    $" AI[{i + 1}] {ai.InputNames[i]} ({sid}) = {sp.Value}");
            }
            else
            {
                Console.WriteLine(
                    $" AI[{i + 1}] {ai.InputNames[i]} ({sid}) = **無法對應**");
            }
        }
    }

    // ================= AO =================
    if (dev is clsAO ao)
    {
        for (int i = 0; i < 20; i++)
        {
            if (string.IsNullOrWhiteSpace(ao.OutputCIDs[i]))
                continue;

            string cid = ao.OutputCIDs[i];

            var sp = resolver.ResolveBySID(cid);

            if (sp != null)
            {
                Console.WriteLine(
                    $" AO[{i + 1}] {ao.OutputNames[i]} ({cid}) = {sp.Value}");
            }
            else
            {
                Console.WriteLine(
                    $" AO[{i + 1}] {ao.OutputNames[i]} ({cid}) = **無法對應**");
            }
        }
    }
}


Console.WriteLine($"Coordinator count = {ScadaRuntime.gcolCoordinator.Count}");


Console.ReadKey();
