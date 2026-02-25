use anyhow::anyhow;
use regex::Regex;
use reqwest::Client;
use reqwest_cookie_store::{CookieStore, CookieStoreMutex};
use std::collections::HashMap;
use std::fs;
use std::path::Path;
use std::sync::Arc;
use tokio::io::AsyncWriteExt;

use crate::models::{CookieModel, HHDVu, LTSuat, NBan, ReadXmlDataResult, TTKhac, TToan};

pub struct BachHoaXanhService {
    client: Client,
    api_url: String,
    cookie_store: Arc<CookieStoreMutex>,
}

impl BachHoaXanhService {
    pub fn new() -> Self {
        let cookie_store = Arc::new(CookieStoreMutex::new(CookieStore::default()));
        let client = Client::builder()
            .cookie_provider(cookie_store.clone())
            .build()
            .unwrap();

        Self {
            client,
            api_url: "https://hddt.bachhoaxanh.com".to_string(),
            cookie_store,
        }
    }

    pub async fn get_captcha_and_asp_session(
        &self,
        folder: String,
    ) -> anyhow::Result<Option<CookieModel>> {
        let (sv_id, prefix) = self.get_prefix_and_svid().await?;
        if sv_id.is_none() || prefix.is_none() {
            return Err(anyhow!("Vui lòng đợi và thử lại trong vài giây"));
        }

        let sv_id = sv_id.unwrap();
        let prefix = prefix.unwrap();
        let url = format!("{}/home/getcaptchaimage?prefix={}", self.api_url, prefix);
        let bytes = self
            .client
            .get(&url)
            .header("cookie", format!("SvID={}", sv_id))
            .send()
            .await?
            .bytes()
            .await?;

        let base = Path::new(folder.as_str());
        let captcha_path = base.join(format!("{}.jpeg", uuid::Uuid::new_v4()));
        let mut file = tokio::fs::File::create(&captcha_path).await?;
        file.write_all(&bytes).await?;

        // Lấy cookie ASP.NET_SessionId
        let asp_session = {
            let store = self
                .cookie_store
                .lock()
                .expect("Failed to lock cookie_store");
            // Sao chép cookie ra Vec để tránh lifetime issue
            let cookies: Vec<_> = store.iter_any().map(|c| c.clone()).collect();

            cookies
                .into_iter()
                .find(|c| c.name() == "ASP.NET_SessionId")
                .map(|c| c.value().to_string())
                .unwrap_or_default()
        };

        let cookie: CookieModel = CookieModel {
            sv_id,
            aspnet_session_id: asp_session,
            captcha_path: captcha_path.display().to_string(),
        };
        Ok(Some(cookie))
    }

    pub async fn get_xml_invoice_data(
        &self,
        sv_id: &str,
        asp_session: &str,
        captcha: &str,
        phone: &str,
        invoice_num: &str,
        folder: &str,
    ) -> anyhow::Result<Option<ReadXmlDataResult>> {
        let url = format!("{}/Home/ListInvoice", self.api_url);
        let body = format!(
            "phone={}&invoiceNum={}&captcha={}",
            phone, invoice_num, captcha
        );

        let resp = self
            .client
            .post(&url)
            .header(
                "cookie",
                format!("SvID={}; ASP.NET_SessionId={}", sv_id, asp_session),
            )
            .header("x-requested-with", "XMLHttpRequest")
            .header(
                "content-type",
                "application/x-www-form-urlencoded; charset=UTF-8",
            )
            .body(body)
            .send()
            .await?
            .text()
            .await?;

        // url_xem, url_tai_hdchuyen_doi, url_tai_xml
        let (_, _, url_tai_xml) = Self::get_urls(&resp)?;
        let xml_path = Self::download_xml_file(&self, url_tai_xml.as_str(), folder).await?;

        // let xml_path  = "/home/newtun/.local/share/com.newtun.billinsight/Temps/de306117-9a96-47aa-9352-00a8a0399241.xml".to_string();
        let read_xml_result = Self::parse_xml_data(&self, xml_path.as_str()).await?;

        self.clear_cookies();
        Ok(Some(read_xml_result))
    }

