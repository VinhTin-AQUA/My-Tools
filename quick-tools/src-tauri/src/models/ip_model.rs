use serde::{Deserialize, Serialize};

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct IpAddressInfo {
    pub status: String,  // "success"
    pub country: String, // "Vietnam"

    #[serde(rename = "countryCode")]
    pub country_code: String, // "VN"
    pub region: String, // "SG"

    #[serde(rename = "regionName")]
    pub region_name: String, // "Ho Chi Minh"
    pub city: String,     // "Ho Chi Minh City"
    pub zip: String,      // "700000"
    pub lat: f32,         // 10.822
    pub lon: f32,         // 106.6257
    pub timezone: String, // "Asia/Ho_Chi_Minh"
    pub isp: String,      // "VIETELmetro"
    pub org: String,      // ""

    #[serde(rename = "as")]
    pub r#as: String, // "AS7552 Viettel Group"
    pub query: String, // "115.73.219.185
}

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct IpAddress {
    pub ip: String,
}

