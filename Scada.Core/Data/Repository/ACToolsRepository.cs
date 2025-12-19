using System;
using System.Collections.Generic;
using System.Data;
using Scada.Core.Domain;
using Scada.Core.Data;
using Scada.Core.DeviceClass;

namespace Scada.Core.Data.Repository
{
    /// <summary>
    /// 專門負責讀取 ACTOOLS 資料表，並建立 Control Device 物件
    /// </summary>
    public class ACToolsRepository
    {
        private readonly DbReader _reader;

        public ACToolsRepository(DbReader reader)
        {
            _reader = reader;
        }

        /// <summary>
        /// 目前先只讀 TypeID = 156(AI), 158(AO)
        /// 之後再放開成讀全部
        /// </summary>
        public List<clsControlDevice> LoadControlDevices()
        {
            const string sql = "SELECT * FROM ACTOOLS WHERE TypeID IN (156, 158)";

            var list = new List<clsControlDevice>();

            foreach (var r in _reader.Query(sql, x => x))
            {
                int typeId = Convert.ToInt32(r["TypeID"]);

                if (typeId == 156)
                    list.Add(BuildAI(r));
                else if (typeId == 158)
                    list.Add(BuildAO(r));
            }


            return list;
        }

        // ======== AI ========
        private clsAI BuildAI(IDataRecord r)
        {
            var ai = new clsAI();

            FillCommon(ai, r);
            FillAIChannels(ai, r);

            return ai;
        }

        // ======== AO ========
        private clsAO BuildAO(IDataRecord r)
        {
            var ao = new clsAO();

            FillCommon(ao, r);
            FillAOChannels(ao, r);

            return ao;
        }

        /// <summary>
        /// 填入共通欄位
        /// </summary>
        private void FillCommon(clsControlDevice dev, IDataRecord r)
        {
            dev.TypeID = Convert.ToInt32(r["TypeID"]);
            dev.MacId = Convert.ToInt64(r["MACID"]);
            dev.Name = r["TypeName"]?.ToString() ?? "";
        }

        /// <summary>
        /// ACTOOLS 結構：
        /// S1Text = Name1
        /// S2Text = SID1
        /// S3Text = Name2
        /// S4Text = SID2
        /// ...
        /// S39Text = Name20
        /// S40Text = SID20
        /// </summary>
                // ==================== AI Channels ====================
        private void FillAIChannels(clsAI ai, IDataRecord r)
        {
            for (int i = 0; i < 20; i++)
            {
                int nameCol = i * 2 + 2;
                int sidCol = i * 2 + 3;

                string nameField = $"S{nameCol}Text";
                string sidField = $"S{sidCol}Text";

                ai.InputNames[i] =
                    r[nameField] == DBNull.Value ? "" : r[nameField].ToString();

                ai.InputSIDs[i] =
                    r[sidField] == DBNull.Value ? "" : r[sidField].ToString();
            }
        }

        // ==================== AO Channels ====================
        private void FillAOChannels(clsAO ao, IDataRecord r)
        {
            for (int i = 0; i < 20; i++)
            {
                int nameCol = i * 2 + 2;
                int sidCol = i * 2 + 3;

                string nameField = $"S{nameCol}Text";
                string sidField = $"S{sidCol}Text";

                ao.OutputNames[i] =
                    r[nameField] == DBNull.Value ? "" : r[nameField].ToString();

                ao.OutputCIDs[i] =
                    r[sidField] == DBNull.Value ? "" : r[sidField].ToString();
            }
        }
    }
}
