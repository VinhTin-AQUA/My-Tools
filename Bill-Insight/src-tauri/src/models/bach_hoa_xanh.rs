use serde::{Deserialize, Serialize};
use uuid::Uuid;

#[derive(Debug, Clone, Default, Serialize, Deserialize)]
pub struct InvoiceModel {
    pub ma_hd: String,
    pub ma_ct: String,
    pub file_path: String,
    pub captcha_code: String,
    pub captcha_path: String,
}

#[derive(Debug, Clone, Default, Serialize, Deserialize)]
pub struct TTKhac {
    pub t_truong: String,
    pub kd_lieu: String,
    pub d_lieu: String,
}

#[derive(Debug, Clone, Default, Serialize, Deserialize)]
pub struct NBan {
    pub ten: String,
    pub mst: String,
    pub d_chi: String,
    pub sdt: String,
    pub tt_khac: Vec<TTKhac>,
}

impl NBan {
    pub fn mst_fmt(&self) -> String {
        format!("MST: {}", self.mst)
    }
    pub fn dchi_fmt(&self) -> String {
        format!("Địa chỉ: {}", self.d_chi)
    }
    pub fn sdt_fmt(&self) -> String {
        format!("ĐT: {}", self.sdt)
    }
}

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct HHDVu {
    pub id: String,
    pub mhhdvu: String,
    pub thhdvu: String,
    pub dv_tinh: String,
    pub s_luong: i32,
    pub d_gia: f64,
    pub th_tien: f64,
    pub t_suat: String,
    pub th_tien_sau_lai_suat: f64,
    pub cash: String,
}

impl HHDVu {
    pub fn new(
        mhhdvu: &str,
        thhdvu: &str,
        dv_tinh: &str,
        s_luong: i32,
        d_gia: f64,
        th_tien: f64,
        t_suat: &str,
    ) -> Self {
        let ts = parse_percentage(t_suat);
        let th_tien_sau = (th_tien + th_tien * ts).round();
        Self {
            id: Uuid::new_v4().to_string(),
            mhhdvu: mhhdvu.to_string(),
            thhdvu: thhdvu.to_string(),
            dv_tinh: dv_tinh.to_string(),
            s_luong,
            d_gia,
            th_tien,
            t_suat: t_suat.to_string(),
            th_tien_sau_lai_suat: th_tien_sau,
            cash: String::new(),
        }
    }

    pub fn empty() -> Self {
        Self {
            id: Uuid::new_v4().to_string(),
            mhhdvu: String::new(),
            thhdvu: String::new(),
            dv_tinh: String::new(),
            s_luong: 0,
            d_gia: 0.0,
            th_tien: 0.0,
            t_suat: String::new(),
            th_tien_sau_lai_suat: 0.0,
            cash: String::new(),
        }
    }
}

fn parse_percentage(input: &str) -> f64 {
    input.trim_end_matches('%').parse::<f64>().unwrap_or(0.0) / 100.0
}

#[derive(Debug, Clone, Default, Serialize, Deserialize)]
pub struct LTSuat {
    pub t_suat: String,
    pub th_tien: f64,
    pub t_thue: f64,
}

#[derive(Debug, Clone, Default, Serialize, Deserialize)]
pub struct TToan {
    pub t_httl_t_suat: Vec<LTSuat>,
    pub tg_tc_thue: f64,
    pub tg_t_thue: f64,
    pub tg_tt_tb_so: f64,
    pub tg_tt_tb_chu: String,
    pub tt_khac: Vec<TTKhac>,
}

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct CookieModel {
    pub sv_id: String,
    pub aspnet_session_id: String,
    pub captcha_path: String,
}

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct ReadXmlDataResult {
    pub nban: NBan,
    pub hhdvus: Vec<HHDVu>,
    pub ttoan: TToan,
}
