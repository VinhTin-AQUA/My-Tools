pub fn parse_vietnamese_number(input: &str) -> Option<f32> {
    // 39.200 đ -> 39200
    let cleaned = input
        .replace('.', "") 
        .replace("đ", "") 
        .replace(" ", "");

    cleaned.parse::<f32>().ok()
}
