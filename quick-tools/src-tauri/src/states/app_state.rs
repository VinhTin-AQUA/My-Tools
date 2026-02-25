use i_love_img_upscale_img_lib::IloveimgUpscaleImgService;
use ip_lib::IpService;
use tokio::sync::Mutex;

pub struct AppState {
    pub iloveimg_upscale_img_service: Mutex<IloveimgUpscaleImgService>,
    pub ip_service: Mutex<IpService>,
}
