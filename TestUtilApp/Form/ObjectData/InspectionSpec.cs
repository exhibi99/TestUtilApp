using Newtonsoft.Json;
using System.Collections.Generic;

namespace TestUtilApp.Models
{
    /// <summary>
    /// GMAP 검사 스펙 정보
    /// </summary>
    public class InspectionSpec
    {
        [JsonProperty("INSTRUMENT_IP")]
        public string InstrumentIp { get; set; }

        [JsonProperty("PLANT_CODE")]
        public string PlantCode { get; set; }

        [JsonProperty("PROJECT_NAME")]
        public string ProjectName { get; set; }

        [JsonProperty("PROD_CODE")]
        public string ProdCode { get; set; }

        [JsonProperty("MODEL_CODE")]
        public string ModelCode { get; set; }

        [JsonProperty("LINE_NM")]
        public string LineName { get; set; }

        [JsonProperty("MC_CODE")]
        public string McCode { get; set; }

        [JsonProperty("COMP_MODEL_NAME")]
        public string CompModelName { get; set; }

        [JsonProperty("BUYER_NAME")]
        public string BuyerName { get; set; }

        [JsonProperty("SET_POWER")]
        public string SetPower { get; set; }

        [JsonProperty("INSPECTION_ID")]
        public string InspectionId { get; set; }

        [JsonProperty("INSP_ITEM_ID")]
        public string InspItemId { get; set; }

        [JsonProperty("INSP_DETAIL_ID")]
        public string InspDetailId { get; set; }

        [JsonProperty("LSL")]
        public string Lsl { get; set; }

        [JsonProperty("USL")]
        public string Usl { get; set; }

        [JsonProperty("START_TIME")]
        public string StartTime { get; set; }

        [JsonProperty("END_TIME")]
        public string EndTime { get; set; }

        [JsonProperty("USE_YN")]
        public string UseYn { get; set; }

        [JsonProperty("UPDATE_DATE")]
        public string UpdateDate { get; set; }

        [JsonProperty("NOTE1")]
        public string Note1 { get; set; }

        [JsonProperty("NOTE2")]
        public string Note2 { get; set; }

        [JsonProperty("NOTE3")]
        public string Note3 { get; set; }

        [JsonProperty("REFRIGERANT_TYPE")]
        public string RefrigerantType { get; set; }

        [JsonProperty("REFRIGERANT_AMT")]
        public string RefrigerantAmt { get; set; }

        /// <summary>
        /// NOTE1에서 CAMERA_NM 값 추출
        /// 예: "INSP_YN=Y&CAMERA_NM=UPPER" -> "UPPER"
        /// </summary>
        public string GetCameraName()
        {
            if (string.IsNullOrEmpty(Note1))
                return null;

            var pairs = Note1.Split('&');
            foreach (var pair in pairs)
            {
                var keyValue = pair.Split('=');
                if (keyValue.Length == 2 && keyValue[0].Trim().Equals("CAMERA_NM", System.StringComparison.OrdinalIgnoreCase))
                {
                    return keyValue[1].Trim();
                }
            }
            return null;
        }

        /// <summary>
        /// NOTE1에서 INSP_YN 값 확인
        /// </summary>
        public bool IsInspectionEnabled()
        {
            if (string.IsNullOrEmpty(Note1))
                return false;

            var pairs = Note1.Split('&');
            foreach (var pair in pairs)
            {
                var keyValue = pair.Split('=');
                if (keyValue.Length == 2 && keyValue[0].Trim().Equals("INSP_YN", System.StringComparison.OrdinalIgnoreCase))
                {
                    return keyValue[1].Trim().Equals("Y", System.StringComparison.OrdinalIgnoreCase);
                }
            }
            return false;
        }
    }
}


