extern crate google_sheets4 as sheets4;

use std::path::Path;
use rustls::crypto::CryptoProvider;
use sheets4::api::ValueRange;
use sheets4::{Result, Error};
use sheets4::{Sheets, FieldMask, hyper_rustls, hyper_util, yup_oauth2};

const SPREADSHEET_ID: &str = ""; // <-- Thay bằng ID sheet của bạn
const SHEET_NAME: &str = "Sheet2"; // <-- Thay bằng tên sheet nếu khác

#[tokio::main]
async fn main() -> Result<()> {

    CryptoProvider::install_default(rustls::crypto::ring::default_provider())
        .expect("failed to install default crypto provider");

    // let secret: yup_oauth2::ApplicationSecret = Default::default();

    let secret = yup_oauth2::read_application_secret("/home/newtun/Desktop/Secrets/client_secret.json")
        .await
        .expect("client secret could not be read");

    let auth = yup_oauth2::InstalledFlowAuthenticator::builder(
        secret,
        yup_oauth2::InstalledFlowReturnMethod::HTTPRedirect,
    )
        .persist_tokens_to_disk(Path::new("token.json"))
        .build().await.unwrap();

    let client = hyper_util::client::legacy::Client::builder(
        hyper_util::rt::TokioExecutor::new()
    )
        .build(
            hyper_rustls::HttpsConnectorBuilder::new()
                .with_native_roots()
                .unwrap()
                .https_or_http()
                .enable_http1()
                .build()
        );
    let mut hub = Sheets::new(client, auth);

    let mut req = ValueRange::default();

    // You can configure optional parameters by calling the respective setters at will, and
    // execute the final call using `doit()`.
    // Values shown here are possibly random and not representative !
    // let result = hub.spreadsheets().values_append(req, SPREADSHEET_ID, "range")
    //     .value_input_option("amet.")
    //     .response_value_render_option("duo")
    //     .response_date_time_render_option("ipsum")
    //     .insert_data_option("gubergren")
    //     .include_values_in_response(true)
    //     .doit().await;

    // match result {
    //     Err(e) => match e {
    //         // The Error enum provides details about what exactly happened.
    //         // You can also just use its `Debug`, `Display` or `Error` traits
    //         Error::HttpError(_)
    //         |Error::Io(_)
    //         |Error::MissingAPIKey
    //         |Error::MissingToken(_)
    //         |Error::Cancelled
    //         |Error::UploadSizeLimitExceeded(_, _)
    //         |Error::Failure(_)
    //         |Error::BadRequest(_)
    //         |Error::FieldClash(_)
    //         |Error::JsonDecodeError(_, _) => println!("{}", e),
    //     },
    //     Ok(res) => println!("Success: {:?}", res),
    // }

    let result = hub
        .spreadsheets()
        .values_get(SPREADSHEET_ID, "Sheet2!E2:K2")
        .major_dimension("ROWS")
        .doit()
        .await?;

    if let Some(values) = result.1.values {
        for row in values {
            println!("{:?}", row);
        }
    }



    Ok(())
}