    async fn download_xml_file(&self, xml_url: &str, folder: &str) -> anyhow::Result<String> {
        let bytes = self.client.get(xml_url).send().await?.bytes().await?;

        let base = Path::new(folder);
        let file_path = base.join(format!("{}.xml", uuid::Uuid::new_v4()));

        fs::write(&file_path, &bytes)?;
        Ok(file_path.to_string_lossy().to_string())
    }

    // ⚙️ Tối giản, chỉ đọc dữ liệu XML demo
    async fn parse_xml_data(&self, xml_path: &str) -> anyhow::Result<ReadXmlDataResult> {
        let xml_content = tokio::fs::read_to_string(xml_path).await?;

        // Ta parse thủ công bằng cách đọc từng section
        let doc = roxmltree::Document::parse(xml_content.as_str())?;

        // --- NBan ---
        let nban_node = doc
            .descendants()
            .find(|n| n.tag_name().name() == "NBan")
            .ok_or_else(|| anyhow::anyhow!("Không tìm thấy NBan"))?;
        let ten = nban_node
            .descendants()
            .find(|n| n.tag_name().name() == "Ten")
            .and_then(|n| n.text())
            .unwrap_or("")
            .to_string();
        let mst = nban_node
            .descendants()
            .find(|n| n.tag_name().name() == "MST")
            .and_then(|n| n.text())
            .unwrap_or("")
            .to_string();
        let dchi = nban_node
            .descendants()
            .find(|n| n.tag_name().name() == "DChi")
            .and_then(|n| n.text())
            .unwrap_or("")
            .to_string();
        let sdt = nban_node
            .descendants()
            .find(|n| n.tag_name().name() == "SDThoai")
            .and_then(|n| n.text())
            .unwrap_or("")
            .to_string();

        let tt_khac: Vec<TTKhac> = nban_node
            .descendants()
            .filter(|n| n.tag_name().name() == "TTin")
            .map(|x| TTKhac {
                t_truong: x
                    .descendants()
                    .find(|c| c.tag_name().name() == "TTruong")
                    .and_then(|c| c.text())
                    .unwrap_or("")
                    .to_string(),
                kd_lieu: x
                    .descendants()
                    .find(|c| c.tag_name().name() == "KDLieu")
                    .and_then(|c| c.text())
                    .unwrap_or("")
                    .to_string(),
                d_lieu: x
                    .descendants()
                    .find(|c| c.tag_name().name() == "DLieu")
                    .and_then(|c| c.text())
                    .unwrap_or("")
                    .to_string(),
            })
            .collect();

        let nban = NBan {
            ten,
            mst,
            d_chi: dchi,
            sdt,
            tt_khac,
        };

        // --- HHDVu ---
        let hhdvu_list: Vec<HHDVu> = doc
            .descendants()
            .filter(|n| n.tag_name().name() == "HHDVu")
            .map(|x| {
                let get = |tag: &str| -> String {
                    x.descendants()
                        .find(|n| n.tag_name().name() == tag)
                        .and_then(|n| n.text())
                        .unwrap_or("")
                        .to_string()
                };
                let sl = get("SLuong").parse::<i32>().unwrap_or(0);
                let dg = get("DGia").parse::<f64>().unwrap_or(0.0);
                let tt = get("ThTien").parse::<f64>().unwrap_or(0.0);
                HHDVu::new(
                    &get("MHHDVu"),
                    &get("THHDVu"),
                    &get("DVTinh"),
                    sl,
                    dg,
                    tt,
                    &get("TSuat"),
                )
            })
            .collect();

        // --- TToan ---
        let ttoan_node = doc
            .descendants()
            .find(|n| n.tag_name().name() == "TToan")
            .ok_or_else(|| anyhow::anyhow!("Không tìm thấy TToan"))?;

        let t_httl_t_suat: Vec<LTSuat> = ttoan_node
            .descendants()
            .filter(|n| n.tag_name().name() == "LTSuat")
            .map(|x| LTSuat {
                t_suat: x
                    .descendants()
                    .find(|c| c.tag_name().name() == "TSuat")
                    .and_then(|c| c.text())
                    .unwrap_or("")
                    .to_string(),
                th_tien: x
                    .descendants()
                    .find(|c| c.tag_name().name() == "ThTien")
                    .and_then(|c| c.text())
                    .unwrap_or("0")
                    .parse()
                    .unwrap_or(0.0),
                t_thue: x
                    .descendants()
                    .find(|c| c.tag_name().name() == "TThue")
                    .and_then(|c| c.text())
                    .unwrap_or("0")
                    .parse()
                    .unwrap_or(0.0),
            })
            .collect();

        let ttoan = TToan {
            t_httl_t_suat,
            tg_tc_thue: Self::get_text(&ttoan_node, "TgTCThue")
                .parse()
                .unwrap_or(0.0),
            tg_t_thue: Self::get_text(&ttoan_node, "TgTThue")
                .parse()
                .unwrap_or(0.0),
            tg_tt_tb_so: Self::get_text(&ttoan_node, "TgTTTBSo")
                .parse()
                .unwrap_or(0.0),
            tg_tt_tb_chu: Self::get_text(&ttoan_node, "TgTTTBChu"),
            tt_khac: ttoan_node
                .descendants()
                .filter(|n| n.tag_name().name() == "TTin")
                .map(|x| TTKhac {
                    t_truong: Self::get_text(&x, "TTruong"),
                    kd_lieu: Self::get_text(&x, "KDLieu"),
                    d_lieu: Self::get_text(&x, "DLieu"),
                })
                .collect(),
        };

        Ok(ReadXmlDataResult {
            nban: nban,
            hhdvus: hhdvu_list,
            ttoan: ttoan,
        })
    }

