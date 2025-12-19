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

        public string strPicFile;
        public int intModbusIDCount;
        public uint lngMac;
        public string strMAC;

        public int intPosX;
        public int intPosY;

        public byte u8Type;
        public ushort u16SleepTime;

        public byte[] u16PANID = new byte[2];
        public byte u8Channel;

        public byte[] u16ProfileID = new byte[2];

        public byte u8SrcEndNode;
        public byte u8ClusterID;
        public byte u8DstEndNode;

        public int intConnectType;
        public bool blnMonitorEnabledWhenOpen;
        public bool blnMonitorEnabled;

        public string strModbusAddress;
        public string strModbusFormat;

        // ===== Runtime 狀態（Phase 1 先留）=====
        public short intMonitorStatus;
        public int ModbusNoResponseMax;
        public int ModbusNoResponseIndex;
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

        public int PressureDIBitCount, PressureDOBitCount;

        public int intMBSendMode, intReturnLen, intTimeout, intMBPacketID, intPacketCount, intDeviceMode, intReConnectTCPAgain;
        public byte[] bytSendTemp = new byte[256];
        public byte u8EDType;

    }
}
