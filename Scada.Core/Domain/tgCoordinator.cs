namespace Scada.Core.Domain
{
    public class tgCoordinator
    {
        // ===== DB 對應欄位 =====
        public ushort ushtMAC;
        public string strName;
        public int intConnPort;
        public string strConnSettings;
        public short intBufferSize;

        public int intModbusIDCount;
        public uint lngMac;
        public string strMAC;
        public byte u8Type;
        public ushort u16SleepTime;

        public int intConnectType;
        public bool blnMonitorEnabledWhenOpen;
        public bool blnMonitorEnabled;

        public string strModbusAddress;
        public string strModbusFormat;

        // ===== Runtime 狀態（Phase 1 先留）=====
        public short intMonitorStatus;
        public bool forceReconnect;
        public bool NeedConnect;

        public Byte intModbusID,  intModbusIIndex;
        public int intModbusPacketMax;

        // ===== 壓力測試系統（EnddeviceInitializer 會用到 PressureNodeSensorCount / intModbusNode）=====
        public int PressureNodeUbound;
        public int[] intModbusNode = new int[500];
        public string strLocalIP;
        public string[] PressureNodeAddressDef, PressureNodeScaleDef, PressureSensorNameDef, PressureSensorUnitDef;
        public string[] PressureSensorRowNumDef, PressureSensorIOTypeDef, PressureSensorSNumberDef;
        public string[] PressureNodeAddressDefType;

        public string[,] PressureNodeAddressNow, PressureNodeScaleNow, PressureSensorNameNow, PressureSensorUnitNow;

        public string[,] PressureNodeAddress;
        public int[] PressureNodeSensorCount; // ✅ EnddeviceInitializer 需要用到
        public float[,] PressureNodeScale, PressureNodeScaleX, PressureNodeScaleC;

        public string[,] PressureNodeScaleCodeType, PressureNodeScaleRatio;

        public int PressureModbusCommandCount;
        public int PressureModbusCommandUbound;

        public int  intReturnLen, intTimeout, intReConnectTCPAgain;
        public byte u8EDType;

    }
}
