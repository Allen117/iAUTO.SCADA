using Scada.Core.Domain;
using Scada.Core.Modbus;
using System.Net.Sockets;
using NModbus;
using System;
using System.Threading;

public sealed class ModbusTcpExecutor : IModbusExecutor
{
    // 🔑 執行緒鎖：確保同一個 Executor 在處理特定 Coordinator 時，讀取與寫入不會碰撞
    private static readonly SemaphoreSlim _networkLock = new SemaphoreSlim(1, 1);

    public ModbusReadResult Execute(tgCoordinator coordinator, ModbusReadCommand command)
    {
        _networkLock.Wait(); // 進入通訊保護區
        try
        {
            using var tcp = new TcpClient();
            tcp.Connect(coordinator.strConnSettings, coordinator.intConnPort);

            var factory = new ModbusFactory();
            var master = factory.CreateMaster(tcp);

            master.Transport.ReadTimeout = 1000;

            ushort start = checked((ushort)ToModbusOffset(command.StartAddress, command.FunctionCode));
            ushort length = checked((ushort)command.Length);
            byte slaveId = (byte)coordinator.intModbusID;

            switch (command.FunctionCode)
            {
                case 3: // Holding Registers
                    var regs3 = master.ReadHoldingRegisters(slaveId, start, length);
                    return new ModbusReadResult { Command = command, Registers = regs3, IsException = false };
                case 4: // Input Registers
                    var regs4 = master.ReadInputRegisters(slaveId, start, length);
                    return new ModbusReadResult { Command = command, Registers = regs4, IsException = false };
                case 1: // Coils
                    var coils1 = master.ReadCoils(slaveId, start, length);
                    return new ModbusReadResult { Command = command, Coils = coils1, IsException = false };
                case 2: // Discrete Inputs
                    var coils2 = master.ReadInputs(slaveId, start, length);
                    return new ModbusReadResult { Command = command, Coils = coils2, IsException = false };
                default:
                    return new ModbusReadResult { Command = command, IsException = true, ExceptionCode = 1 };
            }
        }
        catch
        {
            return new ModbusReadResult { Command = command, IsException = true };
        }
        finally
        {
            _networkLock.Release(); // 釋放通訊保護區
        }
    }

    // ⭐ 新增：寫入單一暫存器方法 (用於 AO 控制)
    public bool ExecuteWrite(tgCoordinator coordinator, int address, double value, byte originalReadFc)
    {
        _networkLock.Wait(); // 保持通訊鎖機制
        try
        {
            using var tcp = new TcpClient();
            tcp.Connect(coordinator.strConnSettings, coordinator.intConnPort);

            var factory = new ModbusFactory();
            var master = factory.CreateMaster(tcp);
            byte slaveId = (byte)coordinator.intModbusID;

            // 💡 根據原始讀取的功能碼，決定寫入的功能碼
            switch (originalReadFc)
            {
                case 1: // 讀取是 Coil -> 寫入用 FC 05 (Write Single Coil)
                case 2: // 讀取是 Discrete Input (通常唯讀，但若要寫入通常對應 Coil)
                    {
                        ushort start = checked((ushort)ToModbusOffset(address, 1));
                        bool coilValue = value > 0; // 非 0 即為 True
                        master.WriteSingleCoil(slaveId, start, coilValue);
                        return true;
                    }

                case 3: // 讀取是 Holding Register -> 寫入用 FC 06 (Write Single Register)
                case 4: // 讀取是 Input Register (通常唯讀，但控制通常轉向 Holding)
                    {
                        ushort start = checked((ushort)ToModbusOffset(address, 3));
                        ushort regValue = checked((ushort)value);
                        master.WriteSingleRegister(slaveId, start, regValue);
                        return true;
                    }

                default:
                    return false;
            }
        }
        catch
        {
            return false;
        }
        finally
        {
            _networkLock.Release();
        }
    }

    public static int ToModbusOffset(int address, byte functionCode)
    {
        return functionCode switch
        {
            3 => address >= 40001 ? address - 40001 : address,
            4 => address >= 30001 ? address - 30001 : address,
            1 => address >= 1 ? address - 1 : address,
            2 => address >= 10001 ? address - 10001 : address,
            _ => throw new ArgumentOutOfRangeException(nameof(functionCode))
        };
    }
}