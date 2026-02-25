mod models;
mod services;

pub use models::*;
pub use services::*;

pub fn add(left: u64, right: u64) -> u64 {
    left + right
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn it_works() {
        let result = add(2, 2);
        assert_eq!(result, 4);
    }

    #[tokio::test]
    async fn test_get_your_ip() {
        let service = IpService::new();
        let r = service.get_your_ip().await;

        assert!(r.is_ok());
        let ip = r.unwrap();
        println!("Your IP: {}", ip.ip);
        assert!(!ip.ip.is_empty());
    }

    #[tokio::test]
    async fn test_get_ip_address_info() {
        let service = IpService::new();
        let r = service.get_your_ip_info("1.55.134.225").await;

        assert!(r.is_ok());
        let ip = r.unwrap();
        println!("Your IP: {}", ip.region_name);
        println!("Your IP: {}", ip.isp);
    }
}