    async fn get_prefix_and_svid(&self) -> anyhow::Result<(Option<String>, Option<String>)> {
        println!("==================================================================================================================================");
        let resp = self.client.get(&self.api_url).send().await?;

        println!("resp {:#?}", resp);

        let cookies = resp
            .cookies()
            .map(|c| (c.name().to_string(), c.value().to_string()))
            .collect::<HashMap<_, _>>();

        println!("cookies {:#?}", cookies);

        let svid = cookies.get("SvID").cloned();

        println!("svid {:#?}", svid);

        let html = resp.text().await?;

        println!("html {:#?}", html);

        let re = Regex::new(r#"<img\s+[^>]*src="/home/getcaptchaimage\?prefix=(\d+)""#)?;
        let prefix = re.captures(&html).map(|c| c[1].to_string());

        println!("prefix {:#?}", prefix);

        Ok((svid, prefix))
    }

    fn get_urls(html: &str) -> anyhow::Result<(String, String, String)> {
        let re =
            Regex::new(r#"<a[^>]*href="([^"]+)"[^>]*>\s*(XEM|Tải HĐ chuyển đổi|TẢI XML)\s*</a>"#)?;
        let mut urls = vec![];
        for cap in re.captures_iter(html) {
            urls.push(cap[1].to_string());
        }

        if urls.len() < 3 {
            anyhow::bail!("Không đủ link trong HTML");
        }

        Ok((urls[0].clone(), urls[1].clone(), urls[2].clone()))
    }

    fn get_text(node: &roxmltree::Node, tag: &str) -> String {
        node.descendants()
            .find(|n| n.tag_name().name() == tag)
            .and_then(|n| n.text())
            .unwrap_or("")
            .to_string()
    }

    // Phương thức clear cookie
    pub fn clear_cookies(&self) {
        let mut store = self.cookie_store.lock().unwrap();
        store.clear();
        println!("Đã xóa toàn bộ cookie!");
    }
}
