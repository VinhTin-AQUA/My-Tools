use crate::{IpAddressInfo, models::IpAddress};

pub struct IpService {}

impl IpService {
    pub fn new() -> Self {
        Self {}
    }

    pub async fn get_your_ip(&self) -> Result<IpAddress, String> {
        let api = "https://api.ipify.org/?format=json";
        let client = reqwest::Client::new();

        let json_data: IpAddress = client
            .get(api)
            .send()
            .await
            .map_err(|e| e.to_string())?
            .json()
            .await
            .map_err(|e| e.to_string())?;



        Ok(json_data)
    }

    pub async fn get_your_ip_info(&self, ip: &str) -> Result<IpAddressInfo, String> {
        let api = format!("http://ip-api.com/json/{}", ip);
        let client = reqwest::Client::new();

        let data: IpAddressInfo = client
            .get(&api)
            .send()
            .await
            .map_err(|e| e.to_string())?
            .json()
            .await
            .map_err(|e| e.to_string())?;

        Ok(data)
    }
}
