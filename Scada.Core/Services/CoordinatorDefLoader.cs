using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Scada.Core.Domain;

namespace Scada.Core.Services
{
    public partial class CoordinatorDefLoader
    {
        /// <summary>
        /// UserPLC（Type 30）定義檔讀取
        /// 對應 VB: ReadDefDataOfType30FromFile
        /// </summary>
        public void LoadType30(tgCoordinator co, string basePath)
        {
            int mintIndex;
            string[] mstrText;

            Dictionary<string, string> mcolCreateTable =
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            try
            {
                // ===== 1. 讀主定義檔 =====
                string filePath = Path.Combine(basePath, "ModbusTCP", co.strModbusAddress);

                using (var sr = new StreamReader(filePath, Encoding.Default))
                {
                    while (!sr.EndOfStream)
                    {
                        mstrText = sr.ReadLine().Split('=');
                        if (mstrText.Length == 2)
                        {
                            mcolCreateTable[mstrText[0].Trim().ToUpper()] =
                                mstrText[1].Trim();
                        }
                    }
                }

                // ===== 2. 基本欄位 =====
                co.u8EDType = Convert.ToByte(mcolCreateTable["TYPEID"]);
                co.strLocalIP = mcolCreateTable["TYPENAME"];
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"LoadType30 failed, MAC={co.strMAC}", ex);
            }
        }
    }
}
