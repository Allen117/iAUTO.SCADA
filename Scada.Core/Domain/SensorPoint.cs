namespace Scada.Core.Domain
{
    public class SensorPoint
    {
        /// <summary>Modbus Address</summary>
        public int Address { get; init; }

        /// <summary>最新數值</summary>
        public double? Value { get; set; }

        /// <summary>資料是否有效</summary>
        public bool IsValid { get; set; } = false;

        /// <summary>最後更新時間</summary>
        public DateTime LastUpdate { get; set; }

        /// <summary>資料品質（Good / Bad / Timeout）</summary>
        public SensorQuality Quality { get; set; } = SensorQuality.Unknown;

        /// <summary>最近一次錯誤訊息</summary>
        public string? ErrorMessage { get; set; }
    }

    public enum SensorQuality
    {
        Unknown,
        Good,
        Bad,
        Timeout
    }
}
