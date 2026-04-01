use tokio::sync::Mutex;

use crate::services::{
    iloveimg_upscale_img_service::IloveimgUpscaleImgService, ip_service::IpService,
};

pub struct AppState {
    pub iloveimg_upscale_img_service: Mutex<IloveimgUpscaleImgService>,
    pub ip_service: Mutex<IpService>,
}
